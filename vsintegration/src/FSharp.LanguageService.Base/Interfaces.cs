// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;
using System.IO;
using System.Globalization;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.FSharp.LanguageService
{

    enum RequireFreshResults
    {
        Yes = 1,
        No = 0
    }

    // The interface between FSharpSourceBase+FSharpSource and the rest of the language service.
    interface ISource : IDisposable
    {
        void Open();
        bool Close();
        bool IsClosed { get; }
        int ChangeCount { get; set; }
        int DirtyTime { get; set; }
        TextSpan GetDocumentSpan();
        IVsTextLines GetTextLines();
        int GetPositionOfLineIndex(int line, int col);
        string GetText();
        string GetText(int startLine, int startCol, int endLine, int endCol);
        void SetText(string newText);
        void SetText(TextSpan span, string newText);
        void ReformatSpan(EditArray mgr, TextSpan span);
        int GetLineLength(int line);
        TextSpan UncommentSpan(TextSpan span);
        TextSpan CommentSpan(TextSpan span);
        ExpansionProvider GetExpansionProvider();
        BackgroundRequest BeginBackgroundRequest(int line, int idx, TokenInfo info, BackgroundRequestReason reason, IVsTextView view, RequireFreshResults requireFreshResults, BackgroundRequestResultHandler callback, MethodTipMiscellany misc = 0);
        CompletionSet CompletionSet { get; }
        void Completion(IVsTextView textView, TokenInfo info, BackgroundRequestReason reason, RequireFreshResults requireFreshResults);
        bool IsCompletorActive { get; }
        void DismissCompletor();
        void ToggleRegions();
        // Called to notify the source that the user has changed the source text in the editor.
        void RecordChangeToView();
        // Called to notify the source the file has been redisplayed.
        void RecordViewRefreshed();
        // If true, the file displayed has changed and needs to be redisplayed to some extent.
        bool NeedsVisualRefresh { get; }
        bool OutliningEnabled { get; set; }
        void DisableOutlining();
        void OnCommand(IVsTextView textView, VsCommands2K command, char ch);
        TokenInfo GetTokenInfo(int line, int col);
        void MethodTip(IVsTextView textView, int line, int index, TokenInfo info, MethodTipMiscellany methodTipMiscellany, RequireFreshResults requireFreshResults);
        void GetPairExtents(IVsTextView textView, int line, int col, out TextSpan span);
        bool GetWordExtent(int line, int idx, WORDEXTFLAGS flags, out int startIdx, out int endIdx);
        int GetLineCount();
        LanguageService LanguageService { get; }
        void Recolorize(int startLine, int endLine);
        TextSpan DirtySpan { get; }
        Colorizer GetColorizer();
        AuthoringSink CreateAuthoringSink(BackgroundRequestReason reason, int line, int col);
        string GetFilePath();
        void OnIdle(bool periodic);
        void HandleUntypedParseOrFullTypeCheckResponse(BackgroundRequest req);
        IVsHiddenTextSession GetHiddenTextSession();
        string GetExpressionAtPosition(int line, int column);
        DateTime OpenedTime { get; }
    }

    /// <summary>
    /// Interface provides inside-VS test hooks for the F# language service.
    /// </summary>
    internal interface ILanguageServiceTestHelper
    {
        /// <summary>
        /// Version number will increment to indicate a change in the semantics of preexisting methods. 
        /// </summary>
        int GetSemanticsVersion();
    }
}
