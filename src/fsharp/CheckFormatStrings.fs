// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckFormatStrings

open System.Text
open FSharp.Compiler 
open FSharp.Compiler.AbstractIL.Internal.Library 
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Range
open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TcGlobals

type FormatItem = Simple of TType | FuncAndVal 

let copyAndFixupFormatTypar m tp = 
    let _,_,tinst = FreshenAndFixupTypars m TyparRigidity.Flexible [] [] [tp]
    List.head tinst

let lowestDefaultPriority = 0 (* See comment on TyparConstraint.DefaultsTo *)

let mkFlexibleFormatTypar m tys dflt = 
    let tp = Construct.NewTypar (TyparKind.Type,TyparRigidity.Rigid,Typar(mkSynId m "fmt",HeadTypeStaticReq,true),false,TyparDynamicReq.Yes,[],false,false)
    tp.SetConstraints [ TyparConstraint.SimpleChoice (tys,m); TyparConstraint.DefaultsTo (lowestDefaultPriority,dflt,m)]
    copyAndFixupFormatTypar m tp

let mkFlexibleIntFormatTypar (g: TcGlobals) m = 
    mkFlexibleFormatTypar m [ g.byte_ty; g.int16_ty; g.int32_ty; g.int64_ty;  g.sbyte_ty; g.uint16_ty; g.uint32_ty; g.uint64_ty;g.nativeint_ty;g.unativeint_ty; ] g.int_ty

let mkFlexibleDecimalFormatTypar (g: TcGlobals) m =
    mkFlexibleFormatTypar m [ g.decimal_ty ] g.decimal_ty
    
let mkFlexibleFloatFormatTypar (g: TcGlobals) m = 
    mkFlexibleFormatTypar m [ g.float_ty; g.float32_ty; g.decimal_ty ] g.float_ty

type FormatInfoRegister = 
  { mutable leftJustify    : bool 
    mutable numPrefixIfPos : char option
    mutable addZeros       : bool
    mutable precision      : bool}

let newInfo () = 
  { leftJustify    = false
    numPrefixIfPos = None
    addZeros       = false
    precision      = false}

