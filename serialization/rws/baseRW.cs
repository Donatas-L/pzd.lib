using System;
using pzd.lib.collection;
using pzd.lib.functional;

namespace pzd.lib.serialization.rws {
  public abstract class BaseRW<A> : ISerializedRW<A> {
    public Either<string, DeserializeInfo<A>> deserialize(byte[] serialized, int startIndex) {
      try {
        return tryDeserialize(serialized, startIndex);
      }
      catch (Exception e) {
        return $"Deserializing {typeof(A).FullName} at index {startIndex} threw {e}";
      }
    }

    protected abstract DeserializeInfo<A> tryDeserialize(byte[] serialized, int startIndex);

    public abstract Rope<byte> serialize(A a);
  }
}