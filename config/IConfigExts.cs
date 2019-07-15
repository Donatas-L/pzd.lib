using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using pzd.lib.functional;
using static pzd.lib.config.Config;

namespace pzd.lib.config {
  [PublicAPI] public static class IConfigExts {
    #region getters

    public static object getObject(this IConfig cfg, string key) => cfg.get(key, objectParser);
    public static string getString(this IConfig cfg, string key) => cfg.get(key, stringParser);
    public static int getInt(this IConfig cfg, string key) => cfg.get(key, intParser);
    public static uint getUInt(this IConfig cfg, string key) => cfg.get(key, uintParser);
    public static long getLong(this IConfig cfg, string key) => cfg.get(key, longParser);
    public static ulong getULong(this IConfig cfg, string key) => cfg.get(key, ulongParser);
    public static float getFloat(this IConfig cfg, string key) => cfg.get(key, floatParser);
    public static double getDouble(this IConfig cfg, string key) => cfg.get(key, doubleParser);
    public static bool getBool(this IConfig cfg, string key) => cfg.get(key, boolParser);
    public static DateTime getDateTime(this IConfig cfg, string key) => cfg.get(key, dateTimeParser);
    public static IConfig getSubConfig(this IConfig cfg, string key) => cfg.get(key, configParser);
    public static IList<IConfig> getSubConfigList(this IConfig cfg, string key) => cfg.get(key, configListParser);

    #endregion

    /* Some(value) if found, None if not found or wrong type. */

    #region opt getters

    public static Option<List<A>> optList<A>(this IConfig cfg, string key, Parser<object, A> parser) =>
      cfg.eitherList(key, parser).rightValue;

    public static Option<Dictionary<K, V>> optDict<K, V>(
      this IConfig cfg, string key, Parser<string, K> keyParser, Parser<object, V> valueParser
    ) => cfg.eitherDict(key, keyParser, valueParser).rightValue;

    public static Option<object> optObject(this IConfig cfg, string key) => cfg.optGet(key, objectParser);
    public static Option<string> optString(this IConfig cfg, string key) => cfg.optGet(key, stringParser);
    public static Option<int> optInt(this IConfig cfg, string key) => cfg.optGet(key, intParser);
    public static Option<uint> optUInt(this IConfig cfg, string key) => cfg.optGet(key, uintParser);
    public static Option<long> optLong(this IConfig cfg, string key) => cfg.optGet(key, longParser);
    public static Option<ulong> optULong(this IConfig cfg, string key) => cfg.optGet(key, ulongParser);
    public static Option<float> optFloat(this IConfig cfg, string key) => cfg.optGet(key, floatParser);
    public static Option<double> optDouble(this IConfig cfg, string key) => cfg.optGet(key, doubleParser);
    public static Option<bool> optBool(this IConfig cfg, string key) => cfg.optGet(key, boolParser);
    public static Option<DateTime> optDateTime(this IConfig cfg, string key) => cfg.optGet(key, dateTimeParser);
    public static Option<IConfig> optSubConfig(this IConfig cfg, string key) => cfg.optGet(key, configParser);
    public static Option<List<IConfig>> optSubConfigList(this IConfig cfg, string key) => cfg.optList(key, configParser);

    #endregion

    #region either getters

    public static Either<ConfigLookupError, List<A>> eitherList<A>(
      this IConfig cfg, string key, Parser<object, A> parser
    ) => cfg.eitherGet(key, listParser(parser));

    public static Either<ConfigLookupError, Dictionary<K, V>> eitherDict<K, V>(
      this IConfig baseCfg, string key, Parser<string, K> keyParser, Parser<object, V> valueParser
    ) => baseCfg.eitherGet(key, dictParser(keyParser, valueParser));

    public static Either<ConfigLookupError, object> eitherObject(this IConfig cfg, string key) => cfg.eitherGet(key, objectParser);
    public static Either<ConfigLookupError, string> eitherString(this IConfig cfg, string key) => cfg.eitherGet(key, stringParser);
    public static Either<ConfigLookupError, int> eitherInt(this IConfig cfg, string key) => cfg.eitherGet(key, intParser);
    public static Either<ConfigLookupError, uint> eitherUInt(this IConfig cfg, string key) => cfg.eitherGet(key, uintParser);
    public static Either<ConfigLookupError, long> eitherLong(this IConfig cfg, string key) => cfg.eitherGet(key, longParser);
    public static Either<ConfigLookupError, ulong> eitherULong(this IConfig cfg, string key) => cfg.eitherGet(key, ulongParser);
    public static Either<ConfigLookupError, float> eitherFloat(this IConfig cfg, string key) => cfg.eitherGet(key, floatParser);
    public static Either<ConfigLookupError, double> eitherDouble(this IConfig cfg, string key) => cfg.eitherGet(key, doubleParser);
    public static Either<ConfigLookupError, bool> eitherBool(this IConfig cfg, string key) => cfg.eitherGet(key, boolParser);
    public static Either<ConfigLookupError, DateTime> eitherDateTime(this IConfig cfg, string key) => cfg.eitherGet(key, dateTimeParser);
    public static Either<ConfigLookupError, IConfig> eitherSubConfig(this IConfig cfg, string key) => cfg.eitherGet(key, configParser);
    public static Either<ConfigLookupError, List<IConfig>> eitherSubConfigList(this IConfig cfg, string key) => cfg.eitherList(key, configParser);

    #endregion
  }
}