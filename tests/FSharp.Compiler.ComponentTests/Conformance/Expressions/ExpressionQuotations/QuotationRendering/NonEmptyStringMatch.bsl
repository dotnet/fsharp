Lambda (x,
        IfThenElse (Call (None, op_Equality, [x, Value ("foo")]), Value (1),
                    IfThenElse (Call (None, op_Equality, [x, Value ("bar")]),
                                Value (2), Value (0))))
