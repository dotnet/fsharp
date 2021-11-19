#r "nuget: FsCheck, 3.0.0-alpha4"

// See https://github.com/dotnet/fsharp/pull/12420 and https://github.com/dotnet/fsharp/issues/12322

type ReproBuilder () =
    member _.Delay x = printfn "Delay"; x ()
    member _.Yield  (x) = printfn "Yield"; x
    member _.Combine (x, y) = printfn "Combine"; x + y

let repro = ReproBuilder ()

// The de-sugaring of this is a mass of nested function calls
let reallyBigComputationExpression () =
    repro {
        // Commenting out some of the below is enough to avoid StackOverflow on my machine.
        0
        1
        2
        3
        4
        5
        6
        7
        8
        9
        0
        1
        2
        3
        4
        5
        6
        7
        8
        9
        0
        1
        2
        3
        4
        5
        6
        7
        8
        9
        0
        1
        2
        3
        4
        5
        6
        7
        8
        9
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
        4
        0
        1
        2
        3
    }

let f x = printfn "call"; printfn "call"; printfn "call"; printfn "call"; x

let manyPipes () =
    1 |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f
    |> f |> f |> f |> f |> f |> f |> f |> f |> f |> f


let deepCalls () =
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
    f(f(f(f(f(f(f(f(f(f(
         1
    ))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))
    ))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))

open FsCheck

/// This was another repro for computation expressions
let g =
    gen {
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        let! _ = Arb.generate<int>
        return ()
    }

/// This was failing when writing debug scopes
module LotsOfLets =
    //let a1 = "foo"
    let a2 = "foo"
    let a3 = "foo"
    let a4 = "foo"
    let a5 = "foo"
    let a6 = "foo"
    let a7 = "foo"
    let a8 = "foo"
    let a9 = "foo"
    let a10 = "foo"
    let a11 = "foo"
    let a12 = "foo"
    let a13 = "foo"
    let a14 = "foo"
    let a15 = "foo"
    let a16 = "foo"
    let a17 = "foo"
    let a18 = "foo"
    let a19 = "foo"
    let a20 = "foo"
    let a21 = "foo"
    let a22 = "foo"
    let a23 = "foo"
    let a24 = "foo"
    let a25 = "foo"
    let a26 = "foo"
    let a27 = "foo"
    let a28 = "foo"
    let a29 = "foo"
    let a30 = "foo"
    let a31 = "foo"
    let a32 = "foo"
    let a33 = "foo"
    let a34 = "foo"
    let a35 = "foo"
    let a36 = "foo"
    let a37 = "foo"
    let a38 = "foo"
    let a39 = "foo"
    let a40 = "foo"
    let a41 = "foo"
    let a42 = "foo"
    let a43 = "foo"
    let a44 = "foo"
    let a45 = "foo"
    let a46 = "foo"
    let a47 = "foo"
    let a48 = "foo"
    let a49 = "foo"
    let a50 = "foo"
    let a51 = "foo"
    let a52 = "foo"
    let a53 = "foo"
    let a54 = "foo"
    let a55 = "foo"
    let a56 = "foo"
    let a57 = "foo"
    let a58 = "foo"
    let a59 = "foo"
    let a60 = "foo"
    let a61 = "foo"
    let a62 = "foo"
    let a63 = "foo"
    let a64 = "foo"
    let a65 = "foo"
    let a66 = "foo"
    let a67 = "foo"
    let a68 = "foo"
    let a69 = "foo"
    let a70 = "foo"
    let a71 = "foo"
    let a72 = "foo"
    let a73 = "foo"
    let a74 = "foo"
    let a75 = "foo"
    let a76 = "foo"
    let a77 = "foo"
    let a78 = "foo"
    let a79 = "foo"
    let a80 = "foo"
    let a81 = "foo"
    let a82 = "foo"
    let a83 = "foo"
    let a84 = "foo"
    let a85 = "foo"
    let a86 = "foo"
    let a87 = "foo"
    let a88 = "foo"
    let a89 = "foo"
    let a90 = "foo"
    let a91 = "foo"
    let a92 = "foo"
    let a93 = "foo"
    let a94 = "foo"
    let a95 = "foo"
    let a96 = "foo"
    let a97 = "foo"
    let a98 = "foo"
    let a99 = "foo"
    let a100 = "foo"
    let a101 = "foo"
    let a102 = "foo"
    let a103 = "foo"
    let a104 = "foo"
    let a105 = "foo"
    let a106 = "foo"
    let a107 = "foo"
    let a108 = "foo"
    let a109 = "foo"
    let a110 = "foo"
    let a111 = "foo"
    let a112 = "foo"
    let a113 = "foo"
    let a114 = "foo"
    let a115 = "foo"
    let a116 = "foo"
    let a117 = "foo"
    let a118 = "foo"
    let a119 = "foo"
    let a120 = "foo"
    let a121 = "foo"
    let a122 = "foo"
    let a123 = "foo"
    let a124 = "foo"
    let a125 = "foo"
    let a126 = "foo"
    let a127 = "foo"
    let a128 = "foo"
    let a129 = "foo"
    let a130 = "foo"
    let a131 = "foo"
    let a132 = "foo"
    let a133 = "foo"
    let a134 = "foo"
    let a135 = "foo"
    let a136 = "foo"
    let a137 = "foo"
    let a138 = "foo"
    let a139 = "foo"
    let a140 = "foo"
    let a141 = "foo"
    let a142 = "foo"
    let a143 = "foo"
    let a144 = "foo"
    let a145 = "foo"
    let a146 = "foo"
    let a147 = "foo"
    let a148 = "foo"
    let a149 = "foo"
    let a150 = "foo"
    let a151 = "foo"
    let a152 = "foo"
    let a153 = "foo"
    let a154 = "foo"
    let a155 = "foo"
    let a156 = "foo"
    let a157 = "foo"
    let a158 = "foo"
    let a159 = "foo"
    let a160 = "foo"
    let a161 = "foo"
    let a162 = "foo"
    let a163 = "foo"
    let a164 = "foo"
    let a165 = "foo"
    let a166 = "foo"
    let a167 = "foo"
    let a168 = "foo"
    let a169 = "foo"
    let a170 = "foo"
    let a171 = "foo"
    let a172 = "foo"
    let a173 = "foo"
    let a174 = "foo"
    let a175 = "foo"
    let a176 = "foo"
    let a177 = "foo"
    let a178 = "foo"
    let a179 = "foo"
    let a180 = "foo"
    let a181 = "foo"
    let a182 = "foo"
    let a183 = "foo"
    let a184 = "foo"
    let a185 = "foo"
    let a186 = "foo"
    let a187 = "foo"
    let a188 = "foo"
    let a189 = "foo"
    let a190 = "foo"
    let a191 = "foo"
    let a192 = "foo"
    let a193 = "foo"
    let a194 = "foo"
    let a195 = "foo"
    let a196 = "foo"
    let a197 = "foo"
    let a198 = "foo"
    let a199 = "foo"
    let a200 = "foo"
    let a201 = "foo"
    let a202 = "foo"
    let a203 = "foo"
    let a204 = "foo"
    let a205 = "foo"
    let a206 = "foo"
    let a207 = "foo"
    let a208 = "foo"
    let a209 = "foo"
    let a210 = "foo"
    let a211 = "foo"
    let a212 = "foo"
    let a213 = "foo"
    let a214 = "foo"
    let a215 = "foo"
    let a216 = "foo"
    let a217 = "foo"
    let a218 = "foo"
    let a219 = "foo"
    let a220 = "foo"
    let a221 = "foo"
    let a222 = "foo"
    let a223 = "foo"
    let a224 = "foo"
    let a225 = "foo"
    let a226 = "foo"
    let a227 = "foo"
    let a228 = "foo"
    let a229 = "foo"
    let a230 = "foo"
    let a231 = "foo"
    let a232 = "foo"
    let a233 = "foo"
    let a234 = "foo"
    let a235 = "foo"
    let a236 = "foo"
    let a237 = "foo"
    let a238 = "foo"
    let a239 = "foo"
    let a240 = "foo"
    let a241 = "foo"
    let a242 = "foo"
    let a243 = "foo"
    let a244 = "foo"
    let a245 = "foo"
    let a246 = "foo"
    let a247 = "foo"
    let a248 = "foo"
    let a249 = "foo"
    let a250 = "foo"
    let a251 = "foo"
    let a252 = "foo"
    let a253 = "foo"
    let a254 = "foo"
    let a255 = "foo"
    let a256 = "foo"
    let a257 = "foo"
    let a258 = "foo"
    let a259 = "foo"
    let a260 = "foo"
    let a261 = "foo"
    let a262 = "foo"
    let a263 = "foo"
    let a264 = "foo"
    let a265 = "foo"
    let a266 = "foo"
    let a267 = "foo"
    let a268 = "foo"
    let a269 = "foo"
    let a270 = "foo"
    let a271 = "foo"
    let a272 = "foo"
    let a273 = "foo"
    let a274 = "foo"
    let a275 = "foo"
    let a276 = "foo"
    let a277 = "foo"
    let a278 = "foo"
    let a279 = "foo"
    let a280 = "foo"
    let a281 = "foo"
    let a282 = "foo"
    let a283 = "foo"
    let a284 = "foo"
    let a285 = "foo"
    let a286 = "foo"
    let a287 = "foo"
    let a288 = "foo"
    let a289 = "foo"
    let a290 = "foo"
    let a291 = "foo"
    let a292 = "foo"
    let a293 = "foo"
    let a294 = "foo"
    let a295 = "foo"
    let a296 = "foo"
    let a297 = "foo"
    let a298 = "foo"
    let a299 = "foo"
    let a300 = "foo"
    let a301 = "foo"
    let a302 = "foo"
    let a303 = "foo"
    let a304 = "foo"
    let a305 = "foo"
    let a306 = "foo"
    let a307 = "foo"
    let a308 = "foo"
    let a309 = "foo"
    let a310 = "foo"
    let a311 = "foo"
    let a312 = "foo"
    let a313 = "foo"
    let a314 = "foo"
    let a315 = "foo"
    let a316 = "foo"
    let a317 = "foo"
    let a318 = "foo"
    let a319 = "foo"
    let a320 = "foo"
    let a321 = "foo"
    let a322 = "foo"
    let a323 = "foo"
    let a324 = "foo"
    let a325 = "foo"
    let a326 = "foo"
    let a327 = "foo"
    let a328 = "foo"
    let a329 = "foo"
    let a330 = "foo"
    let a331 = "foo"
    let a332 = "foo"
    let a333 = "foo"
    let a334 = "foo"
    let a335 = "foo"
    let a336 = "foo"
    let a337 = "foo"
    let a338 = "foo"
    let a339 = "foo"
    let a340 = "foo"
    let a341 = "foo"
    let a342 = "foo"
    let a343 = "foo"
    let a344 = "foo"
    let a345 = "foo"
    let a346 = "foo"
    let a347 = "foo"
    let a348 = "foo"
    let a349 = "foo"
    let a350 = "foo"
    let a351 = "foo"
    let a352 = "foo"
    let a353 = "foo"
    let a354 = "foo"
    let a355 = "foo"
    let a356 = "foo"
    let a357 = "foo"
    let a358 = "foo"
    let a359 = "foo"
    let a360 = "foo"
    let a361 = "foo"
    let a362 = "foo"
    let a363 = "foo"
    let a364 = "foo"
    let a365 = "foo"
    let a366 = "foo"
    let a367 = "foo"
    let a368 = "foo"
    let a369 = "foo"
    let a370 = "foo"
    let a371 = "foo"
    let a372 = "foo"
    let a373 = "foo"
    let a374 = "foo"
    let a375 = "foo"
    let a376 = "foo"
    let a377 = "foo"
    let a378 = "foo"
    let a379 = "foo"
    let a380 = "foo"
    let a381 = "foo"
    let a382 = "foo"
    let a383 = "foo"
    let a384 = "foo"
    let a385 = "foo"
    let a386 = "foo"
    let a387 = "foo"
    let a388 = "foo"
    let a389 = "foo"
    let a390 = "foo"
    let a391 = "foo"
    let a392 = "foo"
    let a393 = "foo"
    let a394 = "foo"
    let a395 = "foo"
    let a396 = "foo"
    let a397 = "foo"
    let a398 = "foo"
    let a399 = "foo"
    let a400 = "foo"
    let a401 = "foo"
    let a402 = "foo"
    let a403 = "foo"
    let a404 = "foo"
    let a405 = "foo"
    let a406 = "foo"
    let a407 = "foo"
    let a408 = "foo"
    let a409 = "foo"
    let a410 = "foo"
    let a411 = "foo"
    let a412 = "foo"
    let a413 = "foo"
    let a414 = "foo"
    let a415 = "foo"
    let a416 = "foo"
    let a417 = "foo"
    let a418 = "foo"
    let a419 = "foo"
    let a420 = "foo"
    let a421 = "foo"
    let a422 = "foo"
    let a423 = "foo"
    let a424 = "foo"
    let a425 = "foo"
    let a426 = "foo"
    let a427 = "foo"
    let a428 = "foo"
    let a429 = "foo"
    let a430 = "foo"
    let a431 = "foo"
    let a432 = "foo"
    let a433 = "foo"
    let a434 = "foo"
    let a435 = "foo"
    let a436 = "foo"
    let a437 = "foo"
    let a438 = "foo"
    let a439 = "foo"
    let a440 = "foo"
    let a441 = "foo"
    let a442 = "foo"
    let a443 = "foo"
    let a444 = "foo"
    let a445 = "foo"
    let a446 = "foo"
    let a447 = "foo"
    let a448 = "foo"
    let a449 = "foo"
    let a450 = "foo"
    let a451 = "foo"
    let a452 = "foo"
    let a453 = "foo"
    let a454 = "foo"
    let a455 = "foo"
    let a456 = "foo"
    let a457 = "foo"
    let a458 = "foo"
    let a459 = "foo"
    let a460 = "foo"
    let a461 = "foo"
    let a462 = "foo"
    let a463 = "foo"
    let a464 = "foo"
    let a465 = "foo"
    let a466 = "foo"
    let a467 = "foo"
    let a468 = "foo"
    let a469 = "foo"
    let a470 = "foo"
    let a471 = "foo"
    let a472 = "foo"
    let a473 = "foo"
    let a474 = "foo"
    let a475 = "foo"
    let a476 = "foo"
    let a477 = "foo"
    let a478 = "foo"
    let a479 = "foo"
    let a480 = "foo"
    let a481 = "foo"
    let a482 = "foo"
    let a483 = "foo"
    let a484 = "foo"
    let a485 = "foo"
    let a486 = "foo"
    let b2 = "foo"
    let b3 = "foo"
    let b4 = "foo"
    let b5 = "foo"
    let b6 = "foo"
    let b7 = "foo"
    let b8 = "foo"
    let b9 = "foo"
    let b10 = "foo"
    let b11 = "foo"
    let b12 = "foo"
    let b13 = "foo"
    let b14 = "foo"
    let b15 = "foo"
    let b16 = "foo"
    let b17 = "foo"
    let b18 = "foo"
    let b19 = "foo"
    let b20 = "foo"
    let b21 = "foo"
    let b22 = "foo"
    let b23 = "foo"
    let b24 = "foo"
    let b25 = "foo"
    let b26 = "foo"
    let b27 = "foo"
    let b28 = "foo"
    let b29 = "foo"
    let b30 = "foo"
    let b31 = "foo"
    let b32 = "foo"
    let b33 = "foo"
    let b34 = "foo"
    let b35 = "foo"
    let b36 = "foo"
    let b37 = "foo"
    let b38 = "foo"
    let b39 = "foo"
    let b40 = "foo"
    let b41 = "foo"
    let b42 = "foo"
    let b43 = "foo"
    let b44 = "foo"
    let b45 = "foo"
    let b46 = "foo"
    let b47 = "foo"
    let b48 = "foo"
    let b49 = "foo"
    let b50 = "foo"
    let b51 = "foo"
    let b52 = "foo"
    let b53 = "foo"
    let b54 = "foo"
    let b55 = "foo"
    let b56 = "foo"
    let b57 = "foo"
    let b58 = "foo"
    let b59 = "foo"
    let b60 = "foo"
    let b61 = "foo"
    let b62 = "foo"
    let b63 = "foo"
    let b64 = "foo"
    let b65 = "foo"
    let b66 = "foo"
    let b67 = "foo"
    let b68 = "foo"
    let b69 = "foo"
    let b70 = "foo"
    let b71 = "foo"
    let b72 = "foo"
    let b73 = "foo"
    let b74 = "foo"
    let b75 = "foo"
    let b76 = "foo"
    let b77 = "foo"
    let b78 = "foo"
    let b79 = "foo"
    let b80 = "foo"
    let b81 = "foo"
    let b82 = "foo"
    let b83 = "foo"
    let b84 = "foo"
    let b85 = "foo"
    let b86 = "foo"
    let b87 = "foo"
    let b88 = "foo"
    let b89 = "foo"
    let b90 = "foo"
    let b91 = "foo"
    let b92 = "foo"
    let b93 = "foo"
    let b94 = "foo"
    let b95 = "foo"
    let b96 = "foo"
    let b97 = "foo"
    let b98 = "foo"
    let b99 = "foo"
    let b100 = "foo"
    let b101 = "foo"
    let b102 = "foo"
    let b103 = "foo"
    let b104 = "foo"
    let b105 = "foo"
    let b106 = "foo"
    let b107 = "foo"
    let b108 = "foo"
    let b109 = "foo"
    let b110 = "foo"
    let b111 = "foo"
    let b112 = "foo"
    let b113 = "foo"
    let b114 = "foo"
    let b115 = "foo"
    let b116 = "foo"
    let b117 = "foo"
    let b118 = "foo"
    let b119 = "foo"
    let b120 = "foo"
    let b121 = "foo"
    let b122 = "foo"
    let b123 = "foo"
    let b124 = "foo"
    let b125 = "foo"
    let b126 = "foo"
    let b127 = "foo"
    let b128 = "foo"
    let b129 = "foo"
    let b130 = "foo"
    let b131 = "foo"
    let b132 = "foo"
    let b133 = "foo"
    let b134 = "foo"
    let b135 = "foo"
    let b136 = "foo"
    let b137 = "foo"
    let b138 = "foo"
    let b139 = "foo"
    let b140 = "foo"
    let b141 = "foo"
    let b142 = "foo"
    let b143 = "foo"
    let b144 = "foo"
    let b145 = "foo"
    let b146 = "foo"
    let b147 = "foo"
    let b148 = "foo"
    let b149 = "foo"
    let b150 = "foo"
    let b151 = "foo"
    let b152 = "foo"
    let b153 = "foo"
    let b154 = "foo"
    let b155 = "foo"
    let b156 = "foo"
    let b157 = "foo"
    let b158 = "foo"
    let b159 = "foo"
    let b160 = "foo"
    let b161 = "foo"
    let b162 = "foo"
    let b163 = "foo"
    let b164 = "foo"
    let b165 = "foo"
    let b166 = "foo"
    let b167 = "foo"
    let b168 = "foo"
    let b169 = "foo"
    let b170 = "foo"
    let b171 = "foo"
    let b172 = "foo"
    let b173 = "foo"
    let b174 = "foo"
    let b175 = "foo"
    let b176 = "foo"
    let b177 = "foo"
    let b178 = "foo"
    let b179 = "foo"
    let b180 = "foo"
    let b181 = "foo"
    let b182 = "foo"
    let b183 = "foo"
    let b184 = "foo"
    let b185 = "foo"
    let b186 = "foo"
    let b187 = "foo"
    let b188 = "foo"
    let b189 = "foo"
    let b190 = "foo"
    let b191 = "foo"
    let b192 = "foo"
    let b193 = "foo"
    let b194 = "foo"
    let b195 = "foo"
    let b196 = "foo"
    let b197 = "foo"
    let b198 = "foo"
    let b199 = "foo"
    let b200 = "foo"
    let b201 = "foo"
    let b202 = "foo"
    let b203 = "foo"
    let b204 = "foo"
    let b205 = "foo"
    let b206 = "foo"
    let b207 = "foo"
    let b208 = "foo"
    let b209 = "foo"
    let b210 = "foo"
    let b211 = "foo"
    let b212 = "foo"
    let b213 = "foo"
    let b214 = "foo"
    let b215 = "foo"
    let b216 = "foo"
    let b217 = "foo"
    let b218 = "foo"
    let b219 = "foo"
    let b220 = "foo"
    let b221 = "foo"
    let b222 = "foo"
    let b223 = "foo"
    let b224 = "foo"
    let b225 = "foo"
    let b226 = "foo"
    let b227 = "foo"
    let b228 = "foo"
    let b229 = "foo"
    let b230 = "foo"
    let b231 = "foo"
    let b232 = "foo"
    let b233 = "foo"
    let b234 = "foo"
    let b235 = "foo"
    let b236 = "foo"
    let b237 = "foo"
    let b238 = "foo"
    let b239 = "foo"
    let b240 = "foo"
    let b241 = "foo"
    let b242 = "foo"
    let b243 = "foo"
    let b244 = "foo"
    let b245 = "foo"
    let b246 = "foo"
    let b247 = "foo"
    let b248 = "foo"
    let b249 = "foo"
    let b250 = "foo"
    let b251 = "foo"
    let b252 = "foo"
    let b253 = "foo"
    let b254 = "foo"
    let b255 = "foo"
    let b256 = "foo"
    let b257 = "foo"
    let b258 = "foo"
    let b259 = "foo"
    let b260 = "foo"
    let b261 = "foo"
    let b262 = "foo"
    let b263 = "foo"
    let b264 = "foo"
    let b265 = "foo"
    let b266 = "foo"
    let b267 = "foo"
    let b268 = "foo"
    let b269 = "foo"
    let b270 = "foo"
    let b271 = "foo"
    let b272 = "foo"
    let b273 = "foo"
    let b274 = "foo"
    let b275 = "foo"
    let b276 = "foo"
    let b277 = "foo"
    let b278 = "foo"
    let b279 = "foo"
    let b280 = "foo"
    let b281 = "foo"
    let b282 = "foo"
    let b283 = "foo"
    let b284 = "foo"
    let b285 = "foo"
    let b286 = "foo"
    let b287 = "foo"
    let b288 = "foo"
    let b289 = "foo"
    let b290 = "foo"
    let b291 = "foo"
    let b292 = "foo"
    let b293 = "foo"
    let b294 = "foo"
    let b295 = "foo"
    let b296 = "foo"
    let b297 = "foo"
    let b298 = "foo"
    let b299 = "foo"
    let b300 = "foo"
    let b301 = "foo"
    let b302 = "foo"
    let b303 = "foo"
    let b304 = "foo"
    let b305 = "foo"
    let b306 = "foo"
    let b307 = "foo"
    let b308 = "foo"
    let b309 = "foo"
    let b310 = "foo"
    let b311 = "foo"
    let b312 = "foo"
    let b313 = "foo"
    let b314 = "foo"
    let b315 = "foo"
    let b316 = "foo"
    let b317 = "foo"
    let b318 = "foo"
    let b319 = "foo"
    let b320 = "foo"
    let b321 = "foo"
    let b322 = "foo"
    let b323 = "foo"
    let b324 = "foo"
    let b325 = "foo"
    let b326 = "foo"
    let b327 = "foo"
    let b328 = "foo"
    let b329 = "foo"
    let b330 = "foo"
    let b331 = "foo"
    let b332 = "foo"
    let b333 = "foo"
    let b334 = "foo"
    let b335 = "foo"
    let b336 = "foo"
    let b337 = "foo"
    let b338 = "foo"
    let b339 = "foo"
    let b340 = "foo"
    let b341 = "foo"
    let b342 = "foo"
    let b343 = "foo"
    let b344 = "foo"
    let b345 = "foo"
    let b346 = "foo"
    let b347 = "foo"
    let b348 = "foo"
    let b349 = "foo"
    let b350 = "foo"
    let b351 = "foo"
    let b352 = "foo"
    let b353 = "foo"
    let b354 = "foo"
    let b355 = "foo"
    let b356 = "foo"
    let b357 = "foo"
    let b358 = "foo"
    let b359 = "foo"
    let b360 = "foo"
    let b361 = "foo"
    let b362 = "foo"
    let b363 = "foo"
    let b364 = "foo"
    let b365 = "foo"
    let b366 = "foo"
    let b367 = "foo"
    let b368 = "foo"
    let b369 = "foo"
    let b370 = "foo"
    let b371 = "foo"
    let b372 = "foo"
    let b373 = "foo"
    let b374 = "foo"
    let b375 = "foo"
    let b376 = "foo"
    let b377 = "foo"
    let b378 = "foo"
    let b379 = "foo"
    let b380 = "foo"
    let b381 = "foo"
    let b382 = "foo"
    let b383 = "foo"
    let b384 = "foo"
    let b385 = "foo"
    let b386 = "foo"
    let b387 = "foo"
    let b388 = "foo"
    let b389 = "foo"
    let b390 = "foo"
    let b391 = "foo"
    let b392 = "foo"
    let b393 = "foo"
    let b394 = "foo"
    let b395 = "foo"
    let b396 = "foo"
    let b397 = "foo"
    let b398 = "foo"
    let b399 = "foo"
    let b400 = "foo"
    let b401 = "foo"
    let b402 = "foo"
    let b403 = "foo"
    let b404 = "foo"
    let b405 = "foo"
    let b406 = "foo"
    let b407 = "foo"
    let b408 = "foo"
    let b409 = "foo"
    let b410 = "foo"
    let b411 = "foo"
    let b412 = "foo"
    let b413 = "foo"
    let b414 = "foo"
    let b415 = "foo"
    let b416 = "foo"
    let b417 = "foo"
    let b418 = "foo"
    let b419 = "foo"
    let b420 = "foo"
    let b421 = "foo"
    let b422 = "foo"
    let b423 = "foo"
    let b424 = "foo"
    let b425 = "foo"
    let b426 = "foo"
    let b427 = "foo"
    let b428 = "foo"
    let b429 = "foo"
    let b430 = "foo"
    let b431 = "foo"
    let b432 = "foo"
    let b433 = "foo"
    let b434 = "foo"
    let b435 = "foo"
    let b436 = "foo"
    let b437 = "foo"
    let b438 = "foo"
    let b439 = "foo"
    let b440 = "foo"
    let b441 = "foo"
    let b442 = "foo"
    let b443 = "foo"
    let b444 = "foo"
    let b445 = "foo"
    let b446 = "foo"
    let b447 = "foo"
    let b448 = "foo"
    let b449 = "foo"
    let b450 = "foo"
    let b451 = "foo"
    let b452 = "foo"
    let b453 = "foo"
    let b454 = "foo"
    let b455 = "foo"
    let b456 = "foo"
    let b457 = "foo"
    let b458 = "foo"
    let b459 = "foo"
    let b460 = "foo"
    let b461 = "foo"
    let b462 = "foo"
    let b463 = "foo"
    let b464 = "foo"
    let b465 = "foo"
    let b466 = "foo"
    let b467 = "foo"
    let b468 = "foo"
    let b469 = "foo"
    let b470 = "foo"
    let b471 = "foo"
    let b472 = "foo"
    let b473 = "foo"
    let b474 = "foo"
    let b475 = "foo"
    let b476 = "foo"
    let b477 = "foo"
    let b478 = "foo"
    let b479 = "foo"
    let b480 = "foo"
    let b481 = "foo"
    let b482 = "foo"
    let b483 = "foo"
    let b484 = "foo"
    let b485 = "foo"
    let b486 = "foo"

// This is a compilation test, not a lot actually happens in the test
do (System.Console.Out.WriteLine "Test Passed"; 
    System.IO.File.WriteAllText("test.ok", "ok"); 
    exit 0)

