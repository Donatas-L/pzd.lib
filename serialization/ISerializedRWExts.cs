using JetBrains.Annotations;

namespace pzd.lib.serialization {
  [PublicAPI] public static class ISerializedRWExts {
    public static A clone<A>(this A a, ISerializedRW<A> rw) {
      var bytes = rw.serialize(a).toArray();
      return rw.deserialize(bytes, 0).rightOrThrow.value;
    }
  }
}