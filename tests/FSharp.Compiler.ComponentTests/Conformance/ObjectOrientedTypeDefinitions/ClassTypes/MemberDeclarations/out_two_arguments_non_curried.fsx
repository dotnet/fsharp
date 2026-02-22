// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions
// Regression test for FSHARP1.0:5580
// disallow curried byref parameters
// Out arguments - non curried
type Misc1() =
    static member M ([<System.Runtime.InteropServices.OutAttribute>] foo : int byref, ([<System.Runtime.InteropServices.OutAttribute>] bar : int byref)) = foo <- 10
