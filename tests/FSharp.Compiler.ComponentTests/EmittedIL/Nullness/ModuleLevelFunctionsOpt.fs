module MyTestModule

let nonNullableInputOutputFunc (x:string) = x
let nullableStringInputOutputFunc (x: string | null) = x
let nonNullableIntFunc (x:int) = x
let nullableIntFunc (x:System.Nullable<int>) = x
let genericValueTypeTest (x: struct(string * (string|null) * int * int * int * int)) = x
let genericRefTypeTest (x: string * (string|null) * int * int * int * int) = x
let nestedGenericsTest (x: list<list<string | null> | null> | null) = x
let multiArgumentTest (x:string) (y:string | null) = 42