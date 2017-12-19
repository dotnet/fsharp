// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.CheckFormatStrings

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.TcGlobals
open Microsoft.FSharp.Compiler.ConstraintSolver

type FormatItem = Simple of TType | FuncAndVal 

let copyAndFixupFormatTypar m tp = 
    let _,_,tinst = FreshenAndFixupTypars m TyparRigidity.Flexible [] [] [tp]
    List.head tinst

let lowestDefaultPriority = 0 (* See comment on TyparConstraint.DefaultsTo *)

let mkFlexibleFormatTypar m tys dflt = 
    let tp = NewTypar (TyparKind.Type,TyparRigidity.Rigid,Typar(mkSynId m "fmt",HeadTypeStaticReq,true),false,TyparDynamicReq.Yes,[],false,false)
    tp.FixupConstraints [ TyparConstraint.SimpleChoice (tys,m); TyparConstraint.DefaultsTo (lowestDefaultPriority,dflt,m)]
    copyAndFixupFormatTypar m tp

let mkFlexibleIntFormatTypar (g: TcGlobals) m = 
    mkFlexibleFormatTypar m [ g.byte_ty; g.int16_ty; g.int32_ty; g.int64_ty;  g.sbyte_ty; g.uint16_ty; g.uint32_ty; g.uint64_ty;g.nativeint_ty;g.unativeint_ty; ] g.int_ty

let mkFlexibleDecimalFormatTypar (g: TcGlobals) m =
    mkFlexibleFormatTypar m [ g.decimal_ty ] g.decimal_ty
    
let mkFlexibleFloatFormatTypar (g: TcGlobals) m = 
    mkFlexibleFormatTypar m [ g.float_ty; g.float32_ty; g.decimal_ty ] g.float_ty

let isDigit c = ('0' <= c && c <= '9')

type FormatInfoRegister = 
  { mutable leftJustify    : bool 
    mutable numPrefixIfPos : char option
    mutable addZeros       : bool
    mutable precision      : bool}

let newInfo ()= 
  { leftJustify    = false
    numPrefixIfPos = None
    addZeros       = false
    precision      = false}

