// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions
// Regression test for FSHARP1.0:5580
// disallow curried byref parameters
// Out arguments
type Misc0() =
    static member M ([<System.Runtime.InteropServices.OutAttribute>] foo : int byref) = foo <- 10
