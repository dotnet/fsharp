// Quite literal port of https://github.com/dotnet/roslyn/blob/d36121da4b527ee0617e4b0940b9d0b17b584470/src/Compilers/Core/Portable/CvtRes.cs
// And its dependencies (some classes)
module internal FSharp.Compiler.AbstractIL.Internal.CVTres

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Text
open System.Diagnostics

type BYTE = System.Byte
type DWORD = System.UInt32
type WCHAR = System.Char
type WORD = System.UInt16

module CONVHELPER =
    let inline WORD s = uint16 s
    let inline DWORD s = uint32 s
    let inline WCHAR s = char s
    let inline BYTE s = byte s

open CONVHELPER

open Checked // make sure stuff works properly

open System.Reflection.PortableExecutable


type ResourceException(name : string, ?inner : Exception) =
    inherit Exception(name, defaultArg inner null)
    //ew(name : string, ?inner : Exception) as this = 
    //   (ResourceException ())
    //   then
    //       let inner = (defaultArg inner) Unchecked.defaultof<_>
    //       ()

type RESOURCE_STRING() =
    member val Ordinal = Unchecked.defaultof<WORD> with get, set
    member val theString = Unchecked.defaultof<string> with get, set
type RESOURCE() =
    member val pstringType = Unchecked.defaultof<RESOURCE_STRING> with get, set
    member val pstringName = Unchecked.defaultof<RESOURCE_STRING> with get, set
    member val DataSize = Unchecked.defaultof<DWORD> with get, set
    member val HeaderSize = Unchecked.defaultof<DWORD> with get, set
    member val DataVersion = Unchecked.defaultof<DWORD> with get, set
    member val MemoryFlags = Unchecked.defaultof<WORD> with get, set
    member val LanguageId = Unchecked.defaultof<WORD> with get, set
    member val Version = Unchecked.defaultof<DWORD> with get, set
    member val Characteristics = Unchecked.defaultof<DWORD> with get, set
    member val data = Unchecked.defaultof<byte[]> with get, set
type CvtResFile() =
    static member val private RT_DLGINCLUDE = 17 with get, set
    static member ReadResFile(stream : Stream) = 
        let mutable reader = new BinaryReader(stream, Encoding.Unicode)
        let mutable resourceNames = new List<RESOURCE>()
        // The stream might be empty ...  so lets check
        if not (reader.PeekChar() = -1) then
            let mutable startPos = stream.Position
            let mutable initial32Bits = reader.ReadUInt32 ()
            if initial32Bits <> uint32 0
            then raise <| ResourceException("Stream does not begin with a null resource and is not in .RES format.")
            stream.Position <- startPos
            while (stream.Position < stream.Length) do
                let mutable cbData = reader.ReadUInt32 ()
                let mutable cbHdr = reader.ReadUInt32 ()
                if cbHdr < 2u * uint32 sizeof<DWORD>
                then raise <| ResourceException(String.Format ("Resource header beginning at offset 0x{0:x} is malformed.", (stream.Position - 8L)))
                if cbData = 0u
                then 
                    stream.Position <- stream.Position + int64 cbHdr - 2L * int64 sizeof<DWORD>
                else            
                    let mutable pAdditional = RESOURCE() 
                    pAdditional.HeaderSize <- cbHdr
                    pAdditional.DataSize <- cbData
                    pAdditional.pstringType <- CvtResFile.ReadStringOrID (reader)
                    pAdditional.pstringName <- CvtResFile.ReadStringOrID (reader)
                    stream.Position <- stream.Position + 3L &&& ~~~3L
                    pAdditional.DataVersion <- reader.ReadUInt32 ()
                    pAdditional.MemoryFlags <- reader.ReadUInt16 ()
                    pAdditional.LanguageId <- reader.ReadUInt16 ()
                    pAdditional.Version <- reader.ReadUInt32 ()
                    pAdditional.Characteristics <- reader.ReadUInt32 ()
                    pAdditional.data <- Array.zeroCreate (int pAdditional.DataSize)
                    reader.Read (pAdditional.data, 0, pAdditional.data.Length) |> ignore<int>
                    stream.Position <- stream.Position + 3L &&& ~~~3L
                    if pAdditional.pstringType.theString = Unchecked.defaultof<_> && (pAdditional.pstringType.Ordinal = uint16 CvtResFile.RT_DLGINCLUDE)
                    then () (* ERROR ContinueNotSupported *)
                    else resourceNames.Add (pAdditional)
        resourceNames
    static member private ReadStringOrID(fhIn : BinaryReader) = 
        let mutable (pstring : RESOURCE_STRING) = RESOURCE_STRING()
        let mutable (firstWord : WCHAR) = (fhIn.ReadChar ())
        if int firstWord = 0xFFFF
        then pstring.Ordinal <- fhIn.ReadUInt16 ()
        else 
            pstring.Ordinal <- uint16 0xFFFF
            let mutable (sb : StringBuilder) = StringBuilder()
            let mutable (curChar : WCHAR) = firstWord
            while (curChar <> char 0) do
                sb.Append(curChar) |> ignore<StringBuilder>
                curChar <- fhIn.ReadChar()
            pstring.theString <- sb.ToString ()
        pstring


