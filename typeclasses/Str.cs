using System;
using System.Globalization;
using JetBrains.Annotations;

namespace pzd.lib.typeclasses {
  /// <summary>
  /// Defines A -> string conversion.
  /// </summary>
  [PublicAPI] public interface Str<in A> {
    string asString(A a);
  }

  /// <summary>
  /// When you can implement this in your classes if they are convertible to string.
  /// </summary>
  [PublicAPI] public interface IStr {
    string asString();
  }

  /// <summary>
  /// For convenience use:
  /// 
  /// using static pzd.lib.typeclasses.Str;
  /// </summary>
  [PublicAPI] public static class Str {
    public static string s(byte v) => v.ToString();
    public static string s(short v) => v.ToString();
    public static string s(ushort v) => v.ToString();
    public static string s(int v) => v.ToString();
    public static string s(uint v) => v.ToString();
    public static string s(long v) => v.ToString();
    public static string s(ulong v) => v.ToString();
    public static string s(float v) => v.ToString(CultureInfo.InvariantCulture);
    public static string s(double v) => v.ToString(CultureInfo.InvariantCulture);
    public static string s(bool v) => v ? "true" : "false";
    public static string s(string v) => v;
    public static string s(DateTime v) => v.ToString("yyyy-MM-dd hh:mm:ss.fff zzz");
    public static string s(Uri v) => v.ToString();
    public static string s(Guid v) => v.ToString();
    public static string s<A>(A v) where A : IStr => v.asString();
    public static string s<A>(A a, Str<A> str) => str.asString(a);

    /** Easily define Str instances. */
    public class LambdaStr<A> : Str<A> {
      readonly Func<A, string> _asString;

      public LambdaStr(Func<A, string> s) { _asString = s; }

      public string asString(A a) => _asString(a);
    }
  }
}