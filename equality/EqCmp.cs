using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace pzd.lib.equality {
  public static class EqCmp {
    [PublicAPI] public static Action<Type> typeDoesNotImplementEquatable;
  }
  
  public class EqCmp<A> : IEqualityComparer<A> {
    public static readonly EqCmp<A> Default = new EqCmp<A>();

    readonly EqualityComparer<A> cmp = EqualityComparer<A>.Default;

    EqCmp() {
      var type = typeof(A);
      if (type.IsValueType && !typeof(IEquatable<A>).IsAssignableFrom(type)) {
        EqCmp.typeDoesNotImplementEquatable?.Invoke(type);
      }
    }

    public bool Equals(A x, A y) => cmp.Equals(x, y);
    public int GetHashCode(A obj) => cmp.GetHashCode(obj);
  }
}