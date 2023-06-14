// #Regression #Conformance #DeclarationElements #Accessibility 
#light

// See bug 1537.
//<Expects id="FS0575" span="(10,21-10,41)" status="error">Accessibility modifiers are not permitted on record fields\. Use 'type R = internal \.\.\.' or 'type R = private \.\.\.' to give an accessibility to the whole representation</Expects>
//<Expects id="FS0575" span="(11,21-11,39)" status="error">Accessibility modifiers are not permitted on record fields\. Use 'type R = internal \.\.\.' or 'type R = private \.\.\.' to give an accessibility to the whole representation</Expects>
//<Expects id="FS0575" span="(12,21-12,37)" status="error">Accessibility modifiers are not permitted on record fields\. Use 'type R = internal \.\.\.' or 'type R = private \.\.\.' to give an accessibility to the whole representation</Expects>

type Person = { 
                    public Name : string; 
                    internal Age : int; 
                    private ID : int 
              }

exit 0
(*
// Bug repro steps
type Person2 = 
    {  Name : string; 
       Age : int; 
       ID : int 
    } 
    static member CreatePerson() = {Name = "bob"; Age = 10; ID = 2}
    
let t1 = {Name = "Bob"; Age = 25; ID = 0}


// Basic usage
let t1 = {Name = "Bob"; Age = 25; ID = 0}
let t2 = {Name = "Joe"; Age = 25}
(*
The record field 'ID' is not accessible from this code location.	5	10	
The record field 'ID' is not accessible from this code location.    5	10	
no assignment given for field 'ID'.		6	10	
*)

// Using a static factor on the record type itself
let p = Person.CreatePerson()
let {Name = x; Age = y; ID = z} = p
// The record field 'ID' is not accessible from this code location. 18	6	

// If we consider allowing accessibility modifers on records, then this
// should work, but does not.
let {Name = x2; Age = y2; ID = _} = p
//The record field 'ID' is not accessible from this code location.	23	6	*)