[<Flags>]
type SectionCharacteristics =
    | TypeReg = 0u
    | TypeDSect = 1u
    | TypeNoLoad = 2u
    | TypeGroup = 4u
    | TypeNoPad = 8u
    | TypeCopy = 16u
    | ContainsCode = 32u
    | ContainsInitializedData = 64u
    | ContainsUninitializedData = 128u
    | LinkerOther = 256u
    | LinkerInfo = 512u
    | TypeOver = 1024u
    | LinkerRemove = 2048u
    | LinkerComdat = 4096u
    | MemProtected = 16384u
    | NoDeferSpecExc = 16384u
    | GPRel = 32768u
    | MemFardata = 32768u
    | MemSysheap = 65536u
    | MemPurgeable = 131072u
    | Mem16Bit = 131072u
    | MemLocked = 262144u
    | MemPreload = 524288u
    | Align1Bytes = 1048576u
    | Align2Bytes = 2097152u
    | Align4Bytes = 3145728u
    | Align8Bytes = 4194304u
    | Align16Bytes = 5242880u
    | Align32Bytes = 6291456u
    | Align64Bytes = 7340032u
    | Align128Bytes = 8388608u
    | Align256Bytes = 9437184u
    | Align512Bytes = 10485760u
    | Align1024Bytes = 11534336u
    | Align2048Bytes = 12582912u
    | Align4096Bytes = 13631488u
    | Align8192Bytes = 14680064u
    | AlignMask = 15728640u
    | LinkerNRelocOvfl = 16777216u
    | MemDiscardable = 33554432u
    | MemNotCached = 67108864u
    | MemNotPaged = 134217728u
    | MemShared = 268435456u
    | MemExecute = 536870912u
    | MemRead = 1073741824u
    | MemWrite = 2147483648u

type ResourceSection() =
    new(sectionBytes : byte[], relocations : uint32[]) as this = 
        (ResourceSection ())
        then
            Debug.Assert (sectionBytes :> obj <> Unchecked.defaultof<_>)
            Debug.Assert (relocations :> obj <> Unchecked.defaultof<_>)
            this.SectionBytes <- sectionBytes
            this.Relocations <- relocations
    member val SectionBytes = Unchecked.defaultof<byte[]> with get,set
    member val Relocations = Unchecked.defaultof<uint32[]> with get,set
    
open System.Runtime.CompilerServices
[<Extension>]
type StreamExtensions () =
    [<Extension>]
    static member TryReadAll(stream : Stream, buffer : byte[], offset : int, count : int) = 
        Debug.Assert (count > 0)
        let mutable (totalBytesRead : int) = Unchecked.defaultof<int>
        let mutable (isFinished : bool) = false
        let mutable (bytesRead : int) = 0
        do 
            totalBytesRead <- 0
            while totalBytesRead < count && not isFinished do
                bytesRead <- stream.Read (buffer, (offset + totalBytesRead), (count - totalBytesRead))
                if bytesRead = 0
                then isFinished <- true // break;
                else totalBytesRead <- totalBytesRead + bytesRead
        totalBytesRead

