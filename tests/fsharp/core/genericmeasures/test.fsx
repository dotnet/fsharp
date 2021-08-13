#if COMPILED
module Core_genericMeasures
#else
module Core_genericMeasures =
#endif

    [<AllowNullLiteral>]
    type C<'T> = class end
    
    [<Measure>] type t
    [<Measure>] type t2
    let f1 (_ : int<t>) = ()
    let f2 (_ : float<t>) = ()
    let f3 (_ : int<_>) = ()
    let f4 (_ : float<_>) = ()
    let f5 (_ : C<'a>) = ()
    let f6 (xs : list<'a>) = 
        match box xs with
        | null -> failwith "unexpected null list"
        | _ -> if List.length xs <> 0 then failwith "expected empty list"
    let f7 (xs : list<'a>) = 
        match box xs with
        | null -> failwith "unexpected null list"
        | _ -> if List.length xs <> 0 then failwith "expected empty list"
    
    let foo() =
        let a = 0<_>
        let b = 0.0<_>
        let c = null : C<int<_>>
        let c2 = c : C<int<_>>
        let d = null : C<float<_>>
        let e = [] : list<int<_>>
        let f = [] : list<float<_>>
        let g = null : C<int<_> * _>
        let h = null : C<_ * int<_> * _>
        let i : List<int<_>> = List.empty
        let j : List<float<_>> = List.empty
        let k : List<float<_>> = j
    
        f1 a
        f2 b
        f3 a
        f4 b
        f5 c
        f5 c2
        f5 d
        f6 e
        f6 f
        f5 g
        f5 h
        f6 i
        f6 j
        f7 (i : List<int<t>>)
        f7 (i : List<int<t2>>)
        f7 (j : List<float<t>>)
        f7 (j : List<float<t2>>)
        f7 (k : List<float<t>>)
        f7 (k : List<float<t2>>)
    
    type T = 
        static member Foo(_ : int<t>) = ()
        static member Foo1(_ : int<_>) = ()
    
        static member Bar() =
            let x = 0<_>
            T.Foo(x)
    
        static member Baz() =
            let x = 0<_>
            T.Foo1(x)
    
    let RunAll() = 
        foo()
        T.Bar()
        T.Baz()
    

#if TESTS_AS_APP
    let RUN() = RunAll(); []
#else
    RunAll();
    stdout.WriteLine "Test Passed"
    System.IO.File.WriteAllText("test.ok","ok")
    exit 0
#endif


