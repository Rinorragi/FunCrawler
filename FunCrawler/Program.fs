namespace FunCrawler

module Program =
    open System
    open System.Net.Http
    open System.Net.Http.Headers
    open System.Net
    open HttpCrawler

    let printResponse(entry:HttpCrawlerResult) =
        printfn "Result for url %s was: %s v%s" entry.Url (entry.StatusCode.ToString()) entry.Version

        // This is collection<string,collection<string>>
        entry.Headers
        |> Seq.iter (fun h -> printfn "Header: %s Values: [%s]" h.Key (h.Value |> Seq.fold (fun v v2 -> v + "(" + v2 + ")") ""))

    [<EntryPoint>]
    let main argv =
        let httpClient = getHttpCrawlerClient()
        // We want to have few maps that have a 
        let domains = [
            "dev.solita.fi";
            "www.solita.fi"; 
            "www.wikipedia.com"; 
            "www.stackoverflow.com"]
    
        let httpResults = 
            domains 
            |> List.map (fun url -> getHttpCrawlerResultAsync (httpClient, url, "http://"))
            |> Async.Parallel
            |> Async.RunSynchronously
            |> Array.map(fun result -> (result.Domain, result))
            |> Map.ofArray
        printfn "HTTP results!"
        httpResults
        |> Map.iter (fun k v -> printResponse(v))
    
        let httpsResults = 
            domains 
            |> List.map (fun url -> getHttpCrawlerResultAsync (httpClient, url, "https://"))
            |> Async.Parallel
            |> Async.RunSynchronously
            |> Array.map(fun result -> (result.Domain, result))
            |> Map.ofArray 
        printfn "HTTPS results!"
        httpsResults 
        |> Map.iter (fun k v -> printResponse(v))

        // Just to make it pause
        let _ = Console.ReadLine()
        0 // return an integer exit code
