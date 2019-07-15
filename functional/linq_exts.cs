using System;
using JetBrains.Annotations;

namespace pzd.lib.functional {
  [PublicAPI] public static class OptionLinqExts {
    public static Option<B> Select<A, B>(this Option<A> opt, Func<A, B> f) => opt.map(f);
    public static Option<B> SelectMany<A, B>(this Option<A> opt, Func<A, Option<B>> f) => opt.flatMap(f);
    public static Option<C> SelectMany<A, B, C>(
      this Option<A> opt, Func<A, Option<B>> f, Func<A, B, C> g
    ) => opt.flatMap(f, g);
    public static Option<A> Where<A>(this Option<A> opt, Func<A, bool> f) => opt.filter(f);
  }

  [PublicAPI] public static class EitherLinqExts {
    public static Either<L, R1> Select<L, R, R1>(this Either<L, R> e, Func<R, R1> f) =>
      e.mapRight(f);

    public static Either<L, R1> SelectMany<L, R, R1>(this Either<L, R> e, Func<R, Either<L, R1>> f) =>
      e.flatMapRight(f);

    public static Either<L, R2> SelectMany<L, R, R1, R2>(
      this Either<L, R> opt, Func<R, Either<L, R1>> f, Func<R, R1, R2> g
    ) => opt.flatMapRight(f, g);
  }
}