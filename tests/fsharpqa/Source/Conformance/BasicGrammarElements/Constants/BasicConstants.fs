// #Conformance #BasicGrammarElements #Constants 
#light

// Verify the ability to specify basic constants    

let sbyteConst = 1y
let int16Const = 1us
let int32Const = 1ul
let int64Const = 1UL

let byteConst  = 1uy
let uint16Const = 1us
let uint32Const = 1ul
let uint64Const = 1uL

let ieee32Const1 = 1.0f
let ieee32Const2 = 1.0F
let ieee32Const3 = 0x0000000000000001lf

let ieee64Const1 = 1.0
let ieee64Const2 = 0x0000000000000001LF

let bigintConst = 1I

// let bignumConst = 1N - you need a reference to PowerPack.dll now

let decimalConst1 = 1.0M
let decimalConst2 = 1.0m

let charConst = '1'

let stringConst = "1"

let bytestringConst = "1"B

let bytecharConst = '1'B

let boolConst1 = true
let boolConst2 = false

let unitConst = ()

exit 0
