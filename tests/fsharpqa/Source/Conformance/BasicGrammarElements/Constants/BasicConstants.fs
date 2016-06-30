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
if creditCardNumber <> 1234567890123456L then
    failwith "Wrong parsing"

let socialSecurityNumber = 999_99_9999L
if socialSecurityNumber <> 999999999L then
    failwith "Wrong parsing"

let pi =    3.14_15F
if pi <> 3.1415F then
    failwith "Wrong parsing"

let hexBytes = 0xFF_EC_DE_5E
if hexBytes <> 0xFFECDE5E then
    failwith "Wrong parsing"

let hexWords = 0xCAFE_BABE
if hexWords <> 0xCAFEBABE then
    failwith "Wrong parsing"

let maxLong = 0x7fff_ffff_ffff_ffffL
if maxLong <> 0x7fffffffffffffffL then
    failwith "Wrong parsing"

let nybbles = 0b0010_0101
if nybbles <> 0b00100101 then
    failwith "Wrong parsing"

let bytes = 0b11010010_01101001_10010100_10010010
if bytes <> 0b11010010011010011001010010010010 then
    failwith "Wrong parsing"

let x2 = 5_2
if x2 <> 52 then
    failwith "Wrong parsing"

let x4 = 5_______2
if x4 <> 52 then
    failwith "Wrong parsing"

let x7 = 0x5_2
if x7 <> 0x52 then
    failwith "Wrong parsing"

let x9 = 0_52
if x9 <> 052 then
    failwith "Wrong parsing"

let x10 = 05_2
if x10 <> 052 then
    failwith "Wrong parsing"

let x14 = 0o5_2
if x14 <> 0o52 then
    failwith "Wrong parsing"

exit 0
