// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// API to the compiler as an incremental service for lexing.
//----------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range
open System.Collections.Generic

/// Represents encoded information for the end-of-line continutation of lexing
type internal LexState = int64

/// A line/column pair
type internal Position = int * int

/// A start-position/end-position pair
type internal Range = Position * Position

type internal TokenColorKind =
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
    
type internal TriggerClass =
    | None         = 0x00000000
    | MemberSelect = 0x00000001
    | MatchBraces  = 0x00000002
    | ChoiceSelect = 0x00000004
    | MethodTip    = 0x000000F0
    | ParamStart   = 0x00000010
    | ParamNext    = 0x00000020
    | ParamEnd     = 0x00000040    
    
type internal TokenCharKind = 
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
    
/// Information about a particular token from the tokenizer
type internal TokenInformation = 
    { /// Left column of the token.
      LeftColumn:int
      /// Right column of the token.
      RightColumn:int
      ColorClass:TokenColorKind
      CharClass:TokenCharKind
      /// Actions taken when the token is typed
      TriggerClass:TriggerClass
      /// The tag is an integer identifier for the token
      Tag:int
      /// Provides additional information about the token
      TokenName:string }

/// Object to tokenize a line of F# source code, starting with the given lexState.  The lexState should be 0 for
/// the first line of text. Returns an array of ranges of the text and two enumerations categorizing the
/// tokens and characters covered by that range, i.e. TokenColorKind and TokenCharKind.  The enumerations
/// are somewhat adhoc but useful enough to give good colorization options to the user in an IDE.
///
/// A new lexState is also returned.  An IDE-plugin should in general cache the lexState 
/// values for each line of the edited code.
[<Sealed>] 
type internal LineTokenizer =
    /// Scan one token from the line
    member ScanToken : lexState:LexState -> TokenInformation option * LexState

/// Tokenizer for a source file. Holds some expensive-to-compute resources at the scope of the file.
[<Sealed>]
type internal SourceTokenizer =
    new : conditionalDefines:string list * fileName:string -> SourceTokenizer
    member CreateLineTokenizer : lineText:string -> LineTokenizer
    

module internal TestExpose =     
    val TokenInfo                                    : Parser.token -> (TokenColorKind * TokenCharKind * TriggerClass) 

module internal Flags =
    val init : unit -> unit