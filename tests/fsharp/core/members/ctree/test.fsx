// #Conformance #MemberDefinitions 
#if TESTS_AS_APP
module Core_members_ctree
#endif

let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

let check s b1 b2 = test s (b1 = b2)

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


#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      printf "TEST PASSED OK" ;
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif
