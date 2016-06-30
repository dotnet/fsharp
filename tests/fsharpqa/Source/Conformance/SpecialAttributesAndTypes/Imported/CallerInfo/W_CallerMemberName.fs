//<Expects id="FS3206" span="(7,41-7,57)" status="warning">The CallerMemberNameAttribute applied to parameter 'name' will have no effect. It is overridden by the CallerFilePathAttribute.</Expects>
namespace Test

open System.Runtime.CompilerServices

type MyTy() =
    static member GetCallerMemberName([<CallerMemberName; CallerFilePath>] ?name : string) =
        name