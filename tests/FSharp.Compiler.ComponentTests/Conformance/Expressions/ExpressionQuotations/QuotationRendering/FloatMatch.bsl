Lambda (f,
        IfThenElse (Call (None, op_Equality, [f, Value (1.0)]), Value ("a"),
                    IfThenElse (Call (None, op_Equality, [f, Value (2.0)]),
                                Value ("b"), Value ("other"))))
