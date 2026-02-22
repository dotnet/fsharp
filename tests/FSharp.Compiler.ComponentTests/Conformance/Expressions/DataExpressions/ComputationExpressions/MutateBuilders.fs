// #Conformance #DataExpressions #ComputationExpressions 
// Verify you can mutate the computation expression builder
// Verify you can use a builder returned from a function

type SimpleWorkflow(logMsg : string) =

    member this.Bind(result : string, restOfComputation : string -> 'a) =
        
        restOfComputation (result + "!" + logMsg + "!" + result)
        
    member this.Return x = x
    
    
let getBuilder logMsg = new SimpleWorkflow(logMsg)

// Get builder from function
let result = 
    getBuilder "Message for you sir" {
        let! x = "foo"
        let! y = "bar"
        return (x + "~" + y)
    }
    
if result <> "foo!Message for you sir!foo~bar!Message for you sir!bar" then 
    exit 1
    
// Mutate the builder
let mutable builder = getBuilder "*"
let test1 = builder { 
                let! x = "1"
                return x }

if test1 <> "1!*!1" then exit 1

builder <- getBuilder "#"

let test2 = builder { 
                let! x = "2"
                return x }

if test2 <> "2!#!2" then exit 1

exit 0
