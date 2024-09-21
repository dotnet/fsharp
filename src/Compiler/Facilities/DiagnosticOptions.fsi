// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// This file contains simple types related to diagnostics that are made public in the
// FSharp.Compiler.Service API but which are also used throughout the
// F# compiler.

namespace FSharp.Compiler.Diagnostics

open FSharp.Compiler.Text

/// The range between #nowarn and #warnon, or #warnon and #nowarn, for a warning number.
/// Or between the directive and eof, for the "Open" cases.
[<RequireQualifiedAccess>]
type WarnScope =
    | Off of range
    | On of range
    | OpenOff of range
    | OpenOn of range

/// The collected WarnScope objects (collected during lexing)
type WarnScopeMap = WarnScopeMap of Map<int64, WarnScope list>

/// Information about the mapping implied by the #line directives
type LineMap = LineMap of Map<int, int>
    with static member Empty : LineMap

[<RequireQualifiedAccess>]
type FSharpDiagnosticSeverity =
    | Hidden
    | Info
    | Warning
    | Error

type FSharpDiagnosticOptions =
    { WarnLevel: int
      GlobalWarnAsError: bool
      WarnOff: int list
      WarnOn: int list
      WarnAsError: int list
      WarnAsWarn: int list
      mutable FSharp9CompatibleNowarn: bool
      mutable LineMap: LineMap
      mutable WarnScopes: WarnScopeMap }

    static member Default: FSharpDiagnosticOptions

    member CheckXmlDocs: bool
