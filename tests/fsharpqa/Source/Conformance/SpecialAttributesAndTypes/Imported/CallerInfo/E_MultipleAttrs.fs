//<Expects id="FS1246" span="(11,62-11,63)" status="error">'CallerFilePath' must be applied to an argument of type 'string', but has been applied to an argument of type 'int'</Expects>
//<Expects id="FS1246" span="(14,62-14,63)" status="error">'CallerFilePath' must be applied to an argument of type 'string', but has been applied to an argument of type 'int'</Expects>
//<Expects id="FS1246" span="(17,62-17,63)" status="error">'CallerLineNumber' must be applied to an argument of type 'int', but has been applied to an argument of type 'string'</Expects>
//<Expects id="FS1246" span="(20,62-20,63)" status="error">'CallerLineNumber' must be applied to an argument of type 'int', but has been applied to an argument of type 'string'</Expects>

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
