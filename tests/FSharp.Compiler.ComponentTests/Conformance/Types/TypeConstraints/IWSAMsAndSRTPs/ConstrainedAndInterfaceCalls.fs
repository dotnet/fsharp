#nowarn "64"
open System
open System.Numerics

module ConstrainedCall =
    let ``'T.op_Addition``<'T & #IAdditionOperators<'T, 'T, 'T>>        x y    = 'T.op_Addition        (x, y)
    let ``'T.(+)``<'T & #IAdditionOperators<'T, 'T, 'T>>                x y    = 'T.(+)                (x, y)
    let ``'T.op_CheckedAddition``<'T & #IAdditionOperators<'T, 'T, 'T>> x y    = 'T.op_CheckedAddition (x, y)
    let ``'T.Parse``<'T & #IParsable<'T>>                               x      = 'T.Parse              (x, null)

module InterfaceCall =
    let ``IAdditionOperators.op_Addition``                              x y    = IAdditionOperators.op_Addition        (x, y)
    let ``IAdditionOperators.(+)``                                      x y    = IAdditionOperators.(+)                (x, y)
    let ``IAdditionOperators.op_CheckedAddition``                       x y    = IAdditionOperators.op_CheckedAddition (x, y)
    let ``IParsable.Parse``<'T & #IParsable<'T>>                        x : 'T = IParsable.Parse                       (x, null)

