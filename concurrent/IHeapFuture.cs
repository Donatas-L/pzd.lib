using System;
using JetBrains.Annotations;
using pzd.lib.functional;

namespace pzd.lib.concurrent {
  // Can't split into two interfaces and use variance because mono runtime
  // often crashes with variance.
  [PublicAPI] public interface IHeapFuture<A> {
    bool isCompleted { get; }
    void onComplete(Action<A> action);
    Option<A> value { get; }
    bool valueOut(out A a);
  }
}