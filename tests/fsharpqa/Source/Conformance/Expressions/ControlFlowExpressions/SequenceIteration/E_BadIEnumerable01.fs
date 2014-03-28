// #Conformance #ControlFlow #Sequences #Regression #InterfacesAndImplementations
//<Expects status="error" span="(19,24-19,32)" id="FS1231">The type 'En<string>' is not a valid enumerator type , i\.e\. does not have a 'MoveNext\(\)' method returning a bool, and a 'Current' property$</Expects>
//<Expects status="error" span="(20,24-20,36)" id="FS1231">The type 'En<Struct>' is not a valid enumerator type , i\.e\. does not have a 'MoveNext\(\)' method returning a bool, and a 'Current' property$</Expects>
//<Expects status="error" span="(21,26-21,29)" id="FS1231">The type 'En<'a>' is not a valid enumerator type , i\.e\. does not have a 'MoveNext\(\)' method returning a bool, and a 'Current' property$</Expects>

[<Struct>] 
type Struct =
  val x:int
  val y:int
 
type En<'a>(mvNext:'a) =
  member x.Current = 1
  member x.MoveNext() = mvNext
 
type T<'a>(mvNext:'a) =
  member x.GetEnumerator() = En mvNext
 
let s = seq { for i in T true -> i }
let t = seq { for i in T "test" -> i }
let u = seq { for i in T (Struct()) -> i }
let v a = seq { for i in T a -> i }

exit 1