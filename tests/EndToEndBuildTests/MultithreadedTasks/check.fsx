open System
open System.IO
open System.Threading

let marker = File.ReadAllText("sentinel.txt")
for variable in [ "FSHARP_MT_MARKER"; "FSHARP_MT_TOOL_MARKER" ] do
    if Environment.GetEnvironmentVariable variable <> marker then
        failwithf "%s did not reach the Fsi child for %s" variable marker

// Four live Fsi processes must enter before any can leave.
Directory.CreateDirectory("../barrier") |> ignore
File.WriteAllText("../barrier/" + marker, string Environment.ProcessId)
let deadline = DateTime.UtcNow.AddSeconds 90
while Directory.GetFiles("../barrier").Length < 4 do
    if DateTime.UtcNow > deadline then failwith "Fsi project concurrency barrier timed out"
    Thread.Sleep 50
File.WriteAllText("fsi.txt", marker)
