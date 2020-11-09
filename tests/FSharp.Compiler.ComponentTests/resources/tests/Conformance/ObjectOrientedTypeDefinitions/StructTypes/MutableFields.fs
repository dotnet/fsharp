// #Conformance #ObjectOrientedTypes #Structs 
#light
namespace NS
  module M = 
    type S1 (i : int) =
      struct
        [<DefaultValue>]
        val mutable m : int
      end
      
    type S2 =
      struct
        val mutable m : int
      end

    type S3 (i : int) =
      struct
        member x.M = i
      end

  module Test = 
    open M
    let mutable res = true
    
    let mutable a = new S1(0)
    let b = S2()
    let c = S3()
    let d = S3(4)

    if not (a.m = 0) then
      printf "Lib.MutableFields failed\n"
      res <- false

    if not (b.m = 0) then
      printf "Lib.MutableFields failed\n"
      res <- false

    if not (c.M = 0) then
      printf "Lib.MutableFields failed\n"
      res <- false

    if not (d.M = 4) then
      printf "Lib.MutableFields failed\n"
      res <- false

    (if (res) then 0 else 1) |> exit
