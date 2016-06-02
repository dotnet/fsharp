// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Collections.CollectionModulesConsistency

open System
open System.Collections.Generic
open NUnit.Framework
open FsCheck
open Utils

let allPairs<'a when 'a : equality> (xs : list<'a>) (xs2 : list<'a>) =
    let s = xs |> Seq.allPairs xs2
    let l = xs |> List.allPairs xs2
    let a = xs |> Seq.toArray |> Array.allPairs (Seq.toArray xs2)
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``allPairs is consistent`` () =
    Check.QuickThrowOnFailure allPairs<int>
    Check.QuickThrowOnFailure allPairs<string>
    Check.QuickThrowOnFailure allPairs<NormalFloat>

let append<'a when 'a : equality> (xs : list<'a>) (xs2 : list<'a>) =
    let s = xs |> Seq.append xs2 
    let l = xs |> List.append xs2
    let a = xs |> Seq.toArray |> Array.append (Seq.toArray xs2)
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``append is consistent`` () =
    Check.QuickThrowOnFailure append<int>
    Check.QuickThrowOnFailure append<string>
    Check.QuickThrowOnFailure append<NormalFloat>

let averageFloat (xs : NormalFloat []) =
    let xs = xs |> Array.map float
    let s = run (fun () -> xs |> Seq.average)
    let l = run (fun () -> xs |> List.ofArray |> List.average)
    let a = run (fun () -> xs |> Array.average)
    s = a && l = a

[<Test>]
let ``average is consistent`` () =
    Check.QuickThrowOnFailure averageFloat

let averageBy (xs : float []) f =
    let xs = xs |> Array.map float
    let f x = (f x : NormalFloat) |> float
    let s = run (fun () -> xs |> Seq.averageBy f)
    let l = run (fun () -> xs |> List.ofArray |> List.averageBy f)
    let a = run (fun () -> xs |> Array.averageBy f)
    s = a && l = a

[<Test>]
let ``averageBy is consistent`` () =
    Check.QuickThrowOnFailure averageBy