let parseFormatStringInternal (m:range) (g: TcGlobals) isInterpolated isFormattableString (context: FormatStringCheckContext option) fmt printerArgTy printerResidueTy = 
    // Offset is used to adjust ranges depending on whether input string is regular, verbatim or triple-quote.
    // We construct a new 'fmt' string since the current 'fmt' string doesn't distinguish between "\n" and escaped "\\n".
    let (offset, fmt) = 
        match context with
        | Some context ->
            let sourceText = context.SourceText
            let lineStartPositions = context.LineStartPositions
            let length = sourceText.Length
            if m.EndLine < lineStartPositions.Length then
                let startIndex = lineStartPositions.[m.StartLine-1] + m.StartColumn
                let endIndex = lineStartPositions.[m.EndLine-1] + m.EndColumn - 1
                if startIndex < length-3 && sourceText.SubTextEquals("\"\"\"", startIndex) then
                    (3, sourceText.GetSubTextString(startIndex + 3, endIndex - startIndex))
                elif startIndex < length-2 && sourceText.SubTextEquals("@\"", startIndex) then
                    (2, sourceText.GetSubTextString(startIndex + 2, endIndex + 1 - startIndex))
                else (1, sourceText.GetSubTextString(startIndex + 1, endIndex - startIndex))
            else (1, fmt)
        | None -> (1, fmt)

    let len = String.length fmt

    let specifierLocations = ResizeArray()

    // For FormattableString we collect a .NET Format String with {0} etc. replacing text.  '%%' are replaced
    // by '%', we check there are no '%' formats, and '{{' and '}}' are *not* replaced since the subsequent
    // call to String.Format etc. will process them.
    let dotnetFormatString = StringBuilder() 
    let appendToDotnetFormatString (s: string) = dotnetFormatString.Append(s) |> ignore 
    let mutable dotnetFormatStringInterpolationHoleCount = 0
    let percentATys = ResizeArray<_>()

    let rec parseLoop acc (i, relLine, relCol) = 
       if i >= len then
           let argtys =
               if acc |> List.forall (fun (p, _) -> p = None) then // without positional specifiers
                   acc |> List.map snd |> List.rev
               else  
                   raise (Failure (FSComp.SR.forPositionalSpecifiersNotPermitted()))
           argtys
       elif System.Char.IsSurrogatePair(fmt,i) then 
          appendToDotnetFormatString (fmt.[i..i+1])
          parseLoop acc (i+2, relLine, relCol+2)
       else 
          let c = fmt.[i]
          match c with
          | '%' ->
              let startCol = relCol
              let relCol = relCol+1
              let i = i+1 
              if i >= len then raise (Failure (FSComp.SR.forMissingFormatSpecifier()))
              let info = newInfo()

              let rec flags i =
                if i >= len then raise (Failure (FSComp.SR.forMissingFormatSpecifier()))
                match fmt.[i] with
                | '-' -> 
                    if info.leftJustify then raise (Failure (FSComp.SR.forFlagSetTwice("-")))
                    info.leftJustify <- true
                    flags(i+1)
                | '+' -> 
                    if info.numPrefixIfPos <> None then raise (Failure (FSComp.SR.forPrefixFlagSpacePlusSetTwice()))
                    info.numPrefixIfPos <- Some '+'
                    flags(i+1)
                | '0' -> 
                    if info.addZeros then raise (Failure (FSComp.SR.forFlagSetTwice("0")))
                    info.addZeros <- true
                    flags(i+1)
                | ' ' -> 
                    if info.numPrefixIfPos <> None then raise (Failure (FSComp.SR.forPrefixFlagSpacePlusSetTwice()))
                    info.numPrefixIfPos <- Some ' '
                    flags(i+1)
                | '#' -> raise (Failure (FSComp.SR.forHashSpecifierIsInvalid() ))
                | _ -> i

              let rec digitsPrecision i = 
                if i >= len then raise (Failure (FSComp.SR.forBadPrecision()))
                match fmt.[i] with
                | c when System.Char.IsDigit c -> digitsPrecision (i+1)
                | _ -> i 

              let precision i = 
                if i >= len then raise (Failure (FSComp.SR.forBadWidth()))
                match fmt.[i] with
                | c when System.Char.IsDigit c -> info.precision <- true; false,digitsPrecision (i+1)
                | '*' -> info.precision <- true; true,(i+1)
                | _ -> raise (Failure (FSComp.SR.forPrecisionMissingAfterDot()))

              let optionalDotAndPrecision i = 
                if i >= len then raise (Failure (FSComp.SR.forBadPrecision()))
                match fmt.[i] with
                | '.' -> precision (i+1)
                | _ -> false,i

              let rec digitsWidthAndPrecision n i = 
                if i >= len then raise (Failure (FSComp.SR.forBadPrecision()))
                match fmt.[i] with
                | c when System.Char.IsDigit c -> digitsWidthAndPrecision (n*10 + int c - int '0') (i+1)
                | _ -> Some n, optionalDotAndPrecision i

              let widthAndPrecision i = 
                if i >= len then raise (Failure (FSComp.SR.forBadPrecision()))
                match fmt.[i] with
                | c when System.Char.IsDigit c -> false,digitsWidthAndPrecision 0 i
                | '*' -> true, (None, optionalDotAndPrecision (i+1))
                | _ -> false, (None, optionalDotAndPrecision i)

              let rec digitsPosition n i =
                  if i >= len then raise (Failure (FSComp.SR.forBadPrecision()))
                  match fmt.[i] with
                  | c when System.Char.IsDigit c -> digitsPosition (n*10 + int c - int '0') (i+1)
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
              let widthArg,(widthValue, (precisionArg,i)) = widthAndPrecision i 
              let relCol = relCol + i - oldI

              if i >= len then raise (Failure (FSComp.SR.forBadPrecision()))

              let acc = if precisionArg then (Option.map ((+)1) posi, g.int_ty) :: acc else acc 

              let acc = if widthArg then (Option.map ((+)1) posi, g.int_ty) :: acc else acc 

              let checkNoPrecision c =
                  if info.precision then raise (Failure (FSComp.SR.forFormatDoesntSupportPrecision(c.ToString())))

              let checkNoZeroFlag c =
                  if info.addZeros then raise (Failure (FSComp.SR.forDoesNotSupportZeroFlag(c.ToString())))

              let checkNoNumericPrefix c =
                  match info.numPrefixIfPos with 
                  | Some n -> raise (Failure (FSComp.SR.forDoesNotSupportPrefixFlag(c.ToString(), n.ToString())))
                  | None -> ()

              let checkOtherFlags c = 
                  checkNoPrecision c 
                  checkNoZeroFlag c 
                  checkNoNumericPrefix c

              // Explicitly typed holes in interpolated strings "....%d{x}..." get additional '%P()' as a hole place marker
              let skipPossibleInterpolationHole i =
                  if isInterpolated then 
                      if i+1 < len && fmt.[i] = '%' && fmt.[i+1] = 'P'  then
                          let i = i + 2
                          if i+1 < len && fmt.[i] = '('  && fmt.[i+1] = ')' then 
                              if isFormattableString then 
                                  raise (Failure (FSComp.SR.forFormatInvalidForInterpolated4()))
                              i + 2
                          else 
                              raise (Failure (FSComp.SR.forFormatInvalidForInterpolated2()))
                      else
                          raise (Failure (FSComp.SR.forFormatInvalidForInterpolated()))
                  else i

              // Implicitly typed holes in interpolated strings are translated to '... %P(...)...' in the
              // type checker.  They should always have '(...)' after for format string.  
              let requireAndSkipInterpolationHoleFormat i =
                  if i < len && fmt.[i] = '(' then 
                      let i2 = fmt.IndexOf(")", i+1)
                      if i2 = -1 then 
                          raise (Failure (FSComp.SR.forFormatInvalidForInterpolated3()))
                      else 
                          let dotnetAlignment = match widthValue with None -> "" | Some w -> "," + (if info.leftJustify then "-" else "") + string w
                          let dotnetNumberFormat = match fmt.[i+1..i2-1] with "" -> "" | s -> ":" + s
                          appendToDotnetFormatString ("{" + string dotnetFormatStringInterpolationHoleCount + dotnetAlignment  + dotnetNumberFormat + "}") 
                          dotnetFormatStringInterpolationHoleCount <- dotnetFormatStringInterpolationHoleCount + 1
                          i2+1
                  else
                      raise (Failure (FSComp.SR.forFormatInvalidForInterpolated3()))

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
                  appendToDotnetFormatString "%"
                  parseLoop acc (i+1, relLine, relCol+1) 

              | ('d' | 'i' | 'o' | 'u' | 'x' | 'X') ->
                  if info.precision then raise (Failure (FSComp.SR.forFormatDoesntSupportPrecision(ch.ToString())))
                  collectSpecifierLocation relLine relCol 1
                  let i = skipPossibleInterpolationHole (i+1)
                  parseLoop ((posi, mkFlexibleIntFormatTypar g m) :: acc) (i, relLine, relCol+1)

              | ('l' | 'L') ->
                  if info.precision then raise (Failure (FSComp.SR.forFormatDoesntSupportPrecision(ch.ToString())))
                  let relCol = relCol+1
                  let i = i+1
                  
                  // "bad format specifier ... In F# code you can use %d, %x, %o or %u instead ..."
                  if i >= len then 
                      raise (Failure (FSComp.SR.forBadFormatSpecifier()))
                  // Always error for %l and %Lx
                  raise (Failure (FSComp.SR.forLIsUnnecessary()))
                  match fmt.[i] with
                  | ('d' | 'i' | 'o' | 'u' | 'x' | 'X') -> 
                      collectSpecifierLocation relLine relCol 1
                      let i = skipPossibleInterpolationHole (i+1)
                      parseLoop ((posi, mkFlexibleIntFormatTypar g m) :: acc)  (i, relLine, relCol+1)
                  | _ -> raise (Failure (FSComp.SR.forBadFormatSpecifier()))

              | ('h' | 'H') ->
                  raise (Failure (FSComp.SR.forHIsUnnecessary()))

              | 'M' ->
                  collectSpecifierLocation relLine relCol 1
                  let i = skipPossibleInterpolationHole (i+1)
                  parseLoop ((posi, mkFlexibleDecimalFormatTypar g m) :: acc) (i, relLine, relCol+1)

              | ('f' | 'F' | 'e' | 'E' | 'g' | 'G') ->
                  collectSpecifierLocation relLine relCol 1
                  let i = skipPossibleInterpolationHole (i+1)
                  parseLoop ((posi, mkFlexibleFloatFormatTypar g m) :: acc) (i, relLine, relCol+1)

              | 'b' ->
                  checkOtherFlags ch
                  collectSpecifierLocation relLine relCol 1
                  let i = skipPossibleInterpolationHole (i+1)
                  parseLoop ((posi, g.bool_ty)  :: acc) (i, relLine, relCol+1)

              | 'c' ->
                  checkOtherFlags ch
                  collectSpecifierLocation relLine relCol 1
                  let i = skipPossibleInterpolationHole (i+1)
                  parseLoop ((posi, g.char_ty)  :: acc) (i, relLine, relCol+1)

              | 's' ->
                  checkOtherFlags ch
                  collectSpecifierLocation relLine relCol 1
                  let i = skipPossibleInterpolationHole (i+1)
                  parseLoop ((posi, g.string_ty)  :: acc) (i, relLine, relCol+1)

              | 'O' ->
                  checkOtherFlags ch
                  collectSpecifierLocation relLine relCol 1
                  let i = skipPossibleInterpolationHole (i+1)
                  parseLoop ((posi, NewInferenceType ()) :: acc) (i, relLine, relCol+1)

              // residue of hole "...{n}..." in interpolated strings become %P(...) 
              | 'P' when isInterpolated ->
                  checkOtherFlags ch
                  let i = requireAndSkipInterpolationHoleFormat (i+1)
                  parseLoop ((posi, NewInferenceType ()) :: acc) (i, relLine, relCol+1)

              | 'A' ->
                  match info.numPrefixIfPos with
                  | None     // %A has BindingFlags=Public, %+A has BindingFlags=Public | NonPublic
                  | Some '+' -> 
                      collectSpecifierLocation relLine relCol 1
                      let i = skipPossibleInterpolationHole (i+1)
                      let xty = NewInferenceType ()
                      percentATys.Add(xty)
                      parseLoop ((posi, xty) :: acc)  (i, relLine, relCol+1)
                  | Some n -> raise (Failure (FSComp.SR.forDoesNotSupportPrefixFlag(ch.ToString(), n.ToString())))

              | 'a' ->
                  checkOtherFlags ch
                  let xty = NewInferenceType () 
                  let fty = printerArgTy --> (xty --> printerResidueTy)
                  collectSpecifierLocation relLine relCol 2
                  let i = skipPossibleInterpolationHole (i+1)
                  parseLoop ((Option.map ((+)1) posi, xty) ::  (posi, fty) :: acc) (i, relLine, relCol+1)

              | 't' ->
                  checkOtherFlags ch
                  collectSpecifierLocation relLine relCol 1
                  let i = skipPossibleInterpolationHole (i+1)
                  parseLoop ((posi, printerArgTy --> printerResidueTy) :: acc)  (i, relLine, relCol+1)

              | c -> raise (Failure (FSComp.SR.forBadFormatSpecifierGeneral(String.make 1 c)))
          
          | '\n' ->
              appendToDotnetFormatString fmt.[i..i]
              parseLoop acc (i+1, relLine+1, 0)   
          | _ ->
              appendToDotnetFormatString fmt.[i..i]
              parseLoop acc (i+1, relLine, relCol+1)
           
    let results = parseLoop [] (0, 0, m.StartColumn)
    results, Seq.toList specifierLocations, dotnetFormatString.ToString(), percentATys.ToArray()

let ParseFormatString m g isInterpolated isFormattableString formatStringCheckContext fmt printerArgTy printerResidueTy printerResultTy = 
    let argTys, specifierLocations, dotnetFormatString, percentATys = parseFormatStringInternal m g isInterpolated isFormattableString formatStringCheckContext fmt printerArgTy printerResidueTy
    let printerTy = List.foldBack (-->) argTys printerResultTy
    let printerTupleTy = mkRefTupledTy g argTys
    argTys, printerTy, printerTupleTy, percentATys, specifierLocations, dotnetFormatString

let TryCountFormatStringArguments m g isInterpolated fmt printerArgTy printerResidueTy =
    try
        let argTys, _specifierLocations, _dotnetFormatString, _percentATys = parseFormatStringInternal m g isInterpolated false None fmt printerArgTy printerResidueTy
        Some argTys.Length
    with _ ->
        None

