using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Utilities.Combinator
{
    using CanTarget = And<And<InRange, IsAlive>, LineOfSight>;

    // ---------- Domain ----------
    class Health : MonoBehaviour
    {
        public int hp = 10;
    }

    [RequireComponent(typeof(Health))]
    class Enemy : MonoBehaviour
    {
        public Health health { get; private set; }

        void Awake() => health = GetComponent<Health>();
    }

    readonly struct TargetCtx
    {
        public readonly Vector3 origin;
        public readonly float r2;
        public readonly LayerMask los;

        public TargetCtx(Vector3 origin, float range, LayerMask los)
        {
            this.origin = origin;
            r2 = range * range;
            this.los = los;
        }
    }

    // ---------- Predicate interface ----------
    interface IPred
    {
        bool Test(Enemy e, in TargetCtx c);
    }

    // ---------- Leaves ----------
    readonly struct InRange : IPred
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Test(Enemy e, in TargetCtx c) => (e.transform.position - c.origin).sqrMagnitude <= c.r2;
    }

    readonly struct LineOfSight : IPred
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Test(Enemy e, in TargetCtx c) => !Physics.Linecast(c.origin, e.transform.position, c.los);
    }

    readonly struct IsAlive : IPred
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Test(Enemy e, in TargetCtx c) => e.health && e.health.hp > 0;
    }

    // ---------- Identities ----------
    readonly struct TruePred : IPred
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Test(Enemy e, in TargetCtx c) => true;

        public static readonly TruePred Instance = default;
    }

    readonly struct FalsePred : IPred
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Test(Enemy e, in TargetCtx c) => false;
    }

    // ---------- Combinators ----------
    readonly struct And<A, B> : IPred where A : struct, IPred where B : struct, IPred
    {
        public readonly A a;
        public readonly B b;

        public And(A a, B b)
        {
            this.a = a;
            this.b = b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Test(Enemy e, in TargetCtx c) => a.Test(e, in c) && b.Test(e, in c);
    }

    readonly struct Or<A, B> : IPred where A : struct, IPred where B : struct, IPred
    {
        public readonly A a;
        public readonly B b;

        public Or(A a, B b)
        {
            this.a = a;
            this.b = b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Test(Enemy e, in TargetCtx c) => a.Test(e, in c) || b.Test(e, in c);
    }

    readonly struct Not<A> : IPred where A : struct, IPred
    {
        public readonly A a;

        public Not(A a)
        {
            this.a = a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Test(Enemy e, in TargetCtx c) => !a.Test(e, in c);
    }

    // ---------- Fluent builder ----------
    readonly struct Chain<TPred> where TPred : struct, IPred
    {
        public readonly TPred p;

        public Chain(TPred p)
        {
            this.p = p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Chain<And<TPred, TNext>> And<TNext>(TNext n) where TNext : struct, IPred => new(new And<TPred, TNext>(p, n));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Chain<Or<TPred, TNext>> Or<TNext>(TNext n) where TNext : struct, IPred => new(new Or<TPred, TNext>(p, n));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Chain<Not<TPred>> Not() => new(new Not<TPred>(p));

        // Grouping helpers for parentheses: build sub-chain then combine
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Chain<And<TPred, TSub>> AndGroup<TSub>(Func<Chain<TruePred>, Chain<TSub>> g) where TSub : struct, IPred
            => new(new And<TPred, TSub>(p, g(PredChain.All()).Build()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Chain<Or<TPred, TSub>> OrGroup<TSub>(Func<Chain<TruePred>, Chain<TSub>> g) where TSub : struct, IPred
            => new(new Or<TPred, TSub>(p, g(PredChain.All()).Build()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TPred Build() => p;
    }

    static class PredChain
    {
        // Start with a leaf
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Chain<TLeaf> Start<TLeaf>(TLeaf leaf) where TLeaf : struct, IPred => new(leaf);

        // Identities
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Chain<TruePred> All() => new(TruePred.Instance); // AND identity

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Chain<FalsePred> Any() => new(new FalsePred()); // OR  identity

        // Convenience overloads
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Chain<T1> All<T1>(T1 a) where T1 : struct, IPred => new(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Chain<And<T1, T2>> All<T1, T2>(T1 a, T2 b) where T1 : struct, IPred where T2 : struct, IPred
            => new(new And<T1, T2>(a, b));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Chain<And<And<T1, T2>, T3>> All<T1, T2, T3>(T1 a, T2 b, T3 c)
            where T1 : struct, IPred
            where T2 : struct, IPred
            where T3 : struct, IPred
            => new(new And<And<T1, T2>, T3>(new And<T1, T2>(a, b), c));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Chain<Or<T1, T2>> Any<T1, T2>(T1 a, T2 b) where T1 : struct, IPred where T2 : struct, IPred
            => new(new Or<T1, T2>(a, b));
    }

    // ---------- Demo ----------
    class Targeting : MonoBehaviour
    {
        public Transform origin;
        public float range = 12f;
        public LayerMask occluders;
        public List<Enemy> enemies = new();
        CanTarget pred;

        void Awake()
        {
            // Cheap → expensive: distance, alive, then physics LOS
            pred = PredChain.All(new InRange(), new IsAlive(), new LineOfSight()).Build();

            var filterA = PredChain.Start(new IsAlive()).And(new InRange()).And(new LineOfSight()).Build();
            var filterB = PredChain.All(new InRange()).AndGroup(g => g.And(new IsAlive()).And(new LineOfSight()).Not()).Build(); // InRange && !(IsAlive && LOS)
            var filterC = PredChain.Start(new InRange()).Or(new LineOfSight()).And(new IsAlive()).Build(); // (InRange || LOS) && IsAlive
        }

        void Update()
        {
            var ctx = new TargetCtx(origin.position, range, occluders);

            for (int i = 0; i < enemies.Count; i++)
            {
                var e = enemies[i];
                if (!e) continue;
                if (pred.Test(e, in ctx)) Debug.Log(e.name);
            }
        }
    }
}