Lambda (i,
        IfThenElse (Call (None, op_Equality, [i, Value (1)]), Value ("a"),
                    IfThenElse (Call (None, op_Equality, [i, Value (5)]),
                                Value ("b"),
                                IfThenElse (Call (None, op_Equality,
                                                  [i, Value (10)]), Value ("c"),
                                            Value ("other")))))
