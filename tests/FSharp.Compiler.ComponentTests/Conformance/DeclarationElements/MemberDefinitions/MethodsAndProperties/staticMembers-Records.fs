// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

// Testing instance methods on record types
type Person = 
    {
        Name : string; 
        DateOfBirth : System.DateTime; 
    }
    static member SomeStaticMethod (param1, param2) = "PersonRecordType"

let p = {Name = "John"; DateOfBirth = System.DateTime.Now }

if Person.SomeStaticMethod (None, []) <> "PersonRecordType" then failwith "Failed: 1"

