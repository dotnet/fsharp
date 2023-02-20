
module Meh

type X =
    member a : int = 10

/// Represents a line number when using zero-based line counting (used by Visual Studio)
#if CHECK_LINE0_TYPES

#else
type Y = int
#endif

type Z with
    static member P : int -> int
