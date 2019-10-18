// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Layout

open System
open System.Collections.Generic
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
        member x.Tag = taggedText.Tag
        member x.Text = taggedText.Text
let mkNav r t = NavigableTaggedText(t, r) :> TaggedText

let spaces n = new String(' ', n)


//--------------------------------------------------------------------------
// INDEX: support
//--------------------------------------------------------------------------

let rec juxtLeft = function
  | ObjLeaf (jl, _text, _jr)         -> jl
  | Leaf (jl, _text, _jr)            -> jl
  | Node (jl, _l, _jm, _r, _jr, _joint) -> jl
  | Attr (_tag, _attrs, l)           -> juxtLeft l

let rec juxtRight = function
  | ObjLeaf (_jl, _text, jr)         -> jr
  | Leaf (_jl, _text, jr)            -> jr
  | Node (_jl, _l, _jm, _r, jr, _joint) -> jr
  | Attr (_tag, _attrs, l)           -> juxtRight l

// NOTE: emptyL might be better represented as a constructor, so then (Sep"") would have true meaning
let emptyL = Leaf (true, Internal.Utilities.StructuredFormat.TaggedTextOps.tag LayoutTag.Text "", true)
let isEmptyL = function Leaf(true, tag, true) when tag.Text = "" -> true | _ -> false
      
let mkNode l r joint =
   if isEmptyL l then r else
   if isEmptyL r then l else
   let jl = juxtLeft  l 
   let jm = juxtRight l || juxtLeft r 
   let jr = juxtRight r 
   Node(jl, l, jm, r, jr, joint)


//--------------------------------------------------------------------------
//INDEX: constructors
//--------------------------------------------------------------------------

let wordL  (str:TaggedText) = Leaf (false, str, false)
let sepL   (str:TaggedText) = Leaf (true, str, true)   
let rightL (str:TaggedText) = Leaf (true, str, false)   
let leftL  (str:TaggedText) = Leaf (false, str, true)

module TaggedTextOps =
    let tagActivePatternCase = Internal.Utilities.StructuredFormat.TaggedTextOps.tag LayoutTag.ActivePatternCase
    let tagActivePatternResult = Internal.Utilities.StructuredFormat.TaggedTextOps.tag LayoutTag.ActivePatternResult
    let tagAlias = Internal.Utilities.StructuredFormat.TaggedTextOps.tagAlias
    let tagClass = Internal.Utilities.StructuredFormat.TaggedTextOps.tagClass
    let tagUnion = Internal.Utilities.StructuredFormat.TaggedTextOps.tag LayoutTag.Union
    let tagUnionCase = Internal.Utilities.StructuredFormat.TaggedTextOps.tagUnionCase
    let tagDelegate = Internal.Utilities.StructuredFormat.TaggedTextOps.tagDelegate
    let tagEnum = Internal.Utilities.StructuredFormat.TaggedTextOps.tagEnum
    let tagEvent = Internal.Utilities.StructuredFormat.TaggedTextOps.tagEvent
    let tagField = Internal.Utilities.StructuredFormat.TaggedTextOps.tagField
    let tagInterface = Internal.Utilities.StructuredFormat.TaggedTextOps.tagInterface
    let tagKeyword = Internal.Utilities.StructuredFormat.TaggedTextOps.tagKeyword
    let tagLineBreak = Internal.Utilities.StructuredFormat.TaggedTextOps.tagLineBreak
    let tagLocal = Internal.Utilities.StructuredFormat.TaggedTextOps.tagLocal
    let tagRecord = Internal.Utilities.StructuredFormat.TaggedTextOps.tagRecord
    let tagRecordField = Internal.Utilities.StructuredFormat.TaggedTextOps.tagRecordField
    let tagMethod = Internal.Utilities.StructuredFormat.TaggedTextOps.tagMethod
    let tagMember = Internal.Utilities.StructuredFormat.TaggedTextOps.tag LayoutTag.Member
    let tagModule = Internal.Utilities.StructuredFormat.TaggedTextOps.tagModule
    let tagModuleBinding = Internal.Utilities.StructuredFormat.TaggedTextOps.tagModuleBinding
    let tagNamespace = Internal.Utilities.StructuredFormat.TaggedTextOps.tagNamespace
    let tagNumericLiteral = Internal.Utilities.StructuredFormat.TaggedTextOps.tagNumericLiteral
    let tagOperator = Internal.Utilities.StructuredFormat.TaggedTextOps.tagOperator
    let tagParameter = Internal.Utilities.StructuredFormat.TaggedTextOps.tagParameter
    let tagProperty = Internal.Utilities.StructuredFormat.TaggedTextOps.tagProperty
    let tagSpace = Internal.Utilities.StructuredFormat.TaggedTextOps.tagSpace
    let tagStringLiteral = Internal.Utilities.StructuredFormat.TaggedTextOps.tagStringLiteral
    let tagStruct = Internal.Utilities.StructuredFormat.TaggedTextOps.tagStruct
    let tagTypeParameter = Internal.Utilities.StructuredFormat.TaggedTextOps.tagTypeParameter
    let tagText = Internal.Utilities.StructuredFormat.TaggedTextOps.tagText
    let tagPunctuation = Internal.Utilities.StructuredFormat.TaggedTextOps.tagPunctuation
    let tagUnknownEntity = Internal.Utilities.StructuredFormat.TaggedTextOps.tag LayoutTag.UnknownEntity
    let tagUnknownType = Internal.Utilities.StructuredFormat.TaggedTextOps.tag LayoutTag.UnknownType

    module Literals =
        // common tagged literals
        let lineBreak = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.lineBreak
        let space = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.space
        let comma = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.comma
        let semicolon = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.semicolon
        let leftParen = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.leftParen
        let rightParen = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.rightParen
        let leftBracket = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.leftBracket
        let rightBracket = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.rightBracket
        let leftBrace = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.leftBrace
        let rightBrace = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.rightBrace
        let leftBraceBar = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.leftBraceBar
        let rightBraceBar = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.rightBraceBar
        let equals = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.equals
        let arrow = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.arrow
        let questionMark = Internal.Utilities.StructuredFormat.TaggedTextOps.Literals.questionMark
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
//INDEX: breaks v2
//--------------------------------------------------------------------------
 
