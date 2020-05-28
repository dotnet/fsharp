// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// DSL to create structured layout objects with optional breaks and render them
module public FSharp.Compiler.Layout

open System.Text
open System.IO
open FSharp.Compiler.Range
open Internal.Utilities.StructuredFormat

type layout = Internal.Utilities.StructuredFormat.Layout
type LayoutTag = Internal.Utilities.StructuredFormat.LayoutTag
type TaggedText = Internal.Utilities.StructuredFormat.TaggedText

type NavigableTaggedText =
    new : TaggedText * range -> NavigableTaggedText
    member Range: range
    interface TaggedText

val mkNav : range -> TaggedText -> TaggedText

val emptyL                : Layout
val isEmptyL              : Layout -> bool
  
val wordL                 : TaggedText -> Layout

val sepL                  : TaggedText -> Layout

val rightL                : TaggedText -> Layout

val leftL                 : TaggedText -> Layout

/// never break "glue"
val ( ^^ )                : Layout -> Layout -> Layout   

/// optional break, indent=0
val ( ++ )                : Layout -> Layout -> Layout

// optional break, indent=1
val ( -- )                : Layout -> Layout -> Layout

/// optional break, indent=2
val ( --- )               : Layout -> Layout -> Layout

/// optional break, indent=3
val ( ---- )              : Layout -> Layout -> Layout

/// optional break, indent=4
val ( ----- )             : Layout -> Layout -> Layout

/// non-optional break ident=0
val ( @@ )                : Layout -> Layout -> Layout

/// non-optional break ident=1
val ( @@- )               : Layout -> Layout -> Layout

/// non-optional break ident=2
val ( @@-- )              : Layout -> Layout -> Layout

val commaListL            : Layout list -> Layout

val spaceListL            : Layout list -> Layout

val semiListL             : Layout list -> Layout

val sepListL              : Layout -> Layout list -> Layout

val bracketL              : Layout -> Layout

val tupleL                : Layout list -> Layout

val aboveL                : Layout -> Layout -> Layout

val aboveListL            : Layout list -> Layout

val optionL               : ('a -> Layout) -> 'a option -> Layout    

val listL                 : ('a -> Layout) -> 'a list   -> Layout

val squashTo              : int -> Layout -> Layout

val showL                 : Layout -> string

val outL                  : TextWriter -> Layout -> unit

val bufferL               : StringBuilder -> Layout -> unit

module TaggedTextOps =
    val tagActivePatternCase : (string -> TaggedText)
    val tagActivePatternResult : (string -> TaggedText)
    val tagAlias : (string -> TaggedText)
    val tagClass : (string -> TaggedText)
    val tagUnion : (string -> TaggedText)
    val tagUnionCase : (string -> TaggedText)
    val tagDelegate : (string -> TaggedText)
    val tagEnum : (string -> TaggedText)
    val tagEvent : (string -> TaggedText)
    val tagField : (string -> TaggedText)
    val tagInterface : (string -> TaggedText)
    val tagKeyword : (string -> TaggedText)
    val tagLineBreak : (string -> TaggedText)
    val tagMethod : (string -> TaggedText)
    val tagLocal : (string -> TaggedText)
    val tagRecord : (string -> TaggedText)
    val tagRecordField : (string -> TaggedText)
    val tagModule : (string -> TaggedText)
    val tagModuleBinding : (string -> TaggedText)
    val tagMember : (string -> TaggedText)
    val tagNamespace : (string -> TaggedText)
    val tagNumericLiteral : (string -> TaggedText)
    val tagOperator : (string -> TaggedText)
    val tagParameter : (string -> TaggedText)
    val tagProperty : (string -> TaggedText)
    val tagSpace : (string -> TaggedText)
    val tagStringLiteral : (string -> TaggedText)
    val tagStruct : (string -> TaggedText)
    val tagTypeParameter : (string -> TaggedText)
    val tagText : (string -> TaggedText)
    val tagPunctuation : (string -> TaggedText)
    val tagUnknownEntity : (string -> TaggedText)
    val tagUnknownType : (string -> TaggedText)

    module Literals =
        // common tagged literals
        val lineBreak : TaggedText
        val space : TaggedText
        val comma : TaggedText
        val dot : TaggedText
        val semicolon : TaggedText
        val leftParen : TaggedText
        val rightParen : TaggedText
        val leftBracket : TaggedText
        val rightBracket : TaggedText
        val leftBrace: TaggedText
        val rightBrace : TaggedText
        val leftBraceBar: TaggedText
        val rightBraceBar : TaggedText
        val leftAngle: TaggedText
        val rightAngle: TaggedText
        val equals : TaggedText
        val arrow : TaggedText
        val questionMark : TaggedText
        val colon: TaggedText
        val minus: TaggedText
        val keywordTrue: TaggedText
        val keywordFalse: TaggedText

module SepL =
    val dot: Layout
    val star: Layout
    val colon: Layout
    val questionMark: Layout
    val leftParen: Layout
    val comma: Layout
    val space: Layout
    val leftBracket: Layout
    val leftAngle: Layout
    val lineBreak: Layout
    val rightParen: Layout

module WordL =
    val arrow: Layout
    val star: Layout
    val colon: Layout
    val equals: Layout
    val keywordNew: Layout
    val structUnit: Layout
    val keywordStatic: Layout
    val keywordMember: Layout
    val keywordVal: Layout
    val keywordEvent: Layout
    val keywordWith: Layout
    val keywordSet: Layout
    val keywordGet: Layout
    val keywordTrue: Layout
    val keywordFalse: Layout
    val bar: Layout
    val keywordStruct: Layout
    val keywordInherit: Layout
    val keywordEnd: Layout
    val keywordNested: Layout
    val keywordType: Layout
    val keywordDelegate: Layout
    val keywordOf: Layout
    val keywordInternal: Layout
    val keywordPrivate: Layout
    val keywordAbstract: Layout
    val keywordOverride: Layout
    val keywordEnum: Layout

module LeftL =
    val leftParen: Layout
    val questionMark: Layout
    val colon: Layout
    val leftBracketAngle: Layout
    val leftBracketBar: Layout
    val keywordTypeof: Layout
    val keywordTypedefof: Layout

module RightL =
    val comma: Layout
    val rightParen: Layout
    val colon: Layout
    val rightBracket: Layout
    val rightAngle: Layout
    val rightBracketAngle: Layout
    val rightBracketBar: Layout

/// Render a Layout yielding an 'a using a 'b (hidden state) type 
type LayoutRenderer<'a,'b> =
    abstract Start    : unit -> 'b
    abstract AddText  : 'b -> TaggedText -> 'b
    abstract AddBreak : 'b -> int -> 'b
    abstract AddTag   : 'b -> string * (string * string) list * bool -> 'b
    abstract Finish   : 'b -> 'a

type NoState = NoState
type NoResult = NoResult

/// Run a render on a Layout       
val renderL  : LayoutRenderer<'b,'a> -> Layout -> 'b

/// Render layout to string 
val stringR  : LayoutRenderer<string,string list>

/// Render layout to channel
val channelR : TextWriter -> LayoutRenderer<NoResult,NoState>

/// Render layout to StringBuilder
val bufferR  : StringBuilder -> LayoutRenderer<NoResult,NoState>

/// Render layout to collector of TaggedText
val taggedTextListR  : collector: (TaggedText -> unit) -> LayoutRenderer<NoResult, NoState>

