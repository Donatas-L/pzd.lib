using JetBrains.Annotations;

namespace pzd.lib.collection {
  /// <summary>Allows sharing empty array instances.</summary>
  [PublicAPI] public static class EmptyArray<T> {
    public static readonly T[] _ = new T[0];
  }
}