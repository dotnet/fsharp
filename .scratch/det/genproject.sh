#!/usr/bin/env bash
# Generate N files that exercise: anon records, tuple-arg funcs, nested lambdas,
# generic comparison/equality augmentation.
N=${1:-12}
OUT=${2:-./src}
rm -rf "$OUT"
mkdir -p "$OUT"

for i in $(seq 1 $N); do
cat > "$OUT/File$i.fs" << EOFI
module File$i

let processTuple$i (a: int, b: string) =
    let inner x = x + a
    let nested () = inner 42
    (nested (), b.Length)

let anon$i () =
    {| Name = "f$i"; Index = $i; Children = [| 1; 2; 3 |] |}

let anon${i}b () =
    {| Tag = "T$i"; Value = $i * 7; Extras = "x" |}

let useAnon$i () =
    let r = anon$i ()
    let r2 = anon${i}b ()
    let composed (k: int) =
        let h x = x + r.Index + r2.Value + k
        h 100
    composed 0 + r.Name.Length

[<Struct>]
type Rec$i = { X: int; Y: string }

let mkRec$i () = { X = $i; Y = "rec$i" }

let mainCall$i () =
    let _ = processTuple$i (1, "a")
    let _ = useAnon$i ()
    let _ = mkRec$i ()
    ()
EOFI
done

# Generate the response file with all source files in deterministic order
> ./files.rsp
for i in $(seq 1 $N); do
    echo "$OUT/File$i.fs" >> ./files.rsp
done

echo "Generated $N files in $OUT"
