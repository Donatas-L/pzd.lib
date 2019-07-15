using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;
using pzd.lib.equality;

namespace pzd.lib.functional {
  // Generic classes can't have explicit layouts.
  [PublicAPI] public struct Either<A, B> : IEquatable<Either<A, B>> {
    public readonly bool isLeft;
    public readonly A __unsafeGetLeft;
    public readonly B __unsafeGetRight;
    
    public bool isRight => !isLeft;
    
    public override string ToString() =>
      isLeft ? $"Left({__unsafeGetLeft})" : $"Right({__unsafeGetRight})";

    #region Creation
    
    public Either(A a) {
      isLeft = true;
      __unsafeGetRight = default;
      __unsafeGetLeft = a;
    }
    
    /// <summary>Useful when A and B are the same type.</summary>
    public static Either<A, B> Left(A a) => new Either<A, B>(a);

    public Either(B b) {
      isLeft = false;
      __unsafeGetLeft = default;
      __unsafeGetRight = b;
    }
    
    /// <summary>Useful when A and B are the same type.</summary>
    public static Either<A, B> Right(B a) => new Either<A, B>(a);
    
    public static implicit operator Either<A, B>(A a) => new Either<A, B>(a); 
    public static implicit operator Either<A, B>(B b) => new Either<A, B>(b); 
    
    #endregion

    #region Equality

    public bool Equals(Either<A, B> other) {
      if (isLeft && other.isLeft) return EqCmp<A>.Default.Equals(__unsafeGetLeft, other.__unsafeGetLeft);
      else if (isRight && other.isRight) return EqCmp<B>.Default.Equals(__unsafeGetRight, other.__unsafeGetRight);
      else return false;
    }

    public override bool Equals(object obj) => obj is Either<A, B> other && Equals(other);

    public override int GetHashCode() {
      unchecked {
        var hashCode = isLeft.GetHashCode();
        hashCode = (hashCode * 397) ^ EqualityComparer<A>.Default.GetHashCode(__unsafeGetLeft);
        hashCode = (hashCode * 397) ^ EqualityComparer<B>.Default.GetHashCode(__unsafeGetRight);
        return hashCode;
      }
    }

    public static bool operator ==(Either<A, B> left, Either<A, B> right) => left.Equals(right);
    public static bool operator !=(Either<A, B> left, Either<A, B> right) => !left.Equals(right);

    #endregion
    
    #region Getting values out
    
    public EitherEnumerator<A, B> GetEnumerator() => new EitherEnumerator<A, B>(this);
    
    public bool leftValueOut(out A a) {
      a = isLeft ? __unsafeGetLeft : default;
      return isLeft;
    }
    
    public bool rightValueOut(out B b) {
      b = isRight ? __unsafeGetRight : default;
      return isRight;
    }
    
    public A leftOrThrow { get {
      if (isLeft) return __unsafeGetLeft;
      throw new WrongEitherSideException($"Expected to have Left({typeof(A)}), but had {this}.");
    } }

    public B rightOrThrow { get {
      if (isRight) return __unsafeGetRight;
      throw new WrongEitherSideException($"Expected to have Right({typeof(B)}), but had {this}.");
    } }

    public Option<A> leftValue => isLeft ? Some.a(__unsafeGetLeft) : None._;
    public Option<B> rightValue => isLeft ? None._ : Some.a(__unsafeGetRight);
    
    public B getOrElse(B onLeft) => isLeft ? onLeft : __unsafeGetRight;
    public B getOrElse(Func<B> onLeft) => isLeft ? onLeft() : __unsafeGetRight;
    
    public void voidFold(Action<A> onLeft, Action<B> onRight) {
      if (isLeft) onLeft(__unsafeGetLeft);
      else onRight(__unsafeGetRight);
    }

    public C fold<C>(C onLeft, Func<B, C> onRight) =>
      isLeft ? onLeft : onRight(__unsafeGetRight);

    public C fold<C>(Func<A, C> onLeft, Func<B, C> onRight) =>
      isLeft ? onLeft(__unsafeGetLeft) : onRight(__unsafeGetRight);
    
    #endregion

    #region Transformations
    
    public Either<AA, B> mapLeft<AA>(Func<A, AA> mapper) =>
      isLeft ? new Either<AA, B>(mapper(__unsafeGetLeft)) : new Either<AA, B>(__unsafeGetRight);
    
    public Either<A, BB> mapRight<BB>(Func<B, BB> mapper) =>
      isLeft ? new Either<A, BB>(__unsafeGetLeft) : new Either<A, BB>(mapper(__unsafeGetRight));

    public Either<C, B> flatMapLeft<C>(Func<A, Either<C, B>> mapper) =>
      isLeft ? mapper(__unsafeGetLeft) : new Either<C, B>(__unsafeGetRight);

    public Either<A, C> flatMapRight<C>(Func<B, Either<A, C>> mapper) =>
      isLeft ? new Either<A, C>(__unsafeGetLeft) : mapper(__unsafeGetRight);

    public Either<A, BBB> flatMapRight<BB, BBB>(Func<B, Either<A, BB>> f, Func<B, BB, BBB> g) {
      if (isLeft) {
        return new Either<A, BBB>(__unsafeGetLeft);
      }
      else {
        var aOrBB = f(__unsafeGetRight);
        return aOrBB.isLeft 
          ? new Either<A, BBB>(aOrBB.__unsafeGetLeft) 
          : new Either<A, BBB>(g(__unsafeGetRight, aOrBB.__unsafeGetRight));
      }
    }
    
    /// <summary>Change type of left side, throwing exception if this Either is of left side.</summary>
    public Either<AA, B> __unsafeCastLeft<AA>() {
      if (isLeft) throw new WrongEitherSideException(
        $"Can't {nameof(__unsafeCastLeft)}, because this is {this}"
      );
      return new Either<AA, B>(__unsafeGetRight);
    }

    /// <summary>Change type of right side, throwing exception if this Either is of right side.</summary>
    public Either<A, BB> __unsafeCastRight<BB>() {
      if (isRight) throw new WrongEitherSideException(
        $"Can't {nameof(__unsafeCastRight)}, because this is {this}"
      );
      return new Either<A, BB>(__unsafeGetLeft);
    }

    #endregion
  }