// A very quick implementation of break stack.
type breaks = Breaks of 
                 /// pos of next free slot 
                 int *     
                 /// pos of next possible "outer" break - OR - outer=next if none possible 
                 int *     
                 /// stack of savings, -ve means it has been broken 
                 int array 

// next  is next slot to push into - aka size of current occupied stack.
// outer counts up from 0, and is next slot to break if break forced.
// - if all breaks forced, then outer=next.
// - popping under these conditions needs to reduce outer and next.
let chunkN = 400      
let breaks0 () = Breaks(0, 0, Array.create chunkN 0)
let pushBreak saving (Breaks(next, outer, stack)) =
    let stack = if next = stack.Length then
                  Array.append stack (Array.create chunkN 0) (* expand if full *)
                else
                  stack
    stack.[next] <- saving
    Breaks(next+1, outer, stack)

let popBreak (Breaks(next, outer, stack)) =
    if next=0 then raise (Failure "popBreak: underflow")
    let topBroke = stack.[next-1] < 0 
    let outer = if outer=next then outer-1 else outer   (* if all broken, unwind *)
    let next  = next - 1 
    Breaks(next, outer, stack), topBroke

let forceBreak (Breaks(next, outer, stack)) =
    if outer=next then
      (* all broken *)
      None
    else
      let saving = stack.[outer] 
      stack.[outer] <- -stack.[outer]    
      let outer = outer+1 
      Some (Breaks(next, outer, stack), saving)

