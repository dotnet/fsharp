module FSharp.Compiler.Service.Tests.TooltipDiscriminatedUnionsTests

open System
open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Tokenization
open TestFramework

let private assertTooltipContainsWithProvider (expected: string) (markedSource: string) =
    Checker.getTooltipWithOptions
        [| "-r:" + PathRelativeToTestAssembly("DummyProviderForLanguageServiceTesting.dll") |]
        markedSource
    |> foldToolTip
    |> assertFoldedTooltipContains true "provider tooltip" expected

let private priorityQueueSource =
    """open System
type PriorityQueue(*MarkerType*)<'k,'a> =
  | Nil(*MarkerDataConstructor*)
  | Branch of 'k * 'a * PriorityQueue<'k,'a> * PriorityQueue<'k,'a>
module PriorityQueue(*MarkerModule*) =
  let empty = Nil
  let minKeyValue = function
    | Nil             -> failwith "empty queue"
    | Branch(k,a,_,_) -> (k,a)
  let minKey pq = fst (minKeyValue pq(*MarkerVal*))
  let singleton(*MarkerLastLine*) k a = Branch(k,a,Nil,Nil)"""

[<Fact>]
let ``TypeConstructorQuickInfo`` () =
    assertTooltipContainsInOrder
        [ "type PriorityQueue<'k,'a> ="
          "| Nil"
          "| Branch of 'k * 'a * PriorityQueue<'k,'a> * PriorityQueue<'k,'a>" ]
        (markAtStartOfMarker priorityQueueSource "(*MarkerType*)")

    assertTooltipContains
        "union case PriorityQueue.Nil: PriorityQueue<'k,'a>"
        (markAtStartOfMarker priorityQueueSource "(*MarkerDataConstructor*)")

    assertTooltipContainsInOrder
        [ "module PriorityQueue"; "from Test" ]
        (markAtStartOfMarker priorityQueueSource "(*MarkerModule*)")

    assertTooltipContains "val pq: PriorityQueue<'a,'b>" (markAtStartOfMarker priorityQueueSource "(*MarkerVal*)")

    assertTooltipContains
        "val singleton: k: 'a -> a: 'b -> PriorityQueue<'a,'b>"
        (markAtStartOfMarker priorityQueueSource "(*MarkerLastLine*)")

[<Fact>]
let ``NamedDUFieldQuickInfo`` () =
    let source =
        """type NamedFieldDU(*MarkerType*) =
  | Case1(*MarkerCase1*) of V1 : int * bool * V3 : float
  | Case2(*MarkerCase2*) of ``Big Name`` : int * Item2 : bool
  | Case3(*MarkerCase3*) of Item : int
exception NamedExn(*MarkerException*) of int * V2 : string * bool * Data9 : float"""

    assertTooltipContainsInOrder
        [ "type NamedFieldDU ="
          "| Case1 of V1: int * bool * V3: float"
          "| Case2 of ``Big Name`` : int * bool"
          "| Case3 of int" ]
        (markAtStartOfMarker source "(*MarkerType*)")

    assertTooltipContains
        "union case NamedFieldDU.Case1: V1: int * bool * V3: float -> NamedFieldDU"
        (markAtStartOfMarker source "(*MarkerCase1*)")

    assertTooltipContains
        "union case NamedFieldDU.Case2: ``Big Name`` : int * bool -> NamedFieldDU"
        (markAtStartOfMarker source "(*MarkerCase2*)")

    assertTooltipContains "union case NamedFieldDU.Case3: int -> NamedFieldDU" (markAtStartOfMarker source "(*MarkerCase3*)")

    assertTooltipContains
        "exception NamedExn of int * V2: string * bool * Data9: float"
        (markAtStartOfMarker source "(*MarkerException*)")

[<Fact>]
let ``Regression.InDeclaration.Bug3176d`` () =
    let source =
        """type DU<'a> =
  | DULabel of 'a"""

    assertTooltipContains "DULabel: 'a -> DU<'a>" (markAtEndOfMarker source "DULab")

[<Fact>]
let ``IdentifiersForUnionCases`` () =
    let source =
        String.concat "\n" [ "type TestType10 = Case1 | Case2 of int"; "let test12 = (Case1,Case2(3))" ]

    walk source "type TestType10 = " "Case1" "union case TestType10.Case1"
    walk source "type TestType10 = Case1 | " "Case2" "union case TestType10.Case2"
    walk source "let test12 = (" "Case1" "union case TestType10.Case1"
    walk source "let test12 = (Case1," "Case2" "union case TestType10.Case2"

