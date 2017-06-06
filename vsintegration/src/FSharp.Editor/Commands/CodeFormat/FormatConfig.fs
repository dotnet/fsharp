// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.FormatConfig

open System
open System.IO
open System.Collections.Generic
open System.Text.RegularExpressions
open System.CodeDom.Compiler

open Microsoft.FSharp.Compiler.Range
open Microsoft.VisualStudio.FSharp.Editor.TokenMatcher

type FormatException(msg : string) =
    inherit Exception(msg)

type Num = int

type FormatConfig = 
    { /// Number of spaces for each indentation
      IndentSpaceNum : Num;
      /// The column where we break to new lines
      PageWidth : Num;
      SemicolonAtEndOfLine : bool;
      SpaceBeforeArgument : bool;
      SpaceBeforeColon : bool;
      SpaceAfterComma : bool;
      SpaceAfterSemicolon : bool;
      IndentOnTryWith : bool;
      /// Reordering and deduplicating open statements
      ReorderOpenDeclaration : bool;
      SpaceAroundDelimiter : bool;
      /// Prettyprinting based on ASTs only
      StrictMode : bool }

    static member Default = 
        { IndentSpaceNum = 4; PageWidth = 80;
          SemicolonAtEndOfLine = false; SpaceBeforeArgument = true; SpaceBeforeColon = true;
          SpaceAfterComma = true; SpaceAfterSemicolon = true; 
          IndentOnTryWith = false; ReorderOpenDeclaration = false; 
          SpaceAroundDelimiter = true; StrictMode = false }

    static member create(indentSpaceNum, pageWith, semicolonAtEndOfLine, 
                         spaceBeforeArgument, spaceBeforeColon, spaceAfterComma, 
                         spaceAfterSemicolon, indentOnTryWith, reorderOpenDeclaration) =
        { FormatConfig.Default with
              IndentSpaceNum = indentSpaceNum; 
              PageWidth = pageWith;
              SemicolonAtEndOfLine = semicolonAtEndOfLine; 
              SpaceBeforeArgument = spaceBeforeArgument; 
              SpaceBeforeColon = spaceBeforeColon;
              SpaceAfterComma = spaceAfterComma; 
              SpaceAfterSemicolon = spaceAfterSemicolon; 
              IndentOnTryWith = indentOnTryWith; 
              ReorderOpenDeclaration = reorderOpenDeclaration }

    static member create(indentSpaceNum, pageWith, semicolonAtEndOfLine, 
                         spaceBeforeArgument, spaceBeforeColon, spaceAfterComma, 
                         spaceAfterSemicolon, indentOnTryWith, reorderOpenDeclaration, spaceAroundDelimiter) =
        { FormatConfig.Default with
              IndentSpaceNum = indentSpaceNum; 
              PageWidth = pageWith;
              SemicolonAtEndOfLine = semicolonAtEndOfLine; 
              SpaceBeforeArgument = spaceBeforeArgument; 
              SpaceBeforeColon = spaceBeforeColon;
              SpaceAfterComma = spaceAfterComma; 
              SpaceAfterSemicolon = spaceAfterSemicolon; 
              IndentOnTryWith = indentOnTryWith; 
              ReorderOpenDeclaration = reorderOpenDeclaration;
              SpaceAroundDelimiter = spaceAroundDelimiter }

    static member create(indentSpaceNum, pageWith, semicolonAtEndOfLine, 
                         spaceBeforeArgument, spaceBeforeColon, spaceAfterComma, 
                         spaceAfterSemicolon, indentOnTryWith, reorderOpenDeclaration, 
                         spaceAroundDelimiter, strictMode) =
        { FormatConfig.Default with
              IndentSpaceNum = indentSpaceNum; 
              PageWidth = pageWith;
              SemicolonAtEndOfLine = semicolonAtEndOfLine; 
              SpaceBeforeArgument = spaceBeforeArgument; 
              SpaceBeforeColon = spaceBeforeColon;
              SpaceAfterComma = spaceAfterComma; 
              SpaceAfterSemicolon = spaceAfterSemicolon; 
              IndentOnTryWith = indentOnTryWith; 
              ReorderOpenDeclaration = reorderOpenDeclaration;
              SpaceAroundDelimiter = spaceAroundDelimiter;
              StrictMode = strictMode }

