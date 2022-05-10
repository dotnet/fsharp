// (c) Microsoft Corporation 2005-2009.

#nowarn "47" // recursive initialization of LexBuffer


#if INTERNALIZED_FSLEXYACC_RUNTIME
namespace Internal.Utilities.Text.Lexing

#else
namespace Microsoft.FSharp.Text.Lexing 
#endif

    open System.Collections.Generic

    // REVIEW: This type showed up on a parsing-intensive performance measurement. Consider whether it can be a struct-record later when we have this feature. -jomo
#if INTERNALIZED_FSLEXYACC_RUNTIME
    type internal Position = 
#else
    type Position = 
#endif
        { pos_fname : string;
          pos_lnum : int;
#if INTERNALIZED_FSLEXYACC_RUNTIME
          pos_orig_lnum : int;
#endif
          pos_bol : int;
          pos_cnum : int; }
        member x.FileName = x.pos_fname
        member x.Line = x.pos_lnum
#if INTERNALIZED_FSLEXYACC_RUNTIME
        member x.OriginalLine = x.pos_orig_lnum
#endif
        member x.Char = x.pos_cnum
        member x.AbsoluteOffset = x.pos_cnum
        member x.StartOfLine = x.pos_bol
        member x.StartOfLineAbsoluteOffset = x.pos_bol
        member x.Column = x.pos_cnum - x.pos_bol
        member pos.NextLine = 
            { pos with 
#if INTERNALIZED_FSLEXYACC_RUNTIME
                    pos_orig_lnum = pos.OriginalLine + 1;
#endif
                    pos_lnum = pos.Line+1; 
                    pos_bol = pos.AbsoluteOffset }
        member pos.EndOfToken(n) = {pos with pos_cnum=pos.pos_cnum + n }
        member pos.AsNewLinePos() = pos.NextLine
        member pos.ShiftColumnBy(by) = {pos with pos_cnum = pos.pos_cnum + by}
        static member Empty = 
            { pos_fname=""; 
              pos_lnum= 0; 
#if INTERNALIZED_FSLEXYACC_RUNTIME
              pos_orig_lnum = 0;
#endif
              pos_bol= 0; 
              pos_cnum=0 }
        static member FirstLine(filename) = 
            { pos_fname=filename; 
#if INTERNALIZED_FSLEXYACC_RUNTIME
              pos_orig_lnum = 1;
#endif
              pos_lnum= 1; 
              pos_bol= 0; 
              pos_cnum=0 }

#if INTERNALIZED_FSLEXYACC_RUNTIME
    type internal LexBufferFiller<'char> = 
#else
    type LexBufferFiller<'char> = 