type COFFResourceReader() =
    static member private ConfirmSectionValues(hdr : SectionHeader, fileSize : System.Int64) = 
        if int64 hdr.PointerToRawData + int64 hdr.SizeOfRawData > fileSize
        then raise <| ResourceException("CoffResourceInvalidSectionSize")
        ()
    static member ReadWin32ResourcesFromCOFF(stream : Stream) = 
        let mutable peHeaders = new PEHeaders(stream)
        let mutable rsrc1 = SectionHeader()
        let mutable rsrc2 = SectionHeader()
        let mutable (foundCount : int) = 0
        for sectionHeader in peHeaders.SectionHeaders do
            if sectionHeader.Name = ".rsrc$01"
            then 
                rsrc1 <- sectionHeader
                foundCount <- foundCount + 1
            else 
                if sectionHeader.Name = ".rsrc$02"
                then 
                    rsrc2 <- sectionHeader
                    foundCount <- foundCount + 1
        if foundCount <> 2
        then raise <| ResourceException("CoffResourceMissingSection")
        COFFResourceReader.ConfirmSectionValues (rsrc1, stream.Length)
        COFFResourceReader.ConfirmSectionValues (rsrc2, stream.Length)
        let mutable imageResourceSectionBytes = Array.zeroCreate (rsrc1.SizeOfRawData + rsrc2.SizeOfRawData)
        stream.Seek (int64 rsrc1.PointerToRawData, SeekOrigin.Begin) |> ignore<int64>
        stream.TryReadAll (imageResourceSectionBytes, 0, rsrc1.SizeOfRawData) |> ignore<int>
        stream.Seek (int64 rsrc2.PointerToRawData, SeekOrigin.Begin) |> ignore<int64>
        stream.TryReadAll (imageResourceSectionBytes, rsrc1.SizeOfRawData, rsrc2.SizeOfRawData) |> ignore<int>
        let mutable (SizeOfRelocationEntry : int) = 10
        try
            let mutable relocLastAddress = rsrc1.PointerToRelocations + (int rsrc1.NumberOfRelocations * SizeOfRelocationEntry)
            if int64 relocLastAddress > stream.Length
            then raise <| ResourceException("CoffResourceInvalidRelocation")
        with
            :? OverflowException -> (raise <| ResourceException("CoffResourceInvalidRelocation"))
        let mutable relocationOffsets = Array.zeroCreate (int rsrc1.NumberOfRelocations)
        let mutable relocationSymbolIndices = Array.zeroCreate (int rsrc1.NumberOfRelocations)
        let mutable reader = new BinaryReader(stream, Encoding.Unicode)
        stream.Position <- int64 rsrc1.PointerToRelocations
        do 
            let mutable (i : int) = 0
            while (i < int rsrc1.NumberOfRelocations) do
                relocationOffsets.[i] <- reader.ReadUInt32 ()
                relocationSymbolIndices.[i] <- reader.ReadUInt32 ()
                reader.ReadUInt16 () |> ignore<uint16> //we do nothing with the "Type"
                i <- i + 1
        stream.Position <- int64 peHeaders.CoffHeader.PointerToSymbolTable
        let mutable (ImageSizeOfSymbol : System.UInt32) = 18u
        try
            let mutable lastSymAddress = int64 peHeaders.CoffHeader.PointerToSymbolTable + int64 peHeaders.CoffHeader.NumberOfSymbols * int64 ImageSizeOfSymbol (* ERROR UnknownNode *)
            if lastSymAddress > stream.Length
            then raise <| ResourceException("CoffResourceInvalidSymbol")
        with
            :? OverflowException -> (raise <| ResourceException("CoffResourceInvalidSymbol"))
        let mutable outputStream = new MemoryStream(imageResourceSectionBytes)
        let mutable writer = new BinaryWriter(outputStream)
        do 
            let mutable (i : int) = 0
            while (i < relocationSymbolIndices.Length) do
                if int relocationSymbolIndices.[i] > peHeaders.CoffHeader.NumberOfSymbols
                then raise <| ResourceException("CoffResourceInvalidRelocation")
                let mutable offsetOfSymbol = int64 peHeaders.CoffHeader.PointerToSymbolTable + int64 relocationSymbolIndices.[i] * int64 ImageSizeOfSymbol
                stream.Position <- offsetOfSymbol
                stream.Position <- stream.Position + 8L
                let mutable symValue = reader.ReadUInt32 ()
                let mutable symSection = reader.ReadInt16 ()
                let mutable symType = reader.ReadUInt16 ()
                let mutable (IMAGE_SYM_TYPE_NULL : System.UInt16) = uint16 0x0000
                if symType <> IMAGE_SYM_TYPE_NULL || symSection <> 3s
                then raise <| ResourceException("CoffResourceInvalidSymbol")
                outputStream.Position <- int64 relocationOffsets.[i]
                writer.Write (uint32 (int64 symValue + int64 rsrc1.SizeOfRawData))
                i <- i + 1

        ResourceSection(imageResourceSectionBytes, relocationOffsets)

type ICONDIRENTRY =
    struct
        val mutable bWidth: BYTE
        val mutable bHeight: BYTE
        val mutable bColorCount: BYTE
        val mutable bReserved: BYTE
        val mutable wPlanes: WORD
        val mutable wBitCount: WORD
        val mutable dwBytesInRes: DWORD
        val mutable dwImageOffset: DWORD
    end

