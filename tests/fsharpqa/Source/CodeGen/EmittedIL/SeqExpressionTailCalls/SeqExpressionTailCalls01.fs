// #NoMono #NoMT #CodeGen #EmittedIL #Sequences #TailCalls #NETFX20Only #NETFX40Only 
module SeqExpressionTailCalls01 // Regression test for FSHARP1.0:4135
let rec rwalk x = seq { yield x; yield! rwalk (x+1) }

