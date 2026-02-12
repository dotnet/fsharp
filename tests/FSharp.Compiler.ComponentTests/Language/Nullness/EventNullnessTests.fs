module Language.EventNullnessTests

open Xunit
open FSharp.Test.Compiler

// Test for https://github.com/dotnet/fsharp/issues/18361
// INotifyPropertyChanged CLIEvent causes FS3261 nullness warning
// 
// Fixed: The compiler now correctly handles delegate nullness in CLIEvent properties.
[<Fact>]
let ``INotifyPropertyChanged CLIEvent should not produce nullness warning`` () =
    FSharp """
module TestModule

open System.ComponentModel

type XViewModel() =
    let propertyChanged = Event<PropertyChangedEventHandler, PropertyChangedEventArgs>()
    
    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged = propertyChanged.Publish
    """
    |> asLibrary
    |> withLangVersionPreview
    |> withCheckNulls
    |> withWarnOn 3261
    |> compile
    |> shouldSucceed

// Test for https://github.com/dotnet/fsharp/issues/18349
// ICommand CLIEvent CanExecuteChanged causes FS3261 nullness warning
// 
// Fixed: The compiler now correctly handles delegate nullness in CLIEvent properties.
[<Fact>]
let ``ICommand CanExecuteChanged CLIEvent should not produce nullness warning`` () =
    FSharp """
module TestModule

open System
open System.Windows.Input

type Command() =
    let canExecuteChanged = Event<EventHandler, EventArgs>()
    
    interface ICommand with
        [<CLIEvent>]
        member _.CanExecuteChanged = canExecuteChanged.Publish
        
        member _.CanExecute(_parameter) = true
        member _.Execute(_parameter) = ()
    """
    |> asLibrary
    |> withLangVersionPreview
    |> withCheckNulls
    |> withWarnOn 3261
    |> compile
    |> shouldSucceed
