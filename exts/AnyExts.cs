using System.Collections;
using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class AnyExts {
    public static CastBuilder<A> cast<A>(this A a) where A : class => new CastBuilder<A>(a);
    
    public static string asDebugString<A>(this A a) {
      // strings are enumerable, but we don't want to look at them like that...
      if (a is string) return $"'{a}'";
      // ReSharper disable once InvokeAsExtensionMethod
      return a is IEnumerable enumerable
        ? IEnumerableExts.asDebugString(enumerable)
        : a == null ? "null" : a.ToString();
    }
  }
}