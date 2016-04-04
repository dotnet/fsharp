// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Tests.LanguageService.ErrorList

open NUnit.Framework
open System
open System.IO
open System.Diagnostics
open Microsoft.FSharp.Build
open Microsoft.Build.BuildEngine
open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open UnitTests.TestLib.Utils
open UnitTests.TestLib.Utils.FilesystemHelpers
open Microsoft.VisualStudio.Shell

open Salsa.Salsa
open Salsa.VsMocks

type TextSpan = Microsoft.VisualStudio.TextManager.Interop.TextSpan
type DocumentTask = Microsoft.VisualStudio.FSharp.LanguageService.DocumentTask

[<TestFixture>]
type TaskReporter() = 
    static let err(line) : 'a = 
        printfn "err() called on line %s with %s" line System.Environment.StackTrace 
        failwith "not implemented"

    let mockOleComponentManager = 
        { new Microsoft.VisualStudio.OLE.Interop.IOleComponentManager with
            member x.FContinueIdle() = 1
            member x.FCreateSubComponentManager(piunkOuter, piunkServProv, riid, ppvObj) = err(__LINE__)
            member x.FGetActiveComponent(dwgac, ppic, pcrinfo, dwReserved) = err(__LINE__)
            member x.FGetParentComponentManager(ppicm) = err(__LINE__)
            member x.FInState(uStateID, pvoid) = err(__LINE__)
            member x.FOnComponentActivate(dwComponentID) = err(__LINE__)
            member x.FOnComponentExitState(dwComponentID, uStateID, uContext, cpicmExclude, rgpicmExclude) = err(__LINE__)
            member x.FPushMessageLoop(dwComponentID, uReason, pvLoopData) = err(__LINE__)
            member x.FRegisterComponent(piComponent, pcrinfo, pdwComponentID) = err(__LINE__)
            member x.FReserved1(dwReserved, message, wParam, lParam) = err(__LINE__)
            member x.FRevokeComponent(dwComponentID) = err(__LINE__)
            member x.FSetTrackingComponent(dwComponentID, fTrack) = err(__LINE__)
            member x.FUpdateComponentRegistration(dwComponentID, pcrinfo) = err(__LINE__)
            member x.OnComponentEnterState(dwComponentID, uStateID, uContext, cpicmExclude, rgpicmExclude, dwReserved) = err(__LINE__)
            member x.QueryService(guidService, iid, ppvObj) = err(__LINE__) 
            }


    (* Asserts ----------------------------------------------------------------------------- *)
    let AssertEqual expected actual =
        if expected<>actual then 
            let message = sprintf "Expected\n%A\nbut got\n%A" expected actual
            printfn "%s" message
            Assert.Fail(message)
            
    let AssertNotEqual expected actual =
        if expected = actual then
            let message = sprintf "Did not want\n%A\nbut got it anyway" expected
            printfn "%s" message
            Assert.Fail(message)
            
    let GetNewDocumentTask filePath subcategory span errorMessage taskPriority taskCategory taskErrorCategory =
        let dt = new Microsoft.VisualStudio.FSharp.LanguageService.DocumentTask(null, 
                        Salsa.VsMocks.Vs.MakeTextLines(), 
                        Microsoft.VisualStudio.TextManager.Interop.MARKERTYPE.MARKER_CODESENSE_ERROR,
                        span,
                        filePath,
                        subcategory)
        dt.Text <- errorMessage ;
        dt.Priority <- taskPriority ;
        dt.Category <- taskCategory ;
        dt.ErrorCategory <- taskErrorCategory ;
        dt
        
    let CreateTaskListProvider() =
        let tasks : Task array ref = ref [||]
        
        { new Microsoft.VisualStudio.FSharp.LanguageService.ITaskListProvider with
            member tl.Count() = (!tasks).Length
            member tl.GetTask i = (!tasks).[i]
            member tl.Add t = tasks := Array.append !tasks [| t |]
            member tl.SuspendRefresh() = ()
            member tl.ResumeRefresh() = ()
            member tl.Clear() = tasks := [||]
            member tl.Refresh() = ()
        }
        
    let CreateTaskReporter() =
        UIStuff.SetupSynchronizationContext()
        let taskReporter = new Microsoft.VisualStudio.FSharp.LanguageService.TaskReporter("unit test (taskreporter.unittests.fs)")
        taskReporter.TaskListProvider <- CreateTaskListProvider() ;
        taskReporter

