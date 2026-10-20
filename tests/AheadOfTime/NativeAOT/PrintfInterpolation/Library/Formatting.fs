module PrintfInterpolationLibrary

let timestamp (now: System.DateTime) =
    $"%02d{now.Hour}:%02d{now.Minute}:%02d{now.Second}"
