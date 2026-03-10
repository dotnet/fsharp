// #Regression #Conformance #DataExpressions #ObjectConstructors 
// good - bug 6350
module mod6350

open System

type X = struct

    val mutable dt : DateTime
    member public x.Dt = x.dt
    //member public x.Foo() = x.dt <- new DateTime(1,1,1)

    new (d : int, m : int, y: int) =
        {dt = new DateTime(y,m,d)}

end
