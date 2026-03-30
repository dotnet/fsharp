module RecordTest.Usage

open RecordTest.Types

/// Disambiguation by full field set — Age only exists on Person
let makePerson () : Person = { Name = "Alice"; Age = 30 }

/// Disambiguation by type annotation
let makeCompany () : Company = { Name = "Acme"; Founded = 1990 }

/// Disambiguation by field unique to Pet
let makePet () = { Name = "Whiskers"; Species = "Cat" }

/// Record update — must resolve to correct type
let birthday (p: Person) = { p with Age = p.Age + 1 }
