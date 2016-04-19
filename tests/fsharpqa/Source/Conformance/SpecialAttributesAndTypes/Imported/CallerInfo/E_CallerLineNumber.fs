//<Expects id="FS1246" span="(8,19-8,44)" status="error">'CallerLineNumber' must be applied to an argument of type 'int', but has been applied to an argument of type 'string'</Expects>
//<Expects id="FS1247" span="(11,19-11,49)" status="error">'CallerLineNumber' can only be applied to optional arguments</Expects>
namespace Test

open System.Runtime.CompilerServices

type MyTy() =
    static member GetCallerLineNumberNotInt([<CallerLineNumber>] ?line : string) =
        line

    static member GetCallerLineNumberNotOptional([<CallerLineNumber>] line : string) =
        line