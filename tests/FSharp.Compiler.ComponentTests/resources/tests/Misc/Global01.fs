// #Regression #Misc
// Regression for FSHARP1.0:6307. Global keyword used to not parse correctly in all cases

let i : global.System.Int32 = 0
let t = typeof<global.System.Int32> // used to not parse

exit 0
