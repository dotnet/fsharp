// #Conformance #TypeConstraints 
#light

// Test Microsoft.FSharp.Reflection.IsRecord function works properly on various types

open Microsoft.FSharp.Reflection

// Record type
type Employee =
  { Name : string;
    DateOfBirth : System.DateTime;
    SSN : string;
    YearsOfService : int; }

// Type abbreviation
type SalesPerson = Employee

// Union type
type Color = White | Black | Gray
type FTE = | Employee

// Enum type
type NonStandardType =
    | Record    = 0
    | DiscUnion = 1
    | Tuple     = 2
    | Function  = 3

// Check if
let isOK = 
    not (FSharpType.IsRecord ( typeof<Color> )) &&                  // Color is not record
    not (FSharpType.IsRecord ( typeof<FTE> )) &&                    // Union type is not record
    not (FSharpType.IsRecord ( typeof<NonStandardType> )) &&        // Enum type is not record
    not (FSharpType.IsRecord ( typeof<Employee -> Employee> )) &&   // Function from Record type to Record type is not record
    not (FSharpType.IsRecord ( typeof<Employee * Employee> )) &&    // Tuple of Records is not record
    not (FSharpType.IsRecord ( typeof<System.Object> )) &&          // Object is not record
    not (FSharpType.IsRecord ( typeof<System.ValueType> )) &&       // ValueType is not record
         
         FSharpType.IsRecord ( typeof<Employee> ) &&                // Employee is record
         FSharpType.IsRecord ( typeof<SalesPerson> )                // Abbreviated SalesPerson type is record
         
if not isOK then exit 1

exit 0
