// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions
// Regression test for FSHARP1.0:5580
// disallow curried byref parameters
// byref arguments - non curried
type Misc1() =
    static member M (foo : int byref, bar : int byref) = foo <- 10
