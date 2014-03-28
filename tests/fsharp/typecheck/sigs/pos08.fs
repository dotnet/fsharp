module Pos08

module Tests2 = 
    let f2 () = 
       lazy 
         (let x = 1    // expect no error
          x + x)

    let f3 () = 
       assert 
         (let x = 1    // expect no error
          (x + x = x + x))
       1+2

    let f21 () = 
       lazy 
          (match () with () -> 1) // expect no error

    let f31 () = 
       assert 
          (match () with () -> true) // expect no error
       1+2

    let f22 () = 
       lazy 
          (while false do ())  // expect no error

    let f32 () = 
       assert 
          (while false do ()  // expect no error
           true)
       1+2

    let f23 () = 
       lazy 
         (try 1 with _ -> 2)  // expect no error

    let f33 () = 
       assert 
         (try true with _ -> false)  // expect no error
       1+2


    let f24 () = 
       lazy 
          (if true then 1 else 2)  // expect no error

    let f34 () = 
       assert 
          (if true then true else false)  // expect no error
       1+2


    let f25 () = 
       lazy 
          (for i in 0 .. 0 do ())  // expect no error

    let f35 () = 
       assert 
          (for i in 0 .. 0 do ()// expect no error
           true)
       1+2

    let f26 () = 
       lazy 1 // expect no error

    let f36 () = 
       assert true // expect no error
       1+2


    let f27 () = 
       lazy id 1 // expect no error

    let f37 () = 
       assert id true // expect no error
       1+2

module Tests3 = 
    let f2 () = 
       lazy 
          let x = 1    // expect no error
          x + x

    let f3 () = 
       assert 
          let x = 1    // expect no error
          (x + x = x + x)
       1+2

    let f21 () = 
       lazy 
          match () with () -> 1 // expect no error

    let f31 () = 
       assert 
          match () with () -> true // expect no error
       1+2

    let f22 () = 
       lazy 
          while false do ()  // expect no error

    let f32 () = 
       assert 
          while false do ()  // expect no error
          true 
       1+2

    let f23 () = 
       lazy 
          try 1 with _ -> 2  // expect no error

    let f33 () = 
       assert 
          try true with _ -> false  // expect no error
       1+2


    let f24 () = 
       lazy 
          if true then 1 else 2  // expect no error

    let f34 () = 
       assert 
          if true then true else false  // expect no error
       1+2


    let f25 () = 
       lazy 
          for i in 0 .. 0 do ()  // expect no error

    let f35 () = 
       assert 
          for i in 0 .. 0 do ()// expect no error
          true 
       1+2

    let gf21 () = 
       lazy match () with () -> 1 // expect no error

    let gf31 () = 
       assert match () with () -> true // expect no error
       1+2

    let gf22 () = 
       lazy while false do ()  // expect no error

    let gf32 () = 
       assert while false do ()  // expect no error
              true 
       1+2

    let gf23 () = 
       lazy try 1 with _ -> 2  // expect no error

    let gf33 () = 
       assert try true with _ -> false  // expect no error
       1+2


    let gf24 () = 
       lazy if true then 1 else 2  // expect no error

    let gf34 () = 
       assert if true then true else false  // expect no error
       1+2


    let gf25 () = 
       lazy for i in 0 .. 0 do ()  // expect no error

    let gf35 () = 
       assert for i in 0 .. 0 do ()// expect no error
              true 
       1+2

    let gf26 () = 
       lazy let x = 1    // expect no error
            x + x

    let gf36 () = 
       assert let x = 1  // expect no error
              x = x 
       1+2

module NonControlFlowTests = 
    let (a: Lazy<int>), (b: int) = lazy 1, 2
    
    let (++) (a:Lazy<int>) (b:Lazy<int>) = a.Force() + b.Force()
    let (b2: int) = lazy 1 ++ lazy 2
    
    let (a3: Lazy<int>), (b3: int) = 
        lazy 1, 
        2
    
module CheckOverloadResolutionAgainstSignatureInformationGivenByPatterns = 

    module Positive1 = 
        type R1 = { f1 : int }
        type R2 = { f2 : int }
        type D() = 
            member x.N = x.M { f1 = 3 } // Expect no error
            member x.M(y: R1) = ()
            member x.M(y: R2) = ()

    module Positive2 = 
        type R1 = { f1 : int }
        type R2 = { f2 : int }
        type D() = 
            member x.N = x.M { f1 = 3 } // Expect no error
            member x.M((y: R1)) = ()
            member x.M((y: R2)) = ()
        
    module Positive3 = 
        type R1 = { f1 : int }
        type R2 = { f2 : int }
        type D() = 
            member x.N = x.M { f1 = 3 } // Expect no error
            member x.M(_ as p : R2) = ()
            member x.M(_ as p : R1) = ()

    module Positive4 = 
        type R1 = { f1 : int }
        type R2 = { f2 : int }
        let (|Fail|_|) (x : 'a) : 'a option= None
        type D() = 
            member x.N = x.M { f1 = 3 } // Expect no error
            member x.M(Fail(_) as p : R2) = ()
            member x.M(Fail(_) as p : R1) = ()

    module Positive5 = 
        type R1 = { f1 : int }
        type R2 = { f2 : int }
        type D() = 
            member x.N = x.M { f1 = 3 } // Expect no error
            member x.M((y: R1)) = ()
            member x.M(()) = ()
        
module CheckInitializationGraphInStaticMembers = 

    module Positive6 = 
        type C() = 
           static let rec x = (); (fun () -> ignore x; ())        // expect warning, no runtime error
           static member A = x
           
        C.A () // expect no runtime error