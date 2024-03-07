let mutable c = 0L

let f0 () =
    for n in 10L..1L do
        c <- n

let f00 () =
    for n in 10L..1L..1L do
        c <- n

let f1 () =
    for n in 1L..10L do
        c <- n

let f2 start =
    for n in start..10L do
        c <- n

let f3 finish =
    for n in 1L..finish do
        c <- n

let f4 (start: int64) finish =
    for n in start..finish do
        c <- n

let f5 () =
    for n in 1L..1L..10L do
        c <- n

let f6 () =
    for n in 1L..2L..10L do
        c <- n

let f7 start =
    for n in start..2L..10L do
        c <- n

let f8 step =
    for n in 1L..step..10L do
         c <- n

let f9 finish =
    for n in 1L..2L..finish do
         c <- n

let f10 (start: int64) step finish =
    for n in finish..step..finish do
        c <- n

let f11 start finish =
    for n in start..0L..finish do
        c <- n

let f12 () =
    for n in 1L..0L..10L do
        c <- n

let f13 () =
    for n in 10L.. -1L ..1L do
        c <- n

let f14 () =
    for n in 10L.. -2L ..1L do
        c <- n
