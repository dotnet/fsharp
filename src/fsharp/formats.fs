// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Formats

open Internal.Utilities
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Env
open Microsoft.FSharp.Compiler.ConstraintSolver

type FormatItem = Simple of TType | FuncAndVal 

let copyAndFixupFormatTypar m tp = 
    let _,_,tinst = FreshenAndFixupTypars m TyparRigidity.Flexible [] [] [tp]
    List.head tinst

let lowestDefaultPriority = 0 (* See comment on TyparConstraint.DefaultsTo *)

let mkFlexibleFormatTypar m tys dflt = 
    let tp = NewTypar (TyparKind.Type,TyparRigidity.Rigid,Typar(mkSynId m "fmt",HeadTypeStaticReq,true),false,TyparDynamicReq.Yes,[],false,false)
    tp.FixupConstraints [ TyparConstraint.SimpleChoice (tys,m); TyparConstraint.DefaultsTo (lowestDefaultPriority,dflt,m)];
    copyAndFixupFormatTypar m tp

let mkFlexibleIntFormatTypar g m = 
    mkFlexibleFormatTypar m [ g.byte_ty; g.int16_ty; g.int32_ty; g.int64_ty;  g.sbyte_ty; g.uint16_ty; g.uint32_ty; g.uint64_ty;g.nativeint_ty;g.unativeint_ty; ] g.int_ty
    
    
let mkFlexibleFloatFormatTypar g m = 
    mkFlexibleFormatTypar m [ g.float_ty; g.float32_ty; g.decimal_ty ] g.float_ty

let isDigit c = ('0' <= c && c <= '9')

type FormatInfoRegister = 
  { mutable leftJustify    : bool; 
    mutable numPrefixIfPos : char option;
    mutable addZeros       : bool;
    mutable precision      : bool}

let newInfo ()= 
  { leftJustify    = false;
    numPrefixIfPos = None;
    addZeros       = false;
    precision      = false}

