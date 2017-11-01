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
* Integrate to latest http://github.com/fsharp/fsharp (#80f9221f811217bd890b3a670d717ebc510aeeaf)

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
* Change IsProperty and IsEvent to only return true for the symbols for properties and events, rather than the methods assocaited with these
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


