// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

open FSharp.Compiler.AbstractIL.IL
    
/// Represents a type in an external (non F#) assembly.
[<RequireQualifiedAccess>]
type FSharpExternalType =
    /// Type defined in non-F# assembly.
    | Type of fullName: string * genericArgs: FSharpExternalType list

    /// Array of type that is defined in non-F# assembly.
    | Array of inner: FSharpExternalType

    /// Pointer defined in non-F# assembly.
    | Pointer of inner: FSharpExternalType

    /// Type variable defined in non-F# assembly.
    | TypeVar of typeName: string

    override ToString : unit -> string
        
module FSharpExternalType =
    val internal tryOfILType : string array -> ILType -> FSharpExternalType option

/// Represents the type of a single method parameter
[<Sealed>]
type FSharpExternalParam =

    member IsByRef: bool

    member ParameterType: FSharpExternalType

    static member Create: parameterType: FSharpExternalType * isByRef: bool -> FSharpExternalParam

    override ToString : unit -> string

module FSharpExternalParam =

    val internal tryOfILType : string array -> ILType -> FSharpExternalParam option

    val internal tryOfILTypes : string array -> ILType list -> FSharpExternalParam list option

/// Represents a symbol in an external (non F#) assembly
[<RequireQualifiedAccess>]
type FSharpExternalSymbol =
    | Type of fullName: string

    | Constructor of typeName: string * args: FSharpExternalParam list

    | Method of typeName: string * name: string * paramSyms: FSharpExternalParam list * genericArity: int 

    | Field of typeName: string * name: string

    | Event of typeName: string * name: string

    | Property of typeName: string * name: string

    override ToString : unit -> string

    member internal ToDebuggerDisplay : unit -> string