type VersionHelper() =
    /// <summary>
    /// Parses a version string of the form "major [ '.' minor [ '.' build [ '.' revision ] ] ]".
    /// </summary>
    /// <param name="s">The version string to parse.</param>
    /// <param name="version">If parsing succeeds, the parsed version. Otherwise a version that represents as much of the input as could be parsed successfully.</param>
    /// <returns>True when parsing succeeds completely (i.e. every character in the string was consumed), false otherwise.</returns>
    static member TryParse(s : string, [<System.Runtime.InteropServices.Out>] version : byref<Version>) = 
        VersionHelper.TryParse (s, false, UInt16.MaxValue, true, ref version)
    /// <summary>
    /// Parses a version string of the form "major [ '.' minor [ '.' ( '*' | ( build [ '.' ( '*' | revision ) ] ) ) ] ]"
    /// as accepted by System.Reflection.AssemblyVersionAttribute.
    /// </summary>
    /// <param name="s">The version string to parse.</param>
    /// <param name="allowWildcard">Indicates whether or not a wildcard is accepted as the terminal component.</param>
    /// <param name="version">
    /// If parsing succeeded, the parsed version. Otherwise a version instance with all parts set to zero.
    /// If <paramref name="s"/> contains * the version build and/or revision numbers are set to <see cref="ushort.MaxValue"/>.
    /// </param>
    /// <returns>True when parsing succeeds completely (i.e. every character in the string was consumed), false otherwise.</returns>

    static member TryParseAssemblyVersion(s : string, allowWildcard : System.Boolean, [<System.Runtime.InteropServices.Out>] version : byref<Version>) = 
        VersionHelper.TryParse (s, allowWildcard, (UInt16.MaxValue - 1us), false, ref version)
    
    static member private NullVersion = new Version(0, 0, 0, 0)
    /// <summary>
    /// Parses a version string of the form "major [ '.' minor [ '.' ( '*' | ( build [ '.' ( '*' | revision ) ] ) ) ] ]"
    /// as accepted by System.Reflection.AssemblyVersionAttribute.
    /// </summary>
    /// <param name="s">The version string to parse.</param>
    /// <param name="allowWildcard">Indicates whether or not we're parsing an assembly version string. If so, wildcards are accepted and each component must be less than 65535.</param>
    /// <param name="maxValue">The maximum value that a version component may have.</param>
    /// <param name="allowPartialParse">Allow the parsing of version elements where invalid characters exist. e.g. 1.2.2a.1</param>
    /// <param name="version">
    /// If parsing succeeded, the parsed version. When <paramref name="allowPartialParse"/> is true a version with values up to the first invalid character set. Otherwise a version with all parts set to zero.
    /// If <paramref name="s"/> contains * and wildcard is allowed the version build and/or revision numbers are set to <see cref="ushort.MaxValue"/>.
    /// </param>
    /// <returns>True when parsing succeeds completely (i.e. every character in the string was consumed), false otherwise.</returns>
    static member private TryParse(s : string, allowWildcard : System.Boolean, maxValue : System.UInt16, allowPartialParse : System.Boolean, [<System.Runtime.InteropServices.Out>] version : byref<Version>) = 
        Debug.Assert (not allowWildcard || maxValue < UInt16.MaxValue)
        if String.IsNullOrWhiteSpace (s)
        then 
            version <- VersionHelper.NullVersion
            false
        else
            let mutable (elements : string[]) = s.Split ('.')
            let mutable (hasWildcard : System.Boolean) = allowWildcard && elements.[(int (elements.Length - 1))] = "*"
            if hasWildcard && elements.Length < 3 || elements.Length > 4
            then 
                version <- VersionHelper.NullVersion
                false
            else
                let mutable (values : uint16[]) = Array.zeroCreate 4
                let mutable (lastExplicitValue : int) = 
                    if hasWildcard
                    then elements.Length - 1
                    else elements.Length
                let mutable (parseError : System.Boolean) = false
                let mutable earlyReturn = None
                do 
                    let mutable (i : int) = 0
                    let mutable breakLoop = false
                    while (i < lastExplicitValue) && not breakLoop do
                        if not (UInt16.TryParse (elements.[i], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, ref values.[i])) || values.[i] > maxValue
                        then 
                            if not allowPartialParse
                            then
                                earlyReturn <- Some false
                                breakLoop <- true
                                version <- VersionHelper.NullVersion
                            else
                                parseError <- true
                                if String.IsNullOrWhiteSpace (elements.[i])
                                then 
                                    values.[i] <- 0us
                                    breakLoop <- true
                                else
                                    if values.[i] > maxValue
                                    then 
                                        values.[i] <- 0us
                                        breakLoop <- true
                                    else
                                        let mutable (invalidFormat : System.Boolean) = false
                                        //let mutable (number : System.Numerics.BigInteger) = 0I
                                        do 
                                            let mutable idx = 0
                                            let mutable breakLoop = false
                                            while (idx < elements.[i].Length) && not breakLoop do
                                                if not (Char.IsDigit (elements.[i].[idx]))
                                                then 
                                                    invalidFormat <- true
                                                    VersionHelper.TryGetValue ((elements.[i].Substring (0, idx)), ref values.[i]) |> ignore<bool>
                                                    breakLoop <- true
                                                else
                                                    idx <- idx + 1
                                        let mutable doBreak = true
                                        if not invalidFormat
                                        then 
                                            if VersionHelper.TryGetValue (elements.[i], ref values.[i])
                                            then 
                                                //For this scenario the old compiler would continue processing the remaining version elements
                                                //so continue processing
                                                doBreak <- false
                                                () (* ERROR ContinueNotSupported *)
                                        (* ERROR BreakNotSupported *)
                        if not breakLoop then
                            i <- i + 1
                if hasWildcard
                then 
                    do 
                        let mutable (i : int) = lastExplicitValue
                        while (i < values.Length) do
                            values.[i] <- UInt16.MaxValue
                            i <- i + 1
                version <- new Version(int values.[0], int values.[1], int values.[2], int values.[3])
                not parseError
    static member private TryGetValue(s : string, [<System.Runtime.InteropServices.Out>] value : byref<System.UInt16>) : bool = 
        let mutable (number : System.Numerics.BigInteger) = Unchecked.defaultof<System.Numerics.BigInteger>
        if System.Numerics.BigInteger.TryParse (s, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, ref number)
        then 
            value <- uint16 (number % bigint 65536)
            true
        else
            value <- 0us
            false
    static member GenerateVersionFromPatternAndCurrentTime(time : DateTime, pattern : Version) = 
        if pattern = Unchecked.defaultof<_> || pattern.Revision <> int UInt16.MaxValue
        then pattern
        else
            let mutable time = time
            // MSDN doc on the attribute: 
            // "The default build number increments daily. The default revision number is the number of seconds since midnight local time 
            // (without taking into account time zone adjustments for daylight saving time), divided by 2."
            if time = Unchecked.defaultof<DateTime>
            then time <- DateTime.Now
            let mutable (revision : int) = int time.TimeOfDay.TotalSeconds / 2
            Debug.Assert (revision < int UInt16.MaxValue)
            if pattern.Build = int UInt16.MaxValue
            then 
                let mutable (days : TimeSpan) = time.Date - new DateTime(2000, 1, 1)
                let mutable (build : int) = Math.Min (int UInt16.MaxValue, (int days.TotalDays))
                new Version(pattern.Major, pattern.Minor, int (uint16 build), int (uint16 revision))
            else new Version(pattern.Major, pattern.Minor, pattern.Build, int (uint16 revision))

