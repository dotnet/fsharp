// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices.ServiceFormatting

open System
open Fantomas.FormatConfig
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

[<Sealed>]
type CodeFormatter =
    /// Parse a source string using given config
    static member Parse : fileName:string * source:string -> ParsedInput
    /// Parse a source string using given config
    static member ParseAsync : fileName:string * source:string * projectOptions:FSharpProjectOptions * checker:FSharpChecker -> Async<ParsedInput> 
    /// Format an abstract syntax tree using an optional source for looking up literals
    static member FormatAST : ast:ParsedInput * fileName:string * source:string option * config:FormatConfig -> string
    
    /// Infer selection around cursor by looking for a pair of '[' and ']', '{' and '}' or '(' and ')'. 
    static member InferSelectionFromCursorPos : fileName:string * cursorPos:pos * source:string -> range
    
    /// Format around cursor delimited by '[' and ']', '{' and '}' or '(' and ')' using given config; keep other parts unchanged. 
    /// (Only use in testing.)
    static member internal FormatAroundCursorAsync : 
        fileName:string * cursorPos:pos * source:string * config:FormatConfig * projectOptions:FSharpProjectOptions * checker:FSharpChecker -> Async<string>
    
    /// Format a source string using given config
    static member FormatDocument : 
        fileName:string * source:string * config:FormatConfig -> string
    
    /// Format a source string using given config
    static member FormatDocumentAsync : 
        fileName:string * source:string * config:FormatConfig * projectOptions:FSharpProjectOptions * checker:FSharpChecker -> Async<string>
    
    /// Format a part of source string using given config, and return the (formatted) selected part only.
    /// Beware that the range argument is inclusive. If the range has a trailing newline, it will appear in the formatted result.
    static member FormatSelection : 
        fileName:string * selection:range * source:string * config:FormatConfig -> string
    
    /// Format a part of source string using given config, and return the (formatted) selected part only.
    /// Beware that the range argument is inclusive. If the range has a trailing newline, it will appear in the formatted result.
    static member FormatSelectionAsync : 
        fileName:string * selection:range * source:string * config:FormatConfig * projectOptions:FSharpProjectOptions * checker:FSharpChecker -> Async<string>
   
    /// Format a selected part of source string using given config; keep other parts unchanged. 
    /// (Only use in testing.)
    static member internal FormatSelectionInDocumentAsync : 
        fileName:string * selection:range * source:string * config:FormatConfig * projectOptions:FSharpProjectOptions * checker:FSharpChecker -> Async<string>
     
    /// Check whether an AST consists of parsing errors 
    static member IsValidAST : ast:ParsedInput -> bool
    /// Check whether an input string is invalid in F# by looking for erroneous nodes in ASTs
    static member IsValidFSharpCode : fileName:string * source:string -> bool
    /// Check whether an input string is invalid in F# by looking for erroneous nodes in ASTs
    static member IsValidFSharpCodeAsync : fileName:string * source:string * projectOptions:FSharpProjectOptions * checker:FSharpChecker -> Async<bool>
    
    static member MakePos : line:int * col:int -> pos
    static member MakeRange : fileName:string * startLine:int * startCol:int * endLine:int * endCol:int -> range

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CodeFormatter =
    /// Parse a source code string
    [<Obsolete("Please use 'CodeFormatter.ParseAsync' instead.")>]
    val parse : isFsiFile:bool -> sourceCode:string -> ParsedInput

    [<Obsolete("Please use 'CodeFormatter.MakePos' instead.")>]
    val makePos : line:int -> col:int -> pos

    [<Obsolete("Please use 'CodeFormatter.MakeRange' instead.")>]
    val makeRange : startLine:int -> startCol:int -> endLine:int -> endCol:int -> range

    /// Check whether an AST consists of parsing errors 
    [<Obsolete("Please use 'CodeFormatter.IsValidAST' instead.")>]
    val isValidAST : ast:ParsedInput -> bool

    /// Check whether an input string is invalid in F# by looking for erroneous nodes in ASTs
    [<Obsolete("Please use 'CodeFormatter.IsValidFSharpCodeAsync' instead.")>]
    val isValidFSharpCode : isFsiFile:bool -> sourceCode:string -> bool

    /// Format a source string using given config
    [<Obsolete("Please use 'CodeFormatter.FormatDocumentAsync' instead.")>]
    val formatSourceString : isFsiFile:bool -> sourceCode:string -> config:FormatConfig -> string

    /// Format an abstract syntax tree using given config
    [<Obsolete("Please use 'CodeFormatter.FormatAST' instead.")>]
    val formatAST : ast:ParsedInput -> sourceCode:string option -> config:FormatConfig -> string

    /// Format a part of source string using given config, and return the (formatted) selected part only.
    /// Beware that the range argument is inclusive. If the range has a trailing newline, it will appear in the formatted result.
    [<Obsolete("Please use 'CodeFormatter.FormatSelectionAsync' instead.")>]
    val formatSelectionOnly : isFsiFile:bool -> range:range -> sourceCode:string -> config:FormatConfig -> string

    /// Format a selected part of source string using given config; expanded selected ranges to parsable ranges. 
    [<Obsolete("Please use 'CodeFormatter.FormatSelectionAsync' instead.")>]
    val formatSelectionExpanded : isFsiFile:bool -> range:range -> sourceCode:string -> config:FormatConfig -> string * range

    /// Format a selected part of source string using given config; keep other parts unchanged. 
    [<Obsolete("Please use 'CodeFormatter.FormatSelectionAsync' instead.")>]
    val formatSelectionFromString : isFsiFile:bool -> range:range -> sourceCode:string -> config:FormatConfig -> string

    /// Format around cursor delimited by '[' and ']', '{' and '}' or '(' and ')' using given config; keep other parts unchanged. 
    [<Obsolete("Please use 'CodeFormatter.FormatSelectionAsync' instead.")>]
    val formatAroundCursor : isFsiFile:bool -> cursorPos:pos -> sourceCode:string -> config:FormatConfig -> string

    /// Infer selection around cursor by looking for a pair of '[' and ']', '{' and '}' or '(' and ')'. 
    [<Obsolete("Please use 'CodeFormatter.InferSelectionFromCursorPos' instead.")>]
    val inferSelectionFromCursorPos : cursorPos:pos -> sourceCode:string -> range