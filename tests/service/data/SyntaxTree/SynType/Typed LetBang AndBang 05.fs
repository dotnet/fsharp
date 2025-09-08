module Module

async {
    let! ({ Name = name; Age = age }: Person) = asyncPerson()
    and! { Id = id }: User = asyncUser()
    return name
}