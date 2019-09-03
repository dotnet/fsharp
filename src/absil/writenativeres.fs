// Quite literal port of https://github.com/dotnet/roslyn/blob/fab7134296816fc80019c60b0f5bef7400cf23ea/src/Compilers/Core/Portable/PEWriter/NativeResourceWriter.cs
// And its dependencies (some classes)
module internal FSharp.Compiler.AbstractIL.Internal.WriteNativeRes

open System
open System.Collections.Generic
open System.Linq
open System.Diagnostics
open System.IO
open System.Reflection.Metadata
//open Roslyn.Utilities;

type DWORD = System.UInt32

type Win32Resource(data : byte[], codePage : DWORD, languageId : DWORD, id : int, name : string, typeId : int, typeName : string) =
    member val Data = data
    member val CodePage = codePage
    member val LanguageId = languageId
    member val Id = id
    member val Name = name
    member val TypeId = typeId
    member val TypeName = typeName

type Directory(name, id) =
    member val Name = name
    member val ID = id
    member val NumberOfNamedEntries = Unchecked.defaultof<System.UInt16> with get, set
    member val NumberOfIdEntries = Unchecked.defaultof<System.UInt16> with get, set
    member val Entries = new List<System.Object>()

type NativeResourceWriter() =
    static member private CompareResources (left : Win32Resource) (right : Win32Resource) = 
        let mutable (result : int) = NativeResourceWriter.CompareResourceIdentifiers (left.TypeId, left.TypeName, right.TypeId, right.TypeName)
        if result = 0
        then NativeResourceWriter.CompareResourceIdentifiers (left.Id, left.Name, right.Id, right.Name)
        else result
    static member private CompareResourceIdentifiers(xOrdinal : int, xString : string, yOrdinal : int, yString : string) = 
        if xString = Unchecked.defaultof<_>
        then 
            if yString = Unchecked.defaultof<_>
            then xOrdinal - yOrdinal
            else 1
        else 
            if yString = Unchecked.defaultof<_>
            then - 1
            else String.Compare (xString, yString, StringComparison.OrdinalIgnoreCase)
    static member SortResources(resources : IEnumerable<Win32Resource>) = 
        resources.OrderBy ((fun d -> d), Comparer<_>.Create(Comparison<_> NativeResourceWriter.CompareResources)) :> IEnumerable<Win32Resource>
    static member SerializeWin32Resources(builder : BlobBuilder, theResources : IEnumerable<Win32Resource>, resourcesRva : int) = 
        let theResources = NativeResourceWriter.SortResources (theResources)
        let mutable (typeDirectory : Directory) = new Directory(String.Empty, 0)
        let mutable (nameDirectory : Directory) = Unchecked.defaultof<_>
        let mutable (languageDirectory : Directory) = Unchecked.defaultof<_>
        let mutable (lastTypeID : int) = Int32.MinValue
        let mutable (lastTypeName : string) = Unchecked.defaultof<_>
        let mutable (lastID : int) = Int32.MinValue
        let mutable (lastName : string) = Unchecked.defaultof<_>
        let mutable (sizeOfDirectoryTree : System.UInt32) = 16u
        for (r : Win32Resource) in theResources do
            let mutable (typeDifferent : System.Boolean) = r.TypeId < 0 && r.TypeName <> lastTypeName || r.TypeId > lastTypeID
            if typeDifferent
            then 
                lastTypeID <- r.TypeId
                lastTypeName <- r.TypeName
                if lastTypeID < 0
                then 
                    Debug.Assert ((typeDirectory.NumberOfIdEntries = 0us), "Not all Win32 resources with types encoded as strings precede those encoded as ints")
                    typeDirectory.NumberOfNamedEntries <- typeDirectory.NumberOfNamedEntries + 1us
                else 
                    typeDirectory.NumberOfIdEntries <- typeDirectory.NumberOfIdEntries + 1us
                sizeOfDirectoryTree <- sizeOfDirectoryTree + 24u
                nameDirectory <- new Directory(lastTypeName, lastTypeID)
                typeDirectory.Entries.Add (nameDirectory)
            if typeDifferent || r.Id < 0 && r.Name <> lastName || r.Id > lastID
            then 
                lastID <- r.Id
                lastName <- r.Name
                if lastID < 0
                then 
                    Debug.Assert ((nameDirectory.NumberOfIdEntries = 0us), "Not all Win32 resources with names encoded as strings precede those encoded as ints")
                    nameDirectory.NumberOfNamedEntries <- nameDirectory.NumberOfNamedEntries + 1us
                else 
                    nameDirectory.NumberOfIdEntries <- nameDirectory.NumberOfIdEntries + 1us
                sizeOfDirectoryTree <- sizeOfDirectoryTree + 24u
                languageDirectory <- new Directory(lastName, lastID)
                nameDirectory.Entries.Add (languageDirectory)
            languageDirectory.NumberOfIdEntries <- languageDirectory.NumberOfIdEntries + 1us
            sizeOfDirectoryTree <- sizeOfDirectoryTree + 8u
            languageDirectory.Entries.Add (r)
        let mutable dataWriter = new BlobBuilder()
        NativeResourceWriter.WriteDirectory (typeDirectory, builder, (0u), (0u), sizeOfDirectoryTree, resourcesRva, dataWriter)
        builder.LinkSuffix (dataWriter)
        builder.WriteByte (0uy)
        builder.Align (4)
    static member private WriteDirectory(directory : Directory, writer : BlobBuilder, offset : System.UInt32, level : System.UInt32, sizeOfDirectoryTree : System.UInt32, virtualAddressBase : int, dataWriter : BlobBuilder) = 
        writer.WriteUInt32 (0u)
        writer.WriteUInt32 (0u)
        writer.WriteUInt32 (0u)
        writer.WriteUInt16 (directory.NumberOfNamedEntries)
        writer.WriteUInt16 (directory.NumberOfIdEntries)
        let mutable (n : System.UInt32) = uint32 directory.Entries.Count
        let mutable (k : System.UInt32) = offset + 16u + n * 8u
        do 
            let mutable (i : uint32) = 0u
            while (i < n) do
                let mutable (id : int) = Unchecked.defaultof<int>
                let mutable (name : string) = Unchecked.defaultof<string>
                let mutable (nameOffset : System.UInt32) = uint32 dataWriter.Count + sizeOfDirectoryTree
                let mutable (directoryOffset : System.UInt32) = k
                let isDir =
                    match directory.Entries.[int i] with
                    | :? Directory as subDir ->
                        id <- subDir.ID
                        name <- subDir.Name
                        if level = 0u
                        then k <- k + NativeResourceWriter.SizeOfDirectory (subDir)
                        else k <- k + 16u + 8u * uint32 subDir.Entries.Count
                        true
                    | :? Win32Resource as r ->
                        id <- 
                            if level = 0u
                            then r.TypeId
                            else 
                                if level = 1u
                                then r.Id
                                else int r.LanguageId
                        name <- 
                            if level = 0u
                            then r.TypeName
                            else 
                                if level = 1u
                                then r.Name
                                else Unchecked.defaultof<_>
                        dataWriter.WriteUInt32 ((uint32 virtualAddressBase + sizeOfDirectoryTree + 16u + uint32 dataWriter.Count))
                        let mutable (data : byte[]) = (new List<System.Byte>(r.Data)).ToArray ()
                        dataWriter.WriteUInt32 (uint32 data.Length)
                        dataWriter.WriteUInt32 (r.CodePage)
                        dataWriter.WriteUInt32 (0u)
                        dataWriter.WriteBytes (data)
                        while (dataWriter.Count % 4 <> 0) do
                            dataWriter.WriteByte (0uy)
                        false
                    | e -> failwithf "Unknown entry %s" (if isNull e then "<NULL>" else e.GetType().FullName)
                if id >= 0
                then writer.WriteInt32 (id)
                else 
                    if name = Unchecked.defaultof<_>
                    then name <- String.Empty
                    writer.WriteUInt32 (nameOffset ||| 0x80000000u)
                    dataWriter.WriteUInt16 (uint16 name.Length)
                    dataWriter.WriteUTF16 (name)
                if isDir
                then writer.WriteUInt32 (directoryOffset ||| 0x80000000u)
                else writer.WriteUInt32 (nameOffset)
                i <- i + 1u

        k <- offset + 16u + n * 8u
        do 
            let mutable (i : int) = 0
            while (uint32 i < n) do
                match directory.Entries.[i] with
                | :? Directory as subDir ->
                    NativeResourceWriter.WriteDirectory (subDir, writer, k, (level + 1u), sizeOfDirectoryTree, virtualAddressBase, dataWriter)
                    if level = 0u
                    then k <- k + NativeResourceWriter.SizeOfDirectory (subDir)
                    else k <- k + 16u + 8u * uint32 subDir.Entries.Count
                | _ -> ()
                i <- i + 1
        ()
    static member private SizeOfDirectory(directory : Directory) = 
        let mutable (n : System.UInt32) = uint32 directory.Entries.Count
        let mutable (size : System.UInt32) = 16u + 8u * n
        do 
            let mutable (i : int) = 0
            while (uint32 i < n) do
                match directory.Entries.[i] with
                | :? Directory as subDir ->
                    size <- size + 16u + 8u * uint32 subDir.Entries.Count
                | _ -> ()
                i <- i + 1
        size
    (*
    static member SerializeWin32Resources(builder : BlobBuilder, resourceSections : ResourceSection, resourcesRva : int) = 
        let mutable sectionWriter = new BlobWriter(builder.ReserveBytes (resourceSections.SectionBytes.Length))
        sectionWriter.WriteBytes (resourceSections.SectionBytes)
        let mutable readStream = new MemoryStream(resourceSections.SectionBytes)
        let mutable reader = new BinaryReader(readStream)
        for (addressToFixup : int) in resourceSections.Relocations do
            sectionWriter.Offset <- addressToFixup
            reader.BaseStream.Position <- addressToFixup
            sectionWriter.WriteUInt32 (reader.ReadUInt32 () + resourcesRva :> System.UInt32)
        ()*)