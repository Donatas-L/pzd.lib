using System;
using pzd.lib.collection;
using pzd.lib.functional;

namespace pzd.lib.serialization.rws {
  class AndRW2<A1, A2, B> : ISerializedRW<B> {
    readonly ISerializedRW<A1> a1RW;
    readonly ISerializedRW<A2> a2RW;
    readonly Func<A1, A2, B> mapper;
    readonly Func<B, A1> getA1;
    readonly Func<B, A2> getA2;

    public AndRW2(
      ISerializedRW<A1> a1Rw, ISerializedRW<A2> a2Rw, Func<A1, A2, B> mapper, Func<B, A1> a1, Func<B, A2> a2
    ) {
      a1RW = a1Rw;
      a2RW = a2Rw;
      this.mapper = mapper;
      getA1 = a1;
      getA2 = a2;
    }

    public Either<string, DeserializeInfo<B>> deserialize(byte[] serialized, int startIndex) {
      try {
        var a1Opt = a1RW.deserialize(serialized, startIndex);
        if (a1Opt.leftValueOut(out var a1Err)) return $"{nameof(AndRW2<A1, A2, B>)} a1 failed: {a1Err}";
        var a1Info = a1Opt.__unsafeGetRight;
        var a2Opt = a2RW.deserialize(serialized, startIndex + a1Info.bytesRead);
        if (a2Opt.leftValueOut(out var a2Err)) return $"{nameof(AndRW2<A1, A2, B>)} a2 failed: {a2Err}";
        var a2Info = a2Opt.__unsafeGetRight;
        return new DeserializeInfo<B>(
          mapper(a1Info.value, a2Info.value),
          a1Info.bytesRead + a2Info.bytesRead
        );
      }
      catch (Exception e) {
        return $"{nameof(AndRW2<A1, A2, B>)} threw {e}";
      }
    }

    public Rope<byte> serialize(B b) =>
      a1RW.serialize(getA1(b)) + a2RW.serialize(getA2(b));
  }

  class AndRW3<A1, A2, A3, B> : ISerializedRW<B> {
    readonly ISerializedRW<A1> a1RW;
    readonly ISerializedRW<A2> a2RW;
    readonly ISerializedRW<A3> a3RW;
    readonly Func<A1, A2, A3, B> mapper;
    readonly Func<B, A1> getA1;
    readonly Func<B, A2> getA2;
    readonly Func<B, A3> getA3;

    public AndRW3(
      ISerializedRW<A1> a1Rw, ISerializedRW<A2> a2Rw, ISerializedRW<A3> a3Rw, Func<A1, A2, A3, B> mapper,
      Func<B, A1> a1, Func<B, A2> a2, Func<B, A3> a3
    ) {
      a1RW = a1Rw;
      a2RW = a2Rw;
      a3RW = a3Rw;
      this.mapper = mapper;
      getA1 = a1;
      getA2 = a2;
      getA3 = a3;
    }

    public Either<string, DeserializeInfo<B>> deserialize(byte[] serialized, int startIndex) {
      try {
        var a1Opt = a1RW.deserialize(serialized, startIndex);
        if (a1Opt.leftValueOut(out var a1Err)) return $"{nameof(AndRW3<A1, A2, A3, B>)} a1 failed: {a1Err}";
        var a1Info = a1Opt.__unsafeGetRight;
        var a2Opt = a2RW.deserialize(serialized, startIndex + a1Info.bytesRead);
        if (a2Opt.leftValueOut(out var a2Err)) return $"{nameof(AndRW3<A1, A2, A3, B>)} a2 failed: {a2Err}";
        var a2Info = a2Opt.__unsafeGetRight;
        var a3Opt = a3RW.deserialize(serialized, startIndex + a1Info.bytesRead + a2Info.bytesRead);
        if (a3Opt.leftValueOut(out var a3Err)) return $"{nameof(AndRW3<A1, A2, A3, B>)} a3 failed: {a3Err}";
        var a3Info = a3Opt.__unsafeGetRight;
        var info = new DeserializeInfo<B>(
          mapper(a1Info.value, a2Info.value, a3Info.value),
          a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead
        );
        return info;
      }
      catch (Exception e) {
        return $"{nameof(AndRW3<A1, A2, A3, B>)} threw {e}";
      }
    }

