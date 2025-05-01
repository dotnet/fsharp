// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Convert quoted TAST data structures to structures ready for pickling
module internal FSharp.Compiler.QuotationTranslator

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Import
open FSharp.Compiler.Text
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

exception InvalidQuotedTerm of exn
exception IgnoringPartOfQuotedTermWarning of string * range

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
type QuotationGenerationScope =
    static member Create:
        TcGlobals * ImportMap * CcuThunk * ConstraintSolver.TcValF * IsReflectedDefinition -> QuotationGenerationScope

    member Close: unit -> ILTypeRef list * (TType * range) list * (Expr * range) list
    static member ComputeQuotationFormat: TcGlobals -> QuotationSerializationFormat

val ConvExprPublic: QuotationGenerationScope -> suppressWitnesses: bool -> Expr -> QuotationPickler.ExprData

val ConvReflectedDefinition:
    QuotationGenerationScope -> string -> Val -> Expr -> QuotationPickler.MethodBaseData * QuotationPickler.ExprData

[<return: Struct>]
val (|ModuleValueOrMemberUse|_|):
    TcGlobals -> Expr -> (ValRef * ValUseFlag * Expr * TType * TypeInst * Expr list) voption

[<return: Struct>]
val (|SimpleArrayLoopUpperBound|_|): Expr -> unit voption

[<return: Struct>]
val (|SimpleArrayLoopBody|_|): TcGlobals -> Expr -> (Expr * TType * Expr) voption

[<return: Struct>]
val (|ObjectInitializationCheck|_|): TcGlobals -> Expr -> unit voption
