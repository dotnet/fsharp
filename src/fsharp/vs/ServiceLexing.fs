// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for lexing.
//--------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open System
open System.IO
open System.Collections.Generic
 
open Microsoft.FSharp.Compiler.AbstractIL.Internal  
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Internal.Utilities.Debug
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Lexhelp
open Microsoft.FSharp.Compiler.Lib

type Position = int * int
type Range = Position * Position
           
/// This corresponds to a token categorization originally used in Visual Studio 2003.
/// 
/// NOTE: This corresponds to a token categorization originally used in Visual Studio 2003 and the original Babel source code.
/// It is not clear it is a primary logical classification that should be being used in the 
/// more recent language service work.
type TokenColorKind =
      Default = 0
    | Text = 0
    | Keyword = 1
    | Comment = 2
    | Identifier = 3
    | String = 4
    | UpperIdentifier = 5
    | InactiveCode = 7
    | PreprocessorKeyword = 8
    | Number = 9
    | Operator = 10
#if COLORIZE_TYPES
    | TypeName = 11
#endif

/// Categorize an action the editor should take in respons to a token, e.g. brace matching
/// 
/// NOTE: This corresponds to a token categorization originally used in Visual Studio 2003 and the original Babel source code.
/// It is not clear it is a primary logical classification that should be being used in the 
/// more recent language service work.
type TriggerClass =
      None         = 0x00000000
    | MemberSelect = 0x00000001
    | MatchBraces  = 0x00000002 
    | ChoiceSelect = 0x00000004
    | MethodTip    = 0x000000F0
    | ParamStart   = 0x00000010
    | ParamNext    = 0x00000020
    | ParamEnd     = 0x00000040
    
    
/// This corresponds to a token categorization originally used in Visual Studio 2003.
/// 
/// NOTE: This corresponds to a token categorization originally used in Visual Studio 2003 and the original Babel source code.
/// It is not clear it is a primary logical classification that should be being used in the 
/// more recent language service work.
type TokenCharKind = 
      Default     = 0x00000000
    | Text        = 0x00000000
    | Keyword     = 0x00000001
    | Identifier  = 0x00000002
    | String      = 0x00000003
    | Literal     = 0x00000004
    | Operator    = 0x00000005
    | Delimiter   = 0x00000006
    | WhiteSpace  = 0x00000008
    | LineComment = 0x00000009
    | Comment     = 0x0000000A    


/// Information about a particular token from the tokenizer
type TokenInformation = {
    LeftColumn:int;
    RightColumn:int;
    ColorClass:TokenColorKind;
    CharClass:TokenCharKind;
    TriggerClass:TriggerClass;
    Tag:int
    TokenName:string }

//----------------------------------------------------------------------------
// Flags
//--------------------------------------------------------------------------

module internal Flags = 
#if SILVERLIGHT
    let init ()= ()
#else
#if DEBUG
    let loggingTypes             = System.Environment.GetEnvironmentVariable("mFSharp_Logging")
    let logging                  = not (String.IsNullOrEmpty(loggingTypes))
    let initialLoggingGUITypes   = loggingTypes
    let loggingGUI               = not (String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("mFSharp_LogToWinForm")))
    let loggingStdOut            = not (String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("mFSharp_LogToStdOut")))
#else
    let loggingTypes             = ""
    let logging                  = false
    let initialLoggingGUITypes   = ""
    let loggingGUI               = false
    let loggingStdOut            = false
