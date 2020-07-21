// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open FSharp.Compiler.Range
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.SyntaxTree
        
#if !FX_NO_INDENTED_TEXT_WRITER
/// Capture information about an interface in ASTs
[<RequireQualifiedAccess; NoEquality; NoComparison>]
type InterfaceData =
    | Interface of SynType * SynMemberDefns option
    | ObjExpr of SynType * SynBinding list
    member Range : range
    member TypeParameters : string[]

module InterfaceStubGenerator =

    /// Get members in the decreasing order of inheritance chain
    val getInterfaceMembers : FSharpEntity -> seq<FSharpMemberOrFunctionOrValue * seq<FSharpGenericParameter * FSharpType>>

    /// Check whether an interface is empty
    val hasNoInterfaceMember : FSharpEntity -> bool

    /// Get associated member names and ranges
    /// In case of properties, intrinsic ranges might not be correct for the purpose of getting
    /// positions of 'member', which indicate the indentation for generating new members
    val getMemberNameAndRanges : InterfaceData -> (string * range) list

    val getImplementedMemberSignatures : getMemberByLocation: (string * range -> Async<FSharpSymbolUse option>) -> FSharpDisplayContext -> InterfaceData -> Async<Set<string>>

    /// Check whether an entity is an interface or type abbreviation of an interface
    val isInterface : FSharpEntity -> bool

    /// Generate stub implementation of an interface at a start column
    val formatInterface : startColumn: int  -> indentation: int -> typeInstances: string [] ->  objectIdent: string -> methodBody: string -> displayContext: FSharpDisplayContext ->  excludedMemberSignatures : Set<string> -> FSharpEntity -> verboseMode : bool -> string

    /// Find corresponding interface declaration at a given position
    val tryFindInterfaceDeclaration: pos -> parsedInput: ParsedInput -> InterfaceData option
#endif
