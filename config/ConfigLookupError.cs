using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;
using pzd.lib.exts;
using pzd.lib.functional;

namespace pzd.lib.config {
  [PublicAPI] public struct ConfigLookupError {
    public enum Kind : byte { KEY_NOT_FOUND, WRONG_TYPE, EXCEPTION }

    public readonly Kind kind;
    public readonly ImmutableArray<KeyValuePair<string, string>> extras;
    public readonly Option<Exception> exception;

    ConfigLookupError(Kind kind, ImmutableArray<KeyValuePair<string, string>> extras) {
      this.kind = kind;
      this.extras = extras;
      exception = None._;
    }

    ConfigLookupError(Exception e) {
      kind = Kind.EXCEPTION;
      extras = ImmutableArray<KeyValuePair<string, string>>.Empty;
      exception = Some.a(e);
    }

    public static ConfigLookupError keyNotFound(ImmutableArray<KeyValuePair<string, string>> extras) =>
      new ConfigLookupError(Kind.KEY_NOT_FOUND, extras);

    public static ConfigLookupError wrongType(ImmutableArray<KeyValuePair<string, string>> extras) =>
      new ConfigLookupError(Kind.WRONG_TYPE, extras);

    public static ConfigLookupError fromException(Exception e) =>
      new ConfigLookupError(e);

    public override string ToString() => 
      $"{nameof(ConfigLookupError)}[" +
      $"{kind}, " +
      $"{(kind == Kind.EXCEPTION ? exception.__unsafeGet.ToString() : extras.mkStringEnum())}" +
      $"]";
  }
}