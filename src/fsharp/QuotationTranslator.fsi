// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Convert quoted TAST data structures to structures ready for pickling 

module internal FSharp.Compiler.QuotationTranslator

open FSharp.Compiler 
open FSharp.Compiler.Range
open FSharp.Compiler.Import
open FSharp.Compiler.Tast
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.IL

[<Sealed>]
type QuotationTranslationEnv =
   static member CreateEmpty : TcGlobals -> QuotationTranslationEnv
   member BindTypars : Typars -> QuotationTranslationEnv
   member BindWitnessInfos : TraitWitnessInfo list -> QuotationTranslationEnv

exception InvalidQuotedTerm of exn
exception IgnoringPartOfQuotedTermWarning of string * Range.range

[<RequireQualifiedAccess>]
type IsReflectedDefinition =
    | Yes
    | No

[<RequireQualifiedAccess>]
type QuotationSerializationFormat =
    { 
      /// Indicates that witness parameters are recorded
      SupportsWitnesses: bool 
      
      /// Indicates that type references are emitted as integer indexes into a supplied table
      SupportsDeserializeEx: bool 
    }

[<Sealed>]
type QuotationGenerationScope  =
    static member Create: TcGlobals * ImportMap * CcuThunk * ConstraintSolver.TcValF * IsReflectedDefinition -> QuotationGenerationScope
    member Close: unit -> ILTypeRef list * (TType * range) list * (Expr * range) list 
    static member ComputeQuotationFormat : TcGlobals -> QuotationSerializationFormat

val ConvExprPublic : QuotationGenerationScope -> QuotationTranslationEnv -> Expr -> QuotationPickler.ExprData 
val ConvMethodBase  : QuotationGenerationScope -> QuotationTranslationEnv ->  string * Val  -> QuotationPickler.MethodBaseData


val (|ModuleValueOrMemberUse|_|) : TcGlobals -> Expr -> (ValRef * ValUseFlag * Expr * TType * TypeInst * Expr list) option
val (|SimpleArrayLoopUpperBound|_|) : Expr -> unit option
val (|SimpleArrayLoopBody|_|) : TcGlobals -> Expr -> (Expr * TType * Expr) option
val (|ObjectInitializationCheck|_|) : TcGlobals -> Expr -> unit option
val isSplice : TcGlobals -> ValRef -> bool

