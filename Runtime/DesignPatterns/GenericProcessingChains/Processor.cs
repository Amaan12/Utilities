using System;
using UnityEngine;

namespace Utilities.GenericProcessingChains
{
    public interface IProcessor<in TIn, out TOut>
    {
        TOut Process(TIn input);
    }

    public delegate TOut ProcessorDelegate<in TIn, out TOut>(TIn input);

    public class ThresholdFilter : IProcessor<float, bool>
    {
        readonly Func<float> getThreshold;

        public ThresholdFilter(Func<float> getThreshold)
        {
            this.getThreshold = getThreshold;
        }

        public bool Process(float score) => score >= getThreshold();
    }

    public class DistanceScorer : IProcessor<float, float>
    {
        public float Process(float distance) => 1f / (1f + distance);
    }

    public class DistanceFromPlayer : IProcessor<Vector3, float>
    {
        readonly Transform player;

        public DistanceFromPlayer(Transform player)
        {
            this.player = player;
        }

        public float Process(Vector3 point) => Vector3.Distance(player.position, point);
    }

    public class ClampByMaxDistance : IProcessor<float, float>
    {
        readonly float maxDistanceScoreThreshold;

        public ClampByMaxDistance(float maxDistance)
        {
            maxDistanceScoreThreshold = 1f / (1f + maxDistance);
        }

        public float Process(float score) => score < maxDistanceScoreThreshold ? 0f : score;
    }

    public class HighlightIfClose : IProcessor<bool, bool>
    {
        readonly Transform transform;

        public HighlightIfClose(Transform target) => transform = target;

        public bool Process(bool isCloseEnough)
        {
            // TODO cache propertyblock and null check
            var renderer = transform.GetComponent<Renderer>();
            renderer.material.color = isCloseEnough ? Color.red : Color.white;
            return isCloseEnough;
        }
    }

    class Combined<A, B, C> : IProcessor<A, C>
    {
        readonly IProcessor<A, B> first;
        readonly IProcessor<B, C> second;

        public Combined(IProcessor<A, B> first, IProcessor<B, C> second)
        {
            this.first = first;
            this.second = second;
        }

        public C Process(A input) => second.Process(first.Process(input));
    }

    class Chain<TIn, TOut>
    {
        readonly IProcessor<TIn, TOut> processor;

        Chain(IProcessor<TIn, TOut> processor)
        {
            this.processor = processor;
        }

        public static Chain<TIn, TOut> Start(IProcessor<TIn, TOut> processor)
        {
            return new Chain<TIn, TOut>(processor);
        }

        public Chain<TIn, TNext> Then<TNext>(IProcessor<TOut, TNext> next)
        {
            var combined = new Combined<TIn, TOut, TNext>(processor, next);
            return new Chain<TIn, TNext>(combined);
        }

        public TOut Run(TIn input) => processor.Process(input);
        public ProcessorDelegate<TIn, TOut> Compile() => input => processor.Process(input); // Turns the pipeline into a reusable function
    }

    // ──────────────────────────────────────────────────────────────
    // Advanced Fluent Chain API
    // ──────────────────────────────────────────────────────────────
    public static class Chain
    {
        public static DistanceChain FromPlayer(Transform player) => new DistanceChain(new DistanceFromPlayer(player));
        public static DistanceChain Start(DistanceFromPlayer processor) => new DistanceChain(processor);
        // public static DistanceChain Start<TProcessor>(TProcessor processor) where TProcessor : IProcessor<Vector3, float> => new DistanceChain(processor);
    }

    // ──────────────────────────────────────────────────────────────
    // Fluent Chain Base Class
    // Without TDerived in the base, C# would not allow a derived class
    // to add a method that returns a more specific type than the base returns.
    // ──────────────────────────────────────────────────────────────
    public abstract class FluentChain<TIn, TOut, TDerived> where TDerived : FluentChain<TIn, TOut, TDerived>
    {
        public IProcessor<TIn, TOut> processor;

