// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties
type T() =  
    member val P = [||] with get,set

let t = T()
t.P <- [|1|]
if t.P <> [|1|] then printfn "array failed"; exit 1
