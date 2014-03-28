module Test
type M5 = byref<int> * int             (* type alias for tuple with byref fields, trapped as tuple instance *)
let useM5 (x:M5) = x                   (* trap: tuple with byref type component *)
