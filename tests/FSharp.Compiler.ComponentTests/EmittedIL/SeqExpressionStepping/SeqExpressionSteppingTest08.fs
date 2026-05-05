// #NoMono #NoMT #CodeGen #EmittedIL #Sequences
module SeqExpressionSteppingTest8
module SeqExpressionSteppingTest8 =

    let directValues () =
        seq { 1; 2; 3 }

    let doubledWithForArrow xs =
        seq { for x in xs -> x * 2 }

    let _ = directValues () |> Seq.length
    let _ = doubledWithForArrow [ 1; 2; 3 ] |> Seq.length
