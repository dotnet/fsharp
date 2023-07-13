// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:4277

// Compile with -a

namespace N
module Entry =
    [<EntryPoint>]
    let Main (args:string[]) =
        0
