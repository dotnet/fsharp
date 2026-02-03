// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions
// Regression test for FSHARP1.0:5580
// disallow curried byref parameters
// byref arguments
type Misc0() =
    static member M (foo : int byref) = foo <- 10
