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

/// Whether conformance checks on this thread leave the other ranges of the signature's items as they are, writing
/// only the implementation's: the signature's items are shared with every file checked after the signature.
[<Sealed; AbstractClass>]
type SignatureRanges =
    static member Keep: bool with get, set

/// Whether conformance checks on this thread record, in MutationLog.Current, how to undo what they change in the
/// implementation's items, so a check can see what conformance would leave and then have it undone.
[<Sealed; AbstractClass>]
type UndoableConformance =
    static member Recording: bool with get, set

    /// When set, Isolate records in it, and only in it, how to undo what the conformance check it runs changes.
    static member Log: ResizeArray<unit -> unit> | null with get, set

    /// Runs a conformance check, recording in Log how to undo what it changes when Log is set.
    static member Isolate: check: (unit -> 'T) -> 'T

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
