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

open ConstrainedCall

printfn $"{``'T.op_Addition`` 1 2}"                         // Prints 3.
printfn $"{``'T.(+)`` 1 2}"                                 // Prints 3.
printfn $"{``'T.op_CheckedAddition`` 1 2}"                  // Prints 3.
printfn $"""{``'T.Parse``<int> "3"}"""                      // Prints 3.

open InterfaceCall

printfn $"{``IAdditionOperators.op_Addition`` 1 2}"         // System.BadImageFormatException: Bad IL format.
printfn $"{``IAdditionOperators.(+)`` 1 2}"                 // System.BadImageFormatException: Bad IL format.
printfn $"{``IAdditionOperators.op_CheckedAddition`` 1 2}"  // Prints 3.
printfn $"""{``IParsable.Parse``<int> "3"}"""               // System.BadImageFormatException: Bad IL format.
