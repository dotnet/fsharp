//==========================================================================
// The interface to the module 
// is similar to that found in versions of other ML implementations, 
// but is not an exact match.  The type signatures in this interface
// are an edited version of those generated automatically by running 
// "bin\fsc.exe -i" on the implementation file.
//===========================================================================

/// Common filename operations.  This module is included to make it possible to cross-compile 
/// code with other ML compilers.  See also <c>System.IO.Path</c>
[<CompilerMessage("This module is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
module Microsoft.FSharp.Compatibility.OCaml.Filename

/// "check_suffix f s" returns true if filename "f" ends in suffix "s",
/// e.g. check_suffix "abc.fs" ".fs" returns true.
val check_suffix: string -> string -> bool

/// "chop_extension f" removes the extension from the given
/// filename. Raises ArgumentException if no extension is present.
val chop_extension: string -> string

/// Assuming "check_suffix f s" holds, "chop_suffix f s" returns the
/// filename "f" with the suffix "s" removed.
val chop_suffix: string -> string -> string

/// "concat a b" returns System.IO.Path.Combine(a,b), i.e. the
/// two names conjoined by the appropriate directory separator character
/// for this architecture.
val concat: string -> string -> string

/// "dirname" and "basename" decompose a filename into a directory name
/// and a filename, i.e. "concat (dirname s) (basename s) = s"
val dirname: string -> string

/// "dirname" and "basename" decompose a filename into a directory name
/// and a filename, i.e. "concat (dirname s) (basename s) = s"
val basename: string -> string

/// The name used for the current directory on this OS. 
val current_dir_name: string

/// "parent_dir_name" returns the name for the directory above the current directory on
/// this OS.
val parent_dir_name: string

/// Return true if the filename has a "." extension
val has_extension: string -> bool

/// Is the path is relative to the current directory or absolute.
val is_relative: string -> bool

/// Returns true if the path is relative to the current directory but does not begin with 
/// an explicit "." or ".."
val is_implicit: string -> bool

/// "quote s" is designed for use to quote a filename when using it
/// for a system command.  It returns ("\'" ^ s ^ "\'").  
val quote: string -> string

/// "temp_file f s" returns a hitherto unused new file name.  "f" and "s"
/// are hints as to a suitable file name and suffix for the file.
val temp_file: string -> string -> string
