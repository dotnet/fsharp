// #Regression #Conformance #PatternMatching #ActivePatterns 
// FSHARP1.0:5590 (see also FSHARP1.0:1104, which is where this source came from)
//<Expects status="warning" span="(23,11-23,12)" id="FS0025">Incomplete pattern matches on this expression\.$</Expects>
//<Expects status="warning" span="(30,11-30,12)" id="FS0025">Incomplete pattern matches on this expression\.$</Expects>
//<Expects status="error" span="(22,6-22,38)" id="FS1210">Active pattern '\|ClientExternalTypeUse\|WillFail\|' has a result type containing type variables that are not determined by the input\. The common cause is a when a result case is not mentioned, e\.g\. 'let \(\|A\|B\|\) \(x:int\) = A x'\. This can be fixed with a type constraint, e\.g\. 'let \(\|A\|B\|\) \(x:int\) : Choice<int,unit> = A x'$</Expects>

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

// Error when multicase active patterns result type doesn't correspond to input value  
let (|ClientExternalTypeUse|WillFail|) e =
    match e with
      | (MethodCall(MapDeclaringType(_, CastToMemberInfo(_)), _))
      | (CtorCall(MapDeclaringType(_, CastToMemberInfo(_)), _)) ->
          failwith ""

// Error above doesn't apply for single case active patterns 
let (|ClientExternalTypeUse|_|) e =
    match e with
      | (MethodCall(MapDeclaringType(_, CastToMemberInfo(_)), _))
      | (CtorCall(MapDeclaringType(_, CastToMemberInfo(_)), _)) ->
          failwith ""
