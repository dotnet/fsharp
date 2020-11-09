// #Regression #Conformance #TypeInference #Recursion 
// Regression test for FSharp1.0:3187
// Title: better inference for mutually recursrive generic classes
// Descr: Verify types are inferred correctly for generic classes

module OO_Example_Example1 = 
    type C<'T>() = 
        member x.P = 1
        member x.Q = (new C<int>()).P


// In the negative case, this tests that eager generalization is based on a transitive (greatest-fixed-point) computation
module OO_Example_GreatestFixedPoint = 
    type C() = 
        member x.M1() = x.M2() |> fst
        member x.M2() = failwith "", x.M3()
#if NEGATIVE
        member x.Mbbb() = 
            (x.M1() : int) |> ignore 
            (x.M2() : string) |> ignore   // M1 should not be generalized at this point
#endif
        member x.M3() = 1


module OO_Example_RecordMemberCanBeGeneralized = 
    type R<'T> =  
        { x : 'T } 
        member p.X = p.x
        // R.X is now generalized
        member p.OneA = {x = 3}.X, {x = "3"}.X


module OO_Example_MethodParametersCanAlwaysBeGeneralized1 = 
    type C() = 
        static member M1<'T> (arg : 'T) = ()
        // C.M1 is now generalized

        static member M2() = C.M1<int>(2) ; C.M1<string>("2")


module OO_Example_MethodParametersCanAlwaysBeGeneralized2 = 
    type C() = 
        static member M1<'T> (arg : 'T) = C.M1b(arg); 1
        // C.M1 should now generalized since its signature is closed. However M1b will still share a (generalized) type variable with M1.
        static member Mtest() = 
            C.M1<int>(2) |> ignore
            C.M1<string>("2")

        static member M1b (arg) = C.M1b(arg)


module OO_Example3 = 
    type C<'T>() = 
        member x.Identity v = v
        member x.Q = 
            (new C<int>()).Identity 3  |> ignore
            (new C<int>()).Identity "3" |> ignore


module OO_Example4 = 
    type Vec<'a>(x:'a,y:'a) =
      member v.x = x
      member v.y = y
      member v.Mul(w:Vec<'b>) : Vec<'a*'b> = new Vec<'a*'b>((v.x,w.x),(v.y,w.y))


module OO_Example5 = 

    type Fred<'f>( nval: 'f ) =
        member this.Val = nval
        member this.DuplicateWith n2 =
            let t = new Tom<'f>(nval)
            t.HintVal n2
            t.ToFred()
     
    and Tom<'t>(initval: 't) =
        let mutable tval = initval
        member this.Val = tval
        member this.HintVal (z:'t) : unit = tval <- z
        member this.ToFred () :Fred<'t> = new Fred<_>(tval)
     
    let f = new Fred<int>(3)
    let t = new Tom<int>(5)


// In this example, 'f' can be generalized early because its type doesn't involve the type of 'g' 
module Example1 = 
    let rec Example1_f x = x
    and Example1_g x = Example1_f 1


// SImilar to Example1 but actually excercise 'f' at multiple types within the recursive group
module Example1b = 
    let rec Example1b_f x = x
    and Example1b_g x = 
        Example1b_f 1 |> ignore
        Example1b_f "1" |> ignore
        Example1b_f 1.0 |> ignore
        Example1b_f x |> ignore


// In this example, 'f' can NOT be generalized early because its type involves the (undetermined) 
// type of 'g' 
module Example2 = 
    let rec Example2_f x = Example2_g 1
    and Example2_g x = x


// In this example, 'f' can be generalized early because the type of 'g' is now fully known after type checking 'f'
module Example3 = 
    let rec Example3_f x = (Example3_g 1 : int) 
    and Example3_g x = x

// Similar to Example3 but actually excercise 'f' at multiple types within the recursive group
module Example3b = 
    let rec Example3b_f x = (Example3b_g 1 : int) 
    and Example3b_g x = 
        Example3b_f 1 |> ignore
        Example3b_f "1" |> ignore
        Example3b_f 1.0 |> ignore
        Example3b_f x |> ignore
        3



// In this example, 'f1' and 'f2' can be generalized together after checking 'f2'
// because the type of 'g' is fully known after type checking 'f2'
module Example4 = 
    let rec Example4_f1 x = Example4_g 1 
    and Example4_f2 x = (Example4_g 1 : int) 
    and Example4_g x = x

// Similar to Example4 but actually excercise 'f1' and 'f2' at multiple types within the recursive group
module Example4b = 
    let rec Example4b_f1 x = Example4b_g 1 
    and Example4b_f2 x = (Example4b_g 1 : int) 
    and Example4b_g x = 
        Example4b_f1 1 |> ignore
        Example4b_f1 "1" |> ignore
        Example4b_f1 1.0 |> ignore
        Example4b_f1 x |> ignore
        Example4b_f2 1 |> ignore
        Example4b_f2 "1" |> ignore
        Example4b_f2 1.0 |> ignore
        Example4b_f2 x |> ignore
        3

// In this example, 'f' SHOULD BE ABLE TO BE GENERALIZED EARLY because after checking 'f', its signature only involves the type
// variable associated with 'r', which is free in the environment. 
module Example5 = 
    let r = ref []
    let rec Example5_f x = (Example5_g r : int), r 
    and Example5_g x = 3
    r := [1]


// Similar to Example5 but actually excercise 'f' at multiple types within the recursive group
module Example5b = 
    let r = ref []
    let rec Example5b_f x = (Example5b_g r : int), r 
    and Example5b_g x = 
        Example5b_f 1 |> ignore
        Example5b_f "1" |> ignore
        Example5b_f 1.0 |> ignore
        Example5b_f x |> ignore
        3
    r := [1]


module Example6 = 
    let rec Example6_f x = Example6_g () |> ignore
    and Example6_g () = 
        Example6_f 1 |> ignore
        Example6_f "1" |> ignore
        Example6_f 1.0 |> ignore
        Example6_f () |> ignore
        failwith ""
