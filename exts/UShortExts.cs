using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class UShortExts {
    public static byte toByteClamped(this ushort v) => 
      v > byte.MaxValue ? byte.MaxValue : (byte) v;
  }
}