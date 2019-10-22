using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace pzd.lib.functional {
  [PublicAPI] public struct Try<A> {
    public readonly A __unsafeGet;
    public readonly Exception __unsafeException;

    public Try(A value) {
      __unsafeGet = value;
      __unsafeException = null;
    }

    public Try(Exception ex) {
      __unsafeGet = default;
      __unsafeException = ex;
    }

    public bool isSuccess => __unsafeException == null;
    public bool isError => __unsafeException != null;

    public Option<A> toOption => isSuccess ? Some.a(__unsafeGet) : None._;
    public Option<Exception> exception => isSuccess ? None._ : Some.a(__unsafeException);
    
    public bool valueOut(out A v) {
      v = isSuccess ? __unsafeGet : default;
      return isSuccess;
    }
    
    public bool exceptionOut(out Exception e) {
      e = isError ? __unsafeException : default;
      return isError;
    }

    public A getOrThrow => isSuccess ? __unsafeGet : throw __unsafeException;

    public A getOrElse(A a) => isSuccess ? __unsafeGet : a;
    public A getOrElse(Func<A> a) => isSuccess ? __unsafeGet : a();

    public Either<Exception, A> toEither =>
      isSuccess ? (Either<Exception, A>) __unsafeGet : __unsafeException;

    public Either<ImmutableList<string>, A> toValidation =>
      isSuccess
      ? Either<ImmutableList<string>, A>.Right(__unsafeGet)
      : Either<ImmutableList<string>, A>.Left(ImmutableList.Create(__unsafeException.ToString()));

    public B fold<B>(B onValue, Func<Exception, B> onException) =>
      isSuccess ? onValue : onException(__unsafeException);

    public B fold<B>(Func<A, B> onValue, Func<Exception, B> onException) =>
      isSuccess ? onValue(__unsafeGet) : onException(__unsafeException);

    public void voidFold(Action<A> onValue, Action<Exception> onException) {
      if (isSuccess) onValue(__unsafeGet); else onException(__unsafeException);
    }

    public Try<B> map<B>(Func<A, B> onValue) {
      if (isSuccess) {
        try { return new Try<B>(onValue(__unsafeGet)); }
        catch (Exception e) { return new Try<B>(e); }
      }
      return new Try<B>(__unsafeException);
    }

    public Try<B> flatMap<B>(Func<A, Try<B>> onValue) {
      if (isSuccess) {
        try { return onValue(__unsafeGet); }
        catch (Exception e) { return new Try<B>(e); }
      }
      return new Try<B>(__unsafeException);
    }

    public Try<B1> flatMap<B, B1>(Func<A, Try<B>> onValue, Func<A, B, B1> g) {
      if (isSuccess) {
        try {
          var a = __unsafeGet;
          return onValue(a).map(b => g(a, b));
        }
        catch (Exception e) { return new Try<B1>(e); }
      }
      return new Try<B1>(__unsafeException);
    }

    [PublicAPI] public override string ToString() =>
      isSuccess ? $"Success({__unsafeGet})" : $"Error({__unsafeException})";
    
    public static implicit operator Try<A>(A a) => new Try<A>(a);
    public static implicit operator Try<A>(Exception ex) => new Try<A>(ex);
  }

  [PublicAPI] public static class Try {
    public static Try<A> a<A>(Func<A> f) {
      try { return f(); }
      catch (Exception e) { return e; }
    }
  }

  [PublicAPI] public static class TryExts {
    public static Try<ImmutableList<A>> sequence<A>(
      this IEnumerable<Try<A>> enumerable
    ) {
      // mutable for performance
      var b = ImmutableList.CreateBuilder<A>();
      foreach (var t in enumerable) {
        if (t.isError) return t.__unsafeException;
        b.Add(t.__unsafeGet);
      }
      return b.ToImmutable();
    }
  }
}