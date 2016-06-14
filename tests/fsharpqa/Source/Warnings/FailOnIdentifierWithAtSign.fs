// #Warnings
//<Expects status="Error" span="(5,14)" id="FS1104">Identifiers containing .@. are reserved for use in F# code generation</Expects>

type Foo() =
  static let ``Bar@`` = "hello"
  static member BarAt = ``Bar@``
  static member val Bar = "bar"

do
  printfn "BarAt = %s" Foo.BarAt
  printfn "Bar = %s" Foo.Bar
    
exit 0