let parseFormatStringInternal (m:range) (g: TcGlobals) (source: string option) fmt bty cty = 
    // Offset is used to adjust ranges depending on whether input string is regular, verbatim or triple-quote.
    // We construct a new 'fmt' string since the current 'fmt' string doesn't distinguish between "\n" and escaped "\\n".
    let (offset, fmt) = 
        match source with
        | Some source ->
            let source = source.Replace("\r\n", "\n").Replace("\r", "\n")
            let positions =
                source.Split('\n')
                |> Seq.map (fun s -> String.length s + 1)
                |> Seq.scan (+) 0
                |> Seq.toArray
            let length = source.Length
            if m.EndLine < positions.Length then
                let startIndex = positions.[m.StartLine-1] + m.StartColumn
                let endIndex = positions.[m.EndLine-1] + m.EndColumn - 1
                if startIndex < length-3 && source.[startIndex..startIndex+2] = "\"\"\"" then
                    (3, source.[startIndex+3..endIndex-3])
                elif startIndex < length-2 && source.[startIndex..startIndex+1] = "@\"" then
                    (2, source.[startIndex+2..endIndex-1])
                else (1, source.[startIndex+1..endIndex-1])
            else (1, fmt)
        | None -> (1, fmt)

    let len = String.length fmt

    let specifierLocations = ResizeArray()

    let rec parseLoop acc (i, relLine, relCol) = 
       if i >= len then
           let argtys =
               if acc |> List.forall (fun (p, _) -> p = None) then // without positional specifiers
                   acc |> List.map snd |> List.rev
               else  
                   failwithf "%s" <| FSComp.SR.forPositionalSpecifiersNotPermitted()
           argtys
       elif System.Char.IsSurrogatePair(fmt,i) then 
          parseLoop acc (i+2, relLine, relCol+2)
       else 
          let c = fmt.[i]
          match c with
          | '%' ->
              let startCol = relCol
              let relCol = relCol+1
              let i = i+1 
              if i >= len then failwithf "%s" <| FSComp.SR.forMissingFormatSpecifier()
              let info = newInfo()

              let rec flags i =
                if i >= len then failwithf "%s" <| FSComp.SR.forMissingFormatSpecifier()
                match fmt.[i] with
                | '-' -> 
                    if info.leftJustify then failwithf "%s" <| FSComp.SR.forFlagSetTwice("-")
                    info.leftJustify <- true
                    flags(i+1)
                | '+' -> 
                    if info.numPrefixIfPos <> None then failwithf "%s" <| FSComp.SR.forPrefixFlagSpacePlusSetTwice()
                    info.numPrefixIfPos <- Some '+'
                    flags(i+1)
                | '0' -> 
                    if info.addZeros then failwithf "%s" <| FSComp.SR.forFlagSetTwice("0")
                    info.addZeros <- true
                    flags(i+1)
                | ' ' -> 
                    if info.numPrefixIfPos <> None then failwithf "%s" <| FSComp.SR.forPrefixFlagSpacePlusSetTwice()
                    info.numPrefixIfPos <- Some ' '
                    flags(i+1)
                | '#' -> failwithf "%s" <| FSComp.SR.forHashSpecifierIsInvalid() 
                | _ -> i

              let rec digitsPrecision i = 
                if i >= len then failwithf "%s" <| FSComp.SR.forBadPrecision()
                match fmt.[i] with
                | c when isDigit c -> digitsPrecision (i+1)
                | _ -> i 

              let precision i = 
                if i >= len then failwithf "%s" <| FSComp.SR.forBadWidth()
                match fmt.[i] with
                | c when isDigit c -> info.precision <- true; false,digitsPrecision (i+1)
                | '*' -> info.precision <- true; true,(i+1)
                | _ -> failwithf "%s" <| FSComp.SR.forPrecisionMissingAfterDot()

              let optionalDotAndPrecision i = 
                if i >= len then failwithf "%s" <| FSComp.SR.forBadPrecision()
                match fmt.[i] with
                | '.' -> precision (i+1)
                | _ -> false,i

              let rec digitsWidthAndPrecision i = 
                if i >= len then failwithf "%s" <| FSComp.SR.forBadPrecision()
                match fmt.[i] with
                | c when isDigit c -> digitsWidthAndPrecision (i+1)
                | _ -> optionalDotAndPrecision i

              let widthAndPrecision i = 
                if i >= len then failwithf "%s" <| FSComp.SR.forBadPrecision()
                match fmt.[i] with
                | c when isDigit c -> false,digitsWidthAndPrecision i
                | '*' -> true,optionalDotAndPrecision (i+1)
                | _ -> false,optionalDotAndPrecision i

              let rec digitsPosition n i =
                  if i >= len then failwithf "%s" <| FSComp.SR.forBadPrecision()
                  match fmt.[i] with
                  | c when isDigit c -> digitsPosition (n*10 + int c - int '0') (i+1)
                  | '$' -> Some n, i+1
                  | _ -> None, i

              let position i =
                  match fmt.[i] with
                  | c when c >= '1' && c <= '9' ->
                      let p, i' = digitsPosition (int c - int '0') (i+1)
                      if p = None then None, i else p, i'
                  | _ -> None, i
              
              let oldI = i
              let posi, i = position i
              let relCol = relCol + i - oldI

              let oldI = i
              let i = flags i 
              let relCol = relCol + i - oldI

              let oldI = i
              let widthArg,(precisionArg,i) = widthAndPrecision i 
              let relCol = relCol + i - oldI

              if i >= len then failwithf "%s" <| FSComp.SR.forBadPrecision()

              let acc = if precisionArg then (Option.map ((+)1) posi, g.int_ty) :: acc else acc 

              let acc = if widthArg then (Option.map ((+)1) posi, g.int_ty) :: acc else acc 

              let checkNoPrecision     c = if info.precision then failwithf "%s" <| FSComp.SR.forFormatDoesntSupportPrecision(c.ToString())
              let checkNoZeroFlag      c = if info.addZeros then failwithf "%s" <| FSComp.SR.forDoesNotSupportZeroFlag(c.ToString())
              let checkNoNumericPrefix c = if info.numPrefixIfPos <> None then
                                              failwithf "%s" <| FSComp.SR.forDoesNotSupportPrefixFlag(c.ToString(), (Option.get info.numPrefixIfPos).ToString())

              let checkOtherFlags c = 
                  checkNoPrecision c 
                  checkNoZeroFlag c 
                  checkNoNumericPrefix c

              let collectSpecifierLocation relLine relCol numStdArgs = 
                  let numArgsForSpecifier =
                    numStdArgs + (if widthArg then 1 else 0) + (if precisionArg then 1 else 0)
                  match relLine with
                  | 0 ->
                      specifierLocations.Add(
                        (Range.mkFileIndexRange m.FileIndex 
                                (Range.mkPos m.StartLine (startCol + offset)) 
                                (Range.mkPos m.StartLine (relCol + offset + 1))), numArgsForSpecifier)
                  | _ ->
                      specifierLocations.Add(
                        (Range.mkFileIndexRange m.FileIndex 
                                (Range.mkPos (m.StartLine + relLine) startCol) 
                                (Range.mkPos (m.StartLine + relLine) (relCol + 1))), numArgsForSpecifier)

              let ch = fmt.[i]
              match ch with
              | '%' ->
                  collectSpecifierLocation relLine relCol 0
                  parseLoop acc (i+1, relLine, relCol+1) 

              | ('d' | 'i' | 'o' | 'u' | 'x' | 'X') ->
                  if info.precision then failwithf "%s" <| FSComp.SR.forFormatDoesntSupportPrecision(ch.ToString())
                  collectSpecifierLocation relLine relCol 1
                  parseLoop ((posi, mkFlexibleIntFormatTypar g m) :: acc) (i+1, relLine, relCol+1)

              | ('l' | 'L') ->
                  if info.precision then failwithf "%s" <| FSComp.SR.forFormatDoesntSupportPrecision(ch.ToString())
                  let relCol = relCol+1
                  let i = i+1
                  
                  // "bad format specifier ... In F# code you can use %d, %x, %o or %u instead ..."
                  if i >= len then 
                      failwithf "%s" <| FSComp.SR.forBadFormatSpecifier()
                  // Always error for %l and %Lx
                  failwithf "%s" <| FSComp.SR.forLIsUnnecessary()
                  match fmt.[i] with
                  | ('d' | 'i' | 'o' | 'u' | 'x' | 'X') -> 
                      collectSpecifierLocation relLine relCol 1
                      parseLoop ((posi, mkFlexibleIntFormatTypar g m) :: acc)  (i+1, relLine, relCol+1)
                  | _ -> failwithf "%s" <| FSComp.SR.forBadFormatSpecifier()

              | ('h' | 'H') ->
                  failwithf "%s" <| FSComp.SR.forHIsUnnecessary()

              | 'M' ->
                  collectSpecifierLocation relLine relCol 1
                  parseLoop ((posi, mkFlexibleDecimalFormatTypar g m) :: acc) (i+1, relLine, relCol+1)

              | ('f' | 'F' | 'e' | 'E' | 'g' | 'G') ->
                  collectSpecifierLocation relLine relCol 1
                  parseLoop ((posi, mkFlexibleFloatFormatTypar g m) :: acc) (i+1, relLine, relCol+1)

              | 'b' ->
                  checkOtherFlags ch
                  collectSpecifierLocation relLine relCol 1
                  parseLoop ((posi, g.bool_ty)  :: acc) (i+1, relLine, relCol+1)

              | 'c' ->
                  checkOtherFlags ch
                  collectSpecifierLocation relLine relCol 1
                  parseLoop ((posi, g.char_ty)  :: acc) (i+1, relLine, relCol+1)

              | 's' ->
                  checkOtherFlags ch
                  collectSpecifierLocation relLine relCol 1
                  parseLoop ((posi, g.string_ty)  :: acc) (i+1, relLine, relCol+1)

              | 'O' ->
                  checkOtherFlags ch
                  collectSpecifierLocation relLine relCol 1
                  parseLoop ((posi, NewInferenceType ()) :: acc) (i+1, relLine, relCol+1)

              | 'A' ->
                  match info.numPrefixIfPos with
                  | None     // %A has BindingFlags=Public, %+A has BindingFlags=Public | NonPublic
                  | Some '+' -> 
                      collectSpecifierLocation relLine relCol 1
                      parseLoop ((posi, NewInferenceType ()) :: acc)  (i+1, relLine, relCol+1)
                  | Some _   -> failwithf "%s" <| FSComp.SR.forDoesNotSupportPrefixFlag(ch.ToString(), (Option.get info.numPrefixIfPos).ToString())

              | 'a' ->
                  checkOtherFlags ch
                  let xty = NewInferenceType () 
                  let fty = bty --> (xty --> cty)
                  collectSpecifierLocation relLine relCol 2
                  parseLoop ((Option.map ((+)1) posi, xty) ::  (posi, fty) :: acc) (i+1, relLine, relCol+1)

              | 't' ->
                  checkOtherFlags ch
                  collectSpecifierLocation relLine relCol 1
                  parseLoop ((posi, bty --> cty) :: acc)  (i+1, relLine, relCol+1)

              | c -> failwithf "%s" <| FSComp.SR.forBadFormatSpecifierGeneral(String.make 1 c) 
          
          | '\n' -> parseLoop acc (i+1, relLine+1, 0)   
          | _ -> parseLoop acc (i+1, relLine, relCol+1)
           
    let results = parseLoop [] (0, 0, m.StartColumn)
    results, Seq.toList specifierLocations

let ParseFormatString m g source fmt bty cty dty = 
    let argtys, specifierLocations = parseFormatStringInternal m g source fmt bty cty
    let aty = List.foldBack (-->) argtys dty
    let ety = mkRefTupledTy g argtys
    (aty, ety), specifierLocations 

let TryCountFormatStringArguments m g fmt bty cty =
    try
        let argtys, _specifierLocations = parseFormatStringInternal m g None fmt bty cty
        Some argtys.Length
    with _ ->
        None

