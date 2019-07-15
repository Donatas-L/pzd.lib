using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using pzd.lib.functional;

namespace pzd.lib.config {
  /**
   * Config class that fetches JSON configuration from `url`. Contents of
   * `url` are expected to be a JSON object.
   *
   * Create one with `Config.apply(url)` or `new Config(json)`.
   *
   * Paths are specified in "key.subkey.subsubkey" format.
   *
   * You can specify references by giving value in format of '#REF=some.config.key#'.
   **/
  [PublicAPI] public interface IConfig {
    /* scope of the config, "" if root, "foo.bar.baz" if nested. */
    ConfigPath scope { get; }
    /** Immediate keys of this config object. */
    ICollection<string> keys { get; }

    /** Tries to parse current config object with given parser. */
    A as_<A>(Config.Parser<object, A> parser);
    Option<A> optAs<A>(Config.Parser<object, A> parser);
    Either<ConfigLookupError, A> eitherAs<A>(Config.Parser<object, A> parser);

    /** value if ok, ConfigFetchException if error. */
    A get<A>(string key, Config.Parser<object, A> parser);
    Option<A> optGet<A>(string key, Config.Parser<object, A> parser);
    Either<ConfigLookupError, A> eitherGet<A>(string key, Config.Parser<object, A> parser);
  }
  
  public class ConfigFetchException : Exception {
    public readonly ConfigLookupError error;

    public ConfigFetchException(ConfigLookupError error) : base(error.ToString())
    { this.error = error; }
  }
}