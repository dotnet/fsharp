// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Internal.Utilities.Collections

open System.Collections.Generic

/// Maps with a specific comparison function
type internal Zmap<'Key, 'T> = Internal.Utilities.Collections.Tagged.Map<'Key, 'T>

module internal Zmap =

    val empty: IComparer<'Key> -> Zmap<'Key, 'T>
    val isEmpty: Zmap<'Key, 'T> -> bool

    val add: 'Key -> 'T -> Zmap<'Key, 'T> -> Zmap<'Key, 'T>
    val remove: 'Key -> Zmap<'Key, 'T> -> Zmap<'Key, 'T>
    val mem: 'Key -> Zmap<'Key, 'T> -> bool
    val memberOf: Zmap<'Key, 'T> -> 'Key -> bool
    val tryFind: 'Key -> Zmap<'Key, 'T> -> 'T option
    val find: 'Key -> Zmap<'Key, 'T> -> 'T // raises KeyNotFoundException

    val map: mapping: ('T -> 'U) -> Zmap<'Key, 'T> -> Zmap<'Key, 'U>
    val mapi: mapping: ('Key -> 'T -> 'U) -> Zmap<'Key, 'T> -> Zmap<'Key, 'U>
    val fold: ('Key -> 'T -> 'U -> 'U) -> Zmap<'Key, 'T> -> 'U -> 'U
    val foldMap: ('State -> 'Key -> 'T -> 'State * 'U) -> 'State -> Zmap<'Key, 'T> -> 'State * Zmap<'Key, 'U>
    val iter: action: ('T -> 'U -> unit) -> Zmap<'T, 'U> -> unit

    val foldSection: 'Key -> 'Key -> ('Key -> 'T -> 'U -> 'U) -> Zmap<'Key, 'T> -> 'U -> 'U

    val first: ('Key -> 'T -> bool) -> Zmap<'Key, 'T> -> ('Key * 'T) option
    val exists: ('Key -> 'T -> bool) -> Zmap<'Key, 'T> -> bool
    val forall: ('Key -> 'T -> bool) -> Zmap<'Key, 'T> -> bool

    val choose: ('Key -> 'T -> 'U option) -> Zmap<'Key, 'T> -> 'U option
    val chooseL: ('Key -> 'T -> 'U option) -> Zmap<'Key, 'T> -> 'U list

    val toList: Zmap<'Key, 'T> -> ('Key * 'T) list
    val ofList: IComparer<'Key> -> ('Key * 'T) list -> Zmap<'Key, 'T>

    val keys: Zmap<'Key, 'T> -> 'Key list
    val values: Zmap<'Key, 'T> -> 'T list
