// #Regression #TypeProvider #Fields #Literals
// This is regression test for DevDiv:214000, but it also covers field literals in general
//<Expects status="error" span="(9,9-9,48)" id="FS3149">Invalid provided literal value '\?'$</Expects>

//<Expects status="error" span="(17,9-17,26)" id="FS3149">Invalid provided literal value '10'$</Expects>

// Some invalid types (see ConstantObjToILFieldInit() in infos.fs)

let _ = N.T.Field_TypeThatHasToStringThatThrows
#if INTERACTIVE
;;
#endif
let _ = N.T.Field_enum        // THis is now totally fine!
#if INTERACTIVE
;;
#endif
let _ = N.T.Field_decimal
#if INTERACTIVE
;;
#endif
