using System;
using JetBrains.Annotations;
using pzd.lib.exts;

namespace pzd.lib.data {
  /// <summary>
  /// Implementation of XORSHIFT, random number generation algorithm unity uses for Random.
  ///
  /// https://forum.unity3d.com/threads/which-random-number-generator-does-unity-use.148601/
  /// https://en.wikipedia.org/wiki/Xorshift
  ///
  /// This implementation uses xorshift* version.
  /// </summary>
  ///
  /// All ranges are inclusive
  [PublicAPI] public struct Rng {
    public struct Seed {
      public readonly ulong seed;

      public Seed(ulong seed) {
        // XORSHIFT does not work with 0 seeds.
        if (seed == 0) throw new ArgumentOutOfRangeException(nameof(seed), seed, "seed can't be 0!");
        this.seed = seed;
      }

      public bool isInitialized => seed != 0;

      public override string ToString() => $"{nameof(Seed)}({seed})";
    }

    public readonly Seed seed;

    public static Seed seedFrom(DateTime dt) => new Seed(unchecked((ulong) dt.Ticks));
    public static Seed nowSeed => seedFrom(DateTime.Now);
    public static Rng now => new Rng(nowSeed);

    public Rng(Seed seed) {
      this.seed = seed;
    }

    public Rng(DateTime seed) : this(seedFrom(seed)) {}

    public bool isInitialized => seed.isInitialized;

    public override string ToString() => $"{nameof(Rng)}({seed})";

    public (Rng, ulong) nextULongT { get {
      var res = nextULong(out var newState);
      return (newState, res);
    } }
    public static readonly Func<Rng, (Rng, ulong)> nextULongS = rng => rng.nextULongT;

    public ulong nextULong(out Rng newState) {
      var x = seed.seed;
      x ^= x >> 12; // a
      x ^= x << 25; // b
      x ^= x >> 27; // c
      newState = new Rng(new Seed(x));
      return x * 0x2545F4914F6CDD1D;
    }

    #region bool

    const ulong HALF_OF_ULONG = ulong.MaxValue / 2;
    static bool ulongToBool(ulong v) => v >= HALF_OF_ULONG;
    public (Rng, bool) nextBoolT => nextULongT.map2(ulongToBool);
    public bool nextBool(out Rng newState) => ulongToBool(nextULong(out newState));

    #endregion

    #region int

    static int ulongToInt(ulong v) => unchecked((int)v);
    public (Rng, int) nextIntT => nextULongT.map2(ulongToInt);
    public int nextInt(out Rng newState) => ulongToInt(nextULong(out newState));

    static int ulongToIntInRange(int from, int to, ulong v) =>
      from + (int) (v % (ulong) (to - from + 1));
    public (Rng, int) nextIntInRangeT(int from, int to) =>
      nextULongT.map2(v => ulongToIntInRange(from, to, v));
    public static Func<Rng, (Rng, int)> nextIntInRangeS(int from, int to) =>
      rng => rng.nextIntInRangeT(from, to);
    public int nextIntInRange(int from, int to, out Rng newState) =>
      ulongToIntInRange(from, to, nextULong(out newState));

    #endregion

    #region uint

    static uint ulongToUInt(ulong v) => unchecked((uint)v);
    public (Rng, uint) nextUIntT => nextULongT.map2(ulongToUInt);
    public static readonly Func<Rng, (Rng, uint)> nextUIntS = rng => rng.nextUIntT;
    public uint nextUInt(out Rng newState) => ulongToUInt(nextULong(out newState));

    static uint ulongToUIntInRange(uint from, uint to, ulong v) =>
      from + (uint)(v % (to - from + 1));
    public (Rng, uint) nextUIntInRangeT(uint from, uint to) =>
      nextUIntT.map2(v => ulongToUIntInRange(from, to, v));
    public static Func<Rng, (Rng, uint)> nextUIntInRangeS(uint from, uint to) =>
      rng => rng.nextUIntInRangeT(from, to);
    public uint nextUIntInRange(uint from, uint to, out Rng newState) =>
      ulongToUIntInRange(from, to, nextULong(out newState));

    #endregion

    #region float

    static float ulongToFloat(ulong v) => (float)v / ulong.MaxValue;
    public (Rng, float) nextFloatT => nextULongT.map2(ulongToFloat);
    /// <returns>0f to 1f</returns>
    public float nextFloat(out Rng newState) => ulongToFloat(nextULong(out newState));

    static float floatToFloatInRange(float from, float to, float v) =>
      from + (to - from) * v;
    public (Rng, float) nextFloatInRangeT(float from, float to) =>
      nextFloatT.map2(v => floatToFloatInRange(from, to, v));
    public float nextFloatInRange(float from, float to, out Rng newState) =>
      floatToFloatInRange(from, to, nextFloat(out newState));

    #endregion
  }
}