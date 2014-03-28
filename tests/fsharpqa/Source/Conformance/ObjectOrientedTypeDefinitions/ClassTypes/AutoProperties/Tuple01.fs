// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties
type T() =  
    member val P = (0,0) with get,set

let t = T()
t.P <- (3,3)
if t.P <> (3,3) then exit 1
