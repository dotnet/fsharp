// #Regression #Conformance #LexicalAnalysis 
// Reserved identifiers and keywords
// Also cover regression test for FSHARP1.0:5367 (keyword 'virtual')
let atomic = 10
let break = 10
let checked = 10
let component = 10

let constraint = 10
let constructor  = 10
let continue = 10
let eager = 10
let fixed = 10
let fori = 10
let functor = 10
let include  = 10
let method = 10
let measure = 10
let mixin = 10
let object = 10
let parallel = 10
let params = 10
let process = 10
let protected = 10
let pure  = 10
let recursive = 10
let sealed = 10
let tailcall = 10
let trait = 10
let virtual = 10
let volatile  = 10

//<Expects span="(4,5-4,11)" status="warning" id="FS0046">The identifier 'atomic' is reserved for future use by F#</Expects>
//<Expects span="(5,5-5,10)" status="warning" id="FS0046">The identifier 'break' is reserved for future use by F#</Expects>
//<Expects span="(6,5-6,12)" status="warning" id="FS0046">The identifier 'checked' is reserved for future use by F#</Expects>
//<Expects span="(7,5-7,14)" status="warning" id="FS0046">The identifier 'component' is reserved for future use by F#</Expects>

//<Expects span="(9,5-9,15)" status="warning" id="FS0046">The identifier 'constraint' is reserved for future use by F#</Expects>
//<Expects span="(10,5-10,16)" status="warning" id="FS0046">The identifier 'constructor' is reserved for future use by F#</Expects>
//<Expects span="(11,5-11,13)" status="warning" id="FS0046">The identifier 'continue' is reserved for future use by F#</Expects>
//<Expects span="(12,5-12,10)" status="warning" id="FS0046">The identifier 'eager' is reserved for future use by F#</Expects>
//<Expects span="(13,5-13,10)" status="warning" id="FS0046">The identifier 'fixed' is reserved for future use by F#</Expects>
//<Expects span="(14,5-14,9)" status="warning" id="FS0046">The identifier 'fori' is reserved for future use by F#</Expects>
//<Expects span="(15,5-15,12)" status="warning" id="FS0046">The identifier 'functor' is reserved for future use by F#</Expects>
//<Expects span="(16,5-16,12)" status="warning" id="FS0046">The identifier 'include' is reserved for future use by F#</Expects>
//<Expects span="(17,5-17,11)" status="warning" id="FS0046">The identifier 'method' is reserved for future use by F#</Expects>
//<Expects span="(18,5-18,12)" status="warning" id="FS0046">The identifier 'measure' is reserved for future use by F#</Expects>
//<Expects span="(19,5-19,10)" status="warning" id="FS0046">The identifier 'mixin' is reserved for future use by F#</Expects>
//<Expects span="(20,5-20,11)" status="warning" id="FS0046">The identifier 'object' is reserved for future use by F#</Expects>
//<Expects span="(21,5-21,13)" status="warning" id="FS0046">The identifier 'parallel' is reserved for future use by F#</Expects>
//<Expects span="(22,5-22,11)" status="warning" id="FS0046">The identifier 'params' is reserved for future use by F#</Expects>
//<Expects span="(23,5-23,12)" status="warning" id="FS0046">The identifier 'process' is reserved for future use by F#</Expects>
//<Expects span="(24,5-24,14)" status="warning" id="FS0046">The identifier 'protected' is reserved for future use by F#</Expects>
//<Expects span="(25,5-25,9)" status="warning" id="FS0046">The identifier 'pure' is reserved for future use by F#</Expects>
//<Expects span="(26,5-26,14)" status="warning" id="FS0046">The identifier 'recursive' is reserved for future use by F#</Expects>
//<Expects span="(27,5-27,11)" status="warning" id="FS0046">The identifier 'sealed' is reserved for future use by F#</Expects>
//<Expects span="(28,5-28,13)" status="warning" id="FS0046">The identifier 'tailcall' is reserved for future use by F#</Expects>
//<Expects span="(29,5-29,10)" status="warning" id="FS0046">The identifier 'trait' is reserved for future use by F#</Expects>
//<Expects span="(30,5-30,12)" status="warning" id="FS0046">The identifier 'virtual' is reserved for future use by F#</Expects>
//<Expects span="(31,5-31,13)" status="warning" id="FS0046">The identifier 'volatile' is reserved for future use by F#</Expects>
