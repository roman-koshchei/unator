using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Unator.Email.Senders;

public class Postmark : UEmailSender
{
    private const string url = "https://api.postmarkapp.com/email";
    private readonly HttpClient httpClient;

    public Postmark(string key)
    {
        httpClient = UEmailSender.JsonHttpClient(headers =>
        {
            headers.Add("X-Postmark-Server-Token", key);
        });
    }

    public async Task<Exception?> SendOne(string from, string to, string subject, string html)
    {
        try
        {
            string jsonBody = $@"{{""From"": ""{from}"",""To"": ""{to}"",""Subject"": ""{subject}"",""HtmlBody"": ""{html}"",""MessageStream"": ""outbound""}}";

            var response = await UEmailSender.JsonPost(httpClient, url, jsonBody);

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            if (response.IsSuccessStatusCode)
            {
                return null;
            }

            return new SenderServerFailException();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}