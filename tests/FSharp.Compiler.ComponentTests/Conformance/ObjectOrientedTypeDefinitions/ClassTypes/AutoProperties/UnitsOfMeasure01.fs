// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #UnitsOfMeasure

[<Measure>] type MyM
type T() =  
    member val P = 0<MyM> with get,set

let t = T()
t.P <- 3<MyM>
if t.P <> 3<MyM> then exit 1
