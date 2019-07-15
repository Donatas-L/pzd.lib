using System;
using pzd.lib.collection;
using pzd.lib.exts;

namespace pzd.lib.serialization.rws {
  public class DateTimeRW : BaseRW<DateTime> {
    protected override DeserializeInfo<DateTime> tryDeserialize(byte[] serialized, int startIndex) =>
      SerializedRW.lng.deserialize(serialized, startIndex)
        .mapRight(di => di.map(DateTime.FromBinary)).rightOrThrow;

    public override Rope<byte> serialize(DateTime a) => SerializedRW.lng.serialize(a.ToBinary());
  }
  
  public class DateTimeMillisTimestampRW : BaseRW<DateTime> {
    protected override DeserializeInfo<DateTime> tryDeserialize(byte[] serialized, int startIndex) =>
      SerializedRW.lng.deserialize(serialized, startIndex)
        .mapRight(di => di.map(DateTimeExts.fromUnixTimestampInMilliseconds)).rightOrThrow;

    public override Rope<byte> serialize(DateTime a) => SerializedRW.lng.serialize(a.toUnixTimestampInMilliseconds());
  }
}