// #Regression #Conformance #ObjectOrientedTypes #Enums 
#light

// Verify that an Enum brings a named type into scope
//<Expects id="FS0037" status="error">Duplicate.*EnumType</Expects>
namespace NS
  module Test = 
    type EnumType =
        static member A = 0

    type EnumType = 
        | A = 0
    
    
  exit 1
