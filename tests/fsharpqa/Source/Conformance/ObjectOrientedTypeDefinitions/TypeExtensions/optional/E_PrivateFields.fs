// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions  
//verify that private fields cannot be accessed by optional extension members
//<Expects id="FS1096" span="(24,27-24,42)" status="error">The record, struct or class field 'instanceField' is not accessible from this code location</Expects>
//<Expects id="FS1096" span="(25,35-25,50)" status="error">The record, struct or class field 'instanceField' is not accessible from this code location</Expects>
//<Expects id="FS1096" span="(28,27-28,42)" status="error">The record, struct or class field 'staticField' is not accessible from this code location</Expects>
//<Expects id="FS1096" span="(29,35-29,50)" status="error">The record, struct or class field 'staticField' is not accessible from this code location</Expects>
//<Expects id="FS1096" span="(35,27-35,42)" status="error">The record, struct or class field 'instanceField' is not accessible from this code location</Expects>
//<Expects id="FS1096" span="(36,34-36,49)" status="error">The record, struct or class field 'instanceField' is not accessible from this code location</Expects>
//<Expects id="FS1096" span="(39,27-39,42)" status="error">The record, struct or class field 'staticField' is not accessible from this code location</Expects>
//<Expects id="FS1096" span="(40,34-40,49)" status="error">The record, struct or class field 'staticField' is not accessible from this code location</Expects>
//<Expects id="FS0064" span="(40,53-40,56)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'</Expects>
//<Expects id="FS0660" span="(33,17-33,19)" status="error">This code is less generic than required by its annotations because the explicit type variable 'a' could not be generalized\. It was constrained to be 'int'</Expects>

namespace NS
  module M = 
    type Lib with
    
    // Extension Methods
          member x.ExtensionMember () = 1
          static member StaticExtensionMember() =1
          
    // Extension Properties
          member x.ExtensionProperty004 
            with get () = x.instanceField
            and set (inp : int) = x.instanceField <- inp             

          static member StaticExtensionProperty004 
            with get () = Lib.staticField
            and set (inp : int) = Lib.staticField <- inp             


  module N =
    type LibGen<'a> with
          member x.ExtensionProperty004 
            with get () = x.instanceField
            and set (inp : 'a) = x.instanceField <- inp

          static member StaticExtensionProperty004 
            with get () = Lib.staticField
            and set (inp : 'a) = Lib.staticField <- inp
