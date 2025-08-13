// Expected: No warning - constructs in quotations are data
module Module

open FSharp.Quotations

type MyClass() =
    member _.GetQuotation() =
        <@
            type T = int
            module M =
                let x = 1
        @>
    
    member _.GetQuotation2() =
        <@@
            exception E of string
            open System
        @@>
