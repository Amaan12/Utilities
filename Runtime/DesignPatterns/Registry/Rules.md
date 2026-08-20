## Prompt

> Design a small, reusable C# registry library targeting modern .NET.
>
> ### Goals
>
> Create a common interface named `IRegistry<T>` and implement multiple registry types that share the same API but use different underlying collections.
>
> ### Requirements
>
> #### Interface
>
> Define a generic interface:
>
> ```csharp
> public interface IRegistry<T>
> ```
>
> It should expose only the core operations:
>
> * `bool Add(T item)`
> * `bool Remove(T item)`
> * `void Clear()`
> * `bool Contains(T item)`
> * `int Count`
> * `IEnumerable<T> All`
>
> Do **not** wrap LINQ methods like `Where`, `First`, `Any`, etc. Consumers should use LINQ directly on `All`.
>
> Keep the interface minimal.
>
> ---
>
> #### Implementations
>
> Implement the following classes:
>
> ### 1. HashsetRegistry<T>
>
> Uses `HashSet<T>` internally.
>
> Characteristics:
>
> * unique items
> * fast lookup
> * implements `IRegistry<T>`
>
> ---
>
> ### 2. ListRegistry<T>
>
> Uses `List<T>` internally.
>
> Characteristics:
>
> * preserves insertion order
> * allows duplicates
> * implements `IRegistry<T>`
>
> ---
>
> ### 3. DictionaryRegistry<TKey, TValue>
>
> Uses `Dictionary<TKey, TValue>`.
>
> Since this is keyed, define a separate interface:
>
> ```csharp
> public interface IDictionaryRegistry<TKey, TValue>
> ```
>
> with methods like:
>
> * `bool Add(TKey key, TValue value)`
> * `bool Remove(TKey key)`
> * `bool TryGet(TKey key, out TValue value)`
> * `bool ContainsKey(TKey key)`
> * `int Count`
> * `IEnumerable<KeyValuePair<TKey,TValue>> All`
>
> Do **not** try to force this into `IRegistry<T>`.
>
> ---
>
> ### 4. ObservableRegistry<T>
>
> Uses Cysharp's ObservableCollections package.
>
> Backed by an observable hash set (or the closest equivalent provided by the library).
>
> Requirements:
>
> * implements `IRegistry<T>`
> * exposes the observable collection so UI frameworks can subscribe
> * does not manually implement events
> * relies on ObservableCollections for notifications
>
> ---
>
> ### Design Requirements
>
> * Use nullable reference types.
> * Use expression-bodied members where appropriate.
> * Keep implementations concise.
> * Avoid unnecessary inheritance.
> * Prefer composition over abstract base classes.
> * Keep APIs idiomatic for modern C#.
> * Do not add thread safety.
> * Do not add LINQ wrapper methods.
> * Do not add selection strategies.
> * Do not add registration scopes.
> * Do not add dependency injection helpers.
> * Do not overengineer.
>
> The goal is to build a lightweight registry library that feels like the BCL: simple, composable, and minimal.

---

One suggestion I'd make is to **rename `IRegistry<T>.Add` to `TryAdd`**.

Why? `HashSet<T>.Add` returns `false` if the item already exists, while `List<T>.Add` always succeeds. Using `TryAdd` communicates that the operation *may* fail and keeps the semantics consistent across implementations:

```csharp
public interface IRegistry<T>
{
    bool TryAdd(T item);
    bool Remove(T item);
    bool Contains(T item);

    void Clear();

    int Count { get; }

    IEnumerable<T> All { get; }
}
```

Then:

* `HashSetRegistry<T>` returns `false` for duplicates.
* `ListRegistry<T>` always returns `true`.
* `ObservableHashSetRegistry<T>` mirrors `HashSetRegistry<T>`.

That gives every implementation the same contract without surprising behavior.
