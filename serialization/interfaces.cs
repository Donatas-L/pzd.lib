using System;
using pzd.lib.collection;
using pzd.lib.functional;

namespace pzd.lib.serialization {
  public interface ISerializer<in A> {
    Rope<byte> serialize(A a);
  }

  public interface IDeserializer<A> {
    Either<string, DeserializeInfo<A>> deserialize(byte[] serialized, int startIndex);
  }

  public interface ISerializedRW<A> : IDeserializer<A>, ISerializer<A> { }
  
  public delegate Either<string, A> Deserialize<A>(byte[] serialized, int startIndex);

  public delegate Rope<byte> Serialize<in A>(A a);
}