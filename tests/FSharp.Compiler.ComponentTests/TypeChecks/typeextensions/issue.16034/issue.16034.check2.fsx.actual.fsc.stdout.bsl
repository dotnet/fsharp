

Copyright (c) Microsoft Corporation. All Rights Reserved.

For help type #help;;

> [Loading /tests/FSharp.Compiler.ComponentTests/TypeChecks/typeextensions/issue.16034/issue.16034.fsx
 Loading /tests/FSharp.Compiler.ComponentTests/TypeChecks/typeextensions/issue.16034/issue.16034.check2.fsx]
module FSI_0001.Issue.16034
val mutable i: int
type T =
  new: unit -> T
  member indexed1: a1: obj -> int with get
  member indexed1: a1: obj -> int with set
module Extensions =
  val mutable j: int
  type T with
    member indexed1: aa1: obj -> int with get
  type T with
    member indexed1: aa1: obj -> int with set
val t: T

module FSI_0001.Issue.16034.check2

error FS0073: internal error: The input must be non-negative. (Parameter 'n') (ArgumentException)