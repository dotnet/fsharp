
neg_tupled_arguments.fsx(6,9,6,31): typecheck error FS0041: No overloads match for method 'A'.

Known types of arguments: (int * (int * string) * int) * int

Available overloads:
 - static member A.A: ('a0 * ('a1 * int) * 'a2) * ('a3 * ('a4 * 'a5)) -> 'a0 * 'a1 * int * 'a2 * 'a3 * 'a4 * 'a5 // Argument at index 1 doesn't match
 - static member A.A: ('a0 * ('a1 * int) * 'a2) * e: 'a3 -> 'a0 * 'a1 * int * 'a2 * 'a3 // Argument at index 1 doesn't match

neg_tupled_arguments.fsx(7,9,7,28): typecheck error FS0503: A member or object constructor 'A' taking 4 arguments is not accessible from this code location. All accessible versions of method 'A' take 2 arguments.

neg_tupled_arguments.fsx(14,9,14,40): typecheck error FS0041: No overloads match for method 'B'.

Known types of arguments: int * int * (int * (int * int * int * (int * int))) * int * int

Available overloads:
 - static member B.B: a: 'a0 * b: 'a1 * ('a2 * ('a3 * 'a4 * 'a5 * ('a6 * decimal))) * i: 'a7 * j: 'a8 -> 'a0 * 'a1 * 'a2 * 'a3 * 'a4 * 'a5 * 'a6 * decimal * 'a7 * 'a8 // Argument at index 3 doesn't match
 - static member B.B: a: 'a0 * b: 'a1 * ('a2 * ('a3 * 'a4 * 'a5 * ('a6 * string))) * i: 'a7 * j: 'a8 -> 'a0 * 'a1 * 'a2 * 'a3 * 'a4 * 'a5 * 'a6 * string * 'a7 * 'a8 // Argument at index 3 doesn't match
