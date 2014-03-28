namespace Microsoft.FSharp.Compatibility

/// Compatibility module for string processing.  Richer string operations
/// are available via the member functions on strings and other functionality in
/// the <c>System.String</c> type
/// and the <c>System.Text.RegularExpressions</c> namespace.
[<RequireQualifiedAccess>]
module String = 

    /// Return a string with the first character converted to uppercase.
    val capitalize: string -> string

    /// Return a string with the first character converted to lowercase.
    val uncapitalize: string -> string

#if FX_NO_STRING_SPLIT_OPTIONS
#else
    /// Split the string using the given list of separator characters.
    /// Trimming is also performed at both ends of the string and any empty
    /// strings that result from the split are discarded.
    val split: char list -> (string -> string list)
#endif

    /// Removes all occurrences of a set of characters specified in a
    /// list from the beginning and end of this instance.
    val trim: char list -> (string -> string)

    /// Compare the given strings using ordinal comparison
    [<CompilerMessage("This construct is for ML compatibility. Consider using 'Operators.compare' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val compare: string -> string -> int

    /// Returns the character at the specified position in the string
    [<CompilerMessage("This construct is for ML compatibility. Consider using 'str.[i]' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val get: string -> int -> char

    /// Return a substring of length 'length' starting index 'start'.
    [<CompilerMessage("This construct is for ML compatibility. Consider using 'str.[i..j]' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val sub: string -> start:int -> length:int -> string

    /// Return a new string with all characters converted to lowercase
    [<CompilerMessage("This construct is for ML compatibility. Consider using 'str.ToLower()' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val lowercase: string -> string

    /// Return a string of the given length containing repetitions of the given character
    [<CompilerMessage("This construct is for ML compatibility. Consider using 'String.replicate' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val make: int -> char -> string

    /// Return s string of length 1 containing the given character
    [<CompilerMessage("This construct is for ML compatibility. Consider using the overloaded 'string' operator instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val of_char: char -> string

    /// Return true is the given string contains the given character
    [<CompilerMessage("This construct is for ML compatibility. Consider using 'str.Contains' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val contains: string -> char -> bool

    /// Return true is the given string contains the given character in the
    /// range specified by the given start index and the given length
    [<CompilerMessage("This construct is for ML compatibility. Consider using 'str.IndexOf' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val contains_between: string -> start:int -> length:int -> char -> bool

    /// Return true is the given string contains the given character in the
    /// range from the given start index to the end of the string.
    [<CompilerMessage("This construct is for ML compatibility. Consider using 'str.IndexOf' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val contains_from: string -> int -> char -> bool

    /// Return the first index of the given character in the
    /// string.  Raise <c>KeyNotFoundException</c> if
    /// the string does not contain the given character.
    [<CompilerMessage("This construct is for ML compatibility. Consider using 'str.IndexOf' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val index: string -> char -> int

    /// Return the first index of the given character in the
    /// range from the given start position to the end of the string.  
    /// Raise <c>KeyNotFoundException</c> if
    /// the string does not contain the given character.
    [<CompilerMessage("This construct is for ML compatibility. Consider using 'str.IndexOf' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val index_from: string -> start:int -> char -> int

    /// Return true if the string contains the given character prior to the given index
    [<CompilerMessage("This construct is for ML compatibility. Consider using 'str.LastIndexOf' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val rcontains_from: string -> start:int -> char -> bool

    /// Return the index of the first occurrence of the given character 
    /// from the end of the string proceeding backwards
    [<CompilerMessage("This construct is for ML compatibility. Consider using 'str.LastIndexOf' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val rindex: string -> char -> int

    /// Return the index of the first occurrence of the given character 
    /// starting from the given index proceeding backwards.
    [<CompilerMessage("This construct is for ML compatibility. Consider using 'str.LastIndexOf' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val rindex_from: string -> start:int -> char -> int

    /// Return a string with all characters converted to uppercase.
    [<CompilerMessage("This construct is for ML compatibility. Consider using 'str.ToUpper' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
    val uppercase: string -> string

