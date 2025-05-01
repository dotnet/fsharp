// #Regression #Conformance #DeclarationElements #Accessibility 
//
//<Expects status="error" span="(15,14-15,20)" id="FS0010">Unexpected keyword 'public' in member definition\. Expected identifier, '\(', '\(\*\)' or other token\.$</Expects>
//



type public IDoStuffAsWell =
    abstract SomeStuff : int -> unit
    
type internal IMightDoStuffAsWell =
    abstract SomeStuff : int -> unit

type private IDoStuff =
    abstract public   SomeStuffb1 : int -> int -> (int * int)
