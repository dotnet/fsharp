// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Internal.Utilities.Debug

open System
open System.IO
open System.Threading
open System.Diagnostics
open System.Runtime.InteropServices


module internal TraceInterop = 
    type MessageBeepType =
        | Default = -1
        | Ok = 0x00000000
        | Error = 0x00000010
        | Question = 0x00000020
        | Warning = 0x00000030
        | Information = 0x00000040

    [<DllImport("user32.dll", SetLastError=true)>]
    let MessageBeep(_mbt:MessageBeepType):bool=failwith "" 

[<AbstractClass>]
[<Sealed>]
type internal Trace private() =
    static let mutable log = ""
#if DEBUG_WITH_TIME_AND_THREAD_INFO
    static let TMinusZero = DateTime.Now
#endif    
    static let noopDisposable =
        {   new IDisposable with
                member this.Dispose() = ()
        }
    static let mutable out = Console.Out
    [<ThreadStatic>] [<DefaultValue>] static val mutable private indent:int    
    [<ThreadStatic>] [<DefaultValue>] static val mutable private threadName:string

    /// Set to the semicolon-delimited names of the logging classes to be reported.
    /// Use * to mean all.
    static member Log 
        with get() = log
        and set(value) = log<-value

    /// Output destination.
    static member Out 
        with get() = out
        and set(value:TextWriter) = out<-value

    /// True if the given logging class should be logged.
    static member ShouldLog(loggingClass) =
        let result = Trace.Log = "*" || Trace.Log.Contains(loggingClass^";") || Trace.Log.EndsWith(loggingClass,StringComparison.Ordinal)
        result

    /// Description of the current thread.     
    static member private CurrentThreadInfo() =
        if String.IsNullOrEmpty(Trace.threadName) then sprintf "(id=%d)" Thread.CurrentThread.ManagedThreadId
        else sprintf "(id=%d,name=%s)" Thread.CurrentThread.ManagedThreadId Trace.threadName
        
    /// Report the elapsed time since start.
    static member private ElapsedTime(start) = 
        let elapsed : TimeSpan = (DateTime.Now-start)
        sprintf "%A ms" elapsed.TotalMilliseconds
        
    /// Get a string with spaces for indention.
    static member private IndentSpaces() = new string(' ', Trace.indent)
        
    /// Log a message.
    static member private LogMessage(msg:string) =
        Trace.Out.Write(sprintf "%s%s" (Trace.IndentSpaces()) msg) 
        Trace.Out.Flush()
        if Trace.Out<>Console.Out then 
            // Always log to console.
            Console.Out.Write(sprintf "%s%s" (Trace.IndentSpaces()) msg) 
        
    /// Name the current thread.
    static member private NameCurrentThread(threadName) =
        match threadName with 
        | Some(threadName)->
            let current = Trace.threadName
            if String.IsNullOrEmpty(current) then Trace.threadName <- threadName
            else if not(current.Contains(threadName)) then Trace.threadName <- current^","^threadName
        | None -> ()

    /// Base implementation of the call function.
    static member private CallImpl(loggingClass,functionName,descriptionFunc,threadName:string option) : IDisposable = 
        #if DEBUG
        if Trace.ShouldLog(loggingClass) then 
            Trace.NameCurrentThread(threadName)
            
            let description = try descriptionFunc() with e->"No description because of exception"
            
#if DEBUG_WITH_TIME_AND_THREAD_INFO
            let threadInfo = Trace.CurrentThreadInfo()
            let indent = Trace.IndentSpaces()
            let start = DateTime.Now
            Trace.LogMessage(sprintf "Entering %s(%s) %s t-plus %fms %s\n"
                                functionName
                                loggingClass
                                threadInfo
                                (start-TMinusZero).TotalMilliseconds
                                description)
#else
            Trace.LogMessage(sprintf "Entering %s(%s) %s\n"
                                functionName
                                loggingClass
                                description)
#endif
            Trace.indent<-Trace.indent+1
    
            {new IDisposable with
                member d.Dispose() = 
                    Trace.indent<-Trace.indent-1
#if DEBUG_WITH_TIME_AND_THREAD_INFO
                    Trace.LogMessage(sprintf "Exitting %s %s %s\n" 
                                       functionName 
                                       threadInfo
                                       (Trace.ElapsedTime(start)))}
#else
                    Trace.LogMessage(sprintf "Exiting %s\n" 
                                       functionName)}
#endif
        else 
            noopDisposable : IDisposable  
        #else
            ignore(loggingClass,functionName,descriptionFunc,threadName)
            noopDisposable : IDisposable  
        #endif                                       
                
    /// Log a method as it's called.
    static member Call(loggingClass:string,functionName:string,descriptionFunc:unit->string) = Trace.CallImpl(loggingClass,functionName,descriptionFunc,None)
    /// Log a method as it's called. Expected always to be called on the same thread which will be named 'threadName'.
    static member CallByThreadNamed(loggingClass:string,functionName:string,threadName:string,descriptionFunc:unit->string) = Trace.CallImpl(loggingClass,functionName,descriptionFunc,Some(threadName))
    /// Log a message by logging class.
    static member PrintLine(loggingClass:string, messageFunc:unit->string) = 
    #if DEBUG
        if Trace.ShouldLog(loggingClass) then 
            let message = try messageFunc() with _-> "No message because of exception.\n"
            Trace.LogMessage(sprintf "%s%s" message System.Environment.NewLine)
    #else
        ignore(loggingClass,messageFunc)
    #endif            

    /// Log a message by logging class.
    static member Print(loggingClass:string, messageFunc:unit->string) = 
    #if DEBUG
        if Trace.ShouldLog(loggingClass) then 
            let message = try messageFunc() with _-> "No message because of exception.\n"
            Trace.LogMessage(message)
    #else
        ignore(loggingClass,messageFunc)
    #endif
            
    /// Make a beep when the given loggingClass is matched.
    static member private BeepHelper(loggingClass,beeptype) = 
    #if DEBUG
        if Trace.ShouldLog(loggingClass) then 
            TraceInterop.MessageBeep(beeptype) |> ignore
    #else
        ignore(loggingClass,beeptype)
    #endif                    
        
    /// Make the "OK" sound when the given loggingClass is matched.
    static member BeepOk(loggingClass:string) = Trace.BeepHelper(loggingClass,TraceInterop.MessageBeepType.Ok)
            
    /// Make the "Error" sound when the given loggingClass is matched. 
    static member BeepError(loggingClass:string) = Trace.BeepHelper(loggingClass,TraceInterop.MessageBeepType.Error)
        
    /// Make the default sound when the given loggingClass is matched. 
    static member Beep(loggingClass:string) = Trace.BeepHelper(loggingClass,TraceInterop.MessageBeepType.Default)
            
            
