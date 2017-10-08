// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler

open System
open Microsoft.FSharp.Compiler.SourceCodeServices

#if COMPILER_PUBLIC_API
type PartialLongName =
#else
type internal PartialLongName =
#endif
    { QualifyingIdents: string list
      PartialIdent: string
      LastDotPos: int option }
    static member Default : PartialLongName

#if COMPILER_PUBLIC_API
module QuickParse =
#else
module internal QuickParse =
#endif
    val MagicalAdjustmentConstant : int
    val CorrectIdentifierToken : s: string -> tokenTag: int -> int
    val GetCompleteIdentifierIsland : tolerateJustAfter: bool -> s : string -> p : int -> (string * int * bool) option
    /// Get the partial long name of the identifier to the left of index.
    val GetPartialLongName : line : string * index : int -> (string list * string)
    /// Get the partial long name of the identifier to the left of index.
    /// For example, for `System.DateTime.Now` it returns PartialLongName ([|"System"; "DateTime"|], "Now", Some 32), where "32" pos of the last dot.
    val GetPartialLongNameEx : line : string * index : int -> PartialLongName
    /// Tests whether the user is typing something like "member x." or "override (*comment*) x."
    val TestMemberOrOverrideDeclaration : tokens: FSharpTokenInfo[] -> bool