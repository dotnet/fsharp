// #Stress 
#indent "off"

  module NodeSet = Set
  module NodeMap = Map
  type node = int
  type node_set = node Set
  type 'a node_map = (node,'a) Map

  type info = {
      succ : node_set;
      pred : node_set;
      mutable dfNumber   : int;
      mutable mark1      : int;
      mutable mark2      : int;
      mutable lowLink    : int;
    }
      
  let info() = 
    {
     succ = NodeSet.empty;
     pred = NodeSet.empty;
     mark1 = 0;
     mark2 = 0;
     lowLink = 0;
     dfNumber = 0;
   }
      
  type t = {
      mutable nodes : info node_map;
      mutable generation : int;
(*
      mutable isLogged : bool;
      log : Log.t;
*)
    }
      
  let is_marked    t info = info.mark1 = t.generation
  let set_mark     t info = info.mark1 <- t.generation
  let unset_mark   t info = info.mark1 <- 0

  let is_onStack   t info = info.mark2 = t.generation
  let set_onStack  t info = info.mark2 <- t.generation
  let set_offStack t info = info.mark2 <- 0

  let is_marked2  = is_onStack
  let set_mark2   = set_onStack
  let unset_mark2 = set_offStack
      
  let set_lowLink  t info n = info.lowLink <- n
  let get_lowLink  t info = info.lowLink

  let set_dfNumber t info n = info.dfNumber <- n
  let get_dfNumber t info = info.dfNumber

  let nodeInfo t node = NodeMap.find node t.nodes 

(* constructors *)
  let clearGraph t = t.generation <- t.generation + 1
          
  let empty() = 
    {
     nodes = NodeMap.empty;
     generation = 0;
(*
     isLogged = false;
     log = Log.empty()
*)
   }

  let setInfo t node info oldInfo = 
    t.nodes <- NodeMap.add node info t.nodes
(*
    if t.isLogged then
      Log.append t.log 
	(fun () -> 
	  (match oldInfo with
	  | None -> t.nodes <- NodeMap.remove node t.nodes
	  | Some info -> t.nodes <- NodeMap.add node info t.nodes)
	)
*)

  let getInfo t node = 
    try
      let info0 = NodeMap.find node t.nodes in
      (Some info0, info0)
    with
      :? System.Collections.Generic.KeyNotFoundException -> (None, info())

  let setPred t node fn = 
    let info0, info1 = getInfo t node in
    let info2 = { info1 with pred = fn info1.pred  } in
    setInfo t node info1 info0

  let setSucc t node fn =
    let info0, info1 = getInfo t node in
    let info2 = { info1 with succ = fn info1.succ  } in
    setInfo t node info2 info0
    
  let addPred t src dst = setPred t dst (NodeSet.add src)
  let addSucc t src dst = setSucc t src (NodeSet.add dst)
      
  let edge t src dst = 
    addSucc t src dst;
    addPred t src dst
      

(* inspectors *)

  let nodes t f a = 
    NodeMap.foldBack (fun key _ a -> f key a) t.nodes a

  let edges t f a = 
    NodeMap.foldBack
      (fun src info a -> 
	(NodeSet.foldBack (fun dst a -> f src dst a) info.succ a))
      t.nodes a

