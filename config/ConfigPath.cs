using System.Collections.Immutable;
using JetBrains.Annotations;
using pzd.lib.exts;
using pzd.lib.functional;

namespace pzd.lib.config {
  /** Representation of configuration path. */
  [PublicAPI] public struct ConfigPath {
    public const char SEPARATOR = '.';
    public static ConfigPath root = new ConfigPath(ImmutableList<string>.Empty);

    public readonly ImmutableList<string> path;
    public readonly Option<string> basedFrom;

    public ConfigPath(ImmutableList<string> path, Option<string> basedFrom) {
      this.path = path;
      this.basedFrom = basedFrom;
    }

    ConfigPath(ImmutableList<string> path) : this(path, None._) {}

    public ConfigPath baseOn(ConfigPath basePath) =>
      new ConfigPath(path, Some.a(basePath.pathStrWithBase));

    public bool isRoot => path.isEmpty();

    public string pathStr => path.mkString(SEPARATOR);

    public string pathStrWithBase { get {
      var basedS = basedFrom.isSome ? $"({basedFrom.__unsafeGet})." : "";
      return $"{basedS}{pathStr}";
    } }

    public override string ToString() => $"{nameof(ConfigPath)}[{pathStrWithBase}]";

    public static ConfigPath operator /(ConfigPath s1, string s2) =>
      new ConfigPath(s1.path.AddRange(s2.Split(SEPARATOR)), s1.basedFrom);

    public ConfigPath indexed(int idx) => this / $"[{idx}]";

    public ConfigPath keyed(string key) => this / $"[key={key}]";
  }
}