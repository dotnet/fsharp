// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

type State() =

    let shutdownEvent = new Event<_>()

    [<CLIEvent>]
    member __.Shutdown = shutdownEvent.Publish

    member __.DoShutdown() = shutdownEvent.Trigger()
