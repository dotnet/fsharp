// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CreateILModule

open Internal.Utilities.Library
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.StrongNameSign
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.IlxGen
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

/// Represents the configuration settings used to perform strong-name signing
type StrongNameSigningInfo

/// Validate the attributes and configuration settings used to perform strong-name signing
val ValidateKeySigningAttributes: tcConfig: TcConfig * tcGlobals: TcGlobals * TopAttribs -> StrongNameSigningInfo

/// Get the object used to perform strong-name signing
val GetStrongNameSigner: signingInfo: StrongNameSigningInfo -> ILStrongNameSigner option

/// Helpers for finding attributes
module AttributeHelpers =
    val TryFindStringAttribute: g: TcGlobals -> attrib: string -> attribs: Attribs -> string option

module MainModuleBuilder =

    /// Put together all the pieces of information to create the overall IL ModuleDef for
    /// the generated assembly
    val CreateMainModule:
        ctok: CompilationThreadToken *
        tcConfig: TcConfig *
        tcGlobals: TcGlobals *
        tcImports: TcImports *
        pdbfile: 't option *
        assemblyName: string *
        outfile: string *
        topAttrs: TopAttribs *
        sigDataAttributes: ILAttribute list *
        sigDataResources: ILResource list *
        optDataResources: ILResource list *
        codegenResults: IlxGenResults *
        assemVerFromAttrib: ILVersionInfo option *
        metadataVersion: string *
        secDecls: ILSecurityDecls ->
            ILModuleDef

    /// For unit testing
    val ComputeILFileVersion:
        findStringAttr: (string -> string option) -> assemblyVersion: ILVersionInfo -> ILVersionInfo

    /// For unit testing
    val ComputeProductVersion: findStringAttr: (string -> string option) -> fileVersion: ILVersionInfo -> string

    /// For unit testing
    val ConvertProductVersionToILVersionInfo: string -> ILVersionInfo
