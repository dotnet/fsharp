// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

open FSharp.Reflection
open FSharp.Compiler.AbstractIL.IL
open System.Diagnostics

module Option =

    let ofOptionList (xs : 'a option list) : 'a list option =

        if xs |> List.forall Option.isSome then
            xs |> List.map Option.get |> Some
        else
            None
    
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

    override this.ToString() =
        match this with
        | Type (name, genericArgs) ->
            match genericArgs with
            | [] -> ""
            | args ->
                args
                |> List.map (sprintf "%O")
                |> String.concat ", "
                |> sprintf "<%s>"
            |> sprintf "%s%s" name
        | Array inner -> sprintf "%O[]" inner
        | Pointer inner -> sprintf "&%O" inner
        | TypeVar name -> sprintf "'%s" name
        
module FSharpExternalType =
    let rec tryOfILType (typeVarNames: string array) (ilType: ILType) =
        
        match ilType with
        | ILType.Array (_, inner) ->
            tryOfILType typeVarNames inner |> Option.map FSharpExternalType.Array
        | ILType.Boxed tyspec
        | ILType.Value tyspec ->
            tyspec.GenericArgs
            |> List.map (tryOfILType typeVarNames)
            |> Option.ofOptionList
            |> Option.map (fun genericArgs -> FSharpExternalType.Type (tyspec.FullName, genericArgs))
        | ILType.Ptr inner ->
            tryOfILType typeVarNames inner |> Option.map FSharpExternalType.Pointer
        | ILType.TypeVar ordinal ->
            typeVarNames
            |> Array.tryItem (int ordinal)
            |> Option.map (fun typeVarName -> FSharpExternalType.TypeVar typeVarName)
        | _ ->
            None

[<RequireQualifiedAccess>]
type FSharpExternalParam =

    | Param of parameterType: FSharpExternalType

    | Byref of parameterType: FSharpExternalType

    member c.IsByRef = match c with Byref _ -> true | _ -> false

    member c.ParameterType = match c with Byref ty -> ty | Param ty -> ty

    static member Create(parameterType, isByRef) = 
        if isByRef then Byref parameterType else Param parameterType

    override this.ToString () =
        match this with
        | Param t -> t.ToString()
        | Byref t -> sprintf "ref %O" t

module FSharpExternalParam =
    let tryOfILType (typeVarNames : string array) =
        function
        | ILType.Byref inner -> FSharpExternalType.tryOfILType typeVarNames inner |> Option.map FSharpExternalParam.Byref
        | ilType -> FSharpExternalType.tryOfILType typeVarNames ilType |> Option.map FSharpExternalParam.Param

    let tryOfILTypes typeVarNames ilTypes =
        ilTypes |> List.map (tryOfILType typeVarNames) |> Option.ofOptionList
    
[<RequireQualifiedAccess>]
[<DebuggerDisplay "{ToDebuggerDisplay(),nq}">]
type FSharpExternalSymbol =
    | Type of fullName: string
    | Constructor of typeName: string * args: FSharpExternalParam list
    | Method of typeName: string * name: string * paramSyms: FSharpExternalParam list * genericArity: int
    | Field of typeName: string * name: string
    | Event of typeName: string * name: string
    | Property of typeName: string * name: string

    override this.ToString () =
        match this with
        | Type fullName -> fullName
        | Constructor (typeName, args) ->
            args
            |> List.map (sprintf "%O")
            |> String.concat ", "
            |> sprintf "%s..ctor(%s)" typeName
        | Method (typeName, name, args, genericArity) ->
            let genericAritySuffix =
                if genericArity > 0 then sprintf "`%d" genericArity
                else ""

            args
            |> List.map (sprintf "%O")
            |> String.concat ", "
            |> sprintf "%s.%s%s(%s)" typeName name genericAritySuffix
        | Field (typeName, name)
        | Event (typeName, name)
        | Property (typeName, name) ->
            sprintf "%s.%s" typeName name

    member this.ToDebuggerDisplay () =
        let caseInfo, _ = FSharpValue.GetUnionFields(this, typeof<FSharpExternalSymbol>)
        sprintf "%s %O" caseInfo.Name this
