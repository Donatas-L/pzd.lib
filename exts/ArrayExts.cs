using System;

namespace pzd.lib.exts {
  public static class ArrayExts {
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
  }
}