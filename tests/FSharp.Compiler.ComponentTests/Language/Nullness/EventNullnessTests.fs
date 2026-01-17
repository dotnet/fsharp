module Language.EventNullnessTests

open Xunit
open FSharp.Test.Compiler

// Test for https://github.com/dotnet/fsharp/issues/18361
// INotifyPropertyChanged CLIEvent causes FS3261 nullness warning
// 
// This test documents the current buggy behavior. When the bug is fixed,
// change `shouldFail` to `shouldSucceed` and remove `withDiagnostics`.
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
    // BUG: Currently produces FS3261 warning - this should succeed without warnings
    // When the bug is fixed, replace with: |> shouldSucceed
    |> shouldFail
    |> withDiagnostics [
        (Warning 3261, Line 11, Col 39, Line 11, Col 62, 
         "Nullness warning: A non-nullable 'PropertyChangedEventHandler' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression.")
    ]

// Test for https://github.com/dotnet/fsharp/issues/18349
// ICommand CLIEvent CanExecuteChanged causes FS3261 nullness warning
// 
// This test documents the current buggy behavior. When the bug is fixed,
// change `shouldFail` to `shouldSucceed` and remove `withDiagnostics`.
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
    // BUG: Currently produces FS3261 warning - this should succeed without warnings
    // When the bug is fixed, replace with: |> shouldSucceed
    |> shouldFail
    |> withDiagnostics [
        (Warning 3261, Line 12, Col 38, Line 12, Col 63, 
         "Nullness warning: A non-nullable 'EventHandler' was expected but this expression is nullable. Consider either changing the target to also be nullable, or use pattern matching to safely handle the null case of this expression.")
    ]
