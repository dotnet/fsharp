namespace Foobar

val v : int =
    #if DEBUG
    30
    #else
    42
    #endif