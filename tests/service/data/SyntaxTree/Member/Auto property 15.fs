module A

type X() =
    member val internal Y: int = 7 with public get, private set