let squashTo maxWidth layout =
   // breaks = break context, can force to get indentation savings.
   // pos    = current position in line
   // layout = to fit
   //------
   // returns:
   // breaks
   // layout - with breaks put in to fit it.
   // pos    - current pos in line = rightmost position of last line of block.
   // offset - width of last line of block
   // NOTE: offset <= pos -- depending on tabbing of last block
   let rec fit breaks (pos, layout) =
       (*printf "\n\nCalling pos=%d layout=[%s]\n" pos (showL layout)*)
       let breaks, layout, pos, offset =
           match layout with
           | ObjLeaf _ -> failwith "ObjLeaf should not appear here"
           | Attr (tag, attrs, l) ->
               let breaks, layout, pos, offset = fit breaks (pos, l) 
               let layout = Attr (tag, attrs, layout) 
               breaks, layout, pos, offset
           | Leaf (_jl, taggedText, _jr) ->
               let textWidth = taggedText.Text.Length 
               let rec fitLeaf breaks pos =
                 if pos + textWidth <= maxWidth then
                   breaks, layout, pos + textWidth, textWidth (* great, it fits *)
                 else
                   match forceBreak breaks with
                     None                 -> (breaks, layout, pos + textWidth, textWidth (* tough, no more breaks *))
                   | Some (breaks, saving) -> (let pos = pos - saving in fitLeaf breaks pos) 
               fitLeaf breaks pos

           | Node (jl, l, jm, r, jr, joint) ->
               let mid = if jm then 0 else 1 
               match joint with
               | Unbreakable    ->
                   let breaks, l, pos, offsetl = fit breaks (pos, l)     (* fit left *)
                   let pos = pos + mid                               (* fit space if juxt says so *)
                   let breaks, r, pos, offsetr = fit breaks (pos, r)     (* fit right *)
                   breaks, Node (jl, l, jm, r, jr, Unbreakable), pos, offsetl + mid + offsetr
               | Broken indent ->
                   let breaks, l, pos, offsetl = fit breaks (pos, l)     (* fit left *)
                   let pos = pos - offsetl + indent                  (* broken so - offset left + indent *)
                   let breaks, r, pos, offsetr = fit breaks (pos, r)     (* fit right *)
                   breaks, Node (jl, l, jm, r, jr, Broken indent), pos, indent + offsetr
               | Breakable indent ->
                   let breaks, l, pos, offsetl = fit breaks (pos, l)     (* fit left *)
                   (* have a break possibility, with saving *)
                   let saving = offsetl + mid - indent 
                   let pos = pos + mid 
                   if saving>0 then
                     let breaks = pushBreak saving breaks 
                     let breaks, r, pos, offsetr = fit breaks (pos, r) 
                     let breaks, broken = popBreak breaks 
                     if broken then
                       breaks, Node (jl, l, jm, r, jr, Broken indent), pos, indent + offsetr
                     else
                       breaks, Node (jl, l, jm, r, jr, Breakable indent), pos, offsetl + mid + offsetr
                   else
                     (* actually no saving so no break *)
                     let breaks, r, pos, offsetr = fit breaks (pos, r) 
                     breaks, Node (jl, l, jm, r, jr, Breakable indent), pos, offsetl + mid + offsetr
       (*printf "\nDone:     pos=%d offset=%d" pos offset*)
       breaks, layout, pos, offset
   let breaks = breaks0 () 
   let pos = 0 
   let _breaks, layout, _pos, _offset = fit breaks (pos, layout) 
   layout

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
      | ObjLeaf _ -> failwith "ObjLeaf should never apper here"
        (* pos is tab level *)
      | Leaf (_, text, _)                 -> 
          k(rr.AddText z text, i + text.Text.Length)
      | Node (_, l, _, r, _, Broken indent) -> 
          addL z pos i l <|
            fun (z, _i) ->
              let z, i = rr.AddBreak z (pos+indent), (pos+indent) 
              addL z (pos+indent) i r k
      | Node (_, l, jm, r, _, _)             -> 
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
      member x.Start () = []
      member x.AddText rstrs taggedText = taggedText.Text :: rstrs
      member x.AddBreak rstrs n = (spaces n) :: "\n" ::  rstrs 
      member x.AddTag z (_, _, _) = z
      member x.Finish rstrs = String.Join("", Array.ofList (List.rev rstrs)) }

type NoState = NoState
type NoResult = NoResult

/// string render 
let taggedTextListR collector =
  { new LayoutRenderer<NoResult, NoState> with 
      member x.Start () = NoState
      member x.AddText z text = collector text; z
      member x.AddBreak rstrs n = collector Literals.lineBreak; collector (tagSpace(spaces n)); rstrs 
      member x.AddTag z (_, _, _) = z
      member x.Finish rstrs = NoResult }


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