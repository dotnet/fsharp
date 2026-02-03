// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties
type T() =  
    member val P = 0 with get,set

let t = T()
t.P <- 3
if t.P <> 3 then exit 1

type T2() =  
    member val P = 0.0M with get,set

let t2 = T2()
t2.P <- 3.0M
if t2.P <> 3.0M then exit 1
