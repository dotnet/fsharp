#nowarn "64"
open Types

module ``Use SRTP from IWSAM generic code`` =
    module ``Use SRTP operators from generic IWSAM code`` =
        let fAdd<'T when 'T :> IAdditionOperator<'T>>(x: 'T, y: 'T) =
            x + y

        let fSin<'T when ISinOperator<'T>>(x: 'T) =
            sin x

    module ``Use SRTP operators from generic IWSAM code not rigid`` =
        let fAdd(x: 'T when 'T :> IAdditionOperator<'T>, y: 'T) =
            x + y

        let fSin(x: 'T when ISinOperator<'T>) =
            sin x

    module ``Use SRTP operators from generic IWSAM code flex`` =
        let fAdd(x: #IAdditionOperator<'T>, y) =
            x + y

        let fSin(x: #ISinOperator<'T>) =
            sin x

    module ``Use SRTP operators from generic IWSAM code super flex`` =
        let fAdd(x: #IAdditionOperator<_>, y) =
            x + y

        let fSin(x: #ISinOperator<_>) =
            sin x

    printfn ""
