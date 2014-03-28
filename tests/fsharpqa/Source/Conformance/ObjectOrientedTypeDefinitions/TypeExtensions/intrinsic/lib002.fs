// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
// verify that over-riding a virtual method of a type defined in another assembly gives an error.
//<Expects id="FS0854" span="(8,20-8,25)" status="error">Method overrides and interface implementations are not permitted here</Expects>
#light
namespace NS
    [<AbstractClass>]
    type Lib() =
      class
        abstract M : int -> int
     end

