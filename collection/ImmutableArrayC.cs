using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace pzd.lib.collection {
  [PublicAPI] public sealed class ImmutableArrayC<A> : IReadOnlyList<A> {
    readonly A[] array;

    public ImmutableArrayC(A[] array) { this.array = array; }

    public IEnumerator<A> GetEnumerator() { foreach (var a in array) yield return a; }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int Count => array.Length;
    public A this[int index] => array[index];
  }

  [PublicAPI] public static class ImmutableArrayC {
    public static ImmutableArrayC<A> create<A>(params A[] array) => new ImmutableArrayC<A>(array);
    
    public static ImmutableArrayC<A> move<A>(A[] array) => new ImmutableArrayC<A>(array);

    public static ImmutableArrayC<A> copy<A>(A[] array) {
      var a2 = new A[array.Length];
      array.CopyTo(a2, 0);
      return new ImmutableArrayC<A>(a2);
    }

    public static ImmutableArrayC<A> toImmutableArrayC<A>(this IEnumerable<A> enumerable) =>
      move(enumerable.ToArray());
  }
}