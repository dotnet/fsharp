// Chars
Lambda (x,
        IfThenElse (Call (None, op_Equality, [x, Value ('a')]), Value (1),
                    IfThenElse (Call (None, op_Equality, [x, Value ('b')]),
                                Value (2), Value (0))))
