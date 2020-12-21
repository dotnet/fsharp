// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.ProjectSystem

open System
open System.Runtime.InteropServices
open Microsoft.VisualStudio.Shell.Interop

type internal WaitDialogOptions =
    {
        WaitCaption : string
        WaitMessage : string
        ProgressText : string option
        StatusBmpAnim : obj
        StatusBarText : string option
        DelayToShowDialogSecs : int
        IsCancelable : bool
        ShowMarqueeProgress : bool
    }

module internal WaitDialog =

    let start (sp : IServiceProvider) (options : WaitDialogOptions) =
        let waitDialogFactory = sp.GetService(typeof<SVsThreadedWaitDialogFactory>) :?> IVsThreadedWaitDialogFactory
        let waitDialog = ref null
        waitDialogFactory.CreateInstance waitDialog |> Marshal.ThrowExceptionForHR

        waitDialog.Value.StartWaitDialog(
            szWaitCaption = options.WaitCaption,
            szWaitMessage = options.WaitMessage,
            szProgressText = Option.toObj options.ProgressText,
            varStatusBmpAnim = options.StatusBmpAnim,
            szStatusBarText = Option.toObj options.StatusBarText,
            iDelayToShowDialog = options.DelayToShowDialogSecs,
            fIsCancelable = options.IsCancelable,
            fShowMarqueeProgress = options.ShowMarqueeProgress
            )
        |> Marshal.ThrowExceptionForHR

        { new IDisposable with
            override _.Dispose () =
                let cancelled = ref 0
                waitDialog.Value.EndWaitDialog cancelled |> Marshal.ThrowExceptionForHR
        }