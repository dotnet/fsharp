// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Some filename operations.    
module internal Internal.Utilities.Filename

exception IllegalFileNameChar of string * char

/// "checkSuffix f s" returns true if filename "f" ends in suffix "s",
/// e.g. checkSuffix "abc.fs" ".fs" returns true.
val checkSuffix: string -> string -> bool

/// "chopExtension f" removes the extension from the given
/// filename. Raises ArgumentException if no extension is present.
val chopExtension: string -> string

/// "directoryName" " decomposes a filename into a directory name
val directoryName: string -> string

/// Return true if the filename has a "." extension
val hasExtension: string -> bool

/// Get the filename of the given path
val fileNameOfPath: string -> string

/// Get the filename without extension of the given path
val fileNameWithoutExtension: string -> string


