// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler

module internal SR =
    val GetString: string -> string

module internal DiagnosticMessage =
    type ResourceString<'T> =
        new: string * Printf.StringFormat<'T> -> ResourceString<'T>
        member Format: 'T

    val DeclareResourceString: string * Printf.StringFormat<'T> -> ResourceString<'T>
