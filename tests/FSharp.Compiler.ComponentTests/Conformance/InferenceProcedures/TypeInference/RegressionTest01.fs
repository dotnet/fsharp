// #Regression #TypeInference 
// Regression test for FSHARP1.0:4758
// Type Inference
namespace N
module ActualTests1 = 
    type Var<'a> =
        static member Foo(x:Var<'a>,y:'a)      = failwith "" : Var<bool>
        static member Foo(x:Var<'a>,y:Var<'a>) = failwith "" : Var<bool>

    module M = 
        // let,let-rec,static-member with explicit instantiation and fully typed arguments and return type on lhs and Var<'a>.call
        let               a1<'a> (x:Var<'a>,y:'a) : Var<bool> = Var<'a>.Foo( (x:Var<'a>), (y:'a) ) : Var<bool>
        let               a2 (x:Var<'a>,y:'a) : Var<bool> = Var<'a>.Foo( (x:Var<'a>), (y:'a) ) : Var<bool>
        let rec           c1<'a> (x:Var<'a>,y:'a) : Var<bool> = Var<'a>.Foo( (x:Var<'a>), (y:'a) ) : Var<bool>
        let rec           c2 (x:Var<'a>,y:'a) : Var<bool> = Var<'a>.Foo( (x:Var<'a>), (y:'a) ) : Var<bool>
        type Var<'a> with
           static member c1<'a> (x:Var<'a>,y:'a) : Var<bool> = Var<'a>.Foo( (x:Var<'a>), (y:'a) ) : Var<bool>
           static member c2<'b> (x:Var<'b>,y:'b) : Var<bool> = Var<'b>.Foo( (x:Var<'b>), (y:'b) ) : Var<bool>
           static member c3<'c> (x:Var<'c>,y:'c) : Var<bool> = Var<'c>.Foo( (x:Var<'c>), (y:'c) ) : Var<bool>
           static member c4 (x:Var<'b>,y:'b) : Var<bool> = Var<'b>.Foo( (x:Var<'b>), (y:'b) ) : Var<bool>
           static member c5 (x:Var<'a>,y:'a) : Var<bool> = Var<'a>.Foo( (x:Var<'a>), (y:'a) ) : Var<bool>
           // Expected all OK.

