// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Internal.Utilities.FileSystem

  [<Class>]
  type internal File =
      static member SafeExists : filename:string -> bool

  [<Class>]
  type internal Path =
      static member IsInvalidDirectory : path:string -> bool
      static member IsInvalidPath : path:string -> bool
