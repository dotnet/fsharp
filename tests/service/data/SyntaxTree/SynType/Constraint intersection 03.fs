module Module

type I =
    abstract h: #IDisposable & #seq<int> & #I -> unit