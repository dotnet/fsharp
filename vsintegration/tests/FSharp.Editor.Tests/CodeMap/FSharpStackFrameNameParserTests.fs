// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests.CodeMap

open System
open Xunit
open Microsoft.VisualStudio.FSharp.Editor

/// The frame names below were captured from managed stack traces of a sample exercising every
/// construct the F# compiler mangles differently, so they are the shapes a debug engine reports.
module FSharpStackFrameNameParserTests =

    let private renderMember frameMember =
        match frameMember with
        | FrameMethod name -> $"method %s{name}"
        | FrameConstructor -> "ctor"
        | FrameStaticConstructor -> "cctor"
        | FramePropertyGetter name -> $"get %s{name}"
        | FramePropertySetter name -> $"set %s{name}"
        | FrameActivePattern cases -> $"""activepattern %s{String.Join("|", cases)}"""
        | FrameClosureBody origin ->
            let ordinal =
                match origin.Ordinal with
                | ValueSome n -> $"-%d{n}"
                | ValueNone -> ""

            $"closure %s{origin.EnclosingName}@%d{origin.Line}%s{ordinal}"
        | FrameStartupCode -> "startupcode"

    let private render (frame: ParsedFrame) =
        let path =
            frame.Path
            |> List.map (fun segment ->
                if segment.GenericArity = 0 then
                    segment.Name
                else
                    $"%s{segment.Name}`%d{segment.GenericArity}")
            |> String.concat "."

        let arity =
            if frame.MethodGenericArity = 0 then
                ""
            else
                $" <%d{frame.MethodGenericArity}>"

        $"%s{path} :: %s{renderMember frame.Member}%s{arity}"

    let frames: obj[] list =
        [
            // module-level functions: curried and tupled arguments are both flattened by the compiler
            [|
                "GateCSample.Library.tupledFunction"
                "GateCSample.Library :: method tupledFunction"
            |]
            [|
                "GateCSample.Library.curriedFunction"
                "GateCSample.Library :: method curriedFunction"
            |]
            [|
                "GateCSample.Library.recursiveFunction"
                "GateCSample.Library :: method recursiveFunction"
            |]
            [| "GateCSample.Program.main"; "GateCSample.Program :: method main" |]

            // generic methods carry their arguments in brackets
            [|
                "GateCSample.Library.genericFunction[T]"
                "GateCSample.Library :: method genericFunction <1>"
            |]
            [|
                "GateCSample.Library.inlineFunction[a]"
                "GateCSample.Library :: method inlineFunction <1>"
            |]
            [|
                "GateCSample.Library.SampleClass.GenericMember[T]"
                "GateCSample.Library.SampleClass :: method GenericMember <1>"
            |]

            // generic types carry arity on the type segment
            [|
                "GateCSample.Library.GenericClass`1.Get"
                "GateCSample.Library.GenericClass`1 :: method Get"
            |]

            // operators and [<CompiledName>] reach metadata renamed
            [|
                "GateCSample.Library.op_PlusBangPlus"
                "GateCSample.Library :: method op_PlusBangPlus"
            |]
            [|
                "GateCSample.Library.RenamedInMetadata"
                "GateCSample.Library :: method RenamedInMetadata"
            |]

            // active patterns keep their pipes in metadata
            [|
                "GateCSample.Library.|Even|Odd|"
                "GateCSample.Library :: activepattern Even|Odd"
            |]
            [|
                "GateCSample.Library.|Positive|_|"
                "GateCSample.Library :: activepattern Positive|_"
            |]

            // closures, lambdas, local functions and computation-expression bodies all lift to `name@line`
            [|
                "GateCSample.Library.pipelineLambda@42.Invoke"
                "GateCSample.Library :: closure pipelineLambda@42"
            |]
            [|
                "GateCSample.Library.inner@48.Invoke"
                "GateCSample.Library :: closure inner@48"
            |]
            [|
                "GateCSample.Library.outer@47-1.Invoke"
                "GateCSample.Library :: closure outer@47-1"
            |]
            [|
                "GateCSample.Library.localHelper@55.Invoke"
                "GateCSample.Library :: closure localHelper@55"
            |]
            [|
                "GateCSample.Library.asyncFunction@61-1.Invoke"
                "GateCSample.Library :: closure asyncFunction@61-1"
            |]
            [|
                "GateCSample.Library.taskFunction@67-3.Invoke"
                "GateCSample.Library :: closure taskFunction@67-3"
            |]
            [|
                "GateCSample.Library.taskFunction@1-2.Invoke"
                "GateCSample.Library :: closure taskFunction@1-2"
            |]
            [|
                "GateCSample.Library.seqFunction@72.GenerateNext"
                "GateCSample.Library :: closure seqFunction@72"
            |]

            // `@` without a line number is not a closure - the compiler also uses it for debug proxies
            [|
                "GateCSample.Library.Shape.Circle@DebugTypeProxy.Item"
                "GateCSample.Library.Shape.Circle@DebugTypeProxy :: method Item"
            |]

            // constructors
            [|
                "GateCSample.Library.SampleClass..ctor"
                "GateCSample.Library.SampleClass :: ctor"
            |]
            [|
                "GateCSample.Library.SampleClass..cctor"
                "GateCSample.Library.SampleClass :: cctor"
            |]

            // properties
            [|
                "GateCSample.Library.SampleClass.get_Property"
                "GateCSample.Library.SampleClass :: get Property"
            |]
            [|
                "GateCSample.Library.SampleClass.set_Property"
                "GateCSample.Library.SampleClass :: set Property"
            |]

            // members on classes, unions and records
            [|
                "GateCSample.Library.SampleClass.InstanceMethod"
                "GateCSample.Library.SampleClass :: method InstanceMethod"
            |]
            [|
                "GateCSample.Library.SampleClass.StaticMethod"
                "GateCSample.Library.SampleClass :: method StaticMethod"
            |]
            [|
                "GateCSample.Library.Shape.Area"
                "GateCSample.Library.Shape :: method Area"
            |]
            [|
                "GateCSample.Library.Point.Norm"
                "GateCSample.Library.Point :: method Norm"
            |]

            // an explicit interface implementation embeds the whole interface name in the method name
            [|
                "GateCSample.Library.Greeter.GateCSample.Library.IGreeter.Greet"
                "GateCSample.Library.Greeter.GateCSample.Library.IGreeter :: method Greet"
            |]

            // nested modules, and the `Module` suffix added when a module collides with a type
            [|
                "GateCSample.Library.Nested.nestedFunction"
                "GateCSample.Library.Nested :: method nestedFunction"
            |]
            [|
                "GateCSample.Library.Nested.Deeper.deeperFunction"
                "GateCSample.Library.Nested.Deeper :: method deeperFunction"
            |]
            [|
                "GateCSample.Library.CollisionModule.helper"
                "GateCSample.Library.CollisionModule :: method helper"
            |]

            // module-level initialization
            [|
                "<StartupCode$GateCSample>.$GateCSample.Library.main@"
                "GateCSample.Library :: startupcode"
            |]

            // nested types may be reported with the metadata separator instead of a dot
            [|
                "GateCSample.Library+SampleClass.InstanceMethod"
                "GateCSample.Library.SampleClass :: method InstanceMethod"
            |]

            // a parameter list is decoration, not identity
            [|
                "GateCSample.Library.tupledFunction(Int32 a, String b)"
                "GateCSample.Library :: method tupledFunction"
            |]
            [|
                "GateCSample.Library.SampleClass..ctor(Int32 seed)"
                "GateCSample.Library.SampleClass :: ctor"
            |]
        ]

    [<Theory>]
    [<MemberData(nameof frames)>]
    let ``Frame name is parsed into path and member`` (frameName: string) (expected: string) =
        match FSharpStackFrameNameParser.parse frameName with
        | ValueNone -> failwith $"expected %s{frameName} to parse"
        | ValueSome frame -> Assert.Equal(expected, render frame)

    [<Theory>]
    [<InlineData("")>]
    [<InlineData("   ")>]
    [<InlineData(null)>]
    let ``Blank frame names do not parse`` (frameName: string) =
        Assert.True((FSharpStackFrameNameParser.parse frameName).IsNone)
