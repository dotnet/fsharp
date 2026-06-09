// ConsecutiveInts
Lambda (x,
        IfThenElse (Call (None, op_Equality, [x, Value (1)]), Value ("a"),
                    IfThenElse (Call (None, op_Equality, [x, Value (2)]),
                                Value ("b"),
                                IfThenElse (Call (None, op_Equality,
                                                  [x, Value (3)]), Value ("c"),
                                            Value ("z")))))
