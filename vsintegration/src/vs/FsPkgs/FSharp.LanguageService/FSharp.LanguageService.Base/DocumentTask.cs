// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;
using VsShell = Microsoft.VisualStudio.Shell.VsShellUtilities;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.FSharp.LanguageService {
    internal static class UIThread {
        static SynchronizationContext ctxt;
        static bool isUnitTestingMode = false;
#if DEBUG
        static StackTrace captureStackTrace; // stack trace when ctxt was captured
        static Thread uithread; 
#endif
        public static SynchronizationContext TheSynchronizationContext {
            get {
                Debug.Assert(ctxt != null, "Tried to get TheSynchronizationContext before it was captured");
                return ctxt;
            }
        }

        public static void InitUnitTestingMode() {
            Debug.Assert(ctxt == null, "Context has already been captured; too late to InitUnitTestingMode");
            isUnitTestingMode = true;
        }

        [Conditional("DEBUG")]
        public static void MustBeCalledFromUIThread() {
#if DEBUG
            Debug.Assert(uithread == System.Threading.Thread.CurrentThread || isUnitTestingMode, "This must be called from the GUI thread");
#endif
        }
        public static void CaptureSynchronizationContext() {
            if (isUnitTestingMode) return;
#if DEBUG
            uithread = System.Threading.Thread.CurrentThread;
#endif

            if (ctxt == null) {
#if DEBUG
                 // This is a handy place to do this, since the product and all interesting unit tests
                 // must go through this code path.
                 AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(delegate(object sender, UnhandledExceptionEventArgs args)
                 {
                     if (args.IsTerminating)
                     {
                         string s = String.Format("An unhandled exception is about to terminate the process.  Exception info:\n{0}", args.ExceptionObject.ToString());
                         Debug.Assert(false, s);
                     }
                 });
                 captureStackTrace = new StackTrace(true);
#endif
                ctxt = new WindowsFormsSynchronizationContext();
            } else {
#if DEBUG
                 // Make sure we are always capturing the same thread.
                 Debug.Assert(uithread == Thread.CurrentThread);
#endif
            }
        }
        private static readonly Queue<Action> ourUIQueue = new Queue<Action>();
        private static bool ourIsReentrancy;

        // Runs the action on UI thread. Prevents from reentracy.
        public static void Run(Action action) {
            if (isUnitTestingMode) {
                action();
                return;
            }
            Debug.Assert(ctxt != null, "The SynchronizationContext must be captured before calling this method");
#if DEBUG
            StackTrace stackTrace = new StackTrace(true);
#endif
            ctxt.Post(delegate(object ignore) 
            {
                UIThread.MustBeCalledFromUIThread();
                ourUIQueue.Enqueue(action);
                if (ourIsReentrancy) return;
                ourIsReentrancy = true;
                try
                {
                    while (ourUIQueue.Count > 0)
                    {
                        try
                        {
                            var a = ourUIQueue.Dequeue();
                            a();
                        }
#if DEBUG
                        catch (Exception e)
                        {
                            // swallow, random exceptions should not kill process
                            Debug.Assert(false, string.Format("UIThread.Run caught and swallowed exception: {0}\n\noriginally invoked from stack:\n{1}", e.ToString(), stackTrace.ToString()));
                        }
#else
                        catch (Exception) {
                            // swallow, random exceptions should not kill process
                        }
#endif

                    }
                }
                finally
                {
                    ourIsReentrancy = false;
                }
            }, null);

        }

        /// <summary>
        /// RunSync puts orignal exception stacktrace to Exception.Data by this key if action throws on UI thread
        /// </summary>
        /// WrappedStacktraceKey is a string to keep exception serializable.
        public static readonly string WrappedStacktraceKey = "$$Microsoft.VisualStudio.Package.UIThread.WrappedStacktraceKey$$";

        public static void RunSync(Action a)
        {
            if (isUnitTestingMode)
            {
                a();
                return;
            }
            Exception exn = null;
            Debug.Assert(ctxt != null, "The SynchronizationContext must be captured before calling this method");
            // Send on UI thread will execute immediately.
            ctxt.Send(ignore =>
                {
                    try
                    {
                        UIThread.MustBeCalledFromUIThread();
                        a();
                    }
                    catch (Exception e)
                    {
                        exn = e;
                    }
                }, null
            );
            if (exn != null)
            {
                // throw exception on calling thread, preserve stacktrace
                if (!exn.Data.Contains(WrappedStacktraceKey)) exn.Data[WrappedStacktraceKey] = exn.StackTrace;
                throw exn;
            }
        }

        /// <summary>
        /// Performs a callback on the UI thread and blocks until it is done, using the VS mechanism for
        /// ensuring non-reentrancy.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal static T DoOnUIThread<T>(Func<T> callback)
        {
            return Microsoft.VisualStudio.Shell.ThreadHelper.Generic.Invoke<T>(callback);
        }

        /// <summary>
        /// Performs a callback on the UI thread and blocks until it is done, using the VS mechanism for
        /// ensuring non-reentrancy.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal static void DoOnUIThread(Action callback)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.Generic.Invoke(callback);
        }
    }

    // DocumentTask is associated with an IVsTextLineMarker in a specified document and 
    // implements Navigate() to jump to that marker.
    /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask"]/*' />
    [CLSCompliant(false)]
    [System.Runtime.InteropServices.ComVisible(true)]
