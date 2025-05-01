// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

// Testing instance methods on record types
type Person = 
    {
        Name : string; 
        DateOfBirth : System.DateTime; 
    }
    member this.DoStuff param1 param2 = param1 + param2 + 42
    override this.ToString() = this.Name + "+DOB"

let randomPerson = { Name="Chris"; DateOfBirth=System.DateTime.Now }
if randomPerson.DoStuff 1 2 <> 45 then exit 1
if randomPerson.ToString() <> "Chris+DOB" then failwith "Failed: 1"
