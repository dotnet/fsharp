// #Regression #Conformance #TypeInference 
// Regression for Dev11:10649, we used to generalize some members too early which could be unsound if that member was in a generic type
//<Expects status="error" span="(15,9-15,19)" id="FS3068">The function or member 'Inf' is used in a way that requires further type annotations at its definition to ensure consistency of inferred types\. The inferred signature is 'static member private Foo\.Inf: \('T list -> 'T list\)'\.$</Expects>
//<Expects status="error" span="(23,9-23,19)" id="FS3068">The function or member 'Foo' is used in a way that requires further type annotations at its definition to ensure consistency of inferred types\. The inferred signature is 'static member private Qux\.Foo: \('T list -> 'T list\)'\.$</Expects>

// Used to ICE
type Foo<'T> = FooCase of 'T
    with
    //static member private Inf l = ignore l  // ok
    static member private Inf = function
        | h::t -> h :: Foo<_>.Inf t
        | [] -> []
    member this.Bar() =
        let (FooCase(x)) = this
        Foo<_>.Inf [x; x]
 
// compiles ok
type Qux<'T>(x:'T) =
    static member private Foo = function
        | h::t -> h :: Qux<_>.Foo t
        | [] -> []
    member this.Bar() =
        Qux<_>.Foo [x; x]

exit 0