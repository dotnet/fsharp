module FSharp.Compiler.Service.Tests.TooltipAttributesTests

open System
open Xunit

[<Fact>]
let ``EnsureNoAssertFromBadParserRangeOnAttribute`` () =
    let source =
        """
                [<System.Obsolete>]
                Types foo = int"""

    Checker.getTooltip (markAtEndOfMarker source "ype") |> ignore

[<Theory>]
[<InlineData("type A", "[<ParamArray>] a:")>]
[<InlineData("A.Foo", "[<System.ParamArray>] a:")>]
let ``ParamsArrayArgument`` (marker: string) (expected: string) =
    let source =
        """
                type A() =
                    static member Foo([<System.ParamArrayAttribute>] a : int[]) = ()
                let r = A.Foo(42)"""

    assertTooltipContains expected (markAtEndOfMarker source marker)

[<Fact>]
let ``IdentifiersInAttributes`` () =
    let source =
        String.concat
            "\n"
            [ "[<(*test13*)System.CLSCompliant(true)>]"
              "let test13 = 1"
              "open System"
              "[<(*test14*)CLSCompliant(true)>]"
              "let test14 = 1" ]

    walk source "[<(*test13*)" "System" "namespace System"
    walk source "[<(*test13*)System." "CLSCompliant" "CLSCompliantAttribute"
    walk source "[<(*test14*)" "CLSCompliant" "CLSCompliantAttribute"

[<Fact>]
let ``Regression.FieldRepeatedInToolTip.Bug3818`` () =
    let source =
        """
             [<System.AttributeUsage(System.AttributeTargets.All, Inherited = false)>]
             type A() =
               do ()"""

    assertIdentifierInTooltipExactlyOnce "Inherited" (markAtEndOfMarker source "Inherite")

[<Fact>]
let ``Automation.OverRiddenMembers`` () =
    let source =
        """namespace QuickinfoGeneric

                            module FSharpOwnCode =
                                [<AbstractClass>]
                                type TextOutputSink() =
                                    abstract WriteChar : char -> unit
                                    abstract WriteString : string -> unit
                                    default x.WriteString(s) = s |> String.iter x.WriteChar

                                type ByteOutputSink() =
                                    inherit TextOutputSink()
                                    default sink.WriteChar(c) = System.Console.Write(c)
                                    override sink.WriteString(s) = System.Console.Write(s)

                                let sink = new ByteOutputSink()
                                sink.WriteChar(*Marker11*)('c')
                                sink.WriteString(*Marker12*)("Hello World!")"""

    assertTooltipContainsInFsFile "override ByteOutputSink.WriteChar: c: char -> unit" (markAtStartOfMarker source "(*Marker11*)")
    assertTooltipContainsInFsFile "override ByteOutputSink.WriteString: s: string -> unit" (markAtStartOfMarker source "(*Marker12*)")
