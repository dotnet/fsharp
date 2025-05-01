// #Conformance #TypesAndModules #Unions 
// DU may include implement interfaces
//<Expects status="success"></Expects>
#light

type I = interface
            abstract member S : unit -> bool
         end          

type T1 = | C of int * int
          | D of (int * int)
          interface I with
            override x.S() = match x with
                             | C(a,_) -> true
                             | _ -> false

                
let d = C(1,2)
let i = d :> I

if i.S() then 0 else failwith "Failed: 1"
