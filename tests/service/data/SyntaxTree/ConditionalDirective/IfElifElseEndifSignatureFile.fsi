
namespace Foobar

val x : int =
    #if DEBUG
    1
    #elif RELEASE
    2
    #else
    3
    #endif
