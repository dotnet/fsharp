// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Convert quoted TAST data structures to structures ready for pickling 

module internal Microsoft.FSharp.Compiler.QuotationTranslator

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Import
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.TcGlobals
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.AbstractIL.IL

[<Sealed>]
type QuotationTranslationEnv =
   static member Empty : QuotationTranslationEnv
   member BindTypars : Typars -> QuotationTranslationEnv

exception InvalidQuotedTerm of exn
exception IgnoringPartOfQuotedTermWarning of string * Range.range

[<RequireQualifiedAccess>]
type IsReflectedDefinition =
    | Yes
    | No

[<RequireQualifiedAccess>]
type QuotationSerializationFormat =
    /// Indicates that type references are emitted as integer indexes into a supplied table
    | FSharp_40_Plus
    | FSharp_20_Plus

[<Sealed>]
type QuotationGenerationScope  =
    static member Create: TcGlobals * ImportMap * CcuThunk * IsReflectedDefinition -> QuotationGenerationScope
    member Close: unit -> ILTypeRef list * (TType * range) list * (Expr * range) list 
    static member ComputeQuotationFormat : TcGlobals -> QuotationSerializationFormat

val ConvExprPublic : QuotationGenerationScope -> QuotationTranslationEnv -> Expr -> QuotationPickler.ExprData 
val ConvMethodBase  : QuotationGenerationScope -> QuotationTranslationEnv ->  string * Val  -> QuotationPickler.MethodBaseData


val (|ModuleValueOrMemberUse|_|) : TcGlobals -> Expr -> (ValRef * ValUseFlag * Expr * TType * TypeInst * Expr list) option
val (|SimpleArrayLoopUpperBound|_|) : Expr -> unit option
val (|SimpleArrayLoopBody|_|) : TcGlobals -> Expr -> (Expr * TType * Expr) option
val (|ObjectInitializationCheck|_|) : TcGlobals -> Expr -> unit option
val isSplice : TcGlobals -> ValRef -> bool

