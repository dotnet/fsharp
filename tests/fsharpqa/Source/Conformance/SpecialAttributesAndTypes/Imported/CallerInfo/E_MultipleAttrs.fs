//<Expects id="FS1246" span="(11,19-11,20)" status="error">'CallerFilePath' must be applied to an argument of type 'string', but has been applied to an argument of type 'int'</Expects>
//<Expects id="FS1246" span="(14,19-14,20)" status="error">'CallerFilePath' must be applied to an argument of type 'string', but has been applied to an argument of type 'int'</Expects>
//<Expects id="FS1246" span="(17,19-17,20)" status="error">'CallerLineNumber' must be applied to an argument of type 'int', but has been applied to an argument of type 'string'</Expects>
//<Expects id="FS1246" span="(20,19-20,20)" status="error">'CallerLineNumber' must be applied to an argument of type 'int', but has been applied to an argument of type 'string'</Expects>

namespace Test

open System.Runtime.CompilerServices

type MyTy() =
    static member A([<CallerFilePath>] [<CallerLineNumber>] ?x : int) =
        x

    static member B([<CallerLineNumber>] [<CallerFilePath>] ?x : int) =
        x

    static member C([<CallerFilePath>] [<CallerLineNumber>] ?x : string) =
        x

    static member D([<CallerLineNumber>] [<CallerFilePath>] ?x : string) =
        x
