using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using JetBrains.Annotations;
using pzd.lib.collection;
using pzd.lib.exts;
using pzd.lib.functional;
using pzd.lib.json;
using pzd.lib.utils;

namespace pzd.lib.config {
  public class Config : IConfig {
    public struct ParsingError {
      public readonly Option<Exception> exception;
      public readonly string jsonString;

      public ParsingError(Option<Exception> exception, string jsonString) {
        this.exception = exception;
        this.jsonString = jsonString;
      }

      public override string ToString() => 
        $"{nameof(ParsingError)}[{nameof(exception)}: {exception}, {nameof(jsonString)}: {jsonString}]";

      public ImmutableArray<KeyValuePair<string, string>> getExtras() => ImmutableArray.Create(
        KV.a("json", jsonString),
        KV.a("exception", exception.ToString())
      );
    }

    public static Either<ParsingError, IConfig> parseJson(string json) {
      if (json.isEmpty(trim: true)) 
        return new ParsingError(None._, $"<empty>('{json}')");
      
      try {
        return 
          from jsonDict in Json.Deserialize(json).cast().toE<Dictionary<string, object>>().mapLeft(err =>
            new ParsingError(Some.a(new Exception($"Config root must be a JSON object, but: {err}")), json)
          )
          from res in jsonDict == null
            ? Either<ParsingError, IConfig>.Left(new ParsingError(None._, json))
            : Either<ParsingError, IConfig>.Right(new Config(jsonDict))
          select res;
      }
      catch (Exception e) {
        return new ParsingError(Some.a(e), json);
      }
    }

    // Implementation

    #region Parsers

    public delegate Either<ConfigLookupError, To> Parser<in From, To>(ConfigPath path, From node);

    public static ConfigLookupError parseErrorFor<A>(
      ConfigPath path, object node, string extraInfo = null
    ) =>
      ConfigLookupError.wrongType(ImmutableArray.Create(
        KV.a("path", path.pathStrWithBase),
        KV.a("expected-type", typeof(A).FullName),
        KV.a("actual-type", node.GetType().FullName),
        KV.a("extraInfo", extraInfo ?? ""),
        KV.a("node-contents", node.asDebugString())
      ));

    public static Either<ConfigLookupError, A> parseErrorEFor<A>(
      ConfigPath path, object node, string extraInfo = null
    ) => Either<ConfigLookupError, A>.Left(parseErrorFor<A>(path, node, extraInfo));

    public static Parser<object, A> createCastParser<A>() => (path, node) =>
      node is A a
      ? Either<ConfigLookupError, A>.Right(a)
      : parseErrorEFor<A>(path, node);

    /// Parser that always succeeds and returns constant.
    public static Parser<object, A> constantParser<A>(A a) =>
      (path, _) => Either<ConfigLookupError, A>.Right(a);

    /// Parser that always fails and returns constant.
    public static Parser<object, A> constantErrorParser<A>(ConfigLookupError error) =>
      (path, _) => Either<ConfigLookupError, A>.Left(error);

    public static readonly Parser<object, object> objectParser = (_, n) =>
      Either<ConfigLookupError, object>.Right(n);

    public static readonly Parser<object, List<object>> objectListParser = 
      createCastParser<List<object>>();

    public static Parser<From, Option<A>> opt<From, A>(Parser<From, A> parser) =>
      (path, o) =>
        o == null
        ? Either<ConfigLookupError, Option<A>>.Right(None._)
        : parser(path, o).mapRight(Some.a);

    public static Parser<object, CB> collectionParser<CB, A>(
      Parser<object, A> parser,
      Func<int, CB> createCollectionBuilder,
      Func<CB, int, A, CB> add
    ) =>
      objectListParser.flatMap((path, objList) => {
        var builder = createCollectionBuilder(objList.Count);
        for (var idx = 0; idx < objList.Count; idx++) {
          var idxPath = path.indexed(idx);
          var parsedE = parser(idxPath, objList[idx]);
          if (parsedE.isLeft)
            return Either<ConfigLookupError, CB>.Left(parsedE.__unsafeGetLeft);
          builder = add(builder, idx, parsedE.__unsafeGetRight);
        }
        return Either<ConfigLookupError, CB>.Right(builder);
      });

    public static Parser<object, List<A>> listParser<A>(Parser<object, A> parser) =>
      collectionParser(parser, count => new List<A>(count), (l, idx, a) => {
        l.Add(a);
        return l;
      });

    public static Parser<object, ImmutableArray<A>> immutableArrayParser<A>(Parser<object, A> parser) =>
      collectionParser(parser, ImmutableArray.CreateBuilder<A>, (b, idx, a) => {
        b.Add(a);
        return b;
      }).map(_ => _.MoveToImmutable());

