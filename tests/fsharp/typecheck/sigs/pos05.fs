namespace global

module PrefixOperatorsDefaultPositive = 
    let f1 (x:Quotations.Expr<'T>) = <@ id %x @>  // now allowed
    let f2 (x:Quotations.Expr) = <@@ id %%x @@>   // now allowed
    let g (x:byref<int>) = x
    let g2 (x:byref<int>) (x2:byref<int>) = x
    let g3 (x:nativeptr<int>) (x2:nativeptr<int>) = x
    let f3 x = let mutable v = 1 in g &v       // now allowed
    let f4 x = let mutable v = 1 in g2 &v &v   // now allowed
    let f5 x = let mutable v = 1 in g3 &&v &&v // now allowed

module PrefixOperatorsPositive = 
    // special cases:
    let (~+) x = x // keep
    let (~-) x = x // keep
    let (~-.)  (x:float)           =  -x // keep
    let (~+.)  (x:float)           =  -x // keep
    let (~%) x = x // keep
    let (~%%) x = x // keep

    // user-defined prefix operators:
    let (~~) x = x // keep
    let (~~~) x = x // keep
    let (~~~~~~) x = x // keep
    let (!) x = x  // keep
    let (!!) x = x  // keep
    let (!!!) x = x  // keep
    let (!!!!) x = x  // keep
    let (!!!!!) x = x  // keep
    let (!~) x = x // keep
    let (!?) x = x // keep

    let (?) x y = x // keep
    let (?<-) x y z = x // keep

    let (++) x y  = x + y // keep

    let x11 : int = id +2 // keep
    let x12 : int = id -2 // keep
    let x13 : int = id ~~2 // keep
    let x14 : int = id !2 // keep
    let x15 : int = id %2 // keep
    let x16 : int = id %%2 // keep
    let x17 : int = id ~~~2 // keep
    let x18 : float = id -.2.0 // keep
    let x19 : float = id +.2.0 // keep
    let x5 : int = id !~2 // keep
    let x6 : int = id !?2 // keep

    let x7 : int = 1 ++2 // ideally reject, keep as infix, assuming (++) is defined
    let x8 : int = id ~~   2 // keep as prefix
    let x9 : int = id !    2 // keep as prefix
    let x10 : int = id ~~~  2 // keep as prefix

// Check the '**' operator can be overloaded

type Gaussian(x:float,y:float) =
  static member ( ** ) (g: Gaussian, e: float) = g
  static member Pow (g: Gaussian, e: float) = g

module M = 
    let c1 : Gaussian = Gaussian(0.,1.) ** 3.
    let c2 = Gaussian(0.,1.) ** 3.
