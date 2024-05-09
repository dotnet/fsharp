open FSharp.Data.UnitSystems.SI.UnitSymbols

let mutable c = 0L<m>

let f1 () =
    for n in 10L<m>..1L<m>..1L<m> do
        c <- n

let f2 () =
    for n in 1L<m>..1L<m>..10L<m> do
        c <- n

let f3 () =
    for n in 1L<m>..2L<m>..10L<m> do
        c <- n

let f4 start =
    for n in start..2L<m>..10L<m> do
        c <- n

let f5 step =
    for n in 1L<m>..step..10L<m> do
        c <- n

let f6 finish =
    for n in 1L<m>..2L<m>..finish do
        c <- n

let f7 start step finish =
    for n in start..step..finish do
        c <- n

let f8 start finish =
    for n in start..0L<m>..finish do
        c <- n

let f9 () =
    for n in 1L<m>..0L<m>..10L<m> do
        c <- n

let f10 () =
    for n in 10L<m> .. -1L<m>..1L<m> do
        c <- n

let f11 () =
    for n in 10L<m> .. -2L<m>..1L<m> do
        c <- n
