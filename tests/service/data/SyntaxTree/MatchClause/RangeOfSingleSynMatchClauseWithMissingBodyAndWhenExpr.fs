
try
    let content = tryDownloadFile url
    Some content
with
| ex when (isNull ex) ->
