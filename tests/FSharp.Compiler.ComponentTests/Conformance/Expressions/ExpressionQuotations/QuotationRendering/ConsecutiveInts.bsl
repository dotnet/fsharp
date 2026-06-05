Lambda (i,
        IfThenElse (Call (None, op_Equality, [i, Value (1)]), Value ("a"),
                    IfThenElse (Call (None, op_Equality, [i, Value (2)]),
                                Value ("b"),
                                IfThenElse (Call (None, op_Equality,
                                                  [i, Value (3)]), Value ("c"),
                                            Value ("z")))))
