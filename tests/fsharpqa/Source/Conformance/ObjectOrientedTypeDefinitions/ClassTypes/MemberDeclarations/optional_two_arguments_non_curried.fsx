// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions
// Regression test for FSHARP1.0:5580
// disallow curried byref parameters
// optional arguments - non curried
//
type Misc0() =
    static member M (?foo : int, ?bar : int) = 10
