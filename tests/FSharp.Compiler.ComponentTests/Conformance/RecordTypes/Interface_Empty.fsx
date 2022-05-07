// #Conformance #TypesAndModules #Records 
// Record type may implement interfaces
// In this case, the interface has no member
#light

type I = interface
         end

type T1 = { A : int }
          with interface I

let t1 = { A = 10 }

let i = t1 :> I         // ok!
