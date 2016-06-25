//<Expects id="FS1246" span="(9,19-9,45)" status="error">'CallerFilePath' must be applied to an argument of type 'string', but has been applied to an argument of type 'int'</Expects>
//<Expects id="FS1247" span="(12,19-12,47)" status="error">'CallerFilePath' can only be applied to optional arguments</Expects>
//<Expects id="FS1247" span="(15,19-15,47)" status="error">'CallerFilePath' can only be applied to optional arguments</Expects>
namespace Test

open System.Runtime.CompilerServices

type MyTy() =
    static member GetCallerFilePathNotString([<CallerFilePath>] ?path : int) =
        path

    static member GetCallerFilePathNotOptional([<CallerFilePath>] path : string) =
        path

    static member GetCallerFilePathNotOptional([<CallerFilePath>] path : string option) =
        path