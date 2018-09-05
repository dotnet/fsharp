// #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// https://github.com/Microsoft/visualfsharp/issues/351 - slow overlaod resolution
//<Expects id="FS0001" status="error">No overloads match</Expects>
type Switcher = Switcher

let inline checker< ^s, ^r when (^s or ^r) : (static member pass : ^r -> unit)> (s : ^s) (r : ^r) = ()

let inline format () : ^r =
    checker Switcher Unchecked.defaultof< ^r>
    () :> obj :?> ^r

type Switcher with
    static member inline pass(_ : string -> ^r) =
        checker Switcher Unchecked.defaultof< ^r>
    static member inline pass(_ : int -> ^r) =
        checker Switcher Unchecked.defaultof< ^r>
    static member inline pass(_ : unit) = ()
    static member inline pass(_ : int) = ()

[<EntryPoint>]
let main argv = 
    let res : unit = format () "text" 5 "more text" ()
    printfn "%A" res
    System.Console.ReadKey()
    0 // return an integer exit code