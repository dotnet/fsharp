// It is an error to try and use :? IErasedType
//<Expects status="error" span="(5,3-5,10)" id="FS3062">This type test with a provided type 'N\.I1' is not allowed because this provided type will be erased to 'System\.Object' at runtime\.$</Expects>

match box 1 with
| :? N.I1 -> 0
| _ -> 1

