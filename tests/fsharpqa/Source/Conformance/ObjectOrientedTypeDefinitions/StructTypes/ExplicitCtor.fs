// #Conformance #ObjectOrientedTypes #Structs 
#light
namespace NS
  module M = 
    type S1 (i : int) =
      struct
        new (i : string, j : int) = S1(j)
        member x.m = i
      end
      
    type S2 =
      struct
        val mutable m : int
        new (i : int ) = {m=i}
      end


  module Test = 
    open M
    let mutable res = true
    
    let mutable a = new S1("Hello",5)
    let b = S2(6)

    if not (a.m = 5) then
      printf "Lib.MutableFields failed\n"
      res <- false

    if not (b.m = 6) then
      printf "Lib.MutableFields failed\n"
      res <- false

    
    (if (res) then 0 else 1) |> exit
