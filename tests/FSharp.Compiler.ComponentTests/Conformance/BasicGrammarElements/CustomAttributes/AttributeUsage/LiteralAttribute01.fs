[<Struct>] type UserID = private UserID of int
    with [<Literal>] static member Maximum = 9999

type UserID2 =
    UserID2 of int
        [<Literal>]
        static member Maximum = 9999

type UserID3() = 
    [<Literal>]
    let privateLiteral = 3 
    [<Literal>]
    static member PublicLiteral = 3

module UserId4 =
    [<Literal>]
    let privateLiteral = 3