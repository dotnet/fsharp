// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #TypeExtensions

module M =
    type T() =     
        let mutable x = 0
        member this.NormalProperty with get() = x and set(v) = x <- v
        member this.DoStuff() = ()

module M2 =
    open M
    type T() =
        member val AutoProp = "" with get,set

module M3 =
    open M
    open M2
    let t = T()
    t.AutoProp <- "hi"
    if t.AutoProp <> "hi" then exit 1

exit 0