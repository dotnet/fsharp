// #Regression #TypeInference 
// Regression test for FSHARP1.0:4758
// Type Inference
namespace N
module ActualTests2 = 

    type Var<'a> =
        static member Foo(y:'a)      : Var<bool> = failwith "" 
        static member Foo(y:Var<'a>) : Var<bool> = failwith "" 

    module M = 
        // let,let-rec,static-member with explicit instantiation and fully typed arguments and return type on lhs and Var<'a>.call
        let               a<'a> (y:'a) = Var<'a>.Foo( y ) 
        let rec           c<'a> (y:'a) = Var<'a>.Foo( y ) 
        // Expected all OK.
        
