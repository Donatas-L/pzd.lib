using System;
using pzd.lib.functional;
using pzd.lib.typeclasses;

namespace pzd.lib.serialization.rws {
  public class CollectionDeserializer<A, C> : IDeserializer<C> {
    readonly IDeserializer<A> deserializer;
    readonly CollectionBuilderKnownSizeFactory<A, C> builderKnownSizeFactory;

    public CollectionDeserializer(
      IDeserializer<A> deserializer, 
      CollectionBuilderKnownSizeFactory<A, C> builderKnownSizeFactory
    ) {
      this.deserializer = deserializer;
      this.builderKnownSizeFactory = builderKnownSizeFactory;
    }

    public Either<string, DeserializeInfo<C>> deserialize(byte[] serialized, int startIndex) {
      try {
        var count = BitConverter.ToInt32(serialized, startIndex);
        var bytesRead = intRW.LENGTH;

        var builder = builderKnownSizeFactory(count);
        var readIdx = startIndex + bytesRead;
        for (var idx = 0; idx < count; idx++) {
          var aEither = deserializer.deserialize(serialized, readIdx);

          if (aEither.leftValueOut(out var aErr)) {
            return $"Error deserializing index {idx}/{count}: {aErr}";
          }

          var aInfo = aEither.__unsafeGetRight;
          bytesRead += aInfo.bytesRead;
          readIdx += aInfo.bytesRead;
          builder.add(aInfo.value);
        }

        return new DeserializeInfo<C>(builder.build(), bytesRead);
      }
      catch (Exception e) {
        return $"deserializing a collection threw {e}";
      }
    }
  }
}