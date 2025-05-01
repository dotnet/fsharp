// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSHARP1.0:1708
// internal compiler error for free type variable in attribute
//<Expects status="notin">internal error</Expects>
//<Expects status="success"></Expects>

#light

open System
open System.Diagnostics

[<assembly:DebuggerVisualizer(typeof<int>,
                              typeof<int>,
                              Target = typeof<Microsoft.FSharp.Collections.List<'a> >,
                              Description = "FSharp %A Visualizer")>]
do
    let l = [1; 2]
    printfn "%A" l
