#if COMPILED
module Core_genericMeasures
#else
module Core_genericMeasures =
#endif

    [<AllowNullLiteral>]
    type C<'T> = class end
    
    [<Measure>] type t
    let f1 (_ : int<t>) = ()
    let f2 (_ : float<t>) = ()
    let f3 (_ : int<_>) = ()
    let f4 (_ : float<_>) = ()
    let f5 (_ : C<'a>) = ()
    let f6 (_ : list<'a>) = ()
    
    let foo() =
        let a = 0<_>
        let b = 0.0<_>
        let c = null : C<int<_>>
        let d = null : C<float<_>>
        let e = [] : list<int<_>>
        let f = [] : list<float<_>>
        let g = null : C<int<_> * _>
        let h = null : C<_ * int<_> * _>
        let i : List<int<_>> = List.empty
        let j : List<float<_>> = List.empty
    
        f1 a
        f2 b
        f3 a
        f4 b
        f5 c
        f5 d
        f6 e
        f6 f
        f5 g
        f5 h
        f6 i
        f6 j
    
    type T = 
        static member Foo(_ : int<t>) = ()
        static member Foo1(_ : int<_>) = ()
    
        static member Bar() =
            let x = 0<_>
            T.Foo(x)
    
        static member Baz() =
            let x = 0<_>
            T.Foo1(x)
    
    foo()
    T.Bar()
    T.Baz()
    
    let aa = 
        stdout.WriteLine "Test Passed"
        System.IO.File.WriteAllText("test.ok", "ok")
        exit 0