  [PublicAPI] public static class EitherExts {
    public static Either<A, B> flatten<A, B>(this Either<A, Either<A, B>> e) =>
      e.flatMapRight(_ => _);
    
    public static Either<A, ImmutableList<B>> sequence<A, B>(
      this IEnumerable<Either<A, B>> enumerable
    ) {
      // mutable for performance
      var builder = ImmutableList.CreateBuilder<B>();
      foreach (var either in enumerable) {
        if (either.isLeft) return either.__unsafeGetLeft;
        builder.Add(either.__unsafeGetRight);
      }
      return builder.ToImmutable();
    }
    
    public static (ImmutableList<A>, ImmutableList<B>) separate<A, B>(
      this IEnumerable<Either<A, B>> enumerable
    ) {
      var aBuilder = ImmutableList.CreateBuilder<A>();
      var bBuilder = ImmutableList.CreateBuilder<B>();
      foreach (var either in enumerable) {
        if (either.isLeft) aBuilder.Add(either.__unsafeGetLeft);
        else bBuilder.Add(either.__unsafeGetRight);
      }
    
      return (aBuilder.ToImmutable(), bBuilder.ToImmutableList());
    }
  }

  public class WrongEitherSideException : Exception {
    public WrongEitherSideException(string message) : base(message) {}
  }
  
  [PublicAPI] public struct EitherEnumerator<A, B> {
    public readonly Either<A, B> either;
    bool read;

    public EitherEnumerator(Either<A, B> either) {
      this.either = either;
      read = false;
    }
  
    public bool MoveNext() => either.isRight && !read;
  
    public void Reset() => read = false;

    public B Current { get {
      read = true;
      return either.rightOrThrow;
    } }
  }
}