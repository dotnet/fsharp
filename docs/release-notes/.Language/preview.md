`FromEndSlicing` remains available only with `--langversion:preview`. It is not assigned a stable language version.

Features assigned to F# 11.2 are listed in [11.2](https://fsharp.github.io/fsharp-compiler-docs/release-notes/Language.html#11.2). New, unassigned language features belong here.

### Added

* .NET fields can solve SRTP property constraints, e.g. `(^T: (member Id: string) x)` reads the public field `Id` of a C# class or struct, and `(^T: (static member MaxValue: ^T) ())` reads a `const` field such as `Int32.MaxValue`. Fields are used only when no method, property or F# record field solves the constraint. ([Language suggestion #1323](https://github.com/fsharp/fslang-suggestions/issues/1323), [PR #19417](https://github.com/dotnet/fsharp/pull/19417))
* Reads and writes of .NET fields declared `volatile` (for example in C#) are emitted with the `volatile.` prefix, as C# does. ([PR #19417](https://github.com/dotnet/fsharp/pull/19417))
* A property can be set through an SRTP setter constraint with `x.Prop <- v`, e.g. `let inline setX (x: 'T when 'T: (member X: int with set)) v = x.X <- v`, as `x.Prop` already reads it through a getter constraint. ([PR #19417](https://github.com/dotnet/fsharp/pull/19417))

### Fixed

### Changed
