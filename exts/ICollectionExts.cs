using System.Collections.Generic;
using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class ICollectionExts {
    public static bool indexValid<A>(this ICollection<A> collection, int index) =>
      index >= 0 && index < collection.Count;
  }
}