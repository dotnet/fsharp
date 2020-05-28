// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open FSharp.Compiler.AbstractIL.IL
    
/// Represents a type in an external (non F#) assembly.
[<RequireQualifiedAccess>]
type ExternalType =
    /// Type defined in non-F# assembly.
    | Type of fullName: string * genericArgs: ExternalType list

    /// Array of type that is defined in non-F# assembly.
    | Array of inner: ExternalType

    /// Pointer defined in non-F# assembly.
    | Pointer of inner: ExternalType

    /// Type variable defined in non-F# assembly.
    | TypeVar of typeName: string

    override ToString : unit -> string
        
module ExternalType =
    val internal tryOfILType : string array -> ILType -> ExternalType option

/// Represents the type of a single method parameter
[<RequireQualifiedAccess>]
type ParamTypeSymbol =
    | Param of ExternalType
    | Byref of ExternalType
    override ToString : unit -> string

module ParamTypeSymbol =
    val internal tryOfILType : string array -> ILType -> ParamTypeSymbol option
    val internal tryOfILTypes : string array -> ILType list -> ParamTypeSymbol list option

/// Represents a symbol in an external (non F#) assembly
[<RequireQualifiedAccess>]
type ExternalSymbol =
    | Type of fullName: string
    | Constructor of typeName: string * args: ParamTypeSymbol list
    | Method of typeName: string * name: string * paramSyms: ParamTypeSymbol list * genericArity: int 
    | Field of typeName: string * name: string
    | Event of typeName: string * name: string
    | Property of typeName: string * name: string

    override ToString : unit -> string

    member internal ToDebuggerDisplay : unit -> string