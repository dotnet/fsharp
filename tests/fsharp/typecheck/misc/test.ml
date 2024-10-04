// #TypeInference #Regression 
(* Avoid ;; since it allows fsi to recover from type errors *)

open System.Collections.Generic
let repro (ie : IEnumerable<'a>) = None
(* avoid ;; -- to avoid fsi error recovery *)    

(* Bug693: user annotated 'a generalised, and then picked up (out of scope) from tpenv *)
let create() = new Event<_>()
let f () = 
    // let sendM,_ = Event.create() in // : ('a -> unit) * 'a IEvent in
    let sendM = create().Trigger in // : ('a -> unit) in
 // let sendM = create()       : ('a -> unit) in
    let delegateSendM msg = new System.Converter<'a,unit>(fun _ -> sendM msg) in
    let send (msg:'a) = delegateSendM msg in
    send
(* avoid ;; -- to avoid fsi error recovery *)    
      
let Memoize f = 
    let t = new Dictionary<_,_>(1000) in
    fun x -> 
        let ok,v = t.TryGetValue(x) in
        if ok then Some v else None
(* avoid ;; -- to avoid fsi error recovery *)    

(* Bug 1143: are generated signatures correct for these? *)
let fA<'a> = printf "\nRUN at %s\n" (typeof<'a>).FullName;12;;
let fB<'a> (x:int) = printf "\nRUN at %s\n" (typeof<'a>).FullName;12;;
let fC<'a,'b> (x:'a) = printf "\nRUN at %s\n" (typeof<'a>).FullName;12;;
          
let _ =
  (* May not be enough if fsi does error recovery...
   * So avoid using ;;
   *)
  System.Console.WriteLine "Test Passed"; 
  printf "TEST PASSED OK"; 
  exit 0
