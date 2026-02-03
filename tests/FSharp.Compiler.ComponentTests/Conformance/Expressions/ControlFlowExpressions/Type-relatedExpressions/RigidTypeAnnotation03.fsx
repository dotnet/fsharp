// #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Expressions
//
// Rigid type annotation
// expr : ty
//
// rigid type annotation used as arguments to a function
//

[<Measure>] type Kg
[<Measure>] type m
[<Measure>] type s
[<Measure>] type N = Kg * m / s ^ 2

type T = class
            static member M(a:byte) = 1
            static member M(a:float) = 2
            static member M(a:float32<Kg>) = 3
            static member M(a:decimal<Kg>) = 4
            static member M(a:string) = 0
           
         end

(if (T.M(1uy : byte) = 1 && T.M(1.0<_> : float) = 2 && T.M(1.0f<Kg> : float32<_>) = 3 && 
     T.M(1.0M<Kg> : decimal<N*s*s/m>) = 4 && T.M( @"\") = 0) then 0 else 1) |> exit

exit 0
