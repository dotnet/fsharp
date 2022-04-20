// #Regression #Conformance #DeclarationElements #Accessibility 
//
//<Expects status="error" span="(15,14-15,21)" id="FS0010">Unexpected keyword 'private' in member definition\. Expected identifier, '\(', '\(\*\)' or other token\.$</Expects>
//

#light

type public IDoStuffAsWell =
    abstract SomeStuff : int -> unit
    
type internal IMightDoStuffAsWell =
    abstract SomeStuff : int -> unit

type private IDoStuff =
    abstract private   SomeStuffb1 : int -> int -> (int * int)
