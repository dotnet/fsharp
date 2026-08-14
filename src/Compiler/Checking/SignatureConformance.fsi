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

type TypeMismatchSource =
    | NullnessOnlyMismatch
    | RegularMismatch

exception RequiredButNotSpecified of DisplayEnv * ModuleOrNamespaceRef * string * (RichTextBuilder -> unit) * range

exception ValueNotContained of
    kind: TypeMismatchSource *
    DisplayEnv *
    InfoReader *
    ModuleOrNamespaceRef *
    Val *
    Val *
    (RichText * RichText * RichText -> RichText)

exception UnionCaseNotContained of
    DisplayEnv *
    InfoReader *
    Tycon *
    UnionCase *
    UnionCase *
    (RichText * RichText -> RichText)

exception FSharpExceptionNotContained of DisplayEnv * InfoReader * Tycon * Tycon * (RichText * RichText -> RichText)

exception FieldNotContained of
    kind: TypeMismatchSource *
    DisplayEnv *
    InfoReader *
    Tycon *
    Tycon *
    RecdField *
    RecdField *
    (RichText * RichText -> RichText)

exception InterfaceNotRevealed of DisplayEnv * TType * range

exception ArgumentsInSigAndImplMismatch of sigArg: Ident * implArg: Ident

exception DefinitionsInSigAndImplNotCompatibleAbbreviationsDiffer of
    denv: DisplayEnv *
    implTycon: Tycon *
    sigTycon: Tycon *
    implTypeAbbrev: TType *
    sigTypeAbbrev: TType *
    range: range

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
