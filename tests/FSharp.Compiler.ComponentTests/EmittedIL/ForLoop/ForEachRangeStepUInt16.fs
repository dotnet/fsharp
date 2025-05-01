let mutable c = 0us

let f0 () =
    for n in 10us..1us do
        c <- n

let f00 () =
    for n in 10us..1us..1us do
        c <- n

let f1 () =
    for n in 1us..10us do
        c <- n

let f2 start =
    for n in start..10us do
        c <- n

let f3 finish =
    for n in 1us..finish do
        c <- n

let f4 (start: uint16) finish =
    for n in start..finish do
        c <- n

let f5 () =
    for n in 1us..1us..10us do
        c <- n

let f6 () =
    for n in 1us..2us..10us do
        c <- n

let f7 start =
    for n in start..2us..10us do
        c <- n

let f8 step =
    for n in 1us..step..10us do
        c <- n

let f9 finish =
    for n in 1us..2us..finish do
        c <- n

let f10 (start: uint16) step finish =
    for n in finish..step..finish do
        c <- n

let f11 start finish =
    for n in start..0us..finish do
        c <- n

let f12 () =
    for n in 1us..0us..10us do
        c <- n
