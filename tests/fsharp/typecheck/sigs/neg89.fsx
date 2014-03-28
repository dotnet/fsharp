module M1 =
    type A = A list
    and B = A list
   
module M2 = 
    type A = B -> B
    and B = A * A
    and C = A list   
