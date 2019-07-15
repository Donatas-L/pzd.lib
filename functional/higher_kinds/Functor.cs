using System;
using JetBrains.Annotations;

namespace pzd.lib.functional.higher_kinds {
  public interface Functor<Witness> {
    [PublicAPI]
    HigherKind<Witness, B> map<A, B>(HigherKind<Witness, A> data, Func<A, B> mapper);
  }

  public static class FunctorExts {
    [PublicAPI] public static HigherKind<Witness, B> map<Witness, A, B>(
      this HigherKind<Witness, A> hkt, Functor<Witness> F, Func<A, B> mapper
    ) => F.map(hkt, mapper);
  }

  [PublicAPI] public class Functors : Functor<Id.W>, Functor<Option.W> {
    public static readonly Functors i = new Functors();
    Functors() {}
  
    public HigherKind<Id.W, B> map<A, B>(HigherKind<Id.W, A> data, Func<A, B> mapper) =>
      Id.a(mapper(data.narrowK().a));

    public HigherKind<Option.W, B> map<A, B>(HigherKind<Option.W, A> data, Func<A, B> mapper) =>
      data.narrowK().map(mapper);
  }
}