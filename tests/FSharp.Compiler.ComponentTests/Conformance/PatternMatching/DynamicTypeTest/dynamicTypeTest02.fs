// #Conformance #PatternMatching #TypeTests 
#light

open System

type Shape() =
    abstract VirtMember : unit -> string
    default this.VirtMember () = "shape"
    member this.StaticMember() = "shape"

type Ellipse() =
    inherit Shape()
    override this.VirtMember() = "ellipse"
    member this.StaticMember() = "ellipse"
    
type Circle() =
    inherit Shape()
    override this.VirtMember() = "circle"
    member this.StaticMember() = "circle"

let test1 (x:obj) = 
    match x with
    | :? Circle  -> 1
    | :? Ellipse -> 2
    | :? Shape   -> 3
    | _          -> 4

let test2 (x:obj) = 
    match x with
    | :? Shape   -> 1
    | :? Ellipse -> 2
    | :? Circle  -> 3
    | _          -> 4

if test1 (new Circle()  :> obj) <> 1 then exit 1
if test1 (new Ellipse() :> obj) <> 2 then exit 1
if test1 (new Shape()   :> obj) <> 3 then exit 1
if test1 ("foo"         :> obj) <> 4 then exit 1    

if test2 (new Circle()  :> obj) <> 1 then exit 1
if test2 (new Ellipse() :> obj) <> 1 then exit 1
if test2 (new Shape()   :> obj) <> 1 then exit 1
if test2 ("foo"         :> obj) <> 4 then exit 1   

exit 0
