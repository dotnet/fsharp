module Conformance.Expressions.CEExtensionMethodCapture

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``CE doesn't capture an extension method beyond the access domain``() =
    FSharp """
        open System.Runtime.CompilerServices

        type AsyncSeq<'T>(i: 'T) = 
            class
            let l = [i]
            member this.Data = l
            end

        type AsyncSeqBuilder() =
            member _.Yield(x: 'T) : AsyncSeq<'T> =
                AsyncSeq(x)

        [<Extension>]
        type PrivateExtensions =
            [<Extension>]
            static member inline private Run(this: AsyncSeqBuilder) =
                this

        let asyncSeq = AsyncSeqBuilder()

        let xs : AsyncSeq<int> =
            asyncSeq {
                yield 1
            }
    """
    |> asExe
    |> compile
    |> shouldSucceed

[<Fact>]
let ``CE doesn't capture an extension method with generic type``() =
    FSharp """
        open System.Runtime.CompilerServices

        type FooClass = class end

        type AsyncSeq<'T>(i: 'T) = 
            class
            let l = [i]
            member this.Data = l
            end

        type AsyncSeqBuilder() =
            member _.Yield(x: 'T) : AsyncSeq<'T> =
                AsyncSeq(x)

        [<Extension>]
        type PublicExtensions =
            [<Extension>]
            static member inline Run(this: #FooClass) =
                this

        let asyncSeq = AsyncSeqBuilder()

        let xs : AsyncSeq<int> =
            asyncSeq {
                yield 1
            }
    """
    |> asExe
    |> compile
    |> shouldSucceed

// Deliberately trigger an error to ensure that a method is captured
[<Fact>]
let ``CE captures a public extension method and procudes an error due to invalid args``() =
    FSharp """
        open System.Runtime.CompilerServices

        type AsyncSeq<'T>(i: 'T) = 
            class
            let l = [i]
            member this.Data = l
            end

        type AsyncSeqBuilder() =
            member _.Yield(x: 'T) : AsyncSeq<'T> =
                AsyncSeq(x)

        [<Extension>]
        type PublicExtensions =
            [<Extension>]
            static member inline Run(this: AsyncSeqBuilder, invalidArg: string) =
                this

        let asyncSeq = AsyncSeqBuilder()

        let xs : AsyncSeq<int> =
            asyncSeq {
                yield 1
            }
    """
    |> asExe
    |> compile
    |> shouldFail

// Deliberately trigger an error to ensure that a method is captured
[<Fact>]
let ``CE captures a public extension method with valid generic constrainted type and procudes an error due to invalid args``() =
    FSharp """
        open System.Runtime.CompilerServices

        type AsyncSeq<'T>(i: 'T) = 
            class
            let l = [i]
            member this.Data = l
            end

        type AsyncSeqBuilder() =
            member _.Yield(x: 'T) : AsyncSeq<'T> =
                AsyncSeq(x)

        [<Extension>]
        type PublicExtensions =
            [<Extension>]
            static member inline Run(this: #AsyncSeqBuilder, invalidArg: string) =
                this

        let asyncSeq = AsyncSeqBuilder()

        let xs : AsyncSeq<int> =
            asyncSeq {
                yield 1
            }
    """
    |> asExe
    |> compile
    |> shouldFail

// Deliberately trigger an error to ensure that a method is captured
[<Fact>]
let ``CE captures a public extension method with generic type and procudes an error due to invalid args``() =
    FSharp """
        open System.Runtime.CompilerServices

        type AsyncSeq<'T>(i: 'T) = 
            class
            let l = [i]
            member this.Data = l
            end

        type AsyncSeqBuilder() =
            member _.Yield(x: 'T) : AsyncSeq<'T> =
                AsyncSeq(x)

        [<Extension>]
        type PublicExtensions =
            [<Extension>]
            static member Run(this: 'T, invalidArg: string) =
                this

        let asyncSeq = AsyncSeqBuilder()

        let xs : AsyncSeq<int> =
            asyncSeq {
                yield 1
            }
    """
    |> asExe
    |> compile
    |> shouldFail


// ---------------------------------------------------------------------------
// Regression tests for https://github.com/dotnet/fsharp/issues/19936
// FS1110 internal compiler error in `task { let! }` when a generic IL
// extension method (with a method-typar `this`-arg) is in scope via `open`.
// Reproduction requires a C# library because F# extensions take the FSMeth
// path which never crashed.
// ---------------------------------------------------------------------------

