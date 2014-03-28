// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Outcome

open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library

val success : 'T -> ResultOrException<'T>
val raze : exn -> ResultOrException<'T>
val ( ||?> ) : ResultOrException<'T> -> ('T -> ResultOrException<'U>) -> ResultOrException<'U>
val ( |?> ) : ResultOrException<'T> -> ('T -> 'U) -> ResultOrException<'U>
val ForceRaise : ResultOrException<'T> -> 'T
val otherwise : (unit -> ResultOrException<'T>) -> ResultOrException<'T> -> ResultOrException<'T>
