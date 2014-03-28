// #Conformance #ObjectOrientedTypes #Structs 
#light
namespace NS
  module M = 
    let mutable r = 0
    [<Struct>]
    type S(y:int) = 
        static let mutable v = 3
        static do v <- v + 1
        // do r <- r + 1  // this is not allowed anymore!
        static member V = v
        member x.M = y
        
  module Test = 
    open M
    let mutable res = true
    
    if not ((r = 0) && (S.V = 4) && (r = 0) )then  // ctor not called, then cctor is called
      printf "Lib.DoStaticLetDo1 failed\n"
      res <- false
    
    let a = S()

    if not ((r = 0) && (S.V = 4) && (a.M = 0)) then  // nothing called
      printf "Lib.DoStaticLetDo2 failed\n"
      res <- false

    let b = S(5)

    if not ((r = 0) && (S.V = 4) && (b.M = 5) ) then  // only ctor called
      printf "Lib.DoStaticLetDo3 failed\n"
      res <- false



    (if (res) then 0 else 1) |> exit
