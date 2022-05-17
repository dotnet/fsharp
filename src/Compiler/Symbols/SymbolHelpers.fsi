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

    val FlattenItems: TcGlobals -> range -> ItemWithInst -> ItemWithInst list

#if !NO_TYPEPROVIDERS
    val (|ItemIsProvidedType|_|): TcGlobals -> Item -> TyconRef option

    val (|ItemIsWithStaticArguments|_|):
        range -> TcGlobals -> Item -> Tainted<TypeProviders.ProvidedParameterInfo>[] option

    val (|ItemIsProvidedTypeWithStaticArguments|_|):
        range -> TcGlobals -> Item -> Tainted<TypeProviders.ProvidedParameterInfo>[] option
#endif

    val SimplerDisplayEnv: DisplayEnv -> DisplayEnv

    val ItemDisplayPartialEquality: g: TcGlobals -> IPartialEqualityComparer<Item>

    val GetXmlCommentForMethInfoItem: infoReader: InfoReader -> m: range -> d: Item -> minfo: MethInfo -> FSharpXmlDoc

    val FormatTyparMapping: denv: DisplayEnv -> prettyTyparInst: TyparInstantiation -> Layout list
