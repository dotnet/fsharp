module FSharp.Compiler.Service.Tests.TooltipComputationExpressionsTests

open Xunit

let private identifierHaveDiffMeaningsSource = """namespace NS
                            module float(*Marker1_1*) =

                                let GenerateTuple =  fun x ->   let tuple = (x,x.ToString(),(float(*Marker1_2*))x, ( fun y -> (y.ToString(),y+1)) )
                                                                tuple

                                let MySeq : seq(*Marker2_1*)<float(*Marker1_3*)> = 
                                    seq(*Marker2_2*)    {
                
                                                           for i in 1..9 do
                                     
                                                                let myTuple = GenerateTuple i
                                                                let fieldInt,fieldString,fieldFloat,_ = myTuple
                                                                yield fieldFloat
                                                        }
            
                                let MySet : Set(*Marker3_1*)<float> = 
                                    MySeq 
                                    |> Array.ofSeq
                                    |> List.ofArray
                                    |> Set(*Marker3_2*).ofList

                                let int(*Marker4_1*) : int(*Marker4_2*) = 1

                                type int(*Marker4_3*)() = 
                                    member this.M = 1

                                type T(*Marker5_1*)() =
                                    [<DefaultValueAttribute>]
                                    val mutable T : T

                                let T = new T()
                                let t = T.T.T.T(*Marker5_2*);
    
                                type ValType() = 
                                    member this.Value with get(*Marker6_1*) () = 10
                                                       and set(*Marker6_2*) x  = x + 1 |> ignore"""

let private typeAbbreviationsSource = """namespace NS
                            module TypeAbbreviation =
    
                                type MyInt(*Marker1_1*) = int
    
                                type PairOfFloat(*Marker2_1*) = float * float
    
    
                                type AbAttrName(*Marker5_1*) = AbstractClassAttribute
      
    
                                type IA(*Marker3_1*) = 
                                    abstract AbstractMember : int -> int
        
                                [<AbAttrName(*Marker5_2*)>]
                                type ClassIA(*Marker3_2*)() =
                                    interface IA with
                                        member this.AbstractMember x = x + 1
    
                                type GenericClass(*Marker4_1*)<'a when 'a :> IA>() = 
                                    static member StaticMember(x:'a) = x.AbstractMember(1)


                                let GenerateTuple =  fun ( x : MyInt) ->    
                                                        let myInt(*Marker1_2*),float1,float2,function1 = (x,(float)x,(float)x, ( fun y -> (y.ToString(),y+1)) )
                                                        myInt,((float1,float2):PairOfFloat),function1

                                let MySeq(*Marker2_2*) = 
                                    seq     {
                
                                               for i in 1..9 do
                                                    let myInt,pairofFloat,function1 = GenerateTuple i
                        
                                                    yield pairofFloat
                                            }
                
                                let genericClass(*Marker4_2*) = new GenericClass<ClassIA>()"""

let private whereQuickInfoShouldNotShowUpSource = """namespace Test

                            module Helper = 
                                /// Tests if passed System.Numerics.BigInteger(*Marker1*) argument is prime
                                let IsPrime x =  
                                    let mutable i = 2I
                                    let mutable foundFactor = false  
                                    while not foundFactor && i < x do
                                        (*
                                            the most naive way to test for number being prime
                                            Works great for small int(*Marker2*)
                                        *)
                                        if x % i = 0I then  
                                            foundFactor <- true  
                                        i <- i + 1I 
                                    not foundFactor
        
                            module App = 
                                open Helper
    
                                let sumOfAllPrimesUnder1Mi =
                                #if TEST_TWO_MI
                                    seq(*Marker4*) { 1I .. 2000000I }
                                #else
                                    seq { 1I .. 1000000I(*Marker7*) }
                                #endif
                                    |> Seq.filter(IsPrime)
                                    // find result after filtering seq(*Marker3*)
                                    |> Seq.sum
        
                                let myString hello = "hello"(*Marker5*)
    
                                myString "myString"(*Marker8*)
                                |> Seq.filter (fun c -> int c > 75)
                                |> Seq.item 0
                                |> (=) 'e'(*Marker6*)
                                |> ignore"""

