// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Detuple 

open FSharp.Compiler.AbstractIL.Internal 
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

val DetupleImplFile: CcuThunk -> TcGlobals -> TypedImplFile -> TypedImplFile

module GlobalUsageAnalysis = 
    val GetValsBoundInExpr: Expr -> Zset<Val>

    type accessor 

    /// Results is "expr information".
    /// This could extend to be a full graph view of the expr.
    /// Later could support "safe" change operations, and optimisations could be in terms of those.
    type Results =
       {
         /// v -> context / APP inst args 
         Uses: Zmap<Val,(accessor list * TType list * Expr list) list>

         /// v -> binding repr 
         Defns: Zmap<Val,Expr>

         /// bound in a decision tree? 
         DecisionTreeBindings: Zset<Val>

         /// v -> recursive? * v list   -- the others in the mutual binding 
         RecursiveBindings: Zmap<Val,(bool * Vals)>

         /// val not defined under lambdas 
         TopLevelBindings: Zset<Val>

         /// top of expr toplevel? (true) 
         IterationIsAtTopLevel: bool
       }

    val GetUsageInfoOfImplFile: TcGlobals -> TypedImplFile -> Results
