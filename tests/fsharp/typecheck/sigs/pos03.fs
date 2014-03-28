// Bug 5554: constraints containing units-of-measure variables

module MeasureConstr

type MU<[<Measure>] 'ua> = class end
type MU2<[<Measure>] 'ua, [<Measure>] 'ub> = class end
type MT<'a> = class end
type MT2<'a,'b> = class end

type T =  
    static member foo (l: #MT<'c>) = ()

    static member star (l:'a when 'a :> MT<'b>) = ()

    static member bar (a : 'd, l:'a when 'a :> MT2<'b,'c>) = ()

type U =
  static member foo (l: #MU<'uc>) = ()

  static member star (l:'ua when 'ua :> MU<'ub>) = ()

  static member bar (l:'ua when 'ua :> MU2<'ub,'uc>) = ()
  static member bar2 (l:'ua when 'ua :> MU2<'uc*'ub,'ud>) = ()
  static member bar3 (l:'ua when 'ua :> MU2<'ub*'uc,'ub*'uc>, r:'ud when 'ud :> MU2<'uc,'uc>) = ()
