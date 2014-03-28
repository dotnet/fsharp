// #Regression #Regression 
#indent "off"

// Basic Machine Learning tools v20060201a DBW
// - Support for concurrent channels

module DBWchan

open System
open System.Threading
open JSTM

let doDBG		= ref false					// Verbose debugging output

////////////////////////////////// List functions ////////////////////////////////

// Filter a list along with a supplied index value and return a new list
// syntax:		filteri (fun idx val -> bool) [list to filter]
// example:		filteri (fun i j -> i % 2 <> 0) [0;1;2;3;4;5;6;7]
//		creates a new list with all the even elements
let filteri (f : int -> 'a -> bool) (src : 'a list) = 
	let len			= List.length src in
	let rec filteri2 f2 src2 i =
		if i = len then [] else match src2 with
			  []	-> []
			| h::t	-> if f2 i h then h::(filteri2 f2 t (i+1)) else filteri2 f2 t (i+1)
	in
		filteri2 f src 0

///////////////////////////////// Array functions ///////////////////////////////

let iter_and_set f (t: _[]) 		=								// Iterate a function over an array and set values
	Array.iteri (fun i -> fun x -> t.[i] <- f x) t



//////////////////////////// TVar: Transaction Variables /////////////////////////////////

exception BadTxn of string									// Txn exceptions

let retryRaise () 				=							// Abort Txn and try again
	Tx.Retry();
	raise(BadTxn("Read across empty barrier"))				// We should NEVER get here

[<ReferenceEquality>]
type 'a TVar 				= {mutable v: 'a} with			// Txn protected storage
interface JSTM.IUndoable with
		member x.Save () 	= 
			let sav 		= {v=x.v} in box sav.v
		member x.Undo (o : obj) 
							= x.v <- unbox o
	end
end

let getTVar tv 				=								// Get a TVar within a txn
	Tx.Current.OpenRead(tv);
	tv.v

let setTVar tv v 			=								// Set a TVar within a txn
	Tx.Current.OpenWrite(tv);
	tv.v					<-	v

////////////////////// MVar: SingleCast/SingleValue Channels /////////////////////////////

type 'a MVar				= ('a option) TVar				// Single place transacted channel