#endif
    let doInit = 
        if logging && not loggingGUI  && not loggingStdOut then  
            let logFile = ("c:\\fsharp\\log-m"+System.IO.Path.GetFileName(System.AppDomain.CurrentDomain.FriendlyName)+".log") 
            let traceFile = ("c:\\fsharp\\trace-m"+System.IO.Path.GetFileName(System.AppDomain.CurrentDomain.FriendlyName)+".txt") 
            try
                let log = (File.CreateText logFile  :> TextWriter)
                setDiagnosticsChannel(Some(log));
                progress := true;
            with e->
                // Don't kill the language service just because we couldn't log.
                System.Diagnostics.Debug.Assert(false, e.ToString())                
                ()
            if logging then 
                dprintf "Opened log file %s for ML, config follows\n" logFile
                dprintf "logging types = %s\n" loggingTypes
            Trace.Log <- loggingTypes
            Trace.Out <- 
                try 
                    new StreamWriter(traceFile,append=false,encoding=System.Text.Encoding.UTF8) :> TextWriter
                with e -> 
                    // Don't kill the language service just because we couldn't log.
                    System.Diagnostics.Debug.Assert(false, e.ToString())                
                    System.Console.Out 
                    
        elif loggingStdOut then 
            Trace.Log <- initialLoggingGUITypes
            Trace.Out <- System.Console.Out
        elif loggingGUI then 
            let f = new System.Windows.Forms.Form(Visible=true,TopMost=true,Width=600,Height=600)
            let memoryText = new System.Windows.Forms.TextBox(Text = "?? Kb", Width = 200)
            let memoryButton = new System.Windows.Forms.Button(Text = "GC and update Mem", Left = 200)
            memoryButton.Click.AddHandler(fun _ _ -> 
                            GC.Collect()
                            GC.WaitForPendingFinalizers()
                            memoryText.Text <- sprintf "%d Kb" (GC.GetTotalMemory(false) / 1024L)
                        )
            f.Controls.Add(memoryText)
            f.Controls.Add(memoryButton)            
            let rb = new System.Windows.Forms.RichTextBox(Dock=System.Windows.Forms.DockStyle.Fill, Font=new System.Drawing.Font("courier new",8.0f), Top = memoryButton.Height)
            f.Controls.Add(rb)
            rb.DoubleClick.Add(fun _ -> rb.Clear())
            let lab = new System.Windows.Forms.Label(Dock=System.Windows.Forms.DockStyle.Top, Font=new System.Drawing.Font("courier new",8.0f))
            f.Controls.Add(lab)
            let tb = new System.Windows.Forms.TextBox(Text=initialLoggingGUITypes,Height=10,Multiline=false,Dock=System.Windows.Forms.DockStyle.Top, Font=new System.Drawing.Font("courier new",8.0f))
            f.Controls.Add(tb)
            tb.TextChanged.Add (fun _ -> Trace.Log <- tb.Text) 
            
            let log = 
                let addTextOnGuiThread text = 
                    if not rb.IsDisposed then 
                        rb.AppendText(text); 
                        if text.Contains "\n" then 
                            rb.ScrollToCaret();
                            if rb.TextLength > 200000 then 
                               let s = rb.Text
                               rb.Text <- s.[s.Length - 100000..s.Length-1]
                let addText text = 
                    if f.InvokeRequired then 
                        f.BeginInvoke(new System.Windows.Forms.MethodInvoker(fun () -> addTextOnGuiThread text)) |> ignore
                    else
                        addTextOnGuiThread text
                    
                { new System.IO.TextWriter() with 
                      member x.Write(c:char) = addText (string c)
                      member x.Write(s:string) =  addText  s
                      member x.Encoding = System.Text.Encoding.Unicode } 
            setDiagnosticsChannel(Some(log));
            Trace.Log <- if initialLoggingGUITypes <> null then initialLoggingGUITypes else ""
            Trace.Out <- log
        else 
            // Would be nice to leave this at whatever channel was originally assigned.
            // This currently defeats NUnit's ability to capture logging output.
            setDiagnosticsChannel(None) (* VS does not support stderr! *)

    //let stripFSharpCoreReferences   = not (String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("mFSharp_StripFSharpCoreReferences")))
    let init() = doInit
#endif
        
open Flags



//----------------------------------------------------------------------------
// Babel flags
//--------------------------------------------------------------------------

