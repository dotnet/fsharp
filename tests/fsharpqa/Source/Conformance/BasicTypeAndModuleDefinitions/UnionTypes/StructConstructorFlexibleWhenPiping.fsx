// #Conformance #TypesAndModules #Unions 
// Struct DU constructor should be flexible when piping
//<Expects status=success></Expects>

[<Struct>]
type Foo = Items of seq<int>
[1;2;3] |> Items

exit 0