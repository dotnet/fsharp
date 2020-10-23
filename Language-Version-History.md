Features Added in F# Language Versions
====================

# [F# 4.7](https://docs.microsoft.com/en-us/dotnet/fsharp/whats-new/fsharp-47)

- Compiler support for `LangVersion`
- Implicit `yield`s
- No more required double underscore (wildcard identifier)
- Indentation relaxations for parameters passed to constructors and static methods

# [F# 4.6](https://docs.microsoft.com/en-us/dotnet/fsharp/whats-new/fsharp-46)

- Anonymous records
- `ValueOption` module functions

# [F# 4.5](https://docs.microsoft.com/en-us/dotnet/fsharp/whats-new/fsharp-45)

- Versioning alignment of binary, package, and language
- Support for `Span<'T>` and related types
- Ability to produce `byref` returns
- The `voidptr` type
- The `inref<'T>` and `outref<'T>` types to represent readonly and write-only `byref`s
- `IsByRefLike` structs
- `IsReadOnly` structs
- Extension method support for `byref<'T>`/`inref<'T>`/`outref<'T>`
- `match!` keyword in computation expressions
- Relaxed upcast with `yield` in F# sequence/list/array expressions
- Relaxed indentation with list and array expressions
- Enumeration cases emitted as public

# [F# 4.1](https://fsharp.org/specs/language-spec/4.1/FSharpSpec-4.1-latest.pdf)

- Struct tuples which inter-operate with C# tuples
- Struct annotations for Records
- Struct annotations for Single-case Discriminated Unions
- Underscores in numeric literals
- Caller info argument attributes
- Result type and some basic Result functions
- Mutually referential types and modules within the same file
- Implicit `Module` syntax on modules with shared name as type
- Byref returns, supporting consuming C# `ref`-returning methods
- Error message improvements
- Support for `fixed`

# [F# 4.0](https://fsharp.org/specs/language-spec/4.0/FSharpSpec-4.0-final.pdf)

- `printf` on unitized values
- Extension property initializers
- Non-null provided types
- Primary constructors as functions
- Static parameters for provided methods
- `printf` interpolation
- Extended `#if` grammar
- Multiple interface instantiations
- Optional type args
- Params dictionaries

# [F# 3.1](https://fsharp.org/specs/language-spec/3.1/FSharpSpec-3.1-final.pdf)

- Named union type fields
- Extensions to array slicing
- Type inference enhancements

# [F# 3.0](https://fsharp.org/specs/language-spec/3.0/FSharpSpec-3.0-final.pdf)

- Type providers
- LINQ query expressions
- CLIMutable attribute
- Triple-quoted strings
- Auto-properties
- Provided units-of-measure

# [F# 2.0](https://fsharp.org/specs/language-spec/2.0/FSharpSpec-2.0-April-2012.pdf)

- Active patterns
- Units of measure
- Sequence expressions
- Asynchronous programming
- Agent programming
- Extension members
- Named arguments
- Optional arguments
- Array slicing
- Quotations
- Native interoperability
- Computation expressions

# [F# 1.1](https://docs.microsoft.com/en-us/archive/blogs/dsyme/a-taste-of-whats-new-in-f-1-1)

- Interactive environment
- Object programming
- Encapsulation Extensions

# [F# 1.0](https://docs.microsoft.com/en-us/archive/blogs/dsyme/welcome-to-dons-f-blog)

- Discriminated unions
- Records
- Tuples
- Pattern matching
- Type abbreviations
- Object expressions
- Structs
- Signature files
- Imperative programming
- Modules (no functors)
- Nested modules
- .NET Interoperability
