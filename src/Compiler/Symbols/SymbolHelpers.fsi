// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Helpers for quick info and information about items
//----------------------------------------------------------------------------

namespace FSharp.Compiler.Symbols

open Internal.Utilities.Library
open FSharp.Compiler
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Infos
open FSharp.Compiler.NameResolution
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Text
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

/// Describe a comment as either a block of text or a file+signature reference into an intellidoc file.
//
// Note: instances of this type do not hold any references to any compiler resources.
[<RequireQualifiedAccess>]
type public FSharpXmlDoc =
    /// No documentation is available
    | None

    /// The text for documentation for in-memory references.
    | FromXmlText of XmlDoc

    /// Indicates that the XML for the documentation can be found in a .xml documentation file for the given DLL, using the given signature key
    | FromXmlFile of dllName: string * xmlSig: string

/// A declaration whose `///` comment a tooltip's documentation is read from: its file, and a path that names it there
/// from that file's parse alone. The path is the enclosing namespaces, modules and types by name, then the declaration's
/// kind and name, and for a member its place among the members of its kind and name.
type public FSharpDocOwner = { FileName: string; Path: string }

/// Where a tooltip element's documentation comes from: the declarations whose comments it reads, and how FCS makes the
/// documentation from them, so it can be made again from newer comments with the same result as a check of them.
[<Sealed>]
type public FSharpDocSource =
    internal new:
        owners: FSharpDocOwner list *
        stale: XmlDoc *
        inherits: (string * FSharpDocOwner list * XmlDoc) list *
        fallback: FSharpXmlDoc *
        compose: (XmlDoc option -> (unit -> (string * string) option) -> (unit -> FSharpXmlDoc) -> FSharpXmlDoc) ->
            FSharpDocSource

    /// The declarations whose comment this is, in FCS's order: the first with a non-empty comment is the one shown
    member Owners: FSharpDocOwner list

    /// For an `<inheritdoc/>`, the declarations it can inherit from
    member InheritOwners: FSharpDocOwner list

    /// The documentation from each declaration's comment as `commentOf` gives it: XmlDoc.Empty for none, and ValueNone
    /// for a declaration whose comment isn't known, which makes it the one the check has
    member Compose: commentOf: (FSharpDocOwner -> XmlDoc voption) -> FSharpXmlDoc

// Implementation details used by other code in the compiler
module internal SymbolHelpers =

    val ParamNameAndTypesOfUnaryCustomOperation: TcGlobals -> MethInfo -> ParamNameAndType list

    val GetXmlCommentForItem: InfoReader -> range -> Item -> FSharpXmlDoc

    val RemoveDuplicateItems: TcGlobals -> ItemWithInst list -> ItemWithInst list

    val RemoveExplicitlySuppressed: TcGlobals -> ItemWithInst list -> ItemWithInst list

    val GetF1Keyword: TcGlobals -> Item -> string option

    val rangeOfItem: TcGlobals -> bool option -> Item -> range option

    val fileNameOfItem: TcGlobals -> string option -> range -> Item -> string

    val FullNameOfItem: TcGlobals -> Item -> string

    val ccuOfItem: TcGlobals -> Item -> CcuThunk option

    val IsAttribute: InfoReader -> Item -> bool

    val IsExplicitlySuppressed: TcGlobals -> Item -> bool

    val SelectMethodGroupItems2: TcGlobals -> range -> ItemWithInst -> ItemWithInst list

#if !NO_TYPEPROVIDERS
    [<return: Struct>]
    val (|ItemIsProvidedType|_|): TcGlobals -> Item -> TyconRef voption

    [<return: Struct>]
    val (|ItemIsWithStaticArguments|_|):
        range -> TcGlobals -> Item -> Tainted<TypeProviders.ProvidedParameterInfo>[] voption

    [<return: Struct>]
    val (|ItemIsProvidedTypeWithStaticArguments|_|):
        range -> TcGlobals -> Item -> Tainted<TypeProviders.ProvidedParameterInfo>[] voption
#endif

    val SimplerDisplayEnv: DisplayEnv -> DisplayEnv

    val ItemDisplayPartialEquality: g: TcGlobals -> IPartialEqualityComparer<Item>

    val GetXmlCommentForMethInfoItem: infoReader: InfoReader -> m: range -> d: Item -> minfo: MethInfo -> FSharpXmlDoc

    /// What the doc sources of one tooltip share: each type's members, scanned once
    [<Sealed>]
    type DocScope =
        /// `signature`: the check's signature, in which a declaring module is found as it is now
        new:
            signature: ModuleOrNamespaceType option *
            fileName: string *
            defined: Lazy<System.Collections.Generic.HashSet<struct (int * int * string)>> *
            bound: Lazy<System.Collections.Generic.HashSet<struct (Stamp * int * int)>> ->
                DocScope

    /// Where GetXmlCommentForItem's documentation is read from. None when that isn't known.
    val GetDocSourceOfItem:
        infoReader: InfoReader -> scope: DocScope -> m: range -> item: Item -> FSharpDocSource option

    /// Where GetXmlCommentForMethInfoItem's documentation is read from, `loaded` from an XML file when GetXmlDocFromLoader
    /// is applied to it
    val GetDocSourceOfMethInfoItem:
        infoReader: InfoReader ->
        scope: DocScope ->
        m: range ->
        d: Item ->
        minfo: MethInfo ->
        loaded: bool ->
            FSharpDocSource option

    val FormatTyparMapping: denv: DisplayEnv -> prettyTyparInst: TyparInstantiation -> Layout list
