//<Expects id="FS1156" span="(18,11-18,13)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
//<Expects id="FS1156" span="(18,14-18,19)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
//<Expects id="FS0599" span="(18,13-18,14)" status="error">Missing qualification after '.'</Expects>
//<Expects id="FS1156" span="(19,11-19,19)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
//<Expects id="FS1156" span="(20,29-20,42)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
//<Expects id="FS1156" span="(22,10-22,13)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
//<Expects id="FS1156" span="(23,10-23,15)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
//<Expects id="FS1156" span="(24,10-24,15)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
//<Expects id="FS1156" span="(25,10-25,15)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
//<Expects id="FS1156" span="(26,10-26,14)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
//<Expects id="FS1156" span="(27,10-27,15)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
//<Expects id="FS1156" span="(28,10-28,15)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
//<Expects id="FS1156" span="(29,10-29,15)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
//<Expects id="FS1156" span="(30,11-30,18)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
//<Expects id="FS1156" span="(31,11-31,18)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
//<Expects id="FS1156" span="(32,11-32,16)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>

let pi1 = 3_.1415F
let pi2 = 3._1415F
let socialSecurityNumber1 = 999_99_9999_L
let x1 = _52
let x2 = 52_
let x3 = 0_x52
let x4 = 0x_52
let x5 = 0x52_
let x6 = 052_
let x7 = 0_o52
let x8 = 0o_52
let x9 = 0o52_
let x10 = 2.1_e2F
let x11 = 2.1e_2F
let x12 = 1.0_F