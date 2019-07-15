using System;
using JetBrains.Annotations;
using pzd.lib.functional;

namespace pzd.lib.exts {
  [PublicAPI] public struct CastBuilder<From> where From : class {
    public readonly From from;

    public CastBuilder(From from) { this.from = from; }

    public Either<string, To> toE<To>() where To : From {
      if (from is To to) return to;
      else return errorMsg<To>();
    }

    string errorMsg<To>() {
      var valueStr = from == null ? "<null>" : from.ToString();
      return $"Can't cast {typeof(From)} (value='{valueStr}') to {typeof(To)}";
    }

    public To to<To>() where To : class, From {
      if (!(from is To to)) throw new InvalidCastException(errorMsg<To>());
      return to;
    }
  }
}