// #Conformance #ObjectOrientedTypes #TypeExtensions 
// This test used to be about "verifying that No two intrinsic extensions may contain 
// conflicting members.
// After recent changes, DoStuff are no longed intrinsic members; they are instead
// extension methods... so this code compiles just fine.
//<Expects status="success"></Expects>

#light
namespace NS
  module K = 

    // Define Foo
    type Foo() =
         class
         end

  module L = 
    open K
    // Extend Foo
    type Foo with
        static member DoStuff = 1


  module M = 
    open K
    // Extend Foo
    type Foo with
        static member DoStuff = 2

  module N = 
    exit 1