#endif
        { fillSync : (LexBuffer<'char> -> unit) option
          fillAsync : (LexBuffer<'char> -> Async<unit>) option } 
        
    and [<Sealed>]
#if INTERNALIZED_FSLEXYACC_RUNTIME
        internal LexBuffer<'char>(filler: LexBufferFiller<'char>) as this = 
#else
        LexBuffer<'char>(filler: LexBufferFiller<'char>) as this = 
#endif
        let context = new Dictionary<string,obj>(1) in 
        let extendBufferSync = (fun () -> match filler.fillSync with Some refill -> refill this | None -> invalidOp "attempt to read synchronously from an asynchronous lex buffer")
        let extendBufferAsync = (fun () -> match filler.fillAsync with Some refill -> refill this | None -> invalidOp "attempt to read asynchronously from a synchronous lex buffer")
        let mutable buffer=[||];
        /// number of valid charactes beyond bufferScanStart 
        let mutable bufferMaxScanLength=0;
        /// count into the buffer when scanning 
        let mutable bufferScanStart=0;
        /// number of characters scanned so far 
        let mutable bufferScanLength=0;
        /// length of the scan at the last accepting state 
        let mutable lexemeLength=0;
        /// action related to the last accepting state 
        let mutable bufferAcceptAction=0;
        let mutable eof = false;
        let mutable startPos = Position.Empty ;
        let mutable endPos = Position.Empty

        // Throw away all the input besides the lexeme 
              
        let discardInput () = 
            let keep = Array.sub buffer bufferScanStart bufferScanLength
            let nkeep = keep.Length 
            Array.blit keep 0 buffer 0 nkeep;
            bufferScanStart <- 0;
            bufferMaxScanLength <- nkeep
                 
              
        member lexbuf.EndOfScan () : int =
            // Printf.eprintf "endOfScan, lexBuffer.lexemeLength = %d\n" lexBuffer.lexemeLength;
            if bufferAcceptAction < 0 then 
                failwith "unrecognized input"

            //  Printf.printf "endOfScan %d state %d on unconsumed input '%c' (%d)\n" a s (Char.chr inp) inp;
            //   Printf.eprintf "accept, lexeme = %s\n" (lexeme lexBuffer); 
            lexbuf.StartPos <- endPos;
            lexbuf.EndPos <- endPos.EndOfToken(lexbuf.LexemeLength);
            bufferAcceptAction

        member lexbuf.StartPos
           with get() = startPos
           and  set(b) =  startPos <- b
           
        member lexbuf.EndPos 
           with get() = endPos
           and  set(b) =  endPos <- b

        member lexbuf.Lexeme         = Array.sub buffer bufferScanStart lexemeLength
        member lexbuf.LexemeChar(n)  = buffer.[n+bufferScanStart]
        
        member lexbuf.BufferLocalStore = (context :> IDictionary<_,_>)
        member lexbuf.LexemeLength        with get() : int = lexemeLength    and set v = lexemeLength <- v
        member internal lexbuf.Buffer              with get() : 'char[] = buffer              and set v = buffer <- v
        member internal lexbuf.BufferMaxScanLength with get() = bufferMaxScanLength and set v = bufferMaxScanLength <- v
        member internal lexbuf.BufferScanLength    with get() = bufferScanLength    and set v = bufferScanLength <- v
        member internal lexbuf.BufferScanStart     with get() : int = bufferScanStart     and set v = bufferScanStart <- v
        member internal lexbuf.BufferAcceptAction  with get() = bufferAcceptAction  and set v = bufferAcceptAction <- v
        member internal lexbuf.RefillBuffer = extendBufferSync
        member internal lexbuf.AsyncRefillBuffer = extendBufferAsync

        static member LexemeString(lexbuf:LexBuffer<char>) = 
            new System.String(lexbuf.Buffer,lexbuf.BufferScanStart,lexbuf.LexemeLength)

        member lexbuf.IsPastEndOfStream 
           with get() = eof
           and  set(b) =  eof <- b

        member lexbuf.DiscardInput() = discardInput ()

        member x.BufferScanPos = bufferScanStart + bufferScanLength

        member lexbuf.EnsureBufferSize n = 
            if lexbuf.BufferScanPos + n >= buffer.Length then 
                let repl = Array.zeroCreate (lexbuf.BufferScanPos + n) 
                Array.blit buffer bufferScanStart repl bufferScanStart bufferScanLength;
                buffer <- repl

        static member FromReadFunctions (syncRead : ('char[] * int * int -> int) option, asyncRead : ('char[] * int * int -> Async<int>) option) : LexBuffer<'char> = 
            let extension= Array.zeroCreate 4096
            let fillers = 
                { fillSync = 
                    match syncRead with 
                    | None -> None
                    | Some read -> 
                         Some (fun lexBuffer -> 
                             let n = read(extension,0,extension.Length)
                             lexBuffer.EnsureBufferSize n;
                             Array.blit extension 0 lexBuffer.Buffer lexBuffer.BufferScanPos n;
                             lexBuffer.BufferMaxScanLength <- lexBuffer.BufferScanLength + n); 
                  fillAsync = 
                    match asyncRead with 
                    | None -> None
                    | Some read -> 
                         Some (fun lexBuffer -> 
                                  async { 
                                      let! n = read(extension,0,extension.Length)
                                      lexBuffer.EnsureBufferSize n;
                                      Array.blit extension 0 lexBuffer.Buffer lexBuffer.BufferScanPos n;
                                      lexBuffer.BufferMaxScanLength <- lexBuffer.BufferScanLength + n }) }
            new LexBuffer<_>(fillers)

        // A full type signature is required on this method because it is used at more specific types within its own scope
        static member FromFunction (f : 'char[] * int * int -> int) : LexBuffer<'char> =  LexBuffer<_>.FromReadFunctions(Some(f),None)
        static member FromAsyncFunction (f : 'char[] * int * int -> Async<int>) : LexBuffer<'char> =  LexBuffer<_>.FromReadFunctions(None,Some(f))
              
        static member FromCharFunction f : LexBuffer<char> = 
            LexBuffer<char>.FromFunction(fun (buff,start,len) -> 
                let buff2 = Array.zeroCreate len
                let n = f buff2 len 
                Array.blit buff2 0 buff start len
                n)
        static member FromByteFunction f : LexBuffer<byte> = 
            LexBuffer<byte>.FromFunction(fun (buff,start,len) -> 
                let buff2 = Array.zeroCreate len
                let n = f buff2 len 
                Array.blit buff2 0 buff start len
                n)

        // A full type signature is required on this method because it is used at more specific types within its own scope
        static member FromArray (s: 'char[]) : LexBuffer<'char> = 
            let lexBuffer = 
                new LexBuffer<_> 
                    { fillSync = Some (fun _ -> ()); 
                      fillAsync = Some (fun _ -> async { return () }) }
            let buffer = Array.copy s 
            lexBuffer.Buffer <- buffer;
            lexBuffer.BufferMaxScanLength <- buffer.Length;
            lexBuffer

        static member FromBytes    (arr) = LexBuffer<byte>.FromArray(arr)
        static member FromChars    (arr) = LexBuffer<char>.FromArray(arr) 
        static member FromString (s:string) = LexBuffer<char>.FromChars (s.ToCharArray())

        static member FromTextReader (tr:System.IO.TextReader)  : LexBuffer<char> = 
           LexBuffer<char>.FromFunction(tr.Read) 

        static member FromBinaryReader (br:System.IO.BinaryReader)  : LexBuffer<byte> = 
           LexBuffer<byte>.FromFunction(br.Read) 

        static member FromStream (stream:System.IO.Stream)  : LexBuffer<byte> = 
           LexBuffer<byte>.FromReadFunctions(Some(stream.Read),Some(fun (buf,offset,len) -> stream.AsyncRead(buf,offset=offset,count=len))) 

    module GenericImplFragments = 
        let startInterpret(lexBuffer:LexBuffer<_>)= 
            lexBuffer.BufferScanStart <- lexBuffer.BufferScanStart + lexBuffer.LexemeLength;
            lexBuffer.BufferMaxScanLength <- lexBuffer.BufferMaxScanLength - lexBuffer.LexemeLength;
            lexBuffer.BufferScanLength <- 0;
            lexBuffer.LexemeLength <- 0;
            lexBuffer.BufferAcceptAction <- -1;

        let afterRefill (trans: uint16[] array,sentinel,lexBuffer:LexBuffer<_>,scanUntilSentinel,endOfScan,state,eofPos) = 
            // end of file occurs if we couldn't extend the buffer 
            if lexBuffer.BufferScanLength = lexBuffer.BufferMaxScanLength then  
                let snew = int trans.[state].[eofPos] // == EOF 
                if snew = sentinel then 
                    endOfScan()
                else 
                    if lexBuffer.IsPastEndOfStream then failwith "End of file on lexing stream";
                    lexBuffer.IsPastEndOfStream <- true;
                    // Printf.printf "state %d --> %d on eof\n" state snew;
                    scanUntilSentinel(lexBuffer,snew)
            else 
                scanUntilSentinel(lexBuffer, state)

        let onAccept (lexBuffer:LexBuffer<_>,a) = 
            lexBuffer.LexemeLength <- lexBuffer.BufferScanLength;
            lexBuffer.BufferAcceptAction <- a;

    open GenericImplFragments

    [<Sealed>]
#if INTERNALIZED_FSLEXYACC_RUNTIME
    type internal AsciiTables(trans: uint16[] array, accept: uint16[]) =
#else
    type AsciiTables(trans: uint16[] array, accept: uint16[]) =
#endif
        let rec scanUntilSentinel(lexBuffer, state) =
            let sentinel = 255 * 256 + 255 
            // Return an endOfScan after consuming the input 
            let a = int accept.[state] 
            if a <> sentinel then 
                onAccept (lexBuffer,a)
            
            if lexBuffer.BufferScanLength = lexBuffer.BufferMaxScanLength then 
                lexBuffer.DiscardInput();
                lexBuffer.RefillBuffer ();
              // end of file occurs if we couldn't extend the buffer 
                afterRefill (trans,sentinel,lexBuffer,scanUntilSentinel,lexBuffer.EndOfScan,state,256 (* == EOF *) )
            else
                // read a character - end the scan if there are no further transitions 
                let inp = int(lexBuffer.Buffer.[lexBuffer.BufferScanPos])
                let snew = int trans.[state].[inp] 
                if snew = sentinel then 
                    lexBuffer.EndOfScan()
                else 
                    lexBuffer.BufferScanLength <- lexBuffer.BufferScanLength + 1;
                    // Printf.printf "state %d --> %d on '%c' (%d)\n" state snew (Char.chr inp) inp;
                    scanUntilSentinel(lexBuffer, snew)
            
        /// Interpret tables for an ascii lexer generated by fslex. 
        member tables.Interpret(initialState,lexBuffer : LexBuffer<byte>) = 
            startInterpret(lexBuffer)
            scanUntilSentinel(lexBuffer, initialState)

        /// Interpret tables for an ascii lexer generated by fslex. 
        member tables.AsyncInterpret(initialState,lexBuffer : LexBuffer<byte>) = 
        
            let rec scanUntilSentinel(lexBuffer,state) : Async<int> = 
                async {  
                    let sentinel = 255 * 256 + 255 
                    // Return an endOfScan after consuming the input 
                    let a = int accept.[state] 
                    if a <> sentinel then 
                        onAccept (lexBuffer,a)
                    
                    if lexBuffer.BufferScanLength = lexBuffer.BufferMaxScanLength then 
                        lexBuffer.DiscardInput();
                        do! lexBuffer.AsyncRefillBuffer ();
                       // end of file occurs if we couldn't extend the buffer 
                        return! afterRefill (trans,sentinel,lexBuffer,scanUntilSentinel,endOfScan,state,256 (* == EOF *) )
                    else
                        // read a character - end the scan if there are no further transitions 
                        let inp = int(lexBuffer.Buffer.[lexBuffer.BufferScanPos])
                        let snew = int trans.[state].[inp] 
                        if snew = sentinel then 
                            return! endOfScan()
                        else 
                            lexBuffer.BufferScanLength <- lexBuffer.BufferScanLength + 1;
                            return! scanUntilSentinel(lexBuffer,snew)
                }
            and endOfScan() = 
                async { return lexBuffer.EndOfScan() }
            startInterpret(lexBuffer)
            scanUntilSentinel(lexBuffer, initialState)


        static member Create(trans,accept) = new AsciiTables(trans,accept)

    [<Sealed>]
#if INTERNALIZED_FSLEXYACC_RUNTIME
    type internal UnicodeTables(trans: uint16[] array, accept: uint16[]) = 
#else
    type UnicodeTables(trans: uint16[] array, accept: uint16[]) = 
#endif
        let sentinel = 255 * 256 + 255 
        let numUnicodeCategories = 30 
        let numLowUnicodeChars = 128 
        let numSpecificUnicodeChars = (trans.[0].Length - 1 - numLowUnicodeChars - numUnicodeCategories)/2
        let lookupUnicodeCharacters (state,inp: char) = 
            let inpAsInt = int inp
            // Is it a fast ASCII character?
            if inpAsInt < numLowUnicodeChars then 
                int trans.[state].[inpAsInt]
            else 
                // Search for a specific unicode character
                let baseForSpecificUnicodeChars = numLowUnicodeChars
                let rec loop i = 
                    if i >= numSpecificUnicodeChars then 
                        // OK, if we failed then read the 'others' entry in the alphabet,
                        // which covers all Unicode characters not covered in other
                        // ways
                        let baseForUnicodeCategories = numLowUnicodeChars+numSpecificUnicodeChars*2
                        let unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(inp)
                        //System.Console.WriteLine("inp = {0}, unicodeCategory = {1}", [| box inp; box unicodeCategory |]);
                        int trans.[state].[baseForUnicodeCategories + int32 unicodeCategory]
                    else 
                        // This is the specific unicode character
                        let c = char (int trans.[state].[baseForSpecificUnicodeChars+i*2])
                        //System.Console.WriteLine("c = {0}, inp = {1}, i = {2}", [| box c; box inp; box i |]);
                        // OK, have we found the entry for a specific unicode character?
                        if c = inp
                        then int trans.[state].[baseForSpecificUnicodeChars+i*2+1]
                        else loop(i+1)
                
                loop 0
        let eofPos    = numLowUnicodeChars + 2*numSpecificUnicodeChars + numUnicodeCategories 
        
        let rec scanUntilSentinel(lexBuffer,state) =
            // Return an endOfScan after consuming the input 
            let a = int accept.[state] 
            if a <> sentinel then 
                onAccept(lexBuffer,a)
            
            if lexBuffer.BufferScanLength = lexBuffer.BufferMaxScanLength then 
                lexBuffer.DiscardInput();
                lexBuffer.RefillBuffer ();
              // end of file occurs if we couldn't extend the buffer 
                afterRefill (trans,sentinel,lexBuffer,scanUntilSentinel,lexBuffer.EndOfScan,state,eofPos)
            else
                // read a character - end the scan if there are no further transitions 
                let inp = lexBuffer.Buffer.[lexBuffer.BufferScanPos]
                
                // Find the new state
                let snew = lookupUnicodeCharacters (state,inp)

                if snew = sentinel then 
                    lexBuffer.EndOfScan()
                else 
                    lexBuffer.BufferScanLength <- lexBuffer.BufferScanLength + 1;
                    // Printf.printf "state %d --> %d on '%c' (%d)\n" s snew (char inp) inp;
                    scanUntilSentinel(lexBuffer,snew)
                          
        // Each row for the Unicode table has format 
        //      128 entries for ASCII characters
        //      A variable number of 2*UInt16 entries for SpecificUnicodeChars 
        //      30 entries, one for each UnicodeCategory
        //      1 entry for EOF

        member tables.Interpret(initialState,lexBuffer : LexBuffer<char>) = 
            startInterpret(lexBuffer)
            scanUntilSentinel(lexBuffer, initialState)

        member tables.AsyncInterpret(initialState,lexBuffer : LexBuffer<char>) = 

            let rec scanUntilSentinel(lexBuffer, state) =
                async {
                    // Return an endOfScan after consuming the input 
                    let a = int accept.[state] 
                    if a <> sentinel then 
                        onAccept(lexBuffer,a)
                    
                    if lexBuffer.BufferScanLength = lexBuffer.BufferMaxScanLength then 
                        lexBuffer.DiscardInput();
                        lexBuffer.RefillBuffer ();
                        // end of file occurs if we couldn't extend the buffer 
                        return! afterRefill (trans,sentinel,lexBuffer,scanUntilSentinel,endOfScan,state,eofPos)
                    else
                        // read a character - end the scan if there are no further transitions 
                        let inp = lexBuffer.Buffer.[lexBuffer.BufferScanPos]
                        
                        // Find the new state
                        let snew = lookupUnicodeCharacters (state,inp)

                        if snew = sentinel then 
                            return! endOfScan()
                        else 
                            lexBuffer.BufferScanLength <- lexBuffer.BufferScanLength + 1;
                            return! scanUntilSentinel(lexBuffer, snew)
                }
            and endOfScan() = 
                async { return lexBuffer.EndOfScan() } 
            startInterpret(lexBuffer)
            scanUntilSentinel(lexBuffer, initialState)

        static member Create(trans,accept) = new UnicodeTables(trans,accept)
