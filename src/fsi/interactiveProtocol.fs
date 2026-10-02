// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// <summary>The types exchanged between F# Interactive and the editor hosting it.</summary>
/// <remarks>
/// <para>
/// This file is compiled into fsi and linked into the host, so that the two ends of the protocol
/// cannot drift apart. Roslyn achieves the same by having both sides reference one assembly; fsi
/// exposes no public surface to reference, so the source is shared instead.
/// </para>
/// <para>
/// The members are named as they appear on the wire, and the records are <c>CLIMutable</c> so that
/// the JSON-RPC formatter can construct them.
/// </para>
/// <para>
/// Public in the shared source so the host can use the same wire types. The server registers its
/// handlers explicitly; these DTOs are not exposed by reflection as part of the server target.
/// </para>
/// </remarks>
namespace FSharp.Compiler.Interactive.Protocol

/// Method names. Both ends use these rather than repeating string literals.
module Methods =
    [<Literal>]
    let Initialize = "fsi/initialize"

    [<Literal>]
    let Execute = "fsi/execute"

    [<Literal>]
    let ExecuteFile = "fsi/executeFile"

    [<Literal>]
    let SetPaths = "fsi/setPaths"

    [<Literal>]
    let Interrupt = "fsi/interrupt"

    [<Literal>]
    let Shutdown = "fsi/shutdown"

/// How a host asks for this protocol, at the point where there is no protocol yet to ask over.
module CommandLine =
    [<Literal>]
    let ServerOption = "--fsi-server-jsonrpc:"

[<CLIMutable>]
type InitializeResult =
    {
        /// <summary>
        /// The process actually evaluating code, which is what a debugger attaches to.
        /// </summary>
        /// <remarks>
        /// On .NET this is not the process the host launched: <c>dotnet fsi</c> starts a second
        /// process, and it is the inner one that matters.
        /// </remarks>
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

        /// <summary>
        /// Where the text came from, when the host is executing a selection from a file. Together
        /// with <c>startLine</c> this makes diagnostics point at the user's own source rather than
        /// at a position within the submission.
        /// </summary>
        sourcePath: string | null

        startLine: System.Nullable<int>
    }

[<CLIMutable>]
type ExecuteFileRequest = { path: string }

[<CLIMutable>]
type SetPathsRequest =
    {
        includePaths: string[] | null
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
type ValueInfo =
    {
        name: string
        typeName: string
        value: string
    }

[<CLIMutable>]
type ExecutionResult =
    {
        /// The interaction was accepted and ran to completion: no error diagnostic, no escaping
        /// exception, not interrupted. Warnings do not affect it.
        success: bool

        cancelled: bool
        diagnostics: DiagnosticInfo[] | null
        ``exception``: ExceptionInfo | null
        values: ValueInfo[]

        /// Reported after every interaction so that the host can keep its own view of the session
        /// in step with one that changed directory.
        workingDirectory: string
    }

[<CLIMutable>]
type InterruptResult = { interrupted: bool }
