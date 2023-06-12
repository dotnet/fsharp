// #Regression #Conformance #DeclarationElements #Accessibility 
//
//<Expects status="error" span="(13,5-13,67)" id="FS0561">Accessibility modifiers are not allowed on this member\. Abstract slots always have the same visibility as the enclosing type</Expects>
//

type public IDoStuffAsWell =
    abstract SomeStuff : int -> unit
    
type internal IMightDoStuffAsWell =
    abstract SomeStuff : int -> unit

type private IDoStuff =
    public abstract member SomeStuffa1 : int -> int -> (int * int)
