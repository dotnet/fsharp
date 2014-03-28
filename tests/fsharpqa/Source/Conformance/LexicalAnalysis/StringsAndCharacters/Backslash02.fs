// #Conformance #LexicalAnalysis 
#light

// Verify the backslash in string literals chomps all whitespace

// Note that it just chews whitespace up to the next newline. So 
// in this example, all spaces on the next line after 'c' but once
// the line ends, the '\r\n' are added, and the '\t' on the same line as d
// is added as well.

let test = "a\
                                      b\
				c\

	d"

printfn "test = [%s]" test
if test <> "abc\r\n\td" then exit 1

exit 0
