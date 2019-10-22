using System;
using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class ArrayExts {
    public static A[] slice<A>(this A[] a, int startIndex, int count = -1) {
      if (startIndex < 0) throw new ArgumentOutOfRangeException(
        nameof(startIndex), $"{nameof(startIndex)}({startIndex}) < 0"
      );
      if (startIndex >= a.Length) throw new ArgumentOutOfRangeException(
        nameof(startIndex), $"{nameof(startIndex)}({startIndex}) >= array length ({a.Length})"
      );
      if (count < 0) count = a.Length - startIndex;
      var endIdxExclusive = startIndex + count;
      if (endIdxExclusive > a.Length) throw new ArgumentOutOfRangeException(
        nameof(count),
        $"{nameof(count)}({count}) is too big (arr size: {a.Length}, {nameof(endIdxExclusive)}={endIdxExclusive})"
      );
      var arr = new A[count];
      Array.Copy(a, startIndex, arr, 0, count);
      //      for (int srcIdx = startIndex, newIdx = 0; srcIdx < endIdxExclusive; srcIdx++, newIdx++)
      //        arr[newIdx] = a[srcIdx];
      return arr;
    }
    
    /// <summary>
    /// Basically LINQ #Select, but for arrays. Needed because of performance/iOS
    /// limitations of AOT.
    /// </summary>
    public static To[] map<From, To>(
      this From[] source, Func<From, To> mapper
    ) {
      var target = new To[source.Length];
      for (var i = 0; i < source.Length; i++) target[i] = mapper(source[i]);
      return target;
    }

    public static To[] map<From, To>(
      this From[] source, Func<From, int, To> mapper
    ) {
      var target = new To[source.Length];
      for (var i = 0; i < source.Length; i++) target[i] = mapper(source[i], i);
      return target;
    }
  }
}