module Issue19936 =

    /// Minimal C# library reproducing the ReactiveUI shape:
    /// a static extension class with a generic method-typar `this`-arg
    /// constrained by a generic interface that itself mentions another
    /// method-typar. This shape is what triggers FS1110 on HEAD when the
    /// extension is enumerated during CE method resolution.
    let private csLib =
        CSharp """
using System;
using System.Runtime.CompilerServices;

namespace ReactiveUIRepro
{
    public interface IViewFor<TModel> where TModel : class
    {
        TModel ViewModel { get; set; }
    }

    public static class PropertyBindingMixins
    {
        public static IDisposable Bind<TView, TModel, TValue>(this TView view, TModel viewModel, TValue value)
            where TView : class, IViewFor<TModel>
            where TModel : class
        {
            return null;
        }
    }
}
        """
        |> withName "ReactiveUIRepro"

    [<Fact>]
    let ``Issue 19936 - task let! type-checks when generic IL extension method is in scope``() =
        FSharp """
module Repro
open ReactiveUIRepro
open System.Threading.Tasks

let chooser () : Task<string> = Task.FromResult("file.txt")

let result : Task<string> =
    task {
        let! file = chooser ()
        return file
    }
        """
        |> withReferences [ csLib ]
        |> asExe
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Issue 19936 - task return! type-checks when generic IL extension method is in scope``() =
        let csLib2 =
            CSharp """
using System;
using System.Runtime.CompilerServices;

namespace ReactiveUIRepro2
{
    public interface IViewFor<TModel> where TModel : class
    {
        TModel ViewModel { get; set; }
    }

    public static class ReturnFromMixins
    {
        public static IDisposable ReturnFrom<TView, TModel, TValue>(this TView view, TModel viewModel, TValue value)
            where TView : class, IViewFor<TModel>
            where TModel : class
        {
            return null;
        }
    }
}
            """
            |> withName "ReactiveUIRepro2"

        FSharp """
module Repro
open ReactiveUIRepro2
open System.Threading.Tasks

let chooser () : Task<string> = Task.FromResult("file.txt")

let result : Task<string> =
    task {
        return! chooser ()
    }
        """
        |> withReferences [ csLib2 ]
        |> asExe
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Issue 19936 - task use! type-checks when generic IL extension method is in scope``() =
        FSharp """
module Repro
open ReactiveUIRepro
open System
open System.Threading.Tasks

type Resource() =
    interface IDisposable with
        member _.Dispose() = ()

let acquire () : Task<Resource> = Task.FromResult(new Resource())

let result : Task<int> =
    task {
        use! r = acquire ()
        return 1
    }
        """
        |> withReferences [ csLib ]
        |> asExe
        |> compile
        |> shouldSucceed

    /// Regression guard: an in-scope C# generic-extension named `Bind` whose
    /// `this`-typar is constrained by a generic *class* (instead of a generic
    /// interface) - the same broken-import shape via a different metadata
    /// channel - must not crash `task { let! }` resolution.
    [<Fact>]
    let ``Issue 19936 - task let! type-checks when class-constrained generic IL Bind extension is in scope``() =
        let cs =
            CSharp """
using System;
using System.Runtime.CompilerServices;

namespace ClassConstrainedRepro
{
    public abstract class BaseView<TModel> where TModel : class
    {
        public TModel ViewModel { get; set; }
    }

    public static class ClassBindMixins
    {
        public static int Bind<TView, TModel, TValue>(this TView view, TModel viewModel, TValue value)
            where TView : BaseView<TModel>
            where TModel : class
        {
            return 0;
        }
    }
}
            """
            |> withName "ClassConstrainedRepro"

        FSharp """
module Repro
open ClassConstrainedRepro
open System.Threading.Tasks

let chooser () : Task<int> = Task.FromResult(42)

let result : Task<int> =
    task {
        let! v = chooser ()
        return v + 1
    }
        """
        |> withReferences [ cs ]
        |> asExe
        |> compile
        |> shouldSucceed

    /// Preservation guard for PR #19536's original intent (issue #19349):
    /// the inaccessible private extension Run must STILL be skipped so the CE
    /// compiles. (Mirrors the first existing test in this file, but pairs it
    /// with the issue #19936 fix to confirm we did not regress the original fix.)
    [<Fact>]
    let ``Issue 19936 - PR 19536 fix is preserved: inaccessible private extension is still skipped``() =
        FSharp """
open System.Runtime.CompilerServices

type AsyncSeq<'T>(i: 'T) =
    class
    let l = [i]
    member this.Data = l
    end

type AsyncSeqBuilder() =
    member _.Yield(x: 'T) : AsyncSeq<'T> =
        AsyncSeq(x)

[<Extension>]
type PrivateExtensions =
    [<Extension>]
    static member inline private Run(this: AsyncSeqBuilder) =
        this

let asyncSeq = AsyncSeqBuilder()

let xs : AsyncSeq<int> =
    asyncSeq {
        yield 1
    }
        """
        |> asExe
        |> compile
        |> shouldSucceed