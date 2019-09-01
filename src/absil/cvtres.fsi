module internal FSharp.Compiler.AbstractIL.Internal.CVTres

open System
open System.IO

type BYTE = System.Byte
type DWORD = System.UInt32
type WCHAR = System.Char
type WORD = System.UInt16

[<Class>]
type RESOURCE_STRING =
    member Ordinal: WORD with get, set
    member theString : string with get, set

[<Class>]
type RESOURCE =
    member pstringType : RESOURCE_STRING with get, set
    member pstringName : RESOURCE_STRING with get, set
    member DataSize : DWORD with get, set
    member HeaderSize : DWORD with get, set
    member DataVersion : DWORD with get, set
    member MemoryFlags : WORD with get, set
    member LanguageId : WORD with get, set
    member Version : DWORD with get, set
    member Characteristics : DWORD with get, set
    member data : byte[] with get, set

[<Class>]
type CvtResFile =
    static member ReadResFile : stream:Stream -> System.Collections.Generic.List<RESOURCE>

[<Class>]
type Win32ResourceConversions =
    static member AppendIconToResourceStream : resStream:Stream * iconStream:Stream -> unit
    static member AppendVersionToResourceStream : resStream:Stream * isDll:System.Boolean * fileVersion:string * originalFileName:string * internalName:string * productVersion:string * assemblyVersion:Version * ?fileDescription:string * ?legalCopyright:string * ?legalTrademarks:string * ?productName:string * ?comments:string * ?companyName:string -> unit 
    static member AppendManifestToResourceStream : resStream:Stream * manifestStream:Stream * isDll:System.Boolean -> unit 
