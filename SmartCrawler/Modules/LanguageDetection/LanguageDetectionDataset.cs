namespace SmartCrawler.Modules.LanguageDetection;

public class LanguageDetectionDataset
{
    public string Language { get; }

    public LanguageDetectionDataset(string language)
    {
        Language = language;
    }
}