module OutOptionalTests
open System.Runtime.InteropServices

type Thing =
    static member Do(o: outref<int>, [<Optional; DefaultParameterValue(1)>]i: int) = 
        o <- i
        i = 7
let (_:bool), (_:int) = Thing.Do(i = 42)
let (_:bool), (_:int) = Thing.Do()
