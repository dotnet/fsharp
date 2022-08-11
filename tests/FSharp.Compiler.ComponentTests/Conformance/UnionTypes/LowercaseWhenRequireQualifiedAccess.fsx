// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.




[<RequireQualifiedAccess>]
type DU1 =
    | a
    | b
    | c

[<RequireQualifiedAccess>]
type DU2 =
    | a of int
    | B of string
    | C

[<RequireQualifiedAccess>]
type DU3 = | a

[<RequireQualifiedAccess>]
type DU4 = a

[<RequireQualifiedAccess>]
type du1 = du1 of string

[<RequireQualifiedAccess>]
type du2 = | du2 of string

let a = DU1.a
let b = du2.du2
let c = DU2.a(1)
let d = du2.du2("du2")
let e = du1.du1("du1")

let f du1 =
    match du1 with
    | DU1.a -> ()
    | DU1.b -> ()
    | DU1.c -> ()

f DU1.c


