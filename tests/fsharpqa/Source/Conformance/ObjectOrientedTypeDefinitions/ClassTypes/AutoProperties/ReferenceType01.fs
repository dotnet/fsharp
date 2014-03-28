// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties
type T() =  
    member val P = "" with get,set

let t = T()
t.P <- "3"
if t.P <> "3" then exit 1
