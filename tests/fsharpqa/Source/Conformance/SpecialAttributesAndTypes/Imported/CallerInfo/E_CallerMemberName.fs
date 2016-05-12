//<Expects id="FS1246" span="(9,19-9,47)" status="error">'CallerMemberName' must be applied to an argument of type 'string', but has been applied to an argument of type 'int'</Expects>
//<Expects id="FS1247" span="(12,19-12,49)" status="error">'CallerMemberName' can only be applied to optional arguments</Expects>
//<Expects id="FS1247" span="(15,19-15,49)" status="error">'CallerMemberName' can only be applied to optional arguments</Expects>
namespace Test

open System.Runtime.CompilerServices

type MyTy() =
    static member GetCallerMemberNameNotString([<CallerMemberName>] ?name : int) =
        name

    static member GetCallerMemberNameNotOptional([<CallerMemberName>] name : string) =
        name

    static member GetCallerMemberNameNotOptional([<CallerMemberName>] name : string option) =
        name