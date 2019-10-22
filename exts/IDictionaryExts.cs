using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class IDictionaryExts {
    public static void AddDescriptive<K, V>(
      this IDictionary<K, V> dict, K key, V v
    ) {
      try {
        dict.Add(key, v);
      }
      catch (ArgumentException e) {
        var currentExists = dict.TryGetValue(key, out var existingV);
        var currentStr = currentExists ? "is non existent" : existingV.ToString();
        throw new ArgumentException(
          $"Tried to add {key} -> {v}, but it failed (current value {currentStr}", e
        );
      }
    }
    
    public static V getOrUpdate<K, V>(
      this IDictionary<K, V> dict, K key, Func<V> ifNotFound
    ) => getOrUpdate(dict, key, _ => ifNotFound());

    public static V getOrUpdate<K, V>(
      this IDictionary<K, V> dict, K key, Func<K, V> ifNotFound
    ) {
      if (dict.TryGetValue(key, out var outVal))
        return outVal;
      var v = ifNotFound(key);
      dict.Add(key, v);
      return v;
    }
  }
}