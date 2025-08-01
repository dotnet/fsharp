module BackwardCompatTest
        
let makeCustomAttributes() =
    [|
        if true then
            yield "attr1"
            yield! ["attr2"; "attr3"]
        yield! ["attr4"]
    |]

let attrs = makeCustomAttributes()
let expected = [|"attr1"; "attr2"; "attr3"; "attr4"|]

if attrs <> expected then failwithf $"attrs failed: got {attrs}"

printfn "Backward compatibility test passed!"