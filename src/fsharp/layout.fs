// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Layout

open System
open System.IO
open Internal.Utilities.StructuredFormat
open Microsoft.FSharp.Core.Printf

#nowarn "62" // This construct is for ML compatibility.

type layout = Internal.Utilities.StructuredFormat.Layout
let spaces n = new String(' ',n)


//--------------------------------------------------------------------------
// INDEX: support
//--------------------------------------------------------------------------

let rec juxtLeft = function
  | Leaf (jl,_text,_jr)            -> jl
  | Node (jl,_l,_jm,_r,_jr,_joint) -> jl
  | Attr (_tag,_attrs,l)           -> juxtLeft l

let rec juxtRight = function
  | Leaf (_jl,_text,jr)            -> jr
  | Node (_jl,_l,_jm,_r,jr,_joint) -> jr
  | Attr (_tag,_attrs,l)           -> juxtRight l

// NOTE: emptyL might be better represented as a constructor, so then (Sep"") would have true meaning
let emptyL = Leaf (true,box "",true)
let isEmptyL = function Leaf(true,tag,true) when unbox tag = "" -> true | _ -> false
      
let mkNode l r joint =
   if isEmptyL l then r else
   if isEmptyL r then l else
   let jl = juxtLeft  l 
   let jm = juxtRight l || juxtLeft r 
   let jr = juxtRight r 
   Node(jl,l,jm,r,jr,joint)


//--------------------------------------------------------------------------
//INDEX: constructors
//--------------------------------------------------------------------------

let wordL  (str:string) = Leaf (false,box str,false)
let sepL   (str:string) = Leaf (true ,box str,true)   
let rightL (str:string) = Leaf (true ,box str,false)   
let leftL  (str:string) = Leaf (false,box str,true)

let aboveL  l r = mkNode l r (Broken 0)

let tagAttrL str attrs ly = Attr (str,attrs,ly)

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
  | x::xs ->
      let rec process' prefixL = function
      | []    -> prefixL
      | y::ys -> process' ((tagger prefixL) ++ y) ys in
      process' x xs
    
let commaListL x = tagListL (fun prefixL -> prefixL ^^ rightL ",") x
let semiListL x  = tagListL (fun prefixL -> prefixL ^^ rightL ";") x
let spaceListL x = tagListL (fun prefixL -> prefixL) x
let sepListL x y = tagListL (fun prefixL -> prefixL ^^ x) y

let bracketL l = leftL "(" ^^ l ^^ rightL ")"
let tupleL xs = bracketL (sepListL (sepL ",") xs)
let aboveListL = function
  | []    -> emptyL
  | [x]   -> x
  | x::ys -> List.fold (fun pre y -> pre @@ y) x ys

let optionL xL = function
  | None   -> wordL "None"
  | Some x -> wordL "Some" -- (xL x)

let listL xL xs = leftL "[" ^^ sepListL (sepL ";") (List.map xL xs) ^^ rightL "]"


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
let breaks0 () = Breaks(0,0,Array.create chunkN 0)
let pushBreak saving (Breaks(next,outer,stack)) =
    let stack = if next = stack.Length then
                  Array.append stack (Array.create chunkN 0) (* expand if full *)
                else
                  stack
    stack.[next] <- saving;
    Breaks(next+1,outer,stack)

let popBreak (Breaks(next,outer,stack)) =
    if next=0 then raise (Failure "popBreak: underflow");
    let topBroke = stack.[next-1] < 0 
    let outer = if outer=next then outer-1 else outer   (* if all broken, unwind *)
    let next  = next - 1 
    Breaks(next,outer,stack),topBroke

