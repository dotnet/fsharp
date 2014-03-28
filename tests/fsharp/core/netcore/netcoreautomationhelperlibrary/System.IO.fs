// replacements for classes which are entirely removed from the portable profile
// either calls out to hooks, or implements functionality locally for simple cases

namespace System.IO

type File() =
    static member ReadAllLines (path) =
        Hooks.readAllLines.Invoke(path)

    static member ReadAllText (path) =
        System.String.Join("\r\n",  Hooks.readAllLines.Invoke(path))

    static member WriteAllText (path, content) = 
        Hooks.writeAllText.Invoke(path, content)

    static member WriteAllLines (path, content) = 
        Hooks.writeAllLines.Invoke(path, content)

    static member AppendAllText(path, content) = 
        Hooks.appendAllText.Invoke(path, content)

    static member CreateText(path) =
        Hooks.getFileStream.Invoke(path)

    static member OpenText(path) =
        new System.IO.StreamReader(Hooks.getFileStream.Invoke(path))

type FileShare = 
    | None = 0
    | Read = 1
    | Write = 2
    | ReadWrite = 3
    | Delete  = 4
    | Inheritable = 16

type Directory() =
    static member Exists (dir) =
        Hooks.directoryExists.Invoke(dir)

    static member GetFiles (dir) =
        Hooks.getFiles.Invoke(dir, "*")

    static member GetFiles (dir, pattern) =
        Hooks.getFiles.Invoke(dir, pattern)

    static member GetDirectories (dir) =
        Hooks.getDirectories.Invoke(dir)

type Path() =
    static member GetDirectoryName(path) = Hooks.getDirectoryName.Invoke(path)
    static member GetFileName(path) = Hooks.getFileName.Invoke(path)