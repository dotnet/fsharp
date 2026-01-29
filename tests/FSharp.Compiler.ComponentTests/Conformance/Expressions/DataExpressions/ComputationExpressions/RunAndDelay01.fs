// #Conformance #DataExpressions #ComputationExpressions 
// Verify the startup semantics of Run and Delay based on whether or not they
// are present.

type RunDelayWorkflow() =
    member this.Run(x) =
        "RUN-" + x + "-RUN"
        
    member this.Delay(f) = 
        "DELAY-" + f() + "-DELAY"
        
    member this.Return(x) =
        "RET-" + x + "-RET"
        
let test1 = (new RunDelayWorkflow()) { return "1" }
if test1 <> "RUN-DELAY-RET-1-RET-DELAY-RUN" then exit 1

////////////////////////////////////////////////////

type DelayWorkflow() =
        
    member this.Delay(f) = 
        "DELAY-" + f() + "-DELAY"
        
    member this.Return(x) =
        "RET-" + x + "-RET"
        
let test2 = (new DelayWorkflow()) { return "2" }
if test2 <> "DELAY-RET-2-RET-DELAY" then exit 1

////////////////////////////////////////////////////

type RunWorkflow() =
        
    member this.Run(x) = 
        "RUN-" + x + "-RUN"
        
    member this.Return(x) =
        "RET-" + x + "-RET"
        
let test3 = (new RunWorkflow()) { return "3" }
if test3 <> "RUN-RET-3-RET-RUN" then exit 1

////////////////////////////////////////////////////

type NeitherWorkflow() =
        
    member this.Return(x) =
        "RET-" + x + "-RET"
        
let test4 = (new NeitherWorkflow()) { return "4" }
if test4 <> "RET-4-RET" then exit 1

////////////////////////////////////////////////////

exit 0
