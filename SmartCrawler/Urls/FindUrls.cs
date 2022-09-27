string googleQuery = "YOUR_GOOGLE_QUERY";
string key = "YOUR_API_KEY";
string url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={googleQuery}&key={key}";

WebRequest request = WebRequest.Create(url);

WebResponse response = request.GetResponse();

Stream data = response.GetResponseStream();

StreamReader reader = new StreamReader(data);

// json-formatted string from maps api
string responseFromServer = reader.ReadToEnd();

response.Close();