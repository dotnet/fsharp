// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression test for FSHARP1.0:1494 
// New tiebreaker rules for method overloading? 
#light

open System.Reflection
open System.Linq.Expressions

let asExpr x = (x :> Expression)

let expressionTreeForMethod (mi:MethodInfo) = 
    let parameters = mi.GetParameters()
    let paramExprs : Expression[] = parameters |> Array.map (fun pi -> Expression.Parameter(pi.ParameterType, pi.Name) |> asExpr)
    Expression.Call(null , mi, paramExprs)
