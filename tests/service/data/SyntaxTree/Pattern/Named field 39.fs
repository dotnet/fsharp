module Module

match 1 with
| SynExprRecordField(fieldName = SynLongIdent(id = id :: _), _) -> 2
