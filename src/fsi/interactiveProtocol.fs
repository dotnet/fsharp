// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The types exchanged between F# Interactive and the editor hosting it.
///
/// This file is compiled into fsi and linked into the host, so that the two ends of the protocol
/// cannot drift apart. Roslyn achieves the same by having both sides reference one assembly; fsi
/// exposes no public surface to reference, so the source is shared instead.
///
/// The members are named as they appear on the wire, and the records are `CLIMutable` so that the
/// JSON-RPC formatter can construct them.
namespace FSharp.Compiler.Interactive.Protocol

/// Method names. Both ends use these rather than repeating string literals.
module Methods =
    let [<Literal>] Initialize = "fsi/initialize"
    let [<Literal>] Execute = "fsi/execute"
    let [<Literal>] ExecuteFile = "fsi/executeFile"
    let [<Literal>] SetPaths = "fsi/setPaths"
    let [<Literal>] Interrupt = "fsi/interrupt"
    let [<Literal>] Shutdown = "fsi/shutdown"

[<CLIMutable>]
type InitializeRequest =
    {
        /// The process that owns this session. F# Interactive watches it and exits when it goes, so
        /// that a crashed editor does not leave an orphan behind.
        clientProcessId: int
    }

[<CLIMutable>]
type InitializeResult =
    {
        /// The process actually evaluating code, which is what a debugger attaches to.
        ///
        /// On .NET this is not the process the host launched: `dotnet fsi` starts a second process,
        /// and it is the inner one that matters.
        processId: int

        frameworkDescription: string
        processArchitecture: string
        fsiVersion: string
        workingDirectory: string
        supportsInterrupt: bool
    }

[<CLIMutable>]
type ExecuteRequest =
    {
        code: string

        /// Where the text came from, when the host is executing a selection from a file. Together
        /// with `startLine` this makes diagnostics point at the user's own source rather than at a
        /// position within the submission.
        sourcePath: string

        startLine: System.Nullable<int>
    }

[<CLIMutable>]
type ExecuteFileRequest = { path: string }

[<CLIMutable>]
type SetPathsRequest =
    {
        includePaths: string[]
        workingDirectory: string
    }

/// One diagnostic. Lines are one-based and columns zero-based, as they are throughout the compiler.
[<CLIMutable>]
type DiagnosticInfo =
    {
        severity: string
        message: string
        errorNumber: int
        subcategory: string
        fileName: string
        startLine: int
        startColumn: int
        endLine: int
        endColumn: int
    }

/// An exception that escaped an interaction. Null when the interaction merely failed to compile,
/// because the diagnostics already describe that.
[<CLIMutable>]
type ExceptionInfo =
    {
        ``type``: string
        message: string
        stackTrace: string
    }

[<CLIMutable>]
type ExecutionResult =
    {
        /// The interaction was accepted and ran to completion: no error diagnostic, no escaping
        /// exception, not interrupted. Warnings do not affect it.
        success: bool

        cancelled: bool
        diagnostics: DiagnosticInfo[]
        ``exception``: ExceptionInfo

        /// Reported after every interaction so that the host can keep its own view of the session
        /// in step with one that changed directory.
        workingDirectory: string
    }

[<CLIMutable>]
type InterruptResult = { interrupted: bool }
