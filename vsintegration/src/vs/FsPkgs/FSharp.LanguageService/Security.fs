// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService 

open System
open System.Collections.Generic
open System.Collections
open System.ComponentModel
open System.ComponentModel.Design
open System.Diagnostics

open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Shell.Flavor
open Microsoft.VisualStudio

open Internal.Utilities.Debug

open System.Windows.Controls

module internal TypeProviderSecurityGlobals =
    //+++GLOBAL STATE
    // This is poked with a value once, when the LS first initializes (Servicem.fs: CreateService).
    // It is a convenient way for the LS or the PS to invalidate all LS cached results when the set of approved TPs changes.
    let mutable invalidationCallback = Unchecked.defaultof<unit -> unit>

// TODO these are currently named A/B like the spec, but choose better names
/// There are two dialogs, which we refer to as A and B
///  - A: file in editor buffer needs TP reference (may be from project, or a script file)
///  - B: build a project that needs a TP
[<RequireQualifiedAccess>]
type internal TypeProviderSecurityDialogKind =
    | A
    | B

type internal TypeProviderSecurityDialog =
    // if kind=A and projectName is null, then this is a script with a #r
    static member ShowModal(kind, filename, projectName, assem, pubInfo) =
        let originalWidth = 600.0
        let overallMargin = 12.0
        let auto = System.Windows.GridLength(1.0, System.Windows.GridUnitType.Auto)
        let addAt(grid:Grid, col, row, element) =
            Grid.SetRow(element, row)
            Grid.SetColumn(element, col)
            grid.Children.Add(element) |> ignore

        let assem = Microsoft.FSharp.Compiler.ExtensionTyping.ApprovalIO.partiallyCanonicalizeFileName assem
        match kind with
        | TypeProviderSecurityDialogKind.A -> assert(filename <> null || projectName <> null)
        | TypeProviderSecurityDialogKind.B -> assert(filename = null && projectName <> null)

        let di = new Microsoft.VisualStudio.PlatformUI.DialogWindow("type_provider_security_FS") // calling this constructor creates help topic link for F1 and [?] in top right corner
        di.Title <- Strings.GetString "TPSEC_AB_Title"
        let sp = new StackPanel(Orientation = Orientation.Vertical)
                    
        let mkText s = new TextBlock(TextWrapping=System.Windows.TextWrapping.Wrap, Text=s)
        let mkTextBold s = new TextBlock(TextWrapping=System.Windows.TextWrapping.Wrap, FontWeight=System.Windows.FontWeights.Bold, Text=s) 

        let tbTop =
            match kind, projectName with
            | TypeProviderSecurityDialogKind.A, null -> mkText (Strings.GetString "TPSEC_A_Script_Start")
            | TypeProviderSecurityDialogKind.A, _    -> mkText (Strings.GetString "TPSEC_A_Project_Start")
            | TypeProviderSecurityDialogKind.B, _    -> mkText (Strings.GetString "TPSEC_B_Start")

        let gridTopInfo =
            let grid = new Grid()
            grid.ColumnDefinitions.Add(new ColumnDefinition(Width=auto))
            grid.ColumnDefinitions.Add(new ColumnDefinition(Width=System.Windows.GridLength(8.0)))  //empty column yields whitespace 
            grid.ColumnDefinitions.Add(new ColumnDefinition(Width=System.Windows.GridLength(1.0, System.Windows.GridUnitType.Star)))
            let mutable row = 0
            // Publisher info
            grid.RowDefinitions.Add(new RowDefinition(Height=auto))
            addAt(grid, 0, row, mkText (Strings.GetString "TPSEC_PublisherInfo"))
            addAt(grid, 2, row, mkText (match pubInfo with Some(s) -> s | None -> (Strings.GetString "TPSEC_UnknownPublisher")))
            row <- row + 1
            // TP assembly
            grid.RowDefinitions.Add(new RowDefinition(Height=auto))
            addAt(grid, 0, row, mkText (Strings.GetString "TPSEC_TypeProviderReferenced"))
            addAt(grid, 2, row, mkText assem)
            row <- row + 1
            // File
            if filename <> null then
                grid.RowDefinitions.Add(new RowDefinition(Height=auto))
                addAt(grid, 0, row, mkText (Strings.GetString "TPSEC_File"))
                addAt(grid, 2, row, mkText filename)
            // Project
            if projectName <> null then
                grid.RowDefinitions.Add(new RowDefinition(Height=auto))
                addAt(grid, 0, row, mkText (Strings.GetString "TPSEC_Project"))
                addAt(grid, 2, row, mkText projectName)

            grid.Margin <- new System.Windows.Thickness(0.0, 8.0, 0.0, 8.0)
            grid

        let tbQuestion = mkTextBold (Strings.GetString "TPSEC_TopQuestion")
        tbQuestion.Margin <- new System.Windows.Thickness(0.0, 0.0, 0.0, 8.0)

        let tbFinish = mkText (Strings.GetString "TPSEC_AB_Explain")
        tbFinish.Margin <- new System.Windows.Thickness(0.0, 2.0, 0.0, 0.0)

        sp.Children.Add(tbTop) |> ignore
        sp.Children.Add(gridTopInfo) |> ignore
        sp.Children.Add(tbQuestion) |> ignore

        (* see if UX wants the link too 
        let moreInfoRun = new System.Windows.Documents.Run(Strings.GetString "TPSEC_MoreInfoLink")
        let moreInfoLink = new System.Windows.Documents.Hyperlink(moreInfoRun)
        moreInfoLink.NavigateUri <- System.Uri("http://msdn.com") // TODO url
        let moreInfo = new TextBlock()
        moreInfo.Inlines.Add(moreInfoLink)
        moreInfo.Margin <- new System.Windows.Thickness(0.0, 0.0, 0.0, 11.0)
        sp.Children.Add(moreInfo) |> ignore
        *)
                    
        let enabled = ref false
        let enableButton = new Button(Content=Strings.GetString "TPSEC_Enable", MinWidth=75.0, MinHeight=23.0)
        enableButton.Click.Add(fun _ -> enabled := true; di.Close())
        let disableButton = new Button(Content=Strings.GetString "TPSEC_Disable", HorizontalAlignment=System.Windows.HorizontalAlignment.Right, MinWidth=75.0, MinHeight=23.0)
        disableButton.Margin <- new System.Windows.Thickness(7.0, 0.0, 0.0, 0.0)
        disableButton.Click.Add(fun _ -> di.Close())
        let buttonPanel = new StackPanel(Orientation = Orientation.Horizontal, HorizontalAlignment=System.Windows.HorizontalAlignment.Right)
        buttonPanel.Children.Add(enableButton) |> ignore
        buttonPanel.Children.Add(disableButton) |> ignore
        sp.Children.Add(buttonPanel) |> ignore

        let doNotShowAgain = ref false
        let doNotShowAgainCheckbox = new CheckBox(Content=Strings.GetString "TPSEC_DoNotShowAgain")
        doNotShowAgainCheckbox.Click.Add(fun ea ->
            let checkbox = ea.Source :?> CheckBox
            doNotShowAgain := checkbox.IsChecked.HasValue && checkbox.IsChecked.Value)
        let doNotShowAgainPanel = new StackPanel(Orientation = Orientation.Horizontal, HorizontalAlignment=System.Windows.HorizontalAlignment.Left)
        doNotShowAgainPanel.Children.Add(doNotShowAgainCheckbox) |> ignore
        sp.Children.Add(doNotShowAgainPanel) |> ignore

        let separator = new System.Windows.Controls.Separator()
        separator.Margin <- new System.Windows.Thickness(0.0, 8.0, 0.0, 0.0)
        sp.Children.Add(separator) |> ignore

        let theIcon =
            let image = new Image()
            let iconUri = new Uri("pack://application:,,,/FSharp.LanguageService;component/SecurityIconForTypeProviders.ico", UriKind.Absolute)
            try
                use iconStream = System.Windows.Application.GetResourceStream(iconUri).Stream // dispose ok, thanks to OnLoad caching on next line
                let decoder = new System.Windows.Media.Imaging.IconBitmapDecoder(iconStream, System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat, System.Windows.Media.Imaging.BitmapCacheOption.OnLoad)
                image.Source <- decoder.Frames.[0]
            with 
                | :? System.IO.IOException -> ()
            image
        let iconAndFinishGrid =
            let grid = new Grid()
            grid.ColumnDefinitions.Add(new ColumnDefinition(Width=System.Windows.GridLength(32.0)))  // icon width
            grid.ColumnDefinitions.Add(new ColumnDefinition(Width=System.Windows.GridLength(8.0)))   // space between icon and text
            grid.ColumnDefinitions.Add(new ColumnDefinition(Width=System.Windows.GridLength(0.90, System.Windows.GridUnitType.Star)))
            grid.RowDefinitions.Add(new RowDefinition(Height=auto))
            addAt(grid, 0, 0, theIcon)
            addAt(grid, 2, 0, tbFinish)
            grid
        iconAndFinishGrid.Margin <- new System.Windows.Thickness(0.0, 8.0, 0.0, 0.0)
        sp.Children.Add(iconAndFinishGrid) |> ignore

        sp.Margin <- new System.Windows.Thickness(overallMargin)
        let scrollviewer = new ScrollViewer()
        scrollviewer.Content <- sp
        scrollviewer.HorizontalScrollBarVisibility <- ScrollBarVisibility.Disabled
        scrollviewer.VerticalScrollBarVisibility <- ScrollBarVisibility.Auto 
        di.Content <- scrollviewer
        di.SizeToContent <- System.Windows.SizeToContent.Height 
        di.Width <- originalWidth
        di.HorizontalAlignment <- System.Windows.HorizontalAlignment.Center 
        di.VerticalAlignment <- System.Windows.VerticalAlignment.Center
        di.HasHelpButton <- false
        di.WindowStartupLocation <- System.Windows.WindowStartupLocation.CenterOwner
        di.Loaded.Add (fun _ -> System.Windows.Input.Keyboard.Focus(disableButton) |> ignore)
        di.ShowModal() |> ignore
        if !doNotShowAgain
        then Microsoft.FSharp.Compiler.ExtensionTyping.ApprovalsChecking.setAlwaysTrust true
        else
            let approval =
                if !enabled then
                    Microsoft.FSharp.Compiler.ExtensionTyping.ApprovalIO.TypeProviderApprovalStatus.Trusted(assem)
                else
                    Microsoft.FSharp.Compiler.ExtensionTyping.ApprovalIO.TypeProviderApprovalStatus.NotTrusted(assem)
            Microsoft.FSharp.Compiler.ExtensionTyping.ApprovalIO.ReplaceApprovalStatus None approval
            // invalidate any language service caching
            TypeProviderSecurityGlobals.invalidationCallback()
                
        