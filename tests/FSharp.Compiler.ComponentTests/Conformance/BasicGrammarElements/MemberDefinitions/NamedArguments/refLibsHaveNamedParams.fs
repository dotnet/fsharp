// #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
#light

// Test that all types exposed in the base class library have 'named params'
let sl = new System.Collections.Specialized.StringCollection()
sl.Add(value="something")

if System.Boolean.Parse(value="false") <> false then failwith "Failed: 1"

// Leap year baby!
if System.DateTime.DaysInMonth(month=2, year=2008) <> 29 then failwith "Failed: 2"
