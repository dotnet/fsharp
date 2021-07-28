
test-langversion-preview.fsx(4,10,4,15): typecheck error FS3365: The syntax 'expr1[expr2]' is used for indexing/slicing. Consider adding a type annotation to enable indexing/slicing, or if calling a function add a space, e.g. 'expr1 [expr2]'.

test-langversion-preview.fsx(5,10,5,15): typecheck error FS3365: The syntax 'expr1[expr2]' is used for indexing/slicing. Consider adding a type annotation to enable indexing/slicing, or if calling a function add a space, e.g. 'expr1 [expr2]'.

test-langversion-preview.fsx(6,13,6,19): typecheck error FS3369: The syntax '[expr1][expr2]' is ambiguous when used as in an argument list. If you are calling a function, add a space between arguments 'someFunction [expr1] [expr2]'.

test-langversion-preview.fsx(7,13,7,24): typecheck error FS3369: The syntax '(expr1)[expr2]' is ambiguous when used as in an argument list. If you intend indexing or slicing then you must use '(expr1).[expr2]' in argument position. If you are calling a function, add a space between arguments 'someFunction (expr1) [expr2]'.

test-langversion-preview.fsx(9,18,9,21): typecheck error FS0001: The type 'float' does not match the type 'int'

test-langversion-preview.fsx(9,16,9,17): typecheck error FS0043: The type 'float' does not match the type 'int'