let ParseFormatString m g fmt bty cty dty = 
    let len = String.length fmt

    let rec parseLoop acc i = 
       if i >= len then
           let argtys =
               if acc |> List.forall (fun (p, _) -> p = None) then // without positional specifiers
                   acc |> List.map snd |> List.rev
               else  
                   failwithf "%s" <| FSComp.SR.forPositionalSpecifiersNotPermitted()
          
           let aty = List.foldBack (-->) argtys dty
           let ety = mkTupledTy g argtys
           aty,ety
       elif System.Char.IsSurrogatePair(fmt,i) then 
          parseLoop acc (i+2)
       else 
          let c = fmt.[i]
          match c with
          | '%' ->
              let i = i+1 
              if i >= len then failwithf "%s" <| FSComp.SR.forMissingFormatSpecifier()
              let info = newInfo()

              let rec flags i =
                if i >= len then failwithf "%s" <| FSComp.SR.forMissingFormatSpecifier()
                match fmt.[i] with
                | '-' -> 
                    if info.leftJustify then failwithf "%s" <| FSComp.SR.forFlagSetTwice("-");
                    info.leftJustify <- true;
                    flags(i+1)
                | '+' -> 
                    if info.numPrefixIfPos <> None then failwithf "%s" <| FSComp.SR.forPrefixFlagSpacePlusSetTwice()
                    info.numPrefixIfPos <- Some '+';
                    flags(i+1)
                | '0' -> 
                    if info.addZeros then failwithf "%s" <| FSComp.SR.forFlagSetTwice("0");
                    info.addZeros <- true;
                    flags(i+1)
                | ' ' -> 
                    if info.numPrefixIfPos <> None then failwithf "%s" <| FSComp.SR.forPrefixFlagSpacePlusSetTwice();
                    info.numPrefixIfPos <- Some ' ';
                    flags(i+1)
                | '#' -> failwithf "%s" <| FSComp.SR.forHashSpecifierIsInvalid(); 
                | _ -> i

              let rec digitsPrecision i = 
                if i >= len then failwithf "%s" <| FSComp.SR.forBadPrecision();
                match fmt.[i] with
                | c when isDigit c -> digitsPrecision (i+1)
                | _ -> i 

              let precision i = 
                if i >= len then failwithf "%s" <| FSComp.SR.forBadWidth();
                match fmt.[i] with
                | c when isDigit c -> info.precision <- true; false,digitsPrecision (i+1)
                | '*' -> info.precision <- true; true,(i+1)
                | _ -> failwithf "%s" <| FSComp.SR.forPrecisionMissingAfterDot()

              let optionalDotAndPrecision i = 
                if i >= len then failwithf "%s" <| FSComp.SR.forBadPrecision();
                match fmt.[i] with
                | '.' -> precision (i+1)
                | _ -> false,i

              let rec digitsWidthAndPrecision i = 
                if i >= len then failwithf "%s" <| FSComp.SR.forBadPrecision();
                match fmt.[i] with
                | c when isDigit c -> digitsWidthAndPrecision (i+1)
                | _ -> optionalDotAndPrecision i

              let widthAndPrecision i = 
                if i >= len then failwithf "%s" <| FSComp.SR.forBadPrecision();
                match fmt.[i] with
                | c when isDigit c -> false,digitsWidthAndPrecision i
                | '*' -> true,optionalDotAndPrecision (i+1)
                | _ -> false,optionalDotAndPrecision i

              let rec digitsPosition n i =
                  if i >= len then failwithf "%s" <| FSComp.SR.forBadPrecision();
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

              let posi, i = position i

              let i = flags i 

              let widthArg,(precisionArg,i) = widthAndPrecision i 

              if i >= len then failwithf "%s" <| FSComp.SR.forBadPrecision();

              let acc = if precisionArg then (Option.map ((+)1) posi, g.int_ty) :: acc else acc 

              let acc = if widthArg then (Option.map ((+)1) posi, g.int_ty) :: acc else acc 

              let checkNoPrecision     c = if info.precision then failwithf "%s" <| FSComp.SR.forFormatDoesntSupportPrecision(c.ToString())
              let checkNoZeroFlag      c = if info.addZeros then failwithf "%s" <| FSComp.SR.forDoesNotSupportZeroFlag(c.ToString())
              let checkNoNumericPrefix c = if info.numPrefixIfPos <> None then
                                              failwithf "%s" <| FSComp.SR.forDoesNotSupportPrefixFlag(c.ToString(), (Option.get info.numPrefixIfPos).ToString())

              let checkOtherFlags c = 
                  checkNoPrecision c; 
                  checkNoZeroFlag c; 
                  checkNoNumericPrefix c

              let ch = fmt.[i]
              match ch with
              | '%' -> parseLoop acc (i+1) 

              | ('d' | 'i' | 'o' | 'u' | 'x' | 'X') ->
                  if info.precision then failwithf "%s" <| FSComp.SR.forFormatDoesntSupportPrecision(ch.ToString());
                  parseLoop ((posi, mkFlexibleIntFormatTypar g m) :: acc) (i+1)

              | ('l' | 'L') ->
                  if info.precision then failwithf "%s" <| FSComp.SR.forFormatDoesntSupportPrecision(ch.ToString());
                  let i = i+1
                  
                  // "bad format specifier ... In F# code you can use %d, %x, %o or %u instead ..."
                  if i >= len then 
                      failwithf "%s" <| FSComp.SR.forBadFormatSpecifier()
                  // Always error for %l and %Lx
                  failwithf "%s" <| FSComp.SR.forLIsUnnecessary()
                  match fmt.[i] with
                  | ('d' | 'i' | 'o' | 'u' | 'x' | 'X') -> 
                      parseLoop ((posi, mkFlexibleIntFormatTypar g m) :: acc)  (i+1)
                  | _ -> failwithf "%s" <| FSComp.SR.forBadFormatSpecifier()

              | ('h' | 'H') ->
                  failwithf "%s" <| FSComp.SR.forHIsUnnecessary()

              | 'M' -> 
                  parseLoop ((posi, g.decimal_ty) :: acc) (i+1)

              | ('f' | 'F' | 'e' | 'E' | 'g' | 'G') ->  
                  parseLoop ((posi, mkFlexibleFloatFormatTypar g m) :: acc) (i+1)

              | 'b' ->
                  checkOtherFlags ch;
                  parseLoop ((posi, g.bool_ty)  :: acc) (i+1)

              | 'c' ->
                  checkOtherFlags ch;
                  parseLoop ((posi, g.char_ty)  :: acc) (i+1)

              | 's' ->
                  checkOtherFlags ch;
                  parseLoop ((posi, g.string_ty)  :: acc) (i+1)

              | 'O' ->
                  checkOtherFlags ch;
                  parseLoop ((posi, NewInferenceType ()) :: acc)  (i+1)

              | 'A' ->
                  match info.numPrefixIfPos with
                  | None     // %A has BindingFlags=Public, %+A has BindingFlags=Public | NonPublic
                  | Some '+' -> parseLoop ((posi, NewInferenceType ()) :: acc)  (i+1)
                  | Some _   -> failwithf "%s" <| FSComp.SR.forDoesNotSupportPrefixFlag(ch.ToString(), (Option.get info.numPrefixIfPos).ToString())

              | 'a' ->
                  checkOtherFlags ch;
                  let xty = NewInferenceType () 
                  let fty = bty --> (xty --> cty)
                  parseLoop ((Option.map ((+)1) posi, xty) ::  (posi, fty) :: acc) (i+1)

              | 't' ->
                  checkOtherFlags ch;
                  parseLoop ((posi, bty --> cty) :: acc)  (i+1)

              | c -> failwithf "%s" <| FSComp.SR.forBadFormatSpecifierGeneral(String.make 1 c) 
              
          | _ -> parseLoop acc (i+1) 
    parseLoop [] 0

