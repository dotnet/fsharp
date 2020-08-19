// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Layout

open System
open System.IO
open Internal.Utilities.StructuredFormat
open Microsoft.FSharp.Core.Printf

#nowarn "62" // This construct is for ML compatibility.

type layout = Internal.Utilities.StructuredFormat.Layout
type LayoutTag = Internal.Utilities.StructuredFormat.LayoutTag
type TaggedText = Internal.Utilities.StructuredFormat.TaggedText

type NavigableTaggedText(taggedText: TaggedText, range: Range.range) =
    member val Range = range
    interface TaggedText with
        member _.Tag = taggedText.Tag
        member _.Text = taggedText.Text

let mkNav r t = NavigableTaggedText(t, r) :> TaggedText

let spaces n = new String(' ', n)

// NOTE: emptyL might be better represented as a constructor, so then (Sep"") would have true meaning
let emptyL = Leaf (true, TaggedTextOps.mkTag LayoutTag.Text "", true)
let isEmptyL = function Leaf(true, tag, true) when tag.Text = "" -> true | _ -> false
      
let mkNode l r joint =
   if isEmptyL l then r else
   if isEmptyL r then l else
   Node(l, r, joint)

//--------------------------------------------------------------------------
//INDEX: constructors
//--------------------------------------------------------------------------

let wordL  (str:TaggedText) = Leaf (false, str, false)
let sepL   (str:TaggedText) = Leaf (true, str, true)   
let rightL (str:TaggedText) = Leaf (true, str, false)   
let leftL  (str:TaggedText) = Leaf (false, str, true)

module TaggedTextOps =
    let tagActivePatternCase = TaggedTextOps.mkTag LayoutTag.ActivePatternCase
    let tagActivePatternResult = TaggedTextOps.mkTag LayoutTag.ActivePatternResult
    let tagAlias = TaggedTextOps.tagAlias
    let tagClass = TaggedTextOps.tagClass
    let tagUnion = TaggedTextOps.mkTag LayoutTag.Union
    let tagUnionCase = TaggedTextOps.tagUnionCase
    let tagDelegate = TaggedTextOps.tagDelegate
    let tagEnum = TaggedTextOps.tagEnum
    let tagEvent = TaggedTextOps.tagEvent
    let tagField = TaggedTextOps.tagField
    let tagInterface = TaggedTextOps.tagInterface
    let tagKeyword = TaggedTextOps.tagKeyword
    let tagLineBreak = TaggedTextOps.tagLineBreak
    let tagLocal = TaggedTextOps.tagLocal
    let tagRecord = TaggedTextOps.tagRecord
    let tagRecordField = TaggedTextOps.tagRecordField
    let tagMethod = TaggedTextOps.tagMethod
    let tagMember = TaggedTextOps.mkTag LayoutTag.Member
    let tagModule = TaggedTextOps.tagModule
    let tagModuleBinding = TaggedTextOps.tagModuleBinding
    let tagNamespace = TaggedTextOps.tagNamespace
    let tagNumericLiteral = TaggedTextOps.tagNumericLiteral
    let tagOperator = TaggedTextOps.tagOperator
    let tagParameter = TaggedTextOps.tagParameter
    let tagProperty = TaggedTextOps.tagProperty
    let tagSpace = TaggedTextOps.tagSpace
    let tagStringLiteral = TaggedTextOps.tagStringLiteral
    let tagStruct = TaggedTextOps.tagStruct
    let tagTypeParameter = TaggedTextOps.tagTypeParameter
    let tagText = TaggedTextOps.tagText
    let tagPunctuation = TaggedTextOps.tagPunctuation
    let tagUnknownEntity = TaggedTextOps.mkTag LayoutTag.UnknownEntity
    let tagUnknownType = TaggedTextOps.mkTag LayoutTag.UnknownType

    module Literals =
        // common tagged literals
        let lineBreak = TaggedTextOps.Literals.lineBreak
        let space = TaggedTextOps.Literals.space
        let comma = TaggedTextOps.Literals.comma
        let semicolon = TaggedTextOps.Literals.semicolon
        let leftParen = TaggedTextOps.Literals.leftParen
        let rightParen = TaggedTextOps.Literals.rightParen
        let leftBracket = TaggedTextOps.Literals.leftBracket
        let rightBracket = TaggedTextOps.Literals.rightBracket
        let leftBrace = TaggedTextOps.Literals.leftBrace
        let rightBrace = TaggedTextOps.Literals.rightBrace
        let leftBraceBar = TaggedTextOps.Literals.leftBraceBar
        let rightBraceBar = TaggedTextOps.Literals.rightBraceBar
        let equals = TaggedTextOps.Literals.equals
        let arrow = TaggedTextOps.Literals.arrow
        let questionMark = TaggedTextOps.Literals.questionMark
        let dot = tagPunctuation "."
        let leftAngle = tagPunctuation "<"
        let rightAngle = tagPunctuation ">"
        let star = tagOperator "*"
        let colon = tagPunctuation ":"
        let minus = tagPunctuation "-"
        let keywordNew = tagKeyword "new"
        let leftBracketAngle = tagPunctuation "[<"
        let rightBracketAngle = tagPunctuation ">]"
        let structUnit = tagStruct "unit"
        let keywordStatic = tagKeyword "static"
        let keywordMember = tagKeyword "member"
        let keywordVal = tagKeyword "val"
        let keywordEvent = tagKeyword "event"
        let keywordWith = tagKeyword "with"
        let keywordSet = tagKeyword "set"
        let keywordGet = tagKeyword "get"
        let keywordTrue = tagKeyword "true"
        let keywordFalse = tagKeyword "false"
        let bar = tagPunctuation "|"
        let keywordStruct = tagKeyword "struct"
        let keywordInherit = tagKeyword "inherit"
        let keywordEnd = tagKeyword "end"
        let keywordNested = tagKeyword "nested"
        let keywordType = tagKeyword "type"
        let keywordDelegate = tagKeyword "delegate"
        let keywordOf = tagKeyword "of"
        let keywordInternal = tagKeyword "internal"
        let keywordPrivate = tagKeyword "private"
        let keywordAbstract = tagKeyword "abstract"
        let keywordOverride = tagKeyword "override"
        let keywordEnum = tagKeyword "enum"
        let leftBracketBar = tagPunctuation  "[|"
        let rightBracketBar = tagPunctuation "|]"
        let keywordTypeof = tagKeyword "typeof"
        let keywordTypedefof = tagKeyword "typedefof"

