//----------------------------------------------------------------------------
// Copyright (c) 2002-2012 Microsoft Corporation. 
//
// This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
// copy of the license can be found in the License.html file at the root of this distribution. 
// By using this source code in any fashion, you are agreeing to be bound 
// by the terms of the Apache License, Version 2.0.
//
// You must not remove this notice, or any other, from this software.
//----------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open System.Collections.Generic
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range

/// Represents encoded information for the end-of-line continuation of lexing
type FSharpTokenizerLexState = int64

/// Represents stable information for the state of the lexing engine at the end of a line
type internal FSharpTokenizerColorState =
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
    

/// Gives an indication of the color class to assign to the token an IDE
type internal FSharpTokenColorKind =
    | Default = 0
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
    
/// Gives an indication of what should happen when the token is typed in an IDE
type internal FSharpTokenTriggerClass =
    | None         = 0x00000000
    | MemberSelect = 0x00000001
    | MatchBraces  = 0x00000002
    | ChoiceSelect = 0x00000004
    | MethodTip    = 0x000000F0
    | ParamStart   = 0x00000010
    | ParamNext    = 0x00000020
    | ParamEnd     = 0x00000040    
    
/// Gives an indication of the class to assign to the characters of the token in an IDE
type internal FSharpTokenCharKind = 
    | Default     = 0x00000000
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

/// Some of the values in the field FSharpTokenInfo.Tag
module internal FSharpTokenTag = 
    /// Indicates the token is an identifier
    val Identifier: int
    /// Indicates the token is a string
    val String : int
    /// Indicates the token is an identifier (synonym for FSharpTokenTag.Identifer)
    val IDENT : int
    /// Indicates the token is an string (synonym for FSharpTokenTag.String)
    val STRING : int
    /// Indicates the token is a `(`
    val LPAREN : int
    /// Indicates the token is a `)`
    val RPAREN : int
    /// Indicates the token is a `[`
    val LBRACK : int
    /// Indicates the token is a `]`
    val RBRACK : int
    /// Indicates the token is a `{`
    val LBRACE : int
    /// Indicates the token is a `}`
    val RBRACE : int
    /// Indicates the token is a `[<`
    val LBRACK_LESS : int
    /// Indicates the token is a `>]`
    val GREATER_RBRACK : int
    /// Indicates the token is a `<`
    val LESS : int
    /// Indicates the token is a `>`
    val GREATER : int
    /// Indicates the token is a `[|`
    val LBRACK_BAR : int
    /// Indicates the token is a `|]`
    val BAR_RBRACK : int
    /// Indicates the token is a `+` or `-`
    val PLUS_MINUS_OP : int
    /// Indicates the token is a `-`
    val MINUS : int
    /// Indicates the token is a `*`
    val STAR : int
    /// Indicates the token is a `%`
    val INFIX_STAR_DIV_MOD_OP : int
    /// Indicates the token is a `%`
    val PERCENT_OP : int
    /// Indicates the token is a `^`
    val INFIX_AT_HAT_OP : int
    /// Indicates the token is a `?`
    val QMARK : int
    /// Indicates the token is a `:`
    val COLON : int
    /// Indicates the token is a `=`
    val EQUALS : int
    /// Indicates the token is a `;`
    val SEMICOLON : int
    /// Indicates the token is a `,`
    val COMMA : int
    /// Indicates the token is a `.`
    val DOT : int
    /// Indicates the token is a `..`
    val DOT_DOT : int
    /// Indicates the token is a `..`
    val INT32_DOT_DOT : int
    /// Indicates the token is a `..`
    val UNDERSCORE : int
    /// Indicates the token is a `_`
    val BAR : int
    /// Indicates the token is a `:>`
    val COLON_GREATER : int
    /// Indicates the token is a `:?>`
    val COLON_QMARK_GREATER : int
    /// Indicates the token is a `:?`
    val COLON_QMARK : int
    /// Indicates the token is a `|`
    val INFIX_BAR_OP : int
    /// Indicates the token is a `|`
    val INFIX_COMPARE_OP : int
    /// Indicates the token is a `::`
    val COLON_COLON : int
    /// Indicates the token is a `@@`
    val AMP_AMP : int
    /// Indicates the token is a `~`
    val PREFIX_OP : int
    /// Indicates the token is a `:=`
    val COLON_EQUALS : int
    /// Indicates the token is a `||`
    val BAR_BAR : int
    /// Indicates the token is a `->`
    val RARROW : int
    /// Indicates the token is a `"`
    val QUOTE : int
    
/// Information about a particular token from the tokenizer
type internal FSharpTokenInfo = 
    { /// Left column of the token.
      LeftColumn:int
      /// Right column of the token.
      RightColumn:int
      ColorClass:FSharpTokenColorKind
      /// Gives an indication of the class to assign to the token an IDE
      CharClass:FSharpTokenCharKind
      /// Actions taken when the token is typed
      FSharpTokenTriggerClass:FSharpTokenTriggerClass
      /// The tag is an integer identifier for the token
      Tag:int
      /// Provides additional information about the token
      TokenName:string;
      /// The full length consumed by this match, including delayed tokens (which can be ignored in naive lexers)
      FullMatchedLength: int }

/// Object to tokenize a line of F# source code, starting with the given lexState.  The lexState should be 0 for
/// the first line of text. Returns an array of ranges of the text and two enumerations categorizing the
/// tokens and characters covered by that range, i.e. FSharpTokenColorKind and FSharpTokenCharKind.  The enumerations
/// are somewhat adhoc but useful enough to give good colorization options to the user in an IDE.
///
/// A new lexState is also returned.  An IDE-plugin should in general cache the lexState 
/// values for each line of the edited code.
[<Sealed>] 
type internal FSharpLineTokenizer =
    /// Scan one token from the line
    member ScanToken : lexState:FSharpTokenizerLexState -> FSharpTokenInfo option * FSharpTokenizerLexState
    static member ColorStateOfLexState : FSharpTokenizerLexState -> FSharpTokenizerColorState
    static member LexStateOfColorState : FSharpTokenizerColorState -> FSharpTokenizerLexState
    

/// Tokenizer for a source file. Holds some expensive-to-compute resources at the scope of the file.
[<Sealed>]
type internal FSharpSourceTokenizer =
    new : conditionalDefines:string list * fileName:string -> FSharpSourceTokenizer
    member CreateLineTokenizer : lineText:string -> FSharpLineTokenizer
    member CreateBufferTokenizer : bufferFiller:(char[] * int * int -> int) -> FSharpLineTokenizer
    

module internal TestExpose =     
    val TokenInfo                                    : Parser.token -> (FSharpTokenColorKind * FSharpTokenCharKind * FSharpTokenTriggerClass) 


