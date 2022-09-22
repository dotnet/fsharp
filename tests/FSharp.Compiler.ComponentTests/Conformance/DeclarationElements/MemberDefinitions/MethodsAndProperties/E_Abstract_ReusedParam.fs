// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

type I =
    // Tupled.
    abstract M : i:int * i:int -> int
    // Curried.
    abstract N : i:int -> i:int -> int
    // More than two.
    abstract O : i:int * i: int * i:int -> int
    // Multiple distinct names repeated.
    abstract P : i:int * j:int * i:int * j:int -> int