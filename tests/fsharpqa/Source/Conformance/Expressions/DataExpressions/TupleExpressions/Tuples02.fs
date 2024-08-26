// #Conformance #DataExpressions #Tuples 
#light

// Verify tuples work right when crossing the '6' boundary

let tuple5 = 1, 2, 3, 4, 5
let tuple6 = 1, 2, 3, 4, 5, 6
let tuple7 = 1, 2, 3, 4, 5, 6, 7

let tuple11 = 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1
let tuple12 = 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2
let tuple13 = 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3

let tuple17 = 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7
let tuple18 = 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8
let tuple19 = 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9

// --------------------------------------------------

let monsterTuple = 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'

let test26 beastOfATuple = 
    match beastOfATuple with
    | a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z
        -> if a <> 'a' then exit 1
           if b <> 'b' then exit 1
           if c <> 'c' then exit 1
           if d <> 'd' then exit 1
           if e <> 'e' then exit 1
           if f <> 'f' then exit 1
           if g <> 'g' then exit 1
           if h <> 'h' then exit 1
           if i <> 'i' then exit 1
           if j <> 'j' then exit 1
           if k <> 'k' then exit 1
           if l <> 'l' then exit 1
           if m <> 'm' then exit 1
           if n <> 'n' then exit 1
           if o <> 'o' then exit 1
           if p <> 'p' then exit 1
           if q <> 'q' then exit 1
           if r <> 'r' then exit 1
           if s <> 's' then exit 1
           if t <> 't' then exit 1
           if u <> 'u' then exit 1
           if v <> 'v' then exit 1
           if w <> 'w' then exit 1
           if x <> 'x' then exit 1
           if y <> 'y' then exit 1
           if z <> 'z' then exit 1
           true

if test26 monsterTuple <> true then exit 1

exit 0
