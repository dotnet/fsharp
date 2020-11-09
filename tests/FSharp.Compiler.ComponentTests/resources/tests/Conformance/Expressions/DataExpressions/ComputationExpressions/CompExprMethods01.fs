// #Conformance #DataExpressions #ComputationExpressions 
// Verify the ability to define computation expression methods and
// that they get called as part of a custom workflow builder.

// Verify Bind and Return

type BindBuilder() =

    let m_bindResults = ref []

    member this.BindResults = List.toArray (!m_bindResults)

    member this.Bind(result : string, restOfComputation : string -> int) =
        
        m_bindResults := result :: !m_bindResults
        
        // Execute the rest
        restOfComputation result
        
    member this.Return(x : int) = x

let test = new BindBuilder()

let testResult =
    test {
        let! x = "foo"
        let! y = "bar"
        
        if x = "won't match..." then
            return -1
        else
            return x.Length + y.Length
    }
    
if testResult <> 6 then 
    exit 1
    
if test.BindResults <> [| "bar"; "foo" |] then exit 1

exit 0
