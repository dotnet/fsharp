// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Generate the hash/compare functions we add to user-defined types by default.
module internal Microsoft.FSharp.Compiler.AugmentWithHashCompare 

open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler 

open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.TcGlobals

val CheckAugmentationAttribs : bool -> TcGlobals -> Import.ImportMap -> Tycon -> unit
val TyconIsCandidateForAugmentationWithCompare  : TcGlobals -> Tycon -> bool
val TyconIsCandidateForAugmentationWithEquals   : TcGlobals -> Tycon -> bool
val TyconIsCandidateForAugmentationWithHash     : TcGlobals -> Tycon -> bool

val MakeValsForCompareAugmentation                : TcGlobals -> TyconRef -> Val * Val
val MakeValsForCompareWithComparerAugmentation    : TcGlobals -> TyconRef -> Val
val MakeValsForEqualsAugmentation                 : TcGlobals -> TyconRef -> Val * Val
val MakeValsForEqualityWithComparerAugmentation   : TcGlobals -> TyconRef -> Val * Val * Val

val MakeBindingsForCompareAugmentation               : TcGlobals -> Tycon -> Binding list
val MakeBindingsForCompareWithComparerAugmentation   : TcGlobals -> Tycon -> Binding list
val MakeBindingsForEqualsAugmentation                : TcGlobals -> Tycon -> Binding list
val MakeBindingsForEqualityWithComparerAugmentation  : TcGlobals -> Tycon -> Binding list

/// This predicate can be used once type inference is complete, before then it is an approximation
/// that doesn't assert any new constraints
val TypeDefinitelyHasEquality : TcGlobals -> TType -> bool

