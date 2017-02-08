// #UnitGenericAbstractType 
//<Expects status="success"></Expects>

type Foo<'t> =
  abstract member Bar : 't -> int

type Bar() =
  interface Foo<unit> with
    member x.Bar _ = 1