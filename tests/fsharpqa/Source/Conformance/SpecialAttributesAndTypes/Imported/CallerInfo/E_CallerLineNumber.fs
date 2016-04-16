//<Expects id="FS1234" span="(8,19-8,44)" status="error">Bogus caller info definition</Expects>
//<Expects id="FS1234" span="(11,19-11,49)" status="error">Bogus caller info definition</Expects>
namespace Test

open System.Runtime.CompilerServices

type MyTy() =
    static member GetCallerLineNumberNotInt([<CallerLineNumber>] ?line : string) =
        line

    static member GetCallerLineNumberNotOptional([<CallerLineNumber>] line : string) =
        line