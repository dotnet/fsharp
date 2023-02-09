
match data with
| OnePartData( // foo
    part1 = p1
  (* bar *) ) -> p1
| _ -> failwith "todo"
