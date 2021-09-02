
test-langversion-default.fsx(4,10,4,15): typecheck info FS3365: The syntax 'expr1[expr2]' is used for indexing. Consider adding a type annotation to enable indexing, or if calling a function add a space, e.g. 'expr1 [expr2]'.

test-langversion-default.fsx(5,10,5,15): typecheck info FS3365: The syntax 'expr1[expr2]' is used for indexing. Consider adding a type annotation to enable indexing, or if calling a function add a space, e.g. 'expr1 [expr2]'.

test-langversion-default.fsx(6,13,6,19): typecheck error FS3369: The syntax '[expr1][expr2]' is ambiguous when used as an argument. See https://aka.ms/fsharp-index-notation. If you intend indexing or slicing then you must use '(expr1).[expr2]' in argument position. If calling a function with multiple curried arguments, add a space between them, e.g. 'someFunction [expr1] [expr2]'.

test-langversion-default.fsx(7,13,7,24): typecheck error FS3369: The syntax '(expr1)[expr2]' is ambiguous when used as an argument. See https://aka.ms/fsharp-index-notation. If you intend indexing or slicing then you must use '(expr1).[expr2]' in argument position. If calling a function with multiple curried arguments, add a space between them, e.g. 'someFunction (expr1) [expr2]'.

test-langversion-default.fsx(9,18,9,21): typecheck error FS0001: The type 'float' does not match the type 'int'

test-langversion-default.fsx(9,16,9,17): typecheck error FS0043: The type 'float' does not match the type 'int'
