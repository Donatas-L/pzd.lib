using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using pzd.lib.collection;
using pzd.lib.functional;
using pzd.lib.serialization.rws;
using pzd.lib.typeclasses;

namespace pzd.lib.serialization {
  [PublicAPI] public static class SerializedRW {
    public static readonly ISerializedRW<string> str = new stringRW();
    public static readonly ISerializedRW<int> integer = new intRW();
    public static readonly ISerializedRW<byte> byte_ = new byteRW();
    public static readonly ISerializedRW<byte[]> byteArray = new byteArrayRW();
    public static readonly ISerializedRW<uint> uInteger = new uintRW();
    public static readonly ISerializedRW<ulong> uLong = new ulongRW();
    public static readonly ISerializedRW<ushort> uShort = new ushortRW();
    public static readonly ISerializedRW<bool> boolean = new boolRW();
    public static readonly ISerializedRW<float> flt = new floatRW();
    public static readonly ISerializedRW<long> lng = new longRW();
    public static readonly ISerializedRW<DateTime> 
      dateTime = new DateTimeRW(),
      dateTimeMillisTimestamp = new DateTimeMillisTimestampRW();

    public static readonly ISerializedRW<Uri> uri = lambda(
      uri => str.serialize(uri.ToString()),
      (bytes, startIndex) =>
        str.deserialize(bytes, startIndex).flatMapRight(di => di.flatMapTry(s => new Uri(s)))
    );

    public static readonly ISerializedRW<Guid> guid = new GuidRW();
    
    // RWs for library or user defined types go as static fields of those types. 

    /// <summary>Serialized RW for a type that has no parameters (like <see cref="Unit"/>)</summary>
    public static ISerializedRW<A> unitType<A>() where A : new() =>
      Unit.rw.mapNoFail(_ => new A(), _ => Unit._);

    public static ISerializedRW<A> a<A>(
      ISerializer<A> serializer, IDeserializer<A> deserializer
    ) => new JointRW<A>(serializer, deserializer);

    public static ISerializer<B> mapSerialize<A, B>(
      this ISerializer<A> a, Func<B, A> mapper
    ) => new MappedSerializer<A, B>(a, mapper);

    public static IDeserializer<B> map<A, B>(
      this IDeserializer<A> a, Func<A, Either<string, B>> mapper
    ) => new MappedDeserializer<A, B>(a, mapper);

    public static ISerializedRW<B> mapNoFail<A, B>(
      this ISerializedRW<A> aRW,
      Func<A, B> deserializeConversion,
      Func<B, A> serializeConversion
    ) => aRW.map(
      a => Either<string, B>.Right(deserializeConversion(a)), 
      serializeConversion
    );

    public static ISerializedRW<B> map<A, B>(
      this ISerializedRW<A> aRW,
      Func<A, Either<string, B>> deserializeConversion,
      Func<B, A> serializeConversion
    ) => new MappedRW<A, B>(aRW, serializeConversion, deserializeConversion);
    
    public static ISerializedRW<A> mapError<A>(
      this ISerializedRW<A> aRW,
      Func<string, string> mapper
    ) => new Lambda<A>(
      aRW.serialize,
      (serialized, index) => {
        var either = aRW.deserialize(serialized, index);
        return either.leftValueOut(out var err) ? mapper(err) : either;
      }
    );

    public static ISerializedRW<B> mapTry<A, B>(
      this ISerializedRW<A> aRW,
      Func<A, B> deserializeConversion,
      Func<B, A> serializeConversion
    ) => new MappedRW<A, B>(aRW, serializeConversion, a => {
      try {
        return deserializeConversion(a);
      }
      catch (Exception e) {
        return $"mapping from {typeof(A).FullName} to {typeof(B).FullName} threw {e}";
      }
    });

    public static ISerializedRW<A> lambda<A>(
      Serialize<A> serialize, Deserialize<DeserializeInfo<A>> deserialize
    ) => new Lambda<A>(serialize, deserialize);

    // ReSharper disable TypeParameterCanBeVariant
    public delegate Rope<byte> SerializeAggregate<A, Meta>(A a, Meta meta);
    public delegate Either<string, DeserializeInfo<A>> DeserializeAggregate<A, Meta>(
      byte[] data, int offset, Meta meta, A current
    );
    // ReSharper restore TypeParameterCanBeVariant
    /// <summary>
    /// Serialize value with multiple serializers aggregated together.
    /// </summary>
    /// <param name="metas"></param>
    /// <param name="startingValue"></param>
    /// <param name="serialize"></param>
    /// <param name="deserialize"></param>
    public static ISerializedRW<A> aggregate<A, Meta>(
      IEnumerable<Meta> metas, A startingValue, 
      SerializeAggregate<A, Meta> serialize,
      DeserializeAggregate<A, Meta> deserialize
    ) => lambda(
      a => metas.Aggregate(
        Rope.a(EmptyArray<byte>._),
        (current, meta) => current + serialize(a, meta)
      ),
      (bytes, offset) => metas.Aggregate(
        Either<string, DeserializeInfo<A>>.Right(
          new DeserializeInfo<A>(startingValue, 0)
        ),
        (currentEither, meta) => {
          if (currentEither.rightValueOut(out var currentInfo)) {
            var newEither = deserialize(bytes, offset + currentInfo.bytesRead, meta, currentInfo.value);
            return 
              newEither.rightValueOut(out var readInfo) 
                ? new DeserializeInfo<A>(readInfo.value, currentInfo.bytesRead + readInfo.bytesRead) 
                : newEither;
          }
          else {
            return currentEither;
          }
        }
      )
    );

    public static ISerializedRW<KeyValuePair<A, B>> kv<A, B>(
      this ISerializedRW<A> aRW, ISerializedRW<B> bRW
    ) => and(aRW, bRW, (a, b) => new KeyValuePair<A, B>(a, b), t => t.Key, t => t.Value);

    public static ISerializedRW<B> and<A1, A2, B>(
      this ISerializedRW<A1> a1RW, ISerializedRW<A2> a2RW,
      Func<A1, A2, B> mapper, Func<B, A1> getA1, Func<B, A2> getA2
    ) => new AndRW2<A1, A2, B>(a1RW, a2RW, mapper, getA1, getA2);

    public static ISerializedRW<B> and<A1, A2, A3, B>(
      this ISerializedRW<A1> a1RW, ISerializedRW<A2> a2RW, ISerializedRW<A3> a3RW,
      Func<A1, A2, A3, B> mapper, Func<B, A1> getA1, Func<B, A2> getA2, Func<B, A3> getA3
    ) => new AndRW3<A1, A2, A3, B>(a1RW, a2RW, a3RW, mapper, getA1, getA2, getA3);

    public static ISerializedRW<B> and<A1, A2, A3, A4, B>(
      this ISerializedRW<A1> a1RW, ISerializedRW<A2> a2RW, ISerializedRW<A3> a3RW,
      ISerializedRW<A4> a4RW,
      Func<A1, A2, A3, A4, B> mapper, Func<B, A1> getA1, Func<B, A2> getA2, Func<B, A3> getA3, Func<B, A4> getA4
    ) => new AndRW4<A1, A2, A3, A4, B>(a1RW, a2RW, a3RW, a4RW, mapper, getA1, getA2, getA3, getA4);

    public static ISerializedRW<B> and<A1, A2, A3, A4, A5, B>(
      this ISerializedRW<A1> a1RW, ISerializedRW<A2> a2RW, ISerializedRW<A3> a3RW,
      ISerializedRW<A4> a4RW, ISerializedRW<A5> a5RW,
      Func<A1, A2, A3, A4, A5, B> mapper, Func<B, A1> getA1, Func<B, A2> getA2, Func<B, A3> getA3, 
      Func<B, A4> getA4, Func<B, A5> getA5
    ) => new AndRW5<A1, A2, A3, A4, A5, B>(
      a1RW, a2RW, a3RW, a4RW, a5RW, mapper, getA1, getA2, getA3, getA4, getA5
    );

    public static ISerializedRW<B> and<A1, A2, A3, A4, A5, A6, B>(
      this ISerializedRW<A1> a1RW, ISerializedRW<A2> a2RW, ISerializedRW<A3> a3RW,
      ISerializedRW<A4> a4RW, ISerializedRW<A5> a5RW, ISerializedRW<A6> a6RW,
      Func<A1, A2, A3, A4, A5, A6, B> mapper, Func<B, A1> getA1, Func<B, A2> getA2, Func<B, A3> getA3, 
      Func<B, A4> getA4, Func<B, A5> getA5, Func<B, A6> getA6
    ) => new AndRW6<A1, A2, A3, A4, A5, A6, B>(
      a1RW, a2RW, a3RW, a4RW, a5RW, a6RW, mapper, getA1, getA2, getA3, getA4, getA5, getA6
    );

    public static ISerializedRW<B> and<A1, A2, A3, A4, A5, A6, A7, B>(
      this ISerializedRW<A1> a1RW, ISerializedRW<A2> a2RW, ISerializedRW<A3> a3RW,
      ISerializedRW<A4> a4RW, ISerializedRW<A5> a5RW, ISerializedRW<A6> a6RW, ISerializedRW<A7> a7RW,
      Func<A1, A2, A3, A4, A5, A6, A7, B> mapper, Func<B, A1> getA1, Func<B, A2> getA2, Func<B, A3> getA3, 
      Func<B, A4> getA4, Func<B, A5> getA5, Func<B, A6> getA6, Func<B, A7> getA7
    ) => new AndRW7<A1, A2, A3, A4, A5, A6, A7, B>(
      a1RW, a2RW, a3RW, a4RW, a5RW, a6RW, a7RW, mapper, getA1, getA2, getA3, getA4, getA5, getA6, getA7
    );

    public static ISerializedRW<Option<A>> opt<A>(ISerializedRW<A> rw) =>
      new OptRW<A>(rw);

    public static ISerializedRW<Either<A, B>> either<A, B>(ISerializedRW<A> aRW, ISerializedRW<B> bRW) =>
      new EitherRW<A, B>(aRW, bRW);

    public static ISerializedRW<OneOf<A, B, C>> oneOf<A, B, C>(
      ISerializedRW<A> aRW, ISerializedRW<B> bRW, ISerializedRW<C> cRW
    ) => new OneOfRW<A, B, C>(aRW, bRW, cRW);

    public static ISerializedRW<ImmutableArray<A>> immutableArray<A>(
      ISerializedRW<A> rw
    ) => a(
      collectionSerializer<A, ImmutableArray<A>>(rw), 
      collectionDeserializer(rw, CollectionBuilderKnownSizeFactory<A>.immutableArray)
    );

    public static ISerializedRW<ImmutableList<A>> immutableList<A>(
      ISerializedRW<A> rw
    ) => a(
      collectionSerializer<A, ImmutableList<A>>(rw), 
      collectionDeserializer(rw, CollectionBuilderKnownSizeFactory<A>.immutableList)
    );

    public static ISerializedRW<ImmutableHashSet<A>> immutableHashSet<A>(
      ISerializedRW<A> rw
    ) => a(
      collectionSerializer<A, ImmutableHashSet<A>>(rw), 
      collectionDeserializer(rw, CollectionBuilderKnownSizeFactory<A>.immutableHashSet)
    );

    public static ISerializedRW<ImmutableDictionary<K, V>> immutableDictionary<K, V>(
      ISerializedRW<K> kRw, ISerializedRW<V> vRw
    ) => immutableDictionary(kv(kRw, vRw));

    public static ISerializedRW<ImmutableDictionary<K, V>> immutableDictionary<K, V>(
      ISerializedRW<KeyValuePair<K, V>> rw
    ) => a(
      collectionSerializer<KeyValuePair<K, V>, ImmutableDictionary<K, V>>(rw),
      collectionDeserializer(rw, CollectionBuilderKnownSizeFactoryKV<K, V>.immutableDictionary)
    );

    public static ISerializedRW<Dictionary<K, V>> dictionary<K, V>(
      ISerializedRW<K> kRw, ISerializedRW<V> vRw
    ) => dictionary(kv(kRw, vRw));

    public static ISerializedRW<Dictionary<K, V>> dictionary<K, V>(
      ISerializedRW<KeyValuePair<K, V>> rw
    ) => a(
      collectionSerializer<KeyValuePair<K, V>, Dictionary<K, V>>(rw),
      collectionDeserializer(rw, CollectionBuilderKnownSizeFactoryKV<K, V>.dictionary)
    );

    public static ISerializedRW<A[]> array<A>(
      ISerializedRW<A> rw
    ) => a(
      collectionSerializer<A, A[]>(rw),
      collectionDeserializer(rw, CollectionBuilderKnownSizeFactory<A>.array)
    );

    public static ISerializer<ICollection<A>> collectionSerializer<A>(ISerializer<A> serializer) =>
      collectionSerializer<A, ICollection<A>>(serializer);

    public static ISerializer<C> collectionSerializer<A, C>(
      ISerializer<A> serializer
    ) where C : ICollection<A> =>
      new ICollectionSerializer<A, C>(serializer);

    public static IDeserializer<C> collectionDeserializer<A, C>(
      IDeserializer<A> deserializer, CollectionBuilderKnownSizeFactory<A, C> factory
    ) => new CollectionDeserializer<A, C>(deserializer, factory);
  }
}