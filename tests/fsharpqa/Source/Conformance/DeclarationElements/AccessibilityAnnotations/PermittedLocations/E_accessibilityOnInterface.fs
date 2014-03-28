// #Regression #Conformance #DeclarationElements #Accessibility 
#light

//<Expects Status=error>\(17,14-18,12\): error FS0191: abstract slots always have the same visibility as the enclosing type</Expects>
//<Expects Status=error>\(18,14-19,13\): error FS0191: abstract slots always have the same visibility as the enclosing type</Expects>
//<Expects Status=error>\(19,14-20,13\): error FS0191: abstract slots always have the same visibility as the enclosing type</Expects>
//<Expects Status=error>\(20,14-20,20\): error FS0010: unexpected keyword 'public' in member definition. Expected identifier, '\(', '\(\*\)' or other token</Expects>
//<Expects Status=error>\(21,5-22,13\): error FS0191: abstract slots always have the same visibility as the enclosing type</Expects>
//<Expects Status=error>\(22,14-22,22\): error FS0010: unexpected keyword 'internal' in member definition. Expected identifier, '\(', '\(\*\)' or other token</Expects>

type public IDoStuffAsWell =
    abstract SomeStuff : int -> unit
    
type internal IMightDoStuffAsWell =
    abstract SomeStuff : int -> unit

type private IDoStuff =
    public   abstract SomeStuffa1 : int -> int -> (int * int)
    private  abstract SomeStuffa2 : int -> int -> (int * int)
    internal abstract SomeStuffa3 : int -> int -> (int * int)
    abstract public   SomeStuffb1 : int -> int -> (int * int)
    abstract private  SomeStuffb2 : int -> int -> (int * int)
    abstract internal SomeStuffb3 : int -> int -> (int * int)
    
