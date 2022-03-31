// #NoMono #NoMT #CodeGen #EmittedIL   
module IfThenElse01    // Regression test for F#4519
module M =
   let m () = 
        let f5 (x:int) (y:int) (z:'a) (w:'a) = if (x > y) then z else w
        f5 10 10 'a' 'b'
   m()
