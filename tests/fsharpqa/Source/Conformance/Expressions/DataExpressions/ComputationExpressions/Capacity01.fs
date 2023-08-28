// #Regression #Conformance #DataExpressions #ComputationExpressions #ReqRetail 
// Capacity. Verify creating a computation expression with many let! bindings. 
// Notice: this test will fail on non Retail builds (by design, as per FSHARP1.0:5082)
// Sequence expression. Note that 'let!' in sequence expressions isn't permitted.
// Nesting for loops instead...
let test1 =
    seq {
        for a in 0 .. 0 do
        for b in 0 .. 0 do
        for c in 0 .. 0 do
        for d in 0 .. 0 do
        for e in 0 .. 0 do
        for f in 0 .. 0 do
        for g in 0 .. 0 do
        for h in 0 .. 0 do
        for i in 0 .. 0 do
        for j in 0 .. 0 do
        for k in 0 .. 0 do
        for l in 0 .. 0 do
        for m in 0 .. 0 do
        for n in 0 .. 0 do
        for o in 0 .. 0 do
        for p in 0 .. 0 do
        for q in 0 .. 0 do
        for r in 0 .. 0 do
        for s in 0 .. 0 do
        for t in 0 .. 0 do
        for u in 0 .. 0 do
        for v in 0 .. 0 do
        for w in 0 .. 0 do
        for x in 0 .. 0 do
        for y in 0 .. 0 do
        for z in 0 .. 0 do
            yield (a, b, c, d, e, f, g, j, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z)
    }

// Main test is to see if this compiles
if Seq.length test1 <> 1 then exit 1

// Async workflow
let test2 = 
    async {
        let! a = async { return 1 } in let! b = async { return 1 } in let! c = async { return 1 } in
        let! d = async { return 1 } in let! e = async { return 1 } in let! f = async { return 1 } in
        let! g = async { return 1 } in let! h = async { return 1 } in let! i = async { return 1 } in
        let! j = async { return 1 } in let! k = async { return 1 } in let! l = async { return 1 } in
        let! m = async { return 1 } in let! n = async { return 1 } in let! o = async { return 1 } in
        let! p = async { return 1 } in let! q = async { return 1 } in let! r = async { return 1 } in
        let! s = async { return 1 } in let! t = async { return 1 } in let! u = async { return 1 } in
        let! v = async { return 1 } in let! w = async { return 1 } in let! x = async { return 1 } in
        let! y = async { return 1 } in let! z = async { return 1 }
        
        let! a2= async { return 1 } in let! b2= async { return 1 } in let! c2= async { return 1 } in
        let! d2= async { return 1 } in let! e2= async { return 1 } in let! f2= async { return 1 } in
        let! g2= async { return 1 } in let! h2= async { return 1 } in let! i2= async { return 1 } in
        let! j2= async { return 1 } in let! k2= async { return 1 } in let! l2= async { return 1 } in
        let! m2= async { return 1 } in let! n2= async { return 1 } in let! o2= async { return 1 } in
        let! p2= async { return 1 } in let! q2= async { return 1 } in let! r2= async { return 1 } in
        let! s2= async { return 1 } in let! t2= async { return 1 } in let! u2= async { return 1 } in
        let! v2= async { return 1 } in let! w2= async { return 1 } in let! x2= async { return 1 } in
        let! y2= async { return 1 } in let! z2= async { return 1 }
         
        let! a3= async { return 1 } in let! b3= async { return 1 } in let! c3= async { return 1 } in
        let! d3= async { return 1 } in let! e3= async { return 1 } in let! f3= async { return 1 } in
        let! g3= async { return 1 } in let! h3= async { return 1 } in let! i3= async { return 1 } in
        let! j3= async { return 1 } in let! k3= async { return 1 } in let! l3= async { return 1 } in
        let! m3= async { return 1 } in let! n3= async { return 1 } in let! o3= async { return 1 } in
        let! p3= async { return 1 } in let! q3= async { return 1 } in let! r3= async { return 1 } in
        let! s3= async { return 1 } in let! t3= async { return 1 } in let! u3= async { return 1 } in
        let! v3= async { return 1 } in let! w3= async { return 1 } in let! x3= async { return 1 } in
        let! y3= async { return 1 } in let! z3= async { return 1 }

        return (a, b2, c3, d, e2, f3, g, j2, i3, j, k2, l3, m, n2, o3, p, q2, r3, s, t2, u3, v, w2, x3, y, z2)
    } |> Async.RunSynchronously
    
if test2 <> (1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1) then exit 1

exit 0
