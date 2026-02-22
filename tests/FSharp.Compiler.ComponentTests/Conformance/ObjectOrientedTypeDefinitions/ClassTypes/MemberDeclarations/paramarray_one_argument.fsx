// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions
// Regression test for FSHARP1.0:5580
// disallow curried byref parameters
// ParamArray arguments
type Misc0() = 
    static member M([<System.ParamArray>] args : string[]) = args.Length
