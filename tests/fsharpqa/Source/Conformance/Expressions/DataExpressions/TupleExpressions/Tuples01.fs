// #Conformance #DataExpressions #Tuples 
#light

let tuple1 = 1
let tuple2 = 1, 2
let tuple3 = 1, 2, 3
let tuple4 = 1, 2, 3, 4
let tuple5 = 1, 2, 3, 4, 5
let tuple6 = 1, 2, 3, 4, 5, 6
let tuple7 = 1, 2, 3, 4, 5, 6, 7
let tuple8 = 1, 2, 3, 4, 5, 6, 7, 8
let tuple9 = 1, 2, 3, 4, 5, 6, 7, 8, 9
let tuple0 = 1, 2, 3, 4, 5, 6, 7, 8, 9, 0

if sprintf "%A" tuple1 <> "1"                              then exit 1
if sprintf "%A" tuple2 <> "(1, 2)"                         then exit 1
if sprintf "%A" tuple3 <> "(1, 2, 3)"                      then exit 1
if sprintf "%A" tuple4 <> "(1, 2, 3, 4)"                   then exit 1
if sprintf "%A" tuple5 <> "(1, 2, 3, 4, 5)"                then exit 1
if sprintf "%A" tuple6 <> "(1, 2, 3, 4, 5, 6)"             then exit 1
if sprintf "%A" tuple7 <> "(1, 2, 3, 4, 5, 6, 7)"          then exit 1
if sprintf "%A" tuple8 <> "(1, 2, 3, 4, 5, 6, 7, 8)"       then exit 1
if sprintf "%A" tuple9 <> "(1, 2, 3, 4, 5, 6, 7, 8, 9)"    then exit 1
if sprintf "%A" tuple0 <> "(1, 2, 3, 4, 5, 6, 7, 8, 9, 0)" then exit 1

exit 0
