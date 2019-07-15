using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using pzd.lib.equality;
using pzd.lib.functional.higher_kinds;

namespace pzd.lib.functional {
  [PublicAPI] public readonly struct Option<A> : IEquatable<Option<A>>, HigherKind<Option.W, A> {
    public readonly bool isSome;
    public bool isNone => !isSome;
    public readonly A __unsafeGet;

    public Option(A a) {
      isSome = true;
      __unsafeGet = a;
    }

    public static readonly Option<A> None;
    
    public override string ToString() => isSome ? $"Some({__unsafeGet})" : "None";
    
    #region Equality

    public bool Equals(Option<A> other) {
      if (isSome && other.isSome) return EqCmp<A>.Default.Equals(__unsafeGet, other.__unsafeGet);
      else return isNone == other.isNone;
    }

    public override bool Equals(object obj) => obj is Option<A> other && Equals(other);

    public override int GetHashCode() {
      unchecked {
        return (isSome.GetHashCode() * 397) ^ EqCmp<A>.Default.GetHashCode(__unsafeGet);
      }
    }

    public static bool operator ==(Option<A> left, Option<A> right) => left.Equals(right);
    public static bool operator !=(Option<A> left, Option<A> right) => !left.Equals(right);

    #endregion

    #region Getting values out
    
    public bool valueOut(out A a) {
      a = __unsafeGet;
      return isSome;
    }

    public A get { get {
      if (isSome) return __unsafeGet;
      else throw new Exception("#get on None!");
    } }
    
    public A getOrThrow(Func<Exception> getEx) =>isSome ? __unsafeGet : throw getEx();
    public A getOrThrow(string message) => isSome ? __unsafeGet : throw new Exception(message);

    public A getOrElse(A a) => isSome ? __unsafeGet : a;
    public A getOrElse(Func<A> a) => isSome ? __unsafeGet : a();
    
    public OptionEnumerator<A> GetEnumerator() => new OptionEnumerator<A>(this);
    
    public void voidFold(Action ifNone, Action<A> ifSome) {
      if (isSome) ifSome(__unsafeGet);
      else ifNone();
    }

    public IEnumerable<A> asEnumerable { get {
      if (isSome) yield return __unsafeGet;
    } }

    #endregion

    #region Checks
    
    public bool exists(Func<A, bool> predicate) => isSome && predicate(__unsafeGet);
    public bool exists(A a) => exists(a, EqCmp<A>.Default);
    public bool exists(A a, IEqualityComparer<A> comparer) => isSome && comparer.Equals(__unsafeGet, a);

    #endregion
    
    #region Transformations
    
    public Option<B> map<B>(Func<A, B> mapper) =>
      isSome ? new Option<B>(mapper(__unsafeGet)) : new Option<B>();
    
    public Option<B> map<V, B>(V v, Func<A, V, B> mapper) =>
      isSome ? new Option<B>(mapper(__unsafeGet, v)) : new Option<B>();

    public Option<B> flatMap<B>(Func<A, Option<B>> mapper) =>
      isSome ? mapper(__unsafeGet) : new Option<B>();
    
    public Option<B> flatMap<V, B>(V v, Func<A, V, Option<B>> mapper) =>
      isSome ? mapper(__unsafeGet, v) : new Option<B>();
    
    public Option<C> flatMap<B, C>(Func<A, Option<B>> func, Func<A, B, C> mapper) {
      if (isNone) return new Option<C>();
      var bOpt = func(__unsafeGet);
      return bOpt.isNone ? new Option<C>() : new Option<C>(mapper(__unsafeGet, bOpt.__unsafeGet));
    }
    
    public Option<A> filter(bool predicate) =>
      isSome && predicate ? this : new Option<A>();
    
    public Option<A> filter(Func<A, bool> predicate) =>
      isSome && predicate(__unsafeGet) ? this : new Option<A>();

    public B fold<B>(Func<B> ifNone, Func<A, B> ifSome) => isSome ? ifSome(__unsafeGet) : ifNone();
    public B fold<B>(B ifNone, Func<A, B> ifSome) => isSome ? ifSome(__unsafeGet) : ifNone;
    public B fold<B>(B ifNone, B ifSome) => isSome ? ifSome : ifNone;

    public Either<A, B> toLeft<B>(B right) =>
      isSome ? Either<A, B>.Left(__unsafeGet) : Either<A, B>.Right(right);
    
    public Either<A, B> toLeft<B>(Func<B> right) =>
      isSome ? Either<A, B>.Left(__unsafeGet) : Either<A, B>.Right(right());
    
    public Either<B, A> toRight<B>(B left) =>
      isSome ? Either<B, A>.Right(__unsafeGet) : Either<B, A>.Left(left);
    
    public Either<B, A> toRight<B>(Func<B> left) =>
      isSome ? Either<B, A>.Right(__unsafeGet) : Either<B, A>.Left(left());
    
    public Option<C> zip<B, C>(Option<B> opt2, Func<A, B, C> mapper) =>
      isSome && opt2.isSome
      ? new Option<C>(mapper(__unsafeGet, opt2.__unsafeGet))
      : new Option<C>();

    #endregion
    
    /// <summary>Allows implicitly converting <see cref="None"/> to None <see cref="Option{A}"/>.</summary>
    public static implicit operator Option<A>(None _) => new Option<A>();
    
    public static bool operator true(Option<A> opt) => opt.isSome;
    
    /**
      * Required by |.
      *
      * http://stackoverflow.com/questions/686424/what-are-true-and-false-operators-in-c#comment43525525_686473
      * The only situation where operator false matters, seems to be if MyClass also overloads
      * the operator &, in a suitable way. So you can say MyClass conj = GetMyClass1() & GetMyClass2();.
      * Then with operator false you can short-circuit and say
      * MyClass conj = GetMyClass1() && GetMyClass2();, using && instead of &. That will only
      * evaluate the second operand if the first one is not "false".
      **/
    public static bool operator false(Option<A> opt) => opt.isNone;
    
    public static Option<A> operator |(Option<A> o1, Option<A> o2) => o1 ? o1 : o2;
  }

