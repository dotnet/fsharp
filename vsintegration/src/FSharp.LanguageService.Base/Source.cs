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
using Microsoft.VisualStudio.FSharp.LanguageService.Resources;

using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.FSharp.Control;

namespace Microsoft.VisualStudio.FSharp.LanguageService
{
    internal enum Severity
    {
        Hint,
        Warning,
        Error,
        Fatal
    };

    internal struct CommentInfo
    {
        private string lineStart;
        private string blockStart;
        private string blockEnd;
        private bool useLineComments;


        internal string LineStart
        {
            get
            {
                return this.lineStart;
            }
            set
            {
                this.lineStart = value;
            }
        }

        internal string BlockStart
        {
            get
            {
                return this.blockStart;
            }
            set
            {
                this.blockStart = value;
            }
        }

        internal string BlockEnd
        {
            get
            {
                return this.blockEnd;
            }
            set
            {
                this.blockEnd = value;
            }
        }

        internal bool UseLineComments
        {
            get
            {
                return this.useLineComments;
            }
            set
            {
                this.useLineComments = value;
            }
        }

    }

    internal static class SourceConstants
    {
        internal static int HiddenRegionCookie = 25;
    }

    //===================================================================================
    // Default Implementations
    //===================================================================================
    /// <summary>
    /// Source represents one source file and manages the parsing and intellisense on this file
    /// and keeping things like the drop down combos in sync with the source and so on.
    /// </summary>
    abstract internal class FSharpSourceBase : ISource, IVsTextLinesEvents, IVsHiddenTextClient, IVsUserDataEvents
    {
        private LanguageService service;
        private IVsTextLines textLines;
        private Colorizer colorizer;
        private Microsoft.VisualStudio.Shell.TaskProvider taskProvider;
        private TaskReporter taskReporter;
        private CompletionSet completionSet;
        private TextSpan dirtySpan;
        private MethodData methodData;
        private ExpansionProvider expansionProvider;
        private NativeMethods.ConnectionPointCookie textLinesEvents;
        private NativeMethods.ConnectionPointCookie userDataEvents;
        private IVsTextColorState colorState;
        private IVsHiddenTextSession hiddenTextSession;
        private BackgroundRequest lastBraceMatchRequest = null;
        private string originalFileName = null;

        private bool doOutlining;
        private int openCount;
        private int lastOnIdleRequestDuration; // How long did the last completed OnIdle request take?
        static internal WORDEXTFLAGS WholeToken = (WORDEXTFLAGS)0x1000;
        private IntPtr pUnkTextLines;
        private bool disposed = false;
        private DateTime openedTime;

        protected IVsEditorAdaptersFactoryService getEditorAdapter()
        {
            var componentModel = (IComponentModel)service.Site.GetService(typeof(SComponentModel));
            return componentModel.GetService<IVsEditorAdaptersFactoryService>();
        }

        internal FSharpSourceBase(LanguageService service, IVsTextLines textLines, Colorizer colorizer)
        {
#if LANGTRACE
            Tracing.TraceRef(textLines, "Source.textLines");
#endif
            this.service = service;
            this.textLines = textLines;
            // REVIEW: why the next line? RCW in this.textLines holds IUnknown already. (released in Dispose correctly though)
            pUnkTextLines = Marshal.GetIUnknownForObject(this.textLines); //so it can't get disposed on us
            this.colorizer = colorizer;
            this.completionSet = this.CreateCompletionSet();
            this.methodData = new MethodData(this.service.Site);
            this.colorState = (IVsTextColorState)textLines;
            // track source changes
            this.textLinesEvents = new NativeMethods.ConnectionPointCookie(textLines, this, typeof(IVsTextLinesEvents));
            this.userDataEvents = new NativeMethods.ConnectionPointCookie(textLines, this, typeof(IVsUserDataEvents));

            this.doOutlining = this.service.Preferences.AutoOutlining;
            if (this.doOutlining)
            {
                GetHiddenTextSession();
            }
            this.expansionProvider = GetExpansionProvider();

            this.lastOnIdleRequestDuration = 0;

            this.openedTime = System.DateTime.Now;
        }

        ~FSharpSourceBase()
        {
#if LANGTRACE
            Trace.WriteLine("~Source");
#endif
        }

        public DateTime OpenedTime
        {
            get { return this.openedTime; }
        }

        public IVsTextColorState ColorState
        {
            get { return this.colorState; }
            set { this.colorState = value; }
        }

        public LanguageService LanguageService
        {
            get { return this.service; }
        }

        public CompletionSet CompletionSet
        {
            get { return this.completionSet; }
        }

        public Colorizer GetColorizer()
        {
            return this.colorizer;
        }

        public void Recolorize(int startLine, int endLine)
        {
            if (this.colorState != null && this.GetLineCount() > 0)
            {
                int lastLine = this.GetLineCount() - 1;
                startLine = Math.Min(startLine, lastLine);
                endLine = Math.Min(endLine, lastLine);
                this.colorState.ReColorizeLines(startLine, endLine);
            }
        }

        public AuthoringSink CreateAuthoringSink(BackgroundRequestReason reason, int line, int col)
        {
            int maxErrors = this.service.Preferences.MaxErrorMessages;
            TaskReporter tr = this.GetTaskReporter();
            if (null != tr)
                tr.MaxErrors = (uint)maxErrors;
            return new AuthoringSink(reason, line, col, maxErrors);
        }

        public CompletionSet CreateCompletionSet()
        {
            return new CompletionSet(this.service.GetImageList(), this);
        }


        // Overriden in Source.fs, but it calls this base implementation
        public virtual Microsoft.VisualStudio.Shell.TaskProvider GetTaskProvider()
        {
            if (this.taskProvider == null)
            {
                this.taskProvider = new Microsoft.VisualStudio.Shell.ErrorListProvider (service.Site); // task list
                this.taskProvider.ProviderGuid = service.GetLanguageServiceGuid();
				string name;
				((IVsLanguageInfo)service).GetLanguageName(out name);
				this.taskProvider.ProviderName = name;
            }
            return this.taskProvider;
        }

        // Overriden in Source.fs, but it calls this base implementation
        internal virtual TaskReporter GetTaskReporter()
        {
            if (null == taskReporter)
            {
                string name = string.Format("Language service (Source.cs): {0}", this.GetFilePath());
                taskReporter = new TaskReporter(name);
                taskReporter.TaskListProvider = new TaskListProvider(GetTaskProvider());
            }
            return taskReporter;
        }

        public LanguageService Service { get { return this.service; } }


        public ExpansionProvider GetExpansionProvider()
        {
            if (this.expansionProvider == null && this.service != null)
            {
                this.expansionProvider = this.service.CreateExpansionProvider(this);
            }
            return this.expansionProvider;
        }

        /// <devdiv>returns true if either CompletionSet or MethodData is being displayed.</devdiv>
        public bool IsCompletorActive
        {
            get
            {
                return (this.completionSet != null && this.completionSet.IsDisplayed) ||
                    (this.methodData != null && this.methodData.IsDisplayed);
            }
        }

        public void DismissCompletor()
        {
            if (this.completionSet != null && this.completionSet.IsDisplayed)
            {
                this.completionSet.Close();
            }
            if (this.methodData != null && this.methodData.IsDisplayed)
            {
                this.methodData.Close();
            }
        }

        public void Open()
        {
            this.openCount++;
        }

        public bool Close()
        {
#if LANGTRACE
            Trace.WriteLine("Source::Close");
#endif
            if (--this.openCount == 0)
            {
                return true;
            }
            return false;
        }

