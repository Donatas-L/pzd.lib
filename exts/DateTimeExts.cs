using System;
using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class DateTimeExts {
    public static DateTime UNIX_EPOCH_START = 
      new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static uint toUnixTimestamp(this DateTime dateTime) {
      if (dateTime < UNIX_EPOCH_START) throw new Exception($"{dateTime} < UNIX_EPOCH_START");
      return (uint) (dateTime.ToUniversalTime() - UNIX_EPOCH_START).TotalSeconds;
    }

    public static long toUnixTimestampInMilliseconds(this DateTime dateTime) {
      if (dateTime < UNIX_EPOCH_START) throw new Exception($"{dateTime} < UNIX_EPOCH_START");
      return (long) (dateTime.ToUniversalTime() - UNIX_EPOCH_START).TotalMilliseconds;
    }

    public static DateTime fromUnixTimestampInSeconds(this uint timestamp) => 
      UNIX_EPOCH_START.AddSeconds(timestamp);

    public static DateTime fromUnixTimestampInMilliseconds(this long timestamp) => 
      UNIX_EPOCH_START.AddMilliseconds(timestamp);

    public static int secondsFromNow(this DateTime d) =>
      (int)(DateTime.UtcNow - d.ToUniversalTime()).TotalSeconds;
  }
}