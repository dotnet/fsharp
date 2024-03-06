let mutable c = 0u

let f0 () =
    for n in 10u..1u do
        c <- n

let f00 () =
    for n in 10u..1u..1u do
        c <- n

let f1 () =
    for n in 1u..10u do
        c <- n

let f2 start =
    for n in start..10u do
        c <- n

let f3 finish =
    for n in 1u..finish do
        c <- n

let f4 (start: uint32) finish =
    for n in start..finish do
        c <- n

let f5 () =
    for n in 1u..1u..10u do
        c <- n

let f6 () =
    for n in 1u..2u..10u do
        c <- n

let f7 start =
    for n in start..2u..10u do
        c <- n

let f8 step =
    for n in 1u..step..10u do
        c <- n

let f9 finish =
    for n in 1u..2u..finish do
        c <- n

let f10 (start: uint32) step finish =
    for n in finish..step..finish do
        c <- n

let f11 start finish =
    for n in start..0u..finish do
        c <- n

let f12 () =
    for n in 1u..0u..10u do
        c <- n