    public static Parser<object, A[]> arrayParser<A>(Parser<object, A> parser) =>
      collectionParser(parser, size => new A[size], (b, idx, a) => {
        b[idx] = a;
        return b;
      });

    public static Parser<object, ImmutableArrayC<A>> immutableArrayCParser<A>(Parser<object, A> parser) =>
      arrayParser(parser).map(ImmutableArrayC.move);

    public static Parser<object, ImmutableList<A>> immutableListParser<A>(Parser<object, A> parser) =>
      collectionParser(parser, count => ImmutableList.CreateBuilder<A>(), (b, idx, a) => {
        b.Add(a);
        return b;
      }).map(_ => _.ToImmutable());

    public static Parser<object, ImmutableHashSet<A>> immutableHashSetParser<A>(Parser<object, A> parser) =>
      collectionParser(parser, count => ImmutableHashSet.CreateBuilder<A>(), (b, idx, a) => {
        b.Add(a);
        return b;
      }).map(_ => _.ToImmutable());

    public static readonly Parser<object, Dictionary<string, object>> jsClassParser =
      createCastParser<Dictionary<string, object>>();

    public static readonly Parser<object, IConfig> configParser =
      jsClassParser.map((path, dict) => (IConfig) new Config(dict, ConfigPath.root.baseOn(path)));
    public static readonly Parser<object, List<IConfig>> configListParser =
      listParser(configParser);

    public static Parser<object, Dictionary<K, V>> dictParser<K, V>(
      Parser<string, K> keyParser, Parser<object, V> valueParser
    ) =>
      jsClassParser.flatMap((path, untypedDict) => {
        var dict = new Dictionary<K, V>(untypedDict.Count);
        foreach (var kv in untypedDict) {
          var key = kv.Key;
          var parsedKeyE = keyParser(path, key);
          {
            if (parsedKeyE.leftValueOut(out var err)) return err;
          }
          var parsedKey = parsedKeyE.__unsafeGetRight;

          if (dict.ContainsKey(parsedKey))
            return parseErrorEFor<Dictionary<K, V>>(
              path, kv.Key, $"key already exists as '{dict[parsedKey]}'"
            );

          var parsedValE = valueParser(path / key, kv.Value);
          if (parsedValE.isLeft)
            return new Either<ConfigLookupError, Dictionary<K, V>>(parsedValE.__unsafeGetLeft);

          dict.Add(parsedKey, parsedValE.__unsafeGetRight);
        }
        return Either<ConfigLookupError, Dictionary<K, V>>.Right(dict);
      });

    public static Parser<object, ImmutableDictionary<K, V>> immutableDictParser<K, V>(
      Parser<string, K> keyParser, Parser<object, V> valueParser
    ) => dictParser(keyParser, valueParser).map(_ => _.ToImmutableDictionary());
    
    /// <summary>Parse dictionary from [[key, value], ...] pairs</summary>
    public static Parser<object, ImmutableDictionary<K, V>> listImmutableDictParser<K, V>(
      Parser<object, K> kParser, Parser<object, V> vParser
    ) => 
      listParser(kParser.tpl(vParser, KV.a)).map(l => l.ToImmutableDictionary());

    public static Parser<object, A> configPathedParser<A>(string key, Parser<object, A> aParser) =>
      configParser.flatMap((path, cfg) => cfg.eitherGet(key, aParser));

    public static Parser<object, B> configPathedParser<A1, A2, B>(
      string a1Key, Parser<object, A1> a1Parser,
      string a2Key, Parser<object, A2> a2Parser,
      Func<A1, A2, B> mapper
    ) =>
      configPathedParser(a1Key, a1Parser)
      .and(configPathedParser(a2Key, a2Parser), mapper);

    public static Parser<object, B> configPathedParser<A1, A2, A3, B>(
      string a1Key, Parser<object, A1> a1Parser,
      string a2Key, Parser<object, A2> a2Parser,
      string a3Key, Parser<object, A3> a3Parser,
      Func<A1, A2, A3, B> mapper
    ) =>
      configPathedParser(a1Key, a1Parser)
      .and(
        configPathedParser(a2Key, a2Parser),
        configPathedParser(a3Key, a3Parser),
        mapper
      );

    public static Parser<object, B> configPathedParser<A1, A2, A3, A4, B>(
      string a1Key, Parser<object, A1> a1Parser,
      string a2Key, Parser<object, A2> a2Parser,
      string a3Key, Parser<object, A3> a3Parser,
      string a4Key, Parser<object, A4> a4Parser,
      Func<A1, A2, A3, A4, B> mapper
    ) =>
      configPathedParser(a1Key, a1Parser)
      .and(
        configPathedParser(a2Key, a2Parser),
        configPathedParser(a3Key, a3Parser),
        configPathedParser(a4Key, a4Parser),
        mapper
      );