  [PublicAPI] public static class Option {
    public static Option<A> a<A>(A a) where A : class => a == null ? None._ : Some.a(a);
    
    public static void ensureValue<A>(ref Option<A> opt) {
      // if ((object) opt == null) opt = None._;
    }
    
    // Witness for higher kinds
    public struct W {}
    
    public static Option<A> narrowK<A>(this HigherKind<W, A> hkt) => (Option<A>) hkt;
    
    public static A? toNullable<A>(this Option<A> opt) where A : struct => 
      opt.isSome ? opt.__unsafeGet : (A?) null;
    
    public static Option<A> fromNullable<A>(this A? maybeA) where A : struct => 
      maybeA.HasValue ? Some.a(maybeA.Value) : None._;
  }

  [PublicAPI] public static class OptionExts {
    public static A getOrNull<A>(this Option<A> option) where A : class =>
      option.isNone ? null : option.__unsafeGet;

    public static A? nullable<A>(this Option<A> option) where A : struct =>
      option.isNone ? (A?) null : option.__unsafeGet;
  }

  [PublicAPI] public static class Some {
    public static Option<A> a<A>(A a) => new Option<A>(a);
  }

  [PublicAPI] public struct None {
    public static readonly None _; 
  }
  
  [PublicAPI] public struct OptionEnumerator<A> {
    public readonly Option<A> option;
    bool read;

    public OptionEnumerator(Option<A> option) {
      this.option = option;
      read = false;
    }
  
    public bool MoveNext() => option.isSome && !read;
    public void Reset() => read = false;
  
    public A Current { get {
      read = true;
      return option.__unsafeGet;
    } }
  }
}