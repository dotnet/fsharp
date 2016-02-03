// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Detuple 

open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.TcGlobals



(* detuple pass: *)  
val DetupleImplFile : CcuThunk -> TcGlobals -> TypedImplFile -> TypedImplFile

module GlobalUsageAnalysis = 
    val GetValsBoundInExpr : Expr -> Zset<Val>

    type accessor 

    /// Results is "expr information".
    /// This could extend to be a full graph view of the expr.
    /// Later could support "safe" change operations, and optimisations could be in terms of those.
    type Results =
       { /// v -> context / APP inst args 
         Uses   : Zmap<Val,(accessor list * TType list * Expr list) list>; 
         /// v -> binding repr 
         Defns   : Zmap<Val,Expr>;                                    
         /// bound in a decision tree? 
         DecisionTreeBindings    : Zset<Val>;                                              
         /// v -> recursive? * v list   -- the others in the mutual binding 
         RecursiveBindings  : Zmap<Val,(bool * FlatVals)>;                        
         /// val not defined under lambdas 
         TopLevelBindings : Zset<Val>;                                            
         /// top of expr toplevel? (true) 
         IterationIsAtTopLevel      : bool;                                                         
       }
    val GetUsageInfoOfImplFile :  TcGlobals -> TypedImplFile -> Results
