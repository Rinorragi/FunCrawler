namespace FunCrawler

module HttpCrawler =
    open System
    open System.Net.Http
    open System.Net.Http.Headers
    open System.Net

    type HttpCrawlerResult(initDomain:string, initUrl:string, initHeaders:HttpResponseHeaders, initVersion:string, initStatusCode:HttpStatusCode) = 
        member this.Headers:HttpResponseHeaders = initHeaders
        member this.Version:string = initVersion
        member this.StatusCode:HttpStatusCode = initStatusCode
        member this.Domain:string = initDomain
        member this.Url:string = initUrl

    let getHttpCrawlerClient() = 
        // Ignore SSL errors
        let httpHandler = new HttpClientHandler()
        httpHandler.AllowAutoRedirect <- false
        // TODO: Figure out how to pinpoint that validation fails instead of just ignoring
        httpHandler.ServerCertificateCustomValidationCallback <- (fun _ _ _ _ -> true) 
        // Create reusable httpclient
        let httpClient = new HttpClient(httpHandler)
        httpClient
    
    let getHttpCrawlerResultAsync (client:HttpClient, domain:string, prefix:string) = 
        async {
            let url = prefix+domain 
            let! rootHttpResponse = client.GetAsync(url) |> Async.AwaitTask
            //TODO: Figure out how to make this work for non 200 answers if content is some day needed
            //response.EnsureSuccessStatusCode |> ignore
            //let! content =  response.Content.ReadAsStringAsync() |> Async.AwaitTask
            let crawlResponse = HttpCrawlerResult (domain, url, rootHttpResponse.Headers, rootHttpResponse.Version.ToString(), rootHttpResponse.StatusCode)
            return crawlResponse
        }

    

