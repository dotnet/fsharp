// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities

[<TestFixture>]
module ``Cast to Units Tests`` =
    
    [<Test>]
    let ``Casting to Measures should compile``() =
        CompilerAssert.Pass
            """
module M

open Microsoft.FSharp.Core.LanguagePrimitives

[<Measure>]
type M

let r1 = SByteWithMeasure<M> 1y + 2y<M>
let r2 = Int16WithMeasure<M> 2s - 2s<M>
let r3 = Int32WithMeasure<M> 3 * 3<M>
let r4 = Int64WithMeasure<M> 5L / 5L<M>
let r5 = FloatWithMeasure<M> 11.11 + 1.1<M>
let r6 = Float32WithMeasure<M> 22.22f - 11.11f<M>
let r7 = DecimalWithMeasure<M> 33.33M * 44.44M<M>
            """
