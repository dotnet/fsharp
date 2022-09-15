// SDK version 7.0.100-preview.6 or newer has to be installed for this to work
open System.Numerics

type IAdditionOperator<'T when 'T :> IAdditionOperator<'T>> =
    static abstract op_Addition: 'T * 'T -> 'T      // Produces FS3535, advanced feature warning.

type ISinOperator<'T when 'T :> ISinOperator<'T>> =
    static abstract Sin: 'T -> 'T                   // Produces FS3535, advanced feature warning.

let square (x: 'T when 'T :> IMultiplyOperators<'T,'T,'T>) = x * x
//                           ^--- autocompletion works here

let zero (x: 'T when 'T :> INumber<'T>) = 'T.Zero

let add<'T when IAdditionOperators<'T, 'T, 'T>>(x: 'T) (y: 'T) = x + y
let min<'T when INumber<'T>> (x: 'T) (y: 'T) = 'T.Min(x, y)
//              ^                ^-------^--- no type params autocompletion
//              +-- no completion here

// Some declaration tests:
let fAdd<'T when 'T :> IAdditionOperator<'T>>(x: 'T, y: 'T) = x + y
let fSin<'T when ISinOperator<'T>>(x: 'T) = sin x
let fAdd'(x: 'T when 'T :> IAdditionOperator<'T>, y: 'T) = x + y
let fSin'(x: 'T when ISinOperator<'T>) = sin x
let fAdd''(x: #IAdditionOperator<'T>, y) = x + y // Produces FS0064 for x (the construct causes code to be less generic...)
let fSin''(x: #ISinOperator<'T>) = sin x         // Produces FS0064 for x (the construct causes code to be less generic...)
let fAdd'''(x: #IAdditionOperator<_>, y) = x + y // Does not produce FS0064
let fSin'''(x: #ISinOperator<_>) = sin x         // Does not produce FS0064

type AverageOps<'T when 'T: (static member (+): 'T * 'T -> 'T)
                   and  'T: (static member DivideByInt : 'T*int -> 'T)
                   and  'T: (static member Zero : 'T)> = 'T

let inline f_AverageOps<'T when AverageOps<'T>>(xs: 'T[]) =
    let mutable sum = 'T.Zero
    for x in xs do
        sum <- sum + x
    'T.DivideByInt(sum, xs.Length)
//     ^--- autocomplete works here just fine

let testZeroProp () =
    let i = 1I
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

    let i = 1m
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

    let i = 1y
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

    let i = 1uy
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

    let i = 1s
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

    let i = 1us
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

    let i = 1l
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

    let i = 1ul
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

    let i = 1u
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

    let i = 1un
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

    let i = 1L
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

    let i = 1UL
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

    let i = 1F
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

    let i = 1.0
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

    let i : char = 'a'
    let z = zero i
    let h = System.Convert.ToByte(z).ToString("x2")
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {h} ({i.GetType().ToString()})"

    let i = '1'B
    let z = zero i
    printfn $"Get zero for {i} ({i.GetType().ToString()}) = {z} ({i.GetType().ToString()})"

[<EntryPoint>]
let main _ =
    let x = 40
    let y = 20
    printfn $"Square of {x} is {square x}!"
    printfn $"{x} + {y} is {add x y}!"
    printfn $"Min of {x} and {y} is {min x y}"

    testZeroProp ()

    0
