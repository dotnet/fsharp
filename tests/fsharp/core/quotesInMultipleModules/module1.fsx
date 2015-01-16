module Test =
    let inline run() = 
       <@ fun (output:'T[]) (input:'T[]) (length:int) ->
          let start = 0
          let mutable i = start
          while i < length do
             output.[i] <- input.[i]
             i <- i + 1 @>

    let bar() = 
        sprintf "%A" (run())

type C() = 

  [<ReflectedDefinition>]
  static member F x = (C(), System.DateTime.Now)