        protected FluentChain(IProcessor<TIn, TOut> processor)
        {
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        protected TNextSelf Then<TNext, TNextSelf, TProcessor>(
            TProcessor nextProcessor,
            ChainFactory<TIn, TNext, TNextSelf> factory)
            where TNextSelf : FluentChain<TIn, TNext, TNextSelf>
            where TProcessor : class, IProcessor<TOut, TNext>
        {
            if (nextProcessor == null) throw new ArgumentNullException(nameof(nextProcessor));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            return factory(new Combined<TIn, TOut, TNext>(processor, nextProcessor));
        }

        public TOut Run(TIn input)
        {
            if (processor == null) throw new InvalidOperationException("Processor is not initialized. Use Chain.Start() to begin a chain.");
            return processor.Process(input);
        }

        public ProcessorDelegate<TIn, TOut> Compile()
        {
            if (processor == null) throw new InvalidOperationException("Processor is not initialized. Use Chain.Start() to begin a chain.");
            return input => processor.Process(input);
        }
    }

    // ──────────────────────────────────────────────────────────────
    // Factory delegate type for creating chain instances
    // ──────────────────────────────────────────────────────────────
    public delegate TChain ChainFactory<out TIn, in TOut, out TChain>(IProcessor<TIn, TOut> processor)
        where TChain : FluentChain<TIn, TOut, TChain>;

    // ──────────────────────────────────────────────────────────────
    // Concrete Chain Types
    // ──────────────────────────────────────────────────────────────
    public class DistanceChain : FluentChain<Vector3, float, DistanceChain>
    {
        public DistanceChain(IProcessor<Vector3, float> processor) : base(processor) { }

        static ScoredChain CreateScoredChain(IProcessor<Vector3, float> processor)
        {
            return new ScoredChain(processor);
        }

        public ScoredChain Then<TProcessor>(TProcessor scorer) where TProcessor : class, IProcessor<float, float>
            => Then<float, ScoredChain, TProcessor>(scorer, CreateScoredChain);
    }

    public class ScoredChain : FluentChain<Vector3, float, ScoredChain>
    {
        public ScoredChain(IProcessor<Vector3, float> processor) : base(processor) { }

        static FilteredChain CreateFilteredChain(IProcessor<Vector3, bool> processor)
        {
            return new FilteredChain(processor);
        }

        public ScoredChain WithMaxDistance(float maxDist)
        {
            processor = new Combined<Vector3, float, float>(processor, new ClampByMaxDistance(maxDist));
            return new ScoredChain(processor);
        }

        public FilteredChain Then<TProcessor>(TProcessor filter) where TProcessor : class, IProcessor<float, bool>
            => Then<bool, FilteredChain, TProcessor>(filter, CreateFilteredChain);
    }

    public class FilteredChain : FluentChain<Vector3, bool, FilteredChain>
    {
        public FilteredChain(IProcessor<Vector3, bool> processor) : base(processor) { }

        static FilteredChain Create(IProcessor<Vector3, bool> processor)
        {
            return new FilteredChain(processor);
        }

        public FilteredChain LogTo(string system)
        {
            Debug.Log($"#{system}# Filtered Chain!");
            return this;
        }

        public FilteredChain Then<TProcessor>(TProcessor next) where TProcessor : class, IProcessor<bool, bool>
            => Then<bool, FilteredChain, TProcessor>(next, Create);
    }

    public class Example : MonoBehaviour
    {
        ProcessorDelegate<Vector3, bool> shouldAttack;
        ProcessorDelegate<Vector3, bool> highlightClose;

        [Range(0.01f, 1f), SerializeField] float distanceThreshold = 0.1f;
        public Transform target;

        void Start()
        {
            highlightClose = Chain.Start(new DistanceFromPlayer(transform))
                .Then(new DistanceScorer())
                .Then(new ThresholdFilter(() => distanceThreshold))
                .Then(new HighlightIfClose(transform))
                .Compile();

            shouldAttack = Chain.Start(new DistanceFromPlayer(transform))
                .Then(new DistanceScorer())
                .WithMaxDistance(15f)
                .Then(new ThresholdFilter(() => distanceThreshold))
                .LogTo("Score")
                .LogTo("Proximity")
                .Compile();

            bool result = shouldAttack.Invoke(new Vector3(25, 0, 0));
            Debug.Log($"Should attack at (25, 0, 0)? {result}");

            var simpleChain = Chain<Vector3, float>.Start(new DistanceFromPlayer(transform))
                .Then(new DistanceScorer())
                .Then(new ThresholdFilter(() => distanceThreshold))
                .Compile();
        }
    }
}