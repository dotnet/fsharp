// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ConstraintSolver

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module SRTPNullnessConstraints =

    // NOTE: Tests for SRTP nullness constraint resolution with types imported from older assemblies
    // would require actual F#8/F#7 compiled assemblies to test AmbivalentToNull behavior.
    // 
    // The fix ensures that AmbivalentToNull types (imported from older assemblies) only satisfy 
    // 'T : null constraints if they would have satisfied nullness under legacy F# rules,
    // addressing issues #18390 and #18344.
    //
    // For proper testing, use F# Interactive with #r "nuget: FSharpPlus, 1.7.0" and invoke
    // IsAltLeftZero.Invoke None to test with packages compiled using older F# compilers.