/// Wrapping IndentedTextWriter with current column position
type internal ColumnIndentedTextWriter(tw : TextWriter) =
    let indentWriter = new IndentedTextWriter(tw, " ")
    let mutable col = indentWriter.Indent

    member __.Write(s : string) =
        match s.LastIndexOf('\n') with
        | -1 -> col <- col + s.Length
        | i -> col <- s.Length - i - 1
        indentWriter.Write(s)

    member __.WriteLine(s : string) =
        col <- indentWriter.Indent
        indentWriter.WriteLine(s)

    /// Current column of the page in an absolute manner
    member __.Column 
        with get() = col
        and set i = col <- i

    member __.Indent 
        with get() = indentWriter.Indent
        and set i = indentWriter.Indent <- i

    member __.InnerWriter = indentWriter.InnerWriter

    interface IDisposable with
        member __.Dispose() =
            indentWriter.Dispose()    

type internal Context = 
    { Config : FormatConfig; 
      Writer : ColumnIndentedTextWriter;
      mutable BreakLines : bool;
      BreakOn : string -> bool;
      /// The original source string to query as a last resort 
      Content : string; 
      /// Positions of new lines in the original source string
      Positions : int []; 
      /// Comments attached to appropriate locations
      Comments : Dictionary<pos, string list>;
      /// Compiler directives attached to appropriate locations
      Directives : Dictionary<pos, string> }

    /// Initialize with a string writer and use space as delimiter
    static member Default = 
        { Config = FormatConfig.Default;
          Writer = new ColumnIndentedTextWriter(new StringWriter());
          BreakLines = true; BreakOn = (fun _ -> false); 
          Content = ""; Positions = [||]; Comments = Dictionary(); Directives = Dictionary() }

    static member create config (content : string) =
        let content = String.normalizeNewLine content
        let positions = 
            content.Split('\n')
            |> Seq.map (fun s -> String.length s + 1)
            |> Seq.scan (+) 0
            |> Seq.toArray
        let (comments, directives) = filterCommentsAndDirectives content
        { Context.Default with 
            Config = config; Content = content; Positions = positions; 
            Comments = comments; Directives = directives }

    member x.With(writer : ColumnIndentedTextWriter) =
        writer.Indent <- x.Writer.Indent
        writer.Column <- x.Writer.Column
        // Use infinite column width to encounter worst-case scenario
        let config = { x.Config with PageWidth = Int32.MaxValue }
        { x with Writer = writer; Config = config }

let internal dump (ctx: Context) =
    ctx.Writer.InnerWriter.ToString()

// A few utility functions from https://github.com/fsharp/powerpack/blob/master/src/FSharp.Compiler.CodeDom/generator.fs

/// Indent one more level based on configuration
let internal indent (ctx : Context) = 
    ctx.Writer.Indent <- ctx.Writer.Indent + ctx.Config.IndentSpaceNum
    ctx

/// Unindent one more level based on configuration
let internal unindent (ctx : Context) = 
    ctx.Writer.Indent <- max 0 (ctx.Writer.Indent - ctx.Config.IndentSpaceNum)
    ctx

/// Increase indent by i spaces
let internal incrIndent i (ctx : Context) = 
    ctx.Writer.Indent <- ctx.Writer.Indent + i
    ctx

/// Decrease indent by i spaces
let internal decrIndent i (ctx : Context) = 
    ctx.Writer.Indent <- max 0 (ctx.Writer.Indent - i)
    ctx

/// Apply function f at an absolute indent level (use with care)
let internal atIndentLevel level (f : Context -> Context) ctx =
    if level < 0 then
        invalidArg "level" "The indent level cannot be negative."
    let oldLevel = ctx.Writer.Indent
    ctx.Writer.Indent <- level
    let result = f ctx
    ctx.Writer.Indent <- oldLevel
    result

