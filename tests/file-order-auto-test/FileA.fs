module FileA

type Person = {
    Name: string
    Age: int
}

let greet (p: Person) =
    sprintf "Hello, %s! You are %d years old." p.Name p.Age