[<Fact>]
let ``Regression.MemberDefinition.DocComments.Bug5856_3`` () =
    assertTooltipContainsInOrder
        [ "union case Module.Union.Case: int -> Module.Union"; "Case comment" ]
        """module Module =
    /// Union comment
    type Union =
        /// Case comment
        | Case of int

let x() = Module.Ca{caret}se"""

[<Fact>]
let ``Regression.MemberDefinition.DocComments.Bug5856_4`` () =
    assertTooltipContainsInOrder
        [ "type Union ="; "| Case of int"; "Union comment" ]
        """module Module =
    /// Union comment
    type Union =
        /// Case comment
        | Case of int

let _ = typeof<Module.Uni{caret}on>"""

[<Fact>]
let ``Automation.Regression.TypemoduleConstructorLastLine.Bug2494`` () =
    let source =
        """namespace NS
open System
//regression test for bug 2494
type PriorityQueue(*MarkerType*)<'k,'a> =
  | Nil(*MarkerDataConstructor*)
  | Branch of 'k * 'a * PriorityQueue<'k,'a> * PriorityQueue<'k,'a>
module PriorityQueue(*Marker3*) =
  let empty = Nil
  let minKeyValue = function
    | Nil             -> failwith "empty queue"
    | Branch(k,a,_,_) -> (k,a)
  let minKey pq = fst (minKeyValue pq(*MarkerVal*))
  let singleton(*MarkerLastLine*) k a = Branch(k,a,Nil,Nil)"""

    assertTooltipContainsInFsFile "type PriorityQueue" (markAtStartOfMarker source "(*MarkerType*)")

    assertTooltipContainsInFsFile
        "union case PriorityQueue.Nil: PriorityQueue<'k,'a>"
        (markAtStartOfMarker source "(*MarkerDataConstructor*)")

    assertTooltipContainsInFsFile "module PriorityQueue" (markAtStartOfMarker source "(*Marker3*)")
    assertTooltipContainsInFsFile "val pq: PriorityQueue<'a,'b>" (markAtStartOfMarker source "(*MarkerVal*)")

    assertTooltipContainsInFsFile
        "val singleton: k: 'a -> a: 'b -> PriorityQueue<'a,'b>"
        (markAtStartOfMarker source "(*MarkerLastLine*)")

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``XmlDocCommentsForArguments`` () =
    let source =
        """type bar() =
    /// <summary> Test for members</summary>
    /// <param name="x1">x1 param!</param>
    member this.foo
        (x1:int)=
        System.Console.WriteLine(x1.ToString())
type Uni1 =
   /// <summary> Test for unions </summary>
   /// <param name="str">str of case1</param>
   | Case1 of str: string
   | None
/// <summary> Test for exception types</summary>
/// <param name="value">value param</param>
exception Ex1 of value: string
// Methods
let f1 = (new bar()).foo(*Marker0*)(x1(*Marker1*) = 10)
let f2 = System.String.Concat(1, arg1(*Marker2*) = "")
//Unions
let f3 = Case1(str(*Marker3*) = "10")
match f3 with
| Case1(str(*Marker4*) = "10") -> ()
| _ -> ()
//Exceptions
let f4 = Ex1(value(*Marker5*) = "")
try
  ()
with
  Ex1(value(*Marker6*) = v) -> ()
//Static parameters of type providers
type provType = N1.T<Param1(*Marker7*)="hello", ParamIgnored(*Marker8*)=10>"""

    assertTooltipContains "Test for members" (markAtStartOfMarker source "(*Marker0*)")
    assertTooltipContains "x1 param!" (markAtStartOfMarker source "(*Marker1*)")

    assertTooltipContains
        "<summary>Concatenates the string representations of two specified objects.</summary>"
        (markAtStartOfMarker source "(*Marker2*)")

    assertTooltipContains "str of case1" (markAtStartOfMarker source "(*Marker3*)")
    assertTooltipContains "str of case1" (markAtStartOfMarker source "(*Marker4*)")
    assertTooltipContains "value param" (markAtStartOfMarker source "(*Marker5*)")
    assertTooltipContains "value param" (markAtStartOfMarker source "(*Marker6*)")
    assertTooltipContainsWithProvider "Param1 of string" (markAtStartOfMarker source "(*Marker7*)")
    assertTooltipContainsWithProvider "Ignored" (markAtStartOfMarker source "(*Marker8*)")
