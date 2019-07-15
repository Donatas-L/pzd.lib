using System;
using pzd.lib.collection;

namespace pzd.lib.serialization.rws {
  public class boolRW : BaseRW<bool> {
    public const int LENGTH = 1;

    protected override DeserializeInfo<bool> tryDeserialize(byte[] serialized, int startIndex) =>
      new DeserializeInfo<bool>(BitConverter.ToBoolean(serialized, startIndex), LENGTH);

    public override Rope<byte> serialize(bool a) => Rope.a(BitConverter.GetBytes(a));
  }
  
  public class ushortRW : BaseRW<ushort> {
    public const int LENGTH = 2;

    protected override DeserializeInfo<ushort> tryDeserialize(byte[] serialized, int startIndex) =>
      new DeserializeInfo<ushort>(BitConverter.ToUInt16(serialized, startIndex), LENGTH);

    public override Rope<byte> serialize(ushort a) => Rope.a(BitConverter.GetBytes(a));
  }
  
  public class intRW : BaseRW<int> {
    public const int LENGTH = 4;

    protected override DeserializeInfo<int> tryDeserialize(byte[] serialized, int startIndex) =>
      new DeserializeInfo<int>(BitConverter.ToInt32(serialized, startIndex), LENGTH);

    public override Rope<byte> serialize(int a) => Rope.a(BitConverter.GetBytes(a));
  }
  
  public class uintRW : BaseRW<uint> {
    public const int LENGTH = 4;

    protected override DeserializeInfo<uint> tryDeserialize(byte[] serialized, int startIndex) =>
      new DeserializeInfo<uint>(BitConverter.ToUInt32(serialized, startIndex), LENGTH);

    public override Rope<byte> serialize(uint a) => Rope.a(BitConverter.GetBytes(a));
  }
  
  public class floatRW : BaseRW<float> {
    public const int LENGTH = 4;

    protected override DeserializeInfo<float> tryDeserialize(byte[] serialized, int startIndex) =>
      new DeserializeInfo<float>(BitConverter.ToSingle(serialized, startIndex), LENGTH);

    public override Rope<byte> serialize(float a) => Rope.a(BitConverter.GetBytes(a));
  }
  
  public class longRW : BaseRW<long> {
    public const int LENGTH = 8;

    protected override DeserializeInfo<long> tryDeserialize(byte[] serialized, int startIndex) =>
      new DeserializeInfo<long>(BitConverter.ToInt64(serialized, startIndex), LENGTH);

    public override Rope<byte> serialize(long a) => Rope.a(BitConverter.GetBytes(a));
  }
  
  public class ulongRW : BaseRW<ulong> {
    public const int LENGTH = 8;

    protected override DeserializeInfo<ulong> tryDeserialize(byte[] serialized, int startIndex) =>
      new DeserializeInfo<ulong>(BitConverter.ToUInt64(serialized, startIndex), LENGTH);

    public override Rope<byte> serialize(ulong a) => Rope.a(BitConverter.GetBytes(a));
  }
}