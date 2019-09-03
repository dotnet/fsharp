
module internal FSharp.Compiler.AbstractIL.Internal.WriteNativeRes


open System
open System.Collections.Generic
open System.Linq
open System.Diagnostics
open System.IO
open System.Reflection.Metadata
//open Roslyn.Utilities;

type DWORD = System.UInt32

type Win32Resource =
  class
    new : data:byte [] * codePage:DWORD * languageId:DWORD * id:int *
          name:string * typeId:int * typeName:string -> Win32Resource
    member CodePage : DWORD
    member Data : byte []
    member Id : int
    member LanguageId : DWORD
    member Name : string
    member TypeId : int
    member TypeName : string
  end

[<Class>]
type NativeResourceWriter =
    static member SortResources : resources : IEnumerable<Win32Resource> -> IEnumerable<Win32Resource>
    static member SerializeWin32Resources : builder:BlobBuilder * theResources:IEnumerable<Win32Resource> * resourcesRva:int -> unit
    (*
    static member SerializeWin32Resources(builder : BlobBuilder, resourceSections : ResourceSection, resourcesRva : int) -> unit
        ()*)