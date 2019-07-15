using System;
using JetBrains.Annotations;
using pzd.lib.functional;

namespace pzd.lib.exts {
  [PublicAPI] public static class StringExts {
    public static bool isEmpty(this string s, bool trim = false) =>
      (trim ? s.Trim() : s).Length == 0;

    public static Either<string, DateTime> parseDateTime(this string str) {
      if (DateTime.TryParse(str, out var dt)) return dt;
      else return $"can't parse '{dt}' as datetime";
    }
  }
}