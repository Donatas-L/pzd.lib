using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class IListExts {
    public static A a<A>(this IList<A> list, int index) {
      if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException(
        nameof(index),
        $"Index invalid for IList<{typeof(A)}> (count={list.Count}): {index}"
      );
      return list[index];
    }
    
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
    
    public static Dictionary<K, A> toDict<A, K>(
      this IEnumerable<KeyValuePair<K, A>> list
    ) => list.toDict(p => p.Key, p => p.Value);
    
    public static Dictionary<K, A> toDict<A, K>(
      this IEnumerable<A> list, Func<A, K> keyGetter
    ) => list.toDict(keyGetter, _ => _);
    
    /// <summary>
    /// AOT safe version of ToDictionary.
    /// </summary>
    public static Dictionary<K, V> toDict<A, K, V>(
      this IEnumerable<A> list, Func<A, K> keyGetter, Func<A, V> valueGetter
    ) {
      var dict = new Dictionary<K, V>();
      // ReSharper disable once LoopCanBeConvertedToQuery
      // We're trying to avoid LINQ to avoid iOS AOT related issues.
      foreach (var item in list) {
        var key = keyGetter(item);
        var value = valueGetter(item);
        if (dict.ContainsKey(key)) {
          throw new ArgumentException(
            $"Can't add duplicate key '{key}', current value={dict[key]}, new value={value}"
          );
        }
        dict.Add(key, value);
      }
      return dict;
    }
  }
}