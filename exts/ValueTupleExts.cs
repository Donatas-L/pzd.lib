using System;
using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class ValueTupleExts {
    public static (AA, B) map1<A, AA, B>(this (A, B) t, Func<A, AA> f) => (f(t.Item1), t.Item2);
    public static (AA, B) map1<A, AA, B>(this (A, B) t, Func<A, B, AA> f) => (f(t.Item1, t.Item2), t.Item2);
    public static (A, BB) map2<A, B, BB>(this (A, B) t, Func<B, BB> f) => (t.Item1, f(t.Item2));
    public static (A, BB) map2<A, B, BB>(this (A, B) t, Func<A, B, BB> f) => (t.Item1, f(t.Item1, t.Item2));
  }
}