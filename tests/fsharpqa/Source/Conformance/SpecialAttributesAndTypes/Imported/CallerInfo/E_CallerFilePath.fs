//<Expects id="FS1234" span="(8,19-8,45)" status="error">Bogus caller info definition</Expects>
//<Expects id="FS1234" span="(11,19-11,47)" status="error">Bogus caller info definition</Expects>
namespace Test

open System.Runtime.CompilerServices

type MyTy() =
    static member GetCallerFilePathNotString([<CallerFilePath>] ?path : int) =
        path

    static member GetCallerFilePathNotOptional([<CallerFilePath>] path : string) =
        path