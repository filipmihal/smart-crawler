using System.Net;

public  class FindUrls {

    public static string RequestGoogleApi(string key, string searchQuery)
    {
        string url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={searchQuery}&key={key}";

        WebRequest request = WebRequest.Create(url);

        WebResponse response = request.GetResponse();

        Stream data = response.GetResponseStream();

        StreamReader reader = new StreamReader(data);

        // json-formatted string from maps api
        string responseFromServer = reader.ReadToEnd();

        response.Close();

        return responseFromServer;
    }
}