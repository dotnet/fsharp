// Regression test: an SRTP constraint solved by an extension member on a STRUCT.
// Extension members compile to static methods that take the receiver BY VALUE, so the
// witness must not take the struct receiver's address. Before the fix the witness emitted
// `ldarga` (byref) into a by-value parameter, producing invalid IL that threw
// System.InvalidProgramException at runtime. Covers both an instance method and a property.
module StructExtensionMemberSRTP

[<Struct>] type Label = { Text: string }

module Ext =
    type Label with
        member x.Decorate p = p + x.Text + p
        member x.Doubled = x.Text + x.Text

open Ext

let inline decorate (x: ^T) p = (^T: (member Decorate: string -> string) (x, p))
let inline doubled (x: ^T) = (^T: (member Doubled: string) x)

let m = decorate { Text = "core" } "!"
if m <> "!core!" then failwithf "method: %s" m

let p = doubled { Text = "ab" }
if p <> "abab" then failwithf "property: %s" p
