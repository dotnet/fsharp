module MeasureConstr
type MU<[<Measure>] 'ua> =
  class
  end
type MU2<[<Measure>] 'ua,[<Measure>] 'ub> =
  class
  end
type MT<'a> =
  class
  end
type MT2<'a,'b> =
  class
  end
type T =
  class
    static member bar : a:'d * l:#MT2<'b,'c> -> unit
    static member foo : l:#MT<'c> -> unit
    static member star : l:#MT<'b> -> unit
  end
type U =
  class
    static member bar : l:#MU2<'ub,'uc> -> unit
    static member bar2 : l:#MU2<'ub,'ud> -> unit
    static member bar3 : l:#MU2<'ub,'ub> * r:#MU2<'uc,'uc> -> unit
    static member foo : l:#MU<'uc> -> unit
    static member star : l:#MU<'ub> -> unit
  end