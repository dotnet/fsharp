// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace signingtests

open System
open System.Reflection

#if SHA1
#if DELAY
[<assembly:AssemblyDelaySignAttribute(true)>]
[<assembly:AssemblyKeyFileAttribute("sha1delay.snk")>]
#else
[<assembly:AssemblyKeyFileAttribute("sha1full.snk")>]
#endif
#endif
#if SHA256
#if DELAY
[<assembly:AssemblyDelaySignAttribute(true)>]
[<assembly:AssemblyKeyFileAttribute("sha256delay.snk")>]
#else
[<assembly:AssemblyKeyFileAttribute("sha256full.snk")>]
#endif
#endif
#if SHA512
#if DELAY
[<assembly:AssemblyDelaySignAttribute(true)>]
[<assembly:AssemblyKeyFileAttribute("sha512delay.snk")>]
#else
[<assembly:AssemblyKeyFileAttribute("sha512full.snk")>]
#endif
#endif
#if SHA1024
#if DELAY
[<assembly:AssemblyDelaySignAttribute(true)>]
[<assembly:AssemblyKeyFileAttribute("sha1024delay.snk")>]
#else
[<assembly:AssemblyKeyFileAttribute("sha1024full.snk")>]
#endif
#endif
do ()

