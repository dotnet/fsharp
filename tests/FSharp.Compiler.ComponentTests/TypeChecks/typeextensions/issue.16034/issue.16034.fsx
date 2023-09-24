let mutable i = 0
type T() =
  member x.indexed1
    with get (a1: obj) =
      i <- i + 1
      printfn $"T().indexed1 {a1} !\t%03i{i}" 
      1
    and set (a1: obj) (value: int) =
      i <- i + 1
      printfn $"T().indexed1 {a1} <- {value} !\t%03i{i}"

module Extensions =
  let mutable j = 0
  type T with
    member x.indexed1 
      with get (aa1: obj) =
        i <- i + 1
        j <- j + 1
        printfn $"type extensions aa1 {aa1} !\t%03i{i}\t%03i{j}"
        1
      and set (aa1: obj) (value: int) =
        i <- i + 1
        j <- j + 1
        printfn $"type extension aa1 {aa1} <- {value}!\t%03i{i}\t%03i{j}"
let t = T()
t.indexed1 ["ok"] <- 1           // calls the intrinsic property    
t.indexed1 ("ok") <- 2           // calls the intrinsic property    
t.indexed1 "ok" <- 3             // calls the intrinsic property    
t.indexed1 "ok"                  // calls the intrinsic property
t.get_indexed1 "ok"              // calls the intrinsic property
t.set_indexed1 (a1="ok",value=1) // calls the intrinsic property

open Extensions

t.indexed1 "nok"        // calls the intrinsic property?
t.indexed1 (a1="ok")    // calls the intrinsic property
t.indexed1 (aa1="ok")   // calls the type extension property
t.indexed1 ["nok"] <- 1 // calls the intrinsic property?
t.indexed1 ("nok") <- 2 // calls the intrinsic property?
t.indexed1 "nok" <- 3   // calls the intrinsic property?

t.get_indexed1 ("nok") // calls the intrinsic property?
t.get_indexed1 ["nok"] // calls the intrinsic property?
t.get_indexed1 "nok"   // calls the intrinsic property?

t.set_indexed1 ("nok_015",1)          // calls the intrinsic property?
t.set_indexed1 ("nok_016",value=2)    // calls the intrinsic property?
t.set_indexed1 (a1="ok_017",value=1)  // calls the intrinsic property
t.set_indexed1 (aa1="ok_018",value=1) // calls the type extension property
