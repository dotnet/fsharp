module Neg56

// This should not compile. A has been generalized too early.
// In this example, envNonRec contains "?X", later instantiated to T
module InstanceMembersEarlyGeneralizationPotentiallyInvalidAndUltimatelyInconsistent = 
    type C<'T>() = 
        let mutable x = Unchecked.defaultof<_> // this inference variable ?X ultimately becomes T
        member this.A() = x // We incorrectly generalize "A" here, to have type Forall T. C<T> -> unit -> ?X
        member this.B1(c:C<string>) = c.A() 
           // At this point during type inference, the return type of c.A() is ?X
           // After type inference,  the return type of c.A() is 'string' by one measure and T be another - this is the inconsistency
        member this.B2(c:C<int>) = c.A() 
           // At this point during type inference, the return type of c.A() is ?X
           // After type inference,  the return type of c.A() is 'int' by one measure and T be another - this is the inconsistency
        member this.B3<'U>(c:C<'U>) = c.A() // the return type of c.A() is 'U', the return type of B1 is T
           // At this point during type inference, the return type of c.A() is ?X
           // After type inference,  the return type of c.A() is 'U' by one measure and T be another - this is the inconsistency
        member this.C() = (x : 'T)
           // At this point during type inference the inference variable ?X is inferred to be T

// This should not compile. A has been generalized too early.
module StaticMembersEarlyGeneralizationPotentiallyInvalidAndUltimatelyInconsistent = 
    type C<'T>() = 
        static let mutable x = Unchecked.defaultof<_> // this inference variable ultimately becomes T
        static member A() = x // We incorrectly generalize "A" here, to 'Forall T. unit -> ?X
        static member B1() = C<string>.A()
        static member B2() = C<int>.A()
        static member C() = (x : 'T)
