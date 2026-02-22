// #Regression #NoMT #FSI 
// Regression for FSB 5802, Exceptions thrown from non-UI thread in fsi.exe show internal error
// Previously we were showing an internal error :-/ we should surface the actual exception.
//<Expects status="success">System\.Exception: game over man, game over</Expects>


// TODO: this test is unreliable, the test may finish before the async is processed and throws -> test failed
// however, if you put a loop after the async as below to make sure the async is processed the automation
// chokes and sits in a loop never throwing, never printing, very strange
Async.Start (async { failwith "game over man, game over" } );;

//while true do ();;


System.Threading.Thread.Sleep(2000);;

#q;;
