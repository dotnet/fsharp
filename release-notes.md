    Copyright (c) Microsoft Corporation.  All Rights Reserved.  
    See License.txt in the project root for license information.
    
## About the release notes

We deliver F# and F# tools for Visual Studio and .NET Core releases. These can include bug fixes, new tooling features, new compiler features, performance improvements, infrastructure improvements, and new language versions. The most recent release of F# or any F# component will be at the top of this document.

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

[4.0.0]: http://fsharp.org
[3.1.2]: http://blogs.msdn.com/b/fsharpteam/archive/2014/08/20/announcing-the-release-of-visual-f-tools-3-1-2.aspx
[3.1.1]: http://blogs.msdn.com/b/fsharpteam/archive/2014/01/22/announcing-visual-f-3-1-1-and-support-for-desktop-express.aspx

