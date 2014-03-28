// portable test code will use these replacements for APIs which don't exist on the portable profile
module Hooks

let private notImpl() = failwith "This callback has not been hooked by a test driver"

// System.Console
let mutable write = System.Action<string>( fun _ -> notImpl() )

// System.Environment
let mutable getEnvironmentVariable = System.Func<string, string>( fun _ -> notImpl())
let mutable majorVersion = System.Func<int>( fun () -> notImpl() )
let mutable minorVersion = System.Func<int>( fun () -> notImpl() )

// System.IO.Directory
let mutable getFiles = System.Func<string, string, string[]>( fun _ _ -> notImpl() )
let mutable getDirectories = System.Func<string, string[]>( fun _ -> notImpl() )
let mutable directoryExists = System.Func<string, bool>( fun _ -> notImpl() )

// System.IO.File
let mutable writeAllText = System.Action<string, string>( fun _ _ -> notImpl() )
let mutable writeAllLines = System.Action<string, string[]>( fun _ _ -> notImpl() )
let mutable appendAllText = System.Action<string, string>( fun _ _ -> notImpl() )
let mutable readAllLines = System.Func<string, string[]>( fun _ -> notImpl() )

// System.IO.FileStream
let mutable getFileStream = System.Func<string, System.IO.Stream> ( fun _ -> notImpl() )

// System.IO.Path
let mutable getCurrentDirectory = System.Func<string>( fun () -> notImpl() )
let mutable getDirectoryName = System.Func<string, string>( fun _ -> notImpl() )
let mutable getFileName = System.Func<string, string>( fun _ -> notImpl() )

// System.Threading.Thread
let mutable sleep = System.Action<int>( fun _ -> notImpl() )