        // Overriden in source.fs, it calls this base implementation
        public virtual void Dispose()
        {
#if LANGTRACE
            Trace.WriteLine("Source::Cleanup");
#endif
            this.disposed = true;
            try
            {
                if (this.textLinesEvents != null)
                {
                    this.textLinesEvents.Dispose();
                    this.textLinesEvents = null;
                }
            }
            finally
            {
                try
                {
                    if (this.userDataEvents != null)
                    {
                        this.userDataEvents.Dispose();
                        this.userDataEvents = null;
                    }
                }
                finally
                {
                    try
                    {
                        if (this.hiddenTextSession != null)
                        {
                            // We can't throw or exit here because we need to call Dispose on the
                            // other members that need to be disposed.
                            this.hiddenTextSession.UnadviseClient();
                            // This is causing a debug assert in vs\env\msenv\textmgr\vrlist.cpp
                            // at line 1997 in CVisibleRegionList::Terminate
                            //this.hiddenTextSession.Terminate();
                            this.hiddenTextSession = null;
                        }
                    }
                    finally
                    {
                        try
                        {
                            if (this.methodData != null)
                            {
                                this.methodData.Dispose();
                                this.methodData = null;
                            }
                        }
                        finally
                        {
                            try
                            {
                                if (this.completionSet != null)
                                {
                                    this.completionSet.Dispose();
                                    this.completionSet = null;
                                }
                            }
                            finally
                            {
                                try
                                {
                                    // clear out any remaining tasks for this doc in the task list
                                    // tp may not be the same as taskProvider

                                    // REVIEW: This should be: if (null != this.taskReporter)
                                    // Right now, MSBuild 4.0 can clear out build loggers responsibly, so this.taskReporter will always
                                    // be null when we get to this point, so we'll need to create a new taskReporter to clear out the
                                    // background tasks
                                    TaskReporter tr = this.GetTaskReporter();  // may be our own TR or one from ProjectSite of this file
                                    if (null != tr)
                                    {
                                        tr.ClearBackgroundTasksForFile(this.GetFilePath());
                                        // Refresh the task list
                                        tr.OutputTaskList();
                                    }
                                    if (null != this.taskReporter)      // dispose the one we own (do not dispose one shared by project site!)
                                    {
                                        this.taskReporter.Dispose();
                                        this.taskReporter = null;
                                        this.taskProvider = null;
                                    }
                                }
                                finally
                                {
                                    try
                                    {
                                        this.service = null;
                                        if (this.colorizer != null)
                                        {
                                            // The colorizer is owned by the core text editor, so we don't close it, the core text editor
                                            // does that for us when it is ready to do so.
                                            //colorizer.CloseColorizer();
                                            this.colorizer = null;
                                        }
                                    }
                                    finally
                                    {
                                        this.colorState = null;
                                        try
                                        {
                                            if (this.expansionProvider != null)
                                            {
                                                this.expansionProvider.Dispose();
                                                this.expansionProvider = null;
                                            }

                                        }
                                        finally
                                        {
                                            // Sometimes OnCloseSource is called when language service is changed, (for example
                                            // when you save the file with a different file extension) in which case we cannot 
                                            // null out the site because that will cause a crash inside msenv.dll.
                                            //            if (this.textLines != null) {
                                            //                ((IObjectWithSite)this.textLines).SetSite(null);
                                            //            }
                                            if (this.textLines != null)
                                            {
                                                this.textLines = null;
                                                Marshal.Release(pUnkTextLines);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool IsClosed
        {
            get { return this.textLines == null; }
        }


        public IVsTextLines GetTextLines()
        {
            return this.textLines;
        }
        public int GetLineLength(int line)
        {
            int len;
            NativeMethods.ThrowOnFailure(this.textLines.GetLengthOfLine(line, out len));
            return len;
        }

        public int GetLineCount()
        {
            int count;
            NativeMethods.ThrowOnFailure(this.textLines.GetLineCount(out count));
            return count;
        }

        public int GetPositionOfLineIndex(int line, int col)
        {
            int position;
            NativeMethods.ThrowOnFailure(this.textLines.GetPositionOfLineIndex(line, col, out position));
            return position;
        }
        public void GetLineIndexOfPosition(int position, out int line, out int col)
        {
            NativeMethods.ThrowOnFailure(this.textLines.GetLineIndexOfPosition(position, out line, out col));
        }

        public string GetLine(int line)
        {
            int len;
            NativeMethods.ThrowOnFailure(this.textLines.GetLengthOfLine(line, out len));
            return GetText(line, 0, line, len);
        }
        public string GetText()
        {
            int endLine, endCol;
            NativeMethods.ThrowOnFailure(this.textLines.GetLastLineIndex(out endLine, out endCol));
            return GetText(0, 0, endLine, endCol);
        }

        public string GetText(int startLine, int startCol, int endLine, int endCol)
        {
            string text;
            Debug.Assert(TextSpanHelper.ValidCoord(this, startLine, startCol) && TextSpanHelper.ValidCoord(this, endLine, endCol));
            NativeMethods.ThrowOnFailure(this.textLines.GetLineText(startLine, startCol, endLine, endCol, out text));
            return text;
        }

        public string GetText(TextSpan span)
        {
            return GetText(span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex);
        }

        public string GetTextUpToLine(int line)
        {
            Debug.Assert(TextSpanHelper.ValidCoord(this, line, 0));
            int lastLine;
            NativeMethods.ThrowOnFailure(this.textLines.GetLineCount(out lastLine));
            lastLine--;
            if (line > 0) lastLine = Math.Min(line, lastLine);
            int lastIdx;
            NativeMethods.ThrowOnFailure(this.textLines.GetLengthOfLine(lastLine, out lastIdx));
            return GetText(0, 0, lastLine, lastIdx);
        }

        public void SetText(string newText)
        {
            int endLine, endCol;
            NativeMethods.ThrowOnFailure(this.textLines.GetLastLineIndex(out endLine, out endCol));
            int len = (newText == null) ? 0 : newText.Length;
            IntPtr pText = Marshal.StringToCoTaskMemAuto(newText);
            try
            {
                NativeMethods.ThrowOnFailure(this.textLines.ReplaceLines(0, 0, endLine, endCol, pText, len, null));
            }
            finally
            {
                Marshal.FreeCoTaskMem(pText);
            }
        }

        public void SetText(TextSpan span, string newText)
        {
            this.SetText(span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex, newText);
        }

        public void SetText(int startLine, int startCol, int endLine, int endCol, string newText)
        {
            int len = (newText == null) ? 0 : newText.Length;
            int realEndLine, realEndCol;
            // trim to the real bounds of the file so we don't get a COM exception
            NativeMethods.ThrowOnFailure(this.textLines.GetLastLineIndex(out realEndLine, out realEndCol));
            if (endLine > realEndLine)
            {
                endLine = realEndLine;
                endCol = realEndCol;
            }
            else if (endLine == realEndLine && endCol > realEndCol)
            {
                endCol = realEndCol;
            }
            IntPtr pText = Marshal.StringToCoTaskMemAuto(newText);
            try
            {
                NativeMethods.ThrowOnFailure(this.textLines.ReplaceLines(startLine, startCol, endLine, endCol, pText, len, null));
            }
            finally
            {
                Marshal.FreeCoTaskMem(pText);
            }
        }

        public void SetText(ITextSnapshotLine line, int startIndex, int length, string newText, ITextEdit edit)
        {
            var replaceSpan = new Span(line.Extent.Start.Position + startIndex, length);
            edit.Replace(replaceSpan, newText);
        }

        public object GetUserData(ref Guid key)
        {
            object data = null;
            IVsUserData iud = null;
            iud = (IVsUserData)this.textLines;
            int rc = iud.GetData(ref key, out data);
            iud = null;
            return (rc == NativeMethods.S_OK) ? data : null;
        }

        public void SetUserData(ref Guid key, object data)
        {
            IVsUserData iud = (IVsUserData)this.textLines;
            NativeMethods.ThrowOnFailure(iud.SetData(ref key, data));
        }

        // Implemented in Source.fs
        public abstract void RecordChangeToView();
        // Implemented in Source.fs
        public abstract void RecordViewRefreshed();
        // Implemented in Source.fs
        public abstract bool NeedsVisualRefresh { get; }
        // Implemented in Source.fs
        public abstract int DirtyTime { get; set; }
        // Implemented in Source.fs
        public abstract int ChangeCount { get; set; }
        // Implemented in Source.fs
        public abstract string GetExpressionAtPosition(int line, int col);

        public TextSpan DirtySpan
        {
            get
            {
                return this.dirtySpan;
            }
        }

        void AddDirty(TextSpan span)
        {
            if (!this.NeedsVisualRefresh)
            {
                this.dirtySpan = span;
            }
            else
            {
                this.dirtySpan = TextSpanHelper.Merge(dirtySpan, span);
            }
            this.RecordChangeToView();
        }

        /// <summary>
        /// This method formats the given span using the given EditArray. The default behavior does nothing.  
        /// So you need to override this method if you want formatting to work.  
        /// An empty input span means reformat the entire document.
        /// You also need to turn on Preferences.EnableFormatSelection.
        /// </summary>
        public void ReformatSpan(EditArray mgr, TextSpan span)
        {
        }

        // Implemented in Source.fs
        /// <summary>Implement this method to provide different comment delimiters.</summary>
        public abstract CommentInfo GetCommentFormat();

        // Overriden in Source.fs, but it calls this base implementation
        public virtual TextSpan CommentSpan(TextSpan span)
        {
            TextSpan result = span;
            CommentInfo commentInfo = this.GetCommentFormat();

            using (new CompoundAction(this, SR.GetString(SR.CommentSelection)))
            {
                //try to use line comments first, if we can.        
                if (commentInfo.UseLineComments && !string.IsNullOrEmpty(commentInfo.LineStart))
                {
                    span = CommentLines(span, commentInfo.LineStart);
                }
                else if (!string.IsNullOrEmpty(commentInfo.BlockStart) && !string.IsNullOrEmpty(commentInfo.BlockEnd))
                {
                    result = CommentBlock(span, commentInfo.BlockStart, commentInfo.BlockEnd);
                }
            }
            return result;
        }

        /// <summary>
        /// Called from Comment Selection. Default behavior is to insert line style comments
        /// at beginning and end of selection. Override to add custome behavior.
        /// </summary>
        /// <param name="span"></param>
        /// <param name="lineComment"></param>
        /// <returns>The final span of the commented lines including the comment delimiters</returns>
        // Implemented in Source.fs
        public abstract TextSpan CommentLines(TextSpan span, string lineComment);

        /// <summary>
        /// Called from Comment Selection. Default behavior is to insert block style comments
        /// at beginning and end of selection. Override to add custome behavior.
        /// </summary>
        /// <returns>The final span of the commented block including the comment delimiters</returns>
        private TextSpan CommentBlock(TextSpan span, string blockStart, string blockEnd)
        {
            //sp. case no selection
            if (span.iStartIndex == span.iEndIndex &&
                span.iStartLine == span.iEndLine)
            {
                span.iStartIndex = this.ScanToNonWhitespaceChar(span.iStartLine);
                span.iEndIndex = this.GetLineLength(span.iEndLine);
            }
            //sp. case partial selection on single line
            if (span.iStartLine == span.iEndLine)
            {
                span.iEndIndex += blockStart.Length;
            }
            //add start comment
            this.SetText(span.iStartLine, span.iStartIndex, span.iStartLine, span.iStartIndex, blockStart);
            //add end comment
            this.SetText(span.iEndLine, span.iEndIndex, span.iEndLine, span.iEndIndex, blockEnd);
            span.iEndIndex += blockEnd.Length;
            return span;
        }

        /// <summary>
        /// Uncomments the given span of text and returns the span of the uncommented block.
        /// </summary>
        public TextSpan UncommentSpan(TextSpan span)
        {
            CommentInfo commentInfo = this.GetCommentFormat();

            using (new CompoundAction(this, SR.GetString(SR.UncommentSelection)))
            {
                // is block comment selected?
                if (commentInfo.UseLineComments && !string.IsNullOrEmpty(commentInfo.LineStart))
                {
                    span = UncommentLines(span, commentInfo.LineStart);
                }
                else if (commentInfo.BlockStart != null && commentInfo.BlockEnd != null)
                {
                    // TODO: this doesn't work if the selection contains a mix of code and block comments
                    // or multiple block comments!!  We should use the scanner to find the embedded 
                    // comments and uncomment the resulting comment spans only.
                    this.TrimSpan(ref span);
                    span = UncommentBlock(span, commentInfo.BlockStart, commentInfo.BlockEnd);
                }
            }
            return span;
        }

        // Implemented in Source.fs
        public abstract TextSpan UncommentLines(TextSpan span, string lineComment);

        /// <summary>Uncomments the given block and returns the span of the uncommented block</summary>
        public TextSpan UncommentBlock(TextSpan span, string blockStart, string blockEnd)
        {

            int startLen = this.GetLineLength(span.iStartLine);
            int endLen = this.GetLineLength(span.iEndLine);

            TextSpan result = span;

            //sp. case no selection, try and uncomment the current line.
            if (span.iStartIndex == span.iEndIndex &&
                span.iStartLine == span.iEndLine)
            {
                span.iStartIndex = this.ScanToNonWhitespaceChar(span.iStartLine);
                span.iEndIndex = this.GetLineLength(span.iEndLine);
            }

            // Check that comment start and end blocks are possible.
            if (span.iStartIndex + blockStart.Length <= startLen && span.iEndIndex - blockStart.Length >= 0)
            {
                string startText = this.GetText(span.iStartLine, span.iStartIndex, span.iStartLine, span.iStartIndex + blockStart.Length);

                if (startText == blockStart)
                {
                    string endText = null;
                    TextSpan linespan = span;
                    linespan.iStartLine = linespan.iEndLine;
                    linespan.iStartIndex = linespan.iEndIndex - blockEnd.Length;
                    Debug.Assert(TextSpanHelper.IsPositive(linespan));
                    endText = this.GetText(linespan);
                    if (endText == blockEnd)
                    {
                        //yes, block comment selected; remove it        
                        this.SetText(linespan.iStartLine, linespan.iStartIndex, linespan.iEndLine, linespan.iEndIndex, null);
                        this.SetText(span.iStartLine, span.iStartIndex, span.iStartLine, span.iStartIndex + blockStart.Length, null);
                        span.iEndIndex -= blockEnd.Length;
                        if (span.iStartLine == span.iEndLine) span.iEndIndex -= blockStart.Length;
                        result = span;
                    }
                }
            }

            return result;
        }

        public void OnChangeLineText(TextLineChange[] lineChange, int last)
        {
            TextSpan span = new TextSpan();
            span.iStartIndex = lineChange[0].iStartIndex;
            span.iStartLine = lineChange[0].iStartLine;
            span.iEndLine = lineChange[0].iOldEndLine;
            span.iEndIndex = lineChange[0].iOldEndIndex;
            AddDirty(span);
            span.iEndLine = lineChange[0].iNewEndLine;
            span.iEndIndex = lineChange[0].iNewEndIndex;
            AddDirty(span);
        }

        public void OnChangeLineAttributes(int firstLine, int lastLine)
        {
        }


        //===================================================================================
        // Helper methods:
        //===================================================================================   

        public string GetFilePath()
        {
            if (this.textLines == null) return null;
            var filename = FilePathUtilities.GetFilePath(this.textLines);
            if (this.originalFileName == null)
            {
                this.originalFileName = filename; // Save this off for use during rename.
            }
            return filename;
        }

        /// <summary>
        /// Return the column position of 1st non whitespace character on line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public int ScanToNonWhitespaceChar(int line)
        {
            string text = GetLine(line);
            int len = text.Length;
            int i = 0;
            while (i < len && Char.IsWhiteSpace(text[i]))
            {
                i++;
            }
            return i;
        }

        /// <summary>
        /// Return the column position that the user will see given the current
        /// tab size setting.  This is the opposite of VisiblePositionToColumn
        /// </summary>
        public int ColumnToVisiblePosition(int line, int col)
        {
            string text = this.GetLine(line);
            int tabsize = this.LanguageService.Preferences.TabSize;
            int visible = 0;
            for (int i = 0, n = text.Length; i < col && i < n; i++)
            {
                char ch = text[i];
                int step = 1;
                if (ch == '\t')
                {
                    step = tabsize - visible % tabsize;
                }
                visible += step;
            }
            return visible;
        }

        /// <summary>
        /// Convert a user visible position back to char position in the buffer.
        /// This is the opposite of ColumnToVisiblePosition. In this case the 
        /// visible position was off the end of the line, it just returns the 
        /// column position at the end of the line.
        /// </summary>
        public int VisiblePositionToColumn(int line, int visiblePosition)
        {
            string text = this.GetLine(line);
            int tabsize = this.LanguageService.Preferences.TabSize;
            int visible = 0;
            int i = 0;
            for (int n = text.Length; i < n; i++)
            {
                char ch = text[i];
                int step = 1;
                if (ch == '\t')
                {
                    step = visible % tabsize;
                    if (step == 0) step = tabsize;
                }
                visible += step;
                if (visible > visiblePosition)
                    return i;
            }
            return i;
        }

        public TextSpan GetDocumentSpan()
        {
            TextSpan span = new TextSpan();
            span.iStartIndex = span.iStartLine = 0;
            NativeMethods.ThrowOnFailure(this.textLines.GetLastLineIndex(out span.iEndLine, out span.iEndIndex));
            return span;
        }

        /// <summary>
        /// Returns normalized form of given string replacing all control chars.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        // Implemented in Source.fs
        public abstract string NormalizeErrorString(string message);
        /// Converts --flaterrors messages back to messages with embedded newlines
        public abstract string NewlineifyErrorString(string message);

        // helper methods.
        public DocumentTask CreateErrorTaskItem(TextSpan span, string filename, string subcategory, string message, Microsoft.VisualStudio.Shell.TaskPriority priority, Microsoft.VisualStudio.Shell.TaskCategory category, MARKERTYPE markerType, Microsoft.VisualStudio.Shell.TaskErrorCategory errorCategory)
        {
            // create task item

            //TODO this src obj may not be the one matching filename.
            //find the src for the filename only then call ValidSpan.
            //Debug.Assert(TextSpanHelper.ValidSpan(this, span)); 

            DocumentTask taskItem = new DocumentTask(this.service.Site, this.textLines, markerType, span, filename, subcategory);
            taskItem.Priority = priority;
            taskItem.Category = category;
            taskItem.ErrorCategory = errorCategory;
            message = NewlineifyErrorString(message);
            taskItem.Text = message;
            taskItem.IsTextEditable = false;
            taskItem.IsCheckedEditable = false;
            return taskItem;
        }


        // return the type of new line to use that matches the one at the given line.
        public string GetNewLine(int line)
        {
            string eol = "\r\n"; // "\x000D\x000A"
            LINEDATAEX[] ld = new LINEDATAEX[1];
            NativeMethods.ThrowOnFailure(this.textLines.GetLineDataEx(0, line, 0, 0, ld, null));
            uint iEolType = (uint)ld[0].iEolType;
            if (iEolType == (uint)EOLTYPE.eolUNI_LINESEP)
            {
                if (this.textLines is IVsTextLines2)
                {
                    IVsTextLines2 textLines2 = (IVsTextLines2)this.textLines;
                    int hr = textLines2.GetEolTypeEx(ld, out iEolType);
                    if (NativeMethods.Failed(hr))
                    {
#if LANGTRACE
                        Trace.WriteLine("Ignoring actual EOL type and continuing");
#endif
                        iEolType = (uint)EOLTYPE.eolUNI_LINESEP;
                    }
                }
            }

            switch (iEolType)
            {
                case (uint)EOLTYPE.eolCR:
                    eol = "\r"; // "\x000D"
                    break;
                case (uint)EOLTYPE.eolLF:
                    eol = "\n"; // "\x000A"
                    break;
                case (uint)EOLTYPE.eolUNI_LINESEP:
                    eol = "\u2028";
                    break;
                case (uint)EOLTYPE.eolUNI_PARASEP:
                    eol = "\u2029";
                    break;
                case (uint)_EOLTYPE2.eolUNI_NEL:
                    eol = "\u0085";
                    break;
            }

            NativeMethods.ThrowOnFailure(this.textLines.ReleaseLineDataEx(ld));

            return eol;
        }

        public int GetTokenInfoAt(TokenInfo[] infoArray, int col, ref TokenInfo info)
        {
            for (int i = 0, len = infoArray.Length; i < len; i++)
            {
                int start = infoArray[i].StartIndex;
                int end = infoArray[i].EndIndex;

                if (i == 0 && start > col)
                    return -1;

                if (col >= start && col <= end)
                {
                    info = infoArray[i];
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// If the file has been renamed then do some cleanup.
        /// </summary>
        public void CheckForRename()
        {
            var currentFilename = this.GetFilePath();
            if (this.originalFileName != currentFilename)
            {
                TaskReporter tr = this.GetTaskReporter();
                tr.ClearBackgroundTasksForFile(originalFileName);

                // Refresh the task list
                tr.OutputTaskList();

                this.originalFileName = null;

                if (this.taskProvider != null)
                {
                    this.taskProvider = null;
                }
                if (this.taskReporter != null)
                {
                    this.taskReporter.Dispose();
                    this.taskReporter = null;
                }

                this.RecordChangeToView();
            }
        }

        public void OnIdle(bool periodic)
        {
            CheckForRename();

            // Kick of a background parse, but only in the periodic intervals
            if (!periodic || this.service == null || this.service.LastActiveTextView == null)
            {
                return;
            }

            // Don't kick off a background parse, while the user is typing.
            // this.DirtyTime moves with every keystroke.
            int msec = System.Environment.TickCount;
            //int delay = this.service.Preferences.CodeSenseDelay; //  Math.Max(this.lastOnIdleRequestDuration, this.service.Preferences.CodeSenseDelay);
            int delay = Math.Min(1000, Math.Max(this.lastOnIdleRequestDuration / 3, this.service.Preferences.CodeSenseDelay));

            if ((msec < this.DirtyTime) ||   // Environment.TickCount wraps around every 49 days.
                (msec - this.DirtyTime > delay))
            {
                if (this.NeedsVisualRefresh && !this.service.IsServingBackgroundRequest)
                {
                    BackgroundRequest req = this.BeginBackgroundRequest(0, 0, new TokenInfo(), BackgroundRequestReason.FullTypeCheck, this.service.LastActiveTextView, RequireFreshResults.Yes, new BackgroundRequestResultHandler(this.HandleUntypedParseOrFullTypeCheckResponse));
                    if (req != null) req.StartTimeForOnIdleRequest = Environment.TickCount;
                }
            }
        }

        public TokenInfo GetTokenInfo(int line, int col)
        {
            if (col < 0)
            {
                throw new InvalidOperationException("TokenInfo called with negative col");
            }
            //get current line 
            TokenInfo info = new TokenInfo();
            //get line info
            TokenInfo[] lineInfo = this.colorizer.GetLineInfo(this.textLines, line, this.colorState);
            if (lineInfo != null)
            {
                //get character info      
                this.GetTokenInfoAt(lineInfo, col - 1, ref info);
            }

            return info;
        }

        public void OnCommand(IVsTextView textView, VsCommands2K command, char ch)
        {
            if (textView == null || this.service == null || !this.service.Preferences.EnableCodeSense)
                return;

            bool backward = (command == VsCommands2K.BACKSPACE || command == VsCommands2K.BACKTAB || command == VsCommands2K.LEFT || command == VsCommands2K.LEFT_EXT);

            int line, idx;

            var hr = textView.GetCaretPos(out line, out idx);
            if (NativeMethods.Failed(hr))
                return;

            TokenInfo info = GetTokenInfo(line, idx);
            TokenTriggers triggerClass = info.Trigger;


            var matchBraces = false;
            var methodTip = false;
            MethodTipMiscellany misc = 0;

            if ((triggerClass & TokenTriggers.MemberSelect) != 0 && (command == VsCommands2K.TYPECHAR))
            {
                BackgroundRequestReason reason = ((triggerClass & TokenTriggers.MatchBraces) != 0) ? BackgroundRequestReason.MemberSelectAndHighlightBraces : BackgroundRequestReason.MemberSelect;
                this.Completion(textView, info, reason, RequireFreshResults.No);
            }
            else if (this.service.Preferences.EnableMatchBraces &&
                ((command != VsCommands2K.BACKSPACE) && ((command == VsCommands2K.TYPECHAR) || this.service.Preferences.EnableMatchBracesAtCaret)))
            {

                // For brace matching when the caret is before the opening brace, we need to check the token at next index
                TokenInfo nextInfo = GetTokenInfo(line, idx + 1); // ??? overflow
                TokenTriggers nextTriggerClass = nextInfo.Trigger;

                if (((nextTriggerClass & (TokenTriggers.MatchBraces)) != 0) || ((triggerClass & (TokenTriggers.MatchBraces)) != 0))
                    matchBraces = true;
            }

            if ((triggerClass & TokenTriggers.MethodTip) != 0   // open paren, close paren, or comma
                && (command == VsCommands2K.TYPECHAR))          // they typed it, not just arrowed over it
            {
                methodTip = true;

                misc = MethodTipMiscellany.JustPressedOpenParen;
                if ((triggerClass & TokenTriggers.ParameterNext) != 0)
                    misc = MethodTipMiscellany.JustPressedComma;
                if ((triggerClass & TokenTriggers.ParameterEnd) != 0)
                    misc = MethodTipMiscellany.JustPressedCloseParen;
            }
            else if (this.methodData.IsDisplayed)
            {
                if (command == VsCommands2K.BACKSPACE)
                {
                    // the may have just erased a paren or comma, need to re-parse
                    methodTip = true;
                    misc = MethodTipMiscellany.JustPressedBackspace;
                }
                else
                {
                    this.methodData.Refresh(MethodTipMiscellany.Typing);
                }
            }

            if (matchBraces && methodTip)
            {
                // matchBraces = true and methodTip = true

                // backward is true when command is one of these: VsCommands2K.BACKSPACE | VsCommands2K.BACKTAB | VsCommands2K.LEFT | VsCommands2K.LEFT_EXT (1)
                // matchBraces = true when command is not BACKSPACE => BACKSPACE is excluded from the set (1)
                // methodTip = true when command is TYPECHAR or BACKSPACE => BACKSPACE is already excluded and TYPECHAR is not contained in set (1)
                // ergo: backward is always false here
                Debug.Assert(!backward);
                MatchBracesAndMethodTip(textView, line, idx, misc, info);
            }
            else if (matchBraces)
            {
                MatchBraces(textView, line, idx, info);
            }
            else if (methodTip)
            {
                MethodTip(textView, line, (backward && idx > 0) ? idx - 1 : idx, info, misc, RequireFreshResults.No);
            }
        }

        public bool GetWordExtent(int line, int idx, WORDEXTFLAGS flags, out int startIdx, out int endIdx)
        {
            Debug.Assert(line >= 0 && idx >= 0);
            startIdx = endIdx = idx;

            int length;
            NativeMethods.ThrowOnFailure(this.textLines.GetLengthOfLine(line, out length));
            // pin to length of line just in case we return false and skip pinning at the end of this method.
            startIdx = endIdx = Math.Min(idx, length);
            if (length == 0)
            {
                return false;
            }

            //get the character classes
            TokenInfo[] lineInfo = this.colorizer.GetLineInfo(this.textLines, line, this.colorState);
            if (lineInfo == null || lineInfo.Length == 0) return false;

            int count = lineInfo.Length;
            TokenInfo info = new TokenInfo();
            int index = this.GetTokenInfoAt(lineInfo, idx, ref info);

            if (index < 0) return false;
            // don't do anything in comment or text or literal space, unless we
            // are doing intellisense in which case we want to match the entire value
            // of quoted strings.
            TokenType type = info.Type;
            if ((flags != FSharpSourceBase.WholeToken || type != TokenType.String) && (type == TokenType.Comment || type == TokenType.LineComment || type == TokenType.Text || type == TokenType.String || type == TokenType.Literal))
                return false;
            //search for a token
            switch (flags & WORDEXTFLAGS.WORDEXT_MOVETYPE_MASK)
            {
                case WORDEXTFLAGS.WORDEXT_PREVIOUS:
                    index--;
                    while (index >= 0 && !MatchToken(flags, lineInfo[index])) index--;
                    if (index < 0) return false;
                    break;

                case WORDEXTFLAGS.WORDEXT_NEXT:
                    index++;
                    while (index < count && !MatchToken(flags, lineInfo[index])) index++;
                    if (index >= count) return false;
                    break;

                case WORDEXTFLAGS.WORDEXT_NEAREST:
                    {
                        int prevIdx = index;
                        prevIdx--;
                        while (prevIdx >= 0 && !MatchToken(flags, lineInfo[prevIdx])) prevIdx--;
                        int nextIdx = index;
                        while (nextIdx < count && !MatchToken(flags, lineInfo[nextIdx])) nextIdx++;
                        if (prevIdx < 0 && nextIdx >= count) return false;
                        else if (nextIdx >= count) index = prevIdx;
                        else if (prevIdx < 0) index = nextIdx;
                        else if (index - prevIdx < nextIdx - index) index = prevIdx;
                        else
                            index = nextIdx;
                        break;
                    }

                case WORDEXTFLAGS.WORDEXT_CURRENT:
                default:
                    if (!MatchToken(flags, info))
                        return false;

                    break;
            }
            info = lineInfo[index];

            // We found something, set the span, pinned to the valid coordinates for the
            // current line.
            startIdx = Math.Min(length, info.StartIndex);
            endIdx = Math.Min(length, info.EndIndex);

            // The scanner endIndex is the last char of the symbol, but
            // GetWordExtent wants it to be the next char after that, so 
            // we increment the endIdx (if we can).
            if (endIdx < length) endIdx++;
            return true;
        }

        static bool MatchToken(WORDEXTFLAGS flags, TokenInfo info)
        {
            TokenType type = info.Type;
            if ((flags & WORDEXTFLAGS.WORDEXT_FINDTOKEN) != 0)
                return !(type == TokenType.Comment || type == TokenType.LineComment);
            else
                return (type == TokenType.Keyword || type == TokenType.Identifier || type == TokenType.String || type == TokenType.Literal);
        }



        /// Trim whitespace from the beginning and ending of the given span.
        public void TrimSpan(ref TextSpan span)
        {
            // Scan forwards past whitepsace.
            int length;
            NativeMethods.ThrowOnFailure(this.textLines.GetLengthOfLine(span.iStartLine, out length));

            while (span.iStartLine < span.iEndLine || (span.iStartLine == span.iEndLine && span.iStartIndex < span.iEndIndex))
            {
                string text = this.GetText(span.iStartLine, 0, span.iStartLine, length);
                for (int i = span.iStartIndex; i < length; i++)
                {
                    char ch = text[i];
                    if (ch != ' ' && ch != '\t')
                        break;
                    span.iStartIndex++;
                }
                if (span.iStartIndex >= length)
                {
                    span.iStartIndex = 0;
                    span.iStartLine++;
                    NativeMethods.ThrowOnFailure(this.textLines.GetLengthOfLine(span.iStartLine, out length));
                }
                else
                {
                    break;
                }
            }
            // Scan backwards past whitepsace.
            NativeMethods.ThrowOnFailure(this.textLines.GetLengthOfLine(span.iEndLine, out length));

            while (span.iStartLine < span.iEndLine || (span.iStartLine == span.iEndLine && span.iStartIndex < span.iEndIndex))
            {
                string text = GetText(span.iEndLine, 0, span.iEndLine, length);
                for (int i = span.iEndIndex - 1; i >= 0; i--)
                {
                    char ch = text[i];
                    if (ch != ' ' && ch != '\t')
                        break;
                    span.iEndIndex--;
                }
                if (span.iEndIndex <= 0)
                {
                    span.iEndLine--;
                    NativeMethods.ThrowOnFailure(this.textLines.GetLengthOfLine(span.iEndLine, out length));
                    span.iEndIndex = length;
                }
                else
                {
                    break;
                }
            }
        }


        // Implemented in Source.fs
        public abstract void Completion(IVsTextView textView, TokenInfo info, BackgroundRequestReason reason, RequireFreshResults requireFreshResults);

        // Implemented in Source.fs
        public abstract void HandleLostFocus();

        // got this handy code from vsxdisc alias, this is the non-MEF version of http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.editor.ivseditoradaptersfactoryservice.getwpftextview.aspx
        internal static IWpfTextView GetWpfTextViewFromVsTextView(IVsTextView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            IVsUserData userData = view as IVsUserData;
            if (userData == null)
            {
                throw new InvalidOperationException();
            }

            object objTextViewHost;
            if (Microsoft.VisualStudio.VSConstants.S_OK != userData.GetData(Microsoft.VisualStudio.Editor.DefGuidList.guidIWpfTextViewHost, out objTextViewHost))
            {
                throw new InvalidOperationException();
            }

            IWpfTextViewHost textViewHost = objTextViewHost as IWpfTextViewHost;
            if (textViewHost == null)
            {
                throw new InvalidOperationException();
            }

            return textViewHost.TextView;
        }

        public void MethodTip(IVsTextView textView, int line, int index, TokenInfo info, MethodTipMiscellany methodTipMiscellany, RequireFreshResults requireFreshResults)
        {
            this.BeginBackgroundRequest(line, index, info, BackgroundRequestReason.MethodTip, textView, requireFreshResults, new BackgroundRequestResultHandler(this.HandleMethodTipResponse), methodTipMiscellany);
        }

        private void HandleMethodTipResponse(BackgroundRequest req)
        {
            try
            {
                if (this.service == null)
                    return;

                // Don't do anything if the buffer is out of sync
                if (req.Timestamp != this.ChangeCount)
                    return;

                HandleResponseHelper(req);

                if (req.Timestamp != this.ChangeCount)
                {
                    // The user has been typing since we sent off the snapshot to be parsed
                    // TODO maybe this gets into infinite loop of wasted work if something goes wrong
                    this.MethodTip(req.View, req.Line, req.Col, req.TokenInfo, req.MethodTipMiscellany, RequireFreshResults.No);  // try again, need up-to-date parse
                }
                else
                {
                    Microsoft.FSharp.Core.FSharpOption<MethodListForAMethodTip> methodsOpt = req.ResultIntellisenseInfo.GetMethodListForAMethodTip();
					if (methodsOpt != null)
					{
						MethodListForAMethodTip methods = methodsOpt.Value;

						if (methods != null)
						{
							TextSpan spanNotToObscureWithTipPopup = new TextSpan();
							spanNotToObscureWithTipPopup.iStartLine = methods.GetNoteworthyParamInfoLocations()[0].Item1;
							spanNotToObscureWithTipPopup.iStartIndex = methods.GetNoteworthyParamInfoLocations()[0].Item2;
							spanNotToObscureWithTipPopup.iEndLine = req.Line;
							spanNotToObscureWithTipPopup.iEndIndex = req.Col;
							this.methodData.Refresh(req.View, methods, spanNotToObscureWithTipPopup, req.MethodTipMiscellany);

						}
					}
                    else if (req.MethodTipMiscellany == MethodTipMiscellany.JustPressedOpenParen && req.Timestamp != req.ResultTimestamp)
                    {
                        // Second-chance param info: we didn't get any result and the basis typecheck was stale. We need to retrigger the completion.
                        this.MethodTip(req.View, req.Line, req.Col, req.TokenInfo, req.MethodTipMiscellany, RequireFreshResults.Yes);
                    }
                    else
                    {
                        DismissCompletor();
                    }
                }
#if LANGTRACE
            } catch (Exception e) {
                Trace.WriteLine("HandleMethodTipResponse exception: " + e.Message);
#endif
            }
            catch { }
        }

        public void MatchBraces(IVsTextView textView, int line, int index, TokenInfo info)
        {
            this.BeginBackgroundRequest(line, index, info, BackgroundRequestReason.MatchBraces, textView, RequireFreshResults.No, new BackgroundRequestResultHandler(this.HandleMatchBracesResponse));
        }

        public void MatchBracesAndMethodTip(IVsTextView textView, int line, int index, MethodTipMiscellany misc, TokenInfo info)
        {
            this.BeginBackgroundRequest(line, index, info, BackgroundRequestReason.MatchBracesAndMethodTip, textView, RequireFreshResults.No, 
                req =>
                    {
                        HandleMatchBracesResponse(req);
                        HandleMethodTipResponse(req);
                    }
                );
        }

        public void HandleMatchBracesResponse(BackgroundRequest req)
        {
            try
            {
                if (this.service == null || req.Timestamp != this.ChangeCount)
                    return;

                // save request so it can later be reused in GetPairExtent
                HandleGetPairExtentResponse(req);
#if LANGTRACE
                Trace.WriteLine("HandleMatchBracesResponse");
#endif
                // Filter matching braces and find spans to highlight - we want to highlight 
                // matches only before opening and after closing brace
                int line;
                int idx;
                NativeMethods.ThrowOnFailure(req.View.GetCaretPos(out line, out idx));

                // Get all braces from language service and filter them
                List<BraceMatch> braces = new List<BraceMatch>();
                foreach (BraceMatch m in req.ResultSink.Braces) braces.Add(m);
                braces.RemoveAll(delegate(BraceMatch match)
                {
                    if (match.a.iStartLine == line && match.a.iStartIndex == idx) return false;
                    if (match.b.iEndLine == line && match.b.iEndIndex == idx) return false;
                    return true;
                });

                // Transform collection of braces into an array of spans
                List<TextSpan> spans = new List<TextSpan>();
                foreach (BraceMatch m in braces)
                {
                    spans.Add(m.a); spans.Add(m.b);
                }

                // Highlight
                if (spans.Count == 0) return;
                NativeMethods.ThrowOnFailure(req.View.HighlightMatchingBrace((uint)this.service.Preferences.HighlightMatchingBraceFlags, (uint)spans.Count, spans.ToArray()));

                //try to show the matching line in the statusbar
                if (spans.Count > 0 && this.service.Preferences.EnableShowMatchingBrace)
                {
                    IVsStatusbar statusBar = (IVsStatusbar)service.Site.GetService(typeof(SVsStatusbar));
                    if (statusBar != null)
                    {
                        TextSpan span = spans[0];
                        bool found = false;
                        // Gather up the other side of the brace match so we can 
                        // display the text in the status bar. There could be more than one
                        // if MatchTriple was involved, in which case we merge them.
                        for (int i = 0, n = spans.Count; i < n; i++)
                        {
                            TextSpan brace = spans[i];
                            if (brace.iStartLine != req.Line)
                            {
                                if (brace.iEndLine != brace.iStartLine)
                                {
                                    brace.iEndLine = brace.iStartLine;
                                    brace.iEndIndex = this.GetLineLength(brace.iStartLine);
                                }
                                if (!found)
                                {
                                    span = brace;
                                }
                                else if (brace.iStartLine == span.iStartLine)
                                {
                                    span = TextSpanHelper.Merge(span, brace);
                                }
                                found = true;
                            }
                        }
                        if (found)
                        {
                            Debug.Assert(TextSpanHelper.IsPositive(span));
                            string text = this.GetText(span);

                            int start;
                            int len = text.Length;

                            for (start = 0; start < len && Char.IsWhiteSpace(text[start]); start++) ;

                            if (start < span.iEndIndex)
                            {
                                if (text.Length > 80)
                                {
                                    text = String.Format(CultureInfo.CurrentUICulture, SR.GetString(SR.Truncated), text.Substring(0, 80));
                                }
                                text = String.Format(CultureInfo.CurrentUICulture, SR.GetString(SR.BraceMatchStatus), text);
                                NativeMethods.ThrowOnFailure(statusBar.SetText(text));
                            }
                        }
                    }
                }
#if LANGTRACE
            } catch (Exception e) {
                Trace.WriteLine("HandleMatchBracesResponse exception: " + e.Message);
#endif
            }
            catch
            {
            }
        }

        public void GetPairExtents(IVsTextView textView, int line, int col, out TextSpan span)
        {
            span = new TextSpan();
            TextSpan startBraceSpan, endBraceSpan;
            this.GetPairExtents(textView, line, col, out startBraceSpan, out endBraceSpan);
            span.iStartLine = startBraceSpan.iStartLine;
            span.iStartIndex = startBraceSpan.iStartIndex;
            span.iEndLine = endBraceSpan.iStartLine;
            span.iEndIndex = endBraceSpan.iStartIndex;

            TextSpanHelper.MakePositive(ref span);
            Debug.Assert(TextSpanHelper.ValidSpan(this, span));
            return;
        }

        private bool GetPairExtents(IVsTextView textView, int line, int col, out TextSpan startBraceSpan, out TextSpan endBraceSpan)
        {
            bool found = false;
            startBraceSpan = new TextSpan();
            endBraceSpan = new TextSpan();

            // Synchronously return the matching brace location.      
            BackgroundRequest req = null;

            if (this.lastBraceMatchRequest != null && this.lastBraceMatchRequest.Timestamp == this.ChangeCount)
            {
                req = this.lastBraceMatchRequest;
            }
            else
            {
                req = this.BeginBackgroundRequest(line, col, new TokenInfo(), BackgroundRequestReason.MatchBraces, textView, RequireFreshResults.No, new BackgroundRequestResultHandler(HandleGetPairExtentResponse));
                // This blocks the UI thread. Give it a slice of time (20ms) and then just give up on this particular brace match.
                // If we end up aborting here then we'll return false and the user won't see a brace match. When HandleGetPairExtentResponse
                // is called it will fill in lastBraceMatchRequest which may match a future request.
                if (req == null || req.result == null || !req.result.TryWaitForBackgroundRequestCompletion(20))
                {
                    return false;
                }
            }

            if (req.ResultSink == null) return false;

            ArrayList braces = req.ResultSink.Braces;
            int matches = braces.Count;
            if (matches == 0)
                return found;

            // The following algorithm allows for multiple potentially overlapping
            // matches to be returned. This is because the same pairs used here are
            // also used in brace highlighting, and brace highlighting supports
            // MatchTriple which include more than 2 spans.  So here
            // we have to find which of the spans we are currently inside.

            int i = 0;
            for (i = 0; i < matches; i++)
            {
                BraceMatch m = (BraceMatch)req.ResultSink.Braces[i];
                TripleMatch t = m as TripleMatch;
                if (TextSpanHelper.ContainsInclusive(m.a, line, col))
                {
                    startBraceSpan = m.a;
                    endBraceSpan = m.b;
                    found = true;
                    break;
                }
                else if (TextSpanHelper.ContainsInclusive(m.b, line, col))
                {
                    if (t != null)
                    {
                        startBraceSpan = m.b;
                        endBraceSpan = t.c;
                    }
                    else
                    {
                        startBraceSpan = m.a;
                        endBraceSpan = m.b;
                    }
                    found = true;
                    break;
                }
                else if (t != null && TextSpanHelper.ContainsInclusive(t.c, line, col))
                {
                    startBraceSpan = m.a;
                    endBraceSpan = t.c;
                    found = true;
                    break;
                }
            }

            return found;
        }

        void HandleGetPairExtentResponse(BackgroundRequest request)
        {
            if (this.service == null)
            {
                return;
            }
            this.lastBraceMatchRequest = request;
        }

        public BackgroundRequest BeginBackgroundRequest(int line, int idx, TokenInfo info, BackgroundRequestReason reason, IVsTextView view, RequireFreshResults requireFreshResults, BackgroundRequestResultHandler callback, MethodTipMiscellany methodTipMiscellany = 0)
        {
            var wpfTextView = GetWpfTextViewFromVsTextView(view);
            var snapshot = wpfTextView.TextSnapshot;

            if (this.disposed) return null;
            string fname = this.GetFilePath();

            Debug.Assert(snapshot != null);
            Debug.Assert(callback != null);

            // Check if we can shortcut executing the background request and just fill in the latest
            // cached scope for the active view from this.service.RecentFullTypeCheckResults.
            //
            // This must only kick in if ExecuteBackgroundRequest is equivalent to fetching a recent,
            // perhaps out-of-date scope.
            if (!this.NeedsVisualRefresh &&
                this.service.IsRecentScopeSufficientForBackgroundRequest(reason) &&
                (this.service.RecentFullTypeCheckResults != null) &&
                this.service.RecentFullTypeCheckFile.Equals(fname) &&
                requireFreshResults != RequireFreshResults.Yes)
            {
                BackgroundRequest request = this.service.CreateBackgroundRequest(this, line, idx, info, null, snapshot, methodTipMiscellany, fname, reason, view);
                request.ResultIntellisenseInfo = this.service.RecentFullTypeCheckResults;
                request.ResultClearsDirtinessOfFile = false;
                request.Timestamp = this.ChangeCount;
                request.IsSynchronous = true;
                callback(request);
                return request;
            }
            else
            {

                string text = this.GetText(); // get all the text
                BackgroundRequest request = this.service.CreateBackgroundRequest(this, line, idx, info, text, snapshot, methodTipMiscellany, fname, reason, view);
                request.Timestamp = this.ChangeCount;
                request.DirtySpan = this.dirtySpan;
                request.RequireFreshResults = requireFreshResults;

                if (!this.LanguageService.Preferences.EnableAsyncCompletion)
                {
                    request.IsSynchronous = true; //unless registry value indicates that sync ops always prefer async 
                }
                if (request.IsSynchronous)
                {
                    this.service.ExecuteBackgroundRequest(request);
                    callback(request);
                }
                else
                {
                    request.result = this.service.BeginBackgroundRequest(request, callback);
                }
                return request;
            }
        }

        public void HandleResponseHelper(BackgroundRequest req)
        {
            try
            {
                if (this.service == null) return;
                // If the request is out of sync with the buffer, then the error spans
                // and hidden regions could be wrong, so we ignore this parse and wait 
                // for the next OnIdle parse.
                if (req.ResultClearsDirtinessOfFile && req.Timestamp == this.ChangeCount)
                {
                    this.RecordViewRefreshed();
                    if (req.ResultIntellisenseInfo != null)
                    {

                        int end = Environment.TickCount;
                        if (req.StartTimeForOnIdleRequest != 0 && end > req.StartTimeForOnIdleRequest)
                        {
#if LANGTRACE
                            Trace.WriteLine("BackgroundRequest in " + (end - start) + " ticks");
#endif
                            this.lastOnIdleRequestDuration = end - req.StartTimeForOnIdleRequest; // for OnIdle loop
                        }

                        if (req.View == this.service.LastActiveTextView)
                        {
                            this.service.RecentFullTypeCheckResults = req.ResultIntellisenseInfo;
                            this.service.RecentFullTypeCheckFile = req.FileName;
                        }
                        ReportTasks(req.ResultSink.errors);
                    }
                    this.service.OnParseFileOrCheckFileComplete(req);
                }
            }
            catch
            {
            }

        }


        public void HandleUntypedParseOrFullTypeCheckResponse(BackgroundRequest req)
        {
            if (this.service == null) return;
            try
            {
                Debug.Assert((req.Reason == BackgroundRequestReason.ParseFile || req.Reason == BackgroundRequestReason.FullTypeCheck), "this callback is being used for the wrong type of parse request");
#if LANGTRACE
                Trace.WriteLine("HandleUntypedParseOrFullTypeCheckResponse:" + req.Timestamp);
#endif
                if (this.service == null) return;

                HandleResponseHelper(req);
#if LANGTRACE
            } catch (Exception e) {
                Trace.WriteLine("HandleUntypedParseOrFullTypeCheckResponse exception: " + e.Message);
#endif
            }
            catch
            {
            }
        }

        public void FixupMarkerSpan(ref TextSpan span)
        {
            // This is similar to TextSpanHelper.Normalize except that 
            // we try not to create empty spans at end of line, since VS doesn't like
            // empty spans for text markers.  See comment in CreateMaker in DocumentTask.cs 

            //adjust max. lines
            int lineCount;
            if (NativeMethods.Failed(this.textLines.GetLineCount(out lineCount)))
                return;
            span.iEndLine = Math.Min(span.iEndLine, lineCount - 1);
            //make sure the start is still before the end
            if (!TextSpanHelper.IsPositive(span))
            {
                span.iStartLine = span.iEndLine;
                span.iStartIndex = span.iEndIndex;
            }
            //adjust for line length
            int lineLength;
            if (NativeMethods.Failed(this.textLines.GetLengthOfLine(span.iStartLine, out lineLength)))
                return;
            span.iStartIndex = Math.Min(span.iStartIndex, lineLength);
            if (NativeMethods.Failed(this.textLines.GetLengthOfLine(span.iEndLine, out lineLength)))
                return;
            span.iEndIndex = Math.Min(span.iEndIndex, lineLength);

            if (TextSpanHelper.IsEmpty(span) && span.iStartIndex == lineLength && span.iEndLine + 1 < lineCount)
            {
                // Make the span include the newline if it was empty and at the end of the line.
                span.iEndLine++;
                span.iEndIndex = 0;
            }
        }

        public void ReportTasks(ArrayList errors)
        {
            Microsoft.VisualStudio.Shell.TaskProvider taskProvider = this.GetTaskProvider();
            TaskReporter tr = this.GetTaskReporter();

            if (null == taskProvider || null == tr)
            {
                Debug.Assert(false, "null task provider or task reporter - exiting ReportTasks");
                return;
            }

            string fname = this.GetFilePath();

            // Clear out this file's tasks
            tr.ClearBackgroundTasksForFile(fname);

            int errorMax = this.service.Preferences.MaxErrorMessages;
            Microsoft.VisualStudio.Shell.RunningDocumentTable rdt = new Microsoft.VisualStudio.Shell.RunningDocumentTable(this.service.Site);
            IVsHierarchy thisHeirarchy = rdt.GetHierarchyItem(fname);

            // Here we merge errors lists to reduce flicker.  It is not a very intelligent merge
            // but that is ok, the worst case is the task list flickers a bit.  But 99% of the time
            // one error is added or removed as the user is typing, and this merge will reduce flicker
            // in this case.
            errors = GroupBySeverity(errors);
            taskProvider.SuspendRefresh(); // batch updates.
            Microsoft.VisualStudio.Shell.TaskErrorCategory mostSevere = Microsoft.VisualStudio.Shell.TaskErrorCategory.Message;

            for (int i = 0, n = errors.Count; i < n; i++)
            {
                ErrorNode enode = (ErrorNode)errors[i];
                string filename = enode.uri;
                string subcategory = enode.subcategory;
                bool thisFile = (!string.IsNullOrEmpty(filename) && NativeMethods.IsSamePath(fname, filename));

                TextSpan span = enode.context;
                Severity severity = enode.severity;
                string message = enode.message;
                if (message == null) continue;

                message = NormalizeErrorString(message);

                //normalize text span
                if (thisFile)
                {
                    FixupMarkerSpan(ref span);
                }
                else
                {
                    TextSpanHelper.MakePositive(ref span);
                }
                //set options
                Microsoft.VisualStudio.Shell.TaskPriority priority = Microsoft.VisualStudio.Shell.TaskPriority.Normal;
                Microsoft.VisualStudio.Shell.TaskCategory category = Microsoft.VisualStudio.Shell.TaskCategory.BuildCompile;
                MARKERTYPE markerType = MARKERTYPE.MARKER_CODESENSE_ERROR;
                Microsoft.VisualStudio.Shell.TaskErrorCategory errorCategory = Microsoft.VisualStudio.Shell.TaskErrorCategory.Warning;

                if (severity == Severity.Fatal || severity == Severity.Error)
                {
                    priority = Microsoft.VisualStudio.Shell.TaskPriority.High;
                    errorCategory = Microsoft.VisualStudio.Shell.TaskErrorCategory.Error;
                }
                else if (severity == Severity.Hint)
                {
                    category = Microsoft.VisualStudio.Shell.TaskCategory.Comments;
                    markerType = MARKERTYPE.MARKER_INVISIBLE;
                    errorCategory = Microsoft.VisualStudio.Shell.TaskErrorCategory.Message;
                }
                else if (severity == Severity.Warning)
                {
                    markerType = MARKERTYPE.MARKER_COMPILE_ERROR;
                    errorCategory = Microsoft.VisualStudio.Shell.TaskErrorCategory.Warning;
                }
                if (errorCategory < mostSevere)
                {
                    mostSevere = errorCategory;
                }

                IVsHierarchy hierarchy = thisHeirarchy;
                if (!thisFile)
                {
                    // must be an error reference to another file.
                    hierarchy = rdt.GetHierarchyItem(filename);
                    markerType = MARKERTYPE.MARKER_OTHER_ERROR; // indicate to CreateErrorTaskItem
                }

                DocumentTask docTask = this.CreateErrorTaskItem(span, filename, subcategory, message, priority, category, markerType, errorCategory);
                docTask.HierarchyItem = hierarchy;
                tr.AddTask(docTask);

            }

            tr.OutputTaskList();
            taskProvider.ResumeRefresh(); // batch updates.
        }

        private static ArrayList GroupBySeverity(ArrayList errors)
        {
            // Sort the errors by severity so that if there's more than 'max' errors, then
            // the errors actually reported are the most severe.  I do not use ArrayList.Sort 
            // because that would lose the order inherent in each group of errors provided by 
            // the language service, which is most likely some sort of parse-order which will 
            // make more sense to the user.
            ArrayList result = new ArrayList();
            foreach (Severity s in new Severity[] { Severity.Fatal, Severity.Error, Severity.Warning, Severity.Hint })
            {
                foreach (ErrorNode e in errors)
                {
                    if (e.severity == s)
                    {
                        result.Add(e);
                    }
                }
            }
            return result;
        }

        public void RemoveTask(DocumentTask task)
        {
            for (int i = 0, n = this.taskProvider.Tasks.Count; i < n; i++)
            {
                Microsoft.VisualStudio.Shell.Task current = this.taskProvider.Tasks[i];
                if (current == task)
                {
                    this.taskProvider.Tasks.RemoveAt(i); return;
                }
            }
        }

        private void RemoveHiddenRegions()
        {
            IVsHiddenTextSession session = GetHiddenTextSession();
            IVsEnumHiddenRegions ppenum;
            TextSpan[] aspan = new TextSpan[1];
            aspan[0] = GetDocumentSpan();
            NativeMethods.ThrowOnFailure(session.EnumHiddenRegions((uint)FIND_HIDDEN_REGION_FLAGS.FHR_BY_CLIENT_DATA, (uint)SourceConstants.HiddenRegionCookie, aspan, out ppenum));
            uint fetched;
            IVsHiddenRegion[] aregion = new IVsHiddenRegion[1];
            while (ppenum.Next(1, aregion, out fetched) == NativeMethods.S_OK && fetched == 1)
            {
                NativeMethods.ThrowOnFailure(aregion[0].Invalidate((int)CHANGE_HIDDEN_REGION_FLAGS.chrNonUndoable));
            }

        }

        public void ToggleRegions()
        {
            IVsHiddenTextSession session = GetHiddenTextSession();
            IVsEnumHiddenRegions ppenum;
            TextSpan[] aspan = new TextSpan[1];
            aspan[0] = GetDocumentSpan();
            NativeMethods.ThrowOnFailure(session.EnumHiddenRegions((uint)FIND_HIDDEN_REGION_FLAGS.FHR_BY_CLIENT_DATA, (uint)SourceConstants.HiddenRegionCookie, aspan, out ppenum));
            uint fetched;
            IVsHiddenRegion[] aregion = new IVsHiddenRegion[1];
            using (new CompoundAction(this, "ToggleAllRegions"))
            {
                while (ppenum.Next(1, aregion, out fetched) == NativeMethods.S_OK && fetched == 1)
                {
                    uint dwState;
                    aregion[0].GetState(out dwState);
                    dwState ^= (uint)HIDDEN_REGION_STATE.hrsExpanded;
                    NativeMethods.ThrowOnFailure(aregion[0].SetState(dwState,
                        (uint)CHANGE_HIDDEN_REGION_FLAGS.chrDefault));
                }
            }
        }

        public void DisableOutlining()
        {
            this.OutliningEnabled = false;
            this.RemoveHiddenRegions();
        }


        public IVsHiddenTextSession GetHiddenTextSession()
        {
            if (this.hiddenTextSession == null)
            {
                IVsHiddenTextManager htextmgr = service.Site.GetService(typeof(SVsTextManager)) as IVsHiddenTextManager;
                if (htextmgr != null)
                {
                    IVsHiddenTextSession session = null;
                    int hr = htextmgr.GetHiddenTextSession(textLines, out session);
                    if (hr == NativeMethods.E_FAIL)
                    {
                        // Then there was no hidden text session.
                        NativeMethods.ThrowOnFailure(htextmgr.CreateHiddenTextSession(0, textLines, this, out session));
                    }
                    this.hiddenTextSession = session;
                }
            }
            return this.hiddenTextSession;
        }

        public bool OutliningEnabled
        {
            get
            {
                return this.doOutlining;
            }
            set
            {
                if (this.doOutlining != value)
                {
                    this.doOutlining = value;
                    if (value)
                    {
                        this.RecordChangeToView();
                        // force reparse as soon as possible.
                        this.DirtyTime = Math.Max(0, this.DirtyTime - this.service.Preferences.CodeSenseDelay);
                    }
                }
            }
        }

        public void OnHiddenRegionChange(IVsHiddenRegion region, HIDDEN_REGION_EVENT evt, int fBufferModifiable)
        {
        }

        public int GetTipText(IVsHiddenRegion region, string[] result)
        {
            if (result != null && result.Length > 0)
            {
                TextSpan[] aspan = new TextSpan[1];
                NativeMethods.ThrowOnFailure(region.GetSpan(aspan));
                result[0] = this.GetText(aspan[0]);
            }
            return NativeMethods.S_OK;
        }

        public int GetMarkerCommandInfo(IVsHiddenRegion region, int item, string[] outText, uint[] flags)
        {
            if (flags != null && flags.Length > 0)
                flags[0] = 0;
            if (outText != null && outText.Length > 0)
                outText[0] = null;
            return NativeMethods.E_NOTIMPL;
        }

        public int ExecMarkerCommand(IVsHiddenRegion region, int cmd)
        {
            return NativeMethods.E_NOTIMPL;
        }

        public int MakeBaseSpanVisible(IVsHiddenRegion region, TextSpan[] span)
        {
            return NativeMethods.E_NOTIMPL;
        }

        public void OnBeforeSessionEnd()
        {
        }

        public abstract void OnUserDataChange(ref Guid riidKey, object vtNewValue);
    }

    /// <summary>
    /// This class can be used in a using statement to open and close a compound edit action
    /// via IVsCompoundAction interface.  Be sure to call Close() at the end of your using
    /// statement, otherwise Dispose will call Abort.
    /// </summary>
    internal class CompoundAction : IDisposable
    {
        IVsCompoundAction action;
        bool opened;
        ISource src;
        Colorizer colorizer;

        internal CompoundAction(ISource src, string description)
        {
            this.opened = false;
            this.src = src;
            this.action = (IVsCompoundAction)src.GetTextLines();
            if (this.action == null)
            {
                throw new ArgumentNullException("(IVsCompoundAction)src.GetTextLines()");
            }
            NativeMethods.ThrowOnFailure(action.OpenCompoundAction(description));
            this.opened = true;
            this.colorizer = src.GetColorizer();
            if (colorizer != null) colorizer.Suspend(); // batch colorization            
        }

        internal void FlushEditActions()
        {
            // in case there is already a compound action under way, this enables the caller
            // to see a consistent buffer coordinate system.
            action.FlushEditActions(); // sometimes returns E_NOTIMPL and this is expected!            
        }

        public virtual void Dispose()
        {
            Close();
        }

        internal void Close()
        {
            if (opened && action != null)
            {
                action.CloseCompoundAction();
                action = null;
                opened = false;
                ResumeColorization();
            }
        }

        internal void Abort()
        {
            if (opened && action != null)
            {
                action.AbortCompoundAction();
                action = null;
                opened = false;
                ResumeColorization(); // batch colorization
            }
        }

        void ResumeColorization()
        {
            if (colorizer != null)
            {
                colorizer.Resume(); // batch colorization
                TextSpan span = src.DirtySpan;
                int start = span.iStartLine;
                int end = span.iEndLine;
                src.Recolorize(start, end);
                colorizer = null;
            }
        }
    }

    /// <summary>
    /// This class can be used in a using statement to open and close a compound edit action
    /// via IVsCompoundAction interface from an IVsTextView.  This allows the view to optimize 
    /// it's updates based on edits you are making on the buffer, so it's the preferred way of
    /// doing things if you have access to the IVsTextView.  If not, use CompoundAction.
    /// </summary>
    internal class CompoundViewAction : IDisposable
    {
        IVsCompoundAction action;
        bool opened;
        internal CompoundViewAction(IVsTextView view, string description)
        {
            opened = false;
            action = (IVsCompoundAction)view;
            if (this.action == null)
            {
                throw new ArgumentNullException("(IVsCompoundAction)view");
            }
            NativeMethods.ThrowOnFailure(action.OpenCompoundAction(description));
            opened = true;
        }

        internal void FlushEditActions()
        {
            // in case there is already a compound action under way, this enables the caller
            // to see a consistent buffer coordinate system.
            action.FlushEditActions(); // sometimes returns E_NOTIMPL and this is expected!            
        }

        /// <summary>This method calls Close if you have not already called Close</summary>
        public virtual void Dispose()
        {
            Close();
        }

        internal void Close()
        {
            if (opened && action != null)
            {
                action.CloseCompoundAction();
                action = null;
                opened = false;
            }
        }

        internal void Abort()
        {
            if (opened && action != null)
            {
                action.AbortCompoundAction();
                action = null;
                opened = false;
            }
        }
    }


    //==================================================================================
    internal sealed class CompletionSet : IVsCompletionSet, IVsCompletionSetEx, IDisposable
    {
        ImageList imageList;
        bool displayed;
        string committedWord;
        char commitChar;
        int commitIndex;
        IVsTextView textView;
        Declarations decls;
        string filterText;
        ISource source;
        bool isCommitted;
        bool wasUnique;
        int initialLine = 0;
        int initialIndex = 0;
        bool haveInitialLineAndIndex = false;

        internal CompletionSet(ImageList imageList, ISource source)
        {
            this.imageList = imageList;
            this.source = source;
        }

        public bool IsDisplayed
        {
            get
            {
                return this.displayed;
            }
        }

        public bool IsCommitted
        {
            get
            {
                return this.isCommitted;
            }
        }

        public string OnCommitText
        {
            get
            {
                return this.committedWord;
            }
        }

        public void Init(IVsTextView textView, Declarations declarations, bool completeWord)
        {
            Close();
            this.textView = textView;
            this.decls = declarations;
            this.filterText = "";

            //check if we have members
            long count = decls.GetCount(this.filterText);
            if (count <= 0) return;

            //initialise and refresh      
            UpdateCompletionFlags flags = UpdateCompletionFlags.UCS_NAMESCHANGED;

            if (completeWord) flags |= UpdateCompletionFlags.UCS_COMPLETEWORD;

            this.wasUnique = false;
            this.initialLine = 0;
            this.initialIndex = 0;
            this.haveInitialLineAndIndex = false;

            int hr = textView.UpdateCompletionStatus(this, (uint)flags);
            NativeMethods.ThrowOnFailure(hr);

            this.displayed = (!this.wasUnique || !completeWord);
        }

        public void Dispose()
        {
            Close();
            if (imageList != null) imageList.Dispose();
            this.imageList = null;
        }

        public void Close()
        {
            if (this.displayed && this.textView != null)
            {
                // Here we can't throw or exit because we need to call Dispose on
                // the disposable membres.
                try
                {
                    textView.UpdateCompletionStatus(null, 0);
                }
                catch (COMException)
                {
                }
            }
            this.displayed = false;
            this.textView = null;
            this.initialLine = 0;
            this.initialIndex = 0;
            this.haveInitialLineAndIndex = false;
        }

        public char OnAutoComplete()
        {
            this.isCommitted = false;
            if (this.decls != null)
            {
                return this.decls.OnAutoComplete(this.textView, this.committedWord, this.commitChar, this.commitIndex);
            }
            return '\0';
        }

        //--------------------------------------------------------------------------
        //IVsCompletionSet methods
        //--------------------------------------------------------------------------
        public int GetImageList(out IntPtr phImages)
        {
            phImages = this.imageList.Handle;
            return NativeMethods.S_OK;
        }

        public uint GetFlags()
        {
            return (uint)UpdateCompletionFlags.CSF_HAVEDESCRIPTIONS | (uint)UpdateCompletionFlags.CSF_CUSTOMCOMMIT | (uint)UpdateCompletionFlags.CSF_INITIALEXTENTKNOWN | (uint)UpdateCompletionFlags.CSF_CUSTOMMATCHING;
        }

        public int GetCount()
        {
            return this.decls.GetCount(this.filterText);
        }

        public int GetDisplayText(int index, out string text, int[] glyph)
        {
            if (glyph != null)
            {
                glyph[0] = this.decls.GetGlyph(this.filterText, index);
            }
            text = this.decls.GetDisplayText(this.filterText, index);
            return NativeMethods.S_OK;
        }

        public int GetDescriptionText(int index, out string description)
        {
            description = this.decls.GetDescription(this.filterText, index);
            return NativeMethods.S_OK;
        }

        public int GetInitialExtent(out int line, out int startIdx, out int endIdx)
        {

            int hr = NativeMethods.S_OK;

            // Record the initial line and index the first time this is called since Init, for use in the MemberSelect code below.
            if (!this.haveInitialLineAndIndex)
            {
                NativeMethods.ThrowOnFailure(this.textView.GetCaretPos(out this.initialLine, out this.initialIndex));
                this.haveInitialLineAndIndex = true;
            }

            // When we're doing a MemberSelect, don't use the token to the right
            // of the cursor position
            if (this.decls.Reason == BackgroundRequestReason.MemberSelect)
            {
                int lineIgnored;
                int caretIdx;
                NativeMethods.ThrowOnFailure(this.textView.GetCaretPos(out lineIgnored, out caretIdx));
                line = this.initialLine;
                startIdx = this.initialIndex;
                endIdx = caretIdx;
            }
            else
            {
                int idx;
                NativeMethods.ThrowOnFailure(this.textView.GetCaretPos(out line, out idx));
                hr = GetTokenExtent(line, idx, out startIdx, out endIdx);
            }

            Debug.Assert(TextSpanHelper.ValidCoord(this.source, line, startIdx) &&
                TextSpanHelper.ValidCoord(this.source, line, endIdx));
            return hr;
        }

        int GetTokenExtent(int line, int idx, out int startIdx, out int endIdx)
        {
            int hr = Microsoft.VisualStudio.VSConstants.S_OK;
            bool rc = this.source.GetWordExtent(line, idx, FSharpSourceBase.WholeToken, out startIdx, out endIdx);
            // make sure the span is positive.
            endIdx = Math.Max(startIdx, endIdx);

            if (!rc && idx > 0)
            {
                rc = this.source.GetWordExtent(line, idx - 1, FSharpSourceBase.WholeToken, out startIdx, out endIdx);
                if (!rc)
                {
                    // Must stop core text editor from looking at startIdx and endIdx since they are likely
                    // invalid.  So we must return a real failure here, not just S_FALSE.
                    startIdx = endIdx = idx;
                    hr = Microsoft.VisualStudio.VSConstants.E_NOTIMPL;
                }
                else
                {
                    endIdx = Math.Max(endIdx, idx);
                }
            }
            return hr;
        }

        public int GetBestMatch(string textSoFar, int length, out int index, out uint flags)
        {
            flags = 0;
            index = 0;
#if     TRACE_PARSING
            Trace.WriteLine("GetBestMatch '" + textSoFar + "'");
#endif
            if (textSoFar != this.filterText)
            {
                System.Threading.SynchronizationContext.Current.Post((object state) =>
                {
                    this.filterText = textSoFar;
                    if (this.textView != null)
                    {
                        UpdateCompletionFlags ucsFlags = UpdateCompletionFlags.UCS_NAMESCHANGED;
                        NativeMethods.ThrowOnFailure(textView.UpdateCompletionStatus(this, (uint)ucsFlags));
                    }
                }, null);
            }

            bool uniqueMatch = false;
            bool shouldSelectItem = false;
            this.decls.GetBestMatch(this.filterText, textSoFar, out index, out uniqueMatch, out shouldSelectItem);
            if (index < 0 || index >= GetCount())
            {
                index = 0;
                uniqueMatch = false;
            }
            else if (shouldSelectItem)
            {
                // Indicate that we want to select something in the list.
                flags = (uint)UpdateCompletionFlags.GBM_SELECT;
            }
            if (uniqueMatch)
            {
                flags |= (uint)UpdateCompletionFlags.GBM_UNIQUE;
                this.wasUnique = true;
            }
            return NativeMethods.S_OK;
        }

        public int OnCommit(string textSoFar, int index, int selected, ushort commitChar, out string completeWord)
        {
            char ch = (char)commitChar;
            bool isCommitChar = true;
#if     TRACE_PARSING
            Trace.WriteLine("OnCommit '" + textSoFar + "'," + index + "," + selected + "," + commitChar.ToString(CultureInfo.CurrentUICulture));
#endif
            if (commitChar != 0)
            {
                // if the char is in the list of given member names then obviously it
                // is not a commit char.
                int i = (textSoFar == null) ? 0 : textSoFar.Length;
                for (int j = 0, n = decls.GetCount(this.filterText); j < n; j++)
                {
                    string name = decls.GetName(this.filterText, j);
                    if (name.Length > i && name[i] == commitChar)
                    {
                        if (i == 0 || String.Compare(name.Substring(0, i), textSoFar, true, CultureInfo.CurrentUICulture) == 0)
                        {
                            goto nocommit; // cannot be a commit char if it is an expected char in a matching name
                        }
                    }
                }
                isCommitChar = this.decls.IsCommitChar(ch);
            }

            completeWord = textSoFar;
            if (isCommitChar)
            {
                if (selected == 0) index = -1;
                this.committedWord = completeWord = this.decls.OnCommit(this.filterText, index);
                this.commitChar = ch;
                this.commitIndex = index;
                this.isCommitted = true;
                return NativeMethods.S_OK;
            }
        nocommit:
            // S_FALSE return means the character is not a commit character.
            completeWord = textSoFar;
            return NativeMethods.S_FALSE;
        }

        public void Dismiss()
        {
            this.displayed = false;
            this.Close();
        }

        public int CompareItems(string bstrSoFar, string bstrOther, int lCharactersToCompare, out int plResult)
        {
            plResult = 0;
            return NativeMethods.E_NOTIMPL;
        }

        public int IncreaseFilterLevel(int iSelectedItem)
        {
            return NativeMethods.E_NOTIMPL;
        }

        public int DecreaseFilterLevel(int iSelectedItem)
        {
            return NativeMethods.E_NOTIMPL;
        }

        public int GetCompletionItemColor(int iIndex, out uint dwFGColor, out uint dwBGColor)
        {
            dwFGColor = dwBGColor = 0;
            return NativeMethods.E_NOTIMPL;
        }

        public int GetFilterLevel(out int iFilterLevel)
        {
            iFilterLevel = 0;
            return NativeMethods.E_NOTIMPL;
        }

        public int OnCommitComplete()
        {
            CodeWindowManager mgr = this.source.LanguageService.GetCodeWindowManagerForView(this.textView);
            if (mgr != null)
            {
                ViewFilter filter = mgr.GetFilter(this.textView);
                if (filter != null)
                {
                    filter.OnAutoComplete();
                }
            }
            return NativeMethods.S_OK;
        }
    }

    //-------------------------------------------------------------------------------------

    internal sealed class MethodData : IVsMethodData, IDisposable
    {
        IServiceProvider provider;
        IVsMethodTipWindow methodTipWindow;
        MethodListForAMethodTip methods;
        private NativeStringsCacheForOverloads nativeStringsCacheForOverloads;
        int currentParameter;
        int currentMethod;
        bool displayed;
        IVsTextView textView;
        TextSpan context;

        internal MethodData(IServiceProvider site)
        {
            this.provider = site;
            Microsoft.VisualStudio.Shell.Package pkg = (Microsoft.VisualStudio.Shell.Package)site.GetService(typeof(Microsoft.VisualStudio.Shell.Package));
            if (pkg == null)
            {
                throw new NullReferenceException(typeof(Microsoft.VisualStudio.Shell.Package).FullName);
            }
            Guid riid = typeof(IVsMethodTipWindow).GUID;
            Guid clsid = typeof(VsMethodTipWindowClass).GUID;
            this.methodTipWindow = (IVsMethodTipWindow)pkg.CreateInstance(ref clsid, ref riid, typeof(IVsMethodTipWindow));
            if (this.methodTipWindow != null)
            {
                NativeMethods.ThrowOnFailure(methodTipWindow.SetMethodData(this));
            }
        }

        public IServiceProvider Provider
        {
            get { return this.provider; }
            set { this.provider = value; }
        }

        public IVsMethodTipWindow MethodTipWindow
        {
            get { return this.methodTipWindow; }
            set { this.methodTipWindow = value; }
        }

        public IVsTextView TextView
        {
            get { return this.textView; }
            set { this.textView = value; }
        }

        public bool IsDisplayed
        {
            get
            {
                return this.displayed;
            }
        }

        private static bool MethodsSeemToDiffer(MethodListForAMethodTip a, MethodListForAMethodTip b)
        {
            // this is an approximate test, that is good enough in practice
            return (a.GetName(0) != b.GetName(0))
                || (a.GetCount() != b.GetCount())
                || (!(a.GetNoteworthyParamInfoLocations()[0].Equals(b.GetNoteworthyParamInfoLocations()[0])));
        }

        private static HashSet<string> FormalParamNames(MethodListForAMethodTip m, int index)
        {
            int numParams = m.GetParameterCount(index);
            HashSet<string> hs = new HashSet<string>();
            for (int i = 0; i < numParams; ++i)
            {
                string name, display, description;
                m.GetParameterInfo(index, i, out name, out display, out description);
                hs.Add(name);
            }
            return hs;
        }

        public void Refresh(IVsTextView textView, MethodListForAMethodTip methods, TextSpan context, MethodTipMiscellany methodTipMiscellany)
        {
            bool needToDismissNow = false;
            if (this.displayed && methodTipMiscellany == MethodTipMiscellany.JustPressedBackspace
                && MethodsSeemToDiffer(this.methods, methods))
            {
                // We just hit backspace, and apparently the 'set of methods' changed.  This most commonly happens in a case like
                //     foo(42, bar(    // in a tip for bar(), now press backspace
                //     foo(42, bar     // now we're in a location where we'd be in a tip for foo()
                // and we want to dismiss the tip.
                needToDismissNow = true;
            }
            this.methods = methods;
            if (nativeStringsCacheForOverloads != null)
            {
                nativeStringsCacheForOverloads.Free();
            }
            nativeStringsCacheForOverloads = new NativeStringsCacheForOverloads(methods.GetCount(), this);
            this.context = context;
            this.textView = textView;
            if (needToDismissNow)
            {
                this.Dismiss();
            }
            else
            {
                this.Refresh(methodTipMiscellany);
            }
        }
        public void Refresh(MethodTipMiscellany methodTipMiscellany)
        {
            var wpfTextView = FSharpSourceBase.GetWpfTextViewFromVsTextView(textView);
            var ranges = methods.GetParameterRanges();
            Debug.Assert(ranges != null && ranges.Length > 0);

            // Don't do anything for open parens and commas that aren't intrinsic to a method call
            if (!this.displayed && methodTipMiscellany == MethodTipMiscellany.JustPressedCloseParen)
            {
                return;  // close paren must never cause it to appear
            }
            if (!this.displayed && methodTipMiscellany == MethodTipMiscellany.JustPressedOpenParen)
            {
                // the only good open paren is the start of the first param, don't want to cause a tip to be displayed $here$:  foo(x, $($y+z), w)
                if (!ranges[0].GetSpan(wpfTextView.TextSnapshot).Start.Equals(wpfTextView.Caret.Position.BufferPosition))
                    return;
            }
            if (!this.displayed && methodTipMiscellany == MethodTipMiscellany.JustPressedComma)
            {
                // the only good commas will be, interestingly, at the start of the following param span
                // this by virtue of fact that we assume comma is one character after where the previous parameter range ends (and thus the first character of the next range)
                var ok = false;
                for (int i = 1; i < ranges.Length; ++i)
                {
                    if (ranges[i].GetSpan(wpfTextView.TextSnapshot).Start.Equals(wpfTextView.Caret.Position.BufferPosition))
                    {
                        ok = true;
                        break;
                    }
                }
                if (!ok)
                    return;
            }

            // Figure out which parameter we are in based on location of caret within the parse
            var caretPoint = wpfTextView.Caret.Position.BufferPosition;
            var endOfLinePoint = wpfTextView.Caret.ContainingTextViewLine.End;
            bool caretIsAtEndOfLine = caretPoint == endOfLinePoint;
            this.currentParameter = -1;
            for (int i = ranges.Length - 1; i >= 0; --i) // need to traverse backwards, so that "f(1," at end of line sets curParam to 1, not 0
            {
                if (ranges[i].GetSpan(wpfTextView.TextSnapshot).Contains(caretPoint)
                    // ranges are half-open [...), so end-of-line is a special case...
                    || caretIsAtEndOfLine
                       && ranges[i].GetSpan(wpfTextView.TextSnapshot).End == caretPoint
                       && !(i == ranges.Length - 1 && this.methods.IsThereACloseParen())) // ...except on the closing paren (when caret at close paren, should dismiss tip, even if at EOL)
                {
                    this.currentParameter = i;
                    break;
                }
            }
            if (this.currentParameter == -1)
            {
                // a bit of a kludge; if they just backspaced over the last comma and there's no close parenthesis, the caret is just to the right of all the param
                // ranges, but we don't want to dismiss the tip.  so look just left of the caret and see if that would be inside the final param
                if (methodTipMiscellany == MethodTipMiscellany.JustPressedBackspace
                    && ranges[ranges.Length - 1].GetSpan(wpfTextView.TextSnapshot).Contains(wpfTextView.Caret.Position.BufferPosition.Subtract(1)))
                {
                    this.currentParameter = ranges.Length - 1;
                }
                else
                {
                    // the caret moved left of the open parenthesis or right of the close parenthesis
                    this.currentParameter = 0;
                    this.Dismiss();
                }
            }
            // possibly select a method from overload list based on current num of params the user has
            if (methodTipMiscellany == MethodTipMiscellany.ExplicitlyInvokedViaCtrlShiftSpace
                || methodTipMiscellany == MethodTipMiscellany.JustPressedComma)
            {
                Debug.Assert(this.methods != null, "this can happen if we just called Dismiss() because we erroneously decided the caret moves outside the parens");

                var actualParamNames = System.Linq.Enumerable.ToList(System.Linq.Enumerable.Where(this.methods.GetParameterNames(), s => null != s));

                int numOfParamsUserHasSoFar = methods.GetNoteworthyParamInfoLocations().Length - 3; // -3 because first 3 ranges are [LID.start, LID.end, Paren], rest are params
                // however note that this is never zero, "foo(" and "foo(x"  both report 1

                int curMeth = this.currentMethod;  // start wherever the user already is.  the methods are already ordered in increasing order-of-params; only can increase of user has longer param list.
                while (curMeth < this.methods.GetCount()
                       && this.methods.GetParameterCount(curMeth) < numOfParamsUserHasSoFar
                       // if we're on a zero-arg overload, don't go past it when user has one arg; be like C#, good for e.g. Console.WriteLine(
                       && !(numOfParamsUserHasSoFar == 1 && this.methods.GetParameterCount(curMeth) == 0))
                {
                    curMeth++;
                }
                if (curMeth == this.methods.GetCount())
                {
                    // if they have 'too many' parameters, always just start at the beginning of the list
                    curMeth = 0;
                }
                else if (actualParamNames.Count > 0)
                {
                    // there are named parameters, additionally ensure the selected overload has those names, or keep advancing until it does
                    while (curMeth < this.methods.GetCount()
                           && !FormalParamNames(this.methods, curMeth).IsSupersetOf(actualParamNames))
                    {
                        curMeth++;
                    }
                }
                this.currentMethod = curMeth;
            }
            else
            {
                // in other cases, don't update which overload we're on, the user is in control
            }
            this.UpdateView();
        }

        public void Close()
        {
            this.Dismiss();
            this.textView = null;
            this.methods = null;
        }

        public void Dismiss()
        {
            if (this.displayed && this.textView != null)
            {
                NativeMethods.ThrowOnFailure(this.textView.UpdateTipWindow(this.methodTipWindow, (uint)TipWindowFlags.UTW_DISMISS));
            }

            this.OnDismiss();
        }

        public void Dispose()
        {
            Close();
            if (this.methodTipWindow != null)
                NativeMethods.ThrowOnFailure(this.methodTipWindow.SetMethodData(null));
            this.methodTipWindow = null;
            this.provider = null;
        }

        //========================================================================
        //IVsMethodData
        public int GetOverloadCount()
        {
            if (this.textView == null || this.methods == null) return 0;
            return this.methods.GetCount();
        }

        public int GetCurMethod()
        {
            return this.currentMethod;
        }

        public int NextMethod()
        {
            if (GetOverloadCount() > 0)
                this.currentMethod = (this.currentMethod + 1) % GetOverloadCount();

            return this.currentMethod;
        }

        public int PrevMethod()
        {
            if (GetOverloadCount() > 0)
                this.currentMethod = (this.currentMethod + GetOverloadCount() - 1) % GetOverloadCount();

            return this.currentMethod;
        }

        public int GetParameterCount(int method)
        {
            if (this.methods == null) return 0;

            if (method < 0 || method >= GetOverloadCount()) return 0;

            return this.methods.GetParameterCount(method);
        }

        // which parameter is highlighted (bolded) in the tip
        public int GetCurrentParameter(int method)
        {
            if (this.methods == null) return 0;
            if (this.currentParameter < 0) return -1;

            var actualParamNames = this.methods.GetParameterNames();
            if (this.currentParameter >= actualParamNames.Length)
                return -1;  // can happen e.g. if after the last comma, and no param here
            if (actualParamNames[this.currentParameter] != null)
            {
                // current parameter is named
                int numParams = this.methods.GetParameterCount(method);
                for (int i = 0; i < numParams; ++i)
                {
                    string name, display, description;
                    this.methods.GetParameterInfo(method, i, out name, out display, out description);
                    if (actualParamNames[this.currentParameter] == name)
                        return i;
                }
                // did not find a param with same name, don't highlight any
                return -1;
            }
            // if there is a named parameter to the left, don't do positional logic
            bool anyNamedParametersToTheLeftOfTheCurrentOne = false;
            for (int i = 0; i < this.currentParameter; ++i)
                if (actualParamNames[i] != null)
                    anyNamedParametersToTheLeftOfTheCurrentOne = true;
            if (anyNamedParametersToTheLeftOfTheCurrentOne)
                return -1;
            // otherwise do normal positional logic
            if (this.currentParameter >= this.GetParameterCount(this.currentMethod))
                return this.GetParameterCount(this.currentMethod);
            else
                return this.currentParameter;
        }

        public void OnDismiss()
        {
            this.textView = null;
            this.methods = null;
            if (nativeStringsCacheForOverloads != null)
            {
                nativeStringsCacheForOverloads.Free();
                nativeStringsCacheForOverloads = null;
            }
            this.currentMethod = 0;
            this.currentParameter = 0;
            this.displayed = false;
        }

        public void UpdateView()
        {
            if (this.textView != null)
            {
                NativeMethods.ThrowOnFailure(this.textView.UpdateTipWindow(this.methodTipWindow, (uint)TipWindowFlags.UTW_CONTENTCHANGED | (uint)TipWindowFlags.UTW_CONTEXTCHANGED));
                this.displayed = true;
            }
        }

        // What region of the editor not to obscure when drawing the method tip
        public int GetContextStream(out int pos, out int length)
        {
            pos = 0;
            length = 0;
            int vspace;
            NativeMethods.ThrowOnFailure(this.textView.GetNearestPosition(this.context.iStartLine, this.context.iStartIndex, out pos, out vspace));
            int endpos;
            NativeMethods.ThrowOnFailure(this.textView.GetNearestPosition(this.context.iEndLine, this.context.iEndIndex, out endpos, out vspace));
            length = endpos - pos;
            Debug.Assert(length >= 0);
            return NativeMethods.S_OK;
        }


        public IntPtr GetMethodText(int method, MethodTextType type)
        {
            if (this.methods == null) return IntPtr.Zero;

            if (method < 0 || method >= GetOverloadCount()) return IntPtr.Zero;

            //a type
            if ((type == MethodTextType.MTT_TYPEPREFIX && this.methods.TypePrefixed) ||
                (type == MethodTextType.MTT_TYPEPOSTFIX && !this.methods.TypePrefixed))
            {
                var overload = nativeStringsCacheForOverloads.GetOverload(method);
                return overload.GetOrCreatePointerToNativeString(
                    MethodTextType.MTT_TYPEPREFIX,
                    () =>
                        {
                            string str = this.methods.GetReturnTypeText(method);
                            if (str == null) return null;
                            return this.methods.TypePrefix + str + this.methods.TypePostfix;
                        }
                    );
            }
            else
            {
                //other
                switch (type)
                {
                    case MethodTextType.MTT_OPENBRACKET:
                        return nativeStringsCacheForOverloads.GetOrCreateNativePointerForCommonString(MethodTextType.MTT_OPENBRACKET, () => methods.OpenBracket);

                    case MethodTextType.MTT_CLOSEBRACKET:
                        return nativeStringsCacheForOverloads.GetOrCreateNativePointerForCommonString(MethodTextType.MTT_CLOSEBRACKET, () => methods.CloseBracket);

                    case MethodTextType.MTT_DELIMITER:
                        return nativeStringsCacheForOverloads.GetOrCreateNativePointerForCommonString(MethodTextType.MTT_DELIMITER, () => methods.Delimiter);

                    case MethodTextType.MTT_NAME:
                        {
                            var overload = nativeStringsCacheForOverloads.GetOverload(method);
                            return overload.GetOrCreatePointerToNativeString(MethodTextType.MTT_NAME, () => methods.GetName(method));
                        }
                    case MethodTextType.MTT_DESCRIPTION:
                        {
                            var overload = nativeStringsCacheForOverloads.GetOverload(method);
                            return overload.GetOrCreatePointerToNativeString(MethodTextType.MTT_DESCRIPTION, () => methods.GetDescription(method));
                        }

                    case MethodTextType.MTT_TYPEPREFIX:
                    case MethodTextType.MTT_TYPEPOSTFIX:
                    default:
                        break;
                }
            }
            return IntPtr.Zero;
        }

        public IntPtr GetParameterText(int method, int parameter, ParameterTextType type)
        {
            if (this.methods == null) return IntPtr.Zero;

            if (method < 0 || method >= GetOverloadCount()) return IntPtr.Zero;

            if (parameter < 0 || parameter >= GetParameterCount(method)) return IntPtr.Zero;

            var overload = nativeStringsCacheForOverloads.GetOverload(method);
            return overload.GetOrCreateParameter(
                parameter,
                type,
                () =>
                    {
                        string name;
                        string description;
                        string display;

                        this.methods.GetParameterInfo(method, parameter, out name, out display, out description);

                        switch (type)
                        {
                            case ParameterTextType.PTT_NAME:
                                return name;

                            case ParameterTextType.PTT_DESCRIPTION:
                                return description;

                            case ParameterTextType.PTT_DECLARATION:
                                return display;

                            default:
                                throw new ArgumentOutOfRangeException("type");
                        }
                    }
                );
        }

        private class NativeStringsCacheForOverloads
        {
            private MethodData container;
            private const int NUMBER_OF_NATIVE_STRINGS = 3;
            private StringPointers nativeStringsCache = new StringPointers(NUMBER_OF_NATIVE_STRINGS);
            private NativeStringsCacheForOverload[] overloadsCache;
            public NativeStringsCacheForOverloads(int numberOfMethods, MethodData container)
            {
                overloadsCache = new NativeStringsCacheForOverload[numberOfMethods];
                this.container = container;
            }

            public NativeStringsCacheForOverload GetOverload(int methodIndex)
            {
                return overloadsCache[methodIndex] ?? (overloadsCache[methodIndex] = new NativeStringsCacheForOverload(container.GetParameterCount(methodIndex)));
            }

            public IntPtr GetOrCreateNativePointerForCommonString(MethodTextType methodTextType, Func<string> factory)
            {
                return nativeStringsCache.GetOrCreateStringPointer(IndexOfCommonString(methodTextType), factory);
            }

            public void Free()
            {
                nativeStringsCache.Free();

                foreach (var overload in overloadsCache)
                {
                    if (overload != null)
                    {
                        overload.Free();
                    }
                }
                Array.Clear(overloadsCache, 0, overloadsCache.Length);
            }

            private int IndexOfCommonString(MethodTextType methodTextType)
            {
                switch(methodTextType)
                {
                    case MethodTextType.MTT_OPENBRACKET: return 0;
                    case MethodTextType.MTT_CLOSEBRACKET: return 1;
                    case MethodTextType.MTT_DELIMITER: return 2;                    
                }
                throw new ArgumentOutOfRangeException("methodTextType");
            }
        }


        private class NativeStringsCacheForOverload
        {
            const int NUMBER_OF_POINTERS = 3;
            private StringPointers nativeStringsCache = new StringPointers(NUMBER_OF_POINTERS);
            private NativeStringsCacheForParameter[] parametersCache;

            public NativeStringsCacheForOverload(int parametersCount)
            {
                parametersCache = new NativeStringsCacheForParameter[parametersCount];
            }

            public void Free()
            {
                nativeStringsCache.Free();
                foreach (var p in parametersCache)
                {
                    if (p != null)
                    {
                        p.Free();
                    }
                }
                Array.Clear(parametersCache, 0, parametersCache.Length);
            }

            public IntPtr GetOrCreateParameter(int index, ParameterTextType parameterTextType, Func<string> factory)
            {
                var p = parametersCache[index] ?? (parametersCache[index] = new NativeStringsCacheForParameter());
                return p.GetOrCreate(parameterTextType, factory);
            }

            public IntPtr GetOrCreatePointerToNativeString(MethodTextType methodTextType, Func<string> factory)
            {
                return nativeStringsCache.GetOrCreateStringPointer(IndexOfMethodTextType(methodTextType), factory);
            }

            private static int IndexOfMethodTextType(MethodTextType methodTextType)
            {
                switch (methodTextType)
                {
                    case MethodTextType.MTT_TYPEPREFIX:
                    case MethodTextType.MTT_TYPEPOSTFIX:
                        return 0;
                    case MethodTextType.MTT_NAME: return 1;
                    case MethodTextType.MTT_DESCRIPTION: return 2;
                }
                throw new ArgumentOutOfRangeException("methodTextType");
            }
        }

        private class NativeStringsCacheForParameter
        {
            const int NUMBER_OF_POINTERS = 3;
            private StringPointers nativeStringCache = new StringPointers(NUMBER_OF_POINTERS);

            public void Free()
            {
                nativeStringCache.Free();
            }

            public IntPtr GetOrCreate(ParameterTextType parameterTextType, Func<string> factory)
            {
                return nativeStringCache.GetOrCreateStringPointer(IndexOfParameterTextType(parameterTextType), factory);
            }

            private static int IndexOfParameterTextType(ParameterTextType parameterTextType)
            {
                switch (parameterTextType)
                {
                    case ParameterTextType.PTT_DECLARATION:
                        return 0;
                    case ParameterTextType.PTT_DESCRIPTION:
                        return 1;
                    case ParameterTextType.PTT_NAME:
                        return 2;
                }
                throw new ArgumentOutOfRangeException("parameterTextType");
            }
        }

        private struct StringPointers
        {
            private IntPtr[] ptrs;

            public StringPointers(int size)
            {
                ptrs = new IntPtr[size];
            }
            public IntPtr GetOrCreateStringPointer(int index, Func<string> factory)
            {
                var ptr = ptrs[index];
                if (ptr == IntPtr.Zero)
                {
                    var text = factory();
                    if (text != null)
                    {
                        ptr = Marshal.StringToBSTR(text);
                        ptrs[index] = ptr;
                    }
                }
                return ptr;
            }

            public void Free()
            {
                for (var i = 0; i < ptrs.Length; ++i)
                {
                    if (ptrs[i] != IntPtr.Zero)
                    {
                        Marshal.FreeBSTR(ptrs[i]);
                    }
                    ptrs[i] = IntPtr.Zero;
                }
            }
        }   
    }
}
