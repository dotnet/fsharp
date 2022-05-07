// #Regression #NoMono #NoMT #CodeGen #EmittedIL   
// Regression test for FSHARP1.0:1881

// Make sure that when targeting NetFx4.0, we follow the new pattern:
//
//            bool lockTaken = false;
//            try
//            {
//                Monitor.Enter(obj, ref lockTaken);
//                //do stuff
//            }
//            finally
//            {
//                if (lockTaken) 
//                    Monitor.Exit(obj);
//           }


let o = new System.Object()
lock o (fun () -> () )
