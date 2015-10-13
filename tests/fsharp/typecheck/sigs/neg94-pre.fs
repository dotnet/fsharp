namespace Neg94Pre

open System

type Class1() =
  static member inline ($) (r:'R, _) = fun (x:'T) -> ((^R) : (static member method2: ^T -> ^R) x)
  static member inline method1 x = Unchecked.defaultof<'r> $ Class1()

type IComm = 
    [<CLIEvent>]
    abstract CanExecuteChanged : IEvent<EventHandler,EventArgs> 

type Interface2<'T> =
    inherit IComm

type Class<'T>() =
    interface Interface2<'T> with
        [<CLIEvent>]
        member __.CanExecuteChanged : IEvent<EventHandler,EventArgs> = Event<EventHandler,EventArgs>().Publish
