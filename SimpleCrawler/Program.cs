// See https://aka.ms/new-console-template for more information

using SmartCrawler.Urls;

HttpClient Client = new HttpClient();

string[] data =  UrlSamples.LawFirmsInSanFrancisco;
List<string> res = new List<string>();

foreach (var url in data)
{
    res.Add(await Client.GetStringAsync(url));
}

Console.WriteLine("Finished profiling");