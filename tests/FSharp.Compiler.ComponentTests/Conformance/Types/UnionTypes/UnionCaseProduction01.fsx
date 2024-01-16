// #Conformance #TypesAndModules #Unions 
// Union Types
// union-case :=
//  | id	-- nullary union case
//  | id of type * ... * type	-- n-ary union case
//  | id : sig-spec	-- n-ary union case
//<Expects status="success"></Expects>
#light

type T = | A
         | B of int
         | C of int * float
//         | D : int -> T  -- warning (covered in a different TC)
