module Test

[<RequireQualifiedAccess>]
type DU =
     | an of int
     | B of string
     | C
     | ``D`` of bool
     | ``d``


     static do printfn "I am here"

     static let cachedVals = [| DU.an 42; DU.``d`` |]

     static member GetValsAsString() = sprintf "%A" cachedVals