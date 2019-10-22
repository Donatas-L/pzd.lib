using System;
using JetBrains.Annotations;
using pzd.lib.functional;
using pzd.lib.reactive;

namespace pzd.lib.concurrent {
  // Can't split into two interfaces and use variance because mono runtime
  // often crashes with variance.
  [PublicAPI] public interface IHeapFuture<A> {
    bool isCompleted { get; }
    ISubscription onComplete(Action<A> action);
    Option<A> value { get; }
    bool valueOut(out A a);
  }
}