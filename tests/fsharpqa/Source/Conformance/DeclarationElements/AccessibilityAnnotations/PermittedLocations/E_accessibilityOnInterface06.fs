// #Regression #Conformance #DeclarationElements #Accessibility 
//
//<Expects status="error" span="(15,14-15,22)" id="FS0010">Unexpected keyword 'internal' in member definition\. Expected identifier, '\(', '\(\*\)' or other token\.$</Expects>
//



type public IDoStuffAsWell =
    abstract SomeStuff : int -> unit
    
type internal IMightDoStuffAsWell =
    abstract SomeStuff : int -> unit

type private IDoStuff =
    abstract internal   SomeStuffb1 : int -> int -> (int * int)