    public Rope<byte> serialize(B b) =>
      a1RW.serialize(getA1(b)) + a2RW.serialize(getA2(b)) + a3RW.serialize(getA3(b));
  }

  class AndRW4<A1, A2, A3, A4, B> : ISerializedRW<B> {
    readonly ISerializedRW<A1> a1RW;
    readonly ISerializedRW<A2> a2RW;
    readonly ISerializedRW<A3> a3RW;
    readonly ISerializedRW<A4> a4RW;
    readonly Func<A1, A2, A3, A4, B> mapper;
    readonly Func<B, A1> getA1;
    readonly Func<B, A2> getA2;
    readonly Func<B, A3> getA3;
    readonly Func<B, A4> getA4;

    public AndRW4(
      ISerializedRW<A1> a1Rw, ISerializedRW<A2> a2Rw, ISerializedRW<A3> a3Rw, ISerializedRW<A4> a4Rw,
      Func<A1, A2, A3, A4, B> mapper, Func<B, A1> a1, Func<B, A2> a2, Func<B, A3> a3, Func<B, A4> a4
    ) {
      a1RW = a1Rw;
      a2RW = a2Rw;
      a3RW = a3Rw;
      a4RW = a4Rw;
      this.mapper = mapper;
      getA1 = a1;
      getA2 = a2;
      getA3 = a3;
      getA4 = a4;
    }

    public Either<string, DeserializeInfo<B>> deserialize(byte[] serialized, int startIndex) {
      const string rwName = nameof(AndRW4<A1, A2, A3, A4, B>);
      var step = "a1";
      try {
        var a1Opt = a1RW.deserialize(serialized, startIndex);
        if (a1Opt.leftValueOut(out var a1Err)) return $"{rwName} a1 failed: {a1Err}";
        var a1Info = a1Opt.__unsafeGetRight;

        step = "a2";
        var a2Opt = a2RW.deserialize(serialized, startIndex + a1Info.bytesRead);
        if (a2Opt.leftValueOut(out var a2Err)) return $"{rwName} a2 failed: {a2Err}";
        var a2Info = a2Opt.__unsafeGetRight;

        step = "a3";
        var a3Opt = a3RW.deserialize(serialized, startIndex + a1Info.bytesRead + a2Info.bytesRead);
        if (a3Opt.leftValueOut(out var a3Err)) return $"{rwName} a3 failed: {a3Err}";
        var a3Info = a3Opt.__unsafeGetRight;

        step = "a4";
        var a4Opt = a4RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead
        );
        if (a4Opt.leftValueOut(out var a4Err)) return $"{rwName} a4 failed: {a4Err}";
        var a4Info = a4Opt.__unsafeGetRight;

        step = "mapper";
        var info = new DeserializeInfo<B>(
          mapper(a1Info.value, a2Info.value, a3Info.value, a4Info.value),
          a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead
        );
        return info;
      }
      catch (Exception e) {
        return $"{nameof(rwName)} at index {startIndex}, step {step} threw {e}";
      }
    }

