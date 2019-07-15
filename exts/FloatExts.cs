using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class FloatExts {
    /// <summary>Re-maps a number from one range to another</summary>
    public static float remap(this float value, float from1, float to1, float from2, float to2) => 
      (value - from1) / (to1 - from1) * (to2 - from2) + from2;

    public static byte toByteClamped(this float number) {
      if (number > byte.MaxValue) return byte.MaxValue;
      if (number < byte.MinValue) return byte.MinValue;
      return (byte) number;
    }
    
    public static float remap01(this float value, float from1, float to1) =>
      value.remap(from1, to1, 0f, 1f);
  }
}