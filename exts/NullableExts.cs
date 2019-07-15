using System;
using JetBrains.Annotations;
using pzd.lib.functional;

namespace pzd.lib.exts {
  [PublicAPI] public static class NullableExts {
    public static bool valueOut<A>(this A? nullable, out A val) where A : struct {
      if (nullable.HasValue) {
        val = nullable.Value;
        return true;
      }
      val = default;
      return false;
    }

    public static B? map<A, B>(
      this A? nullable, Func<A, B> mapper
    ) where A : struct 
      where B : struct 
      => nullable.HasValue ? (B?) mapper(nullable.Value) : null;

    public static B? map<A, Param, B>(
      this A? nullable, Param param, Func<A, Param, B> mapper
    ) where A : struct 
      where B : struct 
      => nullable.HasValue ? (B?) mapper(nullable.Value, param) : null;

    public static Option<A> toOption<A>(this A? opt) where A : struct =>
      opt.HasValue ? new Option<A>(opt.Value) : new Option<A>();
  }
}