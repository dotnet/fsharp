open Types

module CheckSelfConstrainedSRTP =

    let inline f_StaticProperty_IWSAM<'T when IStaticProperty<'T>>() =
        'T.StaticProperty

    type AverageOps<'T when 'T: (static member (+): 'T * 'T -> 'T)
                       and  'T: (static member DivideByInt : 'T*int -> 'T)
                       and  'T: (static member Zero : 'T)> = 'T

    let inline f_AverageOps<'T when AverageOps<'T>>(xs: 'T[]) =
        let mutable sum = 'T.Zero
        for x in xs do
           sum <- sum + x
        'T.DivideByInt(sum, xs.Length)
        
    printfn ""
