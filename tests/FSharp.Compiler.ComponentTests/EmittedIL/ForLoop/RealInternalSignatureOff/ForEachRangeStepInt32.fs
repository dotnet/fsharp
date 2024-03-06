let mutable c = 0

let f0 () =
    for n in 10..1 do
        c <- n

let f00 () =
    for n in 10..1..1 do
        c <- n

let f1 () =
    for n in 1..10 do
        c <- n

let f2 start =
    for n in start..10 do
        c <- n

let f3 finish =
    for n in 1..finish do
        c <- n

let f4 start finish =
    for n in start..finish do
        c <- n

let f5 () =
    for n in 1..1..10 do
        c <- n

let f6 () =
    for n in 1..2..10 do
        c <- n

let f7 start =
    for n in start..2..10 do
        c <- n

let f8 step =
    for n in 1..step..10 do
        c <- n

let f9 finish =
    for n in 1..2..finish do
        c <- n

let f10 start step finish =
    for n in finish..step..finish do
        c <- n

let f11 start finish =
    for n in start..0..finish do
        c <- n

let f12 () =
    for n in 1..0..10 do
        c <- n

let f13 () =
    for n in 10..-1..1 do
        c <- n

let f14 () =
    for n in 10..-2..1 do
        c <- n
