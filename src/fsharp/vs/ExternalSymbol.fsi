// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open FSharp.Reflection
open Microsoft.FSharp.Compiler.AbstractIL.IL
    

/// Represents a type in an external (non F#) assembly
[<RequireQualifiedAccess>]
type ExternalType =
    | Type of fullName: string * genericArgs: ExternalType list
    | Array of inner: ExternalType
    | Pointer of inner: ExternalType
    | TypeVar of ordinal: uint16
with
    member ToString : unit -> string
        
module ExternalType =
    val internal tryOfILType : ILType -> ExternalType


/// Represents the type of a single method parameter
[<RequireQualifiedAccess>]
type ParamTypeSymbol =
    | Param of ExternalType
    | Byref of ExternalType
with
    member ToString : unit -> string

module ParamTypeSymbol =
    val internal tryOfILType : ILType -> ParamTypeSymbol
    val internal tryOfILTypes : ILType list -> ParamTypeSymbol list option
    

/// Represents a symbol in an external (non F#) assembly
[<RequireQualifiedAccess>]
type ExternalSymbol =
    | Type of fullName: string
    | Constructor of typeName: string * args: ParamTypeSymbol list
    | Method of typeName: string * name: string * args: ParamTypeSymbol list
    | Field of typeName: string * name: string
    | Event of typeName: string * name: string
    | PropertyGet of typeName: string * name: string
    | PropertySet of typeName: string * name: string
with
    member ToString : unit -> string
    member internal ToDebuggerDisplay : unit -> string
