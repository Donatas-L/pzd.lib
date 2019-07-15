using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class UIntExts {
    public static int toIntClamped(this uint a) =>
      a > int.MaxValue ? int.MaxValue : (int) a;
  }
}