Lambda (i,
        IfThenElse (Call (None, op_Equality, [i, Value (1L)]), Value ("a"),
                    IfThenElse (Call (None, op_Equality, [i, Value (2L)]),
                                Value ("b"),
                                IfThenElse (Call (None, op_Equality,
                                                  [i, Value (3L)]), Value ("c"),
                                            Value ("other")))))
