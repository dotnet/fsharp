async {
    let! bar = getBar ()
    and! foo = getFoo () in
    and! meh = getMeh ()
    return bar
}