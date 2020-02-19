
test.fsx(186,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.
Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float32) : int

test.fsx(187,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: x:int

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float32) : int

test.fsx(188,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: y:string

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float32) : int

test.fsx(189,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: x:int option

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float32) : int

test.fsx(190,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: y:string option

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float32) : int

test.fsx(191,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: x:'a option

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float32) : int

test.fsx(192,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: y:'a option

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float32) : int

test.fsx(193,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d:'a option

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float32) : int

test.fsx(196,42): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: 'a0

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int,?y: string,?d: float32) : int

test.fsx(198,34): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.
Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(199,34): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: y:string

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(200,34): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d:Nullable<float>

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(201,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d:float

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(202,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d:float option

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(203,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: x:'a option

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(204,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d:'a option

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(206,42): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: 'a0

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(207,34): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.
Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(208,34): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: y:string

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(209,34): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d:float

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(210,34): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d:Nullable<float>

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(211,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d:float

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(212,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d:float option

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(213,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: x:'a option

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(214,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d:'a option

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(215,42): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: 'a0

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>,?y: string,?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>,?y: string,?d: Nullable<float>) : int

test.fsx(232,15): warning FS0025: Incomplete pattern matches on this expression. For example, the value 'U2 (_, U1 (_, "a"))' may indicate a case not covered by the pattern(s).

test.fsx(249,15): warning FS0025: Incomplete pattern matches on this expression. For example, the value 'U2 (_, U1 (_, "a"))' may indicate a case not covered by the pattern(s).
