using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class ConcurrentQueueExts {
    public static void clear<A>(this ConcurrentQueue<A> q) {
      while (q.TryDequeue(out _)) {}
    }
  }
}