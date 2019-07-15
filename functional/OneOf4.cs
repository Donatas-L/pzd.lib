using System;
using JetBrains.Annotations;
using pzd.lib.equality;

namespace pzd.lib.functional {
  public static class OneOf4 {
    public enum Choice : byte { A, B, C, D }
  }

  [PublicAPI] public struct OneOf<A, B, C, D> : IEquatable<OneOf<A, B, C, D>> {
    readonly A __unsafeGetA;
    readonly B __unsafeGetB;
    readonly C __unsafeGetC;
    readonly D __unsafeGetD;
    public readonly OneOf4.Choice whichOne;

    public OneOf(A a) {
      __unsafeGetA = a;
      __unsafeGetB = default;
      __unsafeGetC = default;
      __unsafeGetD = default;
      whichOne = OneOf4.Choice.A;
    }

    public OneOf(B b) {
      __unsafeGetA = default;
      __unsafeGetB = b;
      __unsafeGetC = default;
      __unsafeGetD = default;
      whichOne = OneOf4.Choice.B;
    }

    public OneOf(C c) {
      __unsafeGetA = default;
      __unsafeGetB = default;
      __unsafeGetC = c;
      __unsafeGetD = default;
      whichOne = OneOf4.Choice.C;
    }

    public OneOf(D d) {
      __unsafeGetA = default;
      __unsafeGetB = default;
      __unsafeGetC = default;
      __unsafeGetD = d;
      whichOne = OneOf4.Choice.D;
    }

#region Equality

    public bool Equals(OneOf<A, B, C, D> other) {
      if (whichOne != other.whichOne) return false;
      switch (whichOne) {
        case OneOf4.Choice.A: return EqCmp<A>.Default.Equals(__unsafeGetA, other.__unsafeGetA);
        case OneOf4.Choice.B: return EqCmp<B>.Default.Equals(__unsafeGetB, other.__unsafeGetB);
        case OneOf4.Choice.C: return EqCmp<C>.Default.Equals(__unsafeGetC, other.__unsafeGetC);
        case OneOf4.Choice.D: return EqCmp<D>.Default.Equals(__unsafeGetD, other.__unsafeGetD);
        default: throw new Exception("Unreachable code");
      }
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;
      return obj is OneOf<A, B, C, D> oneOf && Equals(oneOf);
    }

    public override int GetHashCode() {
      switch (whichOne) {
        case OneOf4.Choice.A: return EqCmp<A>.Default.GetHashCode(__unsafeGetA);
        case OneOf4.Choice.B: return EqCmp<B>.Default.GetHashCode(__unsafeGetB);
        case OneOf4.Choice.C: return EqCmp<C>.Default.GetHashCode(__unsafeGetC);
        case OneOf4.Choice.D: return EqCmp<D>.Default.GetHashCode(__unsafeGetD);
        default: throw new Exception("Unreachable code");
      }
    }

    public static bool operator ==(OneOf<A, B, C, D> lhs, OneOf<A, B, C, D> rhs) => lhs.Equals(rhs);
    public static bool operator !=(OneOf<A, B, C, D> lhs, OneOf<A, B, C, D> rhs) => !(lhs == rhs);

#endregion

    public bool isA => whichOne == OneOf4.Choice.A;
    public Option<A> aValue => isA ? Some.a(__unsafeGetA) : None._;

    public bool isB => whichOne == OneOf4.Choice.B;
    public Option<B> bValue => isB ? Some.a(__unsafeGetB) : None._;

    public bool isC => whichOne == OneOf4.Choice.C;
    public Option<C> cValue => isC ? Some.a(__unsafeGetC) : None._;

    public bool isD => whichOne == OneOf4.Choice.D;
    public Option<D> dValue => isD ? Some.a(__unsafeGetD) : None._;

    public override string ToString() {
      switch (whichOne) {
        case OneOf4.Choice.A: return $"OneOf[{typeof(A)}]({__unsafeGetA})";
        case OneOf4.Choice.B: return $"OneOf[{typeof(B)}]({__unsafeGetB})";
        case OneOf4.Choice.C: return $"OneOf[{typeof(C)}]({__unsafeGetC})";
        case OneOf4.Choice.D: return $"OneOf[{typeof(D)}]({__unsafeGetD})";
        default: throw new Exception("Unreachable code");
      }
    }

    public void voidFold(Action<A> onA, Action<B> onB, Action<C> onC, Action<D> onD) {
      switch (whichOne) {
        case OneOf4.Choice.A:
          onA(__unsafeGetA);
          return;
        case OneOf4.Choice.B:
          onB(__unsafeGetB);
          return;
        case OneOf4.Choice.C:
          onC(__unsafeGetC);
          return;
        case OneOf4.Choice.D:
          onD(__unsafeGetD);
          return;
        default:
          throw new Exception("Unreachable code");
      }
    }

    public R fold<R>(Func<A, R> onA, Func<B, R> onB, Func<C, R> onC, Func<D, R> onD) {
      switch (whichOne) {
        case OneOf4.Choice.A: return onA(__unsafeGetA);
        case OneOf4.Choice.B: return onB(__unsafeGetB);
        case OneOf4.Choice.C: return onC(__unsafeGetC);
        case OneOf4.Choice.D: return onD(__unsafeGetD);
        default: throw new Exception("Unreachable code");
      }
    }

    public static implicit operator OneOf<A, B, C, D>(A a) => new OneOf<A, B, C, D>(a);
    public static implicit operator OneOf<A, B, C, D>(B b) => new OneOf<A, B, C, D>(b);
    public static implicit operator OneOf<A, B, C, D>(C c) => new OneOf<A, B, C, D>(c);
    public static implicit operator OneOf<A, B, C, D>(D d) => new OneOf<A, B, C, D>(d);
  }
}