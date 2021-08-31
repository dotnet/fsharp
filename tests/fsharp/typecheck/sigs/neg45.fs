namespace global

module RequiresExplicitTypeArgumentsAttributeNotRespectedForMembers_FSharp_1_0_6163 = 

    type G1() =
        [<RequiresExplicitTypeArgumentsAttribute>]
        member x.Foo<'a>(y:'a) = printfn "first"
        [<RequiresExplicitTypeArgumentsAttribute>]
        member x.Foo<'a>(y:'a, ?z:int) = printfn "second"

    let g1 = new G1()    
    g1.Foo(42)         // first
    g1.Foo<int>(42)    // first
    g1.Foo(42, 0)      // second
    g1.Foo<int>(42, 0) // second


    type G2() =
        [<RequiresExplicitTypeArgumentsAttribute>]
        member x.Foo<'a>(y:'a, ?z:int) = printfn "second"

    let g2 = new G2()    
    g2.Foo(42)         // second
    g2.Foo<int>(42)    // second
    g2.Foo(42, 0)      // second
    g2.Foo<int>(42, 0) // second


module CompilerStackOverflowOnOperatorOverloading_FSharp_1_0_6164 = 
    type 'a D =
      static member inline (+)(_:^b D, _:^b) : ^b when ^b : (static member (+) : ^b * ^b -> ^b) = failwith "Not implemented"
      static member inline (+)(_:^b, _:^b D) : ^b when ^b : (static member (+) : ^b * ^b -> ^b) = failwith "Not implemented"
 
    let f (x:int D) = x + 1


module ActivePatternsShouldNotBeAllowedOnMembers = 

    // No error in this class, static works great
    type FooBir() = 
        static member (|Foo|Bir|) (x, y) =
            match x = y with
            | true -> Foo
            | false -> Bir
    
        member x.doSomething y =
            match x, y with
            | Foo -> ()  // compiles!  How is 'Foo' in scope?
            | Bir -> ()
    
    match 1,2 with
    | FooBir.Foo -> printfn "hi"  // The field, constructor or member 'Foo' is not defined
    | _ -> ()
    
    type FooBir2() = 
        member x.(|Foo|Bir|) y =
            match x = y with
            | true -> Foo
            | false -> Bir

        // compiler error on "Foo"    
        member x.doSomething y =
            let r = x.``|Foo|Bir|``  y   // compiles!
            match r with
            | Foo -> () // The type 'Choice<unit,unit>' is not compatible with the type 'FooBir2'
            | Bir -> ()

module CheckThatNoConstraintPropagationHappensForTypeParameters = 
    type C<'T when 'T :> System.IComparable>() =
        member x.P = 1

    type Negative1<'T> = C<'T>
    type Negative2<'T>() = inherit C<'T>()        // EXPECT ERROR
    type Negative3<'T >() = abstract X : C<'T>    // EXPECT ERROR
    type Negative4<'T > = UnionCase1 of C<'T>     // EXPECT ERROR
    type Negative5<'T > = { rf1 : C<'T> }         // EXPECT ERROR
    type Negative6<'T >(rf1: C<'T>) = struct end  // EXPECT ERROR
    type Negative7<'T > =  val rf1 : C<'T>          // EXPECT ERROR
    type Negative8<'T >(c: C<'T>) = member x.P = 1  // EXPECT ERROR
    type Negative9<'T>(x : C<'T> when 'T :> System.IComparable) = member x.P = 1  // EXPECT ERROR
    type Negative10<'T when 'T :> C<'T> > = member x.P = 1  // EXPECT ERROR


module CheckNoOverloadResolutionAgainstSignatureInformationGivenByTUpledAndRecordPatterns = 
    module Negative1 = 
        type R1 = { f1 : int }
        type R2 = { f2 : int }
        type D() = 
            member x.N = x.M { f1 = 3 }  /// EXPECT ERROR
            member x.M({ f1 = y }) = ()
            member x.M({ f2 = y }) = ()

    module Negative2 = 
        type R1 = { f1 : int }
        type R2 = { f2 : int }
        type D() = 
            member x.N = x.M (({ f1 = 3 },{ f1 = 3 }))  /// EXPECT ERROR
            member x.M((y1: R1,y2: R1)) = ()
            member x.M((y1: R2,y2: R1)) = ()
        
    module Negative3 = 
        type R1 = { f1 : int }  /// EXPECT ERROR
        type D() = 
            member x.N = x.M 3
            member x.M(1) = ()
            member x.M(()) = ()


module CheckInitializationGraphInStaticMembers = 

    module Positive6 = 
        type C() = 
           static let rec x = (); (fun () -> ignore x; ())        // expect warning
           static member A = x
           
    

module CheckMemberConstraintNegativeCases = 

    let inline staticName< ^T when ^T : (static member Name : string) > () = (^T : (static member Name : string) ())
    let inline staticAge< ^T when ^T : (static member Age : int)> () = (^T : (static member Age : int) ())
    let inline staticLifetime< ^T when  ^T : (static member LifeTime : System.TimeSpan) > () = (^T : (static member LifeTime : System.TimeSpan) ())

    let inline name1< ^T when ^T : (member Name : int -> string) > (x:^T) = (^T : (member Name : int -> string) (x,3))
    let inline age (x:^T) = (^T : (member Age : int) x)
    let inline setName (x:^T) (y:string) = (^T : (member set_Name : string -> unit) (x,y))

    type A1 = { Name: string }
    type A2 = { mutable Name: string }
    type B = { name: string } with member this.Name = this.name
    let a1 : A1 = { Name = "a" }
    let a2 : A2 = { Name = "a" }

    type StaticMutableClassExplicit =
        [<DefaultValue>]
        static val mutable private Name: string

    // check that static member constraints aren't satisfied by record properties
    let _ = setName a1
    let _ = age a1
    let _ = age a2
    let _ = name1<A1> { Name = "1" }
    let _ = name1<A1>
    let _ = staticName<A1>
    let _ = staticName<A2>
    let _ = staticName<B>
    let f() = staticName<StaticMutableClassExplicit>
    