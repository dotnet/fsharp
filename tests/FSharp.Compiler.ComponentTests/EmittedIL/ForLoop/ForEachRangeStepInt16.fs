let mutable c = 0s

let f0 () =
    for n in 10s..1s do
        c <- n

let f00 () =
    for n in 10s..1s..1s do
        c <- n

let f1 () =
    for n in 1s..10s do
        c <- n

let f2 start =
    for n in start..10s do
        c <- n

let f3 finish =
    for n in 1s..finish do
        c <- n

let f4 (start: int16) finish =
    for n in start..finish do
        c <- n

let f5 () =
    for n in 1s..1s..10s do
        c <- n

let f6 () =
    for n in 1s..2s..10s do
        c <- n

let f7 start =
    for n in start..2s..10s do
        c <- n

let f8 step =
    for n in 1s..step..10s do
        c <- n

let f9 finish =
    for n in 1s..2s..finish do
        c <- n

let f10 (start: int16) step finish =
    for n in finish..step..finish do
        c <- n

let f11 start finish =
    for n in start..0s..finish do
        c <- n

let f12 () =
    for n in 1s..0s..10s do
        c <- n

let f13 () =
    for n in 10s .. -1s .. 1s do
        c <- n

let f14 () =
    for n in 10s .. -2s .. 1s do
        c <- n
