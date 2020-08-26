// @@@LearnMore|Learn more about F# at http://docs.microsoft.com/dotnet/fsharp@@@
// @@@SeeTutorial|See the 'F# Tutorial' project for more help.@@@

// Define a function to construct a message to print
let from whom =
    sprintf "from %s" whom

[<EntryPoint>]
let main argv =
    let message = from "F#" // Call the function
    printfn "Hello world %s" message
    0 // @@@Return|return an integer exit code@@@