// One File Tests
    // For the next two, add tasks to the task list more than once to ensure that
    // hashing is occuring correctly   
    [<Test>]
    member public this.``ErrorList.LanguageServiceErrorsProperlyCoalesced``() =  
        use taskReporter = CreateTaskReporter()
        let mutable span = new TextSpan(iStartLine = 1, iEndLine = 1, iStartIndex = 1, iEndIndex = 2)
        let documentTask = GetNewDocumentTask @"c:\foo.fs" "typecheck" span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        
        // add a language service error
        taskReporter.AddTask(documentTask) |> ignore
        
        // add the error to the language service again
        taskReporter.AddTask(documentTask) |> ignore
        
        taskReporter.OutputTaskList()
        
        taskReporter.DoIdle(mockOleComponentManager) |> ignore
        AssertEqual (taskReporter.TaskListProvider.Count()) 1
        ()
        
        
        
    [<Test>]
    member public this.``ErrorList.ProjectSystemErrorsProperlyCoalesced``() =  
        use taskReporter = CreateTaskReporter()
        let mutable span = new TextSpan(iStartLine = 1, iEndLine = 1, iStartIndex = 1, iEndIndex = 2)
        let documentTask = GetNewDocumentTask @"c:\foo.fs" "typecheck" span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        
        // add a project system error
        taskReporter.AddTask(documentTask) |> ignore
        
        // add the error to the project system again
        taskReporter.AddTask(documentTask) |> ignore
        
        taskReporter.OutputTaskList()
        
        taskReporter.DoIdle(mockOleComponentManager) |> ignore
        AssertEqual (taskReporter.TaskListProvider.Count()) 1
        
        ()
        
    /// Test for multiple identical errors being properly coalesced in the error list (bug 2151)
    [<Test>]
    member public this.``ErrorList.ErrorsProperlyCoalesced``() =  
        use taskReporter = CreateTaskReporter()
        let mutable span = new TextSpan(iStartLine = 1, iEndLine = 1, iStartIndex = 1, iEndIndex = 2)
        let documentTask = GetNewDocumentTask @"c:\foo.fs" "typecheck" span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        
        // add a language service error
        taskReporter.AddTask(documentTask) |> ignore
        
        // add a project system error
        taskReporter.AddTask(documentTask) |> ignore
        
        taskReporter.OutputTaskList()
        
        taskReporter.DoIdle(mockOleComponentManager) |> ignore
        AssertEqual (taskReporter.TaskListProvider.Count()) 1
        ()
    
    // modify the span, and check to see if we have two tasks now instead of one    
    [<Test>]
    member public this.``ErrorList.ProjectSystemErrorsProperlyCoalesced2``() =  
        use taskReporter = CreateTaskReporter()
        let mutable span = new TextSpan(iStartLine = 1, iEndLine = 1, iStartIndex = 1, iEndIndex = 2)
        let documentTask1 = GetNewDocumentTask @"c:\foo.fs" "typecheck" span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        
        span.iStartLine <- 2;
        span.iEndLine <- 2;
        
        let documentTask2 = GetNewDocumentTask @"c:\foo.fs" "typecheck" span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        
        // add the errors
        taskReporter.AddTask(documentTask1) |> ignore
        taskReporter.AddTask(documentTask2) |> ignore
        
        taskReporter.OutputTaskList()
        
        taskReporter.DoIdle(mockOleComponentManager) |> ignore
        AssertEqual (taskReporter.TaskListProvider.Count()) 2
        ()
        
    /// Ensure that text line markers are only created when a task is output to the the task list
    [<Test>]
    member public this.``ErrorList.TextLineMarkersCreatedOnce``() =  
        use taskReporter = CreateTaskReporter()
        let mutable span = new TextSpan(iStartLine = 1, iEndLine = 1, iStartIndex = 1, iEndIndex = 2)
        let documentTask = GetNewDocumentTask @"c:\foo.fs" "typecheck" span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        let documentTask2 = GetNewDocumentTask @"c:\foo.fs" "typecheck"  span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        
        // ensure that the tasks don't have a text line marker yet
        AssertEqual documentTask.TextLineMarker null
        AssertEqual documentTask2.TextLineMarker null
        
        // add a language service error
        taskReporter.AddTask(documentTask) |> ignore
        
        // add a project system error
        taskReporter.AddTask(documentTask2) |> ignore
        
        taskReporter.OutputTaskList()
        
        taskReporter.DoIdle(mockOleComponentManager) |> ignore
        AssertEqual (taskReporter.TaskListProvider.Count()) 1
        
        // ensure that the task now has a text line marker
        AssertNotEqual documentTask.TextLineMarker null
        
        // now, make sure that the rejected task does not have a text line marker
        AssertEqual documentTask2.TextLineMarker null
        
        ()                    
 
 // Two file tests
 
 
 
 
    // both files open
    // errors in each file, build - no duplicates  
    [<Test>]
    member public this.``ErrorList.TwoFilesBothOpen``() =  
        use taskReporter = CreateTaskReporter()
        let mutable span = new TextSpan(iStartLine = 1, iEndLine = 1, iStartIndex = 1, iEndIndex = 2)
        let documentTask1 = GetNewDocumentTask @"c:\foo.fs" "typecheck" span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        let documentTask2 = GetNewDocumentTask @"c:\bar.fs" "typecheck" span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        
        // add the errors
        taskReporter.AddTask(documentTask1) |> ignore
        taskReporter.AddTask(documentTask2) |> ignore
        
        taskReporter.OutputTaskList()
        
        // ensure that we now have two distinct tasks in the task list
        taskReporter.DoIdle(mockOleComponentManager) |> ignore
        AssertEqual (taskReporter.TaskListProvider.Count()) 2
        
        // are the tasks ordered correctly?
        let task1 = taskReporter.TaskListProvider.GetTask(0) :?> DocumentTask
        let task2 = taskReporter.TaskListProvider.GetTask(1) :?> DocumentTask
        AssertEqual documentTask1 task2  // reordered based on filename
        AssertEqual documentTask2 task1      
        ()           
 
     // file open, one file closed
     // - error in closed file
    [<Test>]
    member public this.``ErrorList.TwoFilesOneOpenErrorInOpen``() =  
        use taskReporter = CreateTaskReporter()
        let mutable span = new TextSpan(iStartLine = 1, iEndLine = 1, iStartIndex = 1, iEndIndex = 2)
        let documentTask1 = GetNewDocumentTask @"c:\foo.fs" "typecheck" span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        let documentTask2 = GetNewDocumentTask @"c:\bar.fs" "typecheck" span "bar error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        let documentTask3 = GetNewDocumentTask @"c:\foo.fs" "typecheck" span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error        
        
        // add the background task ("foo.fs" is opened)
        taskReporter.AddTask(documentTask1) |> ignore
        
        // now simulate a build - add task 2, and task 3 (which is the same as task 1)
        taskReporter.AddTask(documentTask2) |> ignore
        taskReporter.AddTask(documentTask3) |> ignore
        
        // get "all" tasks - if working correctly - the three tasks will have been coalesced into two (tasks 1 and 3 will be the same)
        taskReporter.OutputTaskList()
        
        taskReporter.DoIdle(mockOleComponentManager) |> ignore
        // ensure that we now have two distinct tasks in the task list
        AssertEqual (taskReporter.TaskListProvider.Count()) 2
        
        // are the tasks ordered correctly?
        let task1 = taskReporter.TaskListProvider.GetTask(0) :?> DocumentTask
        let task2 = taskReporter.TaskListProvider.GetTask(1) :?> DocumentTask
        AssertEqual documentTask3 task2  // reordered based on filename
        AssertEqual documentTask2 task1      
        ()           
 
    // all files open - build, then fix - no errors left
    [<Test>]
    member public this.``ErrorList.TwoFilesCorrectError``() =  
        use taskReporter = CreateTaskReporter()
        let mutable span = new TextSpan(iStartLine = 1, iEndLine = 1, iStartIndex = 1, iEndIndex = 2)
        let documentTask1 = GetNewDocumentTask @"c:\foo.fs" "typecheck" span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        let documentTask2 = GetNewDocumentTask @"c:\bar.fs" "typecheck" span "bar error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        let documentTask3 = GetNewDocumentTask @"c:\foo.fs" "typecheck" span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        let documentTask4 = GetNewDocumentTask @"c:\bar.fs" "typecheck" span "bar error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        
        // add the background tasks ("foo.fs" and "bar".fs are opened)
        taskReporter.AddTask(documentTask1) |> ignore
        taskReporter.AddTask(documentTask2) |> ignore
        
        // now simulate a build - add task 3, and task 4 (which are the same as tasks 1 and 2)
        taskReporter.AddTask(documentTask3) |> ignore
        taskReporter.AddTask(documentTask4) |> ignore
        
        taskReporter.OutputTaskList()
        
        taskReporter.DoIdle(mockOleComponentManager) |> ignore
        // after the "build", we shouldn't have any build tasks, since they already belong to the LS
        let backgroundTasks = taskReporter.GetBackgroundTasks();
        let buildTasks = taskReporter.GetBuildTasks();
             
        // ensure that we now have two distinct tasks in the task list
        AssertEqual (taskReporter.TaskListProvider.Count()) 2
        AssertEqual (backgroundTasks.GetLength(0)) 2
        AssertEqual (buildTasks.GetLength(0)) 0
        
        // are the tasks ordered correctly?
        let task1 = taskReporter.TaskListProvider.GetTask(0) :?> DocumentTask
        let task2 = taskReporter.TaskListProvider.GetTask(1) :?> DocumentTask
        AssertEqual documentTask1 task2  // reordered based on filename
        AssertEqual documentTask2 task1
        
        // simulate a fix by clearing out the c:\foo.fs errors
        taskReporter.ClearBackgroundTasksForFile(@"c:\foo.fs")
        taskReporter.OutputTaskList()
        taskReporter.DoIdle(mockOleComponentManager) |> ignore
        let task1 = taskReporter.TaskListProvider.GetTask(0) :?> DocumentTask
        AssertEqual (taskReporter.TaskListProvider.Count()) 1
        AssertEqual task1 documentTask2       
        ()  
        
        
    // Make sure a 'typecheck' is treated as a background task
    [<Test>]
    member public this.``ErrorList.BackgroundTaskIsClassified``() =  
        use taskReporter = CreateTaskReporter()
        let mutable span = new TextSpan(iStartLine = 1, iEndLine = 1, iStartIndex = 1, iEndIndex = 2)
        let dt = GetNewDocumentTask @"c:\foo.fs" "typecheck" span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        
        taskReporter.AddTask(dt) |> ignore
        
        taskReporter.OutputTaskList()
        
        taskReporter.DoIdle(mockOleComponentManager) |> ignore

        let backgroundTasks = taskReporter.GetBackgroundTasks();
        let buildTasks = taskReporter.GetBuildTasks();
             
        // There should be one error in the list.
        AssertEqual (taskReporter.TaskListProvider.Count()) 1
        
        // It should be a background task.
        AssertEqual (backgroundTasks.GetLength(0)) 1
        
        // There should be no build tasks.
        AssertEqual (buildTasks.GetLength(0)) 0
        
        ()        
        
        
    // Make sure a 'ilxgen' is treated as a build task
    [<Test>]
    member public this.``ErrorList.BuildTaskIsClassified``() =  
        use taskReporter = CreateTaskReporter()
        let mutable span = new TextSpan(iStartLine = 1, iEndLine = 1, iStartIndex = 1, iEndIndex = 2)
        let dt = GetNewDocumentTask @"c:\foo.fs" "ilxgen" span "foo error!" TaskPriority.High TaskCategory.BuildCompile TaskErrorCategory.Error
        
        taskReporter.AddTask(dt) |> ignore
        
        taskReporter.OutputTaskList()
        
        taskReporter.DoIdle(mockOleComponentManager) |> ignore

        let backgroundTasks = taskReporter.GetBackgroundTasks();
        let buildTasks = taskReporter.GetBuildTasks();
             
        // There should be one error in the list.
        AssertEqual (taskReporter.TaskListProvider.Count()) 1
        
        // It should be a build task.
        AssertEqual (buildTasks.GetLength(0)) 1
        
        // There should be no background tasks.
        AssertEqual (backgroundTasks.GetLength(0)) 0
        
        ()                
