// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

type State() =

    let shutdownEvent = new Event<_>()
    let exitEvent = new Event<_>()
    let cancelEvent = new Event<_>()

    [<CLIEvent>]
    member __.Shutdown = shutdownEvent.Publish

    [<CLIEvent>]
    member __.Exit = exitEvent.Publish

    [<CLIEvent>]
    member __.Cancel = cancelEvent.Publish

    member __.DoShutdown() = shutdownEvent.Trigger()

    member __.DoExit() = exitEvent.Trigger()

    member __.DoCancel() = cancelEvent.Trigger()

    member val Options = Options.Default() with get, set
