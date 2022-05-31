// #Regression #Conformance #DeclarationElements #Events 
// Regression for 5917
// Used to throw an internal compiler error on the type declarations

module M

type IParam<'a> =
  [<CLIEvent>]
  abstract ValueChanged : IEvent<unit>
  
type Param<'a>() = 
  interface IParam<'a> with
    [<CLIEvent>]
    member x.ValueChanged = Unchecked.defaultof<IEvent<unit>>

let x = Param<unit>()
let y = Param<int>()
let z = Param<string>()
