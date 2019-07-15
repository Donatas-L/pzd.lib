using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace pzd.lib.pools {
  [PublicAPI] public class Pool<A> {
    readonly Stack<A> values = new Stack<A>();

    readonly Func<A> create;
    readonly Action<A> reset;

    public Pool(Func<A> create, Action<A> reset) {
      this.create = create;
      this.reset = reset;
    }

    public A Borrow() {
      lock (values) {
        return values.Count > 0 ? values.Pop() : create();
      }
    }

    public void Release(A value) {
      reset(value);
      lock (values) {
        values.Push(value);
      }
    }
  }
}