// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.AbstractIL.Internal.StrongNameSign

open System
open System.Collections.Immutable
open System.Reflection.PortableExecutable
open System.Security.Cryptography
open System.Runtime.InteropServices

#nowarn "9"

    [<StructLayout(LayoutKind.Explicit)>]
    type ByteArrayUnion = 
        struct 
            [<FieldOffset(0)>]
            val UnderlyingArray: byte[]

            [<FieldOffset(0)>]
            val ImmutableArray: ImmutableArray<byte>
            new (immutableArray:ImmutableArray<byte>) = { UnderlyingArray = Array.empty<byte> ; ImmutableArray = immutableArray}
        end
    let GetUnderlyingArray (array:ImmutableArray<byte>) =ByteArrayUnion(array).UnderlyingArray;

    // Compute a hash over the elements of an assembly manifest file that should
    // remain static (skip checksum, Authenticode signatures and strong name signature blob)
    let HashAssembly (peReader:PEReader) (hashAlgorithm:HashAlgorithm) =
        // Hash content of all headers
        let peHeaders = peReader.PEHeaders;
        let peHeaderOffset = peHeaders.PEHeaderStartOffset;

        // Even though some data in OptionalHeader is different for 32 and 64,  this field is the same
        let checkSumOffset = peHeaderOffset + 0x40;                         // offsetof(IMAGE_OPTIONAL_HEADER, CheckSum)
        let securityDirectoryEntryOffset, peHeaderSize =
            match peHeaders.PEHeader.Magic with
            | PEMagic.PE32 ->       peHeaderOffset + 0x80, 0xE0             // offsetof(IMAGE_OPTIONAL_HEADER32, DataDirectory[IMAGE_DIRECTORY_ENTRY_SECURITY]), sizeof(IMAGE_OPTIONAL_HEADER32)
            | PEMagic.PE32Plus ->   peHeaderOffset + 0x90,0xF0              // offsetof(IMAGE_OPTIONAL_HEADER64, DataDirectory[IMAGE_DIRECTORY_ENTRY_SECURITY]), sizeof(IMAGE_OPTIONAL_HEADER64)
            | _ -> raise (BadImageFormatException("Invalid Magic value in CLR Header"))

        let allHeadersSize = peHeaderOffset + peHeaderSize + int(peHeaders.CoffHeader.NumberOfSections) * 0x28;      // sizeof(IMAGE_SECTION_HEADER)
        let allHeaders = 
            let array:byte[] = Array.zeroCreate<byte> allHeadersSize
            peReader.GetEntireImage().GetContent().CopyTo(0, array, 0, allHeadersSize);
            array

        // Clear checksum and security data directory
        for i in 0 .. 3 do allHeaders.[checkSumOffset + i] <- 0uy
        for i in 0 .. 7 do allHeaders.[securityDirectoryEntryOffset + i] <- 0uy
        hashAlgorithm.TransformBlock(allHeaders, 0, allHeadersSize, null, 0) |>ignore

        // Hash content of all sections
        let signatureDirectory = peHeaders.CorHeader.StrongNameSignatureDirectory
        let signatureStart =
            match peHeaders.TryGetDirectoryOffset(signatureDirectory) with 
            | true, value -> value
            | _           -> raise (BadImageFormatException("Assembly is not signed" ))
        let signatureEnd = signatureStart + signatureDirectory.Size
        let buffer = GetUnderlyingArray (peReader.GetEntireImage().GetContent())
        let sectionHeaders = peHeaders.SectionHeaders

        for i in 0 .. (sectionHeaders.Length - 1) do

            let section = sectionHeaders.[i]
            let mutable st = section.PointerToRawData;
            let en = st + section.SizeOfRawData;

            if st <= signatureStart && signatureStart < en then do

                // The signature should better end within this section as well
                if not ( (st < signatureEnd) && (signatureEnd <= en)) then raise (BadImageFormatException())

                // Signature starts within this section - hash everything up to the signature start
                hashAlgorithm.TransformBlock(buffer, st, signatureStart - st, null, 0) |>ignore

                // Trim what we have written
                st <- signatureEnd

            hashAlgorithm.TransformBlock(buffer, st, en - st, null, 0) |>ignore
            ()

        hashAlgorithm.TransformFinalBlock(buffer, 0, 0)

    type BlobReader = 
        val mutable _blob:byte[]
        val mutable _offset:int
        new (blob:byte[]) = { _blob = blob; _offset = 0; }

        member x.ReadInt32:int =
            let offset = x._offset
            x._offset <- offset + 4
            int(x._blob.[offset]) ||| (int (x._blob.[offset + 1]) <<< 8) ||| (int (x._blob.[offset + 2]) <<< 16) ||| (int (x._blob.[offset + 3]) <<< 24)

        member x.ReadBigInteger (length:int):byte[] =
            let arr:byte[] = Array.zeroCreate<byte> length
            Array.Copy(x._blob, x._offset, arr, 0, length) |> ignore
            x._offset <- x._offset  + length;
            arr |> Array.rev

    let RSAParamatersFromBlob (blob:byte[]) =
        let mutable reader = BlobReader(blob)
        if reader.ReadInt32 <> 0x00000207 then raise (CryptographicException("Private key expected"))
        reader.ReadInt32 |>ignore                                                                       // ALG_ID
        if reader.ReadInt32 <> 0x32415352 then raise (CryptographicException("RSA key expected"))       // 'RSA2'

        let byteLen, halfLen = 
            let bitLen = reader.ReadInt32
            match bitLen % 16 with
            | 0 -> (bitLen / 8, bitLen / 16)
            | _ -> raise (CryptographicException("Invalid bitLen"))
        let mutable key = RSAParameters()
        key.Exponent <- reader.ReadBigInteger(4)
        key.Modulus <- reader.ReadBigInteger(byteLen)
        key.P <- reader.ReadBigInteger(halfLen)
        key.Q <- reader.ReadBigInteger(halfLen)
        key.DP <- reader.ReadBigInteger(halfLen)
        key.DQ <- reader.ReadBigInteger(halfLen)
        key.InverseQ <- reader.ReadBigInteger(halfLen)
        key.D <- reader.ReadBigInteger(byteLen)
        key

    let CreateSignature (rsa:RSA) (hash:byte[]) =
        let formatter = RSAPKCS1SignatureFormatter(rsa)
        formatter.SetHashAlgorithm("SHA1")
        let signature = formatter.CreateSignature(hash)
        signature |>Array.rev

    let PatchSignature (stream:Stream) (peReader:PEReader) (signature:byte[]) =
        let peHeaders = peReader.PEHeaders
        let signatureDirectory = peHeaders.CorHeader.StrongNameSignatureDirectory
        let signatureOffset =
            if signatureDirectory.Size > signature.Length then raise (BadImageFormatException("Invalid signature size"))
            match peHeaders.TryGetDirectoryOffset(signatureDirectory) with
            | false, _              -> raise (BadImageFormatException("No signature directory"))
            | true, signatureOffset -> int64(signatureOffset)
        stream.Seek(signatureOffset, SeekOrigin.Begin) |>ignore
        stream.Write(signature, 0, signature.Length)

        let corHeaderFlagsOffset = int64(peHeaders.CorHeaderStartOffset + 16)                             // offsetof(IMAGE_COR20_HEADER, Flags)
        stream.Seek(corHeaderFlagsOffset, SeekOrigin.Begin) |>ignore
        stream.WriteByte((byte)(peHeaders.CorHeader.Flags ||| CorFlags.StrongNameSigned))
        ()

    let CreateSignature (privateKeyBlob:byte[]) =
            use rsa = RSA.Create()
            rsa.ImportParameters(RSAParamatersFromBlob(privateKeyBlob))
            CreateSignature rsa hash


    let Sign (stream:Stream) (privateKeyBlob:byte[]) =
        use peReader = new PEReader(stream, PEStreamOptions.PrefetchEntireImage ||| PEStreamOptions.LeaveOpen)
        let hash =
            use hashAlgorithm = SHA1.Create()
            HashAssembly peReader hashAlgorithm |>ignore
            hashAlgorithm.Hash

        let signature = CreateSignature pk

        PatchSignature stream peReader signature
