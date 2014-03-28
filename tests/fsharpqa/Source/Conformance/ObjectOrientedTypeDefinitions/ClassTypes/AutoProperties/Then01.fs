// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties
// modify property in a then
type T(x,y) =
    let mutable field = 0
    member val Property = field with get,set
    member val OtherProperty = "" with get,set
    new(x) as this = 
        T(x,0)
        then 
            this.Property <- 5
            this.OtherProperty <- "hi"

let x = T(2)
if x.Property <> 5 then exit 1
if x.OtherProperty <> "hi" then exit 1

exit 0