type VersionResourceSerializer() =
    member val private _commentsContents = Unchecked.defaultof<string> with get, set
    member val private _companyNameContents = Unchecked.defaultof<string> with get, set
    member val private _fileDescriptionContents = Unchecked.defaultof<string> with get, set
    member val private _fileVersionContents = Unchecked.defaultof<string> with get, set
    member val private _internalNameContents = Unchecked.defaultof<string> with get, set
    member val private _legalCopyrightContents = Unchecked.defaultof<string> with get, set
    member val private _legalTrademarksContents = Unchecked.defaultof<string> with get, set
    member val private _originalFileNameContents = Unchecked.defaultof<string> with get, set
    member val private _productNameContents = Unchecked.defaultof<string> with get, set
    member val private _productVersionContents = Unchecked.defaultof<string> with get, set
    member val private _assemblyVersionContents = Unchecked.defaultof<Version> with get, set
    static member val private vsVersionInfoKey = "VS_VERSION_INFO" with get, set
    static member val private varFileInfoKey = "VarFileInfo" with get, set
    static member val private translationKey = "Translation" with get, set
    static member val private stringFileInfoKey = "StringFileInfo" with get, set
    member val private _langIdAndCodePageKey = Unchecked.defaultof<string> with get, set
    static member val private CP_WINUNICODE = 1200u
    static member val private sizeVS_FIXEDFILEINFO = uint16 (sizeof<DWORD> * 13)
    member val private _isDll = Unchecked.defaultof<System.Boolean> with get, set
    new(isDll : System.Boolean, comments : string, companyName : string, fileDescription : string, fileVersion : string, internalName : string, legalCopyright : string, legalTrademark : string, originalFileName : string, productName : string, productVersion : string, assemblyVersion : Version) as this = 
        (VersionResourceSerializer ())
        then
            this._isDll <- isDll
            this._commentsContents <- comments
            this._companyNameContents <- companyName
            this._fileDescriptionContents <- fileDescription
            this._fileVersionContents <- fileVersion
            this._internalNameContents <- internalName
            this._legalCopyrightContents <- legalCopyright
            this._legalTrademarksContents <- legalTrademark
            this._originalFileNameContents <- originalFileName
            this._productNameContents <- productName
            this._productVersionContents <- productVersion
            this._assemblyVersionContents <- assemblyVersion
            this._langIdAndCodePageKey <- System.String.Format ("{0:x4}{1:x4}", 0, VersionResourceSerializer.CP_WINUNICODE)
    static member val private VFT_APP = 0x00000001u
    static member val private VFT_DLL = 0x00000002u
    member private this.GetVerStrings() = seq {
        if this._commentsContents <> Unchecked.defaultof<_>
        then yield KeyValuePair<_,_>("Comments", this._commentsContents)
        if this._companyNameContents <> Unchecked.defaultof<_>
        then yield KeyValuePair<_,_>("CompanyName", this._companyNameContents)
        if this._fileDescriptionContents <> Unchecked.defaultof<_>
        then yield KeyValuePair<_,_>("FileDescription", this._fileDescriptionContents)
        yield KeyValuePair<_,_>("FileVersion", this._fileVersionContents)
        if this._internalNameContents <> Unchecked.defaultof<_>
        then yield KeyValuePair<_,_>("InternalName", this._internalNameContents)
        if this._legalCopyrightContents <> Unchecked.defaultof<_>
        then yield KeyValuePair<_,_>("LegalCopyright", this._legalCopyrightContents)
        if this._legalTrademarksContents <> Unchecked.defaultof<_>
        then yield KeyValuePair<_,_>("LegalTrademarks", this._legalTrademarksContents)
        if this._originalFileNameContents <> Unchecked.defaultof<_>
        then yield KeyValuePair<_,_>("OriginalFilename", this._originalFileNameContents)
        if this._productNameContents <> Unchecked.defaultof<_>
        then yield KeyValuePair<_,_>("ProductName", this._productNameContents)
        yield KeyValuePair<_,_>("ProductVersion", this._fileVersionContents)
        if this._assemblyVersionContents <> Unchecked.defaultof<_>
        then yield KeyValuePair<_,_>("Assembly Version", this._assemblyVersionContents.ToString())
    }
    member private this.FileType
        with get() : uint32 = 
            if this._isDll
            then VersionResourceSerializer.VFT_DLL
            else VersionResourceSerializer.VFT_APP
    member private this.WriteVSFixedFileInfo(writer : BinaryWriter) = 
        let mutable (fileVersion : Version) = Unchecked.defaultof<Version>
        VersionHelper.TryParse (this._fileVersionContents, ref fileVersion) |> ignore<bool>
        let mutable (productVersion : Version) = Unchecked.defaultof<Version>
        VersionHelper.TryParse (this._productVersionContents, ref productVersion) |> ignore<bool>
        writer.Write (0xFEEF04BDu)
        writer.Write (0x00010000u)
        writer.Write ((uint32 fileVersion.Major <<< 16) ||| uint32 fileVersion.Minor)
        writer.Write ((uint32 fileVersion.Build <<< 16) ||| uint32 fileVersion.Revision)
        writer.Write ((uint32 productVersion.Major <<< 16) ||| uint32 productVersion.Minor)
        writer.Write ((uint32 productVersion.Build <<< 16) ||| uint32 productVersion.Revision)
        writer.Write (0x0000003Fu)
        writer.Write (0u)
        writer.Write (0x00000004u)
        writer.Write (this.FileType)
        writer.Write (0u)
        writer.Write (0u)
        writer.Write (0u)
    static member private PadKeyLen(cb : int) = 
        VersionResourceSerializer.PadToDword (cb + 3 * sizeof<WORD>) - 3 * sizeof<WORD>
    static member private PadToDword(cb : int) = 
        cb + 3 &&& ~~~3
    static member val private HDRSIZE = (int (3 * sizeof<uint16>)) with get, set
    static member private SizeofVerString(lpszKey : string, lpszValue : string) = 
        let mutable (cbKey : int) = Unchecked.defaultof<int>
        let mutable (cbValue : int) = Unchecked.defaultof<int>
        cbKey <- lpszKey.Length + 1 * 2
        cbValue <- lpszValue.Length + 1 * 2
        VersionResourceSerializer.PadKeyLen(cbKey) + cbValue + VersionResourceSerializer.HDRSIZE
    static member private WriteVersionString(keyValuePair : KeyValuePair<string, string>, writer : BinaryWriter) = 
        System.Diagnostics.Debug.Assert (keyValuePair.Value <> Unchecked.defaultof<_>)
        let mutable (cbBlock : System.UInt16) = uint16 <| VersionResourceSerializer.SizeofVerString (keyValuePair.Key, keyValuePair.Value)
        let mutable (cbKey : int) = keyValuePair.Key.Length + 1 * 2
        //let mutable (cbVal : int) = keyValuePair.Value.Length + 1 * 2
        let mutable startPos = writer.BaseStream.Position
        Debug.Assert (startPos &&& 3L = 0L)
        writer.Write (cbBlock)
        writer.Write (uint16 (keyValuePair.Value.Length + 1))
        writer.Write (1us)
        writer.Write (keyValuePair.Key.ToCharArray ())
        writer.Write (uint16 0) //(WORD)'\0'
        writer.Write (Array.zeroCreate (VersionResourceSerializer.PadKeyLen (cbKey) - cbKey) : byte[])
        Debug.Assert (writer.BaseStream.Position &&& 3L = 0L)
        writer.Write (keyValuePair.Value.ToCharArray ())
        writer.Write (uint16 0) // (WORD)'\0'
        System.Diagnostics.Debug.Assert (int64 cbBlock = writer.BaseStream.Position - startPos)
    static member private KEYSIZE(sz : string) = 
        VersionResourceSerializer.PadKeyLen (sz.Length + 1 * sizeof<WCHAR>) / sizeof<WCHAR>
    static member private KEYBYTES(sz : string) = 
        VersionResourceSerializer.KEYSIZE (sz) * sizeof<WCHAR>
    member private this.GetStringsSize() = 
        let mutable (sum : int) = 0
        for verString in this.GetVerStrings () do
            sum <- sum + 3 &&& ~~~3
            sum <- sum + VersionResourceSerializer.SizeofVerString (verString.Key, verString.Value)
        sum
    member this.GetDataSize() = 
        let mutable (sizeEXEVERRESOURCE : int) =
             sizeof<WORD> * 3 * 5 + 2 *  sizeof<WORD> +
             VersionResourceSerializer.KEYBYTES (VersionResourceSerializer.vsVersionInfoKey) +
             VersionResourceSerializer.KEYBYTES (VersionResourceSerializer.varFileInfoKey) +
             VersionResourceSerializer.KEYBYTES (VersionResourceSerializer.translationKey) +
             VersionResourceSerializer.KEYBYTES (VersionResourceSerializer.stringFileInfoKey) +
             VersionResourceSerializer.KEYBYTES (this._langIdAndCodePageKey) +
             int VersionResourceSerializer.sizeVS_FIXEDFILEINFO
        this.GetStringsSize () + sizeEXEVERRESOURCE
    member this.WriteVerResource(writer : BinaryWriter) = 
        let mutable debugPos = writer.BaseStream.Position
        let mutable dataSize = this.GetDataSize ()
        writer.Write (WORD dataSize)
        writer.Write (WORD VersionResourceSerializer.sizeVS_FIXEDFILEINFO)
        writer.Write (WORD 0us)
        writer.Write (VersionResourceSerializer.vsVersionInfoKey.ToCharArray ())
        writer.Write (Array.zeroCreate (VersionResourceSerializer.KEYBYTES (VersionResourceSerializer.vsVersionInfoKey) - VersionResourceSerializer.vsVersionInfoKey.Length * 2) : byte[])
        System.Diagnostics.Debug.Assert (writer.BaseStream.Position &&& 3L = 0L)
        this.WriteVSFixedFileInfo (writer)
        writer.Write (WORD (sizeof<WORD> * 2 + 2 * VersionResourceSerializer.HDRSIZE + VersionResourceSerializer.KEYBYTES (VersionResourceSerializer.varFileInfoKey) + VersionResourceSerializer.KEYBYTES (VersionResourceSerializer.translationKey)))
        writer.Write (WORD 0us)
        writer.Write (WORD 1us)
        writer.Write (VersionResourceSerializer.varFileInfoKey.ToCharArray ())
        writer.Write (Array.zeroCreate (VersionResourceSerializer.KEYBYTES (VersionResourceSerializer.varFileInfoKey) - VersionResourceSerializer.varFileInfoKey.Length * 2): byte[])
        System.Diagnostics.Debug.Assert (writer.BaseStream.Position &&& 3L = 0L)
        writer.Write (WORD (sizeof<WORD> * 2 + VersionResourceSerializer.HDRSIZE + VersionResourceSerializer.KEYBYTES (VersionResourceSerializer.translationKey)))
        writer.Write (WORD (sizeof<WORD> * 2))
        writer.Write (WORD 0us)
        writer.Write (VersionResourceSerializer.translationKey.ToCharArray ())
        writer.Write (Array.zeroCreate (VersionResourceSerializer.KEYBYTES (VersionResourceSerializer.translationKey) - VersionResourceSerializer.translationKey.Length * 2): byte[])
        System.Diagnostics.Debug.Assert (writer.BaseStream.Position &&& 3L = 0L)
        writer.Write (0us)
        writer.Write (WORD VersionResourceSerializer.CP_WINUNICODE)
        System.Diagnostics.Debug.Assert (writer.BaseStream.Position &&& 3L = 0L)
        writer.Write (WORD (2 * VersionResourceSerializer.HDRSIZE + VersionResourceSerializer.KEYBYTES (VersionResourceSerializer.stringFileInfoKey) + VersionResourceSerializer.KEYBYTES (this._langIdAndCodePageKey) + this.GetStringsSize ()))
        writer.Write (0us)
        writer.Write (1us)
        writer.Write (VersionResourceSerializer.stringFileInfoKey.ToCharArray ())
        writer.Write (Array.zeroCreate (VersionResourceSerializer.KEYBYTES (VersionResourceSerializer.stringFileInfoKey) - VersionResourceSerializer.stringFileInfoKey.Length * 2): byte[])
        System.Diagnostics.Debug.Assert (writer.BaseStream.Position &&& 3L = 0L)
        writer.Write (WORD (VersionResourceSerializer.HDRSIZE + VersionResourceSerializer.KEYBYTES (this._langIdAndCodePageKey) + this.GetStringsSize ()))
        writer.Write (0us)
        writer.Write (1us)
        writer.Write (this._langIdAndCodePageKey.ToCharArray ())
        writer.Write (Array.zeroCreate (VersionResourceSerializer.KEYBYTES (this._langIdAndCodePageKey) - this._langIdAndCodePageKey.Length * 2): byte[])
        System.Diagnostics.Debug.Assert (writer.BaseStream.Position &&& 3L = 0L)
        System.Diagnostics.Debug.Assert (writer.BaseStream.Position - debugPos = int64 dataSize - int64  (this.GetStringsSize ()))
        debugPos <- writer.BaseStream.Position
        for entry in this.GetVerStrings () do
            let mutable writerPos = writer.BaseStream.Position
            writer.Write (Array.zeroCreate (int ((writerPos + 3L) &&& ~~~3L - writerPos)): byte[])
            System.Diagnostics.Debug.Assert (entry.Value <> Unchecked.defaultof<_>)
            VersionResourceSerializer.WriteVersionString (entry, writer)
        System.Diagnostics.Debug.Assert (writer.BaseStream.Position - debugPos = int64 (this.GetStringsSize ()))

