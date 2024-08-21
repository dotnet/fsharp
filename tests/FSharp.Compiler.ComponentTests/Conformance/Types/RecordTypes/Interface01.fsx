// #Conformance #TypesAndModules #Records 
// Record type may implement interfaces
// In this case, the interface with one member
#light

type I = interface
            abstract member M : int -> int
         end

type T1 = { A : int }
          interface I with
            member x.M(z) = z + 1

let t1 = { A = 10 }

let v = (t1 :> I).M(t1.A)

(if v = 11 then 0 else failwith "Failed")
