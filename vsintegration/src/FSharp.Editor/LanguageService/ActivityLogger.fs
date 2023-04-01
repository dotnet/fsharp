// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Design
open System.Runtime.InteropServices
open System.Threading
open System.IO
open System.Collections.Immutable
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Options
open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.NativeInterop
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.LanguageService
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.ProjectSystem
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Text.Outlining
open Microsoft.CodeAnalysis.ExternalAccess.FSharp
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.VisualStudio.FSharp.Editor.WorkspaceExtensions
open Microsoft.VisualStudio.FSharp.Editor.Telemetry
open System.Threading.Tasks

open System.Diagnostics
open Microsoft.VisualStudio.FSharp.Editor.Logging

module ActivityLogger =
    let startListening() =

        let indent (activity: Activity) =
            let rec loop (activity: Activity) n =
                if activity.Parent <> null then loop (activity.Parent) (n + 1) else n
            String.replicate (loop activity 0) "  "
        let collectTags (activity: Activity) =
            match [ for tag in activity.Tags -> $"{tag.Key}: {tag.Value.ToString()}" ] with
            | [] -> ""
            | tags -> "| " + String.concat ", " tags

        let listener = new ActivityListener(
            ShouldListenTo = (fun source -> source.Name = "fsc"),
            Sample = (fun _ -> ActivitySamplingResult.AllDataAndRecorded),
            //ActivityStarted = (fun a -> logMsg $"{indent a}{a.OperationName}"),
            ActivityStopped = (fun a -> logMsg $"{indent a}{a.OperationName} {collectTags a}")
        )
        ActivitySource.AddActivityListener(listener)

