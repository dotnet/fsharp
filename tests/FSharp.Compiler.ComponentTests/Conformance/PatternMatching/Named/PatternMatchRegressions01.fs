// #Regression #Conformance #PatternMatching #ActivePatterns 
#light  

// FSB 1103, bad code generation for active patterns from fswebtools example


#nowarn "57"

open System
open System.Collections.Generic
open System.Reflection

open Microsoft.FSharp.Reflection
open Microsoft.FSharp.Quotations

let (|MapDeclaringType|_|) (mi:#MemberInfo) : Option<System.Reflection.MethodInfo> = failwith ""
let (|CastToMemberInfo|_|) (mi:#MemberInfo) : Option<_> =  Some(mi :> MemberInfo)
let (|MethodCall|_|) (e:Expr) : Option<MethodInfo * Expr list>  =  failwith ""
let (|CtorCall|_|) (e:Expr) : Option<ConstructorInfo * Expr list>  =  failwith ""
  
let f e =
    match e with
      | (MethodCall(MapDeclaringType(_), _))  
      | (CtorCall(CastToMemberInfo(_), _)) ->
          failwith ""
          

// Original bug was problem with PEVerify
exit 0
