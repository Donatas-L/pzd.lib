using System.Collections.Immutable;
using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class IImmutableSetExts {
    public static bool isEmpty<A>(this IImmutableSet<A> list) => list.Count == 0;
    public static bool nonEmpty<A>(this IImmutableSet<A> list) => list.Count != 0;
  }
}