using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class Int32Exts {
    public static bool isValidIndex(this int i, int listLength) => i >= 0 && i < listLength;
    
    public static byte toByteClamped(this int number) {
      if (number > byte.MaxValue) return byte.MaxValue;
      if (number < byte.MinValue) return byte.MinValue;
      return (byte) number;
    }
    
    public static short toShortClamped(this int number) {
      if (number > short.MaxValue) return short.MaxValue;
      if (number < short.MinValue) return short.MinValue;
      return (short) number;
    }
    
    public static ushort toUShortClamped(this int number) {
      if (number > ushort.MaxValue) return ushort.MaxValue;
      if (number < ushort.MinValue) return ushort.MinValue;
      return (ushort) number;
    }
    
    public static uint toUIntClamped(this int a) => a < 0 ? 0u : (uint) a;
  }
}