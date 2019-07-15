using System.Collections.Generic;
using JetBrains.Annotations;

namespace pzd.lib.utils {
  [PublicAPI] public static class KV {
    public static KeyValuePair<K, V> a<K, V>(K k, V v) => new KeyValuePair<K, V>(k, v);
  }
}