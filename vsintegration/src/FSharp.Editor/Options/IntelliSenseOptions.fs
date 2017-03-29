namespace Microsoft.VisualStudio.FSharp.Editor.Options

open System.Runtime.InteropServices
open System.Windows
open System.Windows.Controls
open Microsoft.CodeAnalysis.Completion
open Microsoft.VisualStudio.LanguageServices.Implementation.Options

open Microsoft.VisualStudio.FSharp.Editor
open System.Windows.Markup

module IntelliSenseOptionPageStrings =
    let Option_CompletionLists = SR.GetString "Completion_Lists"
    let Option_Show_completion_list_after_a_character_is_typed = SR.GetString "Show_completion_list_after_a_character_is_typed"
    let Option_Show_completion_list_after_a_character_is_deleted = SR.GetString "Show_completion_list_after_a_character_is_deleted"

type IntelliSenseOptionPageControl(serviceProvider) as this =
    inherit AbstractOptionPageControl(serviceProvider)

    let ( *** ) (control : 'T when 'T :> IAddChild) (children: UIElement list) =
        children |> List.iter control.AddChild
        control
    let ( +++ ) (control : 'T when 'T :> ContentControl) content =
        control.Content <- content
        control
    do      
        let Show_completion_list_after_a_character_is_typed = 
            CheckBox (Content = IntelliSenseOptionPageStrings.Option_Show_completion_list_after_a_character_is_typed) 
        let _Show_completion_list_after_a_character_is_deleted = 
            CheckBox (Content = IntelliSenseOptionPageStrings.Option_Show_completion_list_after_a_character_is_deleted)

        this.Content <-
            ScrollViewer (VerticalScrollBarVisibility = ScrollBarVisibility.Auto) 
            +++ StackPanel()
                *** [ GroupBox (Header = IntelliSenseOptionPageStrings.Option_CompletionLists)
                      +++ StackPanel() 
                          *** [ Show_completion_list_after_a_character_is_typed
                                StackPanel (Margin = Thickness(15.0, 0.0, 0.0, 0.0)) 
                                *** [ _Show_completion_list_after_a_character_is_deleted ] ] ]

        this.BindToOption(Show_completion_list_after_a_character_is_typed, CompletionOptions.TriggerOnTypingLetters, "F#")
        //this.BindToOption(Show_completion_list_after_a_character_is_deleted, CompletionOptions.TriggerOnDeletion, "F#")

[<Guid("9b3c6b8a-754a-461d-9ebe-de1a682d57c2")>]
type internal IntelliSenseOptionPage() =
    inherit AbstractOptionPage()
    override __.CreateOptionPage(serviceProvider) =
        IntelliSenseOptionPageControl(serviceProvider) :> _