let private xDelegateSource = """module Test

                            open FSTestLib
                                
                            open System.Runtime.InteropServices
                            let ctrlSignal = ref false
                            [<DllImport("kernel32.dll")>]
                            extern void SetConsoleCtrlHandler(ControlEventHandler callback,bool add)
                            let ctrlEventHandlerStatic     = new ControlEventHandler(MyCar.Run)
                            let ctrlEventHandlerInstance   = new ControlEventHandler( (new MyCar(10, MyColors.Blue)).Repair )

                            let IsInstanceMethod (controlEventHandler:ControlEventHandler) =
                                // TC 32	Identifier	Delegate	Own Code	Pattern Match
                                match controlEventHandler(*Marker1*).Method.IsStatic  with 
                                | true -> printf "It's not a instance method. "
                                | false -> printf " It's a instance method. " 
    
                            // TC 33	Event	DiscUnion	Own Code	Quotation
                            let a = <@ MyDistance.Event(*Marker2*) @>

                            let DelegateSeq =
                                seq {   for i in 1..10 do
                                            let newDelegate = new ControlEventHandler(MyCar.Run)
                                            // TC 35	Identifier	Delegate	Own Code	Comp Expression
                                            yield newDelegate(*Marker3*) }
                
                            let StructFieldSeq =
                                seq { for i in 1..10 do
                                            let a = MyPoint((float)i,2.0)
                                            // TC 36	Field	Struct	Own Code	Comp Expression
                                            yield a.X(*Marker4*) }"""

let private asyncToolTipsSource = """let a = 
                             async {
                                 let ms = new System.IO.MemoryStream(Array.create 1000 1uy)
                                 let toFill = Array.create 2000 0uy
                                 let! x = ms.AsyncRead(2000)
                                 return x
                             }"""

[<Fact(Skip = "DocComment issue")>]
let ``Automation.IdentifierHaveDiffMeanings`` () =
    let source = identifierHaveDiffMeaningsSource
    assertTooltipContainsInFsFile "module float" (markAtStartOfMarker source "(*Marker1_1*)")
    assertTooltipContainsInFsFile "val float: 'T -> float (requires member op_Explicit)" (markAtStartOfMarker source "(*Marker1_2*)")
    assertTooltipContainsInFsFile "Full name: Microsoft.FSharp.Core.Operators.float" (markAtStartOfMarker source "(*Marker1_2*)")
    assertTooltipContainsInFsFile "type float = System.Double" (markAtStartOfMarker source "(*Marker1_3*)")
    assertTooltipContainsInFsFile "Full name: Microsoft.FSharp.Core.float" (markAtStartOfMarker source "(*Marker1_3*)")
    assertTooltipContainsInFsFile "type seq<'T> = System.Collections.Generic.IEnumerable<'T>" (markAtStartOfMarker source "(*Marker2_1*)")
    assertTooltipContainsInFsFile "Full name: Microsoft.FSharp.Collections.seq<_>" (markAtStartOfMarker source "(*Marker2_1*)")
    assertTooltipContainsInFsFile "val seq: 'T seq -> 'T seq" (markAtStartOfMarker source "(*Marker2_2*)")
    assertTooltipContainsInFsFile "Full name: Microsoft.FSharp.Core.Operators.seq" (markAtStartOfMarker source "(*Marker2_2*)")
    assertTooltipContainsInFsFile "type Set<'T (requires comparison)> =" (markAtStartOfMarker source "(*Marker3_1*)")
    assertTooltipContainsInFsFile "Full name: Microsoft.FSharp.Collections.Set" (markAtStartOfMarker source "(*Marker3_1*)")
    assertTooltipContainsInFsFile "module Set" (markAtStartOfMarker source "(*Marker3_2*)")
    assertTooltipContainsInFsFile "Functional programming operators related to the Set<_> type" (markAtStartOfMarker source "(*Marker3_2*)")
    assertTooltipContainsInFsFile "val int: int" (markAtStartOfMarker source "(*Marker4_1*)")
    assertTooltipContainsInFsFile "Full name: NS.float.int" (markAtStartOfMarker source "(*Marker4_1*)")
    assertTooltipContainsInFsFile "type int = int32" (markAtStartOfMarker source "(*Marker4_2*)")
    assertTooltipContainsInFsFile "Full name: Microsoft.FSharp.Core.int" (markAtStartOfMarker source "(*Marker4_2*)")
    assertTooltipContainsInFsFile "type int =" (markAtStartOfMarker source "(*Marker4_3*)")
    assertTooltipContainsInFsFile "member M: int" (markAtStartOfMarker source "(*Marker4_3*)")
    assertTooltipContainsInFsFile "type T =" (markAtStartOfMarker source "(*Marker5_1*)")
    assertTooltipContainsInFsFile "new : unit -> T" (markAtStartOfMarker source "(*Marker5_1*)")
    assertTooltipContainsInFsFile "val mutable T: T" (markAtStartOfMarker source "(*Marker5_1*)")
    assertTooltipContainsInFsFile "T.T: T" (markAtStartOfMarker source "(*Marker5_2*)")
    assertTooltipContainsInFsFile "member ValType.Value : int" (markAtStartOfMarker source "(*Marker6_1*)")
    assertTooltipContainsInFsFile "member ValType.Value : int with set" (markAtStartOfMarker source "(*Marker6_2*)")
    assertTooltipDoesNotContainInFsFile "Microsoft.FSharp.Core.ExtraTopLevelOperators.set" (markAtStartOfMarker source "(*Marker6_2*)")

