// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The frame names below were captured from managed stack traces of a sample exercising every
/// construct the F# compiler mangles differently, so they are the shapes a debug engine reports.
module FSharp.Editor.Tests.CodeMap.FSharpStackFrameNameParserTests

open System
open Xunit
open Microsoft.VisualStudio.FSharp.Editor

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
        |> Seq.map (fun segment ->
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

        // malformed `@` suffixes must not be mistaken for closures
        [|
            "GateCSample.Library.trailingAt@"
            "GateCSample.Library :: method trailingAt@"
        |]
        [|
            "GateCSample.Library.trailingDash@12-"
            "GateCSample.Library :: method trailingDash@12-"
        |]
        [|
            "GateCSample.Library.notDigits@1a2"
            "GateCSample.Library :: method notDigits@1a2"
        |]
        [|
            "GateCSample.Library.twoDashes@12-3-4"
            "GateCSample.Library :: method twoDashes@12-3-4"
        |]
        [| "GateCSample.Library.@12"; "GateCSample.Library :: method @12" |]

        // the last `@` wins, so a closure lifted out of an already-mangled name still parses
        [|
            "GateCSample.Library.outer@10.inner@20.Invoke"
            "GateCSample.Library.outer@10 :: closure inner@20"
        |]

        // the debug engine formats differently from metadata: instantiations in angle brackets,
        // a constructor as `Type.Type`, an accessor as a trailing `.get`/`.set` segment, and a
        // trailing `T` on generic closure classes
        [|
            "ClassLibrary.Demo.genericFunction<int>"
            "ClassLibrary.Demo :: method genericFunction <1>"
        |]
        [|
            "ClassLibrary.Box<System.String>.Unwrap"
            "ClassLibrary.Box`1 :: method Unwrap"
        |]
        [| "ClassLibrary.Initialized.Initialized"; "ClassLibrary.Initialized :: ctor" |]
        [| "ClassLibrary.Worker.Computed.get"; "ClassLibrary.Worker :: get Computed" |]
        [| "ClassLibrary.Worker.Tuned.set"; "ClassLibrary.Worker :: set Tuned" |]
        [|
            "ClassLibrary.Demo.helperTwo@42T<int>.Invoke"
            "ClassLibrary.Demo :: closure helperTwo@42"
        |]
        [| "M.f@10T.Invoke"; "M :: closure f@10" |]

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

        // Captured verbatim from a Code Map's DGML - these are the frames the debug engine really
        // produced for the demo, and each one is a shape that once reached the map unresolved.
        [| "<StartupCode$ClassLibrary>.$Demo.$Demo"; "Demo.Demo :: startupcode" |]
        [|
            "ClassLibrary.Demo.work@107-3.Invoke"
            "ClassLibrary.Demo :: closure work@107-3"
        |]

        // a computation-expression body or pipeline stage is named by a phrase, spaces and all
        [|
            "ClassLibrary.Demo.Pipe #1 input at line 97@99-1.Invoke"
            "ClassLibrary.Demo :: closure Pipe #1 input at line 97@99-1"
        |]
        [|
            "ClassLibrary.Demo.Pipe #1 stage #3 at line 26@26.Invoke"
            "ClassLibrary.Demo :: closure Pipe #1 stage #3 at line 26@26"
        |]

        // FSharp.Core's async/task/seq plumbing. Nothing resolves these - they are marked external so
        // the map folds them away - but they must still parse, because a frame that does not parse is
        // never seen by the provider and so reaches the map as a bare `Invoke` or `MoveNext`.
        [|
            "<StartupCode$FSharp-Core>.$Async.Sleep@1814-3.Invoke"
            "Async.Sleep@1814-3.Invoke :: startupcode"
        |]
        [|
            "<StartupCode$FSharp-Core>.$Tasks.resumptionInfo@159<int>.MoveNext"
            "Tasks.resumptionInfo@159`1.MoveNext :: startupcode"
        |]
        [|
            "Microsoft.FSharp.Core.CompilerServices.ResumableStateMachine<Microsoft.FSharp.Control.TaskStateMachineData<int>>.System.Runtime.CompilerServices.IAsyncStateMachine.MoveNext"
            "Microsoft.FSharp.Core.CompilerServices.ResumableStateMachine`1.System.Runtime.CompilerServices.IAsyncStateMachine :: method MoveNext"
        |]
        [|
            "Microsoft.FSharp.Collections.ListModule.Map<int, int>"
            "Microsoft.FSharp.Collections.ListModule :: method Map <2>"
        |]
        [|
            "Microsoft.FSharp.Primitives.Basics.List.map<int, int>"
            "Microsoft.FSharp.Primitives.Basics.List :: method map <2>"
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
