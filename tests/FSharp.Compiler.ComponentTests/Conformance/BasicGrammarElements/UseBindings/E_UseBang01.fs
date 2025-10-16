async {
    use x: askjdhaskjd = failwith ""
    use! x: askjdhaskjd = failwith ""

    use! (x: askjdhaskjd) = failwith ""
    use! _: askjdhaskjd = failwith ""
    use! (_: askjdhaskjd) = failwith ""
    return 5
}
|> ignore
