using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using pzd.lib.exts;
using pzd.lib.functional;

namespace pzd.lib.config {
  [PublicAPI] public static class ConfigExts {
    public static Config.Parser<From, B> map<From, A, B>(
      this Config.Parser<From, A> aParser, Func<ConfigPath, A, B> f
    ) =>
      (path, o) => aParser(path, o).mapRight(a => f(path, a));

    public static Config.Parser<From, B> map<From, A, B>(
      this Config.Parser<From, A> aParser, Func<A, B> f
    ) =>
      aParser.map((path, a) => f(a));

    public static Config.Parser<From, B> flatMap<From, A, B>(
      this Config.Parser<From, A> aParser, Func<ConfigPath, A, Option<B>> f
    ) => aParser.flatMap((path, a) => {
      var bOpt = f(path, a);
      return bOpt.isSome
        ? Either<ConfigLookupError, B>.Right(bOpt.__unsafeGet)
        : Config.parseErrorEFor<B>(path, a);
    });

    public static Config.Parser<From, B> flatMap<From, A, B>(
      this Config.Parser<From, A> aParser, Func<A, Option<B>> f
    ) => aParser.flatMap((path, a) => f(a));

    public static Config.Parser<From, B> flatMapParser<From, A, B>(
      this Config.Parser<From, A> aParser, Config.Parser<A, B> bParser
    ) => (path, node) => aParser(path, node).flatMapRight(a => bParser(path, a));

    public static Config.Parser<From, B> flatMapParser<From, A, B>(
      this Config.Parser<From, A> aParser, Func<ConfigPath, A, Config.Parser<A, B>> bParser
    ) => (path, node) => aParser(path, node).flatMapRight(a => bParser(path, a)(path, a));

    public static Config.Parser<From, B> flatMap<From, A, B>(
      this Config.Parser<From, A> aParser, Func<ConfigPath, A, Either<ConfigLookupError, B>> f
    ) =>
      (path, o) => aParser(path, o).flatMapRight(a => f(path, a));

    public static Config.Parser<From, B> flatMapTry<From, A, B>(
      this Config.Parser<From, A> aParser, Func<ConfigPath, A, B> f
    ) =>
      (path, o) => aParser(path, o).flatMapRight(a => {
        try { return new Either<ConfigLookupError, B>(f(path, a)); }
        catch (ConfigFetchException e) { return new Either<ConfigLookupError, B>(e.error); }
        catch (Exception e) { return new Either<ConfigLookupError, B>(ConfigLookupError.fromException(e)); }
      });

    public static Config.Parser<From, A> filter<From, A>(
      this Config.Parser<From, A> parser, Func<A, bool> predicate
    ) =>
      (path, o) => parser(path, o).flatMapRight(a =>
        predicate(a)
        ? new Either<ConfigLookupError, A>(a)
        : Config.parseErrorEFor<A>(path, a, "didn't pass predicate")
      );

    public static Config.Parser<From, B> collect<From, A, B>(
      this Config.Parser<From, A> parser, Func<A, Option<B>> collector
    ) =>
      (path, o) => parser(path, o).flatMapRight(a => {
        var bOpt = collector(a);
        return bOpt.isSome
          ? new Either<ConfigLookupError, B>(bOpt.__unsafeGet)
          : Config.parseErrorEFor<B>(path, a, "didn't pass collector");
      });

    public static Config.Parser<From, A> or<From, A>(
      this Config.Parser<From, A> a1, Config.Parser<From, A> a2
    ) =>
      (path, node) => {
        var a1E = a1(path, node);
        return a1E.isRight ? a1E : a2(path, node);
      };

    /// <summary>Try parsing same node with both parsers.</summary>
    public static Config.Parser<From, B> and<From, A1, A2, B>(
      this Config.Parser<From, A1> a1p, Config.Parser<From, A2> a2p, Func<A1, A2, B> f
    ) =>
      (path, node) => {
        var a1E = a1p(path, node);
        if (a1E.isLeft) return a1E.__unsafeCastRight<B>();
        var a2E = a2p(path, node);
        if (a2E.isLeft) return a2E.__unsafeCastRight<B>();
        return f(a1E.__unsafeGetRight, a2E.__unsafeGetRight);
      };

    public static Config.Parser<From, B> and<From, A1, A2, A3, B>(
      this Config.Parser<From, A1> a1p, Config.Parser<From, A2> a2p, Config.Parser<From, A3> a3p, 
      Func<A1, A2, A3, B> f
    ) =>
      (path, node) => {
        var a1E = a1p(path, node);
        if (a1E.isLeft) return a1E.__unsafeCastRight<B>();
        var a2E = a2p(path, node);
        if (a2E.isLeft) return a2E.__unsafeCastRight<B>();
        var a3E = a3p(path, node);
        if (a3E.isLeft) return a3E.__unsafeCastRight<B>();
        return f(a1E.__unsafeGetRight, a2E.__unsafeGetRight, a3E.__unsafeGetRight);
      };

    public static Config.Parser<From, B> and<From, A1, A2, A3, A4, B>(
      this Config.Parser<From, A1> a1p, Config.Parser<From, A2> a2p, Config.Parser<From, A3> a3p, 
      Config.Parser<From, A4> a4p, Func<A1, A2, A3, A4, B> f
    ) =>
      (path, node) => {
        var a1E = a1p(path, node);
        if (a1E.isLeft) return a1E.__unsafeCastRight<B>();
        var a2E = a2p(path, node);
        if (a2E.isLeft) return a2E.__unsafeCastRight<B>();
        var a3E = a3p(path, node);
        if (a3E.isLeft) return a3E.__unsafeCastRight<B>();
        var a4E = a4p(path, node);
        if (a4E.isLeft) return a4E.__unsafeCastRight<B>();
        return f(a1E.__unsafeGetRight, a2E.__unsafeGetRight, a3E.__unsafeGetRight, a4E.__unsafeGetRight);
      };

    public static Config.Parser<From, C> tpl<From, A1, A2, C>(
      this Config.Parser<From, A1> a1p, Config.Parser<From, A2> a2p, Func<A1, A2, C> mapper
    ) =>
      (path, node) => {
        if (node is List<From> list) {
          if (list.Count == 2) {
            return 
              from a1 in a1p(path.indexed(0), list[0])
              from a2 in a2p(path.indexed(1), list[1])
              select mapper(a1, a2);
          }
          else {
            return Config.parseErrorFor<(A1, A2)>(path, node, $"expected list of 2, got {list}");
          }
        }
        else {
          return Config.parseErrorFor<(A1, A2)>(path, node);
        }
      };

    public static Config.Parser<From, C> tpl<From, A1, A2, A3, C>(
      this Config.Parser<From, A1> a1p, Config.Parser<From, A2> a2p, Config.Parser<From, A3> a3p, 
      Func<A1, A2, A3, C> mapper
    ) =>
      (path, node) => {
        if (node is List<From> list) {
          if (list.Count == 3) {
            return 
              from a1 in a1p(path.indexed(0), list[0])
              from a2 in a2p(path.indexed(1), list[1])
              from a3 in a3p(path.indexed(2), list[2])
              select mapper(a1, a2, a3);
          }
          else {
            return Config.parseErrorFor<(A1, A2, A3)>(path, node, $"expected list of 3, got {list}");
          }
        }
        else {
          return Config.parseErrorFor<(A1, A2, A3)>(path, node);
        }
      };

    /// <see cref="Config.tpl{From,R}"/>
    public static Config.Parser<From, C> tpl<From, A1, A2, A3, A4, C>(
      this Config.Parser<From, A1> a1p, Config.Parser<From, A2> a2p, Config.Parser<From, A3> a3p,
      Config.Parser<From, A4> a4p, Func<A1, A2, A3, A4, C> mapper
    ) =>
      (path, node) => {
        if (node is List<From> list) {
          if (list.Count == 4) {
            return 
              from a1 in a1p(path.indexed(0), list[0])
              from a2 in a2p(path.indexed(1), list[1])
              from a3 in a3p(path.indexed(2), list[2])
              from a4 in a4p(path.indexed(3), list[3])
              select mapper(a1, a2, a3, a4);
          }
          else {
            return Config.parseErrorFor<(A1, A2, A3, A4)>(
              path, node, $"expected list of 4, got {list}"
            );
          }
        }
        else {
          return Config.parseErrorFor<(A1, A2, A3, A4)>(path, node);
        }
      };
    
    public static Either<ConfigLookupError, To> aE<From, To>(
      this Config.Parser<From, To> p, Config.ParserInput<From> input
    ) => p(input.path, input.node);
    
    public static To a<From, To>(
      this Config.Parser<From, To> p, Config.ParserInput<From> input
    ) => p(input.path, input.node).rightOrThrow;
  }
}