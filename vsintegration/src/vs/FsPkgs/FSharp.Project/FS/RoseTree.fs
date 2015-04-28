namespace Microsoft.VisualStudio.FSharp.ProjectSystem

type RoseTree<'T> = 
    | Node of 'T * RoseTree<'T> list

module RoseTree = 
    open System
    
    let rec draw f (Node(label, forest)) = 
        let shift start other = 
            let rec inner acc = 
                function 
                | [] -> List.rev acc
                | l :: lines -> 
                    let markup = 
                        if acc = [] then start
                        else other
                    inner (String.concat "" [ markup; l ] :: acc) lines
            inner []
        
        let rec inner forest = 
            match forest with
            | [] -> []
            | t :: ts -> 
                let (start, other) = 
                    match ts with
                    | [] -> ("└── ", "    ")
                    | _ ->  ("├── ", "|   ")
                
                let lines = draw f t
                "|" :: shift start other lines @ inner ts
        
        f label :: inner forest
    
    let appendTree (Node(x, xs)) node = Node(x, (List.rev (node :: (List.rev xs))))
    
    let rec mapForest f (Node(x, xs)) = 
        Node(x, 
             xs
             |> f
             |> List.map (mapForest f))
    
    let rec map f (Node(x, xs)) = Node(f x, xs |> List.map (map f))
    
    let rec choose f (Node(t, children)) = 
        match f t with
        | None -> None
        | Some x -> Some(Node(x, children |> List.choose (choose f)))
    
    let foldSiblings sameNode siblings = 
        let folder visited current = 
            match visited, current with
            | Node(x, xn) :: others, Node(y, xm) -> 
                match sameNode x y with
                | None -> current :: visited
                | Some n -> (Node(n, xn @ xm)) :: others
            | _ -> current :: visited
        siblings
        |> List.fold folder []
        |> List.rev
    
    let rec dfsPre (Node(x, xs)) = 
        seq { 
            yield x
            yield! Seq.collect dfsPre xs
        }
    
    let rec fold f acc (Node(x : 'T, xs)) = List.fold (fold f) (f acc x) xs
    
    let addPath tree = 
        let tree' = tree |> map (fun x -> [], x)
        let rec helper parents (Node((_, x), xs)) = Node((parents, x), xs |> List.map (helper (x :: parents)))
        helper [] tree' |> map (fun (path, x) -> (path |> List.rev, x))
