using System;
using JetBrains.Annotations;
using pzd.lib.equality;

namespace pzd.lib.functional {
  public static class OneOf {
    public enum Choice : byte { A, B, C }
  }

  [PublicAPI] public struct OneOf<A, B, C> : IEquatable<OneOf<A, B, C>> {
    public readonly A __unsafeGetA;
    public readonly B __unsafeGetB;
    public readonly C __unsafeGetC;
    public readonly OneOf.Choice whichOne;

    public OneOf(A a) {
      __unsafeGetA = a;
      __unsafeGetB = default;
      __unsafeGetC = default;
      whichOne = OneOf.Choice.A;
    }

    public OneOf(B b) {
      __unsafeGetA = default;
      __unsafeGetB = b;
      __unsafeGetC = default;
      whichOne = OneOf.Choice.B;
    }

    public OneOf(C c) {
      __unsafeGetA = default;
      __unsafeGetB = default;
      __unsafeGetC = c;
      whichOne = OneOf.Choice.C;
    }

#region Equality

    public bool Equals(OneOf<A, B, C> other) {
      if (whichOne != other.whichOne) return false;
      switch (whichOne) {
        case OneOf.Choice.A: return EqCmp<A>.Default.Equals(__unsafeGetA, other.__unsafeGetA);
        case OneOf.Choice.B: return EqCmp<B>.Default.Equals(__unsafeGetB, other.__unsafeGetB);
        case OneOf.Choice.C: return EqCmp<C>.Default.Equals(__unsafeGetC, other.__unsafeGetC);
        default: throw new Exception("Unreachable code");
      }
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;
      return obj is OneOf<A, B, C> oneOf && Equals(oneOf);
    }

    public override int GetHashCode() {
      switch (whichOne) {
        case OneOf.Choice.A: return EqCmp<A>.Default.GetHashCode(__unsafeGetA);
        case OneOf.Choice.B: return EqCmp<B>.Default.GetHashCode(__unsafeGetB);
        case OneOf.Choice.C: return EqCmp<C>.Default.GetHashCode(__unsafeGetC);
        default: throw new Exception("Unreachable code");
      }
    }

    public static bool operator ==(OneOf<A, B, C> lhs, OneOf<A, B, C> rhs) => lhs.Equals(rhs);
    public static bool operator !=(OneOf<A, B, C> lhs, OneOf<A, B, C> rhs) => !(lhs == rhs);

#endregion

    public bool isA => whichOne == OneOf.Choice.A;
    public Option<A> aValue => isA ? Some.a(__unsafeGetA) : None._;
    public bool aValueOut(out A value) {
      value = __unsafeGetA;
      return isA;
    }

    public bool isB => whichOne == OneOf.Choice.B;
    public Option<B> bValue => isB ? Some.a(__unsafeGetB) : None._;
    public bool bValueOut(out B value) {
      value = __unsafeGetB;
      return isB;
    }

    public bool isC => whichOne == OneOf.Choice.C;
    public Option<C> cValue => isC ? Some.a(__unsafeGetC) : None._;
    public bool cValueOut(out C value) {
      value = __unsafeGetC;
      return isC;
    }

    public override string ToString() =>
        isA ? $"OneOf[{typeof(A)}]({__unsafeGetA})"
      : isB ? $"OneOf[{typeof(B)}]({__unsafeGetB})"
            : $"OneOf[{typeof(C)}]({__unsafeGetC})";

    public void voidFold(Action<A> onA, Action<B> onB, Action<C> onC) {
      switch (whichOne) {
        case OneOf.Choice.A:
          onA(__unsafeGetA);
          return;
        case OneOf.Choice.B:
          onB(__unsafeGetB);
          return;
        case OneOf.Choice.C:
          onC(__unsafeGetC);
          return;
        default:
          throw new Exception("Unreachable code");
      }
    }

    public R fold<R>(Func<A, R> onA, Func<B, R> onB, Func<C, R> onC) {
      switch (whichOne) {
        case OneOf.Choice.A: return onA(__unsafeGetA);
        case OneOf.Choice.B: return onB(__unsafeGetB);
        case OneOf.Choice.C: return onC(__unsafeGetC);
        default: throw new Exception("Unreachable code"); 
      }
    }

    public OneOf<A1, B1, C1> map<A1, B1, C1>(Func<A, A1> ma, Func<B, B1> mb, Func<C, C1> mc) {
      switch (whichOne) {
        case OneOf.Choice.A: return ma(__unsafeGetA);
        case OneOf.Choice.B: return mb(__unsafeGetB);
        case OneOf.Choice.C: return mc(__unsafeGetC);
        default: throw new Exception("Unreachable code"); 
      }
    }

    public static implicit operator OneOf<A, B, C>(A a) => new OneOf<A, B, C>(a);
    public static implicit operator OneOf<A, B, C>(B b) => new OneOf<A, B, C>(b);
    public static implicit operator OneOf<A, B, C>(C c) => new OneOf<A, B, C>(c);
  }
}