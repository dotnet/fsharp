// Regression test for DEV11:5949 - "generic function-valued static property raises "error FS0073: internal error: Undefined or unsolved type variable: 'a""
//<Expects status="warning" span="(11,29-11,41)" id="FS1125">The instantiation of the generic type 'list1' is missing and can't be inferred from the arguments or return type of this member\. Consider providing a type instantiation when accessing this type, e\.g\. 'list1<_>'\.$</Expects>
//<Expects status="error" span="(15,10-15,25)" id="FS3068">The function or member 'toList' is used in a way that requires further type annotations at its definition to ensure consistency of inferred types\. The inferred signature is 'static member private list1\.toList: \('a list1 -> 'a list\)'\.$</Expects>
open System
open System.Collections

type 'a list1 = One of 'a | Many of 'a * 'a list1
  with
    static member private toList = function
      | One x -> [x]
      | Many(x, xs) -> x :: list1.toList xs

    interface IEnumerable with
      member this.GetEnumerator() =
        (list1<_>.toList this :> IEnumerable).GetEnumerator()

    interface Generic.IEnumerable<'a> with
      member this.GetEnumerator() =
        (list1<_>.toList this :> Generic.IEnumerable<'a>).GetEnumerator()
