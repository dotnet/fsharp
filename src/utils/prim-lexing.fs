// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#nowarn "47" // recursive initialization of LexBuffer


namespace Internal.Utilities.Text.Lexing

    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections
    open System.Collections.Generic

    // REVIEW: This type showed up on a parsing-intensive performance measurement. 
    // REVIEW: Consider whether it can be smaller or can be a struct. 
    type internal Position = 
        { /// The file name index for the position, use fileOfFileIndex in range.fs to decode
          posFileIndex: int;
          /// The line number for the position
          posLineNum: int;
          /// The line number for the position in the original source file
          posOriginalLineNum : int;
          /// The absolute offset of the beginning of the line
          posStartOfLineOffset: int;
          /// The absolute offset of the column for the position
          posColumnOffset: int; }
        member x.FileIndex = x.posFileIndex
        member x.Line = x.posLineNum
        member x.OriginalLine = x.posOriginalLineNum
        member x.AbsoluteOffset = x.posColumnOffset
        member x.StartOfLine = x.posStartOfLineOffset
        member x.StartOfLineAbsoluteOffset = x.posStartOfLineOffset
        member x.Column = x.posColumnOffset - x.posStartOfLineOffset
        member pos.NextLine = 
            { pos with 
                    posOriginalLineNum = pos.OriginalLine + 1;
                    posLineNum = pos.Line+1; 
                    posStartOfLineOffset = pos.AbsoluteOffset }
        member pos.EndOfToken n = {pos with posColumnOffset=pos.posColumnOffset + n }
        member pos.ShiftColumnBy by = {pos with posColumnOffset = pos.posColumnOffset + by}
        member pos.ColumnMinusOne = { pos with posColumnOffset = pos.posStartOfLineOffset-1 }

        member pos.ApplyLineDirective (fileIdx, line) =
            {pos with posFileIndex = fileIdx; 
                      posStartOfLineOffset= pos.posColumnOffset;
                      posLineNum=line };

        static member Empty = 
            { posFileIndex=0; 
              posLineNum= 0; 
              posOriginalLineNum = 0;
              posStartOfLineOffset= 0; 
              posColumnOffset=0 }

        static member FirstLine fileIdx = 
            { posFileIndex= fileIdx; 
              posStartOfLineOffset=0;
              posColumnOffset=0;
              posOriginalLineNum = 0;
              posLineNum=1 }

    type internal LexBufferFiller<'Char> = (LexBuffer<'Char> -> unit) 
        
    and [<Sealed>]
        internal LexBuffer<'Char>(filler: LexBufferFiller<'Char>) = 
        let context = new Dictionary<string,obj>(1) 
        let mutable buffer=[||];
        /// number of valid characters beyond bufferScanStart 
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

            //  printf "endOfScan %d state %d on unconsumed input '%c' (%d)\n" a s (Char.chr inp) inp;
            //   Printf.eprintf "accept, lexeme = %s\n" (lexeme lexBuffer); 
            lexbuf.StartPos <- endPos;
            lexbuf.EndPos <- endPos.EndOfToken(lexbuf.LexemeLength);
            bufferAcceptAction

        member lexbuf.StartPos
           with get() = startPos
           and  set b =  startPos <- b
           
        member lexbuf.EndPos 
           with get() = endPos
           and  set b =  endPos <- b

        member lexbuf.Lexeme         = Array.sub buffer bufferScanStart lexemeLength
        
        member lexbuf.BufferLocalStore = (context :> IDictionary<_,_>)
        member lexbuf.LexemeLength        with get() : int = lexemeLength    and set v = lexemeLength <- v
        member lexbuf.Buffer              with get() : 'Char[] = buffer              and set v = buffer <- v
        member lexbuf.BufferMaxScanLength with get() = bufferMaxScanLength and set v = bufferMaxScanLength <- v
        member lexbuf.BufferScanLength    with get() = bufferScanLength    and set v = bufferScanLength <- v
        member lexbuf.BufferScanStart     with get() : int = bufferScanStart     and set v = bufferScanStart <- v
        member lexbuf.BufferAcceptAction  with get() = bufferAcceptAction  and set v = bufferAcceptAction <- v
        member lexbuf.RefillBuffer () = filler lexbuf
        static member LexemeString(lexbuf:LexBuffer<char>) = 
            new System.String(lexbuf.Buffer,lexbuf.BufferScanStart,lexbuf.LexemeLength)

        member lexbuf.IsPastEndOfStream 
           with get() = eof
           and  set(b) =  eof <- b

        member lexbuf.DiscardInput () = discardInput ()

        member x.BufferScanPos = bufferScanStart + bufferScanLength

        member lexbuf.EnsureBufferSize n = 
            if lexbuf.BufferScanPos + n >= buffer.Length then 
                let repl = Array.zeroCreate (lexbuf.BufferScanPos + n) 
                Array.blit buffer bufferScanStart repl bufferScanStart bufferScanLength;
                buffer <- repl


        // A full type signature is required on this method because it is used at more specific types within its own scope
        static member FromFunction (f : 'Char[] * int * int -> int) : LexBuffer<'Char> = 
            let extension= Array.zeroCreate 4096
            let filler (lexBuffer: LexBuffer<'Char>) =
                 let n = f (extension,0,extension.Length)
                 lexBuffer.EnsureBufferSize n;
                 Array.blit extension 0 lexBuffer.Buffer lexBuffer.BufferScanPos n;
                 lexBuffer.BufferMaxScanLength <- lexBuffer.BufferScanLength + n
            new LexBuffer<'Char>(filler)
              
        // A full type signature is required on this method because it is used at more specific types within its own scope
        static member FromArray (s: 'Char[]) : LexBuffer<'Char> = 
            let lexBuffer = new LexBuffer<'Char>(fun _ -> ())
            let buffer = Array.copy s 
            lexBuffer.Buffer <- buffer;
            lexBuffer.BufferMaxScanLength <- buffer.Length;
            lexBuffer

        static member FromChars (arr:char[]) = LexBuffer.FromArray arr 

    module GenericImplFragments = 
        let startInterpret(lexBuffer:LexBuffer<char>)= 
            lexBuffer.BufferScanStart <- lexBuffer.BufferScanStart + lexBuffer.LexemeLength;
            lexBuffer.BufferMaxScanLength <- lexBuffer.BufferMaxScanLength - lexBuffer.LexemeLength;
            lexBuffer.BufferScanLength <- 0;
            lexBuffer.LexemeLength <- 0;
            lexBuffer.BufferAcceptAction <- -1;

        let afterRefill (trans: uint16[][],sentinel,lexBuffer:LexBuffer<char>,scanUntilSentinel,endOfScan,state,eofPos) = 
            // end of file occurs if we couldn't extend the buffer 
            if lexBuffer.BufferScanLength = lexBuffer.BufferMaxScanLength then  
                let snew = int trans.[state].[eofPos] // == EOF 
                if snew = sentinel then 
                    endOfScan()
                else 
                    if lexBuffer.IsPastEndOfStream then failwith "End of file on lexing stream";
                    lexBuffer.IsPastEndOfStream <- true;
                    // printf "state %d --> %d on eof\n" state snew;
                    scanUntilSentinel(lexBuffer,snew)
            else 
                scanUntilSentinel(lexBuffer, state)

        let onAccept (lexBuffer:LexBuffer<char>,a) = 
            lexBuffer.LexemeLength <- lexBuffer.BufferScanLength;
            lexBuffer.BufferAcceptAction <- a;

    open GenericImplFragments


    [<Sealed>]
    type internal UnicodeTables(trans: uint16[] array, accept: uint16[]) = 
        let sentinel = 255 * 256 + 255 
        let numUnicodeCategories = 30 
        let numLowUnicodeChars = 128 
        let numSpecificUnicodeChars = (trans.[0].Length - 1 - numLowUnicodeChars - numUnicodeCategories)/2
        let lookupUnicodeCharacters (state,inp) = 
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
                        let unicodeCategory = System.Char.GetUnicodeCategory(inp)
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
                    // printf "state %d --> %d on '%c' (%d)\n" s snew (char inp) inp;
                    scanUntilSentinel(lexBuffer,snew)
                          
        // Each row for the Unicode table has format 
        //      128 entries for ASCII characters
        //      A variable number of 2*UInt16 entries for SpecificUnicodeChars 
        //      30 entries, one for each UnicodeCategory
        //      1 entry for EOF

        member tables.Interpret(initialState,lexBuffer : LexBuffer<char>) = 
            startInterpret(lexBuffer)
            scanUntilSentinel(lexBuffer, initialState)

        static member Create(trans,accept) = new UnicodeTables(trans,accept)
