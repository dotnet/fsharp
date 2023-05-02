// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.EditorServices

open FSharp.Reflection
open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL.IL
open System.Diagnostics

module Option =

    let ofOptionList (xs: 'a option list) : 'a list option =

        if xs |> List.forall Option.isSome then
            xs |> List.map Option.get |> Some
        else
            None

/// Represents a type in an external (non F#) assembly.
[<RequireQualifiedAccess>]
type FindDeclExternalType =
    /// Type defined in non-F# assembly.
    | Type of fullName: string * genericArgs: FindDeclExternalType list

    /// Array of type that is defined in non-F# assembly.
    | Array of inner: FindDeclExternalType

    /// Pointer defined in non-F# assembly.
    | Pointer of inner: FindDeclExternalType

    /// Type variable defined in non-F# assembly.
    | TypeVar of typeName: string

    override this.ToString() =
        match this with
        | Type (name, genericArgs) ->
            match genericArgs with
            | [] -> ""
            | args -> args |> List.map (sprintf "%O") |> String.concat ", " |> sprintf "<%s>"
            |> sprintf "%s%s" name
        | Array inner -> sprintf "%O[]" inner
        | Pointer inner -> sprintf "&%O" inner
        | TypeVar name -> sprintf "'%s" name

module FindDeclExternalType =
    let rec tryOfILType (typeVarNames: string array) (ilType: ILType) =

        match ilType with
        | ILType.Array (_, inner) -> tryOfILType typeVarNames inner |> Option.map FindDeclExternalType.Array
        | ILType.Boxed tyspec
        | ILType.Value tyspec ->
            tyspec.GenericArgs
            |> List.map (tryOfILType typeVarNames)
            |> Option.ofOptionList
            |> Option.map (fun genericArgs -> FindDeclExternalType.Type(tyspec.FullName, genericArgs))
        | ILType.Ptr inner -> tryOfILType typeVarNames inner |> Option.map FindDeclExternalType.Pointer
        | ILType.TypeVar ordinal ->
            typeVarNames
            |> Array.tryItem (int ordinal)
            |> Option.map (fun typeVarName -> FindDeclExternalType.TypeVar typeVarName)
        | _ -> None

[<RequireQualifiedAccess>]
type FindDeclExternalParam =

    | Param of parameterType: FindDeclExternalType

    | Byref of parameterType: FindDeclExternalType

    member c.IsByRef =
        match c with
        | Byref _ -> true
        | _ -> false

    member c.ParameterType =
        match c with
        | Byref ty -> ty
        | Param ty -> ty

    static member Create(parameterType, isByRef) =
        if isByRef then Byref parameterType else Param parameterType

    override this.ToString() =
        match this with
        | Param t -> t.ToString()
        | Byref t -> sprintf "ref %O" t

module FindDeclExternalParam =
    let tryOfILType (typeVarNames: string array) =
        function
        | ILType.Byref inner ->
            FindDeclExternalType.tryOfILType typeVarNames inner
            |> Option.map FindDeclExternalParam.Byref
        | ilType ->
            FindDeclExternalType.tryOfILType typeVarNames ilType
            |> Option.map FindDeclExternalParam.Param

    let tryOfILTypes typeVarNames ilTypes =
        ilTypes |> List.map (tryOfILType typeVarNames) |> Option.ofOptionList

[<RequireQualifiedAccess>]
[<DebuggerDisplay "{ToDebuggerDisplay(),nq}">]
type FindDeclExternalSymbol =
    | Type of fullName: string
    | Constructor of typeName: string * args: FindDeclExternalParam list
    | Method of typeName: string * name: string * paramSyms: FindDeclExternalParam list * genericArity: int
    | Field of typeName: string * name: string
    | Event of typeName: string * name: string
    | Property of typeName: string * name: string

    override this.ToString() =
        match this with
        | Type fullName -> fullName
        | Constructor (typeName, args) ->
            args
            |> List.map (sprintf "%O")
            |> String.concat ", "
            |> sprintf "%s..ctor(%s)" typeName
        | Method (typeName, name, args, genericArity) ->
            let genericAritySuffix = if genericArity > 0 then sprintf "`%d" genericArity else ""

            args
            |> List.map (sprintf "%O")
            |> String.concat ", "
            |> sprintf "%s.%s%s(%s)" typeName name genericAritySuffix
        | Field (typeName, name)
        | Event (typeName, name)
        | Property (typeName, name) -> sprintf "%s.%s" typeName name

    member this.ToDebuggerDisplay() =
        let caseInfo, _ = FSharpValue.GetUnionFields(this, typeof<FindDeclExternalSymbol>)
        sprintf "%s %O" caseInfo.Name this

[<RequireQualifiedAccess>]
type FindDeclFailureReason =

    // generic reason: no particular information about error
    | Unknown of message: string

    // source code file is not available
    | NoSourceCode

    // trying to find declaration of ProvidedType without TypeProviderDefinitionLocationAttribute
    | ProvidedType of typeName: string

    // trying to find declaration of ProvidedMember without TypeProviderDefinitionLocationAttribute
    | ProvidedMember of memberName: string

[<RequireQualifiedAccess>]
type FindDeclResult =

    /// declaration not found + reason
    | DeclNotFound of FindDeclFailureReason

    /// found declaration
    | DeclFound of location: range

    /// Indicates an external declaration was found
    | ExternalDecl of assembly: string * externalSym: FindDeclExternalSymbol
