using System;
using JetBrains.Annotations;
using pzd.lib.functional;

namespace pzd.lib.serialization {
  public struct DeserializeInfo<A> {
    public readonly A value;
    public readonly int bytesRead;

    public DeserializeInfo(A value, int bytesRead) {
      this.value = value;
      this.bytesRead = bytesRead;
    }

    public Either<string, DeserializeInfo<B>> flatMapTry<B>(Func<A, B> mapper) {
      try {
        return new DeserializeInfo<B>(mapper(value), bytesRead);
      }
      catch (Exception e) {
        return $"{nameof(flatMapTry)} from {typeof(A).FullName} to {typeof(B).FullName} threw {e}";
      }
    }

    public DeserializeInfo<B> map<B>(Func<A, B> mapper) => 
      new DeserializeInfo<B>(mapper(value), bytesRead);
  }

  [PublicAPI] public static class DeserializeInfoExts {
    public static Option<DeserializeInfo<B>> map<A, B>(
      this Option<DeserializeInfo<A>> aOpt, Func<A, B> mapper
    ) {
      if (aOpt.isNone) return None._;
      var aInfo = aOpt.__unsafeGet;
      return Some.a(new DeserializeInfo<B>(mapper(aInfo.value), aInfo.bytesRead));
    }
  }
}