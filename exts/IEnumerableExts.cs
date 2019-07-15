using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace pzd.lib.exts {
  [PublicAPI] public static class IEnumerableExts {
    public static string mkString<A>(
      this IEnumerable<A> e, Action<StringBuilder> appendSeparator,
      string start = null, string end = null
    ) {
      var sb = new StringBuilder();
      if (start != null) sb.Append(start);
      var first = true;
      foreach (var a in e) {
        if (first) first = false;
        else appendSeparator(sb);
        sb.Append(a);
      }
      if (end != null) sb.Append(end);
      return sb.ToString();
    }
    
    public static string mkString<A>(
      this IEnumerable<A> e, string separator, string start = null, string end = null
    ) {
      if (separator.Contains("\0")) throwNullStringBuilderException();
      return e.mkString(sb => sb.Append(separator), start, end);
    }

    public static string mkString<A>(
      this IEnumerable<A> e, char separator, string start = null, string end = null
    ) {
      if (separator == '\0') throwNullStringBuilderException();
      return e.mkString(sb => sb.Append(separator), start, end);
    }

    public static void throwNullStringBuilderException() {
      // var sb = new StringBuilder();
      // sb.Append("foo");
      // sb.Append('\0');
      // sb.Append("bar");
      // sb.ToString() == "foobar" // -> false
      // sb.ToString() == "foo" // -> true
      throw new Exception(
        "Can't have null char in a separator due to a Mono runtime StringBuilder bug!"
      );
    }
    
    public static string mkStringEnum<A>(
      this IEnumerable<A> e, string separator = ", ", string start = "[", string end = "]"
    ) => e.mkString(separator, start, end);
    
    public static string mkStringEnumNewLines<A>(
      this IEnumerable<A> e, string separator = ",\n  ", string start = "[\n  ", string end = "\n]"
    ) => e.mkString(separator, start, end);
    
    /// <summary>
    /// Convert bytes to a hex string.
    /// </summary>
    public static string asHexString(this IEnumerable<byte> data) {
      var sb = new StringBuilder();

      foreach (var t in data) {
        sb.Append(Convert.ToString(t, 16).PadLeft(2, '0'));
      }

      return sb.ToString();
    }
    
    public static IOrderedEnumerable<A> OrderBySafe<A, B>(
      this IEnumerable<A> source, Func<A, B> keySelector
    ) where B : IComparable<B> => source.OrderBy(keySelector);

    public static IOrderedEnumerable<A> OrderByDescendingSafe<A, B>(
      this IEnumerable<A> source, Func<A, B> keySelector
    ) where B : IComparable<B> => source.OrderByDescending(keySelector);

    public static IOrderedEnumerable<A> ThenBySafe<A, B>(
      this IOrderedEnumerable<A> source, Func<A, B> keySelector
    ) where B : IComparable<B> => source.ThenBy(keySelector);
    
    
    /// <summary>
    /// This should really be used only for debugging. It is pretty slow.
    /// </summary>
    public static string asDebugString(
      this IEnumerable enumerable,
      bool newlines = true, bool fullClasses = false
    ) {
      if (enumerable == null) return "null";
      var sb = new StringBuilder();
      asStringRec(enumerable, sb, newlines, fullClasses, 1);
      return sb.ToString();
    }

    static void asStringRec(
      IEnumerable enumerable, StringBuilder sb,
      bool newlines, bool fullClasses, int indent = 0
    ) {
      var type = enumerable.GetType();
      sb.Append(fullClasses ? type.FullName : type.Name);
      sb.Append('[');

      var first = true;
      foreach (var item in enumerable) {
        if (!first) sb.Append(',');
        if (newlines) {
          sb.Append('\n');
          for (var idx = 0; idx < indent; idx++) sb.Append("  ");
        }
        else if (!first) sb.Append(' ');

        switch (item) {
          case string str:
            sb.Append(str);
            break;
          case IEnumerable enumItem:
            asStringRec(enumItem, sb, newlines, fullClasses, indent + 1);
            break;
          default:
            sb.Append(item);
            break;
        }
        first = false;
      }

      if (newlines) {
        sb.Append('\n');
        for (var idx = 0; idx < indent - 1; idx++) sb.Append("  ");
      }
      sb.Append(']');
    }
  }
}