// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Primary relations on types and signatures, with the exception of
/// constraint solving and method overload resolution.
module internal FSharp.Compiler.SignatureConformance

open System.Text

open FSharp.Compiler
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.InfoReader

[<NoComparison; NoEquality>]
exception RequiredButNotSpecified of DisplayEnv * ModuleOrNamespaceRef * string * (StringBuilder -> unit) * range

[<NoComparison; NoEquality>]
exception ValueNotContained of
    DisplayEnv *
    InfoReader *
    ModuleOrNamespaceRef *
    Val *
    Val *
    (string * string * string -> string)

[<NoComparison; NoEquality>]
exception UnionCaseNotContained of DisplayEnv * InfoReader * Tycon * UnionCase * UnionCase * (string * string -> string)

[<NoComparison; NoEquality>]
exception FSharpExceptionNotContained of DisplayEnv * InfoReader * Tycon * Tycon * (string * string -> string)

[<NoComparison; NoEquality>]
exception FieldNotContained of
    DisplayEnv *
    InfoReader *
    Tycon *
    Tycon *
    RecdField *
    RecdField *
    (string * string -> string)

[<NoComparison; NoEquality>]
exception InterfaceNotRevealed of DisplayEnv * TType * range

[<NoComparison; NoEquality>]
exception ArgumentsInSigAndImplMismatch of sigArg: Ident * implArg: Ident

type Checker =

    new:
        g: TcGlobals.TcGlobals *
        amap: Import.ImportMap *
        denv: DisplayEnv *
        remapInfo: SignatureRepackageInfo *
        checkingSig: bool ->
            Checker

    member CheckSignature:
        aenv: TypeEquivEnv ->
        infoReader: InfoReader ->
        implModRef: ModuleOrNamespaceRef ->
        signModType: ModuleOrNamespaceType ->
            bool

    member CheckTypars: m: range -> aenv: TypeEquivEnv -> implTypars: Typars -> signTypars: Typars -> bool

/// Check the names add up between a signature and its implementation. We check this first.
val CheckNamesOfModuleOrNamespace:
    denv: DisplayEnv ->
    infoReader: InfoReader ->
    implModRef: ModuleOrNamespaceRef ->
    signModType: ModuleOrNamespaceType ->
        bool
