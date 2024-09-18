using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
 
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter username:");
        Console.WriteLine("hector.posada@upc.edu");
        string username = "hector.posada@upc.edu";

        Console.WriteLine("Enter password:");
        Console.WriteLine("MatchFEMiot");
        string password = "MatchFEMiot";

        var token = await GetAuthTokenAsync(username, password);
        Console.WriteLine("Token:" + token);

        //Console.WriteLine("Enter thing name:");
        //Console.WriteLine("ThingTest");
        //string thing_name = "ThingTest";

        Console.WriteLine("Enter thing ID:");
        Console.WriteLine("cf9854d0-1018-4eb3-b96f-067693657496");
        string thing_id = "cf9854d0-1018-4eb3-b96f-067693657496";

        //Console.WriteLine("Enter channel name:");
        //Console.WriteLine("Test");
        //string channel_name = "Test";

        //string thing_id = Guid.NewGuid().ToString();

        //Console.WriteLine("Enter channel ID:");
        //Console.WriteLine("65be0105-f846-429a-a6e6-a78175b37d1e");
        //string channel_id = "65be0105-f846-429a-a6e6-a78175b37d1e";

        //await ConnectThings(token, channel_id, thing_id);
        //await CreateThings(token, thing_name, thing_id, channel_id);
        //await GetThingByName(token, thing_name);
        await GetThingByID(token, thing_id);
        //await GetChannelByName(token, channel_name);
        //await GetChannelByID(token, channel_id);
        //await GetThingsInChannels(token, channel_id);

        //Console.WriteLine("Enter thing key:");
        //Console.WriteLine("e1756621-c50c-493a-9b1c-80193945914b");
        //string thing_key = "e1756621-c50c-493a-9b1c-80193945914b";

        //await ReadMessages(channel_id, token, thing_id);
        //await PostMessages(channel_id, thing_key);
        //await GetThingsInChannels(token, channel_id);

        Console.WriteLine("Enter to exit");
        Console.ReadLine();
    }

    static async Task<string> GetAuthTokenAsync(string username, string password)
    {
        // Create an HTTP client
        var client = new HttpClient();
        // Set the base URL
        var baseUrl = "http://mainflux-labs.com";
        // Set the request URL
        var url = $"{baseUrl}/tokens";

        // Set the form data with the username and password
        var payload = new Dictionary<string, string>
            {
                { "email", username },
                { "password", password }
            };

        // Set the request headers
        var jsonPayload = JsonConvert.SerializeObject(payload);
        var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            // Send the POST request to retrieve the token
            var response = await client.PostAsync(url, httpContent);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                var content = await response.Content.ReadAsStringAsync();

                // Extract the token from the response content
                JObject jsonResponse = JObject.Parse(content);
                string token = jsonResponse["token"].ToString();
                return token;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return null;
    }

    static async Task GetChannelByName(string token, string channel_name)
    {
        // Create an HTTP client
        var client = new HttpClient();

        // Set the base URL
        var baseUrl = "http://mainflux-labs.com";

        // Set the request URL
        var url = $"{baseUrl}/channels";

        // Parameters as a dictionary
        Dictionary<string, string> parameters = new Dictionary<string, string>
        {
            {"name", channel_name},
            {"limit", "99" }
        };

        string urlParam = BuildUrlWithParameters(url, parameters);

        // Set the request headers
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            // Send the GET request to retrieve the messages
            var response = await client.GetAsync(urlParam);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Channels: "+content);

                // Extract the token from the response content
                JObject jsonResponse = JObject.Parse(content);
                JToken channelInfo = jsonResponse.GetValue("channels");
                string name = channelInfo[0]["id"].ToString();
                
                Console.WriteLine(name);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static async Task GetChannelByID(string token, string channel_id)
    {
        // Create an HTTP client
        var client = new HttpClient();

        // Set the base URL
        var baseUrl = "https://iot.ashvin.eu";

        // Set the request URL
        var url = $"{baseUrl}/channels/" + channel_id;

        // Set the request headers
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            // Send the GET request to retrieve the messages
            var response = await client.GetAsync(url);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Channels: " + content);

                // Extract the token from the response content
                JObject jsonResponse = JObject.Parse(content);
                string channel_name = jsonResponse.GetValue("name").ToString();
                Console.WriteLine(channel_name);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static async Task GetThingsInChannels(string token, string channel_id)
    {
        // Create an HTTP client
        var client = new HttpClient();

        // Set the base URL
        var baseUrl = "http://mainflux-labs.com";

        // Set the request URL
        var url = $"{baseUrl}/channels/"+channel_id+"/things";

        // Parameters as a dictionary
        int limit = 99;

        Dictionary<string, string> parameters = new Dictionary<string, string>
        {
            {"limit", limit.ToString() },
            {"offset", "0" }
        };

        string urlParam = BuildUrlWithParameters(url, parameters);

        // Set the request headers
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        List<string> things_ids = new List<string>();
        List<string> things_names = new List<string>();
        List<List<string>> metadataKeys = new List<List<string>>();
        List<List<string>> metadataValues = new List<List<string>>();

        try
        {
            // Send the GET request to retrieve the messages
            var response = await client.GetAsync(urlParam);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Things: " + content);

                // Extract the token from the response content
                JObject jsonResponse = JObject.Parse(content);
                int total = Convert.ToInt32(jsonResponse["total"]);

                JToken thingsInfo = jsonResponse.GetValue("things");

                for (int i = 0; i < thingsInfo.Count(); i++)
                {
                    metadataKeys.Add(new List<string> { });
                    metadataValues.Add(new List<string> { });
                }

                for (int i = 0; i < thingsInfo.Count(); i++)
                {
                    things_ids.Add(thingsInfo[i]["id"].ToString());
                    things_names.Add(thingsInfo[i]["name"].ToString());
                    metadataKeys[i] = thingsInfo[i]["metadata"].Value<JObject>().Properties().Select(p => p.Name).ToList();
                    metadataValues[i] = thingsInfo[i]["metadata"].Value<JObject>().Properties().Select(p => p.Value.ToString()).ToList();
                }

                Console.WriteLine(total);
                if (total > limit)
                {
                    int aux = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(total / limit)));
                    List<int> offsets = new List<int>();

                    for (int i = 0; i < aux; i++)
                    {
                        int aux2 = i * limit;
                        offsets.Add(aux2 + limit);
                    }

                    for (int i = 0; i < offsets.Count; i++)
                    {
                        Dictionary<string, string> parameters2 = new Dictionary<string, string>
                        {
                            {"limit", limit.ToString() },
                            {"offset", offsets[i].ToString() },
                        };

                        string urlParam2 = BuildUrlWithParameters(url, parameters2);

                        try
                        {
                            // Send the GET request to retrieve the messages
                            var response2 = await client.GetAsync(urlParam2, HttpCompletionOption.ResponseHeadersRead);

                            // Check if the request was successful
                            if (response2.IsSuccessStatusCode)
                            {
                                // Read the response content as a string
                                var content2 = await response2.Content.ReadAsStringAsync();
                                Console.WriteLine("Things: " + content2);

                                // Extract the token from the response content
                                JObject jsonResponse2 = JObject.Parse(content2);

                                JToken thingsInfo2 = jsonResponse2.GetValue("things");

                                for (int k = 0; k < thingsInfo2.Count(); k++)
                                {
                                    metadataKeys.Add(new List<string> { });
                                    metadataValues.Add(new List<string> { });
                                }

                                for (int j = 0; j < thingsInfo2.Count(); j++)
                                {
                                    things_ids.Add(thingsInfo2[j]["id"].ToString());
                                    things_names.Add(thingsInfo2[j]["name"].ToString());
                                    metadataKeys[j+offsets[i]] = thingsInfo2[j]["metadata"].Value<JObject>().Properties().Select(p => p.Name).ToList();
                                    metadataValues[j + offsets[i]] = thingsInfo2[j]["metadata"].Value<JObject>().Properties().Select(p => p.Value.ToString()).ToList();
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Error: {response2.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        Console.WriteLine("List of ids: " + string.Join(", ", things_ids));
        Console.WriteLine("List of names: " + string.Join(", ", things_names));

        for (int j = 0; j < metadataKeys.Count(); j++)
        {
            Console.WriteLine("List of metadata" + j + ": " + string.Join(", ", metadataKeys[j]));
            Console.WriteLine("List of metadata" + j + ": " + string.Join(", ", metadataValues[j]));
        }
    }

    static async Task GetThingByName(string token, string thing_name)
    {
        // Create an HTTP client
        var client = new HttpClient();

        // Set the base URL
        var baseUrl = "http://mainflux-labs.com";

        // Set the request URL
        var url = $"{baseUrl}/things/search";

        // Set the request headers
        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Set the form data with the username and password
        var data = new Dictionary<string, object>
        {
            {"limit", 99 },
            {"offset",0 },
            {"name", thing_name}
        };
        
        string jsonData = JsonConvert.SerializeObject(data);

        try
        {
            // Send the GET request to retrieve the messages
            var response = await client.PostAsync(url, new StringContent(jsonData, Encoding.UTF8, "application/json"));

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Things: " + content);

                // Extract the token from the response content
                JObject jsonResponse = JObject.Parse(content);
                JToken thingsInfo = jsonResponse.GetValue("things");
                Console.WriteLine(thingsInfo[0]["id"].ToString());
                Console.WriteLine(thingsInfo[0]["key"].ToString());
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public static async Task GetThingByID(string token, string thing_id)
    {
        // Create an HTTP client
        var client = new HttpClient();

        // Set the base URL
        var baseUrl = "http://mainflux-labs.com";

        // Set the request URL
        var url = $"{baseUrl}/things/" + thing_id;

        // Set the request headers
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            // Send the GET request to retrieve the messages
            var response = await client.GetAsync(url);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Things: " + content);

                // Extract the token from the response content
                JObject jsonResponse = JObject.Parse(content);
                string thing_name = jsonResponse.GetValue("name").ToString();
                Console.WriteLine(thing_name);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static async Task CreateThings(string token, string thing_name, string thing_id, string channel_id)
    {
        // Create an HTTP client
        var client = new HttpClient();

        // Set the base URL
        var baseUrl = "https://iot.ashvin.eu";

        // Set the request URL
        var url = $"{baseUrl}/things/bulk";

        // Set the request headers
        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Set metadata
        var metadata = new Dictionary<string, string>();
        List<string> metadataName = new List<string> {"type", "device", "description" };
        List<string> metadataValue = new List<string> { "Sensor", "AccJapoXYZ", "Formwork accelerations during columns concrete vibration" };

        for (int i = 0; i < metadataName.Count; i++)
        {
            metadata.Add(metadataName[i], metadataValue[i]);
        }

        //Set metadata2
        var metadata2 = new Dictionary<string, string>();
        List<string> metadataName2 = new List<string> { "type", "device", "description" };
        List<string> metadataValue2 = new List<string> { "Sensor", "AccJapoXYZ", "Formwork accelerations during columns concrete vibration" };

        for (int i = 0; i < metadataName.Count; i++)
        {
            metadata2.Add(metadataName[i], metadataValue[i]);
        }

        // Set the form data with the username and password
        var data = new List<Dictionary<string, object>>();

        var parameters = new Dictionary<string, object>
        {
            {"name", thing_name},
            {"id", thing_id},
            {"metadata", metadata}
        };

        data.Add(parameters);

        var parameters2 = new Dictionary<string, object>
        {
            {"name", "Thing2"},
            {"id", "5bb37408-0f99-49e4-806f-97c3301c4e33"},
            {"metadata", metadata2}
        };

        data.Add(parameters2);

        string jsonData = JsonConvert.SerializeObject(data);

        try
        {
            // Send the GET request to retrieve the messages
            var response = await client.PostAsync(url, new StringContent(jsonData, Encoding.UTF8, "application/json"));

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Things: " + content);

                await ConnectThings(token, channel_id, thing_id);

            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static async Task ConnectThings(string token, string channel_id, string thing_id)
    {
        // Create an HTTP client
        var client = new HttpClient();

        // Set the base URL
        var baseUrl = "https://iot.ashvin.eu";

        // Set the request URL
        var url = $"{baseUrl}/connect";

        // Set the request headers
        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Set the form data with the username and password
        var data = new Dictionary<string, object>
        {
            {"channel_ids", new List<string> { channel_id }},
            { "thing_ids", new List<string> { thing_id }}
        };
        
        string jsonData = JsonConvert.SerializeObject(data);

        try
        {
            // Send the GET request to retrieve the messages
            var response = await client.PostAsync(url, new StringContent(jsonData, Encoding.UTF8, "application/json"));

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Connection: " + content);

            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static async Task ReadMessages(string channel_id, string token, string thing_id)
    {
        // Create an HTTP client
        var client = new HttpClient();

        // Set the base URL
        var baseUrl = "http://mainflux-labs.com";

        string subtopic = "Level1";

        // Set the request URL
        var url = $"{baseUrl}/reader/channels/" + channel_id + "/messages";

        // Parameters as a dictionary
        int limit = 10;

        Dictionary<string, string> parameters = new Dictionary<string, string>
        {
            {"limit", limit.ToString() },
            {"offset", "0" },
            {"publisher", thing_id },
            //{"subtopic", null }
        };

        string urlParam = BuildUrlWithParameters(url, parameters);

        // Set the request headers
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/senml+json"));
        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/senml+json");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        List<string> subtopics = new List<string>();

        try
        {
            // Send the GET request to retrieve the messages
            var response = await client.GetAsync(urlParam, HttpCompletionOption.ResponseHeadersRead);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
                JObject jsonResponse = JObject.Parse(content);

                int total = Convert.ToInt32(jsonResponse["total"]);

                if (total > limit)
                {
                    int aux = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(total / limit)));
                    List<int> offsets = new List<int>();
                    
                    for (int i = 0; i < aux; i++)
                    {
                        int aux2 = i * limit;
                        offsets.Add(aux2+limit);
                    }

                    for (int i = 0; i < offsets.Count; i++)
                    {
                        Dictionary<string, string> parameters2 = new Dictionary<string, string>
                        {
                            {"limit", limit.ToString() },
                            {"offset", offsets[i].ToString() },
                            {"publisher", thing_id }
                        };

                        string urlParam2 = BuildUrlWithParameters(url, parameters2);

                        try
                        {
                            // Send the GET request to retrieve the messages
                            var response2 = await client.GetAsync(urlParam2, HttpCompletionOption.ResponseHeadersRead);

                            // Check if the request was successful
                            if (response2.IsSuccessStatusCode)
                            {
                                // Read the response content as a string
                                var content2 = await response2.Content.ReadAsStringAsync();
                                Console.WriteLine(content2);
                            }
                            else
                            {
                                Console.WriteLine($"Error: {response2.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static async Task PostMessages(string channel_id, string thing_key)
    {
        // Create an HTTP client
        var client = new HttpClient();

        // Set the base URL
        var baseUrl = "http://mainflux-labs.com";

        // Set the request URL
        var url = $"{baseUrl}/http/channels/" + channel_id + "/messages";

        // Set the request headers
        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/senml+json");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Thing", thing_key);

        // Create the message
        List<double> values = new List<double> {0.24, 0.27, 0.23, 0.26, 0.27, 0.21, 0.25, 0.23, 0.29, 0.19, 0.25, 0.24, 0.26, 0.27, 0.28, 0.25 };
        List<double> times = new List<double> { 1684490800, 1684490900, 1684491000, 1684491100, 1684491200, 1684491300, 
            1684491400, 1684491500, 1684491600, 1684491700, 1684491800, 1684491900, 1684492000, 1684492100, 1684492200, 1684492300};
        string unit = "C";
        string name = "ThermoCoupleK1";

        List<List<double>> valuesPacks = PackValues(values, 10000);
        List<List<double>> timesPacks = PackValues(times, 10000);

        for (int i = 0; i < valuesPacks.Count; i++) 
        { 

        var data = new List<Dictionary<string, object>>();

        for (int j = 0; j < valuesPacks[i].Count; j++)
        {
            var message = new Dictionary<string, object>
            {
                {"n",name },
                {"u",unit},
                {"v",valuesPacks[i][j]},
                {"t",timesPacks[i][j]},
            };

            data.Add(message);
        }
        
        // Set the request headers
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        Console.WriteLine(jsonData);
        var httpContent = new StringContent(jsonData, Encoding.UTF8, "application/senml+json");

            try
        {
            // Send the POST request to post the message
            var response = await client.PostAsync(url, httpContent);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                // Check if the request was successful
                if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
                Console.WriteLine("Message posted successfully!");
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        }
    }

    static string ParseBearerToken(string responseJson)
    {
        JObject jsonResponse = JObject.Parse(responseJson);
        string token = jsonResponse["id_token"].ToString();
        return token;
    }

    static string BuildUrlWithParameters(string baseUrl, Dictionary<string, string> parameters)
    {
        var queryString = new StringBuilder();

        foreach (var parameter in parameters)
        {
            if (queryString.Length > 0)
                queryString.Append('&');

            queryString.Append($"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value)}");
        }

        return $"{baseUrl}?{queryString.ToString()}";
    }

    private static List<List<T>> PackValues<T>(List<T> inputList, int chunkSize)
    {
        List<List<T>> dividedLists = new List<List<T>>();

        for (int i = 0; i < inputList.Count; i += chunkSize)
        {
            List<T> chunk = inputList.Skip(i).Take(chunkSize).ToList();
            dividedLists.Add(chunk);
        }

        return dividedLists;
    }
}
