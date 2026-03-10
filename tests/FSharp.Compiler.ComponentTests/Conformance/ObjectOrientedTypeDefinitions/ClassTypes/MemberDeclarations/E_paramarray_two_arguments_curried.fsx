// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions
// Regression test for FSHARP1.0:5580
// disallow curried byref parameters
// ParamArray arguments - non curried
//<Expects status="error" span="(7,19-7,20)" id="FS0440">Methods with curried arguments cannot declare 'out', 'ParamArray', 'optional', 'ReflectedDefinition', 'byref', 'CallerLineNumber', 'CallerMemberName', or 'CallerFilePath' arguments</Expects>
type Misc0() = 
    static member M ([<System.ParamArray>] args : string[]) ([<System.ParamArray>] argc : int) = args.Length + argc
