module APUsageInModule
let (|A|) = 6
let (|A|B|) x = if x = "Foo" then A else B

let (|A|B|_|) = None

type APUsageInClass() =
    
    let (|A|) = 7

    let (|A|B|) x =
        if x = "Foo" then A
        else B

    let (|A|B|_|) = None

    let (|A|B|) x =
        if x = "Foo" then A
        else B

    static member (|A|) = 7
    static member (|A|B|) = ValueNone
    static member (|C|) = 5
    static member (|A|B|) x = A