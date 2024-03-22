// See https://github.com/dotnet/fsharp/issues/13710
//
// Note FSharp.Data 5.0.1 is split into two packages, where the type providers depend on FSharp.Data.Core
//
// The TypeProviderCOnfig reported to the type providers must contain both FSharp.Data and FSharp.Data.Core.
#r "nuget: FSharp.Data, 5.0.1"

open FSharp.Data
type Auth = JsonProvider<"""{ "test": 1 }""">
let auth = Auth.Parse("""{ "test": 1 }""")
printfn $"{auth.Test}"

// This is a compilation test, not a lot actually happens in the test
do (System.Console.Out.WriteLine "Test Passed"; 
    System.IO.File.WriteAllText("test.ok", "ok"); 
    exit 0)

