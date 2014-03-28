// #NoMono #CodeGen #Optimizations 
module Match01

type Test1 = 
   | X11 of int 
   | X12 of int 
   | X13 of int 
   | X14 of int 

let select1 x = 
   match x with 
   | X11 x1 -> x1
   | X12 _ -> 2 
   | X13 _ -> 3 
   | X14 _ -> 4

// select1 is not supposed to be inlined here
let fm y = select1 y
