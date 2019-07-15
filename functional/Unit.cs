using System;
using JetBrains.Annotations;
using pzd.lib.serialization;
using pzd.lib.serialization.rws;

namespace pzd.lib.functional {
  [Serializable, PublicAPI] public struct Unit : IEquatable<Unit> {
    public static Unit _ { get; }
    public override string ToString() => "()";
    
    public static ISerializedRW<Unit> rw => UnitRW.instance;

    #region Equality

    public bool Equals(Unit other) => true;

    public override bool Equals(object obj) => obj is Unit;

    public override int GetHashCode() => 848053388; // just random numbers

    public static bool operator ==(Unit left, Unit right) => left.Equals(right);
    public static bool operator !=(Unit left, Unit right) => !left.Equals(right);

    #endregion
  }
}