open TaggedTextOps

module SepL =
    let dot = sepL Literals.dot
    let star = sepL Literals.star
    let colon = sepL Literals.colon
    let questionMark = sepL Literals.questionMark
    let leftParen = sepL Literals.leftParen
    let comma = sepL Literals.comma
    let space = sepL Literals.space
    let leftBracket = sepL Literals.leftBracket
    let leftAngle = sepL Literals.leftAngle
    let lineBreak = sepL Literals.lineBreak
    let rightParen = sepL Literals.rightParen

module WordL =
    let arrow = wordL Literals.arrow
    let star = wordL Literals.star
    let colon = wordL Literals.colon
    let equals = wordL Literals.equals
    let keywordNew = wordL Literals.keywordNew
    let structUnit = wordL Literals.structUnit
    let keywordStatic = wordL Literals.keywordStatic
    let keywordMember = wordL Literals.keywordMember
    let keywordVal = wordL Literals.keywordVal
    let keywordEvent = wordL Literals.keywordEvent
    let keywordWith = wordL Literals.keywordWith
    let keywordSet = wordL Literals.keywordSet
    let keywordGet = wordL Literals.keywordGet
    let keywordTrue = wordL Literals.keywordTrue
    let keywordFalse = wordL Literals.keywordFalse
    let bar = wordL Literals.bar
    let keywordStruct = wordL Literals.keywordStruct
    let keywordInherit = wordL Literals.keywordInherit
    let keywordEnd = wordL Literals.keywordEnd
    let keywordNested = wordL Literals.keywordNested
    let keywordType = wordL Literals.keywordType
    let keywordDelegate = wordL Literals.keywordDelegate
    let keywordOf = wordL Literals.keywordOf
    let keywordInternal = wordL Literals.keywordInternal
    let keywordPrivate = wordL Literals.keywordPrivate
    let keywordAbstract = wordL Literals.keywordAbstract
    let keywordOverride = wordL Literals.keywordOverride
    let keywordEnum = wordL Literals.keywordEnum

module LeftL =
    let leftParen = leftL Literals.leftParen
    let questionMark = leftL Literals.questionMark
    let colon = leftL Literals.colon
    let leftBracketAngle = leftL Literals.leftBracketAngle
    let leftBracketBar = leftL Literals.leftBracketBar
    let keywordTypeof = leftL Literals.keywordTypeof
    let keywordTypedefof = leftL Literals.keywordTypedefof

module RightL =
    let comma = rightL Literals.comma
    let rightParen = rightL Literals.rightParen
    let colon = rightL Literals.colon
    let rightBracket = rightL Literals.rightBracket
    let rightAngle = rightL Literals.rightAngle
    let rightBracketAngle = rightL Literals.rightBracketAngle
    let rightBracketBar = rightL Literals.rightBracketBar

let aboveL  l r = mkNode l r (Broken 0)

let tagAttrL str attrs ly = Attr (str, attrs, ly)

//--------------------------------------------------------------------------
//INDEX: constructors derived
//--------------------------------------------------------------------------

let apply2 f l r = if isEmptyL l then r else
                   if isEmptyL r then l else f l r

let (^^)    l r = mkNode l r (Unbreakable)
let (++)    l r = mkNode l r (Breakable 0)
let (--)    l r = mkNode l r (Breakable 1)
let (---)   l r = mkNode l r (Breakable 2)
let (----)  l r = mkNode l r (Breakable 3)
let (-----) l r = mkNode l r (Breakable 4)    
let (@@)    l r = apply2 (fun l r -> mkNode l r (Broken 0)) l r
let (@@-)   l r = apply2 (fun l r -> mkNode l r (Broken 1)) l r
let (@@--)  l r = apply2 (fun l r -> mkNode l r (Broken 2)) l r