type Win32ResourceConversions() =
    static member AppendIconToResourceStream(resStream : Stream, iconStream : Stream) = 
        let mutable iconReader = new BinaryReader(iconStream)
        let mutable reserved = iconReader.ReadUInt16 ()
        if reserved <> 0us
        then raise <| ResourceException("IconStreamUnexpectedFormat")
        let mutable ``type`` = iconReader.ReadUInt16 ()
        if ``type`` <> 1us
        then raise <| ResourceException("IconStreamUnexpectedFormat")
        let mutable count = iconReader.ReadUInt16 ()
        if count = 0us
        then raise <| ResourceException("IconStreamUnexpectedFormat")
        let mutable iconDirEntries : ICONDIRENTRY [] = Array.zeroCreate (int count)
        do 
            let mutable (i : System.UInt16) = 0us
            while (i < count) do
                iconDirEntries.[(int i)].bWidth <- iconReader.ReadByte ()
                iconDirEntries.[(int i)].bHeight <- iconReader.ReadByte ()
                iconDirEntries.[(int i)].bColorCount <- iconReader.ReadByte ()
                iconDirEntries.[(int i)].bReserved <- iconReader.ReadByte ()
                iconDirEntries.[(int i)].wPlanes <- iconReader.ReadUInt16 ()
                iconDirEntries.[(int i)].wBitCount <- iconReader.ReadUInt16 ()
                iconDirEntries.[(int i)].dwBytesInRes <- iconReader.ReadUInt32 ()
                iconDirEntries.[(int i)].dwImageOffset <- iconReader.ReadUInt32 ()
                i <- i + 1us
        
        do 
            let mutable (i : System.UInt16) = 0us
            while (i < count) do
                iconStream.Position <- int64 iconDirEntries.[(int i)].dwImageOffset
                if iconReader.ReadUInt32 () = 40u
                then 
                    iconStream.Position <- iconStream.Position + 8L
                    iconDirEntries.[(int i)].wPlanes <- iconReader.ReadUInt16 ()
                    iconDirEntries.[(int i)].wBitCount <- iconReader.ReadUInt16 ()
                i <- i + 1us
        
        let mutable resWriter = new BinaryWriter(resStream)
        let mutable (RT_ICON : WORD) = 3us
        do 
            let mutable (i : System.UInt16) = 0us
            while (i < count) do
                resStream.Position <- resStream.Position + 3L &&& ~~~3L
                resWriter.Write (iconDirEntries.[(int i)].dwBytesInRes)
                resWriter.Write (0x00000020u)
                resWriter.Write (0xFFFFus)
                resWriter.Write (RT_ICON)
                resWriter.Write (0xFFFFus)
                resWriter.Write ((i + 1us))
                resWriter.Write (0x00000000u)
                resWriter.Write (0x1010us)
                resWriter.Write (0x0000us)
                resWriter.Write (0x00000000u)
                resWriter.Write (0x00000000u)
                iconStream.Position <- int64 iconDirEntries.[(int i)].dwImageOffset
                resWriter.Write (iconReader.ReadBytes (int (iconDirEntries.[int i].dwBytesInRes)))
                i <- i + 1us

        let mutable (RT_GROUP_ICON : WORD) = (RT_ICON + 11us)
        resStream.Position <- resStream.Position + 3L &&& ~~~3L
        resWriter.Write (uint32 (3 * sizeof<WORD> + int count * 14))
        resWriter.Write (0x00000020u)
        resWriter.Write (0xFFFFus)
        resWriter.Write (RT_GROUP_ICON)
        resWriter.Write (0xFFFFus)
        resWriter.Write (0x7F00us)
        resWriter.Write (0x00000000u)
        resWriter.Write (0x1030us)
        resWriter.Write (0x0000us)
        resWriter.Write (0x00000000u)
        resWriter.Write (0x00000000u)
        resWriter.Write (0x0000us)
        resWriter.Write (0x0001us)
        resWriter.Write (count)
        do 
            let mutable (i : System.UInt16) = 0us
            while (i < count) do
                resWriter.Write (iconDirEntries.[(int i)].bWidth)
                resWriter.Write (iconDirEntries.[(int i)].bHeight)
                resWriter.Write (iconDirEntries.[(int i)].bColorCount)
                resWriter.Write (iconDirEntries.[(int i)].bReserved)
                resWriter.Write (iconDirEntries.[(int i)].wPlanes)
                resWriter.Write (iconDirEntries.[(int i)].wBitCount)
                resWriter.Write (iconDirEntries.[(int i)].dwBytesInRes)
                resWriter.Write ((i + 1us))
                i <- i + 1us
        ()
    static member AppendVersionToResourceStream(resStream : Stream, isDll : System.Boolean, fileVersion : string, originalFileName : string, internalName : string, productVersion : string, assemblyVersion : Version, ?fileDescription : string, ?legalCopyright : string, ?legalTrademarks : string, ?productName : string, ?comments : string, ?companyName : string) = 
        let fileDescription = (defaultArg fileDescription) " "
        let legalCopyright = (defaultArg legalCopyright) " "
        let legalTrademarks = (defaultArg legalTrademarks) Unchecked.defaultof<_>
        let productName = (defaultArg productName) Unchecked.defaultof<_>
        let comments = (defaultArg comments) Unchecked.defaultof<_>
        let companyName = (defaultArg companyName) Unchecked.defaultof<_>
        let mutable resWriter = new BinaryWriter(resStream, Encoding.Unicode)
        resStream.Position <- resStream.Position + 3L &&& ~~~3L
        let mutable (RT_VERSION : DWORD) = 16u
        let mutable ver = new VersionResourceSerializer(isDll, comments, companyName, fileDescription, fileVersion, internalName, legalCopyright, legalTrademarks, originalFileName, productName, productVersion, assemblyVersion)
        let mutable startPos = resStream.Position
        let mutable dataSize = ver.GetDataSize ()
        let mutable (headerSize : int) = 0x20
        resWriter.Write (uint32 dataSize)
        resWriter.Write (uint32 headerSize)
        resWriter.Write (0xFFFFus)
        resWriter.Write (uint16 RT_VERSION)
        resWriter.Write (0xFFFFus)
        resWriter.Write (0x0001us)
        resWriter.Write (0x00000000u)
        resWriter.Write (0x0030us)
        resWriter.Write (0x0000us)
        resWriter.Write (0x00000000u)
        resWriter.Write (0x00000000u)
        ver.WriteVerResource (resWriter)
        System.Diagnostics.Debug.Assert (resStream.Position - startPos = int64 dataSize + int64 headerSize)
    static member AppendManifestToResourceStream(resStream : Stream, manifestStream : Stream, isDll : System.Boolean) = 
        resStream.Position <- resStream.Position + 3L &&& ~~~3L (* ERROR UnknownPrefixOperator "~" *)
        let mutable (RT_MANIFEST : WORD) = 24us
        let mutable resWriter = new BinaryWriter(resStream)
        resWriter.Write (uint32 manifestStream.Length)
        resWriter.Write (0x00000020u)
        resWriter.Write (0xFFFFus)
        resWriter.Write (RT_MANIFEST)
        resWriter.Write (0xFFFFus)
        resWriter.Write (
            if isDll then 0x0002us else 0x0001us)
        resWriter.Write (0x00000000u)
        resWriter.Write (0x1030us)
        resWriter.Write (0x0000us)
        resWriter.Write (0x00000000u)
        resWriter.Write (0x00000000u)
        manifestStream.CopyTo (resStream)