// #Regression #Conformance #DeclarationElements #Accessibility 
// On let
//<Expects id="FS0531" span="(11,13-11,20)" status="error">Accessibility modifiers should come immediately prior to the identifier naming a construct</Expects>
//<Expects id="FS0058" span="(12,23-12,26)" status="warning">Possible incorrect indentation: this token is offside of context started at position \(11:23\)\. Try indenting this token further or using standard formatting conventions</Expects>
//<Expects id="FS0531" span="(12,13-12,19)" status="error">Accessibility modifiers should come immediately prior to the identifier naming a construct</Expects>
//<Expects id="FS0058" span="(13,23-13,26)" status="warning">Possible incorrect indentation: this token is offside of context started at position \(12:23\)\. Try indenting this token further or using standard formatting conventions</Expects>
//<Expects id="FS0531" span="(13,13-13,21)" status="error">Accessibility modifiers should come immediately prior to the identifier naming a construct</Expects>


module M =
            private   let x1 = 42      // here
            public    let x2 = 42      // here
            internal  let x3 = 42      // here
