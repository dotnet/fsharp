// #NoMT #FSI 
//<Expect status="success">EventName: Event</Expect>

type T() =
    do 
        printfn "Executing .ctor"
        printfn "EventName: %s" (typeof<T>.GetEvents().[0].Name)
        ()

    [<CLIEvent>]
    member x.Event = Event<int>().Publish;;

let test = new T();;


#q;;
