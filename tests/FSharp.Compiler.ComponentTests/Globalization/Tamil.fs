// #Globalization 
#light

// Note that the first two characters in the string below are not accepted by F#
// (Or C#, for that matter...)
// ்ிகுச்துபக்ூசகதபகது்பகிதசகதி்கதப

// ்ி can be used in a string, but not an identifier...
let குச்துபக்ூசகதபகது்பகிதசகதி்கதப = "்ிகுச்துபக்ூசகதபகது்பகிதசகதி்கதப"
let குச்துபக்ூசகதபகது்பகிதசகதி்கதபப = """்ிகுச்துபக்ூசகதபகது்பகிதசகதி்கதப"""

printfn "%s" குச்துபக்ூசகதபகது்பகிதசகதி்கதப |> ignore
printfn "%s" குச்துபக்ூசகதபகது்பகிதசகதி்கதபப |> ignore
