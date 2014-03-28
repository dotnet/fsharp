// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties
type AA = System.Nullable<int>
type T() =  
    member val P = AA(1) with get,set

let t = T()
t.P <- AA(3)
if t.P <> AA(3) then exit 1