    public static Parser<A, A> idParser<A>() => (path, a) => a; 
    public static readonly Parser<object, string> stringParser = createCastParser<string>();
    public static readonly Parser<object, Guid> guidParser = 
      stringParser.flatMapTry((_, s) => new Guid(s));

    public static readonly Parser<object, Option<Guid>> optGuidParser = opt(guidParser);

    public static readonly Parser<object, int> intParser = (path, n) => {
      try {
        switch (n) {
          case ulong i: return (int)i;
          case long l: return (int)l;
          case uint u: return (int)u;
          case int i1: return i1;
        }
      }
      catch (OverflowException) {}
      return parseErrorEFor<int>(path, n);
    };

    /// <summary>Parses [100,24]</summary>
    public static readonly Parser<object, decimal> decimalParser =
      stringParser.flatMap((path, s) =>
        decimal.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var dec) 
          ? Either<ConfigLookupError, decimal>.Right(dec) 
          : parseErrorEFor<decimal>(path, s)
      );

    public static Parser<object, byte> byteParser =
      intParser.flatMap(i => i < 0 || i > byte.MaxValue ? None._ : Some.a((byte) i)); 

    public static readonly Parser<object, ushort> ushortParser = (path, n) => {
      try {
        switch (n) {
          case ulong u: return (ushort)u;
          case long l: return (ushort)l;
          case uint u1: return (ushort)u1;
          case int i: return (ushort)i;
          case ushort i: return i;
          case short i: return (ushort) i;
        }
      }
      catch (OverflowException) {}
      return parseErrorEFor<ushort>(path, n);
    };

    public static readonly Parser<object, short> shortParser = (path, n) => {
      try {
        switch (n) {
          case ulong u:  return (short)u;
          case long l:   return (short)l;
          case uint u1:  return (short)u1;
          case int i:    return (short)i;
          case ushort i: return (short)i;
          case short i:  return i;
        }
      }
      catch (OverflowException) {}
      return parseErrorEFor<short>(path, n);
    };

    [PublicAPI]
    public static readonly Parser<object, uint> uintParser = (path, n) => {
      try {
        switch (n) {
          case ulong u: return (uint)u;
          case long l: return (uint)l;
          case uint u1: return u1;
          case int i: return (uint)i;
        }
      }
      catch (OverflowException) {}
      return parseErrorEFor<uint>(path, n);
    };

    [PublicAPI]
    public static readonly Parser<object, long> longParser = (path, n) => {
      try {
        switch (n) {
          case ulong l: return (long)l;
          case long l1: return l1;
          case uint u: return u;
          case int i: return i;
        }
      }
      catch (OverflowException) {}
      return parseErrorEFor<long>(path, n);
    };

    [PublicAPI]
    public static readonly Parser<object, ulong> ulongParser = (path, n) => {
      try {
        switch (n) {
          case ulong num: return num;
          case long num: return (ulong) num;
          case uint num: return num;
          case int num: return (ulong) num;
        }
      }
      catch (OverflowException) { }
      return parseErrorEFor<ulong>(path, n);
    };

    [PublicAPI]
    public static readonly Parser<object, float> floatParser = (path, n) => {
      try {
        switch (n) {
          case double _: return (float) (double) n;
          case float _: return (float) n;
          case long _: return (long) n;
          case ulong _: return (ulong) n;
          case int _: return (int) n;
          case uint _: return (uint) n;
        }
      }
      catch (OverflowException) {}
      return parseErrorEFor<float>(path, n);
    };

    [PublicAPI] public static readonly Parser<object, float> floatFromStringParser =
      stringParser.flatMap((_, s) => {
        var success = float.TryParse(
          s, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out var v
        );
        return success ? Some.a(v) : None._;
      });

    [PublicAPI]
    public static readonly Parser<object, double> doubleParser = (path, n) => {
      try {
        if (n is double) return Either<ConfigLookupError, double>.Right((double) n);
        if (n is float) return Either<ConfigLookupError, double>.Right((float) n);
        if (n is long) return Either<ConfigLookupError, double>.Right((long) n);
        if (n is ulong) return Either<ConfigLookupError, double>.Right((ulong) n);
        if (n is int) return Either<ConfigLookupError, double>.Right((int) n);
        if (n is uint) return Either<ConfigLookupError, double>.Right((uint) n);
      }
      catch (OverflowException) { }
      return parseErrorEFor<double>(path, n);
    };

    [PublicAPI] public static readonly Parser<object, double> doubleFromStringParser =
      stringParser.flatMap((_, s) => {
        var success = double.TryParse(
          s, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out var v
        );
        return success ? Some.a(v) : None._;
      });

    [PublicAPI]
    public static readonly Parser<object, bool> boolParser = createCastParser<bool>();

