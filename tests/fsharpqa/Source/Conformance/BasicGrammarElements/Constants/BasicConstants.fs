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

let creditCardNumber = 1234_5678_9012_3456L
let socialSecurityNumber = 999_99_9999L
let pi =    3.14_15F
let hexBytes = 0xFF_EC_DE_5E
let hexWords = 0xCAFE_BABE
let maxLong = 0x7fff_ffff_ffff_ffffL
let nybbles = 0b0010_0101
let bytes = 0b11010010_01101001_10010100_10010010
let x2 = 5_2
let x4 = 5_______2
let x7 = 0x5_2
let x9 = 0_52
let x10 = 05_2
let x14 = 0o5_2

exit 0
