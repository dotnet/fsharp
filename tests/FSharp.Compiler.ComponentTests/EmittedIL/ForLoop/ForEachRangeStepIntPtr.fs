let mutable c = 0n

let f0 () =
    for n in 10n..1n do
        c <- n

let f00 () =
    for n in 10n..1n..1n do
        c <- n

let f1 () =
    for n in 1n..10n do
        c <- n

let f2 start =
    for n in start..10n do
        c <- n

let f3 finish =
    for n in 1n..finish do
        c <- n

let f4 (start: nativeint) finish =
    for n in start..finish do
        c <- n

let f5 () =
    for n in 1n..1n..10n do
        c <- n

let f6 () =
    for n in 1n..2n..10n do
        c <- n

let f7 start =
    for n in start..2n..10n do
        c <- n

let f8 step =
    for n in 1n..step..10n do
         c <- n

let f9 finish =
    for n in 1n..2n..finish do
         c <- n

let f10 (start: nativeint) step finish =
    for n in finish..step..finish do
        c <- n

let f11 start finish =
    for n in start..0n..finish do
        c <- n

let f12 () =
    for n in 1n..0n..10n do
        c <- n

let f13 () =
    for n in 10n.. -1n ..1n do
        c <- n

let f14 () =
    for n in 10n.. -2n ..1n do
        c <- n
