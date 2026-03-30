// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Symbol collection pre-pass ("Enter" phase) for order-independent compilation.
/// Scans all parsed files and collects top-level declarations into stub Entity shells
/// that are folded into TcEnv before type checking begins.
module internal FSharp.Compiler.SymbolCollection

open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TcGlobals

/// What we know about a type declaration from syntax alone
type TypeDeclStub =
    { Name: Ident
      Kind: SynTypeDefnKind
      TypeParamCount: int
      Accessibility: SynAccess option
      RecordFieldNames: Ident list
      UnionCaseNames: Ident list
      Range: range
      FileIndex: int }

/// What we know about a value/function declaration from syntax alone
type ValueDeclStub =
    { Name: Ident
      Accessibility: SynAccess option
      Range: range
      FileIndex: int }

/// What we know about a module from syntax alone
type ModuleDeclStub =
    { Name: Ident
      QualifiedName: Ident list
      Accessibility: SynAccess option
      IsAutoOpen: bool
      Kind: SynModuleOrNamespaceKind
      Types: TypeDeclStub list
      Values: ValueDeclStub list
      NestedModules: ModuleDeclStub list
      Range: range
      FileIndex: int }

/// The collected declarations for one file
type FileDeclarations =
    { FileIndex: int
      FileName: string
      QualifiedName: QualifiedNameOfFile
      TopLevelModules: ModuleDeclStub list
      Opens: LongIdent list
      IdentifierRefs: LongIdent list }

/// Walk a parsed AST and extract top-level declarations.
/// This is a shallow pass — no type checking, no expression walking for imports.
val collectFileDeclarations: fileIndex: int -> fileName: string -> parsedInput: ParsedInput -> FileDeclarations

/// Convert collected file declarations into a ModuleOrNamespaceType stub
/// containing Entity shells with names and arities but no type representations.
val buildFileStub: _g: TcGlobals -> fileDecls: FileDeclarations -> QualifiedNameOfFile * ModuleOrNamespaceType

/// Run the full enter phase: collect declarations from all files, build stubs,
/// and fold them into the given TcEnv via AddLocalRootModuleOrNamespace.
/// Returns the pre-populated TcEnv ready for type checking.
val runEnterPhase:
    g: TcGlobals ->
    amap: Import.ImportMap ->
    tcEnv: CheckBasics.TcEnv ->
    parsedInputs: (string * ParsedInput) array ->
    CheckBasics.TcEnv * FileDeclarations array
