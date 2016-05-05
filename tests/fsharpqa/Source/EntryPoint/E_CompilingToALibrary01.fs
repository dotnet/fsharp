// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:4277
//<Expects id="FS0433" span="(10,9-10,13)" status="error">A function labeled with the 'EntryPointAttribute' attribute must be the last declaration in the last file in the compilation sequence.</Expects>

// Compile with -a

namespace N
module Entry =
    [<EntryPoint>]
    let Main (args:string[]) =
        0
