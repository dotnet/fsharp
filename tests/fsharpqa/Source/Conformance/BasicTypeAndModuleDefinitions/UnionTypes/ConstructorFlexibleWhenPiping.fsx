// #Conformance #TypesAndModules #Unions 
// DU constructor should be acceptable
//<Expects status=success></Expects>

type Foo = Items of seq<int>
[1;2;3] |> Items

exit 0