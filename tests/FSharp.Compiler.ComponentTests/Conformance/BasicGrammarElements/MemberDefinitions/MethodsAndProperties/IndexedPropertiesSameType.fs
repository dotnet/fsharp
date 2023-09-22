// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

type MyIndexerClass() =
    member x.Item
        with get (index: int): string = ""
        and set (index: int) (value: float) = ()