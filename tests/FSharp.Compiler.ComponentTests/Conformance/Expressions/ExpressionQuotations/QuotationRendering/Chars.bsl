Lambda (c,
        IfThenElse (Call (None, op_Equality, [c, Value ('a')]), Value (1),
                    IfThenElse (Call (None, op_Equality, [c, Value ('b')]),
                                Value (2), Value (0))))
