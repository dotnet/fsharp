// #Regression #Conformance #TypesAndModules #Unions 
// Regression test for FSHARP1.0:4331
// Either provide F# reflection access to attributes on union cases, or remove the ability in the language to apply thes

type X = 
  | [<System.Obsolete("no")>] A
  | [<System.Obsolete("yes")>] B of int

let uci = Microsoft.FSharp.Reflection.FSharpType.GetUnionCases (typeof<X>)

let res = 
    uci.[0].GetCustomAttributes(typeof<System.ObsoleteAttribute>).Length = 1 &&
    uci.[1].GetCustomAttributes(typeof<System.ObsoleteAttribute>).Length = 1 &&
    uci.[0].GetCustomAttributes().Length >= 1 &&
    uci.[1].GetCustomAttributes().Length >= 1

if not res then failwith "Failed: 1"
