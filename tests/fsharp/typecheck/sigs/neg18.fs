module Test

module TestSquigglyLocation = 

    type R = { Sizes : int }
    let r = { Sizes = 1 }
    Seq.max r.Sizes

module TestSquigglyExists =
    let f () = 
       match box 1 with 
       | :? list<_> -> 3  // this line should give a warning
       | _ -> 4


module AmbiguousTypeNameTests = 
    module TwoAmbiguousGenericTypes = 
        module M = 
            type C<'a>() = 
               static member P = 1

            type C<'a,'b>() = 
               static member P = 1

        let _ = new M.C() //  give error
        let _ = new M.C<int>() 
        let _ = M.C()     //  give warning
        let _ = M.C<int>()  
        let _ = M.C<int,int>() 
        let _ = M.C<_>()  
        let _ = M.C<_,_>() 

        let _ = M.C.P     //  give warning
        let _ = M.C<_>.P
        let _ = M.C<_,_>.P

        open M

        let _ = new C() //  give error
        let _ = new C<int>() 
        let _ = C()     //  give warning
        let _ = C<int>()  
        let _ = C<int,int>() 
        let _ = C<_>()  
        let _ = C<_,_>() 

        let _ = C.P     //  give warning
        let _ = C<_>.P
        let _ = C<_,_>.P
        
        // open again for good luck
        open M

        let _ = new C() //  give error
        let _ = new C<int>() 
        let _ = C()     //  give warning
        let _ = C<int>()  
        let _ = C<int,int>() 
        let _ = C<_>()  
        let _ = C<_,_>() 

        let _ = C.P     //  give warning
        let _ = C<_>.P
        let _ = C<_,_>.P
        
    module OneGenericTypeAndOneNonGenericType = 
        module M = 
            type C() = 
               static member P = 1

            type C<'a>() = 
               static member P = 1

        let _ = new M.C() 
        let _ = new M.C< >() 
        let _ = new M.C<int>() 
        let _ = M.C()     
        let _ = M.C< >()     
        let _ = M.C<int>()  
        let _ = M.C<_>()  

        let _ = M.C.P     
        let _ = M.C< >.P     
        let _ = M.C<_>.P

        open M

        let _ = new C() 
        let _ = new C< >() 
        let _ = new C<int>() 
        let _ = C()     
        let _ = C< >()     
        let _ = C<int>()  
        let _ = C<_>()  

        let _ = C.P     
        let _ = C< >.P     
        let _ = C<_>.P
        
        // open again for good luck
        open M

        let _ = new C() 
        let _ = new C< >() 
        let _ = new C<int>() 
        let _ = C()     
        let _ = C< >()     
        let _ = C<int>()  
        let _ = C<_>()  

        let _ = C.P     
        let _ = C< >.P     
        let _ = C<_>.P
        
    module OneNonAmbiguousGenericType = 
        module M3 = 
            type C<'a>() = 
               static member P = 1


        let _ = new M3.C()  //  give error
        let _ = M3.C()     
        let _ = M3.C<int>()   

        let _ = M3.C.P      //  give warning
        let _ = M3.C<_>.P

        open M3

        let _ = new C()  //  give error
        let _ = C()     
        let _ = C<int>()   

        let _ = C.P      //  give warning
        let _ = C<_>.P
        
        // open again for good luck
        open M3

        new C()  //  give error
        let _ = C()     
        let _ = C<int>()   

        let _ = C.P      //  give warning
        let _ = C<_>.P

    module ActualRepro1 = 
        type Foo<'a>() =
            member x.f() = ()

        type Foo<'a,'b>() =
            member x.f() = ()

        let foo = Foo()  // Foo<'a,b> !!!

    module ActualRepro2 =     

        type T<'a>() = 
            static member P = sizeof<'a>
            member x.M = T.P  // expect deprecation warning

        let v = (new T<int64>()).M

        printfn "Expected: 8"
        printfn "Actual:   %d" v

module FSharp_1_0_Bug_3183 = 
    type IIndexable<'T> = 
         abstract (.[]) : 'T with get, set       // expect no warning here
    let (.[]) x = x // expect warning here

module FSharp_1_0_Bug_2949 = 
    [<AbstractClass>]
    type T() = 
        abstract X : unit -> int       
        abstract X : unit -> decimal     // expect duplicate member error here
        abstract X : unit -> byte        // expect duplicate member error here
        abstract X : unit -> char        // expect duplicate member error here
        abstract X : unit -> float<_>    // expect duplicate member error here

module TestWarningGivenOnLessGenericCode = 
    let f (x:'a,y:'b) = [x;y]
    
module TestWarningGivenOnRecursiveCodeWithDifferentTypeVariableNames = 
    let rec f (x:'a) = g x
    and g (x:'b) = f x

module TestNoWarningGivenOnRecursiveCodeWithSameTypeVariableNames = 
    let rec f (x:'a) = g x
    and g (x:'a) = f x
    
    