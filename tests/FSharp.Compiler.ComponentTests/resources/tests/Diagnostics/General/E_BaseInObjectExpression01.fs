// #Regression #Diagnostics 
// Regression test for FSHARP1.0:3491
//<Expects id="FS0564" span="(6,26-6,33)" status="error">'inherit' declarations cannot have 'as' bindings\. To access members of the base class when overriding a method, the syntax 'base\.SomeMember' may be used; 'base' is a keyword\. Remove this 'as' binding\.$</Expects>

let obj3 =
   { new System.Object() as base with
         member x.ToString () = "Hello, base.ToString() = " + base.ToString() } 

