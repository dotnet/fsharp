// #Regression #Conformance #DeclarationElements #LetBindings 
//<Expects status="error" span="(10,18)" id="FS0267">This is not a valid constant expression or custom attribute value$</Expects>
//<Expects status="error" span="(10,13)" id="FS0837">This is not a valid constant expression$</Expects>
//<Expects status="error" span="(16,13)" id="FS0267">This is not a valid constant expression or custom attribute value$</Expects>
//<Expects status="error" span="(19,13)" id="FS0267">This is not a valid constant expression or custom attribute value$</Expects>
//<Expects status="error" span="(22,13)" id="FS0267">This is not a valid constant expression or custom attribute value$</Expects>
//<Expects status="warning" span="(25,13)" id="FS3178">This is not valid literal expression. The \[<Literal>\] attribute will be ignored\.$</Expects>

[<Literal>]
let lit01 = (let x = "2" in x)

[<Literal>]
let lit02 = 1.0M

[<Literal>]
let lit03 = ()

[<Literal>]
let lit04 = 100n

[<Literal>]
let lit05 = 100un

[<Literal>]
let lit06 = System.Guid()