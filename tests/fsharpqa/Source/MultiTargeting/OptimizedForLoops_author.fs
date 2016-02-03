module Test
open System

let listFor (xs : 'a list) =
    for x in xs do
        printfn "%O" x
        
let stringFor (xs : string) =
    for x in xs do
        printfn "%O" x
        
let arrayFor (xs : 'a array) =
    for x in xs do
        printfn "%O" x
        
let seqFor (xs : seq<'a>) =
    for x in xs do
        printfn "%O" x