namespace FunCrawler
module DNSCrawler =
    open System
    open DnsClient

    type DNSCrawlerResult(initDomain:string, initDnsResponse:IDnsQueryResponse) = 
        member this.DnsResponse:IDnsQueryResponse = initDnsResponse
        member this.Domain:string = initDomain

    let getDNSCrawlerClient() =
        let client = new LookupClient()
        client.UseCache <- false
        client

    let getDNSCrawlerResultAsync(client:LookupClient, domain:string) = 
        async {
            let! result = client.QueryAsync(domain, QueryType.ANY) |> Async.AwaitTask
            let crawlResponse = DNSCrawlerResult (domain, result)
            return crawlResponse
        }
