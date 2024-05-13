let mutable c = 0y

let f0 () =
    for n in 10y..1y do
        c <- n

let f00 () =
    for n in 10y..1y..1y do
        c <- n

let f1 () =
    for n in 1y..10y do
        c <- n

let f2 start =
    for n in start..10y do
        c <- n

let f3 finish =
    for n in 1y..finish do
        c <- n

let f4 (start: sbyte) finish =
    for n in start..finish do
        c <- n

let f5 () =
    for n in 1y..1y..10y do
        c <- n

let f6 () =
    for n in 1y..2y..10y do
        c <- n

let f7 start =
    for n in start..2y..10y do
        c <- n

let f8 step =
    for n in 1y..step..10y do
        c <- n

let f9 finish =
    for n in 1y..2y..finish do
        c <- n

let f10 (start: sbyte) step finish =
    for n in finish..step..finish do
        c <- n

let f11 start finish =
    for n in start..0y..finish do
        c <- n

let f12 () =
    for n in 1y..0y..10y do
        c <- n

let f13 () =
    for n in 10y .. -1y .. 1y do
        c <- n

let f14 () =
    for n in 10y .. -2y .. 1y do
        c <- n
