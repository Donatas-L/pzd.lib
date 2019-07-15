using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using pzd.lib.concurrent;
using pzd.lib.pools;

namespace pzd.lib.functional {
  [PublicAPI] public static class Lazy {
    public static LazyVal<A> a<A>(Func<A> create, Action<A> afterInitialization = null) =>
      new LazyValImpl<A>(create, afterInitialization);
  }
  
  [PublicAPI] public static class LazyValExts {
    /// <summary>
    /// Create a new lazy value B, based on lazy value A.
    ///
    /// Evaluating B will evaluate A, but evaluating A will not evaluate B.
    /// </summary>
    public static LazyVal<B> lazyMap<A, B>(this LazyVal<A> lazy, Func<A, B> mapper) =>
      Lazy.a(() => mapper(lazy.strict));

    /// <summary>
    /// Create a new lazy value B, based on lazy value A.
    ///
    /// Evaluating B will evaluate A and evaluating A will evaluate B.
    ///
    /// Projector function is called on every access, so make sure it is something lite,
    /// like a cast or field access.
    /// </summary>
    public static LazyVal<B> project<A, B>(this LazyVal<A> lazy, Func<A, B> projector) =>
      new ProjectedLazyVal<A, B>(lazy, projector);

    public static LazyVal<LessSpecific> upcast<MoreSpecific, LessSpecific>(
      this LazyVal<MoreSpecific> lazy
    ) where MoreSpecific : LessSpecific =>
      project(lazy, mst => (LessSpecific) mst);

    public static LazyVal<LessSpecific> upcast<MoreSpecific, LessSpecific>(
      this LazyVal<MoreSpecific> lazy, LessSpecific example
    ) where MoreSpecific : LessSpecific => lazy.upcast<MoreSpecific, LessSpecific>();

    public static A getOrElse<A>(this LazyVal<A> lazy, Func<A> orElse) =>
      lazy.isCompleted ? lazy.strict : orElse();

    public static A getOrElse<A>(this LazyVal<A> lazy, A orElse) =>
      lazy.isCompleted ? lazy.strict : orElse;

    public static A getOrNull<A>(this LazyVal<A> lazy) where A : class =>
      lazy.getOrElse((A) null);
  }

  // Not `Lazy<A>` because of `System.Lazy<A>`.
  public interface LazyVal<A> : IHeapFuture<A> {
    A strict { get; }
  }

  public class NotReallyLazyVal<A> : LazyVal<A> {
    public A strict { get; }

    public NotReallyLazyVal(A get) { strict = get; }

    #region Future
    public bool isCompleted => true;
    public Option<A> value => Some.a(strict);

    public bool valueOut(out A a) {
      a = strict;
      return true;
    }
    
    public void onComplete(Action<A> action) => action(strict);
    #endregion
  }

  public class LazyValImpl<A> : LazyVal<A> {
    static readonly Pool<List<Action<A>>> listenerPool = 
      new Pool<List<Action<A>>>(() => new List<Action<A>>(), _ => _.Clear());

    A obj;
    public bool isCompleted { get; private set; }
    readonly Func<A> initializer;
    readonly Action<A> maybeAfterInitialization;

    List<Action<A>> listeners;

    public LazyValImpl(Func<A> initializer, Action<A> afterInitialization = null) {
      this.initializer = initializer;
      maybeAfterInitialization = afterInitialization;
    }

    public A strict { get {
      if (! isCompleted) {
        obj = initializer();
        isCompleted = true;
        onValueInited(obj);
      }
      return obj;
    } }

    #region Future

    public Option<A> value => isCompleted ? Some.a(obj) : None._;

    public bool valueOut(out A a) {
      a = obj;
      return isCompleted;
    }

    public void onComplete(Action<A> action) {
      if (isCompleted) action(obj);
      else {
        listeners = listeners ?? listenerPool.Borrow();
        listeners.Add(action);
      }
    }

    #endregion

    void onValueInited(A a) {
      if (listeners != null) {
        foreach (var listener in listeners) listener(a);
        listenerPool.Release(listeners);
        listeners = null;
      }
      maybeAfterInitialization?.Invoke(a);
    }
  }

  internal class ProjectedLazyVal<A, B> : LazyVal<B> {
    readonly LazyVal<A> backing;
    readonly Func<A, B> projector;

    public ProjectedLazyVal(LazyVal<A> backing, Func<A, B> projector) {
      this.backing = backing;
      this.projector = projector;
    }

    public void onComplete(Action<B> action) => backing.onComplete(a => action(projector(a)));
    public Option<B> value => backing.value.map(projector);
    public bool isCompleted => backing.isCompleted;
    public B strict => projector(backing.strict);

    public bool valueOut(out B b) {
      if (backing.valueOut(out var a)) {
        b = projector(a);
        return true;
      }
      else {
        b = default;
        return false;
      }
    }
  }
}