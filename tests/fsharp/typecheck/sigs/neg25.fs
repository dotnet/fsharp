module Test

module NegativeTests = 

    module Test1 = 
        type ITest =
            abstract member Meth1: string -> string

        type ITestSub =
            inherit ITest
            abstract member Meth2: int -> int

        [<AbstractClass>]
        type Partial() =  // THis should give an error  - Meth1 is not implemented
            interface ITestSub with 
                override this.Meth2 x = x + 1

        type ErroneousComplete () =   
            inherit Partial()


    module Test2 = 
        type ITest =
            abstract member Meth1: string -> string

        type ITestSub =
            inherit ITest
            abstract member Meth2: int -> int

        [<AbstractClass>]
        type Partial() =  // this should give an error - we can't complete the partial implementation implicitly
            interface ITestSub with
                override this.Meth2 x = x + 1

        type ErroneousComplete () =
            inherit Partial()
            abstract Meth1 : string -> string
            override x.Meth1(s) = s                   


module NegativeTestsActualRepro1 = 

    open System
    [<NoEquality;NoComparison>]
    type Node =
        TestVariableNode of seq<Object>
      | InterleavingNode of seq<Node>
      | SynthesizingNode of seq<Node> * Delegate

    type INodeWrapper =
        abstract member Node: Node


    open System.Collections


    type ITestCaseEnumeratorFactory =
        inherit INodeWrapper
        abstract member CreateEnumerator: uint32 -> IEnumerator;
        abstract member MaximumStrength: System.UInt32;

    [<AbstractClass>]
    type TestCaseEnumeratorFactoryCommonImplementation () as this = // Expect an error here - incomplete interface implementation
        interface ITestCaseEnumeratorFactory with
            override this.CreateEnumerator desiredStrength =
                (Seq.empty :> IEnumerable).GetEnumerator ()    // Stub
            override this.MaximumStrength =
                9u    // Stub

    type TestVariableLevelEnumeratorFactory (levels: seq<Object>) =    // THis should give an error
            inherit TestCaseEnumeratorFactoryCommonImplementation ()
            let node =
               TestVariableNode levels

    module Throwaway =
        let foo =
            TestVariableLevelEnumeratorFactory Seq.empty // Surely I can't do this at compile time?

module NegativeTestsActualRepro3 = 

    open System
    type IThing =
        abstract Name: string
        abstract Action: List< string * (unit -> unit) >
    [<AbstractClass>]
    type Dog() =                     // Expect an error here - incomplete interface implementation
        interface IThing with
            member x.Action = [("bites", fun () -> printfn "ouch")]

    type MyDog() =   // expect an error here - Name not implemented
        inherit Dog() 

module AnotherNegativeTest = 
    type ITest =
        abstract member Meth1: int -> int

    type ITestSub =
        inherit ITest
        abstract member Meth1: int -> int

    [<AbstractClass>]
    type Partial() =
        abstract Meth1 : int -> int
        interface ITestSub
        interface ITest with
            override this.Meth1 x = this.Meth1 x

    type OkComplete () =
        inherit Partial()
        override this.Meth1 x = x

    module Throwaway =
        let foo = OkComplete()

module MissingInterfaceMemberTests = 
        module Test0 = 
            type ITest =
                abstract member Meth1: string -> string

            type ITestSub =
                inherit ITest
                abstract member Meth2: int -> int

            [<AbstractClass>]
            type Partial() =
                interface ITestSub 

            type OkComplete () =
                inherit Partial()
                interface ITest with
                    override this.Meth1 x = x + "1"
                interface ITestSub with
                    override this.Meth2 x = x + 1

            module Throwaway =
                let foo = OkComplete()
        module Test1 = 
            type ITest =
                abstract member Meth1: string -> string

            type ITestSub =
                inherit ITest
                abstract member Meth2: int -> int

            [<AbstractClass>]
            type Partial() =
                interface ITestSub with
                    override this.Meth2 x = x + 1

            type OkComplete () =
                inherit Partial()
                interface ITest with
                    override this.Meth1 x = x + "1"
                interface ITestSub with
                    override this.Meth2 x = x + 1

            module Throwaway =
                let foo = OkComplete()


        module Test3 = 
            type ITest =
                abstract member Meth1: string -> string

            type ITestSub =
                inherit ITest
                abstract member Meth2: int -> int

            [<AbstractClass>]
            type Partial() =
                interface ITestSub with
                    override this.Meth2 x = x + 1
            type OkComplete () =
                inherit Partial()
                interface ITest with
                    override this.Meth1 x = x + "1"

            module Throwaway =
                let foo = OkComplete()



    module NegativeTestsActualRepro2 = 
        type IEvent<'del,'a> = 
            abstract Add : ('a -> unit) -> unit
            abstract Add2 : ('a -> unit) -> unit
            
        [<AbstractClass>]
        type wire<'a>() =
            abstract Send   : 'a -> unit
            abstract Listen : ('a -> unit) -> unit
            interface IEvent<Handler<'a>,'a> with   // Expect an error here - Add2 no implemented
                member x.Add(handler) = x.Listen(handler)

        let createWire() =
            let listeners = ref [] in
            {new wire<'a>() with   
              member obj.Send(x)   = List.iter (fun f -> f x) !listeners   
              member obj.Listen(f) = listeners := f :: !listeners
            }


    module NegativeTestsActualRepro4 = 

        open System
        type IThing =
            abstract Name: string
            abstract Action: List< string * (unit -> unit) >
        [<AbstractClass>]
        type Dog() =                      // Expect an error here - Name not implemented
            interface IThing with 
                member x.Action = [("bites", fun () -> printfn "ouch")]

        let MyDog() =    { new Dog() with member x.ToString() = "2" }   

