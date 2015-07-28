// #Conformance #MemberDefinitions 

#if CoreClr
open coreclrutilities
#endif

let failures = ref false
let report_failure () = 
  stderr.WriteLine " NO"; failures := true
let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure() 

module CTree1 = begin

type 'a ctree = 
  class 
    val isLeaf: bool
    val leafVal: 'a option
    val children: 'a ctree list

    new(x : 'a) = { isLeaf = true; leafVal = Some(x); children = [] }
    
    new((dummy : bool) , (l : 'a ctree list)) = { isLeaf = false; leafVal = None; children = l }

    static member MkNode(l : 'a ctree list) = new ctree<_>(true, l)

  end
 

end

module CTree2 = begin



type 'a ctree = 
  class 
    val isLeaf: bool
    val leafVal: 'a option
    val children: 'a ctree list

    new(x : 'a) = { isLeaf = true; leafVal = Some(x); children = [] }
    
    new((dummy : bool) , (l : 'a ctree list)) = { isLeaf = false; leafVal = None; children = l }

    static member MkNode(l : 'a ctree list) = new ctree<_>(true, l)

  end
 
end

let _ = 
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        System.IO.File.WriteAllText("test.ok","ok"); 
        exit 0)

