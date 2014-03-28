// #Regression #Regression 
#indent "off"

// Basic Machine Learning tools v20060201a DBW
// - Support for concurrent channels


open System
open System.Threading
open JSTM
open DBWchan

type NNdata = float list

let doVerbose		= ref true
let doDBG			= ref true					// Verbose debugging output
let tstCntr			= {v=0}

let doWork numSum inputs	=					// Dummy worker for the work thread
	let sum			= ref 0.0 in
		for i = 1 to numSum do
			sum := List.fold (fun accum vec -> List.fold (+) accum vec) !sum inputs
		done;
		!sum

let wrkThrd (id:int) numIter numSum (mchn : _ MChan) =
	let	name		= String.Format("W{0,2}",id) in
	let v			= ref [[0.0]] in
		if !doVerbose then
			Console.WriteLine("doWork {0}: [{1}]",name,mchn.rcvChn.GetHashCode());
		for i=1 to numIter do
			if !doVerbose then Console.WriteLine("{0}: <?? inps...",name);
			txn(fun() -> v := mchn.rcv());
			if !doVerbose then Console.WriteLine("{0}: <-- inps...",name);
			let sum = doWork numSum !v in
				if !doVerbose then Console.WriteLine("{0}: ??> outs...",name);
				txn(fun() -> mchn.snd (List.head !v));
				if !doVerbose then Console.WriteLine("{0}: --> outs...",name)
		done

[<EntryPoint>]
let main argv =
	let	args		= Array.length argv in
	let numThrds	= if args > 1 then Convert.ToInt32(argv.[1]) else 4 in
	let numIter		= if args > 2 then Convert.ToInt32(argv.[2]) else 100 in
	let numSum		= if args > 3 then Convert.ToInt32(argv.[3]) else 50000 in
	let verbose		= if args > 4 then Convert.ToInt32(argv.[4]) else 0 in
	
	let data		= List.init 10 (fun i -> float i) in
	let protoChan	= new Chan<NNdata>(1,data) in
	let topChan		= new Chan<NNdata>(numThrds,data) in
	let topMChan	= new MChan<NNdata>(topChan) in
	let workers		= wrapChans (List.init numThrds (fun i -> new Chan<NNdata>(protoChan))) in
	let cntr		= ref 999 in
	let prevCntr	= ref !cntr in
	let	start		= System.DateTime.Now in
		
	Console.WriteLine("ML2 Thrd/{0} Iter/{1} Sum/{2} Verb/{3}",
		numThrds,numIter,numSum,verbose);
	
	//doVerbose	:= if verbose = 0 then false else true;
	
	// Link up the workers
	linkMChan	topMChan	workers;
	linkMChans	workers		[topMChan];
	
	// Create threads
	initThreads(numThrds);
	List.iteri (fun i w -> ignore (spawnCntr (lazy (wrkThrd i numIter numSum w)) tstCntr)) workers;

	let v		= ref [data] in
		for i=1 to numIter do
			txn (fun() -> topMChan.snd data);
			if !doVerbose then Console.WriteLine("top  --> outs...");
			txn (fun() -> v	:= topMChan.rcv());
			if !doVerbose then Console.WriteLine("top  <-- inps...");			
			
			txn(fun() -> cntr := getTVar tstCntr);
			if !cntr <> !prevCntr then
				Console.WriteLine("... Thread Counter: {0}",(prevCntr := !cntr;!prevCntr));
		done;
	
	let diff	= System.DateTime.Now.Subtract(start) in
  Console.Write("Total={0,5:f2} secs, thrds={1,2} iters={2} sums={3}",
	diff.TotalSeconds,numThrds,numIter,numSum);
	Console.WriteLine(" Work={0}",numThrds*numIter*numSum);
	System.IO.File.WriteAllText("test.ok","ok"); 
  0