let tagListL tagger = function
  | []    -> emptyL
  | [x]   -> x
  | x :: xs ->
      let rec process' prefixL = function
      | []    -> prefixL
      | y :: ys -> process' ((tagger prefixL) ++ y) ys in
      process' x xs

let commaListL x = tagListL (fun prefixL -> prefixL ^^ rightL Literals.comma) x

let semiListL x  = tagListL (fun prefixL -> prefixL ^^ rightL Literals.semicolon) x

let spaceListL x = tagListL (fun prefixL -> prefixL) x

let sepListL x y = tagListL (fun prefixL -> prefixL ^^ x) y

let bracketL l = leftL Literals.leftParen ^^ l ^^ rightL Literals.rightParen

let tupleL xs = bracketL (sepListL (sepL Literals.comma) xs)

let aboveListL = function
  | []    -> emptyL
  | [x]   -> x
  | x :: ys -> List.fold (fun pre y -> pre @@ y) x ys

let optionL xL = function
  | None   -> wordL (tagUnionCase "None")
  | Some x -> wordL (tagUnionCase "Some") -- (xL x)

let listL xL xs = leftL Literals.leftBracket ^^ sepListL (sepL Literals.semicolon) (List.map xL xs) ^^ rightL Literals.rightBracket

//--------------------------------------------------------------------------
//INDEX: LayoutRenderer
//--------------------------------------------------------------------------

type LayoutRenderer<'a, 'b> =
    abstract Start    : unit -> 'b
    abstract AddText  : 'b -> TaggedText -> 'b
    abstract AddBreak : 'b -> int -> 'b
    abstract AddTag   : 'b -> string * (string * string) list * bool -> 'b
    abstract Finish   : 'b -> 'a
      
let renderL (rr: LayoutRenderer<_, _>) layout =
    let rec addL z pos i layout k = 
      match layout with
      | ObjLeaf _ -> failwith "ObjLeaf should never appear here"
        (* pos is tab level *)
      | Leaf (_, text, _)                 -> 
          k(rr.AddText z text, i + text.Text.Length)
      | Node (l, r, Broken indent) -> 
          addL z pos i l <|
            fun (z, _i) ->
              let z, i = rr.AddBreak z (pos+indent), (pos+indent) 
              addL z (pos+indent) i r k
      | Node (l, r, _)             -> 
          let jm = Layout.JuxtapositionMiddle (l, r)
          addL z pos i l <|
            fun (z, i) ->
              let z, i = if jm then z, i else rr.AddText z Literals.space, i+1 
              let pos = i 
              addL z pos i r k
      | Attr (tag, attrs, l)                -> 
          let z   = rr.AddTag z (tag, attrs, true) 
          addL z pos i l <|
            fun (z, i) ->
              let z   = rr.AddTag z (tag, attrs, false) 
              k(z, i)
    let pos = 0 
    let z, i = rr.Start(), 0 
    let z, _i = addL z pos i layout id
    rr.Finish z

/// string render 
let stringR =
  { new LayoutRenderer<string, string list> with 
      member _.Start () = []
      member _.AddText rstrs taggedText = taggedText.Text :: rstrs
      member _.AddBreak rstrs n = (spaces n) :: "\n" ::  rstrs 
      member _.AddTag z (_, _, _) = z
      member _.Finish rstrs = String.Join("", Array.ofList (List.rev rstrs)) }

type NoState = NoState
type NoResult = NoResult

/// string render 
let taggedTextListR collector =
  { new LayoutRenderer<NoResult, NoState> with 
      member _.Start () = NoState
      member _.AddText z text = collector text; z
      member _.AddBreak rstrs n = collector Literals.lineBreak; collector (tagSpace(spaces n)); rstrs 
      member _.AddTag z (_, _, _) = z
      member _.Finish rstrs = NoResult }


/// channel LayoutRenderer
let channelR (chan:TextWriter) =
  { new LayoutRenderer<NoResult, NoState> with 
      member r.Start () = NoState
      member r.AddText z s = chan.Write s.Text; z
      member r.AddBreak z n = chan.WriteLine(); chan.Write (spaces n); z
      member r.AddTag z (tag, attrs, start) =  z
      member r.Finish z = NoResult }

/// buffer render
let bufferR os =
  { new LayoutRenderer<NoResult, NoState> with 
      member r.Start () = NoState
      member r.AddText z s = bprintf os "%s" s.Text; z
      member r.AddBreak z n = bprintf os "\n"; bprintf os "%s" (spaces n); z
      member r.AddTag z (tag, attrs, start) = z
      member r.Finish z = NoResult }

//--------------------------------------------------------------------------
//INDEX: showL, outL are most common
//--------------------------------------------------------------------------

let showL                   layout = renderL stringR         layout
let outL (chan:TextWriter)  layout = renderL (channelR chan) layout |> ignore
let bufferL os              layout = renderL (bufferR os)    layout |> ignore