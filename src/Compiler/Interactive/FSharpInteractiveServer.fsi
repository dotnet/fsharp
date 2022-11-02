// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.Interactive

module CtrlBreakHandlers =

    [<AbstractClass>]
    type public CtrlBreakService =
        new: channelName: string -> CtrlBreakService

        abstract Interrupt: unit -> unit

        member Run: unit -> unit

    type public CtrlBreakClient =
        new: channelName: string -> CtrlBreakClient

        member Interrupt: unit -> unit

        member Start: unit -> unit
