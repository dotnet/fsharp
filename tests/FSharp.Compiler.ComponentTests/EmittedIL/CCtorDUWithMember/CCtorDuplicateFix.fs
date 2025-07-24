// Test case to reproduce duplicate .cctor issue from #18767
module CCtorDuplicateFix

// This specific pattern causes duplicate .cctor methods:
// Generic DU with nullary case and static member val
type U<'T> =
    | A
    static member val X = 3

// Additional test case to ensure normal cases still work
type V =
    | B
    static member val Y = 4