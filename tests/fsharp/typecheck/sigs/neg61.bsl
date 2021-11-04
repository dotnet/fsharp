
neg61.fs(10,37,10,42): typecheck error FS3125: Invalid join relation in 'groupJoin'. Expected 'expr <op> expr', where <op> is =, =?, ?= or ?=?.

neg61.fs(14,13,14,22): typecheck error FS3097: Incorrect syntax for 'groupJoin'. Usage: groupJoin var in collection on (outerKey = innerKey) into group. Note that parentheses are required after 'on'.

neg61.fs(18,13,18,22): typecheck error FS3097: Incorrect syntax for 'groupJoin'. Usage: groupJoin var in collection on (outerKey = innerKey) into group. Note that parentheses are required after 'on'.

neg61.fs(22,13,22,16): typecheck error FS3145: This is not a known query operator. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type.

neg61.fs(22,13,22,16): typecheck error FS0039: The value or constructor 'zip' is not defined.

neg61.fs(26,13,26,19): typecheck error FS3099: 'select' is used with an incorrect number of arguments. This is a custom operation in this query or computation expression. Expected 1 argument(s), but given 0.

neg61.fs(30,13,30,16): typecheck error FS3145: This is not a known query operator. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type.

neg61.fs(30,13,30,16): typecheck error FS0039: The value or constructor 'zip' is not defined.

neg61.fs(34,13,34,22): typecheck error FS3096: 'groupJoin' must be followed by a variable name. Usage: groupJoin var in collection on (outerKey = innerKey) into group. Note that parentheses are required after 'on'.

neg61.fs(34,13,34,22): typecheck error FS3167: 'groupJoin' must be followed by 'in'. Usage: groupJoin var in collection on (outerKey = innerKey) into group. Note that parentheses are required after 'on'.

neg61.fs(34,13,34,22): typecheck error FS3098: 'groupJoin' must come after a 'for' selection clause and be followed by the rest of the query. Syntax: ... groupJoin var in collection on (outerKey = innerKey) into group. Note that parentheses are required after 'on' ...

neg61.fs(38,13,38,17): typecheck error FS3096: 'join' must be followed by a variable name. Usage: join var in collection on (outerKey = innerKey). Note that parentheses are required after 'on'.

neg61.fs(38,13,38,17): typecheck error FS3167: 'join' must be followed by 'in'. Usage: join var in collection on (outerKey = innerKey). Note that parentheses are required after 'on'.

neg61.fs(38,13,38,17): typecheck error FS3098: 'join' must come after a 'for' selection clause and be followed by the rest of the query. Syntax: ... join var in collection on (outerKey = innerKey). Note that parentheses are required after 'on' ...

neg61.fs(42,13,42,15): typecheck error FS3145: This is not a known query operator. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type.

neg61.fs(42,16,42,22): typecheck error FS3095: 'select' is not used correctly. This is a custom operation in this query or computation expression.

neg61.fs(47,13,47,15): typecheck error FS3145: This is not a known query operator. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type.

neg61.fs(47,16,47,20): typecheck error FS3095: 'join' is not used correctly. Usage: join var in collection on (outerKey = innerKey). Note that parentheses are required after 'on'. This is a custom operation in this query or computation expression.

neg61.fs(52,13,52,15): typecheck error FS3145: This is not a known query operator. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type.

neg61.fs(52,16,52,25): typecheck error FS3095: 'groupJoin' is not used correctly. Usage: groupJoin var in collection on (outerKey = innerKey) into group. Note that parentheses are required after 'on'. This is a custom operation in this query or computation expression.

neg61.fs(56,13,56,15): typecheck error FS3145: This is not a known query operator. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type.

neg61.fs(56,16,56,19): typecheck error FS0039: The value or constructor 'zip' is not defined.

neg61.fs(60,13,60,21): typecheck error FS3145: This is not a known query operator. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type.