module internal TokenClassifications = 

    //----------------------------------------------------------------------------
    //From tokens to flags  
    //--------------------------------------------------------------------------

    let tokenInfo token = 
        match token with 
        | IDENT s
          -> 
            if s.Length <= 0 then 
                System.Diagnostics.Debug.Assert(false, "BUG:Received zero length IDENT token.")
                // This is related to 4783. Recover by treating as lower case identifier.
                (TokenColorKind.Identifier,TokenCharKind.Identifier,TriggerClass.None)  
            else 
                if System.Char.ToUpperInvariant s.[0] = s.[0] then
                    (TokenColorKind.UpperIdentifier,TokenCharKind.Identifier,TriggerClass.None)
                else
                    (TokenColorKind.Identifier,TokenCharKind.Identifier,TriggerClass.None)  

        // 'in' when used in a 'join' in a query expression
        | JOIN_IN ->
                    (TokenColorKind.Identifier,TokenCharKind.Identifier,TriggerClass.None)  
        | DECIMAL _
        | BIGNUM _ | INT8 _  | UINT8 _ | INT16 _  | UINT16 _ | INT32 _ | UINT32 _ | INT64 _ | UINT64 _ 
        | UNATIVEINT _ | NATIVEINT _ | IEEE32 _ |  IEEE64 _
          -> (TokenColorKind.Number,TokenCharKind.Literal,TriggerClass.None)

        | INT32_DOT_DOT _ 
          // This will color the whole "1.." expression in a 'number' color 
          // (this isn't entirely correct, but it'll work for now - see bug 3727)
          -> (TokenColorKind.Number,TokenCharKind.Operator,TriggerClass.None)
        
        | INFIX_STAR_DIV_MOD_OP ("mod"  | "land" |  "lor" | "lxor")
        | INFIX_STAR_STAR_OP ("lsl" | "lsr" | "asr")
          -> (TokenColorKind.Keyword,TokenCharKind.Keyword,TriggerClass.None)

        | LPAREN_STAR_RPAREN
        | DOLLAR | COLON_GREATER  | COLON_COLON  
        | PERCENT_OP _ | PLUS_MINUS_OP _ | PREFIX_OP _ | COLON_QMARK_GREATER   
        | AMP   | AMP_AMP  | BAR_BAR  | QMARK | QMARK_QMARK | COLON_QMARK
        | QUOTE   | STAR  | HIGH_PRECEDENCE_TYAPP 
        | COLON    | COLON_EQUALS   | LARROW | EQUALS | RQUOTE_DOT _
        | MINUS | ADJACENT_PREFIX_OP _  
          -> (TokenColorKind.Operator,TokenCharKind.Operator,TriggerClass.None)

        | INFIX_COMPARE_OP _ // This is a whole family: .< .> .= .!= .$
        | FUNKY_OPERATOR_NAME _ // This is another whole family, including: .[] and .()
        | INFIX_AT_HAT_OP _
        | INFIX_STAR_STAR_OP _
        | INFIX_AMP_OP _
        | INFIX_BAR_OP _
        | INFIX_STAR_DIV_MOD_OP _
        | INFIX_AMP_OP _ ->
                (TokenColorKind.Operator,TokenCharKind.Operator,TriggerClass.None)

        | DOT_DOT
          -> 
            (TokenColorKind.Operator,TokenCharKind.Operator,TriggerClass.MemberSelect)

        | COMMA
          -> (TokenColorKind.Text,TokenCharKind.Delimiter,TriggerClass.ParamNext)
              
        | DOT 
          -> (TokenColorKind.Operator,TokenCharKind.Delimiter,TriggerClass.MemberSelect)
              
        | BAR
          -> (TokenColorKind.Text,TokenCharKind.Delimiter,TriggerClass.None (* TriggerClass.ChoiceSelect *))
              
        | HASH | UNDERSCORE   
        | SEMICOLON    | SEMICOLON_SEMICOLON
          -> (TokenColorKind.Text,TokenCharKind.Delimiter,TriggerClass.None)

        | LESS  _
          -> (TokenColorKind.Operator,TokenCharKind.Operator,TriggerClass.ParamStart)  // for type provider static arguments
        | GREATER _ 
          -> (TokenColorKind.Operator,TokenCharKind.Operator,TriggerClass.ParamEnd)    // for type provider static arguments
              
        | LPAREN
          // We need 'ParamStart' to trigger the 'GetDeclarations' method to show param info automatically
          // this is needed even if we don't use MPF for determining information about params
          -> (TokenColorKind.Text,TokenCharKind.Delimiter, TriggerClass.ParamStart ||| TriggerClass.MatchBraces)
              
        | RPAREN | RPAREN_COMING_SOON | RPAREN_IS_HERE
          -> (TokenColorKind.Text,TokenCharKind.Delimiter, TriggerClass.ParamEnd ||| TriggerClass.MatchBraces)
              
        | LBRACK_LESS  | LBRACE_LESS
          -> (TokenColorKind.Text,TokenCharKind.Delimiter,TriggerClass.None )
          
        | LQUOTE _  | LBRACK  | LBRACE | LBRACK_BAR 
          -> (TokenColorKind.Text,TokenCharKind.Delimiter,TriggerClass.MatchBraces )
          
        | GREATER_RBRACE   | GREATER_RBRACK  | GREATER_BAR_RBRACK
          -> (TokenColorKind.Text,TokenCharKind.Delimiter,TriggerClass.None )

        | RQUOTE _  | RBRACK  | RBRACE | RBRACE_COMING_SOON | RBRACE_IS_HERE | BAR_RBRACK   
          -> (TokenColorKind.Text,TokenCharKind.Delimiter,TriggerClass.MatchBraces )
              
        | PUBLIC | PRIVATE | INTERNAL | BASE | GLOBAL
        | CONSTRAINT | INSTANCE | DELEGATE | INHERIT|CONSTRUCTOR|DEFAULT|OVERRIDE|ABSTRACT|CLASS
        | MEMBER | STATIC | NAMESPACE
        | OASSERT | OLAZY | ODECLEND | OBLOCKSEP | OEND | OBLOCKBEGIN | ORIGHT_BLOCK_END | OBLOCKEND | OBLOCKEND_COMING_SOON | OBLOCKEND_IS_HERE | OTHEN | OELSE | OLET(_) | OBINDER _ | BINDER _ | ODO | OWITH | OFUNCTION | OFUN | ORESET | ODUMMY _ | DO_BANG | ODO_BANG | YIELD _ | YIELD_BANG  _ | OINTERFACE_MEMBER
        | ELIF | RARROW | SIG | STRUCT 
        | UPCAST   | DOWNCAST   | NULL   | RESERVED    | MODULE    | AND    | AS   | ASSERT   | ASR
        | DOWNTO   | EXCEPTION   | FALSE   | FOR   | FUN   | FUNCTION
        | FINALLY   | LAZY   | MATCH  | MUTABLE   | NEW   | OF    | OPEN   | OR | VOID | EXTERN
        | INTERFACE | REC   | TO   | TRUE   | TRY   | TYPE   |  VAL   | INLINE   | WHEN  | WHILE   | WITH
        | IF | THEN  | ELSE | DO | DONE | LET(_) | IN (*| NAMESPACE*) | CONST
        | HIGH_PRECEDENCE_PAREN_APP
        | HIGH_PRECEDENCE_BRACK_APP
        | TYPE_COMING_SOON | TYPE_IS_HERE | MODULE_COMING_SOON | MODULE_IS_HERE
          -> (TokenColorKind.Keyword,TokenCharKind.Keyword,TriggerClass.None)
              
        | BEGIN  
          -> (TokenColorKind.Keyword,TokenCharKind.Keyword,TriggerClass.None)

        | END 
          -> (TokenColorKind.Keyword,TokenCharKind.Keyword,TriggerClass.None)
        | HASH_LIGHT _
        | HASH_LINE _
        | HASH_IF _
        | HASH_ELSE _
        | HASH_ENDIF _ -> 
            (TokenColorKind.PreprocessorKeyword,TokenCharKind.WhiteSpace,TriggerClass.None)
        | INACTIVECODE _ -> 
            (TokenColorKind.InactiveCode,TokenCharKind.WhiteSpace,TriggerClass.None)
          

        | LEX_FAILURE _
        | WHITESPACE _ -> 
            (TokenColorKind.Default,TokenCharKind.WhiteSpace,TriggerClass.None)

        | COMMENT _ -> 
            (TokenColorKind.Comment,TokenCharKind.Comment,TriggerClass.None)
        | LINE_COMMENT _ -> 
            (TokenColorKind.Comment,TokenCharKind.LineComment,TriggerClass.None)
        | STRING_TEXT _ -> 
            (TokenColorKind.String,TokenCharKind.String,TriggerClass.None)
        | KEYWORD_STRING _ -> 
           (TokenColorKind.Keyword,TokenCharKind.Keyword,TriggerClass.None)
        | BYTEARRAY _ | STRING  _
        | CHAR _ (* bug://2863 asks to color 'char' as "string" *)
          -> (TokenColorKind.String,TokenCharKind.String,TriggerClass.None)
        | EOF _ -> failwith "tokenInfo"

module internal TestExpose = 
  let TokenInfo tok = TokenClassifications.tokenInfo tok
  
    //----------------------------------------------------------------------------
    // Lexer states encoded to/from integers
    //--------------------------------------------------------------------------
type LexState = int64

type ColorState =
    | Token = 1
    | IfDefSkip = 3
    | String = 4
    | Comment = 5
    | StringInComment = 6
    | VerbatimStringInComment = 7
    | CamlOnly = 8
    | VerbatimString = 9
    | SingleLineComment = 10
    | EndLineThenSkip = 11
    | EndLineThenToken = 12
    | TripleQuoteString = 13
    | TripleQuoteStringInComment = 14
    
    | InitialState = 0 
    

module internal LexerStateEncoding = 

    let computeNextLexState token (prevLexcont:LexerWhitespaceContinuation) = 
      match token with 
      | HASH_LINE s
      | HASH_LIGHT s
      | HASH_IF(_, _, s)
      | HASH_ELSE(_, _, s)
      | HASH_ENDIF(_, _, s)
      | INACTIVECODE s
      | WHITESPACE s  
      | COMMENT s 
      | LINE_COMMENT s 
      | STRING_TEXT s 
      | EOF s -> s
      | BYTEARRAY _ | STRING _ -> LexCont.Token(prevLexcont.LexerIfdefStack)
      | _ -> prevLexcont

    // Note that this will discard all lexcont state, including the ifdefStack.
    let revertToDefaultLexCont = LexCont.Token []

    let resize32 (i:int32) : LexState = int64 i

    let lexstateNumBits = 4
    let ncommentsNumBits = 2
    let startPosNumBits = pos.EncodingSize
    let hardwhiteNumBits = 1
    let ifdefstackCountNumBits = 4
    let ifdefstackNumBits = 16           // 0 means if, 1 means else
    let _ = assert (lexstateNumBits 
                    + ncommentsNumBits 
                    + startPosNumBits 
                    + hardwhiteNumBits 
                    + ifdefstackCountNumBits 
                    + ifdefstackNumBits <= 64)

    let lexstateStart         = 0
    let ncommentsStart        = lexstateNumBits
    let startPosStart         = lexstateNumBits+ncommentsNumBits
    let hardwhitePosStart     = lexstateNumBits+ncommentsNumBits+startPosNumBits
    let ifdefstackCountStart  = lexstateNumBits+ncommentsNumBits+startPosNumBits+hardwhiteNumBits
    let ifdefstackStart       = lexstateNumBits+ncommentsNumBits+startPosNumBits+hardwhiteNumBits+ifdefstackCountNumBits
    
    let lexstateMask          = Bits.mask64 lexstateStart lexstateNumBits
    let ncommentsMask         = Bits.mask64 ncommentsStart ncommentsNumBits
    let startPosMask          = Bits.mask64 startPosStart startPosNumBits
    let hardwhitePosMask      = Bits.mask64 hardwhitePosStart hardwhiteNumBits
    let ifdefstackCountMask   = Bits.mask64 ifdefstackCountStart ifdefstackCountNumBits
    let ifdefstackMask        = Bits.mask64 ifdefstackStart ifdefstackNumBits

    let bitOfBool b = if b then 1 else 0
    let boolOfBit n = (n = 1L)
        
    let encodeLexCont (colorState:ColorState) ncomments (b:pos) ifdefStack light = 
        let mutable ifdefStackCount = 0
        let mutable ifdefStackBits = 0
        for ifOrElse in ifdefStack do
            match ifOrElse with 
                | (IfDefIf,_) -> ()
                | (IfDefElse,_) -> 
                    ifdefStackBits <- (ifdefStackBits ||| (1 <<< ifdefStackCount))
            ifdefStackCount <- ifdefStackCount + 1

        let lexstate = int64 colorState
        ((lexstate <<< lexstateStart)  &&& lexstateMask)
        ||| ((ncomments <<< ncommentsStart) &&& ncommentsMask)
        ||| ((resize32 b.Encoding <<< startPosStart) &&& startPosMask)
        ||| ((resize32 (bitOfBool light) <<< hardwhitePosStart) &&& hardwhitePosMask)
        ||| ((resize32 ifdefStackCount <<< ifdefstackCountStart) &&& ifdefstackCountMask)
        ||| ((resize32 ifdefStackBits <<< ifdefstackStart) &&& ifdefstackMask)
    
    let decodeLexCont (state:LexState) = 
        let mutable ifDefs = []
        let ifdefStackCount = (int32) ((state &&& ifdefstackCountMask) >>> ifdefstackCountStart)
        if ifdefStackCount>0 then 
            let ifdefStack = (int32) ((state &&& ifdefstackMask) >>> ifdefstackStart)
            for i in 1..ifdefStackCount do
                let bit = ifdefStackCount-i
                let mask = 1 <<< bit
                let ifDef = (if ifdefStack &&& mask = 0 then IfDefIf else IfDefElse)
                ifDefs<-(ifDef,range0)::ifDefs
        enum<ColorState> (int32 ((state &&& lexstateMask)  >>> lexstateStart)),
        (int32) ((state &&& ncommentsMask) >>> ncommentsStart),
        pos.Decode (int32 ((state &&& startPosMask) >>> startPosStart)),
        ifDefs,
        boolOfBit ((state &&& hardwhitePosMask) >>> hardwhitePosStart)

    let encodeLexInt lightSyntaxStatus (lexcont:LexerWhitespaceContinuation) = 
        let tag,n1,p1,ifd = 
            match lexcont with 
            | LexCont.Token ifd                                       -> ColorState.Token,                     0L,         pos0,    ifd
            | LexCont.IfDefSkip (ifd,n,m)                             -> ColorState.IfDefSkip,                 resize32 n, m.Start, ifd
            | LexCont.EndLine(LexerEndlineContinuation.Skip(ifd,n,m)) -> ColorState.EndLineThenSkip,           resize32 n, m.Start, ifd
            | LexCont.EndLine(LexerEndlineContinuation.Token(ifd))    -> ColorState.EndLineThenToken,          0L,         pos0,    ifd
            | LexCont.String (ifd,m)                                  -> ColorState.String,                    0L,         m.Start, ifd
            | LexCont.Comment (ifd,n,m)                               -> ColorState.Comment,                   resize32 n, m.Start, ifd
            | LexCont.SingleLineComment (ifd,n,m)                     -> ColorState.SingleLineComment,         resize32 n, m.Start, ifd
            | LexCont.StringInComment (ifd,n,m)                       -> ColorState.StringInComment,           resize32 n, m.Start, ifd
            | LexCont.VerbatimStringInComment (ifd,n,m)               -> ColorState.VerbatimStringInComment,   resize32 n, m.Start, ifd
            | LexCont.TripleQuoteStringInComment (ifd,n,m)            -> ColorState.TripleQuoteStringInComment,resize32 n, m.Start, ifd
            | LexCont.MLOnly (ifd,m)                                  -> ColorState.CamlOnly,                  0L,         m.Start, ifd
            | LexCont.VerbatimString (ifd,m)                          -> ColorState.VerbatimString,            0L,         m.Start, ifd
            | LexCont.TripleQuoteString (ifd,m)                       -> ColorState.TripleQuoteString,         0L,         m.Start, ifd
        encodeLexCont tag n1 p1 ifd lightSyntaxStatus
        

    let decodeLexInt (state:LexState) = 
        let tag,n1,p1,ifd,lightSyntaxStatusInital = decodeLexCont state 
        let lexcont = 
            match tag with 
            |  ColorState.Token                      -> LexCont.Token ifd
            |  ColorState.IfDefSkip                  -> LexCont.IfDefSkip (ifd,n1,mkRange "file" p1 p1)
            |  ColorState.String                     -> LexCont.String (ifd,mkRange "file" p1 p1)
            |  ColorState.Comment                    -> LexCont.Comment (ifd,n1,mkRange "file" p1 p1)
            |  ColorState.SingleLineComment          -> LexCont.SingleLineComment (ifd,n1,mkRange "file" p1 p1)
            |  ColorState.StringInComment            -> LexCont.StringInComment (ifd,n1,mkRange "file" p1 p1)
            |  ColorState.VerbatimStringInComment    -> LexCont.VerbatimStringInComment (ifd,n1,mkRange "file" p1 p1)
            |  ColorState.TripleQuoteStringInComment -> LexCont.TripleQuoteStringInComment (ifd,n1,mkRange "file" p1 p1)
            |  ColorState.CamlOnly                   -> LexCont.MLOnly (ifd,mkRange "file" p1 p1)
            |  ColorState.VerbatimString             -> LexCont.VerbatimString (ifd,mkRange "file" p1 p1)
            |  ColorState.TripleQuoteString          -> LexCont.TripleQuoteString (ifd,mkRange "file" p1 p1)
            |  ColorState.EndLineThenSkip            -> LexCont.EndLine(LexerEndlineContinuation.Skip(ifd,n1,mkRange "file" p1 p1))
            |  ColorState.EndLineThenToken           -> LexCont.EndLine(LexerEndlineContinuation.Token(ifd))
            | _ -> LexCont.Token [] 
        lightSyntaxStatusInital,lexcont

    let callLexCont lexcont args skip lexbuf = 
        let argsWithIfDefs ifd = 
            if !args.ifdefStack = ifd then 
                args
            else 
                {args with ifdefStack = ref ifd}
        match lexcont with 
        | LexCont.EndLine cont                         -> Lexer.endline cont args skip lexbuf
        | LexCont.Token ifd                            -> Lexer.token (argsWithIfDefs ifd) skip lexbuf 
        | LexCont.IfDefSkip (ifd,n,m)                  -> Lexer.ifdefSkip n m (argsWithIfDefs ifd) skip lexbuf 
        // Q: What's this magic 100 number for? Q: it's just an initial buffer size. 
        | LexCont.String (ifd,m)                       -> Lexer.string (ByteBuffer.Create 100,defaultStringFinisher,m,(argsWithIfDefs ifd)) skip lexbuf
        | LexCont.Comment (ifd,n,m)                    -> Lexer.comment (n,m,(argsWithIfDefs ifd)) skip lexbuf
        // The first argument is 'None' because we don't need XML comments when called from VS
        | LexCont.SingleLineComment (ifd,n,m)          -> Lexer.singleLineComment (None,n,m,(argsWithIfDefs ifd)) skip lexbuf
        | LexCont.StringInComment (ifd,n,m)            -> Lexer.stringInComment n m (argsWithIfDefs ifd) skip lexbuf
        | LexCont.VerbatimStringInComment (ifd,n,m)    -> Lexer.verbatimStringInComment n m (argsWithIfDefs ifd) skip lexbuf
        | LexCont.TripleQuoteStringInComment (ifd,n,m) -> Lexer.tripleQuoteStringInComment n m (argsWithIfDefs ifd) skip lexbuf
        | LexCont.MLOnly (ifd,m)                       -> Lexer.mlOnly m (argsWithIfDefs ifd) skip lexbuf
        | LexCont.VerbatimString (ifd,m)               -> Lexer.verbatimString (ByteBuffer.Create 100,defaultStringFinisher,m,(argsWithIfDefs ifd)) skip lexbuf
        | LexCont.TripleQuoteString (ifd,m)            -> Lexer.tripleQuoteString (ByteBuffer.Create 100,defaultStringFinisher,m,(argsWithIfDefs ifd)) skip lexbuf

//----------------------------------------------------------------------------
// Colorization
//----------------------------------------------------------------------------

// Information beyond just tokens that can be derived by looking at just a single line.
// For example metacommands like #load.
type SingleLineTokenState =
    | BeforeHash = 0
    | NoFurtherMatchPossible = 1
    

/// Split a line into tokens and attach information about the tokens. This information is used by Visual Studio.
[<Sealed>]
type internal LineTokenizer(text:string, 
                            filename : string, 
                            lexArgsLightOn : lexargs,
                            lexArgsLightOff : lexargs
                            ) = 

    let skip = false   // don't skip whitespace in the lexer 
    let lexbuf = UnicodeLexing.StringAsLexbuf text
    
    let mutable singleLineTokenState = SingleLineTokenState.BeforeHash
    let fsx = Build.IsScript(filename)

    // ----------------------------------------------------------------------------------
    // This implements post-processing of #directive tokens - not very elegant, but it works...
    // We get the whole "   #if IDENT // .. .. " thing as a single token from the lexer,
    // so we need to split it into tokens that are used by VS for colorization
    
    // Stack for tokens that are split during postrpocessing    
    let mutable tokenStack = new Stack<_>()
    let delayToken tok = tokenStack.Push(tok)

    // Process: anywhite* #<directive>
    let processDirective (str:string) directiveLength delay cont =
        let hashIdx = str.IndexOf("#")
        if (hashIdx <> 0) then delay(WHITESPACE cont, 0, hashIdx - 1)
        delay(HASH_IF(range0, "", cont), hashIdx, hashIdx + directiveLength)
        hashIdx + directiveLength + 1
    
    // Process: anywhite* ("//" [^'\n''\r']*)?
    let processWhiteAndComment (str:string) offset delay cont = 
        let rest = str.Substring(offset, str.Length - offset)
        let comment = rest.IndexOf('/')
        let spaceLength = if comment = -1 then rest.Length else comment
        if (spaceLength > 0) then delay(WHITESPACE cont, offset, offset + spaceLength - 1)
        if (comment <> -1) then delay(COMMENT(cont), offset + comment, offset + rest.Length - 1) 
    
    // Split a directive line from lexer into tokens usable in VS
    let processDirectiveLine ofs f =
        let delayed = new ResizeArray<_>()
        f (fun (tok, s, e) -> delayed.Add (tok, s + ofs, e + ofs) )
        // delay all the tokens and return the remaining one
        for i = delayed.Count - 1 downto 1 do delayToken delayed.[i]
        delayed.[0]
      
    // Split the following line:
    //  anywhite* ("#else"|"#endif") anywhite* ("//" [^'\n''\r']*)?
    let processHashEndElse ofs (str:string) length cont = 
        processDirectiveLine ofs (fun delay ->
            // Process: anywhite* "#else"   /   anywhite* "#endif"
            let offset = processDirective str length delay cont
            // Process: anywhite* ("//" [^'\n''\r']*)?
            processWhiteAndComment str offset delay cont )            
          
    // Split the following line:
    //  anywhite* "#if" anywhite+ ident anywhite* ("//" [^'\n''\r']*)?
    let processHashIfLine ofs (str:string) cont =
        let With n m = if (n < 0) then m else n
        processDirectiveLine ofs (fun delay ->
            // Process: anywhite* "#if"
            let offset = processDirective str 2 delay cont      
            // Process: anywhite+ ident
            let rest, spaces = 
                let w = str.Substring(offset) 
                let r = w.TrimStart [| ' '; '\t' |]
                r, w.Length - r.Length      
            let beforeIdent = offset + spaces      
            let identLength = With (rest.IndexOfAny([| '/'; '\t'; ' ' |])) rest.Length 
            delay(WHITESPACE cont, offset, beforeIdent - 1)            
            delay(IDENT(rest.Substring(0, identLength)), beforeIdent, beforeIdent + identLength - 1)           
            // Process: anywhite* ("//" [^'\n''\r']*)?
            let offset = beforeIdent + identLength
            processWhiteAndComment str offset delay cont )
      
    // ----------------------------------------------------------------------------------
          


    do resetLexbufPos filename lexbuf 
    
    member x.ScanToken(lexintInitial) : Option<TokenInformation> * LexState = 
        use unwindBP = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parse)
        use unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> DiscardErrorsLogger)

        let lightSyntaxStatusInital, lexcontInitial = LexerStateEncoding.decodeLexInt lexintInitial 
        let lightSyntaxStatus = LightSyntaxStatus(lightSyntaxStatusInital,false)  

        // Build the arguments to the lexer function
        let lexargs = if lightSyntaxStatusInital then lexArgsLightOn else lexArgsLightOff

        let GetTokenWithPosition(lexcontInitial) = 
            // Column of token
            let ColumnsOfCurrentToken() = 
                let leftp = lexbuf.StartPos 
                let rightp = lexbuf.EndPos 
                let leftc = leftp.Column 
                let rightc = if rightp.Line > leftp.Line then text.Length else rightp.Column 
                let rightc = rightc - 1   
                leftc,rightc

            // Get the token & position - either from a stack or from the lexer        
            try 
                if (tokenStack.Count > 0) then true, tokenStack.Pop()
                else                    
                  // Choose which lexer entrypoint to call and call it
                  let token = LexerStateEncoding.callLexCont lexcontInitial lexargs skip lexbuf 
                  let leftc, rightc = ColumnsOfCurrentToken()
                  
                  // Splits tokens like ">." into multiple tokens - this duplicates behavior from the 'lexfilter'
                  // which cannot be (easily) used from the langauge service. The rules here are not always valid,
                  // because sometimes token shouldn't be split. However it is just for colorization & 
                  // for VS (which needs to recognize when user types ".").
                  match token with
                  | HASH_IF(m, lineStr, cont) when lineStr <> "" ->
                      false, processHashIfLine m.StartColumn lineStr cont
                  | HASH_ELSE(m, lineStr, cont) when lineStr <> "" ->
                      false, processHashEndElse m.StartColumn lineStr 4 cont                  
                  | HASH_ENDIF(m, lineStr, cont) when lineStr <> "" ->
                      false, processHashEndElse m.StartColumn lineStr 5 cont
                  | RQUOTE_DOT (s,raw) -> 
                      delayToken(DOT, rightc, rightc)
                      false, (RQUOTE (s,raw), leftc, rightc - 1)
                  | INFIX_COMPARE_OP (Lexfilter.TyparsCloseOp(greaters,afterOp) as opstr) -> 
                      match afterOp with
                      | None -> ()
                      | Some tok -> delayToken(tok, leftc + greaters.Length, rightc)
                      for i = greaters.Length - 1 downto 1 do
                          delayToken(greaters.[i] false, leftc + i, rightc - opstr.Length + i + 1)
                      false, (greaters.[0] false, leftc, rightc - opstr.Length + 1)
                  // break up any operators that start with '.' so that we can get auto-popup-completion for e.g. "x.+1"  when typing the dot
                  | INFIX_STAR_STAR_OP opstr when opstr.StartsWith(".") ->
                      delayToken(INFIX_STAR_STAR_OP(opstr.Substring(1)), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | PLUS_MINUS_OP opstr when opstr.StartsWith(".") ->
                      delayToken(PLUS_MINUS_OP(opstr.Substring(1)), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | INFIX_COMPARE_OP opstr when opstr.StartsWith(".") ->
                      delayToken(INFIX_COMPARE_OP(opstr.Substring(1)), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | INFIX_AT_HAT_OP opstr when opstr.StartsWith(".") ->
                      delayToken(INFIX_AT_HAT_OP(opstr.Substring(1)), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | INFIX_BAR_OP opstr when opstr.StartsWith(".") ->
                      delayToken(INFIX_BAR_OP(opstr.Substring(1)), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | PREFIX_OP opstr when opstr.StartsWith(".") ->
                      delayToken(PREFIX_OP(opstr.Substring(1)), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | INFIX_STAR_DIV_MOD_OP opstr when opstr.StartsWith(".") ->
                      delayToken(INFIX_STAR_DIV_MOD_OP(opstr.Substring(1)), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | INFIX_AMP_OP opstr when opstr.StartsWith(".") ->
                      delayToken(INFIX_AMP_OP(opstr.Substring(1)), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | ADJACENT_PREFIX_OP opstr when opstr.StartsWith(".") ->
                      delayToken(ADJACENT_PREFIX_OP(opstr.Substring(1)), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | FUNKY_OPERATOR_NAME opstr when opstr.StartsWith(".") ->
                      delayToken(FUNKY_OPERATOR_NAME(opstr.Substring(1)), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | _ -> false, (token, leftc, rightc)
            with
            | e -> false, (EOF LexerStateEncoding.revertToDefaultLexCont, 0, 0) // REVIEW: report lex failure here        
        
        // Grab a token
        let isCached, (token, leftc, rightc) = GetTokenWithPosition(lexcontInitial)
                 
        // Check for end-of-string and failure
        let tokenDataOption, lexcontFinal, tokenTag = 
            match token with 
            | EOF lexcont -> 
                // End of text! No more tokens.
                None,lexcont,0 
            | LEX_FAILURE s -> 
                // REVIEW: report this error
                Trace.PrintLine("Lexing", fun _ -> sprintf "LEX_FAILURE:%s\n" s)
                None, LexerStateEncoding.revertToDefaultLexCont, 0
            | _ ->
                // Get the information about the token
                let (colorClass,charClass,triggerClass) = TokenClassifications.tokenInfo token 
                let lexcontFinal = 
                    // If we're using token from cache, we don't move forward with lexing
                    if isCached then lexcontInitial else LexerStateEncoding.computeNextLexState token lexcontInitial 
                let tokenTag = tagOfToken token 
                let tokenData = {TokenName = token_to_string token; LeftColumn=leftc; RightColumn=rightc;ColorClass=colorClass;CharClass=charClass;TriggerClass=triggerClass;Tag=tokenTag} 
                Some(tokenData), lexcontFinal, tokenTag
                
        // Get the final lex int and color state                
        let FinalState(lexcontFinal) = 
            LexerStateEncoding.encodeLexInt lightSyntaxStatus.Status lexcontFinal 
                
        // Check for patterns like #-IDENT and see if they look like meta commands for .fsx files. If they do then merge them into a single token.
        let tokenDataOption,lexintFinal = 
            let lexintFinal = FinalState(lexcontFinal)
            match tokenDataOption, singleLineTokenState, tokenTagToTokenId tokenTag with 
            | Some(tokenData), SingleLineTokenState.BeforeHash, TOKEN_HASH ->
                // Don't allow further matches.
                singleLineTokenState <- SingleLineTokenState.NoFurtherMatchPossible
                // Peek at the next token
                let isCached, (nextToken, _, rightc) = GetTokenWithPosition(lexcontInitial)
                match nextToken with 
                | IDENT possibleMetacommand -> 
                    match fsx,possibleMetacommand with
                    // These are for script (.fsx and .fsscript) files.
                    | true,"r" 
                    | true,"reference" 
                    | true,"I" 
                    | true,"load" 
                    | true,"time" 
                    | true,"cd" 
#if DEBUG
                    | true,"terms" 
                    | true,"types" 
                    | true,"savedll" 
                    | true,"nosavedll" 
#endif
                    | true,"silentCd" 
                    | true,"q" 
                    | true,"quit" 
                    | true,"help" 
                    // These are for script and non-script
                    | _,"nowarn" -> 
                        // Merge both tokens into one.
                        let lexcontFinal = if (isCached) then lexcontInitial else LexerStateEncoding.computeNextLexState token lexcontInitial 
                        let tokenData = {tokenData with RightColumn=rightc;ColorClass=TokenColorKind.PreprocessorKeyword;CharClass=TokenCharKind.Keyword;TriggerClass=TriggerClass.None} 
                        let lexintFinal = FinalState(lexcontFinal)
                        Some(tokenData),lexintFinal
                    | _ -> tokenDataOption,lexintFinal
                | _ -> tokenDataOption,lexintFinal
            | _, SingleLineTokenState.BeforeHash, TOKEN_WHITESPACE -> 
                // Allow leading whitespace.
                tokenDataOption,lexintFinal
            | _ -> 
                singleLineTokenState <- SingleLineTokenState.NoFurtherMatchPossible
                tokenDataOption,lexintFinal
            
        tokenDataOption, lexintFinal

[<Sealed>]
type SourceTokenizer(defineConstants : string list, filename : string) =     
    let lexResourceManager = new Lexhelp.LexResourceManager() 

    let lexArgsLightOn = mkLexargs(filename,defineConstants,LightSyntaxStatus(true,false),lexResourceManager, ref [],DiscardErrorsLogger) 
    let lexArgsLightOff = mkLexargs(filename,defineConstants,LightSyntaxStatus(false,false),lexResourceManager, ref [],DiscardErrorsLogger) 
    
    member this.CreateLineTokenizer(lineText: string) = 
        LineTokenizer(lineText, filename, lexArgsLightOn, lexArgsLightOff)

