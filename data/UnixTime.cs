using System;
using System.Globalization;
using JetBrains.Annotations;
using pzd.lib.config;
using pzd.lib.exts;
using pzd.lib.functional;

namespace pzd.lib.data {
  [PublicAPI] public readonly partial struct UnixTime : IEquatable<UnixTime> {
    public readonly long secondsSinceEpoch;

    public UnixTime(long secondsSinceEpoch) { this.secondsSinceEpoch = secondsSinceEpoch; }
    public UnixTime(DateTime dt) : this(dt.toUnixTimestamp()) {}
    
    #region Equality

    public bool Equals(UnixTime other) => secondsSinceEpoch == other.secondsSinceEpoch;
    public override bool Equals(object obj) => obj is UnixTime other && Equals(other);
    public override int GetHashCode() => (int) secondsSinceEpoch;
    public static bool operator ==(UnixTime left, UnixTime right) => left.Equals(right);
    public static bool operator !=(UnixTime left, UnixTime right) => !left.Equals(right);

    #endregion
    
    public static readonly Config.Parser<object, UnixTime> parser = Config.uintParser.map(l => new UnixTime(l));
    public static readonly Config.Parser<object, Option<UnixTime>> parserOpt = Config.opt(parser);

    public static implicit operator long(UnixTime time) => time.secondsSinceEpoch;
    public static implicit operator DateTime(UnixTime time) => time.secondsSinceEpoch.fromUnixTimestampInSeconds();
    
    public static UnixTime operator +(UnixTime t, long seconds) => 
      new UnixTime(t.secondsSinceEpoch + seconds);
    
    public static UnixTime operator -(UnixTime t, long seconds) =>
      new UnixTime(t.secondsSinceEpoch - seconds);
    
    public static TimeSpan operator -(UnixTime a, UnixTime b) =>
      new TimeSpan(TimeSpan.TicksPerSecond * (a.secondsSinceEpoch - b.secondsSinceEpoch));

    public static UnixTime now => new UnixTime(DateTime.Now);

    public string asTimeAgoString() {
      var dateTime = (DateTime) this;
      try {
        return Humanizer.DateHumanizeExtensions.Humanize(dateTime, culture: getCulture());
      }
      catch (Exception) {
        // It was throwing exceptions on IL2CPP, because assembly was signed
        // Rebuilt Humanizer.dll without signing
        // https://forum.unity.com/threads/securityexception-only-on-il2cpp-and-not-mono-with-unity-2018-2-1f1.543509/
        return "";
      }

      CultureInfo getCulture() {
        try {
          return CultureInfo.GetCultureInfo("en-US");
        }
        catch (Exception) {
          return null;
        }
      }
    }
  }
}