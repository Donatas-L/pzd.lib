using System;
using JetBrains.Annotations;
using pzd.lib.equality;

namespace pzd.lib.functional {
  public static class OneOf5 {
    public enum Choice : byte { A, B, C, D, E }
  }

  [PublicAPI] 
  public struct OneOf<A, B, C, D, E> : IEquatable<OneOf<A, B, C, D, E>> {
    readonly A __unsafeGetA;
    readonly B __unsafeGetB;
    readonly C __unsafeGetC;
    readonly D __unsafeGetD;
    readonly E __unsafeGetE;
    public readonly OneOf5.Choice whichOne;

    public OneOf(A a) : this() {
      __unsafeGetA = a;
      whichOne = OneOf5.Choice.A;
    }

    public OneOf(B b) : this() {
      __unsafeGetB = b;
      whichOne = OneOf5.Choice.B;
    }

    public OneOf(C c) : this()  {
      __unsafeGetC = c;
      whichOne = OneOf5.Choice.C;
    }

    public OneOf(D d) : this() {
      __unsafeGetD = d;
      whichOne = OneOf5.Choice.D;
    }

    public OneOf(E e) : this() {
      __unsafeGetE = e;
      whichOne = OneOf5.Choice.E;
    }

#region Equality

    public bool Equals(OneOf<A, B, C, D, E> other) {
      if (whichOne != other.whichOne) return false;
      switch (whichOne) {
        case OneOf5.Choice.A: return EqCmp<A>.Default.Equals(__unsafeGetA, other.__unsafeGetA);
        case OneOf5.Choice.B: return EqCmp<B>.Default.Equals(__unsafeGetB, other.__unsafeGetB);
        case OneOf5.Choice.C: return EqCmp<C>.Default.Equals(__unsafeGetC, other.__unsafeGetC);
        case OneOf5.Choice.D: return EqCmp<D>.Default.Equals(__unsafeGetD, other.__unsafeGetD);
        case OneOf5.Choice.E: return EqCmp<E>.Default.Equals(__unsafeGetE, other.__unsafeGetE);
        default: throw new Exception("Unreachable code");
      }
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;
      return obj is OneOf<A, B, C, D> oneOf && Equals(oneOf);
    }

    public override int GetHashCode() {
      switch (whichOne) {
        case OneOf5.Choice.A: return EqCmp<A>.Default.GetHashCode(__unsafeGetA);
        case OneOf5.Choice.B: return EqCmp<B>.Default.GetHashCode(__unsafeGetB);
        case OneOf5.Choice.C: return EqCmp<C>.Default.GetHashCode(__unsafeGetC);
        case OneOf5.Choice.D: return EqCmp<D>.Default.GetHashCode(__unsafeGetD);
        case OneOf5.Choice.E: return EqCmp<E>.Default.GetHashCode(__unsafeGetE);
        default: throw new Exception("Unreachable code");
      }
    }

    public static bool operator ==(OneOf<A, B, C, D, E> lhs, OneOf<A, B, C, D, E> rhs) => lhs.Equals(rhs);
    public static bool operator !=(OneOf<A, B, C, D, E> lhs, OneOf<A, B, C, D, E> rhs) => !(lhs == rhs);

#endregion

    public bool isA => whichOne == OneOf5.Choice.A;
    public Option<A> aValue => isA ? Some.a(__unsafeGetA) : None._;

    public bool isB => whichOne == OneOf5.Choice.B;
    public Option<B> bValue => isB ? Some.a(__unsafeGetB) : None._;

    public bool isC => whichOne == OneOf5.Choice.C;
    public Option<C> cValue => isC ? Some.a(__unsafeGetC) : None._;

    public bool isD => whichOne == OneOf5.Choice.D;
    public Option<D> dValue => isD ? Some.a(__unsafeGetD) : None._;

    public bool isE => whichOne == OneOf5.Choice.E;
    public Option<E> eValue => isE ? Some.a(__unsafeGetE) : None._;

    public override string ToString() {
      switch (whichOne) {
        case OneOf5.Choice.A: return $"OneOf[{typeof(A)}]({__unsafeGetA})";
        case OneOf5.Choice.B: return $"OneOf[{typeof(B)}]({__unsafeGetB})";
        case OneOf5.Choice.C: return $"OneOf[{typeof(C)}]({__unsafeGetC})";
        case OneOf5.Choice.D: return $"OneOf[{typeof(D)}]({__unsafeGetD})";
        case OneOf5.Choice.E: return $"OneOf[{typeof(E)}]({__unsafeGetE})";
        default: throw new Exception("Unreachable code");
      }
    }

    public void voidFold(Action<A> onA, Action<B> onB, Action<C> onC, Action<D> onD, Action<E> onE) {
      switch (whichOne) {
        case OneOf5.Choice.A:
          onA(__unsafeGetA);
          return;
        case OneOf5.Choice.B:
          onB(__unsafeGetB);
          return;
        case OneOf5.Choice.C:
          onC(__unsafeGetC);
          return;
        case OneOf5.Choice.D:
          onD(__unsafeGetD);
          return;
        case OneOf5.Choice.E:
          onE(__unsafeGetE);
          return;
        default:
          throw new Exception("Unreachable code");
      }
    }

    public R fold<R>(Func<A, R> onA, Func<B, R> onB, Func<C, R> onC, Func<D, R> onD, Func<E, R> onE) {
      switch (whichOne) {
        case OneOf5.Choice.A: return onA(__unsafeGetA);
        case OneOf5.Choice.B: return onB(__unsafeGetB);
        case OneOf5.Choice.C: return onC(__unsafeGetC);
        case OneOf5.Choice.D: return onD(__unsafeGetD);
        case OneOf5.Choice.E: return onE(__unsafeGetE);
        default: throw new Exception("Unreachable code");
      }
    }

    public static implicit operator OneOf<A, B, C, D, E>(A a) => new OneOf<A, B, C, D, E>(a);
    public static implicit operator OneOf<A, B, C, D, E>(B b) => new OneOf<A, B, C, D, E>(b);
    public static implicit operator OneOf<A, B, C, D, E>(C c) => new OneOf<A, B, C, D, E>(c);
    public static implicit operator OneOf<A, B, C, D, E>(D d) => new OneOf<A, B, C, D, E>(d);
    public static implicit operator OneOf<A, B, C, D, E>(E e) => new OneOf<A, B, C, D, E>(e);
  }
}