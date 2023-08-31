match Unchecked.defaultof<System.ValueType> with
| :? System.Enum as (a & b) -> let c = a = b in ()
| :? System.Enum as (:? System.ConsoleKey as (d & e)) -> let f = d + e + enum 1 in ()
| g -> ()