    Copyright (c) Microsoft Corporation.  All Rights Reserved.
    See License.txt in the project root for license information.

## About the release notes

We deliver F# and F# tools for Visual Studio and .NET Core releases. These can include bug fixes, new tooling features, new compiler features, performance improvements, infrastructure improvements, and new language versions. The most recent release of F# or any F# component will be at the top of this document.



### FSharpLanguage 5.0.0
* Fix () - Some bug fix [A Developer](https://github.com/Adeveloper).

### FSharp Core 5.0.0
* Fix () - Some bug fix [A Developer](https://github.com/Adeveloper).

### FSharp Tools 11.0.0
* Fix () - Some bug fix [A Developer](https://github.com/Adeveloper).

### FSharp Compiler Service 37.0.0
* Fix () - Some bug fix [A Developer](https://github.com/Adeveloper).



## Visual Studio 15.9

You can find all tracked VS 15.9 items in the [15.9 milestone](https://github.com/Microsoft/visualfsharp/milestone/24).

### F# Compiler

* Fix (#4637) - Can't debug FCS when compiled with portable pdb debug symbols, by [Jason Imison](https://github.com/nosami).
* Fix (#5355) - We fixed a bug where extension methods that take `byref` values could mutate an immutable value.
* Fix (#5446) - We improved the compile error information for overloads on `byref`/`inref`/`outref`, rather than displaying the previously obscure error.
* Fix (#5354) - Optional Type Extensions on `byref`s are now disallowed entirely. They could be declared previously, but were unusable, resulting in a confusing user experience.
* Fix (#5294) - We fixed a bug where `CompareTo` on a struct tuple and causing a type equivalence with an aliased struct tuple would result in a runtime exception.
* Fix (#5621) - We fixed a bug where use of `System.Void` in the context of authoring a Type Provider for .NET Standard could fail to find the `System.Void` type at design-time.
* Fix (#5468) - We fixed a bug where an internal error could occur when a partially applied Discriminated Union constructor is mismatched with an annotated or inferred type for the Discriminated Union.
* Fix (#5540) - We modified the compiler error message when attempting to take an address of an expression (such as accessing a property) to make it more clear that it violates scoping rules for `byref` types.
* Fix (#5536) - We fixed a bug where your program could crash at runtime when partially applying a `byref` type to a method or function. An error message will now display.
* Fix (#5459) - We fixed an issue where an invalid combination of a `byref` and a reference type (such as `byref<int> option`) would fail at runtime and not emit an error message. We now emit an error message.

### F# Tools for Visual Studio

* Fix (#5657) - We resolved an issue where metadata for F# assemblies built with the .NET Core SDK was not shown in file properties on Windows. You can now see this metadata by right-clicking an assembly on Windows and selecting **Properties**.
* Fix (#5615) - We fixed a bug where use of `module global` in F# source could cause Visual Studio to become unresponsive.
* Fix (#5515) - We fixed a bug where extension methods using `inref<'T>` would not show in completion lists.
* Fix (#5514) - We fixed a bug where the TargetFramework dropdown in Project Properties for .NET Framework F# projects was empty.
* Fix (#5507) - We fixed a bug where File | New Project on a .NET Framework 4.0 project would fail.

### F# OSS Build

* Feature (#5027) - Set VisualFSharpFull as the default startup project, by [Robert Jeppesen](https://github.com/rojepp).

## Visual Studio 15.8.5

* Fix (#5504) - Internal MSBuild Error when building non-.NET SDK projects with MSBuild parallelism
* Fix (#5518) - Visual Studio-deployed components are not NGEN'd
* Fix ([Devcom 322883](https://developercommunity.visualstudio.com/content/problem/322883/all-net-framework-f-projects-build-to-4500-regardl.html)) - FSharp.Core 4.5.0.0 binary is deployed to FSharp.Core 4.4.3.0 location

All other closed issues for the VS 15.8 release can be found [here](https://github.com/Microsoft/visualfsharp/milestone/14).

## F# 4.5

We introduced the F# language version 4.5 with this release. This also corresponds with the new 4.5.x family of FSharp.Core (the F# core library). You can read the specs for each of these changes in the [F# RFC repository](https://github.com/fsharp/fslang-design). There are also many improvements to F# tools for Visual Studio with this release.

### Releases

* Visual Studio 2017 update 15.8
* .NET Core SDK version 2.1.400 or higher

### Language features

* Support for `voidptr`
* `NativePtr.ofVoidPtr` and `NativePtr.toVoidPtr` support
* New types: `inref<'T>`, `outref<'T>` to represent read-only and write-only `byref`s
* Support for `IsByRefLike` structs
* Support for `IsReadOnly` structs
* Full support for production and consumption of `byref` returns
* Support for extension methods for `byref`/`inref`/`outref`
* Support for `match!` in computation expressions
* Relaxed upcast requirements for `yield` in sequence, list, and array expressions
* Relaxed indentation requirements for list and array expressions
* Enumeration cases emitted as public
* Various bug fixes with `byref` programming

### FSharp.Core features

* Version aligned to 4.5.x for the NuGet package and 4.5.0.0 for the binary
* Improved strack traces for `async { }` so that user code can now be seen
* Support for `ValueOption<'T>`
* Support for `TryGetValue` on Map

### Compiler improvements

Improvements to the F# compiler in addition to the previously-mentioned language features are in F# 4.5. These include:

* Restored ability to inherit from `FSharpFunc`
* Removed ~2.2% of all allocations in the F# compiler
* F# reference normalization support for user control of transitive assembly references written to an output file
* Respecting `WarningsNotAsErrors`
* Error message improvement when branches of a pattern match do not return the same type
* Respecting `#nowarn "2003"`
* Other smaller performance improvements and many bug fixes

### Tooling improvements

Significant improvements in the F# tools, such as performance enhancements and some new editor features are included this release. As always, with a large number of contributions from the F# open source community. Here are the highlights:

* We improved IntelliSense performance for .NET SDK-style projects of all forms, including those that use multi-targeting.
* A community-driven effort to analyze and improve IntelliSense performance for very large files was contributed by [Vasily Kirichenko](https://github.com/vasily-kirichenko),[ Steffen Forkmann](https://github.com/forki), and [Gauthier Segay](https://github.com/smoothdeveloper). IntelliSense in very large files (10k+ lines of code) is roughly twice as fast now.
* The warning for an outdated FSharp.Core (despite the package being installed) is no longer present in .NET SDK-style projects.
* The description tooltip that displays XML documentation for a member after . in IntelliSense no longer times out after 10 seconds.
* A bug where you could not set breakpoints in object constructor arguments has been fixed.
* A bug where a renamed symbol would be duplicated when it is a generic parameter has been fixed.
* Templates for .NET Framework (classic F# templates) now consume FSharp.Core from a NuGet package, to align with .NET SDK F# templates.
* Automatic, transactional brace completion is now available for `()`, `[]`, `{}`, `[||]`, and `[<>]` brace pairs. We did this work in collaboration with [Gibran Rosa](https://github.com/gibranrosa).
* You can now go to definition with **Ctrl + Click** on an F# symbol. The settings for this gesture are also respected in the **Tools > Options** window.
* The IntelliSense performance UI has been modified to allow configuration of stale typecheck information for various IDE features. Explanations for each option are now present in tooltips for the settings.
* Brace match highlighting now correctly highlights braces, completed in collaboration with [Vasily Kirichenko](https://github.com/vasily-kirichenko).
* Go to definition now navigates correctly when a type is defined recursively, contributed by [Vasily Kirichenko](https://github.com/vasily-kirichenko).
* A bug where an auto-imported namespace wasn't opened when the top of a file was empty has been fixed by [Vasily Kirichenko](https://github.com/vasily-kirichenko).
* A bug where `printf` specifiers that contained dots were miscolored has been fixed by [Vasily Kirichenko](https://github.com/vasily-kirichenko).
* A bug where all opens were considered unused inside of a recursive module has been fixed by [Vasily Kirichenko](https://github.com/vasily-kirichenko).
* Autocompletion for attributes now only suggests options that are actually attributes, contributed by [Vasily Kirichenko](https://github.com/vasily-kirichenko).
* Signature Help tooltips are now generated for Type Provider static parameters at the constructor call site, contributed by[Vasily Kirichenko](https://github.com/vasily-kirichenko).
* A bug where value types used as units of measure were colored as reference types has been fixed by [Vasily Kirichenko](https://github.com/vasily-kirichenko).
* A bug where semantic colorization could disappear for some files while scrolling has been fixed by  [Vasily Kirichenko](https://github.com/vasily-kirichenko).
There is now an experimental CodeLens implementation, contributed by [Victor Peter Rouven Müller](https://github.com/realvictorprm). You can turn it on in **Options > Text Editor > F# > Code Lens**.
* A bug where the F# compiler service would incorrectly elide the module names in XML documentation has been fixed by [Sebastian Urban](https://github.com/surban).
* Code that uses `Dictionary` with `ContainsKey` and subsequent `Item` calls has been changed to use `TryGetValue`, by [Eugene Auduchinok](https://github.com/auduchinok).
* [Jakob Majoka](https://github.com/majocha) also contributed in the process of consuming a different API for Tooltips.

### Infrastructure, Packaging, and Open Source Improvements

We made the following enhancements to infrastructure, packaging, and our open source contribution experience:

* The F# compiler distributed with Visual Studio no longer installs as a singleton in the F# Compiler SDK location. It is now fully side-by-side with Visual Studio, meaning that side-by-side installations of Visual Studio wil finally have truly side-by-side F# tooling and language experiences.
* The FSharp.Core NuGet package is now signed.
* ETW logging has been added to the F# tools and compiler.
* The very large `control.fs`/`control.fsi` files in FSharp.Core have been split into `async.fs`/`async.fsi`, `event.fs`/`event.fsi`, `eventmodule.fs`/`eventmodule.fsi`, `mailbox.fs`/`mailbox.fsi`, and `observable.fs`/`observable.fsi`.
* We added .NET SDK-style versions of our project performance stress test artifacts.
* We removed Newtonsoft.json from our codebase, and you now have one less package downloaded for OSS contributors.
* We now use the latest versions of System.Collections.Immutable and System.Reflection.Metadata.

## F# 4.1

### Releases

* Visual Studio 2017 updates 15.0 to 15.8 (exclusive)
* .NET Core SDK versions 1.0 to 2.1.400 (exclusive)

### Language and Core Library features

* Struct tuples
* Initial support for consuming C#-style `ref` returns
* Struct record support with the `[<Struct>]` attribute
* Struct Discriminated Union support with the `[<Struct>]` attribute
* `Result<'TSuccess, 'TFailure>` type, with supporting functions in FSharp.Core
* Support for the `fixed` keyword to pin a pointer-tyle local to the stack
* Underscores in numeric literals
* Caller Info Attribute Argument support
* `namespace rec` and `module rec` to support mutually referential types and functions within the same file
* Implicit "Module" suffix added to modules that share the same name as a type
* Tuple equality for F# tuples and `System.Tuple`

### Compiler improvements

* Support for generating Portable PDBs
* Significant improvements to error messages, particularly to aid with suggestions
* Performance improvements
* Interoperability improvements
* Support for geenerating F# AssymblyInfo from properties for .NET SDK projects
* `--debug:full` support for F# on .NET Core on Windows
* `MakeTuple` support for struct tuples
* Warnings are forwarded when searching for method overloads
* Support for emitting an enum-specific warning when pattern matching over one
* Many smaller bug fixes

### FSharp.Core features

* Support for `NativePtr.ByRef`
* Support for `Async.StartImmediateAsTask`
* Support for `Seq.transpose`/`Array.transpose`/`List.transpose`
* `IsSerializable` support for `Option` and `Async<'T>`
* Many smaller bug fixes

### IDE features for F# tools in Visual Studio

Most items here contributed by community members.

* Default installation of F# wherever .NET Core is installed
* Significant memory reductions in F# tooling
* IntelliSense Filters and Glyphs
* Support for Go to All
* Find all Reference support
* In-memory cross-project references support
* QuickInfo supports type colorization
* QuickInfo supports navigable links that will invoke Go to Definition
* Inline Rename support
* Go to Definition from F# to C# support
* Semantic document highlighting for selected symbols
* Support for Structured Guidelines and code outlining, which is toggleable
* Support for `EditorBrowsable(EditorBrowsableState.Never)`
* Code fix for making Record and Discriminated Union case lables upper-case
* Code fix to make suggestions for an unkown identifier
* Code fix for prefixing or replacing an unused value with an underscore
* Code fix to add the `new` keyword to a disposable type
* Code fix to add an `open` statement at the top for a symbol coming from an unopened namespace or module
* Code fix to simplify a name by removing unnecessary namespace qualifiers
* Graying out unused values in the editor
* Colorized `fsi.exe` when ran as a standalone console application
* Autocompletion support, including symbols from unopened namespaces
* Colorization for mutable values to distinguish them from immutable values
* Support for move up/down on solution folder nodes
* Support for Blue (High Contrast) theming
* Full support for .NET Core and .NET Standard projects, with the ability to create new ASP.NET Core projects in F#
* Full support for ASP.NET Web SDK tooling, such as Azure publish UI, for F# projects
* Property page auto-sizing support for different monitors
* Smart indentation support which auto-indents based on scope and auto-deindents for bracket-like characters
* XML documentation comment width scaling to prevent it running horizontally off the screen
* Multiple settings pages to modify tooling settings
* Support for Drag and Drop across folders
* Support for nightly builds of the tools
* Expanded debugger view for lists from 50 items to 5000 items
* Support for Optimization APIs in the compiler service
* Support for `IsNameGenerated` in the F# symbols API

## Older F# releases

### [4.0.0] - Visual Studio 2015 Update 1 - 30 November 2015

#### Enhancements
* Perf: `for i in expr do body` optimization [#219](https://github.com/Microsoft/visualfsharp/pull/219)
* Remove type provider security dialog and use custom icon for type provider assembly reference [#448](https://github.com/Microsoft/visualfsharp/pull/448)
* Perf: Enable parallel build inside Visual Studio [#487](https://github.com/Microsoft/visualfsharp/pull/487)
* Perf: Remove StructBox for Value Types [#549](https://github.com/Microsoft/visualfsharp/pull/549)
* Add compiler warnings for redundant arguments in raise/failwith/failwithf/nullArg/invalidOp/invalidArg [#630](https://github.com/Microsoft/visualfsharp/pull/630)
* Add a compiler warning for lower case literals in patterns [#666](https://github.com/Microsoft/visualfsharp/pull/666)

#### Bug fixes
* Fix scope of types for named values in attributes improperly set [#437](https://github.com/Microsoft/visualfsharp/pull/437)
* Add general check for escaping typars to check phase [#442](https://github.com/Microsoft/visualfsharp/pull/442)
* Fix AccessViolationException on obfuscated assemblies [#519](https://github.com/Microsoft/visualfsharp/pull/519)
* Fix memory leaks while reloading solutions in Visual Studio [#591](https://github.com/Microsoft/visualfsharp/pull/591)
* Enable breakpoints in `with` augmentations for class types [#608](https://github.com/Microsoft/visualfsharp/pull/608)
* Fix false escaping type parameter check error [#613](https://github.com/Microsoft/visualfsharp/pull/613)
* Fix quotation of readonly fields [#622](https://github.com/Microsoft/visualfsharp/pull/622)
* Keep the reference icons when opening references [#623](https://github.com/Microsoft/visualfsharp/pull/623)
* Don't suppress missing FSI transitive references [#626](https://github.com/Microsoft/visualfsharp/pull/626)
* Make Seq.cast's non-generic and generic IEnumerable implementations equivalent [#651](https://github.com/Microsoft/visualfsharp/pull/651)

### [4.0.0] - 20 July 2015

Includes commits up to `dd8252eb8d20aaedf7b1c7576cd2a8a82d24f587`

#### Language, compiler, runtime, interactive

* Normalization and expansion of `Array`, `List`, and `Seq` modules
  * New APIs for 4.0: `chunkBySize`, `contains`, `except`, `findBack`, `findInstanceBack`, `indexed`, `item`, `mapFold`, `mapFoldBack`, `sortByDescending`, `sortDescending`, `splitInto`, `tryFindBack`, `tryFindIndexBack`, `tryHead`, `tryItem`, `tryLast`
  ![Collection API additions](http://i.imgur.com/SdJ7Doh.png)
* Other new APIs
  * `Option.filter`, `Option.toObj`, `Option.ofObj`, `Option.toNullable`, `Option.ofNullable`
  * `String.filter`
  * `Checked.int8`, `Checked.uint8`
  * `Async.AwaitTask` (non-generic)
  * `WebClient.AsyncDownloadFile`, `WebClient.AsyncDownloadData`
  * `tryUnbox`, `isNull`
* New active pattern to match constant `Decimal` in quotations
* Slicing support for lists
* Support for consuming high-rank (> 4) arrays
* Support for units of measure in `printf`-family functions
* Support for constructors/class names as first-class functions
* Improved exception stack traces in async code
* Automatic `mutable`/`ref` conversion
* Support for static arguments to provided methods
* Support for non-nullable provided types
* Added `NonStructuralComparison` module containing non-structural comparison operators
* Support for rational exponents in units of measure
* Give fsi.exe, fsiAnyCpi.exe nice icons
* `Microsoft.` optional in namepsace paths from FSharp.Core
* Support for extension properties in object initializers
* Pre-support (not yet used) for additional nativeptr intrinsics
* Simplified, more robust resolution of type references in quotations
* Support for inheritance of types that have multiple interface instantiations
* Extended preprocessor grammar
* Support for implicit quotation of expressions used as method arguments
* Support for multiple properties in `[<StructuredFormatDisplay>]`
* Eliminate tuple allocation for implicitly returned formal arguments
* Perf: fsc.exe now uses `GCLatencyMode.Batch`
* Perf: Improved `hash`/`compare`/`distinctBy`/`groupBy` performance
* Perf: `Seq.toArray` perf improvement
* Perf: Use `OptimizedClosures.FSharpFunc` in seq.fs where applicable
* Perf: Use literals and mutable variables instead of ref cells for better performance in SHA1 calc
* Perf: Use smart blend of `System.Array.Copy` and iterative copy for array copies
* Perf: Change `Seq.toList` to mutation-based to remove reliance on `List.rev`
* Perf: Change `pdbClose` to test if files are locked before inducing GCs
* Perf: Use server GC mode for compiler
* Bugfix: Changed an error message within the Set module to use the correct module name.
* Bugfix: Fix assembly name of warning FS2003
* Bugfix [#132](http://visualfsharp.codeplex.com/workitem/132): FSI Shadowcopy causes a significant degrade in the fsi first execute time
* Bugfix [#131](https://visualfsharp.codeplex.com/workitem/131): Fix getentryassembly return value when shadowcopy is enabled in FSI
* Bugfix [#61](https://visualfsharp.codeplex.com/workitem/61) Nonverifiable code generated with units of measure conversion
* Bugfix [#68](https://visualfsharp.codeplex.com/workitem/68) BadImageFormatException with Units of Measure
* Bugfix [#146](https://visualfsharp.codeplex.com/workitem/146) BadImageFormatException in both Release and Debug build with units of measure
* Bugfix: Incorrent cross-module inlining between different .NET profiles
* Bugfix: Properly document exceptions in `Array` module
* Bugfix [#24](https://visualfsharp.codeplex.com/workitem/24): Error reporting of exceptions in type providers `AddMemberDelayed`
* Bugfix [#13](https://github.com/fsharp/fsharp/issues/13): Error on FSI terminal resize
* Bugfix [#29](https://github.com/fsharp/fsharp/issues/29): Module access modifier `internal` does not give internal access if no namespaces are used
* Bugfix: Fix typo in error message for invalid attribute combination
* Bugfix [#27](https://github.com/microsoft/visualfsharp/issues/27): Private module values can be mutated by other modules
* Bugfix [#38](https://github.com/microsoft/visualfsharp/issues/38): ICE - System.ArgumentException: not a measure abbreviation, or incorrect kind
* Bugfix [#44](https://github.com/microsoft/visualfsharp/issues/44): Problems using FSI to `#load` multiple files contributing to the same namespace
* Bugfix [#95](https://github.com/microsoft/visualfsharp/issues/95): `[<RequireQualifiedAccess>]` allows access to DU member if qualified only by module name
* Bugfix [#89](https://github.com/microsoft/visualfsharp/issues/89): Embedding an untyped quotation in a typed quotation results in ArgumentException
* Bugfix: Show warning when Record is accessed without type but `[<RequireQualifiedAccess>]` was set
* Bugfix [#139](https://visualfsharp.codeplex.com/workitem/139): Memory leak in `Async.AwaitWaitHandle`
* Bugfix [#122](https://github.com/microsoft/visualfsharp/issues/122): `stfld` does not give `.volatile` annotation
* Bugfix [#30](https://github.com/microsoft/visualfsharp/issues/30): Compilation error "Incorrect number of type arguments to local call"
* Bugfix [#163](https://github.com/microsoft/visualfsharp/issues/163): Array slicing does not work properly with non 0-based arrays
* Bugfix [#148](https://github.com/microsoft/visualfsharp/issues/148): XML doc comment generation adding empty garbage
* Bugfix [#98](https://github.com/Microsoft/visualfsharp/issues/98): Using a single, optional, static parameter to a type provider causes failure
* Bugfix [#109](https://github.com/Microsoft/visualfsharp/issues/109): Invalid interface generated by --sig
* Bugfix [#123](https://github.com/Microsoft/visualfsharp/issues/123): Union types without sub-classes should be sealed
* Bugfix [#68](https://github.com/Microsoft/visualfsharp/issues/68): F# 3.1 / Profile 259: `<@ System.Exception() @>` causes AmbiguousMatchException at runtime
* Bugfix [#9](https://github.com/Microsoft/visualfsharp/issues/9): Internal error in FSI: FS0192: binding null type in envBindTypeRef
* Bugfix [#10](https://github.com/Microsoft/visualfsharp/issues/10): Internal error: binding null type in envBindTypeRef
* Bugfix [#266](https://github.com/Microsoft/visualfsharp/issues/266): `windowed` error message incorrectly flags "non-negative" input when "positive" is what's needed
* Bugfix [#270](https://github.com/Microsoft/visualfsharp/issues/270): "internal error: null: convTypeRefAux" in interactive when consuming quotation containing type name with commas or spaces
* Bugfix [#276](https://github.com/Microsoft/visualfsharp/issues/276): Combining struct field with units of measure will result managed type instead of unmanaged type
* Bugfix [#269](https://github.com/Microsoft/visualfsharp/issues/269): Accidentally `#load`ing a DLL in script causes internal error
* Bugfix [#293](https://github.com/Microsoft/visualfsharp/issues/293): `#r` references without relative path are not loaded when file is local
* Bugfix [#237](https://github.com/Microsoft/visualfsharp/issues/237): Problems using FSI on multiple namespaces in a single file
* Bugfix [#338](https://github.com/Microsoft/visualfsharp/issues/338): Escaped unicode characters are encoded incorrectly
* Bugfix [#370](https://github.com/Microsoft/visualfsharp/issues/370): `Seq.sortBy` cannot handle sequences of floats containing NaN
* Bugfix [#368](https://github.com/Microsoft/visualfsharp/issues/368): Optimizer incorrectly assumes immutable field accesses are side-effect free
* Bugfix [#337](https://github.com/Microsoft/visualfsharp/issues/337): Skip interfaces that lie outside the set of referenced assemblies
* Bugfix [#383](https://github.com/Microsoft/visualfsharp/issues/383): Class with `[<AllowNullLiteral(false)>]` barred from inheriting from normal non-nullable class
* Bugfix [#420](https://github.com/Microsoft/visualfsharp/issues/420): Compiler emits incorrect visibility modifier for internal constructors of abstract class
* Bugfix [#362](https://github.com/Microsoft/visualfsharp/issues/362): Depickling assertion followed by nullref internal errors in units-of-measure case
* Bugfix [#342](https://github.com/Microsoft/visualfsharp/issues/342): FS0193 error when specifying sequential struct layout of a type
* Bugfix [#299](https://github.com/Microsoft/visualfsharp/issues/299): AmbiguousMatchException with `[<ReflectedDefinition>]` on overloaded extension methods
* Bugfix [#316](https://github.com/Microsoft/visualfsharp/issues/316): Null array-valued attribute causes internal compiler error
* Bugfix [#147](https://github.com/Microsoft/visualfsharp/issues/147): FS0073: internal error: Undefined or unsolved type variable: 'a
* Bugfix [#34](https://github.com/Microsoft/visualfsharp/issues/34): Error in pass2 for type FSharp.DataFrame.FSharpFrameExtensions, error: duplicate entry 'Frame2.GroupRowsBy' in method table
* Bugfix [#212](https://github.com/Microsoft/visualfsharp/issues/212): Record fields initialized in wrong order
* Bugfix [#445](https://github.com/Microsoft/visualfsharp/issues/445): Inconsistent compiler prompt message when using `--pause` switch
* Bugfix [#238](https://github.com/Microsoft/visualfsharp/issues/238): Generic use of member constraint solved to record field causes crash

#### Visual Studio

* Updated all templates (except tutorial) to include AssemblyInfo.fs setup in the same manner as default C# project templates
* Add keyboard shortcuts for FSI reset and clear all
* Improved debugger view for Map values
* Improved performance reading stdout/stderr from fsi.exe to F# Interactive window
* Support for VS project up-to-date check
* Improved project template descriptions, make it clearer how to target Xamarin platforms
* Intellisense completion in object initializers
* Add menu entry "Open folder in File Explorer" on folder nodes
* Intellisense completion for named arguments
* `Alt+Enter` sends current line of code to interactive if there is no selection
* Support for debugging F# scripts with the VS debugger
* Add support for hexadecimal values (like 0xFF) ??to MSBuild property BaseAddress
* Updated menu icons used for F# interactive to align with other VS interactive windows
* Bugfix: Fix url of fsharp.org website in vs templates
* Bugfix [#141](https://visualfsharp.codeplex.com/workitem/141): The "Error List" window does not parse MSBuild messages correctly
* Bugfix [#147](https://visualfsharp.codeplex.com/workitem/147): Go to definition doesn't work for default struct ctors
* Bugfix [#50](https://github.com/microsoft/visualfsharp/issues/50): Members hidden from IntelliSense still show up in tooltips
* Bugfix [#57](https://github.com/microsoft/visualfsharp/issues/57) (partial): Visual Studio locking access to XML doc files
* Bugfix [#157](https://github.com/Microsoft/visualfsharp/issues/157): Should not allow Framework 4 / F# 3.1 combination in project properties
* Bugfix [#114](https://github.com/Microsoft/visualfsharp/issues/114): Portable Library (legacy) template displays wrong target framework version
* Bugfix [#273](https://github.com/Microsoft/visualfsharp/issues/273): VS editor shows bogus errors when scripts use multi-hop `#r` and `#load` with relative paths
* Bugfix [#312](https://github.com/Microsoft/visualfsharp/issues/312): F# library project templates and portable library templates do not have `AutoGenerateBindingRedirects` set to true
* Bugfix [#321](https://github.com/Microsoft/visualfsharp/issues/321): Provided type quickinfo shouldn't show hidden and obsolete members from base class
* Bugfix [#319](https://github.com/Microsoft/visualfsharp/issues/319): Projects with target runtime 3.0 don't show up correctly on the VS project dialog
* Bugfix [#283](https://github.com/Microsoft/visualfsharp/issues/283): Changing target framework causes incorrect binding redirects to be added to app.config
* Bugfix [#278](https://github.com/Microsoft/visualfsharp/issues/278): NullReferenceException when trying to add some COM references
* Bugfix [#259](https://github.com/Microsoft/visualfsharp/issues/259): Renaming files in folders causes strange UI display
* Bugfix [#350](https://github.com/Microsoft/visualfsharp/issues/350): Renaming linked file results in error dialog
* Bugfix [#381](https://github.com/Microsoft/visualfsharp/issues/381): Intellisense stops working when referencing PCL component from script (requires `#r "System.Runtime"`)
* Bugfix [#104](https://github.com/Microsoft/visualfsharp/issues/104): Using paste to add files to an F# project causes the order of files in the project and on the UI to get out of sync
* Bugfix [#417](https://github.com/Microsoft/visualfsharp/issues/417): 'Move file up/down' keybindings should be scoped to solution explorer
* Bugfix [#246](https://github.com/Microsoft/visualfsharp/issues/246): Fix invalid already rendered folder error
* Bugfix [#106](https://github.com/Microsoft/visualfsharp/issues/106) (partial): Visual F# Tools leak memory while reloading solutions

### [3.1.2] - 20 August 2014

Includes commits up to `3385e58aabc91368c8e1f551650ba48705aaa285`

#### Language, compiler, runtime, interactive

* Allow arbitrary-dimensional slicing
* Ship versions FSharp.Core.dll built on portable profiles 78 and 259
* Support "shebang" (`#!`) in F# source files
* Vertical pipes disallowed in active pattern case identifiers
* Enable non-locking shadow copy of reference assemblies in fsi/fsianycpu
* Inline codegen optimization using structs
* Perf improvement for `Seq.windowed`
* exe.config files for fsc, fsi, fsianycpu now use simple version range instead of long set of explicit version redirects
* Bugfix [#72](https://visualfsharp.codeplex.com/workitem/72): Indexer properties with more than 4 arguments cannot be accessed
* Bugfix [#113](https://visualfsharp.codeplex.com/workitem/113): `Async.Sleep` in .NETCore profiles does not invoke error continuation
* Bugfix [#91](https://visualfsharp.codeplex.com/workitem/91): String module documentation is false
* Bugfix [#78](https://visualfsharp.codeplex.com/workitem/78): Allow space characters in active pattern case identifiers
* Bugfix: Invalid code generated when calling VB methods with optional byref args
* Bugfix [#69](https://visualfsharp.codeplex.com/workitem/69): Invalid code generated when calling C# method with optional nullable args
* Bugfix [#9](https://visualfsharp.codeplex.com/workitem/9): XML doc comments on F# record type fields do not appear when accessing in C#
* Bugfix [#59](https://visualfsharp.codeplex.com/workitem/59): Compiler always requires System.Runtime.InteropServices, this is not present in all portable profiles
* Bugfix [#17](https://visualfsharp.codeplex.com/workitem/17): Incorrect generation of XML from doc comments for Record fields
* Bugfix [#7](https://visualfsharp.codeplex.com/workitem/17): NullRef in list comprehension, when for loop works
* Bugfix [#1](https://visualfsharp.codeplex.com/workitem/1): Type inference involving generic param arrays
* Bugfix [#37](https://visualfsharp.codeplex.com/workitem/37): Perf regression in 3.1.0 related to resolving extension methods
* Bugfix: Can't run F# console application with 'update' in name
* Bugfix: Slicing and range expression inconsistent
* Bugfix: Invalid code is generated when using field initializers in struct constructor

#### Visual Studio

* Project templates for F# portable libraries targeting profiles 78 and 259
* Enable non-locking shadow copy of reference assemblies in fsi/fsianycpu (VS options added)
* Allow breakpoints to be set inside of quotations
* Support "Publish" action in project system for web, Azure
* Bugfix [#126](https://visualfsharp.codeplex.com/workitem/126): F# package installer does not honor custom install paths for express SKUs
* Bugfix [#75](https://visualfsharp.codeplex.com/workitem/75): Microsoft.FSharp.Targets shim not deployed with F# SDK
* Bugfix: Fix crash in smart indent provider
* Bugfix [#55](https://visualfsharp.codeplex.com/workitem/55): Cannot add reference to F# PCL project
* Bugfix: Typos in tutorial project script
* Bugfix: Required C# event members do not appear in intellisense when signature is (object, byref)


### [3.1.1] - 24 January 2014

#### Language, compiler, runtime, interactive

* Improve F# compiler telemetry
* Bugfix: Improper treatment of * in AssemblyVersion attribute
* Bugfix: ``sprintf "%%"`` returns `"%%"` in F# 3.1.0, previously returned `"%"` in F# 3.0 and earlier
* Bugfix: F# 3.0 1D slice setter does not compile in F# 3.1.0

#### Visual Studio

* Enable installation of Visual F# on VS Desktop Express
* Added support for showing xml doc comments for named arguments
* Visual F# package deployable on non-VS machines. Deploys compiler and runtime toolchain plus msbuild targets
* Bugfix: Errors when attempting to add reference to .NET core library
* Bugfix: Crash in `FSComp.SR.RunStartupValidation()`

[4.0.0]: https://fsharp.org
[3.1.2]: http://blogs.msdn.com/b/fsharpteam/archive/2014/08/20/announcing-the-release-of-visual-f-tools-3-1-2.aspx
[3.1.1]: http://blogs.msdn.com/b/fsharpteam/archive/2014/01/22/announcing-visual-f-3-1-1-and-support-for-desktop-express.aspx





#### FSharp Compiler Service Versions Release notes

#### 23.0.1
  * FSharp.Compiler.Service nuget now uses net461, netstandard2.0 and FSharp.Core 4.6.2

#### 22.0.3
  * [Add entity.DeclaringEntity](https://github.com/Microsoft/visualfsharp/pull/4633), [FCS feature request](https://github.com/fsharp/FSharp.Compiler.Service/issues/830)

#### 22.0.2
  * Use correct version number in DLLs (needed until https://github.com/Microsoft/visualfsharp/issues/3113 is fixed)

#### 22.0.1
  * Integrate visualfsharp master
  * Includes recent memory usage reduction work for ByteFile and ILAttributes

#### 21.0.1
  * Use new .NET SDK project files
  * FSharp.Compiler.Service nuget now uses net461 and netstandard2.0
  * FSharp.Compiler.Service netstandard2.0 now supports type providers

#### 19.0.1
  * Rename ``LogicalEnclosingEntity`` to ``ApparentEnclosingEntity`` for consistency int he F# codebase terminology.
  * Rename ``EnclosingEntity`` to ``DeclaringEntity``.  In the case of extension properties, ``EnclosingEntity`` was incorrectly returning the logical enclosing entity (i.e. the type the property appears to extend), and in this case ``ApparentEnclosingEntity`` should be used instead.

#### 18.0.1
  * Integrate visualfsharp master

#### 17.0.2
  * Integrate visualfsharp master

#### 16.0.3
  * [File name deduplication not working with ParseAndCheckFileInProject](https://github.com/fsharp/FSharp.Compiler.Service/issues/819)

#### 16.0.2
  * [ProjectCracker returns *.fsi files in FSharpProjectOptions.SourceFiles array](https://github.com/fsharp/FSharp.Compiler.Service/pull/812)

  * [Fix line endings in the Nuget packages descriptions](https://github.com/fsharp/FSharp.Compiler.Service/pull/811)

#### 16.0.1
  * FSharpChecker provides non-reactor ParseFile instead of ParseFileInProject
  * Add FSharpParsingOptions, GetParsingOptionsFromProjectOptions, GetParsingOptionsFromCommandLine

#### 15.0.1
  * Integrate latest changes from visualfsharp
  * Add implementation file contents to CheckFileResults
  * Fix non-public API in .NET Standard 1.6 version

#### 14.0.1
  * Integrate latest changes from visualfsharp
  * Trial release for new build in fcs\...

#### 13.0.1
  * Move docs --> docssrc

#### 13.0.0
  * Move FSharp.Compiler.Service.MSBuild.v12.dll to a separate nuget package

#### 12.0.8
  * Set bit on output executables correctly

#### 12.0.7
  * Integrate visualfsharp master

#### 12.0.6
  * [758: Fix project cracker when invalid path given](https://github.com/fsharp/FSharp.Compiler.Service/pull/758)

#### 12.0.5
  * Remove dependency on System.ValueTuple

#### 12.0.3
  * [De-duplicate module names again](https://github.com/fsharp/FSharp.Compiler.Service/pull/749)

#### 12.0.2
  * De-duplicate module names

#### 12.0.1
  * [Integrate visualfsharp and fsharp](https://github.com/fsharp/fsharp/pull/696)

#### 11.0.10
  * [Fix F# Interactive on Mono 4.0.9+](https://github.com/fsharp/fsharp/pull/696)

#### 11.0.9
  * [Make incremental builder counter atomic](https://github.com/fsharp/FSharp.Compiler.Service/pull/724)
  * [Add IsValCompiledAsMethod to FSharpMemberOrFunctionOrValue](https://github.com/fsharp/FSharp.Compiler.Service/pull/727)
  * [Check before ILTypeInfo.FromType](https://github.com/fsharp/FSharp.Compiler.Service/issues/734)
  * [Transition over to dotnet cli Fsproj](https://github.com/fsharp/FSharp.Compiler.Service/issues/700)

#### 11.0.8
  * Depend on FSharp.Core package

#### 11.0.6
  * Fix [stack overflow exception](https://github.com/fsharp/FSharp.Compiler.Service/issues/672)

#### 11.0.4
  * Fix [out of range exception](https://github.com/fsharp/FSharp.Compiler.Service/issues/709)

#### 11.0.2
  * Integrate fsharp\fsharp and Microsoft\visualfsharp to 262deb017cfcd0f0d4138779ff42ede7dbf44c46

#### 11.0.1
  * Integrate fsharp\fsharp and Microsoft\visualfsharp to d0cc249b951374257d5a806939e42714d8a2f4c6

#### 10.0.3
  * [Expose assumeDotNetFramework in FSharpChecker.GetProjectOptionsFromScript](https://github.com/fsharp/FSharp.Compiler.Service/pull/699)
  * [SemanticClassificationType should not be internal](https://github.com/fsharp/FSharp.Compiler.Service/pull/696)

#### 10.0.1
  * [Adds FormatValue to FsiEvaluationSession, using the fsi object values for formatting](https://github.com/fsharp/FSharp.Compiler.Service/pull/686)

#### 10.0.0
  * Integrate fsharp\fsharp and Microsoft\visualfsharp to c3e55bf0b10bf08790235dc585b8cdc75f71618e
  * Integrate fsharp\fsharp and Microsoft\visualfsharp to 11c0a085c96a91102cc881145ce281271ac159fe
  * Some API changes for structured text provision for tagged structured text

#### 9.0.0
  * Update names of union fields in AST API
  * Fix load closure for ParseAndCheckInteraction
  * [Fix #631 compiler dependency on MSBuild](https://github.com/fsharp/FSharp.Compiler.Service/pull/657)
  * Fixed netcore codegen on Linux
  * Explicit error when cracker exe is missing

#### 8.0.0
  * Integrate fsharp\fsharp and Microsoft\visualfsharp to c494a9cab525dbd89585f7b733ea5310471a8001
  * Then integrate to 	2002675f8aba5b3576a924a2e1e47b18e4e9a83d
  * [Add module values to navigable items](https://github.com/fsharp/FSharp.Compiler.Service/pull/650)
  * Optionally remove dependency on MSBuild reference resolution https://github.com/fsharp/FSharp.Compiler.Service/pull/649
  * [Compiler api harmonise](https://github.com/fsharp/FSharp.Compiler.Service/pull/639)
  * Various bits of work on .NET Core version (buildable from source but not in nuget package)

#### 7.0.0
  * Integrate fsharp\fsharp and Microsoft\visualfsharp to 835b79c041f9032fceeceb39f680e0662cba92ec

#### 6.0.2
  * [Fix #568: recognize provided expressions](https://github.com/fsharp/FSharp.Compiler.Service/pull/568)

#### 6.0.1
  * [Fix ProjectFileNames order when getting project options from script](https://github.com/fsharp/FSharp.Compiler.Service/pull/594)

#### 6.0.0
  * Switch to new major version on assumption integrated F# compiler changes induce API change

#### 5.0.2
  * Integrate Microsoft\visualfsharp to 688c26bdbbfc766326fc45e4d918f87fcba1e7ba. F# 4.1 work

#### 5.0.1
* [Fixed dependencies in nuget package](https://github.com/fsharp/FSharp.Compiler.Service/pull/608)

#### 5.0.0
* Fixed empty symbol declared pdb #564 from kekyo/fix-empty-pdb
* .NET Core ProjectCracker - updated version and dependencies
* Properly embed 'FSIstrings' resource, fixes #591
* make build.sh work on windows (git bash).
* Added default script references for .NET Core
* Store useMonoResolution flag
* Updated MSBuild version
* Assume FSharp.Core 4.4.0.0

#### 4.0.1
* Integrate Microsoft\visualfsharp and fsharp\fsharp to master (including portable PDB)
* Remove .NET Framework 4.0 support (now needs .NET Framework 4.5)

#### 4.0.0
* Integrate Microsoft\visualfsharp and fsharp\fsharp to master

#### 3.0.0.0
* #538 - BackgroundCompiler takes a very long time on a big solution with a very connected project dependency graph
* #544 - Losing operator call when one of operands is application of a partially applied function
* #534 - Function valued property erasing calls
* #495 - Detupling missing when calling a module function value
* #543 - Tuple not being destructured in AST
* #541 - Results of multiple calls to active pattern are always bound to variable with same name
* #539 - BasicPatterns.NewDelegate shows same value for different arguments

#### 2.0.0.6
* #530 - Adjust ProjectCracker NuGet for VS/NuGet

#### 2.0.0.5
* #527 - Provide API that includes printf specifier arities along with ranges

#### 2.0.0.4
* #519 - Change nuget layout for ProjectCracker package
* #523 - Project cracking: spaces in file paths

#### 2.0.0.3
* #508 - Integrate visualfsharp/master removal of Silverlight #if
* #513 - Make CrackerTool `internal` to prevent accidental usage
* #515 - Add simple Visual Studio version detection for project cracker

#### 2.0.0.2
* Integrate visualfsharp/master and fsharp/master --> master
* Expose QualifiedName and FileName of FSharpImplementationFileContents
* Add FSharpErrorInfo.ErrorNumber

#### 2.0.0.1-beta
* Fix 452 - FSharpField.IsMutable = true for BCL enum cases
* Fix 414 - Add IsInstanceMemberInCompiledCode

#### 2.0.0.0-beta
* Feature #470, #478, #479 - Move ProjectCracker to separate nuget package and DLL, used ProjectCrackerTool.exe to run
* Feature #463 - Expose slot signatures of members in object expressions
* Feature #469, #475 - Add EvalExpressionNonThrowing, EvalInteractionNonThrowing, EvalScriptNonThrowing
* Fix #456 - FCS makes calls to kernel32.dll when running on OSX
* Fix #473 - stack overflow in resolution logic
* Fix #460 - Failure getting expression for a provided method call

#### 1.4.2.3 -
* Fix bug in loop optimization, apply https://github.com/Microsoft/visualfsharp/pull/756/

#### 1.4.2.2 -
* #488 - Performance problems with project references

#### 1.4.2.1 -
* #450 - Correct generation of ReferencedProjects

#### 1.4.2.0 -
* Fix bug in double lookup of cache, see https://github.com/fsharp/FSharp.Compiler.Service/pull/447

#### 1.4.1 -
* Add pause before backgrounnd work starts. The FCS request queue must be empty for 1 second before work will start
* Write trace information about the reactor queue to the event log
* Rewrite reactor to consistently prioritize queued work
* Implement cancellation for queued work if it is cancelled prior to being executed
* Adjust caching to check cache correctly if there is a gap before the request is executed

#### 1.4.0.9 -
* FSharpType.Format fix
* Disable maximum-memory trigger by default until use case ironed out

#### 1.4.0.8 -
* FSharpType.Format now prettifies type variables.  If necessary, FSharpType.Prettify can also be called
* Add maximum-memory trigger to downsize FCS caches. Defaults to 1.7GB of allocaed memory in the system
  process for a 32-bit process, and 2x this for a 64-bit process

#### 1.4.0.7 -
* fix 427 - Make event information available for properties which represent first-class uses of F#-declared events
* fix 410 - Symbols for C# fields (and especially enum fields)
* Expose implemented abstract slots
* Fix problem with obscure filenames caught by Microsoft\visualfsharp tests
* Integrate with visualfsharp master

#### 1.4.0.6 -
* fix 423 - Symbols for non-standard C# events
* fix 235 - XmlDocSigs for references assemblies
* fix 177 - GetAllUsesOfAllSymbolsInFile returns nothing for C# nested enum
* make Internal.Utilities.Text.Lexing.Position a struct
* Exposing assembly attributes on FSharpAssemblySignature
* clean up IncrementalFSharpBuild.frameworkTcImportsCache

#### 1.4.0.5 -
* add more entries to FSharpTokenTag

#### 1.4.0.4 -
* add more entries to FSharpTokenTag
* add PrettyNaming.QuoteIdentifierIfNeeded and PrettyNaming.KeywordNames

#### 1.4.0.3 -
* integrate Microsoft/visualfsharp OOB cleanup via fsharp/fsharp
* Make Parser and Lexer private

#### 1.4.0.2 -
* #387 - types and arrays in F# attribute contructor arguments

#### 1.4.0.1 - F# 4.0 support
* Use FSharp.Core 4.4.0.0 by default for scripting scenarios if not FSharp.Core referenced by host process

#### 1.4.0.0-beta - F# 4.0 support
* Integrate F# 4.0 support into FSharp.Compiler.Service

#### 1.3.1.0 -
* simplified source indexing with new SourceLink
* Add noframework option in AST compiler methods

#### 0.0.90 -
* Add fix for #343 Use ResolveReferences task
* Expose BinFolderOfDefaultFSharpCompiler to editors
* Fix the registry checking on mono to avoid unnecessary exceptions being thrown

#### 0.0.89 -
* Fix output location of referenced projects

#### 0.0.88 -
* Added Fix to allow implicit PCL references to be retrieved

#### 0.0.87 -
* Don't report fake symbols in indexing #325
* Add EnclosingEntity for an active pattern group #327
* Add ImmediateSubExpressions #284
* integrate fsharp/fsharp master into master

#### 0.0.85 -
* Fix for FSharpSymbolUse for single case union type #301
* Added supprt for ReturnParameter in nested functions

#### 0.0.84 -
* Added curried parameter groups for nested functions

#### 0.0.83 -
* Add Overloads to the symbols signature so it is publicly visible
* Update OnEvaluation event to have FSharpSymbolUse information available

#### 0.0.82 -
* Better support for Metadata of C# (and other) Assemblies.
* Expose the DefaultFileSystem as a type instead of anonymous

#### 0.0.81 -
* Update GetDeclarationListSymbols to expose FSharpSymbolUse
* Improve reporting of format specifiers

#### 0.0.80 -
* Update to latest F# 3.1.3 (inclunding updated FsLex/FsYacc used in build of FCS)
* Report printf specifiers from Service API
* Improve Accessibility of non-F# symbols

#### 0.0.79 -
* Do not use memory mapped files when cracking a DLL to get an assembly reference
* Fix for multilanguage projects in project cracker

#### 0.0.78 -
* Reduce background checker memory usage
* add docs on FSharp.Core
* docs on caches and queues

#### 0.0.77 -
* Update to github.com/fsharp/fsharp 05f426cee85609f2fe51b71473b07d7928bb01c8

#### 0.0.76 -
* Fix #249 - Fix TryFullName when used on namespaces of provided erased type definitions
* Add OnEvaluation event to FCS to allow detailed information to be exposed

#### 0.0.75 -
* Do not use shared cursor for IL binaries (https://github.com/fsprojects/VisualFSharpPowerTools/issues/822)

#### 0.0.74 -
* Extension members are returned as members of current modules
* Fix exceptions while cross-reference a type provider project

#### 0.0.73 -
* Add AssemblyContents and FSharpExpr to allow access to resolved, checked expression trees
* Populate ReferencedProjects using ProjectFileInfo
* Fix finding symbols declared in signature files
* Add logging to project cracking facility

#### 0.0.72 -
* Allow project parser to be used on project file with relative paths
* Expose attributes for non-F# symbols

#### 0.0.71 -
* More renamings in SourceCodeServices API for more consistent use of 'FSharp' prefix

#### 0.0.70 -
* Make FSharpProjectFileParser public
* Fixes to project parser for Mono (.NET 4.0 component)
* Renamings in SourceCodeServices API for more consistent use of 'FSharp' prefix

#### 0.0.67 -
* Fixes to project parser for Mono

#### 0.0.66 -
* Fixes to project parser for Mono
* Use MSBuild v12.0 for reference resolution on .NET 4.5+

#### 0.0.65 -
* Fixes to project parser

#### 0.0.64 -
* Add project parser, particularly GetProjectOptionsFromProjectFile

#### 0.0.63 -
* #221 - Normalize return types of .NET events

#### 0.0.62 -
* Integrate to latest https://github.com/fsharp/fsharp (#80f9221f811217bd890b3a670d717ebc510aeeaf)

#### 0.0.61 -
* #216 - Return associated getters/setters from F# properties
* #214 - Added missing XmlDocSig for FSharpMemberOrFunctionOrValue's Events, Methods and Properties
* #213 - Retrieve information for all active pattern cases
* #188 - Fix leak in file handles when using multiple instances of FsiEvaluationSession, and add optionally collectible assemblies

#### 0.0.60 -
* #207 - Add IsLiteral/LiteralValue to FSharpField
* #205 - Add IsOptionalArg and related properties to FSharpParameter
* #210 - Check default/override members via 'IsOverrideOrExplicitMember'
* #209 - Add TryFullName to FSharpEntity

#### 0.0.59 -
* Fix for #184 - Fix EvalScript by using verbatim string for #Load
* Fix for #183 - The line no. reporting is still using 0-based indexes in errors. This is confusing.

#### 0.0.58 -
* Fix for #156 - The FSharp.Core should be retrieved from the hosting environment

#### 0.0.57 -
* Second fix for #160 - Nuget package now contains .NET 4.0 and 4.5

#### 0.0.56 -
* Fix for #160 - Nuget package contains .NET 4.0 and 4.5

#### 0.0.55 -
* Integrate changes for F# 3.1.x, Fix #166

#### 0.0.54 -
* Fix for #159 - Unsubscribe from TP Invalidate events when disposing builders

#### 0.0.53 -
* Add queue length to InteractiveChecker

#### 0.0.52 -
* Fix caches keeping hold of stale entries

#### 0.0.51 -
* Add IsAccessible to FSharpSymbol, and ProjectContext.AccessibilityRights to give the context of an access

#### 0.0.50 -
* Fix #79 - FindUsesOfSymbol returns None at definition of properties with explicit getters and setters

#### 0.0.49 -
* Fix #138 - Fix symbol equality for provided type members
* Fix #150 - Return IsGetterMethod = true for declarations of F# properties (no separate 'property' symbol is yet returned, see #79)
* Fix #132 - Add IsStaticInstantiation on FSharpEntity to allow clients to detect fake symbols arising from application of static parameters
* Fix #154 - Add IsArrayType on FSharpEntity to allow clients to detect the symbols for array types
* Fix #96 - Return resolutions of 'Module' and 'Type' in "Module.field" and "Type.field"

#### 0.0.48 -
* Allow own fsi object without referencing FSharp.Compiler.Interactive.Settings.dll (#127)

#### 0.0.47 -
* Adjust fix for #143 for F# types with abstract+default events

#### 0.0.46 -
* Fix multi-project analysis when referenced projects have changed (#141)
* Fix process exit on bad arguments to FsiEvaluationSession (#126)
* Deprecate FsiEvaluationSession constructor and add FsiEvaluationSession.Create static method to allow for future API that can return errors
* Return additional 'property' and 'event' methods for F#-defined types to regularize symbols (#108, #143)
* Add IsPropertySetterMethod and IsPropertyGetterMethod which only return true for getter/setter methods, not properties. Deprecate IsSetterMethod and IsGetterMethod in favour of these.
* Add IsEventAddMethod and IsEventRemoveMethod which return true for add/remove methods with an associated event
* Change IsProperty and IsEvent to only return true for the symbols for properties and events, rather than the methods associated with these
* Fix value of Assembly for some symbols (e.g. property symbols)

#### 0.0.45 -
* Add optional project cache size parameter to InteractiveChecker
* Switch to openBinariesInMemory for SimpleSourceCodeServices
* Cleanup SimpleSourceCodeServices to avoid code duplication

#### 0.0.44 -
* Integrate latest changes from visualfsharp.codeplex.com via github.com/fsharp/fsharp
* Fix problem with task that generates description text of declaration
* Add AllInterfaceTypes to FSharpEntity and FSharpType
* Add BaseType to FSharpType to propagate instantiation
* Add Instantiate to FSharpType

#### 0.0.43 -
* Fix #109 - Duplicates in GetUsesOfSymbolInFile

#### 0.0.42 -
* Fix #105 - Register enum symbols in patterns
* Fix #107 - Return correct results for inheritance chain of .NET types
* Fix #101 - Add DeclaringEntity property

#### 0.0.41 -
* Fixed #104 - Make all operations that may utilize the FCS reactor async
* Add FSharpDisplayContext and FSharpType.Format
* Replace GetSymbolAtLocationAlternate by GetSymbolUseAtLocation

#### 0.0.40 -
* Fixed #86 - Expose Microsoft.FSharp.Compiler.Interactive.Shell.Settings.fsi
* Fixed #99 - Add IsNamespace property to FSharpEntity

#### 0.0.39 -
* Fixed #79 - Usage points for symbols in union patterns

#### 0.0.38 -
* Fixed #94 and #89 by addition of new properties to the FSharpSymbolUse type
* Fixed #93 by addition of IsOpaque to FSharpEntity type
* Fixed #92 - Issue with nested classes
* Fixed #87 - Allow analysis of members from external assemblies

#### 0.0.37 -
* Obsolete HasDefaultValue - see https://github.com/fsharp/FSharp.Compiler.Service/issues/77

#### 0.0.36 -
* Fix #71 - Expose static parameters and xml docs of type providers
* Fix #63 - SourceCodeServices: #r ignores include paths passed as command-line flags

#### 0.0.35 -
* Fix #38 - FSharp.Compiler.Services should tolerate an FSharp.Core without siginfo/optdata in the search path


#### 0.0.34 -
* Add StaticParameters property to entities, plus FSharpStaticParameter symbol
* Fix #65

#### 0.0.33 -
* Add FullName and Assembly properties for symbols
* Fix #76
* Add Japanese documentation

#### 0.0.32 -
* Make ParseFileInProject asynchronous
* Add ParseAndCheckFileInProject
* Use cached results in ParseAndCheckFileInProject if available

#### 0.0.31 -
* Fix performance problem with CheckFileInProject

#### 0.0.30 -
* Add initial prototype version of multi-project support, through optional ProjectReferences in ProjectOptions. Leave this empty
  to use DLL/file-based references to results from other projects.

#### 0.0.29 -
* Fix symbols for named union fields in patterns

#### 0.0.28 -
* Fix symbols for named union fields
* Add FSharpActivePatternCase to refine FSharpSymbol

#### 0.0.27 -
* Fix exception tag symbol reporting

#### 0.0.26 -
* Fix off-by-one in reporting of range for active pattern name

#### 0.0.25 -
* Add optional source argument to TryGetRecentTypeCheckResultsForFile to specify that source must match exactly

#### 0.0.24 -
* Update version number as nuget package may not have published properly

#### 0.0.23 -
* Move to one-based line numbering everywhere
* Provide better symbol information for active patterns

#### 0.0.22 -
* Provide symbol location for type parameters

#### 0.0.21 -
* Add GetUsesOfSymbolInFile
* Better symbol resolution results for type parameter symbols

#### 0.0.20 -
* Update version number as nuget package may not have published properly

#### 0.0.19 -
* Change return type of GetAllUsesOfSymbol, GetAllUsesOfAllSymbols and GetAllUsesOfAllSymbolsInFile to FSharpSymbolUse
* Add symbol uses when an abstract member is implemented.

#### 0.0.18 -
* Add GetAllUsesOfAllSymbols and GetAllUsesOfAllSymbolsInFile

#### 0.0.17 -
* Improvements to symbol accuracy w.r.t. type abbreviations

#### 0.0.16 -
* Make FSharpEntity.BaseType return an option
* FsiSesion got a new "EvalScript" method which allows to evaluate .fsx files

#### 0.0.15 -
* Update version number as nuget package may not have published properly

#### 0.0.14 -
* Update version number as nuget package may not have published properly

#### 0.0.13-alpha -
* Fix #39 - Constructor parameters are mistaken for record fields in classes

#### 0.0.12-alpha -
* Make the parts of the lexer/parser used by 'XmlDoc' tools in F# VS Power tools public

#### 0.0.11-alpha -
* Add 'IsUnresolved'

#### 0.0.10-alpha -
* Fix bug where 'multiple references to FSharp.Core' was given as error for scripts

#### 0.0.9-alpha -
* Fix fsc corrupting assemblies when generating pdb files (really)
* Give better error messages for missing assemblies
* Report more information about symbols returned by GetSymbolAtLocation (through subtypes)
* Fix typos in docs
* Return full project results from ParseAndCheckInteraction
* Be more robust to missing assembly references by default.

#### 0.0.8-alpha -
* Fix fsc corrupting assemblies when generating pdb files

#### 0.0.7-alpha -
* Fix docs
* Make symbols more robust to missing assemblies
* Be robust to failures on IncrementalBuilder creation
* Allow use of MSBuild resolution by IncrementalBuilder

#### 0.0.6-alpha -
* Fix version number

#### 0.0.5-alpha -
* Added GetUsesOfSymbol(), FSharpSymbol type, GetSymbolAtLocation(...)

#### 0.0.4-alpha -
* Added documentation of file system API
* Reporte errors correctly from ParseAndCheckProject


#### 0.0.3-alpha -
* Integrate FSharp.PowerPack.Metadata as the FSharp* symbol API
* Renamed Param --> MethodGroupItemParameter and hid record from view, made into an object
* Renamed Method --> MethodGroupItem and hid record from view, made into an object
* Renamed Methods --> MethodGroup and hid record from view, made into an object
* Renamed MethodGroup.Name --> MethodGroup.MethodName
* Renamed DataTip --> ToolTip consistently across all text
* Renamed CheckOptions --> ProjectOptions
* Renamed TypeCheckAnswer --> CheckFileAnswer
* Renamed UntypedParseInfo --> ParseFileResults
* Removed GetCheckOptionsFromScriptRoot member overload in favour of optional argument
* Renamed GetCheckOptionsFromScriptRoot --> GetProjectOptionsFromScript
* Renamed UntypedParse --> ParseFileInProject
* Renamed TypeCheckSource --> CheckFileInProjectIfReady
* Added numerous methods to API including CheckFileInProject
* Added experimental GetBackgroundCheckResultsForFileInProject, GetBackgroundParseResultsForFileInProject
* Added PartialAssemblySignature to TypeCheckResults/CheckFileResults
* Added CurrentPartialAssemblySignature to FsiEvaluationSession
* Added ParseAndCheckInteraction to FsiEvaluationSession to support intellisense implementation against a script fragment
* Added initial testing in tests/service
* Added ParseAndCheckProject to SourceCodeServices API. This will eventually return "whole project" information such as symbol tables.
* Added GetDefaultConfiguration to simplify process of configuring FsiEvaluationSession
* Added PartialAssemblySignatureUpdated event to FsiEvaluationSession
* Added travis build

#### 0.0.2-alpha -
* Integrate hosted FSI configuration, SimpleSourceCodeServices, cleanup to SourceCodeServices API