let newMVar (data : 'a) 	=								// Create a channel (not under a TXN)
	let item : 'a option	= None in					// Create a new empty item for the channel to hold
	let chan : 'a MVar		= {v=item} in					// and place it in the channel
		chan												// return the channel

let rcvMVar chan			=								// Take a transacted value from a channel
	let item				= getTVar chan in
		match item with
			  None			-> 	retryRaise ()				// Nothing there, wait for it to fill
			| Some data		-> 	setTVar chan None; 			// Empty the data in the channel
							data							//  and return it

let sndMVar chan data		=								// Put a transacted value on a channel
	let item				= getTVar chan in
		match item with
			  None			-> 	setTVar chan data			// No data, so safe to put a new value
			| Some data		-> 	retryRaise ()				// Something there, have to wait for it to empty
	
//////////// Chan: MultiCast/MultiValue/Bounded Channels  /////////////////////

let newTVary len value	=									// Create an array of inidividual TVars
	let a = Array.create len {v=value} in
		iter_and_set (fun(x) -> {v=value}) a;
		a

type 'a Port				= class							// Read port for a channel
	val mutable idx	: int;									// Index to read from for this port (local "head")
	val chan		: 'a Chan;
	new(chn: 'a Chan)	= {											// Create a new read port (from inside a TXN)
		chan=chn;
		idx=				
			let ports = getTVar chn.ports  in
				setTVar chn.ports (ports+1);
				getTVar chn.tail							// Start after last item in queue
	}
	member x.rcv ()			=								// Read a value off the Chan (within a TXN)
		let chan			= x.chan in
		let tail			= getTVar chan.tail in
			if x.idx = tail then							// Queue empty? 
				retryRaise ()
			else
				let value	= getTVar chan.data.[x.idx] in
				let rcvs	= getTVar chan.rcvs.[x.idx] - 1 in
				let newIdx	= (x.idx+1) % chan.len in
					if rcvs = 0 then						// We were the last rcvr, free the item in the channel
						setTVar chan.head newIdx
					else
						setTVar chan.rcvs.[x.idx] rcvs;		// More rcvrs, decrement the count
					x.idx <- newIdx;						// We move to the next item no matter what
					value
end

// Create a new queue with: new Chan<type>(maxDepth,dummyValue)
and	 'a Chan 				= class							// Channel to write on
	val head		: int TVar;								// index of first element in the queue (rcv incrs)
	val tail		: int TVar;								// index of last element in the queue (snd incrs)
	val ports		: int TVar;								// Total number of read ports on this queue
	val len			: int;									// Max length of queue
	val rcvs		: (int TVar) array;						// Number of reads left on this item
	val data		: ('a TVar) array;						// data storage
	val dummy		: 'a;									// initial dummy value
	new(maxDepth,value)		= {								// Constructor
		dummy=value;
		head={v=0};
		tail={v=0}; 
		ports={v=0};
		len=maxDepth+1;										// Extry entry for 1 entry channels to work
		rcvs=newTVary (maxDepth+1) 0;
		data=newTVary (maxDepth+1) value;
	}
	new(baseChan : 'a Chan)		= {								// Create from a prototype
		dummy=baseChan.dummy;
		head={v=0};
		tail={v=0}; 
		ports={v=0};
		len=baseChan.len;
		rcvs=newTVary baseChan.len 0;
		data=newTVary baseChan.len baseChan.dummy;
	}
	member x.snd value		=								// Send a value on a channel
		let head			= getTVar x.head in
		let tail			= getTVar x.tail in
		let newTl			= (tail + 1) % x.len in
			if !doDBG then Console.WriteLine("DBG Snd0 newTl:{0} head:{1} [{2}]",newTl,head,x.GetHashCode());
			if head = newTl then							// Queue is full
				retryRaise ()
			else
				let ports	= getTVar x.ports in
					if !doDBG then Console.WriteLine("DBG Snd1 value:{0} tail:{1} [{2}]",value,tail,x.GetHashCode());
					setTVar x.data.[tail] value;
					setTVar x.rcvs.[tail] ports;
					setTVar x.tail newTl;
					if !doDBG then Console.WriteLine("DBG Snd2 value:{0} tail:{1} [{2}]",value,tail,x.GetHashCode())
	member x.rcv ()			=								// Read a value off the Chan (within a TXN)
		let ports			= getTVar x.ports in			// Make sure that there is no multi-cast
			if ports > 0 then
				raise(BadTxn("Tried to do a channel read when there were multi-cast ports!"))
			else
				let tail	= getTVar x.tail in
				let head	= getTVar x.head in
					if !doDBG then Console.WriteLine("DBG Rcv0 tail:{0} head:{1} [{2}]",tail,head,x.GetHashCode());
					if head = tail then						// Queue empty? 
						retryRaise ()
					else
						let value	= getTVar x.data.[head] in
						let newIdx	= (head+1) % x.len in
							if !doDBG then Console.WriteLine("DBG Rcv1 value:{0} newIdx:{1} [{2}]",value,newIdx,x.GetHashCode());
							setTVar x.head newIdx;
							if !doDBG then Console.WriteLine("DBG Rcv2 value:{0} newIdx:{1} [{2}]",value,newIdx,x.GetHashCode());
							value
end

////////////////////////////// MultiChannels //////////////////////////////////////////////

type 'a MChan				= class						// Multi Channel (for NN implementation)
	val mutable rcvLen	: int;							// Number of messages to recv (gather)
	val	mutable sndLst	: 'a MChan list;				// List of N channels to send to
	val rcvChn			: 'a Chan;						// Channel to receive N messages on
	new(rcvChan) = {									// Gather channel
		rcvLen=0;
		rcvChn=rcvChan;
		sndLst=[];
	}
	member x.snd value		=							// Send to all channels (inside a TXN)
		if !doDBG then Console.WriteLine("DBG MChan snd to {0} channels",List.length x.sndLst);
		List.iter (fun (mchn : 'a MChan) -> 
			mchn.rcvChn.snd value;
			if !doDBG then Console.WriteLine("DBG MChan snd {0} to [{1}]",value,mchn.rcvChn.GetHashCode())
			) x.sndLst

	member x.rcv ()			=							// Receive from all channels (inside a TXN)
		List.init x.rcvLen (fun i -> x.rcvChn.rcv())
end

let wrapChan (chan : 'a Chan) =							// Wrap a channel in a multi channel list
	let mchn = new MChan<_>(chan) in
		[mchn]

let wrapChans (chans : 'a Chan list)	=				// wrap a list of channels in multi channels
	List.map (fun chn -> new MChan<_>(chn)) chans

let linkMChan (src : 'a MChan) dstLst =								// Link an MChan to a list of MChans
	src.sndLst				<- List.append src.sndLst dstLst;
	List.iter (fun (dst : _ MChan) -> dst.rcvLen <- dst.rcvLen + 1) dstLst

let linkMChans (srcLst:MChan<_> list) (dstLst:MChan<_> list) =							// Link two MChan lists
	List.iter (fun src -> linkMChan src dstLst) srcLst

///////////////////////////// Thread stuff ////////////////////////////////////////////////

let initThreads min =									// Set default number of threads to use
	ignore(ThreadPool.SetMinThreads(min,2))

// spawn: ('a lazy) -> ('a -> unit) -> AsyncResult
let spawn (x: 'a Lazy) g 				=							// Spawn x asyncronusly and call g when completed
  let fd 					= new Converter<_,_>(fun () -> x.Force()) in
  let cb 					= AsyncCallback(fun ar -> g (fd.EndInvoke(ar))) in
	fd.BeginInvoke((), cb, (null : Object))

let txn (x : (unit -> unit))	=							// Group lazy operations under a TXN
	Tx.Atomically(Tx.D x)

let spawnCntr (x: Lazy<_>) (ctr : int TVar) =						// Spawn x asyncronusly and update a ref TVar counter of running threads
	let fd					= new Converter<_,_>(fun() ->
			txn (fun() ->
				setTVar ctr (getTVar ctr + 1)
			);
			x.Force()) in
	let cb					= AsyncCallback(fun ar -> 
			fd.EndInvoke(ar);
			txn (fun() ->
				setTVar ctr (getTVar ctr - 1)
			)) in
		fd.BeginInvoke((), cb, (null : Object))	

