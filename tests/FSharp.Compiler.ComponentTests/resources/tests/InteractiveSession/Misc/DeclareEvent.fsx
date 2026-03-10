// #NoMT #FSI 
//<Expects status="success">EventName: Event</Expects>

type T() =
    do 
        printfn "Executing .ctor"
        printfn "EventName: %s" (typeof<T>.GetEvents().[0].Name)
        ()

    [<CLIEvent>]
    member x.Event = Event<int>().Publish;;

let test = new T();;


#q;;
