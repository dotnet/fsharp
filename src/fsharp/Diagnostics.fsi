// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// This file contains simple types related to diagnsotics that are made public in the
// FSharp.Compiler.Service API but which are also used throughout the
// F# compiler.

namespace FSharp.Compiler.SourceCodeServices

type FSharpDiagnosticOptions =
    { WarnLevel: int
      GlobalWarnAsError: bool
      WarnOff: int list
      WarnOn: int list
      WarnAsError: int list
      WarnAsWarn: int list }

    static member Default: FSharpDiagnosticOptions
  
