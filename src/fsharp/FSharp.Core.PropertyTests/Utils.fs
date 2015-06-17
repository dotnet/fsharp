// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.PropertyTests.Utils

type Result<'a> = 
| Success of 'a
| Error of string

let run f = 
    try
        Success(f())
    with
    | exn -> Error(exn.Message)

let runAndCheckErrorType f = 
    try
        Success(f())
    with
    | exn -> Error(exn.GetType().ToString())

let runAndCheckIfAnyError f = 
    try
        Success(f())
    with
    | exn -> Error("")