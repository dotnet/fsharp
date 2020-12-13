// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.XmlAdapters

  val s_escapeChars : char []

  val getEscapeSequence : c:char -> string

  val escape : str:string -> string