(* algorithms *)
  exception Cycle of node list

  let findCycle t = 
    clearGraph t;
    let rec pop_until n stack rest = 
      match stack with 
      | [] -> rest 
      | n'::ns -> if n = n' then rest else pop_until n ns (n'::rest)
    in
    let rec search stack n info = 
      if is_marked t info then
	if is_onStack t info then
	  raise (Cycle (pop_until n stack [n]))
	else
	  ()
      else
	begin
	  let stack = n::stack in
	  set_mark t info;
	  set_onStack t info;
	  NodeSet.iter
	      (fun key -> search stack key (nodeInfo t key))
	      info.succ;
	  set_offStack t info
	end
    in
    try 
      NodeMap.iter
	(fun src info -> if not (is_marked t info) then search [] src info) 
	t.nodes;
      []
    with
    | Cycle c -> c
	
  let assign t dst src = 
    if src <> dst then
      let dstI = nodeInfo t dst in
      let srcI = nodeInfo t src in
      let updateSet set = NodeSet.add dst (NodeSet.remove src set) in
      NodeSet.iter (fun pred -> setSucc t pred updateSet) srcI.pred;
      NodeSet.iter (fun succ -> setPred t succ updateSet) srcI.succ

module Dfs0 = begin

  (* DFS just using the call-stack *)
  let dfs t pre post succPre succPost node nodeI acc = 
    let rec dfsNode node nodeI acc = 
      let acc = pre node nodeI acc in
      let acc = NodeSet.foldBack (dfsSucc node nodeI) nodeI.succ acc in
      post node nodeI acc
    and dfsSucc node nodeI succ acc = 
      let succI = nodeInfo t succ in
      if succPre node nodeI succ succI then
	let acc = dfsNode succ succI acc in
	succPost node nodeI succ succI acc
      else
	acc  
	in

    dfsNode node nodeI acc

end
module Dfs0b = begin

    let rec dfsNode ((pre,post,_,_,_) as funs) node nodeI acc = 
      let acc = pre node nodeI acc in
      let acc = NodeSet.foldBack (dfsSucc funs node nodeI) nodeI.succ acc in
      post node nodeI acc
    and dfsSucc ((_,_,succPre,succPost,t) as funs) node nodeI succ acc = 
      let succI = nodeInfo t succ in
      if succPre node nodeI succ succI then
	    let acc = dfsNode funs succ succI acc in
	    succPost node nodeI succ succI acc
      else
	    acc

  (* DFS just using the call-stack *)
  let inline dfs t pre post succPre succPost node nodeI acc = 
       dfsNode (pre,post,succPre,succPost,t) node nodeI acc

end


  type sccs = node list list

  type 'a stack = 
    | Nil
    | Cons of ('a stack -> 'a -> ('a * ('a stack))) * ('a stack)

  let cons f stack = Cons(f,stack)
  let nil = Nil

  let rec eval (a, stack) = 
    match stack with
    | Nil -> a
    | Cons (f,stack) -> eval (f stack a)

(* requires rectypes 

  type 'a stack1 = ('a stack1 -> 'a -> ('a * 'a stack1)) list

  let cons1 f stack = f::stack

  let rec eval1 (a, stack) = 
    match stack with
    | [] -> a
    | f::stack -> eval1 (f stack a)
*)

  (* DFS building up a custom call-stack *)
  
  module Dfs1 = begin
  
  let dfs t pre post succPre succPost node nodeI acc = 
    let wrap f node nodeI stack acc = (f node nodeI acc,stack) in
    let pre  = wrap pre          in
    let post = wrap post         in
    let succPost n nI s sI stack acc = (succPost n nI s sI acc,stack) in
    (* DFS search on a node *)
    let rec dfsNode node nodeI stack = 
      cons 
	(pre node nodeI)
	(cons 
	   (fun stack acc -> 
	     (acc, NodeSet.foldBack (dfsSucc node nodeI) nodeI.succ stack))
	   (cons 
	      (post node nodeI)
	      stack))
    (* DFS search on the successor *)
    and dfsSucc node nodeI succ stack = 
      let succI = nodeInfo t succ in
      if succPre node nodeI succ succI then
	cons 
	  (fun stack acc -> (acc, dfsNode succ succI stack))
	  (cons 
	     (succPost node nodeI succ succI)
	     stack)
      else
	stack
    in
    eval (acc, dfsNode node nodeI nil)

 end
 
 module Dfs1b = begin
    let rec dfsNode ((pre,post,succPre,succPost,t) as funs) node nodeI stack = 
      cons 
	(pre node nodeI)
	(cons 
	   (fun stack acc -> 
	     (acc, NodeSet.foldBack (dfsSucc funs node nodeI) nodeI.succ stack))
	   (cons 
	      (post node nodeI)
	      stack))
    (* DFS search on the successor *)
    and dfsSucc ((pre,post,succPre,succPost,t) as funs) node nodeI succ stack = 
      let succI = nodeInfo t succ in
      if succPre node nodeI succ succI then
	cons 
	  (fun stack acc -> (acc, dfsNode funs succ succI stack))
	  (cons 
	     (succPost node nodeI succ succI)
	     stack)
      else
	stack
    

  (* DFS building up a custom call-stack *)
  let dfs t pre post succPre succPost node nodeI acc = 
    let wrap f node nodeI stack acc = (f node nodeI acc,stack) in
    let pre  = wrap pre          in
    let post = wrap post         in
    let succPost n nI s sI stack acc = (succPost n nI s sI acc,stack) in
    (* DFS search on a node *)
    eval (acc, dfsNode (pre,post,succPre,succPost,t) node nodeI nil)
 end
 

(*-------------------------------------------------------------------------
   findSCCs is based upon Tarjan's algorithm, 
   as explained in Aho, Hopcroft and Ullman. 
   It finds all the strongly connected components of a graph.
   It requires the call-stack to traverse the graph.
   ------------------------------------------------------------------------*)

  let findSCCsBase dfs t =
    clearGraph t;
    let count = ref 0  in
    let stack = ref [] in
    let rec pop_until node result = 
      match !stack with
      | [] -> result
      | ((node',info')::rest) -> 
	  stack := rest;
	  set_offStack t info';
	  let result = node'::result in
	  if node = node' then 
	    result
	  else 
	    pop_until node result
    in
    (* called every time visiting a new node *)
    let pre node nodeI sccs = 
      assert (not(is_marked t nodeI));
      set_mark t nodeI;
      set_dfNumber t nodeI !count;
      set_lowLink t nodeI !count;
      set_onStack t nodeI;
      count := !count + 1;
      stack := (node,nodeI)::(!stack);
      sccs
    in
    (* called every time backtracking from a node *)
    let post node nodeI sccs = 
      if get_dfNumber t nodeI = get_lowLink t nodeI then
	(pop_until node [])::sccs
      else
	sccs
    in
    (* called to filter out successor nodes *)
    let succPre node nodeI succ succI = 
      if is_marked t succI then
	begin
	  let dfNext = get_dfNumber t succI in
	  let dfMe   = get_dfNumber t nodeI in
	  if (dfNext < dfMe && is_onStack t succI) then
	    set_lowLink t nodeI (min (get_lowLink t nodeI) dfNext);
	  false
	end
      else
	true
    in
    (* called when backtracking from successor nodes *)
    let succPost node nodeI succ succI sccs = 
      set_lowLink t nodeI 
	(min (get_lowLink t nodeI) (get_lowLink t succI));
      sccs
    in    
    let findSCC node nodeI sccs = 
      if is_marked t nodeI then 
	sccs
      else
    let rec dfsNode node nodeI acc = 
      let acc = pre node nodeI acc in
      let acc = NodeSet.foldBack (dfsSucc  node nodeI) nodeI.succ acc in
      post node nodeI acc
    and dfsSucc node nodeI succ acc = 
      let succI = nodeInfo t succ in
      if succPre node nodeI succ succI then
	    let acc = dfsNode  succ succI acc in
	    succPost node nodeI succ succI acc
      else
	    acc in 

  (* DFS just using the call-stack *)
  let dfs t pre post succPre succPost node nodeI acc = 
       dfsNode node nodeI acc in
	dfs t pre post succPre succPost node nodeI sccs
    in
    NodeMap.foldBack findSCC t.nodes []

  let findSCCs  = findSCCsBase Dfs1.dfs

  let push t = ()
(*
    t.isLogged <- true;
    Log.push t.log
*)

  let pop t = ()
(*
    Log.pop t.log
*)
  exception Found

  let connected t n1 n2 = 
    clearGraph t;
    let rec search node = 
      if node = n2 then
	raise Found
      else
	let nodeI = nodeInfo t node in
	if not (is_marked t nodeI) then
	  begin
	    set_mark t nodeI;
	    NodeSet.iter search nodeI.succ
	  end
    in
    try
      search n1;
      false
    with
    | Found -> true


(* Check whether there is an edge from node to itself *)
  let selfLoop t node = 
    let nodeI = nodeInfo t node in
    NodeSet.contains node nodeI.succ


  (* find shortest path from initial to final, if any *)
  (* instance of bfs *)

  exception FoundPath of node list

  let shortestPath t initial final = 
    clearGraph t;
    List.iter (fun n -> set_mark2 t (nodeInfo t n)) final;
    let expandOne path frontier n =
      let nodeI = nodeInfo t n in
      if is_marked2 t nodeI then
	raise (FoundPath (n::path))
      else if is_marked t nodeI then
	frontier
      else 
	begin
	  set_mark t nodeI;
	  (n::path, nodeI.succ)::frontier
	end
    in
    let expandList frontier (path,succ) = 
      NodeSet.foldBack 
	(fun node frontier -> expandOne path frontier node) 
	succ 
	frontier
    in
    let rec loop frontier = 
      if [] = frontier then 
	None 
      else
	loop (List.fold expandList [] frontier)
    in
    try 
      loop (List.fold (expandOne [])  [] initial)
    with
      FoundPath path -> Some (List.rev path)
	
(*------------------------------------------------------------------------
  Apply fn to all nodes reachable from initial.
  -----------------------------------------------------------------------*)

  let foldReachable t initial fn a = 
    clearGraph t;
    let rec reach a node = 
      let nodeI = nodeInfo t node in
      if is_marked t nodeI then
	a
      else
	let a = fn node a in
	begin
	  set_mark t nodeI;
	  NodeSet.foldBack reach nodeI.succ a
	end
    in
    List.fold reach a initial
	

(*module UnitTest = *)

  let printGraph t =
    (*Graph.*)nodes t (fun n () -> Printf.printf "node: %d\n" n) ();
    (*Graph.*)edges t (fun s d () -> Printf.printf "edge: %d->%d\n" s d) ()


  let printSccs = 
    List.iter (fun scc -> 
      Printf.printf "Scc\n";
      List.iter (fun node -> Printf.printf "%d\n" node) scc)

   let time () = (System.Diagnostics.Process.GetCurrentProcess()).UserProcessorTime.TotalSeconds 
   
  let timeIt f x = 
    let t1 = time() in
    let y  = f x in
    let y  = f x in
    let y  = f x in
    let y  = f x in
    let y  = f x in
    let y  = f x in
    let y  = f x in
    let y  = f x in
    let y  = f x in
    let y  = f x in
    let t2 = time() in
    Printf.printf "time: %f\n" (t2  - t1);
    y

  let test() = 
    let t = (*Graph.*)empty() in
    let _ = (*Graph.*)edge t 1 2 in
    let _ = (*Graph.*)edge t 2 3 in
    let _ = (*Graph.*)edge t 3 1 in
    let _ = (*Graph.*)edge t 1 4 in
    let _ = (*Graph.*)edge t 4 5 in
    let _ = printGraph t     in

    let sccs = (*Graph.*)findSCCs t in
    let _ = printSccs sccs      in
    (* check use of call-stack *)
    let _ = 
      for i = 6 to 100 do
	(*Graph.*)edge t i (i+1)
      done;
      (*Graph.*)edge t 99 6
    in

    Printf.printf "call stack, inner loop: ";
    let sccs = timeIt (*Graph.*)(findSCCsBase Dfs0.dfs) t in
    Printf.printf "call stack, outer loop: ";
    let sccs = timeIt (*Graph.*)(findSCCsBase Dfs0b.dfs) t in
    Printf.printf "heap stack, inner loop; ";
    let sccs = timeIt (*Graph.*)(findSCCsBase Dfs1.dfs) t in
    Printf.printf "heap stack, outer loop: ";
    let sccs = timeIt (*Graph.*)(findSCCsBase Dfs1b.dfs) t in
    Printf.printf "done.\n";
    ()

  let _ = test()

do   (System.Console.WriteLine "Test Passed"; 
       printf "TEST PASSED OK";
       exit 0)