    public static readonly Parser<object, DateTime> dateTimeParser =
      createCastParser<DateTime>()
      .or(stringParser.flatMap((path, s) => {
        var t = s.parseDateTime();
        return t.isRight
          ? Either<ConfigLookupError, DateTime>.Right(t.__unsafeGetRight)
          : parseErrorEFor<DateTime>(path, s, t.__unsafeGetLeft);
      }));

    public static Parser<object, R> rangeParser<A, R>(
      Parser<object, A> aParser, Func<A, A, R> lowerUpperToRange
    ) =>
      configPathedParser("lower", aParser)
      .and(configPathedParser("upper", aParser), lowerUpperToRange);

    public readonly struct ParserInput<Node> {
      public readonly ConfigPath path;
      public readonly Node node;

      public ParserInput(ConfigPath path, Node node) {
        this.path = path;
        this.node = node;
      }
    }
    
    public delegate ParserInput<From> ParseFromList<From>(int index);
    
    /// <summary>
    /// More than 4 generic parameters in <see cref="ConfigExts.tpl{From,A1,A2,A3,A4,C}"/> freak IL2CPP out,
    /// so use this.
    /// </summary>
    public static Parser<From, R> tpl<From, R>(
      Func<ParseFromList<From>, R> parse
    ) =>
      (path, node) => {
        if (node is List<From> list) {
          try {
            return parse(idx => new ParserInput<From>(path.indexed(idx), list.a(idx)));
          }
          catch (Exception e) {
            return parseErrorFor<R>(path, node, $"{list} failed with {e}");
          }
        }
        else {
          return parseErrorFor<List<From>>(path, node);
        }
      };

    [PublicAPI]
    public static Parser<object, A> mappingParser<A>(params KeyValuePair<object, A>[] mapping) {
      var dict = new Dictionary<object, A>();
      foreach (var kv in mapping) dict[kv.Key] = kv.Value;
      return (path, node) => dict.TryGetValue(node, out var a) ? a : parseErrorEFor<A>(path, node);
    }

    public static Parser<object, R> tplObj<R>(Func<ParseFromList<object>, R> parse) => tpl(parse);
    
    #endregion

    public ConfigPath scope { get; }

    readonly Dictionary<string, object> root;

    public Config(
      Dictionary<string, object> root, ConfigPath scope
    ) {
      this.scope = scope;
      this.root = root;
    }

    public Config(Dictionary<string, object> root) : this(root, ConfigPath.root) {}

    public ICollection<string> keys => root.Keys;

    #region Getters

    public A as_<A>(Parser<object, A> parser) =>
      e2a(eitherAs(parser));

    public A get<A>(string key, Parser<object, A> parser) =>
      e2a(internalGet(key, parser));

    static A e2a<A>(Either<ConfigLookupError, A> e) {
      if (e.isLeft) throw new ConfigFetchException(e.__unsafeGetLeft);
      return e.__unsafeGetRight;
    }

    public Option<A> optAs<A>(Parser<object, A> parser) =>
      eitherAs(parser).rightValue;

    public Option<A> optGet<A>(string key, Parser<object, A> parser) =>
      internalGet(key, parser).rightValue;

    public Either<ConfigLookupError, A> eitherAs<A>(Parser<object, A> parser) =>
      parser(scope, root);

    public Either<ConfigLookupError, A> eitherGet<A>(
      string key, Parser<object, A> parser
    ) => internalGet(key, parser);

    #endregion

    Either<ConfigLookupError, A> internalGet<A>(
      string key, Parser<object, A> parser, Dictionary<string, object> current = null
    ) {
      var path = scope / key;
      var parts = path.path;

      current = current ?? root;
      var toIdx = parts.Count - 1;
      for (var idx = 0; idx < toIdx; idx++) {
        var idxPart = parts[idx];
        var either = fetch(current, path, idxPart, jsClassParser);
        if (either.isLeft) return either.__unsafeCastRight<A>();
        current = either.__unsafeGetRight;
      }

      return fetch(current, path, parts[toIdx], parser);
    }

    Either<ConfigLookupError, A> fetch<A>(
      IDictionary<string, object> current, ConfigPath path, string part, Parser<object, A> parser
    ) {
      if (!current.ContainsKey(part))
        return ConfigLookupError.keyNotFound(ImmutableArray.Create(
          KV.a(nameof(part), part),
          KV.a(nameof(path), path.pathStrWithBase),
          KV.a(nameof(current), current.asDebugString()),
          KV.a(nameof(scope), scope.pathStrWithBase)
        ));

      var node = current[part];
      return parser(path, node);
    }

    public override string ToString() =>
      $"{nameof(Config)}({nameof(scope)}: \"{scope}\", {nameof(root)}: {root})";
  }
}