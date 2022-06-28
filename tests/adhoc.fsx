let legacyConcat1 (x: string) (y: string) = x ^ y
let legacyConcat2 (x: string) (y: string) = x ^y
let legacyConcat3 (x: string) (y: string) = x^ y
let legacyConcat4 (x: string) (y: string) = x^y

type IAdditionOperator<'T> =
    static abstract op_Addition: 'T * 'T -> 'T

type C(c: int) =
    member _.Value = c
    interface IAdditionOperator<C> with
        static member op_Addition(x, y) = C(x.Value + y.Value) 

let f<'T when 'T :> IAdditionOperator<'T>>(x: 'T, y: 'T) =
    'T.op_Addition(x, y)

let f2<'T when 'T :> IAdditionOperator<'T>>(x: 'T, y: 'T) =
    'T.(+)(x, y)

if f<C>(C(3), C(4)).Value <> 7 then
    failwith "incorrect value"

let inline f3<^T when ^T :> IAdditionOperator<^T>>(x: ^T, y: ^T) =
    ^T.op_Addition(x,y)

let inline f4<^T when ^T : (static member (+): ^T * ^T -> ^T)>(x: ^T, y: ^T) =
    ^T.op_Addition(x,y)

let inline f5<^T when ^T : (static member (+): ^T * ^T -> ^T)>(x: ^T, y: ^T) =
    ^T.(+)(x,y)

let inline f6<^T when ^T : (static member (+): ^T * ^T -> ^T)>(x: ^T, y: ^T) =
    x + y

//let f7<'T when 'T :> IAdditionOperator<'T>>(x: 'T, y: 'T) =
//    x + y
