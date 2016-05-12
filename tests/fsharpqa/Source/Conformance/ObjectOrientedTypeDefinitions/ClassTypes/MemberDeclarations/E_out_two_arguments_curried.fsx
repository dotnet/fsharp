// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions
// Regression test for FSHARP1.0:5580
// disallow curried byref parameters
// Out arguments - curried
//<Expects status="error" span="(7,19-7,20)" id="FS0440">Methods with curried arguments cannot declare 'out', 'ParamArray', 'optional', 'ReflectedDefinition', 'byref', 'CallerLineNumber', 'CallerMemberName', or 'CallerFilePath' arguments</Expects>
type Misc2() =
    static member M ([<System.Runtime.InteropServices.OutAttribute>] foo : int byref) ([<System.Runtime.InteropServices.OutAttribute>] bar : int byref) = foo <- 10
