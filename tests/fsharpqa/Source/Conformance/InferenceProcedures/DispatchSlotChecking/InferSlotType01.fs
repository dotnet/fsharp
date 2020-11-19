// #Conformance #TypeInference 
#light

// Leave parameters generic, verify ability to infer which dispatch slot member corresponds to

type IFoo =
    abstract DoStuff : int    -> int

type IBar =
    abstract DoStuff : string -> int

type Test() =
    interface IFoo with
         member this.DoStuff x = 1
    interface IBar with
         member this.DoStuff x = 2

let t = new Test()

if (t :> IFoo).DoStuff 1  <> 1 then exit 1
if (t :> IBar).DoStuff "" <> 2 then exit 1

exit 0
