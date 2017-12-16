// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

#if COMPILER_PUBLIC_API
module PathUtils =
#else
module internal PathUtils =
#endif
    [<Sealed>]
    type Path =
        static member GetFullPathSafe : string -> string
        static member GetFileNameSafe : string -> string

    val (</>) : string -> string -> string

#if COMPILER_PUBLIC_API
module HashDirectiveInfo =
#else
module internal HashDirectiveInfo =
#endif

    open Microsoft.FSharp.Compiler.Range
    open Microsoft.FSharp.Compiler.Ast

    /// IncludeDirective (#I) contains the pointed directory
    type IncludeDirective =
        | ResolvedDirectory of string
    
    /// a LoadDirective (#load) either points to an existing file, or an unresolveable location (along with all locations searched)
    type LoadDirective =
        | ExistingFile of string
        | UnresolvableFile of string * previousIncludes : string array

    /// represents #I and #load directive information along with range
    [<NoComparison>]
    type Directive =
        | Include of IncludeDirective * range
        | Load of LoadDirective * range

    /// returns an array of LoadScriptResolutionEntries
    /// based on #I and #load directives
    val getIncludeAndLoadDirectives : ParsedInput -> Directive array

    /// returns Some (complete file name of a resolved #load directive at position) or None
    val getHashLoadDirectiveResolvedPathAtPosition : pos -> ParsedInput -> string option
