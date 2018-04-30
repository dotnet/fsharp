#indent "off"

module Misc

open System
open System.Diagnostics
open System.Reflection

/// Write event entry to Windows Event Log
let WriteEvent (source:string) (message:string) (entryType:EventLogEntryType) =
	try 
		let _ = if not (EventLog.SourceExists(source)) then EventLog.CreateEventSource(source, "Application") else () in
		EventLog.WriteEntry(source, message, entryType)				
	with _ -> ()

/// Get exception message details
let rec GetExceptionMessage (e:#Exception) = 
	(e.GetType()).Name + ": " + e.Message + "\r\n StackTrace: " + e.StackTrace + 
		if e.InnerException = null then "" else "Inner Exception: " + (GetExceptionMessage e.InnerException)
	
/// Log exception message	
let LogWarning e =
	let message = GetExceptionMessage e in 
		Debug.WriteLine(message); WriteEvent e.Source message EventLogEntryType.Warning
 	
/// Async call abstraction
let AsyncApp input f g  =
	let fd = new Converter<_,_>(f) in
	let cb = new AsyncCallback(fun ar -> g (fd.EndInvoke(ar))) in
	fd.BeginInvoke(input, cb, (null : Object) ) |> ignore 	
