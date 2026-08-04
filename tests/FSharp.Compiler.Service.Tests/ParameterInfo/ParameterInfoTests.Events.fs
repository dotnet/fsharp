module FSharp.Compiler.Service.Tests.ParameterInfoEventsTests

open Xunit

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Single.Generics.EventHandler`` () =
    assertParameterInfoOverloads [ [""] ] "open System\nnew System.EventHandler( {caret}"

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Single.Generics.EventHandlerEventArgs`` () =
    assertParameterInfoOverloads [ [""] ] "open System\nSystem.EventHandler<EventArgs>({caret}"

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Single.Generics.EventHandlerEventArgsNew`` () =
    assertParameterInfoOverloads [ [""] ] "open System\nnew System.EventHandler<EventArgs> ( {caret}"
