//<Expects id="FS1246" span="(9,66-9,70)" status="error">'CallerFilePath' must be applied to an argument of type 'string', but has been applied to an argument of type 'int'</Expects>
//<Expects id="FS1247" span="(12,67-12,71)" status="error">'CallerFilePath' can only be applied to optional arguments</Expects>
//<Expects id="FS1247" span="(15,67-15,71)" status="error">'CallerFilePath' can only be applied to optional arguments</Expects>
namespace Test

open System.Runtime.CompilerServices

type MyTy() =
    static member GetCallerFilePathNotString([<CallerFilePath>] ?path : int) =
        path

    static member GetCallerFilePathNotOptional([<CallerFilePath>] path : string) =
        path

    static member GetCallerFilePathNotOptional([<CallerFilePath>] path : string option) =
        path