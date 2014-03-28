// #Conformance #ObjectOrientedTypes #TypeExtensions 
//verify that you can extend a type multiple times
#light
namespace NS
  module M = 

    // Define Foo
    type Foo() =
         class
         end

    type Foo<'a> () =
         class
         end

    // Extend Foo
    type Foo with
        static member DoStuff1 = 1
    type Foo<'a> with
        member x.DoStuff1 = 11

    // Extend Foo again
    type Foo with
        static member DoStuff2 = 2
    type Foo<'a> with
        member x.DoStuff2 = x.DoStuff1 + 11
        


  module N = 
    open M
    let mutable res = true
    let a = new Foo<int> ()
  
    if not (Foo.DoStuff1 = 1) then
      printf "Foo.DoStuff1 failed\n"
      res <- false
    
    if not (Foo.DoStuff2 = 2) then
      printf "Foo.DoStuff2 failed\n"
      res <- false
    
    if not (a.DoStuff1 = 11) then
      printf "Foo<int,char>.DoStuff1 failed\n"
      res <- false
      
    if not (a.DoStuff2 = 22) then
      printf "Foo<int,char>.DoStuff2 failed\n"
      res <- false

    (if (res) then 0 else 1) |> exit
