// #Regression #Conformance #DeclarationElements #Accessibility 
//
//<Expects status="error" id="FS0561" span="(15,5-15,69)">Accessibility modifiers are not allowed on this member\. Abstract slots always have the same visibility as the enclosing type</Expects>
//

#light

type public IDoStuffAsWell =
    abstract SomeStuff : int -> unit
    
type internal IMightDoStuffAsWell =
    abstract SomeStuff : int -> unit

type private IDoStuff =
    internal abstract member SomeStuffa1 : int -> int -> (int * int)
