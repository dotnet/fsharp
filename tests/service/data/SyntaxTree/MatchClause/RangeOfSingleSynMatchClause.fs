
try
    let content = tryDownloadFile url
    Some content
with ex ->
    Infrastructure.ReportWarning ex
    None
