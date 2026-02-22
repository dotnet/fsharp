// #Regression #Misc 
// <Expects status="error" id="FS0035" span="(12,6-12,10)">This construct is deprecated\: The treatment of this operator is now handled directly by the F# compiler and its meaning cannot be redefined</Expects>
// <Expects status="error" id="FS0035" span="(14,6-14,11)">This construct is deprecated\: The treatment of this operator is now handled directly by the F# compiler and its meaning cannot be redefined</Expects>
// <Expects status="error" id="FS0035" span="(16,6-16,12)">This construct is deprecated\: The treatment of this operator is now handled directly by the F# compiler and its meaning cannot be redefined</Expects>
// <Expects status="error" id="FS0035" span="(18,6-18,13)">This construct is deprecated\: The treatment of this operator is now handled directly by the F# compiler and its meaning cannot be redefined</Expects>
// <Expects status="error" id="FS0035" span="(20,6-20,11)">This construct is deprecated\: The treatment of this operator is now handled directly by the F# compiler and its meaning cannot be redefined</Expects>
// <Expects status="error" id="FS0035" span="(22,6-22,14)">This construct is deprecated\: The treatment of this operator is now handled directly by the F# compiler and its meaning cannot be redefined</Expects>

module TestModule

//794   operatorName -> DOT LBRACK COMMA RBRACK 
let (.[,]) v1 v2 = v1 + v2
//795   operatorName -> DOT LBRACK COMMA COMMA RBRACK 
let (.[,,]) v1 v2 = v1 + v2
//797   operatorName -> DOT LBRACK COMMA RBRACK LARROW 
let (.[,]<-) v1 v2 = v1 + v2
//798   operatorName -> DOT LBRACK COMMA COMMA RBRACK LARROW 
let (.[,,]<-) v1 v2 = v1 + v2
//799   operatorName -> DOT LBRACK DOT_DOT RBRACK 
let (.[..]) v1 v2 = v1 + v2
//800   operatorName -> DOT LBRACK DOT_DOT COMMA DOT_DOT RBRACK 
let (.[..,..]) v1 v2 = v1 + v2
