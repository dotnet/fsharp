// #Conformance #Namespaces #SignatureFiles 
namespace Hello.Goodbye

    type A = A | B | C

    module Utils = begin
      let failures = ref false
      let report_failure () = 
        stderr.WriteLine " NO"; failures := true
      let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure() 
    end

    module X  = begin
      let x = 1 

    end



namespace Hello.Beatles

    type Song = HeyJude | Yesterday

    module X  = begin
      let x = 2 
    end


namespace UseMe

    open Hello.Goodbye
    
    module Tests  = 
      Hello.Goodbye.Utils.test "test292jwe" (Hello.Goodbye.X.x + 1 = Hello.Beatles.X.x)
      Hello.Goodbye.Utils.test "test292jwe" (Hello.Beatles.HeyJude <> Hello.Beatles.Yesterday)


    module MoreTests = begin
        open global.Microsoft.FSharp.Core

        let arr1 = global.Microsoft.FSharp.Collections.Array.map (global.Microsoft.FSharp.Core.Operators.(+) 1) [| 1;2;3;4 |]

        let ``global`` = 1

        // THis should still resolve
        let arr2 = global.Microsoft.FSharp.Collections.Array.map (global.Microsoft.FSharp.Core.Operators.(+) 1) [| 1;2;3;4 |]

        let test3 : global.Microsoft.FSharp.Core.int  = 3

        let test4 : global.Microsoft.FSharp.Collections.list<int>  = [3]

        let test5 x = 
            match x with 
            | global.Microsoft.FSharp.Core.None -> 1
            | global.Microsoft.FSharp.Core.Some _ -> 1
    end


namespace global

    type A = A | B | C

    module X  = begin
      let x = 1 

    end




// Check recursive name resolution
namespace CheckRecursiveNameResolution1

    module rec Test =

      module N = 
          let x = Test.M.C()

      module M = 
          [<Sealed>]
          type C() =
             member x.P = M.C()


// Check recursive name resolution
namespace CheckRecursiveNameResolution2

    module rec Test =

      open M

      module N = 
          let x = C()

      module M = 
          [<Sealed>]
          type C() =
             member x.P = C()


// Check recursive name resolution
namespace rec CheckRecursiveNameResolution3

    module Test =

      open M

      module N = 
          let x = C()

      module M = 
          [<Sealed>]
          type C() =
             member x.P = C()


namespace rec CheckRecursiveNameResolution4

    module Test =

      open Test.M // The name Test should be in scope

      module N = 
          let x = C(4)

      module M = 
          [<Sealed>]
          type C(c:int) =
             member x.P = C(0)
             member x.V = c


      do Hello.Goodbye.Utils.test "test292jwf" (Test.N.x.V = 4)
