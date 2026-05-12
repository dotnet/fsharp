// #Regression #Misc 
// This used to be a warning, now it's an error
//<Expects status="error" span="(10,18-10,25)" id="FS0640">A parameter with attributes must also be given a name, e\.g\. '\[<Attribute>\] Name : Type'$</Expects>

type Doc() = 
    inherit System.Attribute()

type IFoo = 
    interface
        abstract Method1 : [<Doc>] int * [<Doc>] unit -> unit
        abstract Method2 : [<Doc>] p1:int * [<Doc>] p2:unit -> unit
        abstract Method3 : [<Doc>] ?p1:int -> unit
        abstract Method4 : ?p1:int -> unit
    end

exit 1
