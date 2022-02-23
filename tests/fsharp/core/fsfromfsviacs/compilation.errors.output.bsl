
test.fsx(216,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.
Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float32) : int

test.fsx(217,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: x: int

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float32) : int

test.fsx(218,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: y: string

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float32) : int

test.fsx(219,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: x: int option

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float32) : int

test.fsx(220,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: y: string option

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float32) : int

test.fsx(221,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: x: 'a option

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float32) : int

test.fsx(222,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: y: 'a option

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float32) : int

test.fsx(223,34): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d: 'a option

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float32) : int

test.fsx(226,42): error FS0041: A unique overload for method 'OverloadedMethodTakingOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: 'a0

Candidates:
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float) : int
 - SomeClass.OverloadedMethodTakingOptionals(?x: int, ?y: string, ?d: float32) : int

test.fsx(228,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.
Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(229,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: y: string

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(230,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d: Nullable<float>

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(231,36): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d: float

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(232,36): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d: float option

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(233,36): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: x: 'a option

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(234,36): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d: 'a option

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(236,43): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionalsWithDefaults' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: 'a0

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionalsWithDefaults(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(238,34): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.
Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(239,34): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: y: string

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(240,33): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d: float

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(241,34): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d: Nullable<float>

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(242,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d: float

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(243,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d: float option

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(244,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: x: 'a option

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(245,35): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: d: 'a option

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(246,42): error FS0041: A unique overload for method 'OverloadedMethodTakingNullableOptionals' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: 'a0

Candidates:
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int64>, ?y: string, ?d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullableOptionals(?x: Nullable<int>, ?y: string, ?d: Nullable<float>) : int

test.fsx(248,93): error FS0691: Named arguments must appear after all other arguments

test.fsx(249,88): error FS0041: A unique overload for method 'OverloadedMethodTakingNullables' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: Nullable<'a> * string * Nullable<'b> when 'a: (new: unit -> 'a) and 'a: struct and 'a :> ValueType and 'b: (new: unit -> 'b) and 'b: struct and 'b :> ValueType

Candidates:
 - SomeClass.OverloadedMethodTakingNullables(x: Nullable<int64>, y: string, d: Nullable<float>) : int
 - SomeClass.OverloadedMethodTakingNullables(x: Nullable<int>, y: string, d: Nullable<float>) : int

test.fsx(266,15): warning FS0025: Incomplete pattern matches on this expression. For example, the value 'U2 (_, U1 (_, "a"))' may indicate a case not covered by the pattern(s).

test.fsx(283,15): warning FS0025: Incomplete pattern matches on this expression. For example, the value 'U2 (_, U1 (_, "a"))' may indicate a case not covered by the pattern(s).

test.fsx(457,94): error FS0193: Type constraint mismatch. The type 
    'Nullable<int>'    
is not compatible with type
    'Nullable<int64>'    


test.fsx(458,82): error FS0193: Type constraint mismatch. The type 
    'Nullable<int>'    
is not compatible with type
    'Nullable<int64>'    

