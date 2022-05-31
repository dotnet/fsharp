// #Regression #NoMT #Printing 
// Regression test for FSHARP1.0:5510
// <Expects status="success">val it: Choice<string,'a></Expects>
// <Expects status="success">val it: Choice<int16,'a></Expects>
// <Expects status="success">val it: Choice<'a,'b,Choice<'c,decimal,'d,'e,'f,'g,'h>,'i></Expects>

Choice1Of2("a");;
Choice1Of2(1s);;
Choice3Of4(Choice2Of7(1m));;

#q;;
