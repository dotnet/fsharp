// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open FSharp.Reflection
open FSharp.Compiler.AbstractIL.IL
open System.Diagnostics

module private Option =

    let ofOptionList (xs : 'a option list) : 'a list option =

        if xs |> List.forall Option.isSome then
            xs |> List.map Option.get |> Some
        else
            None
    
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
        
module ExternalType =
    let rec internal tryOfILType (typeVarNames: string array) (ilType: ILType) =
        
        match ilType with
        | ILType.Array (_, inner) ->
            tryOfILType typeVarNames inner |> Option.map ExternalType.Array
        | ILType.Boxed tyspec
        | ILType.Value tyspec ->
            tyspec.GenericArgs
            |> List.map (tryOfILType typeVarNames)
            |> Option.ofOptionList
            |> Option.map (fun genericArgs -> ExternalType.Type (tyspec.FullName, genericArgs))
        | ILType.Ptr inner ->
            tryOfILType typeVarNames inner |> Option.map ExternalType.Pointer
        | ILType.TypeVar ordinal ->
            typeVarNames
            |> Array.tryItem (int ordinal)
            |> Option.map (fun typeVarName -> ExternalType.TypeVar typeVarName)
        | _ ->
            None

[<RequireQualifiedAccess>]
type ParamTypeSymbol =
    | Param of ExternalType
    | Byref of ExternalType
    override this.ToString () =
        match this with
        | Param t -> t.ToString()
        | Byref t -> sprintf "ref %O" t

module ParamTypeSymbol =
    let rec internal tryOfILType (typeVarNames : string array) =
        function
        | ILType.Byref inner -> ExternalType.tryOfILType typeVarNames inner |> Option.map ParamTypeSymbol.Byref
        | ilType -> ExternalType.tryOfILType typeVarNames ilType |> Option.map ParamTypeSymbol.Param

    let internal tryOfILTypes typeVarNames ilTypes =
        ilTypes |> List.map (tryOfILType typeVarNames) |> Option.ofOptionList
    
[<RequireQualifiedAccess>]
[<DebuggerDisplay "{ToDebuggerDisplay(),nq}">]
type ExternalSymbol =
    | Type of fullName: string
    | Constructor of typeName: string * args: ParamTypeSymbol list
    | Method of typeName: string * name: string * paramSyms: ParamTypeSymbol list * genericArity: int
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

    member internal this.ToDebuggerDisplay () =
        let caseInfo, _ = FSharpValue.GetUnionFields(this, typeof<ExternalSymbol>)
        sprintf "%s %O" caseInfo.Name this