/// Write everything at current column indentation
let internal atCurrentColumn (f : _ -> Context) (ctx : Context) =
    atIndentLevel ctx.Writer.Column f ctx

/// Function composition operator
let internal (+>) (ctx : Context -> Context) (f : _ -> Context) x =
    f (ctx x)

/// Break-line and append specified string
let internal (++) (ctx : Context -> Context) (str : string) x =
    let c = ctx x
    c.Writer.WriteLine("")
    c.Writer.Write(str)
    c

/// Break-line if config says so
let internal (+-) (ctx : Context -> Context) (str : string) x =
    let c = ctx x
    if c.BreakOn str then 
        c.Writer.WriteLine("")
    else
        c.Writer.Write(" ")
    c.Writer.Write(str)
    c

/// Append specified string without line-break
let internal (--) (ctx : Context -> Context) (str : string) x =
    let c = ctx x
    c.Writer.Write(str)
    c

let internal (!-) (str : string) = id -- str 
let internal (!+) (str : string) = id ++ str 

/// Print object converted to string
let internal str (o : 'T) (ctx : Context) =
    ctx.Writer.Write(o.ToString())
    ctx

/// Similar to col, and supply index as well
let internal coli f' (c : seq<'T>) f (ctx : Context) =
    let mutable tryPick = true
    let mutable st = ctx
    let mutable i = 0
    let e = c.GetEnumerator()   
    while (e.MoveNext()) do
        if tryPick then tryPick <- false else st <- f' st
        st <- f i (e.Current) st
        i  <- i + 1
    st

/// Process collection - keeps context through the whole processing
/// calls f for every element in sequence and f' between every two elements 
/// as a separator. This is a variant that works on typed collections.
let internal col f' (c : seq<'T>) f (ctx : Context) =
    let mutable tryPick = true
    let mutable st = ctx
    let e = c.GetEnumerator()   
    while (e.MoveNext()) do
        if tryPick then tryPick <- false else st <- f' st
        st <- f (e.Current) st
    st

