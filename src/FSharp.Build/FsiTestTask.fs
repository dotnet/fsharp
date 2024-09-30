// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.Tests

open System.IO
open Microsoft.Build.Framework

/// MsBuild tool task for capturing stdout from test scripts.
type FsiTestTask() =
    inherit FSharp.Build.Fsi()

    // Just to keep a reasonable amount of test scipt output.
    let outputWriter = new StringWriter()

    override _.LogEventsFromTextOutput(line, msgImportance) =
        outputWriter.WriteLine(line)
        base.LogEventsFromTextOutput(line, msgImportance)

    [<Output>]
    member _.TextOutput = outputWriter.ToString()
