// #Regression #NoMono #NoMT #CodeGen #EmittedIL #Sequences #TailCalls   
// Regression test for FSHARP1.0:4135
// Same as SeqExpressionTailCalls01.fs, but with MUTUALLY RECURSIVE PAIR OF SEQUENCES
module SeqExpressionTailCalls02
let rec rwalk1 x = seq { yield x; yield! rwalk2 (x+1) }
    and rwalk2 x = seq { yield x; yield! rwalk1 (x+1) }
