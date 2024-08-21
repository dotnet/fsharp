module BogusUseOfCLIMutable =  begin

  [<CLIMutable>]
  type BadClass() = member x.P = 1

  [<CLIMutable>]
  type BadUnion = A | B

  [<CLIMutable>]
  type BadInterface = interface end

  [<CLIMutable>]
  type BadClass2 = class end

  [<CLIMutable>]
  type BadEnum = | A = 1 | B = 2

  [<CLIMutable>]
  type BadDelegate = delegate of int * int -> int

  [<CLIMutable>]
  type BadStruct = struct val x : int end

  [<CLIMutable>]
  type BadStruct2(x:int)  = struct member v.X = x end

  [<CLIMutable>]
  type Good1 = { x : int; y : int }
  let good1 = { x = 1; y = 2 }

  [<CLIMutable>]
  type Good2 = { x : int }
  let good2 = { x = 1 }

end