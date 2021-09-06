// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test

[<TestFixture>]
module ``Cast to Units Tests`` =
    
    [<Test>]
    let ``Casting to Measures should compile``() =
        CompilerAssert.PassWithOptions [| "--langversion:preview" |]
            """
module M

open Microsoft.FSharp.Core.LanguagePrimitives

[<Measure>]
type M

let r1 = SByteWithMeasure<M> 1y + 2y<M>
let r2 = Int16WithMeasure<M> 2s - 2s<M>
let r3 = Int32WithMeasure<M> 3 * 3<M>
let r4 = Int64WithMeasure<M> 5L / 5L<M>
let r5a = FloatWithMeasure<M> 11.11 + 1.1<M>
let r5b = FloatWithMeasure<M> 0x0000000000000010LF + 0x0000000000000001LF<M>
let r6a = Float32WithMeasure<M> 22.22f - 11.11f<M>
let r6b = Float32WithMeasure<M> 22.22F - 11.11F<M>
let r6c = Float32WithMeasure<M> 0x00000010lf - 0x00000001lf<M>
let r7a = DecimalWithMeasure<M> 33.33M * 44.44M<M>
let r7b = DecimalWithMeasure<M> 33.33m * 44.44m<M>
let r8 = ByteWithMeasure<M> 1uy + 2uy<M>
let r9 = UInt16WithMeasure<M> 1us + 2us<M>
let r10a = UInt32WithMeasure<M> 1u + 2u<M>
let r10b = UInt32WithMeasure<M> 1ul + 2ul<M>
let r11 = UInt64WithMeasure<M> 1UL + 2UL<M>
let r12 = IntPtrWithMeasure<M> 1n + 2n<M>
let r13 = UIntPtrWithMeasure<M> 1un + 2un<M>
            """
