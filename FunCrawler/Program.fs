namespace FunCrawler

module Program =
    open System
    open HttpCrawler
    open DNSCrawler

    let printHttpResponse(entry:HttpCrawlerResult) =
        printfn "Result for url %s was: %s v%s" entry.Url (entry.StatusCode.ToString()) entry.Version
        // This is collection<string,collection<string>>
        entry.Headers
        |> Seq.iter (fun h -> printfn "Header: %s Values: [%s]" h.Key (h.Value |> Seq.fold (fun v v2 -> v + "(" + v2 + ")") ""))

    let printDNSResponse(entry:DNSCrawlerResult) =
        printfn "Result for domain %s is:" entry.Domain
        // This is
        entry.DnsResponse.AllRecords
        |> Seq.iter (fun d -> printfn "%s" (d.ToString()))

    [<EntryPoint>]
    let main argv =
        // We want to have few maps that have a 
        let domains = [
            "dev.solita.fi";
            "www.solita.fi"]
    
        // HTTP
        let httpClient = getHttpCrawlerClient()
        let httpResults = 
            domains 
            |> List.map (fun url -> getHttpCrawlerResultAsync (httpClient, url, "http://"))
            |> Async.Parallel
            |> Async.RunSynchronously
            |> Array.map(fun result -> (result.Domain, result))
            |> Map.ofArray
        printfn "HTTP results!"
        httpResults
        |> Map.iter (fun k v -> printHttpResponse(v))    
        let httpsResults = 
            domains 
            |> List.map (fun url -> getHttpCrawlerResultAsync (httpClient, url, "https://"))
            |> Async.Parallel
            |> Async.RunSynchronously
            |> Array.map(fun result -> (result.Domain, result))
            |> Map.ofArray 
        printfn "HTTPS results!"
        httpsResults 
        |> Map.iter (fun k v -> printHttpResponse(v))

        // DNS
        let dnsClient = getDNSCrawlerClient()
        let dnsResults = 
            domains
            |> List.map (fun domain -> getDNSCrawlerResultAsync (dnsClient, domain))
            |> Async.Parallel
            |> Async.RunSynchronously
            |> Array.map(fun result -> (result.Domain, result))
            |> Map.ofArray
        printfn "DNS results!"
        dnsResults
        |> Map.iter (fun k v -> printDNSResponse(v))

        // Just to make it pause
        printfn "Finished! Press any key..."
        let _ = Console.ReadLine()
        0 // return an integer exit code
