// #Regression #Conformance #DeclarationElements #Attributes 
// FSB 4029, [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)>]  does not get encoded as 'marshal( fixed sysstring [512]) '
namespace MarshallDemoFS

open System
open System.Runtime.InteropServices

#nowarn "9"     // disable "unverifiable code" warnings

type NagInt = int32         // type Integer in nag.h

type public NagErrorCode =         // NAG error codes    
      NE_NOERROR = 0
    | NE_CODE_NOT_SET = 1

type public NagErrorHandler = delegate of IntPtr * NagInt * IntPtr -> unit

[<StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)>]
type public NagError =
        val code: NagErrorCode                  // Out: Error Code
        val print: bool                         // In: print? yes/no
        [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)>] 
        val message: string                     // InOut: Error message (size is NAG_ERROR_BUF_LEN)
        val handler: NagErrorHandler            // In: Error handling function
        val errnum: NagInt                      // May hold useful value for some errors
        val iflag: NagInt                       // Those two are used for the BLAS; they may be used to
        val ival: NagInt                        // define BLAST-forum standard routines around NAG BLAS
        new() = { code = NagErrorCode.NE_NOERROR; print = false; message = "hello";
                    handler = null; errnum = 0; iflag = 0; ival = 0 }

module Program =
    [<EntryPoint>]
    let main args =
        let err = NagError()
        let size = Marshal.SizeOf(err)
        let expectedsize = if System.IntPtr.Size = 4 then 536 else 544
        if (size = expectedsize) then 
            0
        else 
            printfn "Error: expected size (%A) <> actual size (%A)" expectedsize size 
            failwith "Failed: 1"
