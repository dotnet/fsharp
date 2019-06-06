// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.AbstractIL.Internal.StrongNameSign

#nowarn "9"

open System
open System.IO
open System.Collections.Immutable
open System.Reflection.PortableExecutable
open System.Security.Cryptography
open System.Runtime.InteropServices

    type KeyType =
    | Public
    | KeyPair

    let ALG_TYPE_RSA = int (2 <<< 9)
    let ALG_CLASS_KEY_EXCHANGE = int (5 <<< 13)
    let ALG_CLASS_SIGNATURE = int (1 <<< 13)
    let CALG_RSA_KEYX = int (ALG_CLASS_KEY_EXCHANGE ||| ALG_TYPE_RSA)
    let CALG_RSA_SIGN = int (ALG_CLASS_SIGNATURE ||| ALG_TYPE_RSA)

    let ALG_CLASS_HASH = int (4 <<< 13)
    let ALG_TYPE_ANY = int 0
    let CALG_SHA1 = int (ALG_CLASS_HASH ||| ALG_TYPE_ANY ||| 4)
    let CALG_SHA_256 = int (ALG_CLASS_HASH ||| ALG_TYPE_ANY ||| 12)
    let CALG_SHA_384 = int (ALG_CLASS_HASH ||| ALG_TYPE_ANY ||| 13)
    let CALG_SHA_512 = int (ALG_CLASS_HASH ||| ALG_TYPE_ANY ||| 14)

    let PUBLICKEYBLOB = int 0x6
    let PRIVATEKEYBLOB = int 0x7
    let BLOBHEADER_CURRENT_BVERSION = int 0x2
    let BLOBHEADER_LENGTH = int 20
    let RSA_PUB_MAGIC = int 0x31415352
    let RSA_PRIV_MAGIC = int 0x32415352

    let getResourceString (_, str) = str

    [<Struct; StructLayout(LayoutKind.Explicit)>]
    type ByteArrayUnion =
        [<FieldOffset(0)>]
        val UnderlyingArray: byte[]

        [<FieldOffset(0)>]
        val ImmutableArray: ImmutableArray<byte>

        new (immutableArray: ImmutableArray<byte>) = { UnderlyingArray = Array.empty<byte>; ImmutableArray = immutableArray}

    let getUnderlyingArray (array: ImmutableArray<byte>) =ByteArrayUnion(array).UnderlyingArray

    // Compute a hash over the elements of an assembly manifest file that should
    // remain static (skip checksum, Authenticode signatures and strong name signature blob)
    let hashAssembly (peReader:PEReader) (hashAlgorithm:IncrementalHash ) =
        // Hash content of all headers
        let peHeaders = peReader.PEHeaders
        let peHeaderOffset = peHeaders.PEHeaderStartOffset

        // Even though some data in OptionalHeader is different for 32 and 64,  this field is the same
        let checkSumOffset = peHeaderOffset + 0x40;                         // offsetof(IMAGE_OPTIONAL_HEADER, CheckSum)
        let securityDirectoryEntryOffset, peHeaderSize =
            match peHeaders.PEHeader.Magic with
            | PEMagic.PE32 ->       peHeaderOffset + 0x80, 0xE0             // offsetof(IMAGE_OPTIONAL_HEADER32, DataDirectory[IMAGE_DIRECTORY_ENTRY_SECURITY]), sizeof(IMAGE_OPTIONAL_HEADER32)
            | PEMagic.PE32Plus ->   peHeaderOffset + 0x90,0xF0              // offsetof(IMAGE_OPTIONAL_HEADER64, DataDirectory[IMAGE_DIRECTORY_ENTRY_SECURITY]), sizeof(IMAGE_OPTIONAL_HEADER64)
            | _ -> raise (BadImageFormatException(getResourceString(FSComp.SR.ilSignInvalidMagicValue())))

        let allHeadersSize = peHeaderOffset + peHeaderSize + int (peHeaders.CoffHeader.NumberOfSections) * 0x28;      // sizeof(IMAGE_SECTION_HEADER)
        let allHeaders =
            let array:byte[] = Array.zeroCreate<byte> allHeadersSize
            peReader.GetEntireImage().GetContent().CopyTo(0, array, 0, allHeadersSize)
            array

        // Clear checksum and security data directory
        for i in 0 .. 3 do allHeaders.[checkSumOffset + i] <- 0uy
        for i in 0 .. 7 do allHeaders.[securityDirectoryEntryOffset + i] <- 0uy
        hashAlgorithm.AppendData(allHeaders, 0, allHeadersSize)

        // Hash content of all sections
        let signatureDirectory = peHeaders.CorHeader.StrongNameSignatureDirectory
        let signatureStart =
            match peHeaders.TryGetDirectoryOffset signatureDirectory with
            | true, value -> value
            | _           -> raise (BadImageFormatException(getResourceString(FSComp.SR.ilSignBadImageFormat())))
        let signatureEnd = signatureStart + signatureDirectory.Size
        let buffer = getUnderlyingArray (peReader.GetEntireImage().GetContent())
        let sectionHeaders = peHeaders.SectionHeaders

        for i in 0 .. (sectionHeaders.Length - 1) do
            let section = sectionHeaders.[i]
            let mutable st = section.PointerToRawData
            let en = st + section.SizeOfRawData

            if st <= signatureStart && signatureStart < en then do
                // The signature should better end within this section as well
                if not ( (st < signatureEnd) && (signatureEnd <= en)) then raise (BadImageFormatException())

                // Signature starts within this section - hash everything up to the signature start
                hashAlgorithm.AppendData(buffer, st, signatureStart - st)

                // Trim what we have written
                st <- signatureEnd

            hashAlgorithm.AppendData(buffer, st, en - st)
            ()
        hashAlgorithm.GetHashAndReset()

    type BlobReader =
        val mutable _blob:byte[]
        val mutable _offset:int
        new (blob:byte[]) = { _blob = blob; _offset = 0; }

        member x.ReadInt32:int =
            let offset = x._offset
            x._offset <- offset + 4
            int (x._blob.[offset]) ||| (int (x._blob.[offset + 1]) <<< 8) ||| (int (x._blob.[offset + 2]) <<< 16) ||| (int (x._blob.[offset + 3]) <<< 24)

        member x.ReadBigInteger (length:int):byte[] =
            let arr:byte[] = Array.zeroCreate<byte> length
            Array.Copy(x._blob, x._offset, arr, 0, length) |> ignore
            x._offset <- x._offset  + length
            arr |> Array.rev

    let RSAParamatersFromBlob (blob:byte[]) keyType =
        let mutable reader = BlobReader blob
        if reader.ReadInt32 <> 0x00000207 && keyType = KeyType.KeyPair then raise (CryptographicException(getResourceString(FSComp.SR.ilSignPrivateKeyExpected())))
        reader.ReadInt32 |>ignore                                                                                                       // ALG_ID
        if reader.ReadInt32 <> RSA_PRIV_MAGIC then raise (CryptographicException(getResourceString(FSComp.SR.ilSignRsaKeyExpected())))  // 'RSA2'
        let byteLen, halfLen =
            let bitLen = reader.ReadInt32
            match bitLen % 16 with
            | 0 -> (bitLen / 8, bitLen / 16)
            | _ -> raise (CryptographicException(getResourceString(FSComp.SR.ilSignInvalidBitLen())))
        let mutable key = RSAParameters()
        key.Exponent <- reader.ReadBigInteger 4
        key.Modulus <- reader.ReadBigInteger byteLen
        key.P <- reader.ReadBigInteger halfLen
        key.Q <- reader.ReadBigInteger halfLen
        key.DP <- reader.ReadBigInteger halfLen
        key.DQ <- reader.ReadBigInteger halfLen
        key.InverseQ <- reader.ReadBigInteger halfLen
        key.D <- reader.ReadBigInteger byteLen
        key

    let toCLRKeyBlob (rsaParameters:RSAParameters) (algId:int) : byte[] =
        let validateRSAField (field:byte[]) expected (name:string) =
            if field <> null && field.Length <> expected then
                raise (CryptographicException(String.Format(getResourceString(FSComp.SR.ilSignInvalidRSAParams()), name)))

        // The original FCall this helper emulates supports other algId's - however, the only algid we need to support is CALG_RSA_KEYX. We will not port the codepaths dealing with other algid's.
        if algId <> CALG_RSA_KEYX then raise (CryptographicException(getResourceString(FSComp.SR.ilSignInvalidAlgId())))

        // Validate the RSA structure first.
        if rsaParameters.Modulus = null then raise (CryptographicException(String.Format(getResourceString(FSComp.SR.ilSignInvalidRSAParams()), "Modulus")))
        if rsaParameters.Exponent = null || rsaParameters.Exponent.Length > 4 then raise (CryptographicException(String.Format(getResourceString(FSComp.SR.ilSignInvalidRSAParams()), "Exponent")))

        let modulusLength = rsaParameters.Modulus.Length
        let halfModulusLength = (modulusLength + 1) / 2

        // We assume that if P != null, then so are Q, DP, DQ, InverseQ and D and indicate KeyPair RSA Parameters
        let isPrivate =
            if rsaParameters.P <> null then
                validateRSAField rsaParameters.P halfModulusLength "P"
                validateRSAField rsaParameters.Q halfModulusLength "Q"
                validateRSAField rsaParameters.DP halfModulusLength "DP"
                validateRSAField rsaParameters.InverseQ halfModulusLength "InverseQ"
                validateRSAField rsaParameters.D halfModulusLength "D"
                true
            else false

        let key =
            use ms = new MemoryStream()
            use bw = new BinaryWriter(ms)

            bw.Write(int CALG_RSA_SIGN)                                                // CLRHeader.aiKeyAlg
            bw.Write(int CALG_SHA1)                                                    // CLRHeader.aiHashAlg
            bw.Write(int (modulusLength + BLOBHEADER_LENGTH))                            // CLRHeader.KeyLength

            // Write out the BLOBHEADER
            bw.Write(byte (if isPrivate = true then PRIVATEKEYBLOB else PUBLICKEYBLOB))  // BLOBHEADER.bType
            bw.Write(byte BLOBHEADER_CURRENT_BVERSION)                                 // BLOBHEADER.bVersion
            bw.Write(int16 0)                                                          // BLOBHEADER.wReserved
            bw.Write(int CALG_RSA_SIGN)                                                // BLOBHEADER.aiKeyAlg

            // Write the RSAPubKey header
            bw.Write(int (if isPrivate then RSA_PRIV_MAGIC else RSA_PUB_MAGIC))          // RSAPubKey.magic
            bw.Write(int (modulusLength * 8))                                            // RSAPubKey.bitLen

            let expAsDword =
                let mutable buffer = int 0
                for i in 0 .. rsaParameters.Exponent.Length - 1 do
                   buffer <- (buffer <<< 8) ||| int (rsaParameters.Exponent.[i])
                buffer

            bw.Write expAsDword                                                        // RSAPubKey.pubExp
            bw.Write(rsaParameters.Modulus |> Array.rev)                                // Copy over the modulus for both public and private
            if isPrivate = true then do
                bw.Write(rsaParameters.P  |> Array.rev)
                bw.Write(rsaParameters.Q  |> Array.rev)
                bw.Write(rsaParameters.DP |> Array.rev)
                bw.Write(rsaParameters.DQ |> Array.rev)
                bw.Write(rsaParameters.InverseQ |> Array.rev)
                bw.Write(rsaParameters.D |> Array.rev)

            bw.Flush()
            ms.ToArray()
        key

    let createSignature (hash:byte[]) (keyBlob:byte[]) keyType =
        use rsa = RSA.Create()
        rsa.ImportParameters(RSAParamatersFromBlob keyBlob keyType)
        let signature = rsa.SignHash(hash, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1)
        signature |>Array.rev

    let patchSignature (stream:Stream) (peReader:PEReader) (signature:byte[]) =
        let peHeaders = peReader.PEHeaders
        let signatureDirectory = peHeaders.CorHeader.StrongNameSignatureDirectory
        let signatureOffset =
            if signatureDirectory.Size > signature.Length then raise (BadImageFormatException(getResourceString(FSComp.SR.ilSignInvalidSignatureSize())))
            match peHeaders.TryGetDirectoryOffset signatureDirectory with
            | false, _ -> raise (BadImageFormatException(getResourceString(FSComp.SR.ilSignNoSignatureDirectory())))
            | true, signatureOffset -> int64 signatureOffset
        stream.Seek(signatureOffset, SeekOrigin.Begin) |>ignore
        stream.Write(signature, 0, signature.Length)

        let corHeaderFlagsOffset = int64(peHeaders.CorHeaderStartOffset + 16)           // offsetof(IMAGE_COR20_HEADER, Flags)
        stream.Seek(corHeaderFlagsOffset, SeekOrigin.Begin) |>ignore
        stream.WriteByte (byte (peHeaders.CorHeader.Flags ||| CorFlags.StrongNameSigned))
        ()

    let signStream stream keyBlob =
        use peReader = new PEReader(stream, PEStreamOptions.PrefetchEntireImage ||| PEStreamOptions.LeaveOpen)
        let hash =
            use hashAlgorithm = IncrementalHash.CreateHash(HashAlgorithmName.SHA1)
            hashAssembly peReader hashAlgorithm
        let signature = createSignature hash keyBlob KeyType.KeyPair
        patchSignature stream peReader signature

    let signFile filename keyBlob =
        use fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite)
        signStream fs keyBlob

    let signatureSize (pk:byte[]) =
        if pk.Length < 25 then raise (CryptographicException(getResourceString(FSComp.SR.ilSignInvalidPKBlob())))
        let mutable reader = BlobReader pk
        reader.ReadBigInteger 12 |> ignore                                                     // Skip CLRHeader
        reader.ReadBigInteger 8  |> ignore                                                     // Skip BlobHeader
        let magic = reader.ReadInt32                                                            // Read magic
        if not (magic = RSA_PRIV_MAGIC || magic = RSA_PUB_MAGIC) then                           // RSAPubKey.magic
            raise (CryptographicException(getResourceString(FSComp.SR.ilSignInvalidPKBlob())))
        let x = reader.ReadInt32 / 8
        x

    // Returns a CLR Format Blob public key
    let getPublicKeyForKeyPair keyBlob =
        use rsa = RSA.Create()
        rsa.ImportParameters(RSAParamatersFromBlob keyBlob KeyType.KeyPair)
        let rsaParameters = rsa.ExportParameters false
        toCLRKeyBlob rsaParameters CALG_RSA_KEYX
