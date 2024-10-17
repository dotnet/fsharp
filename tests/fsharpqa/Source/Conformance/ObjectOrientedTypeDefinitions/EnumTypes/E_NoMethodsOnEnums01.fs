// #Regression #Conformance #ObjectOrientedTypes #Enums 
//<Expects id="FS0896" status="error" span="(6,9)">Enumerations cannot have members$</Expects>

type Season = Spring=0 | Summer=1 | Autumn=2 | Winter=3
    with
        static member op_Range(start: Season, stop: Season): seq<Season> =
            let starti = Enum.to_int start
            let stopi = Enum.to_int stop
            { for i in starti .. stopi -> Enum.of_int i }
