// #Regression #Conformance #ObjectOrientedTypes #MethodsAndProperties #MemberDefinitions 
// Regression test for FSHARP1.0:5815
//<Expects status="error" span="(14,19-14,28)" id="FS1201">Cannot call an abstract base member: 'f'$</Expects>
//
 
[<AbstractClass>]
type C1 () =
 abstract f : unit -> C1

type C2 =
 inherit C1
 val x: C1
 new () =  { inherit C1 ();
             x  = base.f ()
           }
 override self.f () = self.x

let c2 = new C2 ()

let c1 = c2.f ()

c1.f () |> ignore

