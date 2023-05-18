module SimplePats

type X(i: int) = class end
type Y(a,b) = class end
type Z([<Foo>] bar, [<Foo>] v: V) = class end
type Unit() = class end