/// Similar to col, apply one more function f2 at the end if the input sequence is not empty
let internal colPost f2 f1 (c : seq<'T>) f (ctx : Context) =
    if Seq.isEmpty c then ctx
    else f2 (col f1 c f ctx)

/// Similar to col, apply one more function f2 at the beginning if the input sequence is not empty
let internal colPre f2 f1 (c : seq<'T>) f (ctx : Context) =
    if Seq.isEmpty c then ctx
    else col f1 c f (f2 ctx)

/// If there is a value, apply f and f' accordingly, otherwise do nothing
let internal opt (f' : Context -> _) o f (ctx : Context) =
    match o with
    | Some x -> f' (f x ctx)
    | None -> ctx

/// Similar to opt, but apply f2 at the beginning if there is a value
let internal optPre (f2 : _ -> Context) (f1 : Context -> _) o f (ctx : Context) =
    match o with
    | Some x -> f1 (f x (f2 ctx))
    | None -> ctx

/// b is true, apply f1 otherwise apply f2
let internal ifElse b (f1 : Context -> Context) f2 (ctx : Context) =
    if b then f1 ctx else f2 ctx

/// Repeat application of a function n times
let internal rep n (f : Context -> Context) (ctx : Context) =
    [1..n] |> List.fold (fun c _ -> f c) ctx

let internal wordAnd = !- " and "
let internal wordOr = !- " or "
let internal wordOf = !- " of "   

// Separator functions
        
let internal sepDot = !- "."
let internal sepSpace = !- " "      
let internal sepNln = !+ ""
let internal sepStar = !- " * "
let internal sepEq = !- " = "
let internal sepArrow = !- " -> "
let internal sepWild = !- "_"
let internal sepNone = id
let internal sepBar = !- "| "

/// opening token of list
let internal sepOpenL (ctx : Context) =  
    if ctx.Config.SpaceAroundDelimiter then str "[ " ctx else str "[" ctx 

/// closing token of list
let internal sepCloseL (ctx : Context) =
    if ctx.Config.SpaceAroundDelimiter then str " ]" ctx else str "]" ctx 

/// opening token of list
let internal sepOpenLFixed = !- "["

/// closing token of list
let internal sepCloseLFixed = !- "]"

/// opening token of array
let internal sepOpenA (ctx : Context) =
    if ctx.Config.SpaceAroundDelimiter then str "[| " ctx else str "[|" ctx 

/// closing token of array
let internal sepCloseA (ctx : Context) = 
    if ctx.Config.SpaceAroundDelimiter then str " |]" ctx else str "|]" ctx 

/// opening token of list
let internal sepOpenAFixed = !- "[|"
/// closing token of list
let internal sepCloseAFixed = !- "|]"

/// opening token of sequence
let internal sepOpenS (ctx : Context) = 
    if ctx.Config.SpaceAroundDelimiter then str "{ " ctx else str "{" ctx 

/// closing token of sequence
let internal sepCloseS (ctx : Context) = 
    if ctx.Config.SpaceAroundDelimiter then str " }" ctx else str "}" ctx

/// opening token of sequence
let internal sepOpenSFixed = !- "{"

/// closing token of sequence
let internal sepCloseSFixed = !- "}"

/// opening token of tuple
let internal sepOpenT = !- "("

/// closing token of tuple
let internal sepCloseT = !- ")"

/// Set a checkpoint to break at an appropriate column
let internal autoNln f (ctx : Context) =
    if ctx.BreakLines then 
        let width = ctx.Config.PageWidth
        // Create a dummy context to evaluate length of current operation
        use colWriter = new ColumnIndentedTextWriter(new StringWriter())
        let dummyCtx = ctx.With(colWriter)
        let col = (f dummyCtx).Writer.Column
        // This isn't accurate if we go to new lines
        if col > width then 
            f (sepNln ctx) 
        else 
            f ctx
    else
        f ctx

/// Similar to col, skip auto newline for index 0
let internal colAutoNlnSkip0i f' (c : seq<'T>) f (ctx : Context) = 
    coli f' c (fun i c -> if i = 0 then f i c else autoNln (f i c)) ctx

/// Similar to col, skip auto newline for index 0
let internal colAutoNlnSkip0 f' c f = colAutoNlnSkip0i f' c (fun _ -> f)

/// Skip all auto-breaking newlines
let internal noNln f (ctx : Context) : Context = 
    ctx.BreakLines <- false
    let res = f ctx
    ctx.BreakLines <- true
    res

let internal sepColon (ctx : Context) = 
    if ctx.Config.SpaceBeforeColon then str " : " ctx else str ": " ctx

let internal sepColonFixed = !- ":"

let internal sepComma (ctx : Context) = 
    if ctx.Config.SpaceAfterComma then str ", " ctx else str "," ctx

let internal sepSemi (ctx : Context) = 
    if ctx.Config.SpaceAfterSemicolon then str "; " ctx else str ";" ctx

let internal sepSemiNln (ctx : Context) =
    // sepNln part is essential to indentation
    if ctx.Config.SemicolonAtEndOfLine then (!- ";" +> sepNln) ctx else sepNln ctx

let internal sepBeforeArg (ctx : Context) = 
    if ctx.Config.SpaceBeforeArgument then str " " ctx else str "" ctx

/// Conditional indentation on with keyword
let internal indentOnWith (ctx : Context) =
    if ctx.Config.IndentOnTryWith then indent ctx else ctx

/// Conditional unindentation on with keyword
let internal unindentOnWith (ctx : Context) =
    if ctx.Config.IndentOnTryWith then unindent ctx else ctx

let internal sortAndDeduplicate by l (ctx : Context) =
    if ctx.Config.ReorderOpenDeclaration then
        l |> Seq.distinctBy by |> Seq.sortBy by |> List.ofSeq
    else l

/// Don't put space before and after these operators
let internal NoSpaceInfixOps = set [".."; "?"]

/// Always break into newlines on these operators
let internal NewLineInfixOps = set ["|>"; "||>"; "|||>"; ">>"; ">>="]

/// Never break into newlines on these operators
let internal NoBreakInfixOps = set ["="; ">"; "<";]

