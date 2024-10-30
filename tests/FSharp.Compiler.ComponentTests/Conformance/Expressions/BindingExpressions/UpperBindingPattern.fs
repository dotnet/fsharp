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

try ()
with Ex -> ()

type AnonymousObject<'T1, 'T2> =
    val private item1: 'T1
    member x.Item1 = x.item1

    new(Item1) = { item1 = Item1 }

type FSharpSource(Item1: string, SourceHash: string) = class end

let _ =
   query {
    for UpperCase in [1..10] do
       join b in [1..2] on (UpperCase = b)
       select b
}

let _ =
   query {
    for UpperCase in [1..10] do
    groupBy UpperCase into g
    select g.Key
}

let _ =
   query {
    for UpperCase in [1..10] do
    groupJoin UpperCase2 in [|1..2|] on (UpperCase = UpperCase2) into g
    for k in g do
    select (k + 1)
}

let _ =
   query {
    for Up in [1..10] do
       join b in [1..2] on (Up = b)
       select b
}

let _ =
   query {
    for Up in [1..10] do
    groupBy Up into g
    select g.Key
}

let _ =
   query {
    for Up in [1..10] do
    groupJoin U2 in [|1..2|] on (Up = U2) into g
    for k in g do
    select (k + 1)
}

type CustomerId2 = CustomerId2 of string * string

let getCustomerId3 (CustomerId2(AA, BBB)) = id

let (CustomerId2(AA, Bb)) = CustomerId2("AA", "BB")

try ()
with Ex as Foo -> ()

try ()
with Ex as Fo -> ()

try ()
with Ex as F -> ()