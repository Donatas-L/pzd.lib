using System.Collections.Generic;
using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class IListExts {
    public static bool tryGet<A>(this IList<A> list, int index, out A a) {
      if (list.indexValid(index)) {
        a = list[index];
        return true;
      }
      else {
        a = default;
        return false;
      }
    }
    
    public static bool tryLast<A>(this IList<A> list, out A a) {
      if (list.Count != 0) {
        a = list[list.Count - 1];
        return true;
      }

      a = default;
      return false;
    }

    public static bool isEmpty<A>(this IList<A> list) => list.Count == 0;
    public static bool nonEmpty<A>(this IList<A> list) => list.Count != 0;
  }
}