let Aaa = ()

let f1 Us Uk = ()

let f2 US CA = ()

let f3 U A = ()

let AAA = ()

let f4 USA CAN = ()

let f5 Usa Can = ()

type Class() =
    static member f1(Us, Uk) = ()

    static member f2 US CA = ()

    static member f3 U A = ()

    static member f4(USA, CAN) = ()

    static member f5 Usa Can = ()

    member this.f6(Us, Uk) = ()

    member this.f7 US CA = ()

    member this.f8 U A = ()

    member this.f9(USA, CAN) = ()

    member this.f10 Usa Can = ()

type CustomerId = CustomerId of string

let customerId = CustomerId("123")

let (CustomerId BBB) = customerId

let getCustomerId (CustomerId CCC) = id

let getCustomerId2 (CustomerId CC) = id

for III in [1..10] do
    ()

for II in [1..10] do
    ()

[ 1; 3; 5 ]
|> List.map (fun DDD ->  DDD + 1)
|> ignore

[ 1; 3; 5 ]
|> List.map (fun DD ->  DD + 1)
|> ignore
