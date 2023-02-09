namespace Foobar

val v : int =
    #if FOO
        #if MEH
        1
        #else
        2
        #endif
    #else
        3
    #endif