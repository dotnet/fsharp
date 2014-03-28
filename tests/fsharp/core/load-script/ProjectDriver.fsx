// #Conformance #FSI 
#load "ThisProject.fsx"

[<System.Obsolete("x")>]
let fn x = 0
let y = fn 1 // This would be an 'obsolete' warning but ThisProject.fsx nowarns it

printfn "Result = %d" (Namespace.Type.Method())

let rf = typeof<System.Web.Mobile.CookielessData>
printfn "Type from referenced assembly = %s" (rf.ToString())
