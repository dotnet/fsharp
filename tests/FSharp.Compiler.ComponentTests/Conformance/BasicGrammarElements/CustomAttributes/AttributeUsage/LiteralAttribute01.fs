[<Struct>] type UserID = private UserID of int
    with [<Literal>] static member Maximum = 9999 //error currently, should allow

type UserID2 =
    UserID2 of int
        [<Literal>]
        static member Maximum = 9999 //error currently, should allow

type UserID3() = 
    [<Literal>]
    let privateLiteral = 3 //currently allowed
    [<Literal>]
    static member PublicLiteral = 3 //error currently, should allow

module UserId4 =
    [<Literal>]
    let privateLiteral = 3 //currently allowed 