// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Detuple 

open Internal.Utilities.Collections
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.TcGlobals

val DetupleImplFile : CcuThunk -> TcGlobals -> TypedImplFile -> TypedImplFile

module GlobalUsageAnalysis = 
    val GetValsBoundInExpr : Expr -> zset<Val,ValOrder>

    type accessor 

    /// Results is "expr information".
    /// This could extend to be a full graph view of the expr.
    /// Later could support "safe" change operations, and optimisations could be in terms of those.
    type Results =
       { /// v -> context / APP inst args 
         Uses   : zmap<Val, ValOrder, (accessor list * TType list * Expr list) list>
         /// v -> binding repr 
         Defns   : zmap<Val, ValOrder, Expr>
         /// bound in a decision tree? 
         DecisionTreeBindings    : zset<Val, ValOrder>
         /// v -> recursive? * v list   -- the others in the mutual binding 
         RecursiveBindings  : zmap<Val, ValOrder, (bool * Vals)>
         /// val not defined under lambdas 
         TopLevelBindings : zset<Val, ValOrder>
         /// top of expr toplevel? (true) 
         IterationIsAtTopLevel      : bool;                                                         
       }
    val GetUsageInfoOfImplFile :  TcGlobals -> TypedImplFile -> Results
