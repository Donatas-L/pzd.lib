using System;
using JetBrains.Annotations;
using pzd.lib.equality;

namespace pzd.lib.functional.higher_kinds {
  [PublicAPI] public static class Id {
    public struct W {}

    public static Id<A> a<A>(A a) => new Id<A>(a);
    public static Id<A> narrowK<A>(this HigherKind<W, A> hkt) => (Id<A>) hkt;
  }
  
  /// <summary>Id monad is a way to lift a value into a monad when dealing with higher kinded code.</summary>
  [PublicAPI] public partial struct Id<A> : HigherKind<Id.W, A>, IEquatable<Id<A>> {
    public readonly A a;

    public Id(A a) { this.a = a; }

    #region Equality

    public bool Equals(Id<A> other) => EqCmp<A>.Default.Equals(a, other.a);
    public override bool Equals(object obj) => obj is Id<A> other && Equals(other);
    public override int GetHashCode() => EqCmp<A>.Default.GetHashCode(a);
    public static bool operator ==(Id<A> left, Id<A> right) { return left.Equals(right); }
    public static bool operator !=(Id<A> left, Id<A> right) { return !left.Equals(right); }

    #endregion

    public static implicit operator A(Id<A> id) => id.a;
    public static implicit operator Id<A>(A a) => new Id<A>(a);
  }
}