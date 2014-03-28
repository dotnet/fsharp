#indent "off"

module DBWchan

open System
open System.Threading
open JSTM

////////////////////////////////// List functions ////////////////////////////////

// Filter a list along with a supplied index value and return a new list
// syntax:		filteri (fun idx val -> bool) [list to filter]
// example:		filteri (fun i j -> i % 2 <> 0) [0;1;2;3;4;5;6;7]
//		creates a new list with all the even elements
val filteri: (int -> 'a -> bool) -> 'a list -> 'a list

///////////////////////////////// Array functions ///////////////////////////////

val iter_and_set: ('a -> 'a) -> 'a array -> unit			// Iterate a function over an array and set values

//////////////////////////// TVar: Transaction Variables /////////////////////////////////
[<ReferenceEquality>]
type 'a TVar				= {mutable v: 'a} with
	interface IUndoable
end
	
val getTVar: 'a TVar -> 'a							// Get a TVar within a TXN
val setTVar: 'a TVar -> 'a -> unit					// Set a TVar within a TXN

////////////////////// MVar: SingleCast/SingleValue Channels /////////////////////////////

type 'a MVar	= ('a option) TVar					// Single valued transacted channel

val newMVar: 'a -> 'a MVar							// Create a channel (not in a TXN)
val rcvMVar: 'a option TVar -> 'a					// Receive a value or block (in a TXN)
val sndMVar: 'a option TVar -> 'a option -> unit	// Send a value or block (in a TXN)

//////////// Chan: MultiCast/MultiValue/Bounded Channels  /////////////////////

type 'a Port = class								// Multicast read port
	new: 'a Chan -> 'a Port							// Channel written to
	member rcv: unit -> 'a							// Receive a value or block in a TXN
end

and 'a Chan = class									// Multicast write channel
	val dummy: 'a;									// initial dummy value
	new: int * 'a -> 'a Chan						// Create the channel (not in a TXN)
	new: 'a Chan -> 'a Chan							// Create the channel (not in a TXN)
	member snd: 'a -> unit							// Send a value or block if full
	member rcv: unit -> 'a							// Receive a value (if no ports (no multicast))
end

////////////////////////////// MultiChannels //////////////////////////////////////////////

type 'a MChan = class									// Multi Channel (for NN implementation)
	val rcvChn: 'a Chan;								// Channel to receive N messages on
	new: 'a Chan -> 'a MChan							// Gather channel
	member snd: 'a -> unit								// Send to all channels (inside a TXN)
	member rcv: unit -> 'a list							// Receive from all channels (inside a TXN)
end

val wrapChan: 'a Chan -> 'a MChan list					// Wrap a channel in a multi channel list
val wrapChans: 'a Chan list -> 'a MChan list			// wrap a list of channels in multi channels

val linkMChan: 'a MChan -> 'a MChan list -> unit			// Link an MChan and an MChan list
val linkMChans: 'a MChan list -> 'a MChan list -> unit		// Link two MChan lists

///////////////////////////// Thread stuff ////////////////////////////////////////////////

val initThreads: int -> unit						// Set default number of threads to use

// 1. Create a delegate of arg1 (lazy unit function) and execute it asynchronusly
// 2. Callback arg2 when arg1 completes
// 3. Return IAsyncResult immediately so async status can be monitored/controlled
//
// Example:
//
// let ar = spawn (lazy (foo a b)) (fun a -> ()) in
//    while ar.IsCompleted = false do
//	    ...

val spawn: Lazy<'a> -> ('a -> unit) -> IAsyncResult	// Spawn an asynchronus function

val txn: (unit -> unit) -> unit						// Group operations inside a transaction

val spawnCntr: Lazy<unit> -> TVar<int> -> IAsyncResult	// Spawn an async function that incrs a counter at start and decrs at stop

