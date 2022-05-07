// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Verify tupled parameters to properties work as expected
// Regression for FSB 5273, ICE: Indexed properties not mixing well with tupled parameters

type T() =
     member this.X
       with set (a:int, b:int) = ()

(new T()).X <- (1,2)  // OK, use of 'tupled setter'

// --------------------------

type T2() =
     member this.X
       with set (a:int) (b:int) = ()

(new T2()).X(1) <- 2  // OK, setter where args are tupled

// --------------------------

type T3() =
     member this.X
       with set (a:int) (b:int, c:int) = ()

(new T3()).X(1) <- (2,3)
