// #Regression #Conformance #PatternMatching #ActivePatterns 
// FSHARP1.0:1104
// Bug's title: *** WARNING: basic block at end of method ends without a leave, branch, return or throw. Adding throw
// Note: repro was updated with type annotation after fix for FSHARP1.0:5590
#nowarn "57"

open System
open System.Collections.Generic
open System.Reflection

open Microsoft.FSharp.Reflection
open Microsoft.FSharp.Quotations
    
let (|MapDeclaringType|_|) (mi:#MemberInfo) : Option<System.Type * System.Reflection.MethodInfo> = failwith ""
let (|CastToMemberInfo|) (mi:#MemberInfo) =  (mi :> MemberInfo)
let (|MethodCall|_|) (e:Expr) : Option<MethodInfo * Expr list>  =  failwith ""
let (|CtorCall|_|) (e:Expr) : Option<ConstructorInfo * Expr list>  =  failwith ""
  
let (|ClientExternalTypeUse|_|) (e:Expr) : option<Expr>=
    match e with
      | (MethodCall(MapDeclaringType(_, CastToMemberInfo(_)), _))  
      | (CtorCall(MapDeclaringType(_, CastToMemberInfo(_)), _)) ->
          failwith ""

// Original bug was problem with PEVerify

exit 0
