// driver program can provide hooks with full implementations
module InitialHook

// System.Console
let setWrite (f : System.Action<string>) = Hooks.write <- f

// System.Environment
let setGetEnvironmentVariable (f : System.Func<string, string>) = Hooks.getEnvironmentVariable <- f
let setMajorVersion (f : System.Func<int>) = Hooks.majorVersion <- f
let setMinorVersion (f : System.Func<int>) = Hooks.minorVersion <- f

// System.IO.Directory
let setGetFiles (f : System.Func<string, string, string[]>) = Hooks.getFiles <- f
let setGetDirectories (f : System.Func<string, string[]>) = Hooks.getDirectories <- f
let setDirectoryExists(f : System.Func<string, bool>) = Hooks.directoryExists <- f

// System.IO.File
let setWriteAllText (f : System.Action<string, string>) = Hooks.writeAllText <- f
let setWriteAllLines (f : System.Action<string, string[]>) = Hooks.writeAllLines <- f
let setAppendAllText (f : System.Action<string, string>) = Hooks.appendAllText <- f
let setReadAllLines(f: System.Func<string, string[]>) = Hooks.readAllLines <- f

// System.IO.FileStream
let setGetFileStream(f : System.Func<string, System.IO.Stream>) = Hooks.getFileStream <- f

// System.IO.Path
let setGetCurrentDirectory (f : System.Func<string>) = Hooks.getCurrentDirectory <- f
let setGetDirectoryName (f : System.Func<string, string>) = Hooks.getDirectoryName <- f
let setGetFileName (f : System.Func<string, string>) = Hooks.getFileName <- f

// System.Threading.Thread
let setSleep(f : System.Action<int>) = Hooks.sleep <- f