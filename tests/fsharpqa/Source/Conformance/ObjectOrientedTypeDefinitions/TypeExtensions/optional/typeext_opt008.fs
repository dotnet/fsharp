// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions  
// Regression test for FSHARP1.0:3593
// "Prefer extension members that have been brought into scope by more recent "open" statements"
//<Expects status="success"></Expects>
namespace NS
  module M = 
    type Lib with
        // Extension Methods
        member x.ExtensionMember () = 1

  module N =
    type Lib with
        // Extension Methods
        member x.ExtensionMember () = 2
  
  module F =
    open M
    open N    // <-- last open
  
    let a = new Lib()
    let b = a.ExtensionMember()   // <- this is no longer ambiguous because of the new rule (prefer extension members that have been brought into scope by more recent "open" statements)   

    (if b = 2 then 0 else 1) |> exit

// Overload resolution rules (see FSHARP1.0:3593)
// 1. Prefer methods whose use does not gives rise to the "this code is less generic because a type variable has been instantiated" warning
// 2. Otherwise, prefer methods that don't use ParamArray arg conversion
// 3. Otherwise,  prefer methods with a more precise param array argument type prior to inference (if two methods do use ParamArray arg conversion)
// 4. Otherwise, prefer methods the don't use out args
// 5. Otherwise, prefer methods that don't use optional arguments
// 6. Otherwise, prefer methods whose argument types prior to inference are all at least as precise and in some way more precise 
// 7. Otherwise, prefer non-extension members
//7b. Otherwise, prefer extension members that have been brought into scope by more recent "open" statements (including AutoOpen statements implicit from assembly references)
// 8. Otherwise, prefer non-generic methods