#if DEBUG
    [System.Diagnostics.DebuggerDisplay("DocumentTask: {Text}")]
#endif
    public class DocumentTask : ErrorTask, IVsTextMarkerClient, IDisposable, IComparable<DocumentTask>, IVsProvideUserContext {
        // Since all taskitems support this field we define it generically. Can use put_Text to set it.
        IServiceProvider site;
        string fileName;
        string subcategory;
        IVsTextLineMarker textLineMarker;
        TextSpan span;
        bool markerValid;
        IVsTextLines buffer;
        MARKERTYPE markerType;

        /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask.DocumentTask"]/*' />
        internal DocumentTask(IServiceProvider site, IVsTextLines buffer, MARKERTYPE markerType, TextSpan span, string fileName, string subcategory) {
            this.site = site;
            this.fileName = fileName;
            this.subcategory = subcategory;
            this.span = span;
            this.Document = this.fileName;
            this.Column = span.iStartIndex;
            this.Line = span.iStartLine;
            this.buffer = buffer;
            this.markerType = markerType;
        }

        /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask.Finalize"]/*' />
        ~DocumentTask() {
            Dispose();
        }

        /// <summary>
        /// Logic to create the TextLineMarker associated with this task
        /// To prevent dangling markers, we'll defer the creation to when the task is added to the task list
        /// </summary>
        public void CreateTextLineMarker() {
            var targetSpan = this.span;
            if (this.textLineMarker != null) {
                TextSpan[] tp = new TextSpan[1];
                this.textLineMarker.GetCurrentSpan(tp);
                targetSpan = tp[0];
            }
            if (this.buffer != null) {
                if (markerType != MARKERTYPE.MARKER_OTHER_ERROR) {
                    // create marker so task item navigation works even after file is edited.
                    IVsTextLineMarker[] marker = new IVsTextLineMarker[1];
                    // bugbug: the following comment in the method CEnumMarkers::Initialize() of
                    // ~\env\msenv\textmgr\markers.cpp means that tool tips on empty spans
                    // don't work:
                    //      "VS7 #23719/#15312 [CFlaat]: exclude adjacent markers when the target span is non-empty"
                    // So I wonder if we should debug assert on that or try and modify the span
                    // in some way to make it non-empty...
                    NativeMethods.ThrowOnFailure(buffer.CreateLineMarker((int)markerType, targetSpan.iStartLine, targetSpan.iStartIndex, targetSpan.iEndLine, targetSpan.iEndIndex, this, marker));
                    this.textLineMarker = marker[0];
                    this.markerValid = true;
                }
            }
        }

        /// <summary>
        /// Whether or not this is a build-only error or warning. These can come from phases in compilation after type checking or even from
        /// other stages in the MSBuild .targets files.
        /// </summary>
        public bool IsBuildTask {
            get {
                // The line below would be the only dependency on FSharp.Compiler.dll in this assembly.  To break that dependency, we duplicate the logic.
                //return !Microsoft.FSharp.Compiler.SourceCodeServices.CompilerEnvironment.IsCheckerSupportedSubcategory(subcategory);
                switch (subcategory)
                {
                    case "compile": return false;
                    case "parameter": return false;
                    case "parse": return false;
                    case "typecheck": return false;
                    default: return true;
                }
            }
        }

        /// <include file='doc\TaskProvider.uex' path='docs/doc[@for="DocumentTask.IsMarkerValid"]/*' />
        public bool IsMarkerValid {
            get {
                return this.markerValid;
            }
        }

        /// <include file='doc\TaskProvider.uex' path='docs/doc[@for="DocumentTask.Site"]/*' />
        public IServiceProvider Site {
            get { return this.site; }
        }

        /// <include file='doc\TaskProvider.uex' path='docs/doc[@for="DocumentTask.Dispose"]/*' />
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <include file='doc\TaskProvider.uex' path='docs/doc[@for="DocumentTask.Dispose1"]/*' />
        protected virtual void Dispose(bool disposing) {
            if (this.textLineMarker != null) {
                NativeMethods.ThrowOnFailure(textLineMarker.Invalidate());
                this.markerValid = false;
                this.textLineMarker.UnadviseClient();
            }

            this.textLineMarker = null;
            this.site = null;
        }

        /// <summary>
        ///  Overridden hash code method - note that we only take a subset of the object's properties into account
        /// </summary>
        /// <returns>An integer hash code</returns>
        public override int GetHashCode() {
            int hashCode = Document.GetHashCode() ^
                // IMPORTANT:
                // - We must use span here, rather than Span, since Span defers to the textLineMarker, which may differ from the original span and mess up the hash.
                // - We intentionally only compare the start line and column not the whole span. This is to tolerate some changes to pretty-up the span in the language service.
                // - We do not consider subcategory. It is safe because no error with the same text and span can come from different build phases. Its necessary because
                //   there is an error recovery mode in the language service that creates a default phase if there isn't one.
                     this.span.iStartLine.GetHashCode() ^
                     this.span.iStartIndex.GetHashCode() ^
                            // this.subcategory.GetHashCode() ^
                     this.Text.GetHashCode() ^
                     this.Category.GetHashCode() ^
                     this.Priority.GetHashCode() ^ 
                     this.ErrorCategory.GetHashCode();
            //Debug.WriteLine("{0}: {1},{2},{3},{4},{5},{6} : {7}", this.Text, this.Document, this.span.iStartLine, this.span.iStartIndex, this.Category, this.Priority, this.ErrorCategory, hashCode);
            return hashCode;
        }

        /// <summary>
        /// Overridden equality method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if equal</returns>
        public override bool Equals(object obj) {
            if (null == obj || !(obj is DocumentTask))
                return false;

            DocumentTask dt = (DocumentTask)obj;

            return this.Document.Equals(dt.Document) &&
                    this.span.iStartLine == dt.span.iStartLine &&
                    this.span.iStartIndex == dt.span.iStartIndex &&
                    // this.subcategory == dt.subcategory && // See GetHashCode for an explanation of why this isn't part of Equals
                    this.Text.Equals(dt.Text) &&
                    this.Category == dt.Category &&
                    this.Priority == dt.Priority &&
                    this.ErrorCategory == dt.ErrorCategory;
        }

        public int CompareTo(DocumentTask other)
        {
            // sort by filename, then location
            int fileComp = this.fileName.CompareTo(other.fileName);
            if (fileComp != 0) return fileComp;
            if (this.span.iStartLine < other.span.iStartLine) return -1;
            if ((this.span.iStartLine == other.span.iStartLine) && (this.span.iStartIndex < other.span.iStartIndex)) return -1;
            if ((this.span.iStartLine == other.span.iStartLine) && (this.span.iStartIndex == other.span.iStartIndex)) return 0;
            return 1;
        }

        /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask.OnNavigate"]/*' />
        // It is important that this function not throw an exception.
        protected override void OnNavigate(EventArgs e) {
            try {
                TextSpan span = this.span;
                if (textLineMarker != null) {
                    TextSpan[] spanArray = new TextSpan[1];
                    if (NativeMethods.Failed(textLineMarker.GetCurrentSpan(spanArray))) {
                        Debug.Assert(false, "Unexpected error getting current span in OnNavigate");
                        return;
                    }
                    span = spanArray[0];
                }

                IVsUIHierarchy hierarchy;
                uint itemID;
                IVsWindowFrame docFrame;
                IVsTextView textView;
                try {
                    VsShell.OpenDocument(this.site, this.fileName, NativeMethods.LOGVIEWID_Code, out hierarchy, out itemID, out docFrame, out textView);
                } catch (System.ArgumentException) {
                    // No assert here because this can legitimately happen when quickly doing F8 during a refresh of language service errors (see 4846)
                    return;
                }
                catch (System.IO.FileNotFoundException)
                {
                    // No assert here because this can legitimately happen, e.g. with type provider errors (which are attributed to "FSC" file), or other cases
                    return;
                }
                if (NativeMethods.Failed(docFrame.Show()))
                {
                    // No assert here because this can legitimately happen when quickly doing F8 during a refresh of language service errors (see 4846)
                    return;
                }
                if (textView != null) {
                    // In the off-chance these methods fail, we 'recover' by continuing. It is more helpful to show the user the file if possible than not.
                    textView.SetCaretPos(span.iStartLine, span.iStartIndex);
                    TextSpanHelper.MakePositive(ref span);
                    textView.SetSelection(span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex);
                    textView.EnsureSpanVisible(span);
                }
                base.OnNavigate(e);
            } catch(Exception exn) {
                System.Diagnostics.Debug.Assert(false, "Unexpected exception thrown from DocumentTask.OnNavigate" + exn.ToString() + exn.StackTrace);
            }
        }

        /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask.OnRemoved"]/*' />
        protected override void OnRemoved(EventArgs e) {
            if (this.textLineMarker != null) {
                NativeMethods.ThrowOnFailure(textLineMarker.Invalidate());
                this.markerValid = false;
            }
            base.OnRemoved(e);
        }

        /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask.Span"]/*' />
        public TextSpan Span {
            get {
                if (textLineMarker != null) {
                    TextSpan[] aSpan = new TextSpan[1];
                    NativeMethods.ThrowOnFailure(textLineMarker.GetCurrentSpan(aSpan));
                    return aSpan[0];
                }
                return this.span;
            }
        }

        /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask.TextLineMarker"]/*' />
        public IVsTextLineMarker TextLineMarker {
            get { return this.textLineMarker; }
        }

        #region IVsTextMarkerClient methods

        /*---------------------------------------------------------
            IVsTextMarkerClient
        -----------------------------------------------------------*/
        /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask.MarkerInvalidated"]/*' />
        public virtual void MarkerInvalidated() {
            this.markerValid = false;
        }

        /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask.OnBufferSave"]/*' />
        public virtual void OnBufferSave(string fileName) {
        }

        /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask.OnBeforeBufferClose"]/*' />
        public virtual void OnBeforeBufferClose() {
        }

        /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask.OnAfterSpanReload"]/*' />
        public virtual void OnAfterSpanReload() {
        }

        /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask.OnAfterMarkerChange"]/*' />
        public virtual int OnAfterMarkerChange(IVsTextMarker marker) {
            return NativeMethods.S_OK;
        }

        /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask.GetTipText"]/*' />
        public virtual int GetTipText(IVsTextMarker marker, string[] tipText) {
            if (this.Text != null && this.Text.Length > 0) tipText[0] = this.Text;
            return NativeMethods.S_OK;
        }

        /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask.GetMarkerCommandInfo"]/*' />
        public virtual int GetMarkerCommandInfo(IVsTextMarker marker, int item, string[] text, uint[] commandFlags) {
            // Returning S_OK results in error message appearing in editor's
            // context menu when you right click over the error message.
            if (commandFlags != null && commandFlags.Length > 0)
                commandFlags[0] = 0;
            if (text != null && text.Length > 0)
                text[0] = null;
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\DocumentTask.uex' path='docs/doc[@for="DocumentTask.ExecMarkerCommand"]/*' />
        public virtual int ExecMarkerCommand(IVsTextMarker marker, int item) {
            return NativeMethods.S_OK;
        }
        #endregion

        int IVsProvideUserContext.GetUserContext(out IVsUserContext ppctx)
        {
            // We don't currently plumb the error code (e.g. 'FS0123') around.  Need to explicitly say NOT_IMPL so that VS fallback
            // logic will kick in and go to 'Error List' help topic when you press F1 on an F# task.
            ppctx = null;
            return NativeMethods.E_NOTIMPL;
        }
    };

    internal interface ITaskListProvider {
        // Get the current number of tasks in the task list
        int Count();

        // Clear the tasks from the task list
        void Clear();

        // Get the task at the specified index
        Task GetTask(int i);

        // Add a task to the end of the task list
        void Add(Task t);

        // Suspend task list refresh
        void SuspendRefresh();

        // Resume task list refresh
        void ResumeRefresh();

        // Refresh the task list appearance
        void Refresh();
    };

    internal class TaskListProvider : ITaskListProvider {
        private TaskProvider taskProvider;

        public TaskListProvider(TaskProvider taskProvider) {
            this.taskProvider = taskProvider;
        }

        public int Count() {
            return this.taskProvider.Tasks.Count;
        }

        public void Clear() {
            this.taskProvider.Tasks.Clear();
        }

        public Task GetTask(int i) {
            return this.taskProvider.Tasks[i];
        }

        public void Add(Task t) {
            this.taskProvider.Tasks.Add(t);
        }

        public void SuspendRefresh() {
            this.taskProvider.SuspendRefresh();
        }

        public void ResumeRefresh() {
            this.taskProvider.ResumeRefresh();
        }

        public void Refresh() {
            this.taskProvider.Refresh();
        }
    };

    internal static class TaskReporterIdleRegistration {
        public static uint nextToken = 0;
        public static Dictionary<uint, TaskReporter> taskReporters = new Dictionary<uint, TaskReporter>();
        public static uint Register(TaskReporter tr) {
            UIThread.MustBeCalledFromUIThread();
            uint n = nextToken++;
            taskReporters.Add(n, tr);
            return n;
        }
        public static void Unregister(uint n) {
            UIThread.MustBeCalledFromUIThread();
            taskReporters.Remove(n);
        }
        // The language service calls this periodically when the UI thread is idle
        public static int DoIdle(IOleComponentManager mgr) {
            UIThread.MustBeCalledFromUIThread();
            int result = 0;
            foreach (TaskReporter tr in taskReporters.Values) {
                result = tr.DoIdle(mgr);
                if (result != 0) {
                    break;
                }
            }
            return result;
        }
    }

    // Collects and manages tasks, controls feeding of tasks to the task window
    internal sealed class TaskReporter : IDisposable {
        // This class is invoked from multiple threads (e.g. background LS thread, 
        // UI thread, MSBuild Logger thread), so we need to synchronize
        // to deal with races in its mutable instance variables (lest one thread
        // foreach over a LinkedList while another Add()s to it). 
        // This class also calls (indirectly) a
        // number of VS methods which rendezvous with the UI thread, and we risk
        // deadlocking the UI thread (e.g. if LS background thread calls a method, 
        // grabs a lock, and before calling rendezvous method, UI thread calls in 
        // and blocks waiting for the lock, deadlock).
        // So we manually marshall all work to the UI thread, using a lock 
        // and a queue.  This won't deadlock because all background threads that 
        // grab the lock will only add work to a queue and then relinquish the
        // lock (without ever rendezvous-ing with UI thread).
        // The owner must arrange for DoIdle() to be called periodically
        // on the UI thread to actually do work.  Dispose() must be called on the UI 
        // thread, it will empty the queue and dispose.

        private object queueLock;  // protects the queue
        private Queue<Action> work;

        // the members below are protected by only being used on the UI thread
        private ITaskListProvider taskListProvider;
        private LinkedList<TaskInfo> backgroundTasks;
        private LinkedList<TaskInfo> buildTasks;
        private bool isDisposed;

        private uint maxErrors;  // unprotected
        private readonly string debugDescription; // readonly
        private readonly uint token; // readonly

#if DEBUG
        public static int AliveCount = 0;
#endif
        public TaskReporter(string debugDescription) {
#if DEBUG
            Interlocked.Increment(ref AliveCount);
#endif
            this.taskListProvider = null;
            this.debugDescription = debugDescription;

            this.backgroundTasks = new LinkedList<TaskInfo>();
            this.buildTasks = new LinkedList<TaskInfo>();

            this.maxErrors = 200;
            this.isDisposed = false;

            this.queueLock = new object();
            this.work = new Queue<Action>();
            this.token = TaskReporterIdleRegistration.Register(this);
        }

        // properties
        public ITaskListProvider TaskListProvider {
            // these methods do not need synchronization, as only call sites
            // of setter are right after thing was just newed up (only one TaskReporter reference exists)
            get { return taskListProvider; }
            set { taskListProvider = value; }
        }

        public uint MaxErrors {
            // these do not need synchronization based on use (Source.cs only consumer of these methods)
            get { return maxErrors; }
            set { maxErrors = value; }
        }

        public void Dispose() {
            UIThread.MustBeCalledFromUIThread();
            lock (queueLock) {
                this.Dispose(true);
#if DEBUG
                Interlocked.Decrement(ref AliveCount);
#endif
                GC.SuppressFinalize(this);
            }
        }

        public void Dispose(bool dispose) {
            UIThread.MustBeCalledFromUIThread();
            lock (queueLock) {
                if (!this.isDisposed) {
                    if (dispose) {
                        TaskReporterIdleRegistration.Unregister(this.token);
                        FinishProcessingQueue();
                        foreach (TaskInfo ti in this.backgroundTasks) {
                            ti.Dispose();
                        }

                        foreach (TaskInfo ti in this.buildTasks) {
                            ti.Dispose();
                        }

                        this.ClearAllTasks();

                        if (null != this.taskListProvider) {
                            this.taskListProvider.Refresh();
                            this.taskListProvider.Clear();
                        }
                    }
                    this.buildTasks = null;
                    this.backgroundTasks = null;
                    this.taskListProvider = null;
                    this.isDisposed = true;
                }
            }
        }

        private void FinishProcessingQueue() {
            UIThread.MustBeCalledFromUIThread();
            lock (queueLock) {
                while (work.Count != 0) {
                    Action workItem = work.Dequeue();
                    workItem();
                }
            }
        }

        public int DoIdle(IOleComponentManager mgr) {
            UIThread.MustBeCalledFromUIThread();
            Debug.Assert(!this.isDisposed, "tried to do idle work on a disposed TaskReporter");
            lock (queueLock) {
                // process up to MAX items at a time
                int MAX = 50;  // How to pick a value?  I tried a couple values with a project with many errors, and this value seems to work well.  This value is also happy for unit tests.
                while (work.Count != 0 && mgr.FContinueIdle() != 0) {
                    int i = 0;
                    while (work.Count != 0 && i < MAX) {
                        Action workItem = work.Dequeue();
                        workItem();
                        ++i;
                    }
                }
                if (work.Count != 0) {
                    return 1;
                } else {
                    return 0;
                }
            }
        }

        ~TaskReporter() {
            Debug.Assert(false, string.Format("leaked a TaskReporter: {0}", this.debugDescription));
            Dispose(false);
        }

        private void ThrowIfDisposed() {
            if (this.isDisposed) {
#if DEBUG
                throw new ObjectDisposedException(string.Format("{0}: {1}", this.GetType().Name, this.debugDescription));
#else
                throw new ObjectDisposedException(this.GetType().Name);
#endif
            }
        }

        private TaskInfo GetTaskInfo(string taskPath, bool isBackgroundTask) {
            LinkedList<TaskInfo> taskList = isBackgroundTask ? backgroundTasks : buildTasks;

            foreach (TaskInfo ti in taskList) {
                // find the task info for that file
                if (0 == String.Compare(taskPath, ti.TaskPath, true, System.Globalization.CultureInfo.InvariantCulture)) {
                    return ti;
                }
            }

            // if we couldn't find one, create it, and add it to the task list
            TaskInfo nti = new TaskInfo(taskPath);
            taskList.AddLast(nti);
            return nti;
        }

        private TaskInfo GetBuildTaskInfo(string taskPath) {
            return GetTaskInfo(taskPath, false);
        }

        private TaskInfo GetBackgroundTaskInfo(string taskPath) {
            return GetTaskInfo(taskPath, true);
        }

        public void AddTask(DocumentTask dt) {
            lock (queueLock) {
                work.Enqueue(delegate() {
                    bool wasAdded = false;
                    bool isBuildTask = dt.IsBuildTask;
                    try {
                        ThrowIfDisposed();
                        if (isBuildTask) {
                            TaskInfo buildTI = GetBuildTaskInfo(dt.Document);
                            if (!buildTI.Contains(dt)) {
                                buildTI.Add(dt);
                                wasAdded = true;
                            }
                        } else {
                            TaskInfo backgroundTI = GetBackgroundTaskInfo(dt.Document);
                            if (!backgroundTI.Contains(dt)) {
                                backgroundTI.Add(dt);
                                wasAdded = true;
                            }
                        }

                    } finally {
                        if (!wasAdded) dt.Dispose();
                    }
                });
            }
        }

        public DocumentTask[] GetBuildTasks() {
            // This method is called only from unit tests or from
            // other synchronized TaskReporter methods, and thus
            // does not require synchronization
            LinkedList<DocumentTask> allBuildTasks = new LinkedList<DocumentTask>();
            // TODO: Should be able to concatenate the lists here
            foreach (TaskInfo ti in buildTasks) {
                LinkedList<DocumentTask> bts = ti.TaskList;

                foreach (DocumentTask dt in bts) {
                    allBuildTasks.AddLast(dt);
                }
            }

            // TODO: Maybe we should just return linked lists
            DocumentTask[] dta = new DocumentTask[allBuildTasks.Count];
            allBuildTasks.CopyTo(dta, 0);

            return dta;
        }

        public DocumentTask[] GetBackgroundTasks() {
            // This method is called only from unit tests or from
            // other synchronized TaskReporter methods, and thus
            // does not require synchronization
            LinkedList<DocumentTask> allBackgroundTasks = new LinkedList<DocumentTask>();

            foreach (TaskInfo ti in backgroundTasks) {
                LinkedList<DocumentTask> bts = ti.TaskList;

                // TODO: Again, should really be able to concatenate here
                foreach (DocumentTask dt in bts) {
                    allBackgroundTasks.AddLast(dt);
                }
            }

            DocumentTask[] dta = new DocumentTask[allBackgroundTasks.Count];
            allBackgroundTasks.CopyTo(dta, 0);

            return dta;
        }

        private DocumentTask[] GetAllTasks() {
            HashSet<DocumentTask> addedTasks = new HashSet<DocumentTask>();
            DocumentTask[] allBackgroundTasks = this.GetBackgroundTasks();
            DocumentTask[] allBuildTasks = this.GetBuildTasks();

            DocumentTask[] allTasks = new DocumentTask[allBackgroundTasks.GetLength(0) + allBuildTasks.GetLength(0)];
            allBackgroundTasks.CopyTo(allTasks, 0);
            allBuildTasks.CopyTo(allTasks, allBackgroundTasks.GetLength(0));


            return allTasks;
        }


        private void ClearBuildTasks() {
            ThrowIfDisposed();
            foreach (TaskInfo ti in buildTasks)
                ti.Clear();
            buildTasks.Clear();
        }

        private void ClearBackgroundTasks() {
            foreach (TaskInfo ti in backgroundTasks)
                ti.Clear();
            backgroundTasks.Clear();
        }

        public void ClearAllTasks() {
            lock (queueLock) {
                work.Enqueue(delegate() {
                    ClearBuildTasks();
                    ClearBackgroundTasks();
                });
            }
        }

        public void ClearBackgroundTasksForFile(string filePath) {
            lock (queueLock) {
                work.Enqueue(delegate() {
                    ThrowIfDisposed();
                    foreach (TaskInfo ti in backgroundTasks) {
                        if (0 == String.Compare(filePath, ti.TaskPath, true, System.Globalization.CultureInfo.InvariantCulture)) {
                            backgroundTasks.Remove(ti);
                            ti.Dispose();
                            return;
                        }
                    }
                });
            }
        }

        // Output all tasks to the task window
        public void OutputTaskList() {
            if (null == taskListProvider)
                return;

            lock (queueLock) {
                work.Enqueue(delegate() {
                    ThrowIfDisposed();
                    taskListProvider.Clear();

                    DocumentTask[] allTasks = GetAllTasks();
                    Array.Sort(allTasks);

                    taskListProvider.SuspendRefresh();

                    try {
                        foreach (DocumentTask dt in allTasks) {
                            dt.CreateTextLineMarker();
                            taskListProvider.Add(dt);
                        }
                    } catch {
                    } finally {
                        taskListProvider.ResumeRefresh();

                        // mark the task provider as dirty
                        taskListProvider.Refresh();
                    }
                });
            }
        }
    }

    internal class TaskInfo : IDisposable {
        private string taskPath;
        private HashSet<DocumentTask> taskSet;
        private HashSet<DocumentTask> deadSet;
        private LinkedList<DocumentTask> taskList;
        private LinkedList<DocumentTask> cachedList;
        private bool isDisposed;

        public TaskInfo(string taskPath) {
            this.taskPath = taskPath;
            this.taskSet = new HashSet<DocumentTask>();
            this.taskList = new LinkedList<DocumentTask>(); // contains both live and dead tasks
            this.deadSet = new HashSet<DocumentTask>();
            this.cachedList = null; // active list containing only live tasks
            this.isDisposed = false;
        }

        private void FreeTaskList() {
            foreach (DocumentTask dt in this.taskList) {
                dt.Dispose();
            }
            this.taskList.Clear();
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool dispose) {
            if (!dispose)
                return;

            if (!this.isDisposed) {
                this.taskSet.Clear();
                this.deadSet.Clear();

                this.FreeTaskList();

                if (null != this.cachedList)
                    this.cachedList.Clear();

                this.taskPath = null;

                this.isDisposed = true;
            }
        }

        public string TaskPath {
            get { return this.taskPath; }
        }

        public HashSet<DocumentTask> TaskSet {
            get { return this.taskSet; }
        }

        public LinkedList<DocumentTask> TaskList {
            get {
                if (null == this.cachedList) {
                    LinkedList<DocumentTask> al = new LinkedList<DocumentTask>(); // TODO: Can cache this list

                    // TODO: Set operations are actually expensive in this case, 
                    // since they rely on native interop when the span info is dug out of the tasks...
                    foreach (DocumentTask dt in this.taskList)
                        if (!deadSet.Contains(dt))
                            al.AddLast(dt);

                    this.cachedList = al;
                    return al;
                } else
                    return this.cachedList;
            }
        }

        public void Add(DocumentTask dt) {
            if (!this.taskSet.Contains(dt)) {
                this.taskSet.Add(dt);
            }
            if (this.deadSet.Contains(dt)) {
                this.deadSet.Remove(dt);
            } else
                this.taskList.AddLast(dt);

            this.cachedList = null;
        }

        public bool Contains(DocumentTask dt) {
            return this.taskSet.Contains(dt) && !this.deadSet.Contains(dt);
        }

        public void Remove(DocumentTask dt) {
            if (this.taskSet.Contains(dt)) {
                this.taskSet.Remove(dt);
                this.deadSet.Add(dt);
                this.cachedList = null;
            }
        }

        public void Clear() {
            this.taskSet.Clear();
            this.deadSet.Clear();
            this.FreeTaskList();
            this.cachedList = null;
        }

    };

}
