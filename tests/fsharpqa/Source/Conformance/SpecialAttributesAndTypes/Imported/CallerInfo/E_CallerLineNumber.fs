//<Expects id="FS1246" span="(9,19-9,44)" status="error">'CallerLineNumber' must be applied to an argument of type 'int', but has been applied to an argument of type 'string'</Expects>
//<Expects id="FS1247" span="(12,19-12,49)" status="error">'CallerLineNumber' can only be applied to optional arguments</Expects>
//<Expects id="FS1247" span="(15,19-15,49)" status="error">'CallerLineNumber' can only be applied to optional arguments</Expects>
namespace Test

open System.Runtime.CompilerServices

type MyTy() =
    static member GetCallerLineNumberNotInt([<CallerLineNumber>] ?line : string) =
        line

    static member GetCallerLineNumberNotOptional([<CallerLineNumber>] line : int) =
        line

    static member GetCallerLineNumberNotOptional([<CallerLineNumber>] line : int option) =
        line