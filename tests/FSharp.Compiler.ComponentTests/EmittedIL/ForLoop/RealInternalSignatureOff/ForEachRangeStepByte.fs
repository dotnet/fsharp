let mutable c = 0uy

let f0 () =
    for n in 10uy..1uy do
        c <- n

let f00 () =
    for n in 10uy..1uy..1uy do
        c <- n

let f1 () =
    for n in 1uy..10uy do
        c <- n

let f2 start =
    for n in start..10uy do
        c <- n

let f3 finish =
    for n in 1uy..finish do
        c <- n

let f4 (start: byte) finish =
    for n in start..finish do
        c <- n

let f5 () =
    for n in 1uy..1uy..10uy do
        c <- n

let f6 () =
    for n in 1uy..2uy..10uy do
        c <- n

let f7uy start =
    for n in start..2uy..10uy do
        c <- n

let f8 step =
    for n in 1uy..step..10uy do
        c <- n

let f9 finish =
    for n in 1uy..2uy..finish do
        c <- n

let f10 (start: byte) step finish =
    for n in finish..step..finish do
        c <- n

let f11 start finish =
    for n in start..0uy..finish do
        c <- n

let f12 () =
    for n in 1uy..0uy..10uy do
        c <- n
