let mutable c = 0un

let f0 () =
    for n in 10un..1un do
        c <- n

let f00 () =
    for n in 10un..1un..1un do
        c <- n

let f1 () =
    for n in 1un..10un do
        c <- n

let f2 start =
    for n in start..10un do
        c <- n

let f3 finish =
    for n in 1un..finish do
        c <- n

let f4 (start: unativeint) finish =
    for n in start..finish do
        c <- n

let f5 () =
    for n in 1un..1un..10un do
        c <- n

let f6 () =
    for n in 1un..2un..10un do
        c <- n

let f7 start =
    for n in start..2un..10un do
        c <- n

let f8 step =
    for n in 1un..step..10un do
        c <- n

let f9 finish =
    for n in 1un..2un..finish do
        c <- n

let f10 (start: unativeint) step finish =
    for n in finish..step..finish do
        c <- n

let f11 start finish =
    for n in start..0un..finish do
        c <- n

let f12 () =
    for n in 1un..0un..10un do
        c <- n