    public Rope<byte> serialize(B b) =>
      a1RW.serialize(getA1(b))
      + a2RW.serialize(getA2(b))
      + a3RW.serialize(getA3(b))
      + a4RW.serialize(getA4(b));
  }

  class AndRW5<A1, A2, A3, A4, A5, B> : ISerializedRW<B> {
    readonly ISerializedRW<A1> a1RW;
    readonly ISerializedRW<A2> a2RW;
    readonly ISerializedRW<A3> a3RW;
    readonly ISerializedRW<A4> a4RW;
    readonly ISerializedRW<A5> a5RW;
    readonly Func<A1, A2, A3, A4, A5, B> mapper;
    readonly Func<B, A1> getA1;
    readonly Func<B, A2> getA2;
    readonly Func<B, A3> getA3;
    readonly Func<B, A4> getA4;
    readonly Func<B, A5> getA5;

    public AndRW5(
      ISerializedRW<A1> a1Rw, ISerializedRW<A2> a2Rw, ISerializedRW<A3> a3Rw, ISerializedRW<A4> a4Rw,
      ISerializedRW<A5> a5Rw, Func<A1, A2, A3, A4, A5, B> mapper, Func<B, A1> a1, Func<B, A2> a2, Func<B, A3> a3,
      Func<B, A4> a4, Func<B, A5> a5
    ) {
      a1RW = a1Rw;
      a2RW = a2Rw;
      a3RW = a3Rw;
      a4RW = a4Rw;
      a5RW = a5Rw;
      this.mapper = mapper;
      getA1 = a1;
      getA2 = a2;
      getA3 = a3;
      getA4 = a4;
      getA5 = a5;
    }

    public Either<string, DeserializeInfo<B>> deserialize(byte[] serialized, int startIndex) {
      const string rwName = nameof(AndRW5<A1, A2, A3, A4, A5, B>);
      var step = "a1";
      try {
        var a1Opt = a1RW.deserialize(serialized, startIndex);
        if (a1Opt.leftValueOut(out var a1Err)) return $"{rwName} a1 failed: {a1Err}";
        var a1Info = a1Opt.__unsafeGetRight;

        step = "a2";
        var a2Opt = a2RW.deserialize(serialized, startIndex + a1Info.bytesRead);
        if (a2Opt.leftValueOut(out var a2Err)) return $"{rwName} a2 failed: {a2Err}";
        var a2Info = a2Opt.__unsafeGetRight;

        step = "a3";
        var a3Opt = a3RW.deserialize(serialized, startIndex + a1Info.bytesRead + a2Info.bytesRead);
        if (a3Opt.leftValueOut(out var a3Err)) return $"{rwName} a3 failed: {a3Err}";
        var a3Info = a3Opt.__unsafeGetRight;

        step = "a4";
        var a4Opt = a4RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead
        );
        if (a4Opt.leftValueOut(out var a4Err)) return $"{rwName} a4 failed: {a4Err}";
        var a4Info = a4Opt.__unsafeGetRight;

        step = "a5";
        var a5Opt = a5RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead
        );
        if (a5Opt.leftValueOut(out var a5Err)) return $"{rwName} a5 failed: {a5Err}";
        var a5Info = a5Opt.__unsafeGetRight;

        step = "mapper";
        var info = new DeserializeInfo<B>(
          mapper(a1Info.value, a2Info.value, a3Info.value, a4Info.value, a5Info.value),
          a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead + a5Info.bytesRead
        );
        return info;
      }
      catch (Exception e) {
        return $"{rwName} at index {startIndex}, step {step} threw {e}";
      }
    }

    public Rope<byte> serialize(B b) =>
      a1RW.serialize(getA1(b))
      + a2RW.serialize(getA2(b))
      + a3RW.serialize(getA3(b))
      + a4RW.serialize(getA4(b))
      + a5RW.serialize(getA5(b));
  }

  class AndRW6<A1, A2, A3, A4, A5, A6, B> : ISerializedRW<B> {
    readonly ISerializedRW<A1> a1RW;
    readonly ISerializedRW<A2> a2RW;
    readonly ISerializedRW<A3> a3RW;
    readonly ISerializedRW<A4> a4RW;
    readonly ISerializedRW<A5> a5RW;
    readonly ISerializedRW<A6> a6RW;
    readonly Func<A1, A2, A3, A4, A5, A6, B> mapper;
    readonly Func<B, A1> getA1;
    readonly Func<B, A2> getA2;
    readonly Func<B, A3> getA3;
    readonly Func<B, A4> getA4;
    readonly Func<B, A5> getA5;
    readonly Func<B, A6> getA6;

    public AndRW6(
      ISerializedRW<A1> a1Rw, ISerializedRW<A2> a2Rw, ISerializedRW<A3> a3Rw, ISerializedRW<A4> a4Rw,
      ISerializedRW<A5> a5Rw, ISerializedRW<A6> a6Rw, Func<A1, A2, A3, A4, A5, A6, B> mapper, Func<B, A1> a1,
      Func<B, A2> a2, Func<B, A3> a3, Func<B, A4> a4, Func<B, A5> a5, Func<B, A6> a6
    ) {
      a1RW = a1Rw;
      a2RW = a2Rw;
      a3RW = a3Rw;
      a4RW = a4Rw;
      a5RW = a5Rw;
      a6RW = a6Rw;
      this.mapper = mapper;
      getA1 = a1;
      getA2 = a2;
      getA3 = a3;
      getA4 = a4;
      getA5 = a5;
      getA6 = a6;
    }

    public Either<string, DeserializeInfo<B>> deserialize(byte[] serialized, int startIndex) {
      const string rwName = nameof(AndRW6<A1, A2, A3, A4, A5, A6, B>);
      var step = "a1";
      try {
        var a1Opt = a1RW.deserialize(serialized, startIndex);
        if (a1Opt.leftValueOut(out var a1Err)) return $"{rwName} a1 failed: {a1Err}";
        var a1Info = a1Opt.__unsafeGetRight;

        step = "a2";
        var a2Opt = a2RW.deserialize(serialized, startIndex + a1Info.bytesRead);
        if (a2Opt.leftValueOut(out var a2Err)) return $"{rwName} a2 failed: {a2Err}";
        var a2Info = a2Opt.__unsafeGetRight;

        step = "a3";
        var a3Opt = a3RW.deserialize(serialized, startIndex + a1Info.bytesRead + a2Info.bytesRead);
        if (a3Opt.leftValueOut(out var a3Err)) return $"{rwName} a3 failed: {a3Err}";
        var a3Info = a3Opt.__unsafeGetRight;

        step = "a4";
        var a4Opt = a4RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead
        );
        if (a4Opt.leftValueOut(out var a4Err)) return $"{rwName} a4 failed: {a4Err}";
        var a4Info = a4Opt.__unsafeGetRight;

        step = "a5";
        var a5Opt = a5RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead
        );
        if (a5Opt.leftValueOut(out var a5Err)) return $"{rwName} a5 failed: {a5Err}";
        var a5Info = a5Opt.__unsafeGetRight;

        step = "a6";
        var a6Opt = a6RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead +
          a5Info.bytesRead
        );
        if (a6Opt.leftValueOut(out var a6Err)) return $"{rwName} a6 failed: {a6Err}";
        var a6Info = a6Opt.__unsafeGetRight;

        step = "mapper";
        var info = new DeserializeInfo<B>(
          mapper(a1Info.value, a2Info.value, a3Info.value, a4Info.value, a5Info.value, a6Info.value),
          a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead + a5Info.bytesRead +
          a6Info.bytesRead
        );
        return info;
      }
      catch (Exception e) {
        return $"{rwName} at index {startIndex}, step {step} threw {e}";
      }
    }

    public Rope<byte> serialize(B b) =>
      a1RW.serialize(getA1(b))
      + a2RW.serialize(getA2(b))
      + a3RW.serialize(getA3(b))
      + a4RW.serialize(getA4(b))
      + a5RW.serialize(getA5(b))
      + a6RW.serialize(getA6(b));
  }

  class AndRW7<A1, A2, A3, A4, A5, A6, A7, B> : ISerializedRW<B> {
    readonly ISerializedRW<A1> a1RW;
    readonly ISerializedRW<A2> a2RW;
    readonly ISerializedRW<A3> a3RW;
    readonly ISerializedRW<A4> a4RW;
    readonly ISerializedRW<A5> a5RW;
    readonly ISerializedRW<A6> a6RW;
    readonly ISerializedRW<A7> a7RW;
    readonly Func<A1, A2, A3, A4, A5, A6, A7, B> mapper;
    readonly Func<B, A1> getA1;
    readonly Func<B, A2> getA2;
    readonly Func<B, A3> getA3;
    readonly Func<B, A4> getA4;
    readonly Func<B, A5> getA5;
    readonly Func<B, A6> getA6;
    readonly Func<B, A7> getA7;

    public AndRW7(
      ISerializedRW<A1> a1Rw, ISerializedRW<A2> a2Rw, ISerializedRW<A3> a3Rw, ISerializedRW<A4> a4Rw,
      ISerializedRW<A5> a5Rw, ISerializedRW<A6> a6Rw, ISerializedRW<A7> a7Rw,
      Func<A1, A2, A3, A4, A5, A6, A7, B> mapper, Func<B, A1> a1, Func<B, A2> a2, Func<B, A3> a3, Func<B, A4> a4,
      Func<B, A5> a5, Func<B, A6> a6, Func<B, A7> a7
    ) {
      a1RW = a1Rw;
      a2RW = a2Rw;
      a3RW = a3Rw;
      a4RW = a4Rw;
      a5RW = a5Rw;
      a6RW = a6Rw;
      a7RW = a7Rw;
      this.mapper = mapper;
      getA1 = a1;
      getA2 = a2;
      getA3 = a3;
      getA4 = a4;
      getA5 = a5;
      getA6 = a6;
      getA7 = a7;
    }

    public Either<string, DeserializeInfo<B>> deserialize(byte[] serialized, int startIndex) {
      const string rwName = nameof(AndRW7<A1, A2, A3, A4, A5, A6, A7, B>);
      var step = "a1";
      try {
        var a1Opt = a1RW.deserialize(serialized, startIndex);
        if (a1Opt.leftValueOut(out var a1Err)) return $"{rwName} a1 failed: {a1Err}";
        var a1Info = a1Opt.__unsafeGetRight;

        step = "a2";
        var a2Opt = a2RW.deserialize(serialized, startIndex + a1Info.bytesRead);
        if (a2Opt.leftValueOut(out var a2Err)) return $"{rwName} a2 failed: {a2Err}";
        var a2Info = a2Opt.__unsafeGetRight;

        step = "a3";
        var a3Opt = a3RW.deserialize(serialized, startIndex + a1Info.bytesRead + a2Info.bytesRead);
        if (a3Opt.leftValueOut(out var a3Err)) return $"{rwName} a3 failed: {a3Err}";
        var a3Info = a3Opt.__unsafeGetRight;

        step = "a4";
        var a4Opt = a4RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead
        );
        if (a4Opt.leftValueOut(out var a4Err)) return $"{rwName} a4 failed: {a4Err}";
        var a4Info = a4Opt.__unsafeGetRight;

        step = "a5";
        var a5Opt = a5RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead
        );
        if (a5Opt.leftValueOut(out var a5Err)) return $"{rwName} a5 failed: {a5Err}";
        var a5Info = a5Opt.__unsafeGetRight;

        step = "a6";
        var a6Opt = a6RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead +
          a5Info.bytesRead
        );
        if (a6Opt.leftValueOut(out var a6Err)) return $"{rwName} a6 failed: {a6Err}";
        var a6Info = a6Opt.__unsafeGetRight;

        step = "a7";
        var a7Opt = a7RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead +
          a5Info.bytesRead + a6Info.bytesRead
        );
        if (a7Opt.leftValueOut(out var a7Err)) return $"{rwName} a7 failed: {a7Err}";
        var a7Info = a7Opt.__unsafeGetRight;

        step = "mapper";
        var info = new DeserializeInfo<B>(
          mapper(a1Info.value, a2Info.value, a3Info.value, a4Info.value, a5Info.value, a6Info.value, a7Info.value),
          a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead + a5Info.bytesRead +
          a6Info.bytesRead + a7Info.bytesRead
        );
        return info;
      }
      catch (Exception e) {
        return $"{rwName} at index {startIndex}, step {step} threw {e}";
      }
    }

    public Rope<byte> serialize(B b) =>
      a1RW.serialize(getA1(b))
      + a2RW.serialize(getA2(b))
      + a3RW.serialize(getA3(b))
      + a4RW.serialize(getA4(b))
      + a5RW.serialize(getA5(b))
      + a6RW.serialize(getA6(b))
      + a7RW.serialize(getA7(b));
  }

  class AndRW8<A1, A2, A3, A4, A5, A6, A7, A8, B> : ISerializedRW<B> {
    readonly ISerializedRW<A1> a1RW;
    readonly ISerializedRW<A2> a2RW;
    readonly ISerializedRW<A3> a3RW;
    readonly ISerializedRW<A4> a4RW;
    readonly ISerializedRW<A5> a5RW;
    readonly ISerializedRW<A6> a6RW;
    readonly ISerializedRW<A7> a7RW;
    readonly ISerializedRW<A8> a8RW;
    readonly Func<A1, A2, A3, A4, A5, A6, A7, A8, B> mapper;
    readonly Func<B, A1> getA1;
    readonly Func<B, A2> getA2;
    readonly Func<B, A3> getA3;
    readonly Func<B, A4> getA4;
    readonly Func<B, A5> getA5;
    readonly Func<B, A6> getA6;
    readonly Func<B, A7> getA7;
    readonly Func<B, A8> getA8;

    public AndRW8(
      ISerializedRW<A1> a1Rw, ISerializedRW<A2> a2Rw, ISerializedRW<A3> a3Rw, ISerializedRW<A4> a4Rw,
      ISerializedRW<A5> a5Rw, ISerializedRW<A6> a6Rw, ISerializedRW<A7> a7Rw, ISerializedRW<A8> a8Rw,
      Func<A1, A2, A3, A4, A5, A6, A7, A8, B> mapper, Func<B, A1> a1, Func<B, A2> a2, Func<B, A3> a3, Func<B, A4> a4,
      Func<B, A5> a5, Func<B, A6> a6, Func<B, A7> a7, Func<B, A8> a8
    ) {
      a1RW = a1Rw;
      a2RW = a2Rw;
      a3RW = a3Rw;
      a4RW = a4Rw;
      a5RW = a5Rw;
      a6RW = a6Rw;
      a7RW = a7Rw;
      a8RW = a8Rw;
      this.mapper = mapper;
      getA1 = a1;
      getA2 = a2;
      getA3 = a3;
      getA4 = a4;
      getA5 = a5;
      getA6 = a6;
      getA7 = a7;
      getA8 = a8;
    }

    public Either<string, DeserializeInfo<B>> deserialize(byte[] serialized, int startIndex) {
      const string rwName = nameof(AndRW7<A1, A2, A3, A4, A5, A6, A7, B>);
      var step = "a1";
      try {
        var a1Opt = a1RW.deserialize(serialized, startIndex);
        if (a1Opt.leftValueOut(out var a1Err)) return $"{rwName} a1 failed: {a1Err}";
        var a1Info = a1Opt.__unsafeGetRight;

        step = "a2";
        var a2Opt = a2RW.deserialize(serialized, startIndex + a1Info.bytesRead);
        if (a2Opt.leftValueOut(out var a2Err)) return $"{rwName} a2 failed: {a2Err}";
        var a2Info = a2Opt.__unsafeGetRight;

        step = "a3";
        var a3Opt = a3RW.deserialize(serialized, startIndex + a1Info.bytesRead + a2Info.bytesRead);
        if (a3Opt.leftValueOut(out var a3Err)) return $"{rwName} a3 failed: {a3Err}";
        var a3Info = a3Opt.__unsafeGetRight;

        step = "a4";
        var a4Opt = a4RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead
        );
        if (a4Opt.leftValueOut(out var a4Err)) return $"{rwName} a4 failed: {a4Err}";
        var a4Info = a4Opt.__unsafeGetRight;

        step = "a5";
        var a5Opt = a5RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead
        );
        if (a5Opt.leftValueOut(out var a5Err)) return $"{rwName} a5 failed: {a5Err}";
        var a5Info = a5Opt.__unsafeGetRight;

        step = "a6";
        var a6Opt = a6RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead +
          a5Info.bytesRead
        );
        if (a6Opt.leftValueOut(out var a6Err)) return $"{rwName} a6 failed: {a6Err}";
        var a6Info = a6Opt.__unsafeGetRight;

        step = "a7";
        var a7Opt = a7RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead +
          a5Info.bytesRead + a6Info.bytesRead
        );
        if (a7Opt.leftValueOut(out var a7Err)) return $"{rwName} a7 failed: {a7Err}";
        var a7Info = a7Opt.__unsafeGetRight;

        step = "a8";
        var a8Opt = a8RW.deserialize(
          serialized,
          startIndex + a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead +
          a5Info.bytesRead + a6Info.bytesRead + a7Info.bytesRead
        );
        if (a8Opt.leftValueOut(out var a8Err)) return $"{rwName} a8 failed: {a8Err}";
        var a8Info = a8Opt.__unsafeGetRight;

        step = "mapper";
        var info = new DeserializeInfo<B>(
          mapper(a1Info.value, a2Info.value, a3Info.value, a4Info.value, a5Info.value, a6Info.value, a7Info.value, a8Info.value),
          a1Info.bytesRead + a2Info.bytesRead + a3Info.bytesRead + a4Info.bytesRead + a5Info.bytesRead +
          a6Info.bytesRead + a7Info.bytesRead + a8Info.bytesRead
        );
        return info;
      }
      catch (Exception e) {
        return $"{rwName} at index {startIndex}, step {step} threw {e}";
      }
    }

    public Rope<byte> serialize(B b) =>
      a1RW.serialize(getA1(b))
      + a2RW.serialize(getA2(b))
      + a3RW.serialize(getA3(b))
      + a4RW.serialize(getA4(b))
      + a5RW.serialize(getA5(b))
      + a6RW.serialize(getA6(b))
      + a7RW.serialize(getA7(b))
      + a8RW.serialize(getA8(b));
  }
}