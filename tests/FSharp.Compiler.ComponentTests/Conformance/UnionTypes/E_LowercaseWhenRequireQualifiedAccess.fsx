﻿// #Conformance #TypesAndModules #Unions 
// This testcase verifies that lower-case discriminated union are not allowed
//<Expects status="error"></Expects>
#light

type DU1 = | ``not.allowed``

type DU2 = ``not.allowed``

[<RequireQualifiedAccess>]
type DU3 = | ``not.allowed``

[<RequireQualifiedAccess>]
type DU4 = ``not.allowed``

type DU5 = | a

type DU6 = a

type du1 = du1 of string

type du2 = | du2 of string