// #Regression #Conformance #ObjectOrientedTypes #Enums 
//<Expects id="FS0896" status="error" span="(7,9)">Enumerations cannot have members$</Expects>
//<Expects status="error" span="(12,15)" id="FS0001">The type 'Season' does not support the operator 'get_One'$</Expects>

type Season = Spring=0 | Summer=1 | Autumn=2 | Winter=3
    with
        static member op_Range(start: Season, stop: Season): seq<Season> =
            let starti = Enum.to_int start
            let stopi = Enum.to_int stop
            { for i in starti .. stopi -> Enum.of_int i }

printfn "%A" [Season.Spring .. Season.Autumn]