let contains<'a when 'a : equality> (xs : 'a []) x  =
    let s = xs |> Seq.contains x
    let l = xs |> List.ofArray |> List.contains x
    let a = xs |> Array.contains x
    s = a && l = a

[<Test>]
let ``contains is consistent`` () =
    Check.QuickThrowOnFailure contains<int>
    Check.QuickThrowOnFailure contains<string>
    Check.QuickThrowOnFailure contains<float>

let choose<'a when 'a : equality> (xs : 'a []) f  =
    let s = xs |> Seq.choose f
    let l = xs |> List.ofArray |> List.choose f
    let a = xs |> Array.choose f
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``choose is consistent`` () =
    Check.QuickThrowOnFailure choose<int>
    Check.QuickThrowOnFailure choose<string>
    Check.QuickThrowOnFailure choose<float>

let chunkBySize<'a when 'a : equality> (xs : 'a []) size =
    let s = run (fun () -> xs |> Seq.chunkBySize size |> Seq.map Seq.toArray |> Seq.toArray)
    let l = run (fun () -> xs |> List.ofArray |> List.chunkBySize size |> Seq.map Seq.toArray |> Seq.toArray)
    let a = run (fun () -> xs |> Array.chunkBySize size |> Seq.map Seq.toArray |> Seq.toArray)
    s = a && l = a

[<Test>]
let ``chunkBySize is consistent`` () =
    Check.QuickThrowOnFailure chunkBySize<int>
    Check.QuickThrowOnFailure chunkBySize<string>
    Check.QuickThrowOnFailure chunkBySize<NormalFloat>

let collect<'a> (xs : 'a []) f  =
    let s = xs |> Seq.collect f
    let l = xs |> List.ofArray |> List.collect (fun x -> f x |> List.ofArray)
    let a = xs |> Array.collect f
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``collect is consistent`` () =
    Check.QuickThrowOnFailure collect<int>
    Check.QuickThrowOnFailure collect<string>
    Check.QuickThrowOnFailure collect<float>

let compareWith<'a>(xs : 'a []) (xs2 : 'a []) f  =
    let s = (xs, xs2) ||> Seq.compareWith f
    let l = (List.ofArray xs, List.ofArray xs2) ||> List.compareWith f
    let a = (xs, xs2) ||> Array.compareWith f
    s = a && l = a

[<Test>]
let ``compareWith is consistent`` () =
    Check.QuickThrowOnFailure compareWith<int>
    Check.QuickThrowOnFailure compareWith<string>
    Check.QuickThrowOnFailure compareWith<float>
        
let concat<'a when 'a : equality> (xs : 'a [][]) =
    let s = xs |> Seq.concat
    let l = xs |> List.ofArray |> List.map List.ofArray |> List.concat
    let a = xs |> Array.concat
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``concat is consistent`` () =
    Check.QuickThrowOnFailure concat<int>
    Check.QuickThrowOnFailure concat<string>
    Check.QuickThrowOnFailure concat<NormalFloat>

let countBy<'a> (xs : 'a []) f =
    let s = xs |> Seq.countBy f
    let l = xs |> List.ofArray |> List.countBy f
    let a = xs |> Array.countBy f
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``countBy is consistent`` () =
    Check.QuickThrowOnFailure countBy<int>
    Check.QuickThrowOnFailure countBy<string>
    Check.QuickThrowOnFailure countBy<float>

let distinct<'a when 'a : comparison> (xs : 'a []) =
    let s = xs |> Seq.distinct 
    let l = xs |> List.ofArray |> List.distinct
    let a = xs |> Array.distinct
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``distinct is consistent`` () =
    Check.QuickThrowOnFailure distinct<int>
    Check.QuickThrowOnFailure distinct<string>
    Check.QuickThrowOnFailure distinct<NormalFloat>

let distinctBy<'a when 'a : equality> (xs : 'a []) f =
    let s = xs |> Seq.distinctBy f
    let l = xs |> List.ofArray |> List.distinctBy f
    let a = xs |> Array.distinctBy f
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``distinctBy is consistent`` () =
    Check.QuickThrowOnFailure distinctBy<int>
    Check.QuickThrowOnFailure distinctBy<string>
    Check.QuickThrowOnFailure distinctBy<NormalFloat>

let exactlyOne<'a when 'a : comparison> (xs : 'a []) =
    let s = runAndCheckErrorType (fun () -> xs |> Seq.exactlyOne)
    let l = runAndCheckErrorType (fun () -> xs |> List.ofArray |> List.exactlyOne)
    let a = runAndCheckErrorType (fun () -> xs |> Array.exactlyOne)
    s = a && l = a

[<Test>]
let ``exactlyOne is consistent`` () =
    Check.QuickThrowOnFailure exactlyOne<int>
    Check.QuickThrowOnFailure exactlyOne<string>
    Check.QuickThrowOnFailure exactlyOne<NormalFloat>

let except<'a when 'a : equality> (xs : 'a []) (itemsToExclude: 'a []) =
    let s = xs |> Seq.except itemsToExclude
    let l = xs |> List.ofArray |> List.except itemsToExclude
    let a = xs |> Array.except itemsToExclude
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``except is consistent`` () =
    Check.QuickThrowOnFailure except<int>
    Check.QuickThrowOnFailure except<string>
    Check.QuickThrowOnFailure except<NormalFloat>

let exists<'a when 'a : equality> (xs : 'a []) f =
    let s = xs |> Seq.exists f
    let l = xs |> List.ofArray |> List.exists f
    let a = xs |> Array.exists f
    s = a && l = a

[<Test>]
let ``exists is consistent`` () =
    Check.QuickThrowOnFailure exists<int>
    Check.QuickThrowOnFailure exists<string>
    Check.QuickThrowOnFailure exists<NormalFloat>

let exists2<'a when 'a : equality> (xs':('a*'a) []) f =    
    let xs = Array.map fst xs'
    let xs2 = Array.map snd xs'
    let s = runAndCheckErrorType (fun () -> Seq.exists2 f xs xs2)
    let l = runAndCheckErrorType (fun () -> List.exists2 f (List.ofSeq xs) (List.ofSeq xs2))
    let a = runAndCheckErrorType (fun () -> Array.exists2 f (Array.ofSeq xs) (Array.ofSeq xs2))
    s = a && l = a
    
[<Test>]
let ``exists2 is consistent for collections with equal length`` () =
    Check.QuickThrowOnFailure exists2<int>
    Check.QuickThrowOnFailure exists2<string>
    Check.QuickThrowOnFailure exists2<NormalFloat>

let filter<'a when 'a : equality> (xs : 'a []) predicate =
    let s = xs |> Seq.filter predicate
    let l = xs |> List.ofArray |> List.filter predicate
    let a = xs |> Array.filter predicate
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``filter is consistent`` () =
    Check.QuickThrowOnFailure filter<int>
    Check.QuickThrowOnFailure filter<string>
    Check.QuickThrowOnFailure filter<NormalFloat>

let find<'a when 'a : equality> (xs : 'a []) predicate =
    let s = run (fun () -> xs |> Seq.find predicate)
    let l = run (fun () -> xs |> List.ofArray |> List.find predicate)
    let a = run (fun () -> xs |> Array.find predicate)
    s = a && l = a

[<Test>]
let ``find is consistent`` () =
    Check.QuickThrowOnFailure find<int>
    Check.QuickThrowOnFailure find<string>
    Check.QuickThrowOnFailure find<NormalFloat>

let findBack<'a when 'a : equality> (xs : 'a []) predicate =
    let s = run (fun () -> xs |> Seq.findBack predicate)
    let l = run (fun () -> xs |> List.ofArray |> List.findBack predicate)
    let a = run (fun () -> xs |> Array.findBack predicate)
    s = a && l = a

[<Test>]
let ``findBack is consistent`` () =
    Check.QuickThrowOnFailure findBack<int>
    Check.QuickThrowOnFailure findBack<string>
    Check.QuickThrowOnFailure findBack<NormalFloat>

let findIndex<'a when 'a : equality> (xs : 'a []) predicate =
    let s = run (fun () -> xs |> Seq.findIndex predicate)
    let l = run (fun () -> xs |> List.ofArray |> List.findIndex predicate)
    let a = run (fun () -> xs |> Array.findIndex predicate)
    s = a && l = a

[<Test>]
let ``findIndex is consistent`` () =
    Check.QuickThrowOnFailure findIndex<int>
    Check.QuickThrowOnFailure findIndex<string>
    Check.QuickThrowOnFailure findIndex<NormalFloat>

let findIndexBack<'a when 'a : equality> (xs : 'a []) predicate =
    let s = run (fun () -> xs |> Seq.findIndexBack predicate)
    let l = run (fun () -> xs |> List.ofArray |> List.findIndexBack predicate)
    let a = run (fun () -> xs |> Array.findIndexBack predicate)
    s = a && l = a

[<Test>]
let ``findIndexBack is consistent`` () =
    Check.QuickThrowOnFailure findIndexBack<int>
    Check.QuickThrowOnFailure findIndexBack<string>
    Check.QuickThrowOnFailure findIndexBack<NormalFloat>

let fold<'a,'b when 'b : equality> (xs : 'a []) f (start:'b) =
    let s = run (fun () -> xs |> Seq.fold f start)
    let l = run (fun () -> xs |> List.ofArray |> List.fold f start)
    let a = run (fun () -> xs |> Array.fold f start)
    s = a && l = a

[<Test>]
let ``fold is consistent`` () =
    Check.QuickThrowOnFailure fold<int,int>
    Check.QuickThrowOnFailure fold<string,string>
    Check.QuickThrowOnFailure fold<float,int>
    Check.QuickThrowOnFailure fold<float,string>

let fold2<'a,'b,'c when 'c : equality> (xs': ('a*'b)[]) f (start:'c) =
    let xs = xs' |> Array.map fst
    let xs2 = xs' |> Array.map snd
    let s = run (fun () -> Seq.fold2 f start xs xs2)
    let l = run (fun () -> List.fold2 f start (List.ofArray xs) (List.ofArray xs2))
    let a = run (fun () -> Array.fold2 f start xs xs2)
    s = a && l = a

[<Test>]
let ``fold2 is consistent`` () =
    Check.QuickThrowOnFailure fold2<int,int,int>
    Check.QuickThrowOnFailure fold2<string,string,string>
    Check.QuickThrowOnFailure fold2<string,int,string>
    Check.QuickThrowOnFailure fold2<string,float,int>
    Check.QuickThrowOnFailure fold2<float,float,int>
    Check.QuickThrowOnFailure fold2<float,float,string>

let foldBack<'a,'b when 'b : equality> (xs : 'a []) f (start:'b) =
    let s = run (fun () -> Seq.foldBack f xs start)
    let l = run (fun () -> List.foldBack f (xs |> List.ofArray) start)
    let a = run (fun () -> Array.foldBack f xs start)
    s = a && l = a

[<Test>]
let ``foldBack is consistent`` () =
    Check.QuickThrowOnFailure foldBack<int,int>
    Check.QuickThrowOnFailure foldBack<string,string>
    Check.QuickThrowOnFailure foldBack<float,int>
    Check.QuickThrowOnFailure foldBack<float,string>

let foldBack2<'a,'b,'c when 'c : equality> (xs': ('a*'b)[]) f (start:'c) =
    let xs = xs' |> Array.map fst
    let xs2 = xs' |> Array.map snd
    let s = run (fun () -> Seq.foldBack2 f xs xs2 start)
    let l = run (fun () -> List.foldBack2 f (List.ofArray xs) (List.ofArray xs2) start)
    let a = run (fun () -> Array.foldBack2 f xs xs2 start)
    s = a && l = a

[<Test>]
let ``foldBack2 is consistent`` () =
    Check.QuickThrowOnFailure foldBack2<int,int,int>
    Check.QuickThrowOnFailure foldBack2<string,string,string>
    Check.QuickThrowOnFailure foldBack2<string,int,string>
    Check.QuickThrowOnFailure foldBack2<string,float,int>
    Check.QuickThrowOnFailure foldBack2<float,float,int>
    Check.QuickThrowOnFailure foldBack2<float,float,string>

let forall<'a when 'a : equality> (xs : 'a []) f =
    let s = xs |> Seq.forall f
    let l = xs |> List.ofArray |> List.forall f
    let a = xs |> Array.forall f
    s = a && l = a

[<Test>]
let ``forall is consistent`` () =
    Check.QuickThrowOnFailure forall<int>
    Check.QuickThrowOnFailure forall<string>
    Check.QuickThrowOnFailure forall<NormalFloat>

let forall2<'a when 'a : equality> (xs':('a*'a) []) f =    
    let xs = Array.map fst xs'
    let xs2 = Array.map snd xs'
    let s = runAndCheckErrorType (fun () -> Seq.forall2 f xs xs2)
    let l = runAndCheckErrorType (fun () -> List.forall2 f (List.ofSeq xs) (List.ofSeq xs2))
    let a = runAndCheckErrorType (fun () -> Array.forall2 f (Array.ofSeq xs) (Array.ofSeq xs2))
    s = a && l = a
    
[<Test>]
let ``forall2 is consistent for collections with equal length`` () =
    Check.QuickThrowOnFailure forall2<int>
    Check.QuickThrowOnFailure forall2<string>
    Check.QuickThrowOnFailure forall2<NormalFloat>

let groupBy<'a when 'a : equality> (xs : 'a []) f =
    let s = run (fun () -> xs |> Seq.groupBy f |> Seq.toArray |> Array.map (fun (x,xs) -> x,xs |> Seq.toArray))
    let l = run (fun () -> xs |> List.ofArray |> List.groupBy f |> Seq.toArray |> Array.map (fun (x,xs) -> x,xs |> Seq.toArray))
    let a = run (fun () -> xs |> Array.groupBy f |> Array.map (fun (x,xs) -> x,xs |> Seq.toArray))
    s = a && l = a

[<Test>]
let ``groupBy is consistent`` () =
    Check.QuickThrowOnFailure groupBy<int>
    Check.QuickThrowOnFailure groupBy<string>
    Check.QuickThrowOnFailure groupBy<NormalFloat>

let head<'a when 'a : equality> (xs : 'a []) =
    let s = runAndCheckIfAnyError (fun () -> xs |> Seq.head)
    let l = runAndCheckIfAnyError (fun () -> xs |> List.ofArray |> List.head)
    let a = runAndCheckIfAnyError (fun () -> xs |> Array.head)
    s = a && l = a

[<Test>]
let ``head is consistent`` () =
    Check.QuickThrowOnFailure head<int>
    Check.QuickThrowOnFailure head<string>
    Check.QuickThrowOnFailure head<NormalFloat>

let indexed<'a when 'a : equality> (xs : 'a []) =
    let s = xs |> Seq.indexed
    let l = xs |> List.ofArray |> List.indexed
    let a = xs |> Array.indexed
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``indexed is consistent`` () =
    Check.QuickThrowOnFailure indexed<int>
    Check.QuickThrowOnFailure indexed<string>
    Check.QuickThrowOnFailure indexed<NormalFloat>

let init<'a when 'a : equality> count f =
    let s = run (fun () -> Seq.init count f |> Seq.toArray)
    let l = run (fun () -> List.init count f |> Seq.toArray)
    let a = run (fun () -> Array.init count f)
    s = a && l = a

[<Test>]
let ``init is consistent`` () =
    Check.QuickThrowOnFailure init<int>
    Check.QuickThrowOnFailure init<string>
    Check.QuickThrowOnFailure init<NormalFloat>

let isEmpty<'a when 'a : equality> (xs : 'a []) =
    let s = xs |> Seq.isEmpty
    let l = xs |> List.ofArray |> List.isEmpty
    let a = xs |> Array.isEmpty
    s = a && l = a

[<Test>]
let ``isEmpty is consistent`` () =
    Check.QuickThrowOnFailure isEmpty<int>
    Check.QuickThrowOnFailure isEmpty<string>
    Check.QuickThrowOnFailure isEmpty<NormalFloat>

let item<'a when 'a : equality> (xs : 'a []) index =
    let s = runAndCheckIfAnyError (fun () -> xs |> Seq.item index)
    let l = runAndCheckIfAnyError (fun () -> xs |> List.ofArray |> List.item index)
    let a = runAndCheckIfAnyError (fun () -> xs |> Array.item index)
    s = a && l = a

[<Test>]
let ``item is consistent`` () =
    Check.QuickThrowOnFailure item<int>
    Check.QuickThrowOnFailure item<string>
    Check.QuickThrowOnFailure item<NormalFloat>

let iter<'a when 'a : equality> (xs : 'a []) f' =
    let list = System.Collections.Generic.List<'a>()
    let f x =
        list.Add x
        f' x

    let s = xs |> Seq.iter f
    let l = xs |> List.ofArray |> List.iter f
    let a =  xs |> Array.iter f

    let xs = Seq.toList xs
    list |> Seq.toList = (xs @ xs @ xs)

[<Test>]
let ``iter looks at every element exactly once and in order - consistenly over all collections`` () =
    Check.QuickThrowOnFailure iter<int>
    Check.QuickThrowOnFailure iter<string>
    Check.QuickThrowOnFailure iter<NormalFloat>

let iter2<'a when 'a : equality> (xs' : ('a*'a) []) f' =
    let xs = xs' |> Array.map fst
    let xs2 = xs' |> Array.map snd
    let list = System.Collections.Generic.List<'a*'a>()
    let f x y =
        list.Add <| (x,y)
        f' x y

    let s = Seq.iter2 f xs xs2
    let l = List.iter2 f (xs |> List.ofArray) (xs2 |> List.ofArray)
    let a = Array.iter2 f xs xs2

    let xs = Seq.toList xs'
    list |> Seq.toList = (xs @ xs @ xs)

[<Test>]
let ``iter2 looks at every element exactly once and in order - consistenly over all collections when size is equal`` () =
    Check.QuickThrowOnFailure iter2<int>
    Check.QuickThrowOnFailure iter2<string>
    Check.QuickThrowOnFailure iter2<NormalFloat>

let iteri<'a when 'a : equality> (xs : 'a []) f' =
    let list = System.Collections.Generic.List<'a>()
    let indices = System.Collections.Generic.List<int>()
    let f i x =
        list.Add x
        indices.Add i
        f' i x

    let s = xs |> Seq.iteri f
    let l = xs |> List.ofArray |> List.iteri f
    let a =  xs |> Array.iteri f

    let xs = Seq.toList xs
    list |> Seq.toList = (xs @ xs @ xs) &&
      indices |> Seq.toList = ([0..xs.Length-1] @ [0..xs.Length-1] @ [0..xs.Length-1])

[<Test>]
let ``iteri looks at every element exactly once and in order - consistenly over all collections`` () =
    Check.QuickThrowOnFailure iteri<int>
    Check.QuickThrowOnFailure iteri<string>
    Check.QuickThrowOnFailure iteri<NormalFloat>

let iteri2<'a when 'a : equality> (xs' : ('a*'a) []) f' =
    let xs = xs' |> Array.map fst
    let xs2 = xs' |> Array.map snd
    let list = System.Collections.Generic.List<'a*'a>()
    let indices = System.Collections.Generic.List<int>()
    let f i x y =
        list.Add <| (x,y)
        indices.Add i
        f' x y

    let s = Seq.iteri2 f xs xs2
    let l = List.iteri2 f (xs |> List.ofArray) (xs2 |> List.ofArray)
    let a = Array.iteri2 f xs xs2

    let xs = Seq.toList xs'
    list |> Seq.toList = (xs @ xs @ xs) &&
      indices |> Seq.toList = ([0..xs.Length-1] @ [0..xs.Length-1] @ [0..xs.Length-1])

[<Test>]
let ``iteri2 looks at every element exactly once and in order - consistenly over all collections when size is equal`` () =
    Check.QuickThrowOnFailure iteri2<int>
    Check.QuickThrowOnFailure iteri2<string>
    Check.QuickThrowOnFailure iteri2<NormalFloat>

let last<'a when 'a : equality> (xs : 'a []) =
    let s = runAndCheckIfAnyError (fun () -> xs |> Seq.last)
    let l = runAndCheckIfAnyError (fun () -> xs |> List.ofArray |> List.last)
    let a = runAndCheckIfAnyError (fun () -> xs |> Array.last)
    s = a && l = a

[<Test>]
let ``last is consistent`` () =
    Check.QuickThrowOnFailure last<int>
    Check.QuickThrowOnFailure last<string>
    Check.QuickThrowOnFailure last<NormalFloat>

let length<'a when 'a : equality> (xs : 'a []) =
    let s = xs |> Seq.length
    let l = xs |> List.ofArray |> List.length
    let a = xs |> Array.length
    s = a && l = a

[<Test>]
let ``length is consistent`` () =
    Check.QuickThrowOnFailure length<int>
    Check.QuickThrowOnFailure length<string>
    Check.QuickThrowOnFailure length<float>

let map<'a when 'a : equality> (xs : 'a []) f =
    let s = xs |> Seq.map f
    let l = xs |> List.ofArray |> List.map f
    let a = xs |> Array.map f
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``map is consistent`` () =
    Check.QuickThrowOnFailure map<int>
    Check.QuickThrowOnFailure map<string>
    Check.QuickThrowOnFailure map<float>

let map2<'a when 'a : equality> (xs' : ('a*'a) []) f' =
    let xs = xs' |> Array.map fst
    let xs2 = xs' |> Array.map snd
    let list = System.Collections.Generic.List<'a*'a>()
    let f x y =
        list.Add <| (x,y)
        f' x y

    let s = Seq.map2 f xs xs2
    let l = List.map2 f (xs |> List.ofArray) (xs2 |> List.ofArray)
    let a = Array.map2 f xs xs2

    let xs = Seq.toList xs'    
    Seq.toArray s = a && List.toArray l = a &&
      list |> Seq.toList = (xs @ xs @ xs)

[<Test>]
let ``map2 looks at every element exactly once and in order - consistenly over all collections when size is equal`` () =
    Check.QuickThrowOnFailure map2<int>
    Check.QuickThrowOnFailure map2<string>
    Check.QuickThrowOnFailure map2<NormalFloat>

let map3<'a when 'a : equality> (xs' : ('a*'a*'a) []) f' =
    let xs = xs' |> Array.map  (fun (x,y,z) -> x)
    let xs2 = xs' |> Array.map (fun (x,y,z) -> y)
    let xs3 = xs' |> Array.map (fun (x,y,z) -> z)
    let list = System.Collections.Generic.List<'a*'a*'a>()
    let f x y z =
        list.Add <| (x,y,z)
        f' x y z

    let s = Seq.map3 f xs xs2 xs3
    let l = List.map3 f (xs |> List.ofArray) (xs2 |> List.ofArray) (xs3 |> List.ofArray)
    let a = Array.map3 f xs xs2 xs3

    let xs = Seq.toList xs'
    Seq.toArray s = a && List.toArray l = a &&
      list |> Seq.toList = (xs @ xs @ xs)

[<Test>]
let ``map3 looks at every element exactly once and in order - consistenly over all collections when size is equal`` () =
    Check.QuickThrowOnFailure map3<int>
    Check.QuickThrowOnFailure map3<string>
    Check.QuickThrowOnFailure map3<NormalFloat>

let mapFold<'a when 'a : equality> (xs : 'a []) f start =
    let s,sr = xs |> Seq.mapFold f start
    let l,lr = xs |> List.ofArray |> List.mapFold f start
    let a,ar = xs |> Array.mapFold f start
    Seq.toArray s = a && List.toArray l = a &&
      sr = lr && sr = ar

[<Test>]
let ``mapFold is consistent`` () =
    Check.QuickThrowOnFailure mapFold<int>
    Check.QuickThrowOnFailure mapFold<string>
    Check.QuickThrowOnFailure mapFold<NormalFloat>

let mapFoldBack<'a when 'a : equality> (xs : 'a []) f start =
    let s,sr = Seq.mapFoldBack f xs start
    let l,lr = List.mapFoldBack f (xs |> List.ofArray) start
    let a,ar = Array.mapFoldBack f xs start
    Seq.toArray s = a && List.toArray l = a &&
      sr = lr && sr = ar

[<Test>]
let ``mapFold2 is consistent`` () =
    Check.QuickThrowOnFailure mapFoldBack<int>
    Check.QuickThrowOnFailure mapFoldBack<string>
    Check.QuickThrowOnFailure mapFoldBack<NormalFloat>

let mapi<'a when 'a : equality> (xs : 'a []) f =
    let s = xs |> Seq.mapi f
    let l = xs |> List.ofArray |> List.mapi f
    let a = xs |> Array.mapi f
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``mapi is consistent`` () =
    Check.QuickThrowOnFailure mapi<int>
    Check.QuickThrowOnFailure mapi<string>
    Check.QuickThrowOnFailure mapi<float>

let mapi2<'a when 'a : equality> (xs' : ('a*'a) []) f' =
    let xs = xs' |> Array.map fst
    let xs2 = xs' |> Array.map snd
    let list = System.Collections.Generic.List<'a*'a>()
    let indices = System.Collections.Generic.List<int>()
    let f i x y =
        indices.Add i
        list.Add <| (x,y)
        f' x y

    let s = Seq.mapi2 f xs xs2
    let l = List.mapi2 f (xs |> List.ofArray) (xs2 |> List.ofArray)
    let a = Array.mapi2 f xs xs2

    let xs = Seq.toList xs'    
    Seq.toArray s = a && List.toArray l = a &&
      list |> Seq.toList = (xs @ xs @ xs) &&
      (Seq.toList indices = [0..xs.Length-1] @ [0..xs.Length-1] @ [0..xs.Length-1])

[<Test>]
let ``mapi2 looks at every element exactly once and in order - consistenly over all collections when size is equal`` () =
    Check.QuickThrowOnFailure mapi2<int>
    Check.QuickThrowOnFailure mapi2<string>
    Check.QuickThrowOnFailure mapi2<NormalFloat>

let max<'a when 'a : comparison> (xs : 'a []) =
    let s = runAndCheckIfAnyError (fun () -> xs |> Seq.max)
    let l = runAndCheckIfAnyError (fun () -> xs |> List.ofArray |> List.max)
    let a = runAndCheckIfAnyError (fun () -> xs |> Array.max)
    s = a && l = a

[<Test>]
let ``max is consistent`` () =
    Check.QuickThrowOnFailure max<int>
    Check.QuickThrowOnFailure max<string>
    Check.QuickThrowOnFailure max<NormalFloat>

let maxBy<'a when 'a : comparison> (xs : 'a []) f =
    let s = runAndCheckIfAnyError (fun () -> xs |> Seq.maxBy f)
    let l = runAndCheckIfAnyError (fun () -> xs |> List.ofArray |> List.maxBy f)
    let a = runAndCheckIfAnyError (fun () -> xs |> Array.maxBy f)
    s = a && l = a

[<Test>]
let ``maxBy is consistent`` () =
    Check.QuickThrowOnFailure maxBy<int>
    Check.QuickThrowOnFailure maxBy<string>
    Check.QuickThrowOnFailure maxBy<NormalFloat>
 
let min<'a when 'a : comparison> (xs : 'a []) =
    let s = runAndCheckIfAnyError (fun () -> xs |> Seq.min)
    let l = runAndCheckIfAnyError (fun () -> xs |> List.ofArray |> List.min)
    let a = runAndCheckIfAnyError (fun () -> xs |> Array.min)
    s = a && l = a

[<Test>]
let ``min is consistent`` () =
    Check.QuickThrowOnFailure min<int>
    Check.QuickThrowOnFailure min<string>
    Check.QuickThrowOnFailure min<NormalFloat>

let minBy<'a when 'a : comparison> (xs : 'a []) f =
    let s = runAndCheckIfAnyError (fun () -> xs |> Seq.minBy f)
    let l = runAndCheckIfAnyError (fun () -> xs |> List.ofArray |> List.minBy f)
    let a = runAndCheckIfAnyError (fun () -> xs |> Array.minBy f)
    s = a && l = a

[<Test>]
let ``minBy is consistent`` () =
    Check.QuickThrowOnFailure minBy<int>
    Check.QuickThrowOnFailure minBy<string>
    Check.QuickThrowOnFailure minBy<NormalFloat>

let pairwise<'a when 'a : comparison> (xs : 'a []) =
    let s = run (fun () -> xs |> Seq.pairwise |> Seq.toArray)
    let l = run (fun () -> xs |> List.ofArray |> List.pairwise |> List.toArray)
    let a = run (fun () -> xs |> Array.pairwise)
    s = a && l = a

[<Test>]
let ``pairwise is consistent`` () =
    Check.QuickThrowOnFailure pairwise<int>
    Check.QuickThrowOnFailure pairwise<string>
    Check.QuickThrowOnFailure pairwise<NormalFloat>

let partition<'a when 'a : comparison> (xs : 'a []) f =
    // no seq version
    let l1,l2 = xs |> List.ofArray |> List.partition f
    let a1,a2 = xs |> Array.partition f
    List.toArray l1 = a1 &&
      List.toArray l2 = a2

[<Test>]
let ``partition is consistent`` () =
    Check.QuickThrowOnFailure partition<int>
    Check.QuickThrowOnFailure partition<string>
    Check.QuickThrowOnFailure partition<NormalFloat>

let permute<'a when 'a : comparison> (xs' : list<int*'a>) =
    let xs = List.map snd xs'
 
    let permutations = 
        List.map fst xs'
        |> List.indexed
        |> List.sortBy snd
        |> List.map fst
        |> List.indexed
        |> dict

    let permutation x = permutations.[x]

    let s = run (fun () -> xs |> Seq.permute permutation |> Seq.toArray)
    let l = run (fun () -> xs |> List.permute permutation |> List.toArray)
    let a = run (fun () -> xs |> Array.ofSeq |> Array.permute permutation)
    s = a && l = a

[<Test>]
let ``permute is consistent`` () =
    Check.QuickThrowOnFailure permute<int>
    Check.QuickThrowOnFailure permute<string>
    Check.QuickThrowOnFailure permute<NormalFloat>

let pick<'a when 'a : comparison> (xs : 'a []) f =
    let s = run (fun () -> xs |> Seq.pick f)
    let l = run (fun () -> xs |> List.ofArray |> List.pick f)
    let a = run (fun () -> xs |> Array.pick f)
    s = a && l = a

[<Test>]
let ``pick is consistent`` () =
    Check.QuickThrowOnFailure pick<int>
    Check.QuickThrowOnFailure pick<string>
    Check.QuickThrowOnFailure pick<NormalFloat>

let reduce<'a when 'a : equality> (xs : 'a []) f =
    let s = runAndCheckErrorType (fun () -> xs |> Seq.reduce f)
    let l = runAndCheckErrorType (fun () -> xs |> List.ofArray |> List.reduce f)
    let a = runAndCheckErrorType (fun () -> xs |> Array.reduce f)
    s = a && l = a

[<Test>]
let ``reduce is consistent`` () =
    Check.QuickThrowOnFailure reduce<int>
    Check.QuickThrowOnFailure reduce<string>
    Check.QuickThrowOnFailure reduce<NormalFloat>

let reduceBack<'a when 'a : equality> (xs : 'a []) f =
    let s = runAndCheckErrorType (fun () -> xs |> Seq.reduceBack f)
    let l = runAndCheckErrorType (fun () -> xs |> List.ofArray |> List.reduceBack f)
    let a = runAndCheckErrorType (fun () -> xs |> Array.reduceBack f)
    s = a && l = a

[<Test>]
let ``reduceBack is consistent`` () =
    Check.QuickThrowOnFailure reduceBack<int>
    Check.QuickThrowOnFailure reduceBack<string>
    Check.QuickThrowOnFailure reduceBack<NormalFloat>

let replicate<'a when 'a : equality> x count =
    let s = runAndCheckIfAnyError (fun () -> Seq.replicate count x |> Seq.toArray)
    let l = runAndCheckIfAnyError (fun () -> List.replicate count x |> List.toArray)
    let a = runAndCheckIfAnyError (fun () -> Array.replicate count x)
    s = a && l = a

[<Test>]
let ``replicate is consistent`` () =
    Check.QuickThrowOnFailure replicate<int>
    Check.QuickThrowOnFailure replicate<string>
    Check.QuickThrowOnFailure replicate<NormalFloat>

let rev<'a when 'a : equality> (xs : 'a []) =
    let s = Seq.rev xs |> Seq.toArray
    let l = xs |> List.ofArray |> List.rev |> List.toArray
    let a = Array.rev xs
    s = a && l = a

[<Test>]
let ``rev is consistent`` () =
    Check.QuickThrowOnFailure rev<int>
    Check.QuickThrowOnFailure rev<string>
    Check.QuickThrowOnFailure rev<NormalFloat>

let scan<'a,'b when 'b : equality> (xs : 'a []) f (start:'b) =
    let s = run (fun () -> xs |> Seq.scan f start |> Seq.toArray)
    let l = run (fun () -> xs |> List.ofArray |> List.scan f start |> Seq.toArray)
    let a = run (fun () -> xs |> Array.scan f start)
    s = a && l = a

[<Test>]
let ``scan is consistent`` () =
    Check.QuickThrowOnFailure scan<int,int>
    Check.QuickThrowOnFailure scan<string,string>
    Check.QuickThrowOnFailure scan<float,int>
    Check.QuickThrowOnFailure scan<float,string>

let scanBack<'a,'b when 'b : equality> (xs : 'a []) f (start:'b) =
    let s = run (fun () -> Seq.scanBack f xs start |> Seq.toArray)
    let l = run (fun () -> List.scanBack f (xs |> List.ofArray) start |> Seq.toArray)
    let a = run (fun () -> Array.scanBack f xs start)
    s = a && l = a

[<Test>]
let ``scanBack is consistent`` () =
    Check.QuickThrowOnFailure scanBack<int,int>
    Check.QuickThrowOnFailure scanBack<string,string>
    Check.QuickThrowOnFailure scanBack<float,int>
    Check.QuickThrowOnFailure scanBack<float,string>

let singleton<'a when 'a : equality> (x : 'a) =
    let s = Seq.singleton x |> Seq.toArray
    let l = List.singleton x |> List.toArray
    let a = Array.singleton x
    s = a && l = a

[<Test>]
let ``singleton is consistent`` () =
    Check.QuickThrowOnFailure singleton<int>
    Check.QuickThrowOnFailure singleton<string>
    Check.QuickThrowOnFailure singleton<NormalFloat>

let skip<'a when 'a : equality> (xs : 'a []) count =
    let s = runAndCheckIfAnyError (fun () -> Seq.skip count xs |> Seq.toArray)
    let l = runAndCheckIfAnyError (fun () -> List.skip count (Seq.toList xs) |> List.toArray)
    let a = runAndCheckIfAnyError (fun () -> Array.skip count xs)
    s = a && l = a

[<Test>]
let ``skip is consistent`` () =
    Check.QuickThrowOnFailure skip<int>
    Check.QuickThrowOnFailure skip<string>
    Check.QuickThrowOnFailure skip<NormalFloat>

let skipWhile<'a when 'a : equality> (xs : 'a []) f =
    let s = runAndCheckIfAnyError (fun () -> Seq.skipWhile f xs |> Seq.toArray)
    let l = runAndCheckIfAnyError (fun () -> List.skipWhile f (Seq.toList xs) |> List.toArray)
    let a = runAndCheckIfAnyError (fun () -> Array.skipWhile f xs)
    s = a && l = a

[<Test>]
let ``skipWhile is consistent`` () =
    Check.QuickThrowOnFailure skipWhile<int>
    Check.QuickThrowOnFailure skipWhile<string>
    Check.QuickThrowOnFailure skipWhile<NormalFloat>

let sort<'a when 'a : comparison> (xs : 'a []) =
    let s = xs |> Seq.sort 
    let l = xs |> List.ofArray |> List.sort
    let a = xs |> Array.sort
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``sort is consistent`` () =
    Check.QuickThrowOnFailure sort<int>
    Check.QuickThrowOnFailure sort<string>
    Check.QuickThrowOnFailure sort<NormalFloat>

let sortBy<'a,'b when 'a : comparison and 'b : comparison> (xs : 'a []) (f:'a -> 'b) =
    let s = xs |> Seq.sortBy f
    let l = xs |> List.ofArray |> List.sortBy f
    let a = xs |> Array.sortBy f

    isSorted (Seq.map f s) && isSorted (Seq.map f l) && isSorted (Seq.map f a) &&
      haveSameElements s xs && haveSameElements l xs && haveSameElements a xs

[<Test>]
let ``sortBy actually sorts (but is inconsistent in regards of stability)`` () =
    Check.QuickThrowOnFailure sortBy<int,int>
    Check.QuickThrowOnFailure sortBy<int,string>
    Check.QuickThrowOnFailure sortBy<string,string>
    Check.QuickThrowOnFailure sortBy<string,int>
    Check.QuickThrowOnFailure sortBy<NormalFloat,int>

let sortWith<'a,'b when 'a : comparison and 'b : comparison> (xs : 'a []) (f:'a -> 'a -> int) =
    let dict = System.Collections.Generic.Dictionary<_,_>()
    let f x y = 
        if x = y then 0 else
        if x = Unchecked.defaultof<_> && y <> Unchecked.defaultof<_> then -1 else
        if y = Unchecked.defaultof<_> && x <> Unchecked.defaultof<_> then 1 else
        let r = f x y |> sign // only use one side
        if x < y then r else r * -1

    let s = xs |> Seq.sortWith f
    let l = xs |> List.ofArray |> List.sortWith f
    let a = xs |> Array.sortWith f
    let isSorted sorted = sorted |> Seq.pairwise |> Seq.forall (fun (a,b) -> f a b <= 0 || a = b)

    isSorted s && isSorted l && isSorted a &&
        haveSameElements s xs && haveSameElements l xs && haveSameElements a xs

[<Test>]
let ``sortWith actually sorts (but is inconsistent in regards of stability)`` () =
    Check.QuickThrowOnFailure sortWith<int,int>
    Check.QuickThrowOnFailure sortWith<int,string>
    Check.QuickThrowOnFailure sortWith<string,string>
    Check.QuickThrowOnFailure sortWith<string,int>
    Check.QuickThrowOnFailure sortWith<NormalFloat,int>

let sortDescending<'a when 'a : comparison> (xs : 'a []) =
    let s = xs |> Seq.sortDescending 
    let l = xs |> List.ofArray |> List.sortDescending
    let a = xs |> Array.sortDescending
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``sortDescending is consistent`` () =
    Check.QuickThrowOnFailure sortDescending<int>
    Check.QuickThrowOnFailure sortDescending<string>
    Check.QuickThrowOnFailure sortDescending<NormalFloat>

let sortByDescending<'a,'b when 'a : comparison and 'b : comparison> (xs : 'a []) (f:'a -> 'b) =
    let s = xs |> Seq.sortByDescending f
    let l = xs |> List.ofArray |> List.sortByDescending f
    let a = xs |> Array.sortByDescending f

    isSorted (Seq.map f s |> Seq.rev) && isSorted (Seq.map f l |> Seq.rev) && isSorted (Seq.map f a |> Seq.rev) &&
      haveSameElements s xs && haveSameElements l xs && haveSameElements a xs

[<Test>]
let ``sortByDescending actually sorts (but is inconsistent in regards of stability)`` () =
    Check.QuickThrowOnFailure sortByDescending<int,int>
    Check.QuickThrowOnFailure sortByDescending<int,string>
    Check.QuickThrowOnFailure sortByDescending<string,string>
    Check.QuickThrowOnFailure sortByDescending<string,int>
    Check.QuickThrowOnFailure sortByDescending<NormalFloat,int>

let sum (xs : int []) =
    let s = run (fun () -> xs |> Seq.sum)
    let l = run (fun () -> xs |> Array.toList |> List.sum)
    let a = run (fun () -> xs |> Array.sum)
    s = a && l = a

[<Test>]
let ``sum is consistent`` () =
    Check.QuickThrowOnFailure sum

let sumBy<'a> (xs : 'a []) (f:'a -> int) =
    let s = run (fun () -> xs |> Seq.sumBy f)
    let l = run (fun () -> xs |> Array.toList |> List.sumBy f)
    let a = run (fun () -> xs |> Array.sumBy f)
    s = a && l = a

[<Test>]
let ``sumBy is consistent`` () =
    Check.QuickThrowOnFailure sumBy<int>
    Check.QuickThrowOnFailure sumBy<string>
    Check.QuickThrowOnFailure sumBy<float>

let splitAt<'a when 'a : equality> (xs : 'a []) index =
    // no seq version
    let l = run (fun () -> xs |> List.ofArray |> List.splitAt index |> fun (a,b) -> List.toArray a,List.toArray b)
    let a = run (fun () -> xs |> Array.splitAt index)
    l = a

[<Test>]
let ``splitAt is consistent`` () =
    Check.QuickThrowOnFailure splitAt<int>
    Check.QuickThrowOnFailure splitAt<string>
    Check.QuickThrowOnFailure splitAt<NormalFloat>

let splitInto<'a when 'a : equality> (xs : 'a []) count =
    let s = run (fun () -> xs |> Seq.splitInto count |> Seq.map Seq.toArray |> Seq.toArray)
    let l = run (fun () -> xs |> List.ofArray |> List.splitInto count |> Seq.map Seq.toArray |> Seq.toArray)
    let a = run (fun () -> xs |> Array.splitInto count |> Seq.map Seq.toArray |> Seq.toArray)
    s = a && l = a

[<Test>]
let ``splitInto is consistent`` () =
    Check.QuickThrowOnFailure splitInto<int>
    Check.QuickThrowOnFailure splitInto<string>
    Check.QuickThrowOnFailure splitInto<NormalFloat>

let tail<'a when 'a : equality> (xs : 'a []) =
    let s = runAndCheckIfAnyError (fun () -> xs |> Seq.tail |> Seq.toArray)
    let l = runAndCheckIfAnyError (fun () -> xs |> List.ofArray |> List.tail |> Seq.toArray)
    let a = runAndCheckIfAnyError (fun () -> xs |> Array.tail)
    s = a && l = a

[<Test>]
let ``tail is consistent`` () =
    Check.QuickThrowOnFailure tail<int>
    Check.QuickThrowOnFailure tail<string>
    Check.QuickThrowOnFailure tail<NormalFloat>

let take<'a when 'a : equality> (xs : 'a []) count =
    let s = runAndCheckIfAnyError (fun () -> Seq.take count xs |> Seq.toArray)
    let l = runAndCheckIfAnyError (fun () -> List.take count (Seq.toList xs) |> List.toArray)
    let a = runAndCheckIfAnyError (fun () -> Array.take count xs)
    s = a && l = a

[<Test>]
let ``take is consistent`` () =
    Check.QuickThrowOnFailure take<int>
    Check.QuickThrowOnFailure take<string>
    Check.QuickThrowOnFailure take<NormalFloat>

let takeWhile<'a when 'a : equality> (xs : 'a []) f =
    let s = runAndCheckIfAnyError (fun () -> Seq.takeWhile f xs |> Seq.toArray)
    let l = runAndCheckIfAnyError (fun () -> List.takeWhile f (Seq.toList xs) |> List.toArray)
    let a = runAndCheckIfAnyError (fun () -> Array.takeWhile f xs)
    s = a && l = a

[<Test>]
let ``takeWhile is consistent`` () =
    Check.QuickThrowOnFailure takeWhile<int>
    Check.QuickThrowOnFailure takeWhile<string>
    Check.QuickThrowOnFailure takeWhile<NormalFloat>

let truncate<'a when 'a : equality> (xs : 'a []) count =
    let s = runAndCheckIfAnyError (fun () -> Seq.truncate count xs |> Seq.toArray)
    let l = runAndCheckIfAnyError (fun () -> List.truncate count (Seq.toList xs) |> List.toArray)
    let a = runAndCheckIfAnyError (fun () -> Array.truncate count xs)
    s = a && l = a

[<Test>]
let ``truncate is consistent`` () =
    Check.QuickThrowOnFailure truncate<int>
    Check.QuickThrowOnFailure truncate<string>
    Check.QuickThrowOnFailure truncate<NormalFloat>

let tryFind<'a when 'a : equality> (xs : 'a []) predicate =
    let s = xs |> Seq.tryFind predicate
    let l = xs |> List.ofArray |> List.tryFind predicate
    let a = xs |> Array.tryFind predicate
    s = a && l = a

[<Test>]
let ``tryFind is consistent`` () =
    Check.QuickThrowOnFailure tryFind<int>
    Check.QuickThrowOnFailure tryFind<string>
    Check.QuickThrowOnFailure tryFind<NormalFloat>

let tryFindBack<'a when 'a : equality> (xs : 'a []) predicate =
    let s = xs |> Seq.tryFindBack predicate
    let l = xs |> List.ofArray |> List.tryFindBack predicate
    let a = xs |> Array.tryFindBack predicate
    s = a && l = a

[<Test>]
let ``tryFindBack is consistent`` () =
    Check.QuickThrowOnFailure tryFindBack<int>
    Check.QuickThrowOnFailure tryFindBack<string>
    Check.QuickThrowOnFailure tryFindBack<NormalFloat>

let tryFindIndex<'a when 'a : equality> (xs : 'a []) predicate =
    let s = xs |> Seq.tryFindIndex predicate
    let l = xs |> List.ofArray |> List.tryFindIndex predicate
    let a = xs |> Array.tryFindIndex predicate
    s = a && l = a

[<Test>]
let ``tryFindIndex is consistent`` () =
    Check.QuickThrowOnFailure tryFindIndex<int>
    Check.QuickThrowOnFailure tryFindIndex<string>
    Check.QuickThrowOnFailure tryFindIndex<NormalFloat>

let tryFindIndexBack<'a when 'a : equality> (xs : 'a []) predicate =
    let s = xs |> Seq.tryFindIndexBack predicate
    let l = xs |> List.ofArray |> List.tryFindIndexBack predicate
    let a = xs |> Array.tryFindIndexBack predicate
    s = a && l = a

[<Test>]
let ``tryFindIndexBack is consistent`` () =
    Check.QuickThrowOnFailure tryFindIndexBack<int>
    Check.QuickThrowOnFailure tryFindIndexBack<string>
    Check.QuickThrowOnFailure tryFindIndexBack<NormalFloat>

let tryHead<'a when 'a : equality> (xs : 'a []) =
    let s = xs |> Seq.tryHead
    let l = xs |> List.ofArray |> List.tryHead
    let a = xs |> Array.tryHead
    s = a && l = a

[<Test>]
let ``tryHead is consistent`` () =
    Check.QuickThrowOnFailure tryHead<int>
    Check.QuickThrowOnFailure tryHead<string>
    Check.QuickThrowOnFailure tryHead<NormalFloat>

let tryItem<'a when 'a : equality> (xs : 'a []) index =
    let s = xs |> Seq.tryItem index
    let l = xs |> List.ofArray |> List.tryItem index
    let a = xs |> Array.tryItem index
    s = a && l = a

[<Test>]
let ``tryItem is consistent`` () =
    Check.QuickThrowOnFailure tryItem<int>
    Check.QuickThrowOnFailure tryItem<string>
    Check.QuickThrowOnFailure tryItem<NormalFloat>

let tryLast<'a when 'a : equality> (xs : 'a []) =
    let s = xs |> Seq.tryLast
    let l = xs |> List.ofArray |> List.tryLast
    let a = xs |> Array.tryLast
    s = a && l = a

[<Test>]
let ``tryLast is consistent`` () =
    Check.QuickThrowOnFailure tryLast<int>
    Check.QuickThrowOnFailure tryLast<string>
    Check.QuickThrowOnFailure tryLast<NormalFloat>

let tryPick<'a when 'a : comparison> (xs : 'a []) f =
    let s = xs |> Seq.tryPick f
    let l = xs |> List.ofArray |> List.tryPick f
    let a = xs |> Array.tryPick f
    s = a && l = a

[<Test>]
let ``tryPick is consistent`` () =
    Check.QuickThrowOnFailure tryPick<int>
    Check.QuickThrowOnFailure tryPick<string>
    Check.QuickThrowOnFailure tryPick<NormalFloat>

let unfold<'a,'b when 'b : equality> f (start:'a) =
    let f() =
        let c = ref 0
        fun x -> 
            if !c > 100 then None else // prevent infinity seqs
            c := !c + 1
            f x

    
    let s : 'b [] = Seq.unfold (f()) start |> Seq.toArray
    let l = List.unfold (f()) start |> List.toArray
    let a = Array.unfold (f()) start
    s = a && l = a


[<Test>]
let ``unfold is consistent`` () =
    Check.QuickThrowOnFailure unfold<int,int>

[<Test; Category("Expensive")>]
let ``unfold is consistent full`` () =
    Check.QuickThrowOnFailure unfold<int,int>
    Check.QuickThrowOnFailure unfold<string,string>
    Check.QuickThrowOnFailure unfold<float,int>
    Check.QuickThrowOnFailure unfold<float,string>

let unzip<'a when 'a : equality> (xs:('a*'a) []) =       
    // no seq version
    let l = runAndCheckErrorType (fun () -> List.unzip (List.ofSeq xs) |> fun (a,b) -> List.toArray a, List.toArray b)
    let a = runAndCheckErrorType (fun () -> Array.unzip xs)
    l = a
    
[<Test>]
let ``unzip is consistent`` () =
    Check.QuickThrowOnFailure unzip<int>
    Check.QuickThrowOnFailure unzip<string>
    Check.QuickThrowOnFailure unzip<NormalFloat>

let unzip3<'a when 'a : equality> (xs:('a*'a*'a) []) =       
    // no seq version
    let l = runAndCheckErrorType (fun () -> List.unzip3 (List.ofSeq xs) |> fun (a,b,c) -> List.toArray a, List.toArray b, List.toArray c)
    let a = runAndCheckErrorType (fun () -> Array.unzip3 xs)
    l = a
    
[<Test>]
let ``unzip3 is consistent`` () =
    Check.QuickThrowOnFailure unzip3<int>
    Check.QuickThrowOnFailure unzip3<string>
    Check.QuickThrowOnFailure unzip3<NormalFloat>

let where<'a when 'a : equality> (xs : 'a []) predicate =
    let s = xs |> Seq.where predicate
    let l = xs |> List.ofArray |> List.where predicate
    let a = xs |> Array.where predicate
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``where is consistent`` () =
    Check.QuickThrowOnFailure where<int>
    Check.QuickThrowOnFailure where<string>
    Check.QuickThrowOnFailure where<NormalFloat>

let windowed<'a when 'a : equality> (xs : 'a []) windowSize =
    let s = run (fun () -> xs |> Seq.windowed windowSize |> Seq.toArray |> Array.map Seq.toArray)
    let l = run (fun () -> xs |> List.ofArray |> List.windowed windowSize |> List.toArray |> Array.map Seq.toArray)
    let a = run (fun () -> xs |> Array.windowed windowSize)
    s = a && l = a

[<Test>]
let ``windowed is consistent`` () =
    Check.QuickThrowOnFailure windowed<int>
    Check.QuickThrowOnFailure windowed<string>
    Check.QuickThrowOnFailure windowed<NormalFloat>

let zip<'a when 'a : equality> (xs':('a*'a) []) =    
    let xs = Array.map fst xs'
    let xs2 = Array.map snd xs'
    let s = runAndCheckErrorType (fun () -> Seq.zip xs xs2 |> Seq.toArray)
    let l = runAndCheckErrorType (fun () -> List.zip (List.ofSeq xs) (List.ofSeq xs2) |> List.toArray)
    let a = runAndCheckErrorType (fun () -> Array.zip (Array.ofSeq xs) (Array.ofSeq xs2))
    s = a && l = a
    
[<Test>]
let ``zip is consistent for collections with equal length`` () =
    Check.QuickThrowOnFailure zip<int>
    Check.QuickThrowOnFailure zip<string>
    Check.QuickThrowOnFailure zip<NormalFloat>

let zip3<'a when 'a : equality> (xs':('a*'a*'a) []) =    
    let xs = Array.map (fun (x,y,z) -> x) xs'
    let xs2 = Array.map (fun (x,y,z) -> y) xs'
    let xs3 = Array.map (fun (x,y,z) -> z) xs'
    let s = runAndCheckErrorType (fun () -> Seq.zip3 xs xs2 xs3 |> Seq.toArray)
    let l = runAndCheckErrorType (fun () -> List.zip3 (List.ofSeq xs) (List.ofSeq xs2) (List.ofSeq xs3) |> List.toArray)
    let a = runAndCheckErrorType (fun () -> Array.zip3 (Array.ofSeq xs) (Array.ofSeq xs2) (Array.ofSeq xs3))
    s = a && l = a
    
[<Test>]
let ``zip3 is consistent for collections with equal length`` () =
    Check.QuickThrowOnFailure zip3<int>
    Check.QuickThrowOnFailure zip3<string>
    Check.QuickThrowOnFailure zip3<NormalFloat>
