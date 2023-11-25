module OrderMatters

let f<'a, 'b> (x: 'b) (y: 'a) = ()

type T() =
    member this.i<'a, 'b> (x: 'b) (y: 'a) = printfn "%A %A" x y

// compound types
let h1<'a, 'b> (x: 'b * 'a) = ()
let h2<'a, 'b> (x: 'b -> 'a) = ()
let h3<'a, 'b> (x: {| F1: 'b; F2: 'a|}) = ()
let h4<'a, 'b> (x: seq<'b> * array<int * 'a>) = ()

// Avoid duplicate names
let z<'a, 'z> (z1: 'z) (z2: 'z) (z3: 'a) : 'z = z1

type IMonad<'a> =
    interface
        // Hash constraint leads to another type parameter
        abstract bind : #IMonad<'a> -> ('a -> #IMonad<'b>) -> IMonad<'b>
    end

open System.Runtime.InteropServices

type A<'zzz>() =
  // Process the solution of typar as well
  static member Foo(argA2: 'a, argB2: 'a -> 'b, argC2: 'b -> 'c, argD: 'c -> 'd, [<Optional>] argZ2: 'zzz) : 'd = argD (argC2( argB2 argA2))

type C<'a>() =
    // The explicit parameters are required here as well.
    static member SM5<'b,'c>(y:'a,z:'b) = 2