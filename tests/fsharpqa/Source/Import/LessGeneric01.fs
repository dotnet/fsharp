// #Regression #NoMT #Import 

namespace NS

// Regression test for FSHARP1.0:3824 - Problems with generic type parameters in type extensions (was: Confusing error/warning on type extension: code is less generic)
//<Expects id="FS0064" span="(13,54-13,57)" status="warning">This construct causes code to be less generic than indicated</Expects>
//<Expects id="FS0660" span="(10,17-10,19)" status="error">This code is less generic than required by its annotations</Expects>

  module N =
    type LibGen<'a> with
          member x.ExtensionProperty004 
            with get () = x.instanceField
            and set (inp : int) = x.instanceField <- inp
