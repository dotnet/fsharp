// #ByRef #Regression #inline
// Regression test for DevDiv:122445 ("Internal compiler error when evaluating code with inline/byref")
//<Expects status="error" span="(6,12)" id="FS3136">Type '\('a -> string -> byref<byref<'a>> -> bool\)' is illegal because in byref<T>, T cannot contain byref types\.$</Expects>
// SHould give an error - not ICE!

let inline f (_:^a) x (y:_ byref) = (^a : (static member TryParse : string * ^a byref -> bool)(x,y))

#q;;