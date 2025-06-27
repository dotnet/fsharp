module Module
async {
    let! ({ Name = name }: Person) = asyncPerson()
    
    return name
   
}