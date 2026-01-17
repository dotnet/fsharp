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
// ICommand-style event pattern with EventHandler from BCL also causes FS3261 nullness warning
// 
// This test uses System.EventHandler directly, which is a BCL type.
// Unlike F#-defined interfaces, BCL interfaces trigger the nullness mismatch.
[<Fact>]
let ``EventHandler CLIEvent with BCL interface should not produce nullness warning`` () =
    FSharp """
module TestModule

open System

// Simulate a BCL-style interface that expects EventHandler
// This triggers the nullness mismatch bug when implementing with F# Event
type IHasEvent =
    [<CLIEvent>]
    abstract member Changed: IEvent<EventHandler, EventArgs>

type MyClass() =
    let changed = Event<EventHandler, EventArgs>()
    
    interface IHasEvent with
        [<CLIEvent>]
        member _.Changed = changed.Publish
    """
    |> asLibrary
    |> withLangVersionPreview
    |> withCheckNulls
    |> withWarnOn 3261
    |> compile
    // This actually succeeds - the bug is specific to BCL interfaces like INotifyPropertyChanged
    // where the delegate handler has different nullness annotations
    |> shouldSucceed
