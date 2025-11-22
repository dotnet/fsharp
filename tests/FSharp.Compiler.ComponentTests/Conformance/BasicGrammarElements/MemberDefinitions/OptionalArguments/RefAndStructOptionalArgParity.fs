module M

type T =
    static member MRef (?x : int) = ()
    static member MStruct ([<Struct>] ?x : int) = ()

T.MRef 3
T.MRef ()
T.MRef (x=3)
T.MRef (?x=None)
T.MRef (?x=Some 3)

T.MStruct 3
T.MStruct ()
T.MStruct (x=3)
T.MStruct (?x=ValueNone)
T.MStruct (?x=ValueSome 3)