neg61.fs(60,13,60,21): typecheck error FS0193: This expression is a function value, i.e. is missing arguments. Its type is  ^a ->  ^a.

neg61.fs(64,13,64,20): typecheck error FS3145: This is not a known query operator. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type.

neg61.fs(69,13,69,23): typecheck error FS3140: 'while' expressions may not be used in queries

neg61.fs(74,13,74,32): typecheck error FS3139: In queries, use the form 'for x in n .. m do ...' for ranging over integers

neg61.fs(79,13,79,16): typecheck error FS3146: 'try/with' expressions may not be used in queries

neg61.fs(86,13,86,16): typecheck error FS3141: 'try/finally' expressions may not be used in queries

neg61.fs(92,21,92,70): typecheck error FS3142: 'use' expressions may not be used in queries

neg61.fs(97,13,97,33): typecheck error FS3143: 'let!', 'use!' and 'do!' expressions may not be used in queries

neg61.fs(102,13,102,28): typecheck error FS3145: This is not a known query operator. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type.

neg61.fs(102,13,102,28): typecheck error FS3143: 'let!', 'use!' and 'do!' expressions may not be used in queries

neg61.fs(107,13,107,21): typecheck error FS3144: 'return' and 'return!' may not be used in queries

neg61.fs(111,13,111,24): typecheck error FS3144: 'return' and 'return!' may not be used in queries

neg61.fs(114,13,114,21): typecheck error FS3145: This is not a known query operator. Query operators are identifiers such as 'select', 'where', 'sortBy', 'thenBy', 'groupBy', 'groupValBy', 'join', 'groupJoin', 'sumBy' and 'averageBy', defined using corresponding methods on the 'QueryBuilder' type.

neg61.fs(114,22,114,23): typecheck error FS0001: The type 'int' does not support the operator 'Truncate'

neg61.fs(133,17,133,20): typecheck error FS3147: This 'let' definition may not be used in a query. Only simple value definitions may be used in queries.

neg61.fs(140,17,140,20): typecheck error FS3147: This 'let' definition may not be used in a query. Only simple value definitions may be used in queries.

neg61.fs(148,20,148,23): typecheck error FS3147: This 'let' definition may not be used in a query. Only simple value definitions may be used in queries.

neg61.fs(156,21,156,22): typecheck error FS3147: This 'let' definition may not be used in a query. Only simple value definitions may be used in queries.

neg61.fs(171,13,171,18): typecheck error FS3099: 'sumBy' is used with an incorrect number of arguments. This is a custom operation in this query or computation expression. Expected 1 argument(s), but given 0.

neg61.fs(174,22,174,23): typecheck error FS0041: No overloads match for method 'Source'.

Known type of argument: int

Available overloads:
 - member Linq.QueryBuilder.Source: source: System.Collections.Generic.IEnumerable<'T> -> Linq.QuerySource<'T,System.Collections.IEnumerable> // Argument 'source' doesn't match
 - member Linq.QueryBuilder.Source: source: System.Linq.IQueryable<'T> -> Linq.QuerySource<'T,'Q> // Argument 'source' doesn't match

neg61.fs(180,19,180,31): typecheck error FS3153: Arguments to query operators may require parentheses, e.g. 'where (x > y)' or 'groupBy (x.Length / 10)'

neg61.fs(186,19,186,31): typecheck error FS3153: Arguments to query operators may require parentheses, e.g. 'where (x > y)' or 'groupBy (x.Length / 10)'

neg61.fs(191,21,191,33): typecheck error FS3153: Arguments to query operators may require parentheses, e.g. 'where (x > y)' or 'groupBy (x.Length / 10)'

neg61.fs(197,21,197,33): typecheck error FS3153: Arguments to query operators may require parentheses, e.g. 'where (x > y)' or 'groupBy (x.Length / 10)'

neg61.fs(202,20,202,31): typecheck error FS3153: Arguments to query operators may require parentheses, e.g. 'where (x > y)' or 'groupBy (x.Length / 10)'
