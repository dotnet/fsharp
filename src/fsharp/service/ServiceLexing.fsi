// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open FSharp.Compiler 

type Position = int * int
type Range = Position * Position

/// Represents encoded information for the end-of-line continuation of lexing
[<Struct; CustomEquality; NoComparison>]
type FSharpTokenizerLexState = 
    { PosBits: int64
      OtherBits: int64 }
    static member Initial : FSharpTokenizerLexState
    member Equals : FSharpTokenizerLexState -> bool

/// Represents stable information for the state of the lexing engine at the end of a line
type FSharpTokenizerColorState =
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
type FSharpTokenColorKind =
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
    | Punctuation = 11
    
/// Gives an indication of what should happen when the token is typed in an IDE
type FSharpTokenTriggerClass =
    | None         = 0x00000000
    | MemberSelect = 0x00000001
    | MatchBraces  = 0x00000002
    | ChoiceSelect = 0x00000004
    | MethodTip    = 0x000000F0
    | ParamStart   = 0x00000010
    | ParamNext    = 0x00000020
    | ParamEnd     = 0x00000040    
    
/// Gives an indication of the class to assign to the characters of the token an IDE
type FSharpTokenCharKind = 
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
module FSharpTokenTag = 
    /// Indicates the token is an identifier
    val Identifier: int
    /// Indicates the token is a string
    val String : int
    /// Indicates the token is an identifier (synonym for FSharpTokenTag.Identifier)
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
    val DOT_DOT_HAT : int
    /// Indicates the token is a `..^`
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
    /// Indicates the token is a `<-`
    val LARROW : int
    /// Indicates the token is a `"`
    val QUOTE : int
    /// Indicates the token is a whitespace
    val WHITESPACE : int
    /// Indicates the token is a comment
    val COMMENT : int
    /// Indicates the token is a line comment
    val LINE_COMMENT : int
    /// Indicates the token is keyword `begin`
    val BEGIN : int
    /// Indicates the token is keyword `do`
    val DO : int
    /// Indicates the token is keyword `function`
    val FUNCTION : int
    /// Indicates the token is keyword `then`
    val THEN : int
    /// Indicates the token is keyword `else`
    val ELSE : int
    /// Indicates the token is keyword `struct`
    val STRUCT : int
    /// Indicates the token is keyword `class`
    val CLASS : int
    /// Indicates the token is keyword `try`
    val TRY : int
    /// Indicates the token is keyword `with`
    val WITH : int
    /// Indicates the token is keyword `with` in #light
    val OWITH : int
    /// Indicates the token is keyword `new` 
    val NEW : int
    
/// Information about a particular token from the tokenizer
type FSharpTokenInfo = 
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

/// Object to tokenize a line of F# source code, starting with the given lexState.  The lexState should be FSharpTokenizerLexState.Initial for
/// the first line of text. Returns an array of ranges of the text and two enumerations categorizing the
/// tokens and characters covered by that range, i.e. FSharpTokenColorKind and FSharpTokenCharKind.  The enumerations
/// are somewhat adhoc but useful enough to give good colorization options to the user in an IDE.
///
/// A new lexState is also returned.  An IDE-plugin should in general cache the lexState 
/// values for each line of the edited code.
[<Sealed>] 
type FSharpLineTokenizer =
    /// Scan one token from the line
    member ScanToken : lexState:FSharpTokenizerLexState -> FSharpTokenInfo option * FSharpTokenizerLexState
    static member ColorStateOfLexState : FSharpTokenizerLexState -> FSharpTokenizerColorState
    static member LexStateOfColorState : FSharpTokenizerColorState -> FSharpTokenizerLexState
    
/// Tokenizer for a source file. Holds some expensive-to-compute resources at the scope of the file.
[<Sealed>]
type FSharpSourceTokenizer =
    new : conditionalDefines:string list * fileName:string option -> FSharpSourceTokenizer
    member CreateLineTokenizer : lineText:string -> FSharpLineTokenizer
    member CreateBufferTokenizer : bufferFiller:(char[] * int * int -> int) -> FSharpLineTokenizer
    
module internal TestExpose =     
    val TokenInfo : Parser.token -> (FSharpTokenColorKind * FSharpTokenCharKind * FSharpTokenTriggerClass)

module Keywords =
    /// Checks if adding backticks to identifier is needed.
    val DoesIdentifierNeedQuotation : string -> bool

    /// Add backticks if the identifier is a keyword.
    val QuoteIdentifierIfNeeded : string -> string

    /// Remove backticks if present.
    val NormalizeIdentifierBackticks : string -> string

    /// Keywords paired with their descriptions. Used in completion and quick info.
    val KeywordsWithDescription : (string * string) list
