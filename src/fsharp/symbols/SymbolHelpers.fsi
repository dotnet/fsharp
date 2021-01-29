// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Helpers for quick info and information about items
//----------------------------------------------------------------------------

namespace FSharp.Compiler.Diagnostics

    open System
    open FSharp.Compiler.Text
    open FSharp.Compiler.ErrorLogger

    [<RequireQualifiedAccess>]
    type public FSharpDiagnosticSeverity = 
        | Hidden
        | Info
        | Warning 
        | Error

    /// Object model for diagnostics
    [<Class>]
    type public FSharpDiagnostic = 
        member FileName: string

        member Start: pos

        member End: pos

        member StartLineAlternate: int

        member EndLineAlternate: int

        member StartColumn: int

        member EndColumn: int

        member Range: range

        member Severity: FSharpDiagnosticSeverity

        member Message: string

        member Subcategory: string

        member ErrorNumber: int

        static member internal CreateFromExceptionAndAdjustEof: PhasedDiagnostic * isError: bool * range * lastPosInFile: (int*int) * suggestNames: bool -> FSharpDiagnostic
        static member internal CreateFromException: PhasedDiagnostic * isError: bool * range * suggestNames: bool -> FSharpDiagnostic

        static member NewlineifyErrorString: message:string -> string

        /// Newlines are recognized and replaced with (ASCII 29, the 'group separator'), 
        /// which is decoded by the IDE with 'NewlineifyErrorString' back into newlines, so that multi-line errors can be displayed in QuickInfo
        static member NormalizeErrorString: text:string -> string
  

    //----------------------------------------------------------------------------
    // Internal only

    // Implementation details used by other code in the compiler    
    [<Sealed>]
    type internal ErrorScope = 
        interface IDisposable
        new : unit -> ErrorScope
        member Diagnostics : FSharpDiagnostic list
        static member Protect<'a> : range -> (unit->'a) -> (string->'a) -> 'a

    /// An error logger that capture errors, filtering them according to warning levels etc.
    type internal CompilationErrorLogger = 
        inherit ErrorLogger

        /// Create the error logger
        new: debugName:string * options: FSharpDiagnosticOptions -> CompilationErrorLogger
            
        /// Get the captured errors
        member GetErrors: unit -> (PhasedDiagnostic * FSharpDiagnosticSeverity)[]

    /// This represents the global state established as each task function runs as part of the build.
    ///
    /// Use to reset error and warning handlers.
    type internal CompilationGlobalsScope =
        new : ErrorLogger * BuildPhase -> CompilationGlobalsScope
        interface IDisposable

    module internal DiagnosticHelpers = 
        val ReportError: FSharpDiagnosticOptions * allErrors: bool * mainInputFileName: string * fileInfo: (int * int) * (PhasedDiagnostic * FSharpDiagnosticSeverity) * suggestNames: bool -> FSharpDiagnostic list

        val CreateDiagnostics: FSharpDiagnosticOptions * allErrors: bool * mainInputFileName: string * seq<(PhasedDiagnostic * FSharpDiagnosticSeverity)> * suggestNames: bool -> FSharpDiagnostic[]

namespace FSharp.Compiler.CodeAnalysis

    open Internal.Utilities.Library
    open FSharp.Compiler 
    open FSharp.Compiler.TcGlobals 
    open FSharp.Compiler.Infos
    open FSharp.Compiler.NameResolution
    open FSharp.Compiler.InfoReader
    open FSharp.Compiler.Text
    open FSharp.Compiler.TextLayout
    open FSharp.Compiler.TypedTree
    open FSharp.Compiler.TypedTreeOps

    /// Describe a comment as either a block of text or a file+signature reference into an intellidoc file.
    //
    // Note: instances of this type do not hold any references to any compiler resources.
    [<RequireQualifiedAccess>]
    type public FSharpXmlDoc =
        /// No documentation is available
        | None

        /// The text for documentation for in-memory references.  Here unprocessedText is the `\n` concatenated
        /// text of the original source and processsedXmlLines is the 
        /// XML produced after all checking and processing by the F# compiler, including
        /// insertion of summary tags, encoding and resolving of cross-references if
        // supported.
        | Text of unprocessedLines: string[] * elaboratedXmlLines: string[]

        /// Indicates that the XML for the documentation can be found in a .xml documentation file, using the given signature key
        | XmlDocFileSignature of file: string * xmlSig: string

    // Implementation details used by other code in the compiler    
    module internal SymbolHelpers =
        val ParamNameAndTypesOfUnaryCustomOperation : TcGlobals -> MethInfo -> ParamNameAndType list

        val GetXmlDocSigOfEntityRef : InfoReader -> range -> EntityRef -> (string option * string) option

        val GetXmlDocSigOfScopedValRef : TcGlobals -> TyconRef -> ValRef -> (string option * string) option

        val GetXmlDocSigOfILFieldInfo : InfoReader -> range -> ILFieldInfo -> (string option * string) option

        val GetXmlDocSigOfRecdFieldInfo : RecdFieldInfo -> (string option * string) option

        val GetXmlDocSigOfUnionCaseInfo : UnionCaseInfo -> (string option * string) option

        val GetXmlDocSigOfMethInfo : InfoReader -> range -> MethInfo -> (string option * string) option

        val GetXmlDocSigOfValRef : TcGlobals -> ValRef -> (string option * string) option

        val GetXmlDocSigOfProp : InfoReader -> range -> PropInfo -> (string option * string) option

        val GetXmlDocSigOfEvent : InfoReader -> range -> EventInfo -> (string option * string) option

        val GetXmlCommentForItem : InfoReader -> range -> Item -> FSharpXmlDoc

        val RemoveDuplicateItems : TcGlobals -> ItemWithInst list -> ItemWithInst list

        val RemoveExplicitlySuppressed : TcGlobals -> ItemWithInst list -> ItemWithInst list

        val GetF1Keyword : TcGlobals -> Item -> string option

        val rangeOfItem : TcGlobals -> bool option -> Item -> range option

        val fileNameOfItem : TcGlobals -> string option -> range -> Item -> string

        val FullNameOfItem : TcGlobals -> Item -> string

        val ccuOfItem : TcGlobals -> Item -> CcuThunk option

        val IsAttribute : InfoReader -> Item -> bool

        val IsExplicitlySuppressed : TcGlobals -> Item -> bool

        val FlattenItems : TcGlobals -> range -> Item -> Item list

#if !NO_EXTENSIONTYPING
        val (|ItemIsProvidedType|_|) : TcGlobals -> Item -> TyconRef option

        val (|ItemIsWithStaticArguments|_|): range -> TcGlobals -> Item -> Tainted<ExtensionTyping.ProvidedParameterInfo>[] option

        val (|ItemIsProvidedTypeWithStaticArguments|_|): range -> TcGlobals -> Item -> Tainted<ExtensionTyping.ProvidedParameterInfo>[] option
#endif

        val SimplerDisplayEnv : DisplayEnv -> DisplayEnv

        val ItemDisplayPartialEquality: g:TcGlobals -> IPartialEqualityComparer<Item>    

        val GetXmlCommentForMethInfoItem: infoReader:InfoReader -> m:range -> d:Item -> minfo:MethInfo -> FSharpXmlDoc    
        
        val FormatTyparMapping: denv:DisplayEnv -> prettyTyparInst:TyparInst -> Layout list