[<Fact>]
let ``Automation.TypeAbbreviations`` () =
    let source = typeAbbreviationsSource
    assertTooltipContainsInFsFile "type MyInt = int" (markAtStartOfMarker source "(*Marker1_1*)")
    assertTooltipContainsInFsFile "val myInt: MyInt" (markAtStartOfMarker source "(*Marker1_2*)")
    assertTooltipContainsInFsFile "type PairOfFloat = float * float" (markAtStartOfMarker source "(*Marker2_1*)")
    assertTooltipContainsInFsFile "val MySeq: PairOfFloat seq" (markAtStartOfMarker source "(*Marker2_2*)")
    assertTooltipContainsInFsFile "type IA =" (markAtStartOfMarker source "(*Marker3_1*)")
    assertTooltipContainsInFsFile "type ClassIA =" (markAtStartOfMarker source "(*Marker3_2*)")
    assertTooltipContainsInFsFile "type GenericClass<'a (requires 'a :> IA)> =" (markAtStartOfMarker source "(*Marker4_1*)")
    assertTooltipContainsInFsFile "val genericClass: GenericClass<ClassIA>" (markAtStartOfMarker source "(*Marker4_2*)")
    assertTooltipContainsInFsFile "type AbAttrName = AbstractClassAttribute" (markAtStartOfMarker source "(*Marker5_1*)")
    assertTooltipContainsInFsFile "type AbAttrName = AbstractClassAttribute" (markAtStartOfMarker source "(*Marker5_2*)")

[<Fact(Skip = "editor-layer quickinfo suppression in comments / string & char literals / inactive #if branches; FCS GetToolTip resolves the identifier island regardless of lexical context")>]
let ``Automation.WhereQuickInfoShouldNotShowUp`` () =
    let source = whereQuickInfoShouldNotShowUpSource
    assertTooltipDoesNotContainInFsFile "BigInteger" (markAtStartOfMarker source "(*Marker1*)")
    assertTooltipDoesNotContainInFsFile "int" (markAtStartOfMarker source "(*Marker2*)")
    assertTooltipDoesNotContainInFsFile "seq" (markAtStartOfMarker source "(*Marker3*)")
    assertTooltipDoesNotContainInFsFile "seq" (markAtStartOfMarker source "(*Marker4*)")
    assertTooltipDoesNotContainInFsFile "hello" (markAtStartOfMarker source "(*Marker5*)")
    assertTooltipDoesNotContainInFsFile "char" (markAtStartOfMarker source "(*Marker6*)")
    assertTooltipDoesNotContainInFsFile "bigint" (markAtStartOfMarker source "(*Marker7*)")
    assertTooltipDoesNotContainInFsFile "myString" (markAtStartOfMarker source "(*Marker8*)")

[<Fact>]
let ``Automation.XDelegateDUStructfromOwnCode`` () =
    let source = xDelegateSource
    assertTooltipContainsWithFsTestLib "val controlEventHandler: ControlEventHandler" (markAtStartOfMarker source "(*Marker1*)")
    assertTooltipContainsWithFsTestLib "property MyDistance.Event: Event<string>" (markAtStartOfMarker source "(*Marker2*)")
    assertTooltipContainsWithFsTestLib "val newDelegate: ControlEventHandler" (markAtStartOfMarker source "(*Marker3*)")
    assertTooltipContainsWithFsTestLib "property MyPoint.X: float" (markAtStartOfMarker source "(*Marker4*)")
    assertTooltipContainsWithFsTestLib "Gets and sets X" (markAtStartOfMarker source "(*Marker4*)")

[<Fact(Skip = "disabled upstream (KnownFail); assertions preserved")>]
let ``Async.AsyncToolTips`` () =
    let source = asyncToolTipsSource
    assertTooltipContains "AsyncBuilder" (markAtEndOfMarker source "asy")
    assertTooltipDoesNotContain "---" (markAtEndOfMarker source "asy")