let forceBreak (Breaks(next,outer,stack)) =
    if outer=next then
      (* all broken *)
      None
    else
      let saving = stack.[outer] 
      stack.[outer] <- -stack.[outer];    
      let outer = outer+1 
      Some (Breaks(next,outer,stack),saving)

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
   let rec fit breaks (pos,layout) =
       (*printf "\n\nCalling pos=%d layout=[%s]\n" pos (showL layout);*)
       let breaks,layout,pos,offset =
           match layout with
           | Attr (tag,attrs,l) ->
               let breaks,layout,pos,offset = fit breaks (pos,l) 
               let layout = Attr (tag,attrs,layout) 
               breaks,layout,pos,offset
           | Leaf (_jl,text,_jr) ->
               let textWidth = (unbox<string> text).Length 
               let rec fitLeaf breaks pos =
                 if pos + textWidth <= maxWidth then
                   breaks,layout,pos + textWidth,textWidth (* great, it fits *)
                 else
                   match forceBreak breaks with
                     None                 -> (breaks,layout,pos + textWidth,textWidth (* tough, no more breaks *))
                   | Some (breaks,saving) -> (let pos = pos - saving in fitLeaf breaks pos) 
               fitLeaf breaks pos

           | Node (jl,l,jm,r,jr,joint) ->
               let mid = if jm then 0 else 1 
               match joint with
               | Unbreakable    ->
                   let breaks,l,pos,offsetl = fit breaks (pos,l)     (* fit left *)
                   let pos = pos + mid                               (* fit space if juxt says so *)
                   let breaks,r,pos,offsetr = fit breaks (pos,r)     (* fit right *)
                   breaks,Node (jl,l,jm,r,jr,Unbreakable),pos,offsetl + mid + offsetr
               | Broken indent ->
                   let breaks,l,pos,offsetl = fit breaks (pos,l)     (* fit left *)
                   let pos = pos - offsetl + indent                  (* broken so - offset left + indent *)
                   let breaks,r,pos,offsetr = fit breaks (pos,r)     (* fit right *)
                   breaks,Node (jl,l,jm,r,jr,Broken indent),pos,indent + offsetr
               | Breakable indent ->
                   let breaks,l,pos,offsetl = fit breaks (pos,l)     (* fit left *)
                   (* have a break possibility, with saving *)
                   let saving = offsetl + mid - indent 
                   let pos = pos + mid 
                   if saving>0 then
                     let breaks = pushBreak saving breaks 
                     let breaks,r,pos,offsetr = fit breaks (pos,r) 
                     let breaks,broken = popBreak breaks 
                     if broken then
                       breaks,Node (jl,l,jm,r,jr,Broken indent)   ,pos,indent + offsetr
                     else
                       breaks,Node (jl,l,jm,r,jr,Breakable indent),pos,offsetl + mid + offsetr
                   else
                     (* actually no saving so no break *)
                     let breaks,r,pos,offsetr = fit breaks (pos,r) 
                     breaks,Node (jl,l,jm,r,jr,Breakable indent)  ,pos,offsetl + mid + offsetr
       (*printf "\nDone:     pos=%d offset=%d" pos offset;*)
       breaks,layout,pos,offset
   let breaks = breaks0 () 
   let pos = 0 
   let _breaks,layout,_pos,_offset = fit breaks (pos,layout) 
   layout

//--------------------------------------------------------------------------
//INDEX: LayoutRenderer
//--------------------------------------------------------------------------

type LayoutRenderer<'a,'b> =
    abstract Start    : unit -> 'b
    abstract AddText  : 'b -> string -> 'b
    abstract AddBreak : 'b -> int -> 'b
    abstract AddTag   : 'b -> string * (string * string) list * bool -> 'b
    abstract Finish   : 'b -> 'a
      
let renderL (rr: LayoutRenderer<_,_>) layout =
    let rec addL z pos i layout k = 
      match layout with
        (* pos is tab level *)
      | Leaf (_,text,_)                 -> 
          k(rr.AddText z (unbox text),i + (unbox<string> text).Length)
      | Node (_,l,_,r,_,Broken indent) -> 
          addL z pos i l <|
            fun (z,_i) ->
              let z,i = rr.AddBreak z (pos+indent),(pos+indent) 
              addL z (pos+indent) i r k
      | Node (_,l,jm,r,_,_)             -> 
          addL z pos i l <|
            fun (z, i) ->
              let z,i = if jm then z,i else rr.AddText z " ",i+1 
              let pos = i 
              addL z pos i r k
      | Attr (tag,attrs,l)                -> 
          let z   = rr.AddTag z (tag,attrs,true) 
          addL z pos i l <|
            fun (z, i) ->
              let z   = rr.AddTag z (tag,attrs,false) 
              k(z,i)
    let pos = 0 
    let z,i = rr.Start(),0 
    let z,_i = addL z pos i layout id
    rr.Finish z

/// string render 
let stringR =
  { new LayoutRenderer<string,string list> with 
      member x.Start () = []
      member x.AddText rstrs text = text::rstrs
      member x.AddBreak rstrs n = (spaces n) :: "\n" ::  rstrs 
      member x.AddTag z (_,_,_) = z
      member x.Finish rstrs = String.Join("",Array.ofList (List.rev rstrs)) }

type NoState = NoState
type NoResult = NoResult

/// channel LayoutRenderer
let channelR (chan:TextWriter) =
  { new LayoutRenderer<NoResult,NoState> with 
      member r.Start () = NoState
      member r.AddText z s = chan.Write s; z
      member r.AddBreak z n = chan.WriteLine(); chan.Write (spaces n); z
      member r.AddTag z (tag,attrs,start) =  z
      member r.Finish z = NoResult }

/// buffer render
let bufferR os =
  { new LayoutRenderer<NoResult,NoState> with 
      member r.Start () = NoState
      member r.AddText z s = bprintf os "%s" s; z
      member r.AddBreak z n = bprintf os "\n"; bprintf os "%s" (spaces n); z
      member r.AddTag z (tag,attrs,start) = z
      member r.Finish z = NoResult }

//--------------------------------------------------------------------------
//INDEX: showL, outL are most common
//--------------------------------------------------------------------------

let showL                   layout = renderL stringR         layout
let outL (chan:TextWriter)  layout = renderL (channelR chan) layout |> ignore
let bufferL os              layout = renderL (bufferR os)    layout |> ignore
