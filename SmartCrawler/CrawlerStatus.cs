namespace SmartCrawler;

/**
* Used mainly to avoid race conditions and race data when using the SmartCrawler
*/
public class CrawlerStatus
{
    public bool CrawlingInProgress = false;
}