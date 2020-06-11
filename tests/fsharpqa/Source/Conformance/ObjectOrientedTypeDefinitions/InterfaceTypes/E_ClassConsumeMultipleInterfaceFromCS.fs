// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations #ReqNOMT 
// It is now allowed to implement the same interface multiple times (RFC FS-1031).
#light	

let mutable res = true

type D() = 
    inherit T()
    interface I_003<int> with
       member xxx.Home(i) = i

if (D() :> I_003<int>).Home(5) <> 5 then
            System.Console.WriteLine("D.Home failed")         
            res <- false


if (res = true) then
    exit 0
    
  
exit 1

