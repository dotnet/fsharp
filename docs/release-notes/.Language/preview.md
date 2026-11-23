`FromEndSlicing` remains available only with `--langversion:preview`. It is not assigned a stable language version.

Features assigned to F# 11.2 are listed in [11.2](https://fsharp.github.io/fsharp-compiler-docs/release-notes/Language.html#11.2). New, unassigned language features belong here.

### Added

* .NET fields can solve SRTP property constraints, e.g. `(^T: (member Id: string) x)` reads the public field `Id` of a C# class or struct, and `(^T: (static member MaxValue: ^T) ())` reads a `const` field such as `Int32.MaxValue`. Fields are used only when no method, property or F# record field solves the constraint, but this can change which overload is picked, see "Changed" below. ([Language suggestion #1323](https://github.com/fsharp/fslang-suggestions/issues/1323), [PR #19417](https://github.com/dotnet/fsharp/pull/19417))
* Reads and writes of .NET fields declared `volatile` (for example in C#) are emitted with the `volatile.` prefix, as C# does. ([PR #19417](https://github.com/dotnet/fsharp/pull/19417))
* A property can be set through an SRTP setter constraint with `x.Prop <- v`, e.g. `let inline setX (x: 'T when 'T: (member X: int with set)) v = x.X <- v`, as `x.Prop` already reads it through a getter constraint. ([PR #19417](https://github.com/dotnet/fsharp/pull/19417))

### Fixed

### Changed

* Breaking change with `SupportILFieldsInSRTP`: an overload whose SRTP constraint was rejected before can now be chosen when a .NET field satisfies it. Code that relies on a fallback overload, like the `Default1`/`Default2` pattern of FSharpPlus, still compiles but can pick a different overload. For example `static member inline Get (_: ^t, _: Default1) : ^t = (^t : (static member MaxValue : ^t) ())` with a `Default2` fallback returning `Unchecked.defaultof<'t>` used to give `0` for `int`, and now gives `Int32.MaxValue`. ([PR #19417](https://github.com/dotnet/fsharp/pull/19417))
