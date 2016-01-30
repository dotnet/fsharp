// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Internal.Utilities.Debug
    module internal TraceInterop =
        type MessageBeepType =
            |  Default  =  -1
            |  Ok  =  0
            |  Error  =  16
            |  Question  =  32
            |  Warning  =  48
            |  Information  =  64
        val MessageBeep : MessageBeepType -> bool
    [<AbstractClass; Sealed>]
    type internal Trace =
        static member Beep : loggingClass:string -> unit
        static member BeepError : loggingClass:string -> unit
        static member BeepOk : loggingClass:string -> unit
        static member Call : loggingClass:string * functionName:string * descriptionFunc:(unit->string) -> System.IDisposable
        static member CallByThreadNamed : loggingClass:string * functionName:string * threadName:string * descriptionFunc:(unit->string) -> System.IDisposable
        static member Print : loggingClass:string * messageFunc:(unit->string) -> unit
        static member PrintLine : loggingClass:string * messageFunc:(unit->string) -> unit
        static member ShouldLog : loggingClass:string -> bool
        static member Log : string with get, set
        static member Out : System.IO.TextWriter with get, set

