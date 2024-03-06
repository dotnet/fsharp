let mutable c = 0UL

let f0 () =
    for n in 10UL..1UL do
        c <- n

let f00 () =
    for n in 10UL..1UL..1UL do
        c <- n

let f1 () =
    for n in 1UL..10UL do
        c <- n

let f2 start =
    for n in start..10UL do
        c <- n

let f3 finish =
    for n in 1UL..finish do
        c <- n

let f4 (start: uint64) finish =
    for n in start..finish do
        c <- n

let f5 () =
    for n in 1UL..1UL..10UL do
        c <- n

let f6 () =
    for n in 1UL..2UL..10UL do
        c <- n

let f7 start =
    for n in start..2UL..10UL do
        c <- n

let f8 step =
    for n in 1UL..step..10UL do
        c <- n

let f9 finish =
    for n in 1UL..2UL..finish do
        c <- n

let f10 (start: uint64) step finish =
    for n in finish..step..finish do
        c <- n

let f11 start finish =
    for n in start..0UL..finish do
        c <- n

let f12 () =
    for n in 1UL..0UL..10UL do
        c <- n
