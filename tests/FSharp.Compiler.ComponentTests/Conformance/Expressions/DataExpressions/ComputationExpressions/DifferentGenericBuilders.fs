// #Conformance #DataExpressions #ComputationExpressions 
// Verify you can customize the behavior of a workflow builder
// through the use of generics

type WorkflowBuilder<'a>() =
    member this.Return(x) = x + typeof<'a>.Name
    
    
let wf1 = new WorkflowBuilder<int>()
let result1 = wf1 { return "test1:" }

if result1 <> "test1:Int32" then exit 1

let wf2 = new WorkflowBuilder<string>()
let result2 = wf2 { return "test2:" }

if result2 <> "test2:String" then exit 1

exit 0

