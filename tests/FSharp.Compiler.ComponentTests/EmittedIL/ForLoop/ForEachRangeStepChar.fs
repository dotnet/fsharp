let mutable c = '\000'

let f0 () =
    for n in 'z'..'a' do
        c <- n

let f00 () =
    for n in 'z'..'\001'..'a' do
        c <- n

let f1 () =
    for n in 'a'..'z' do
        c <- n

let f2 start =
    for n in start..'z' do
        c <- n

let f3 finish =
    for n in 'a'..finish do
        c <- n

let f4 (start: char) finish =
    for n in start..finish do
        c <- n

let f5 () =
    for n in 'a'..'\001'..'z' do
        c <- n

let f6 () =
    for n in 'a'..'\002'..'z' do
        c <- n

let f7 start =
    for n in start..'\002'..'z' do
        c <- n

let f8 step =
    for n in 'a'..step..'z' do
        c <- n

let f9 finish =
    for n in 'a'..'\002'..finish do
        c <- n

let f10 (start: char) step finish =
    for n in finish..step..finish do
        c <- n

let f11 start finish =
    for n in start..'\000'..finish do
        c <- n

let f12 () =
    for n in 'a'..'\000'..'z' do
        c <- n
