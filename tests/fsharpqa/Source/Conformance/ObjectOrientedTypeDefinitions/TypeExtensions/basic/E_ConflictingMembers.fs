// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
// Verify that No two intrinsic extensions may contain conflicting members
// See also FSHARP1.0:4925 (this test will have to be updated once that bug is resolved)
//<Expects id="FS0438" span="(12,27-12,34)" status="error">Duplicate method\. The method 'get_DoStuff' has the same name and signature as another method in type 'K.Foo'</Expects>

namespace NS
  module K = 

    // Define Foo
    type Foo() =
         class
            static member DoStuff = 1
         end
    
    type Foo with
         static member DoStuff = 2
