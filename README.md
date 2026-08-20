# Utilities for Unity (`com.amaan.utilities`)

A comprehensive suite of production-ready Unity C# utilities, design patterns, helper methods, time & cursor controllers, and gameplay development tools.

## Installation

### Via Git URL
Open Unity Package Manager (`Window > Package Manager`), click `+`, select **Add package from git URL...**, and enter:
```
https://github.com/Amaan12/Utilities.git
```

## Features

- **Cursor Management**: Request-stack-based cursor controller (`Utilities.CursorControl`).
- **Time Management**: Smooth time scale tweening, pause, slow motion, and freeze frame tools (`Utilities.TimeControl`).
- **Design Patterns**:
  - **Combinator**: Predicate logic combination (`Utilities.Combinator`).
  - **Generic Processing Chains**: Fluent chaining and transformation pipelines (`Utilities.GenericProcessingChains`).
  - **IDisposables**: Clean `using`-statement resource cleanup for UI locking, time freeze, animations, and listeners (`Utilities.IDisposableUtils`).
  - **Object Pool**: Lightweight generic object pool (`Utilities.QuickPool`).
  - **Observer**: Observable value and list wrappers (`Utilities.Observer`).
  - **Registry**: Generic spatial and LINQ entity registry (`Utilities.SimpleRegistry`).
  - **Reorderable If Chains**: Reorderable condition-action runners (`Utilities.ReorderableIfChains`).
  - **Singleton**: Generic persistent/scene singleton base class (`Utilities.SimpleSingleton`).
  - **Spawner**: Strategy-based factory entity spawner (`Utilities.Platformer`).
  - **StateMachine**: Enum-based hierarchical state machine (`Utilities.SimpleStateMachine`).
  - **Unreal Blueprint Nodes**: Blueprint-style flow control nodes like `DoOnce`, `DoN`, `Gate`, `MultiGate`, `FlipFlop`, `Sequence` (`Utilities.BlueprintNodes`).
- **Utilities & Helpers**:
  - `Helpers.cs`, `MyDebug.cs`, `MyGizmos.cs`, `MyMathf.cs`, `EnumUtility.cs`, `SplinePoint.cs`.
- **Documentation**: Scene and GameObject in-editor documentation helpers (`Utilities.Documentation`).
- **Version Numbering**: Runtime and editor version display component (`Utilities.VersionNumbering`).
- **Dice & Coin**: Quick randomization helpers (`Utilities.Dice`, `Utilities.Coin`).
- **Popups**: Pop-up floating text manager (`Utilities.Popper`).

## Samples

Import sample scenes and example scripts from the Package Manager under the **Samples** tab.