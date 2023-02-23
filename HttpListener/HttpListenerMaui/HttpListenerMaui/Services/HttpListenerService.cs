using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HttpListenerMaui.Services
{
    public class HttpListenerService
    {
        HttpListener listener;
        HttpListenerContext context;

        public HttpListenerService()
        {
        }

        public async Task Listen()
        {
            try
            {
                listener = new HttpListener();
                string uriPref = string.Format("http://{0}:{1}/", "localhost", "8080");
                listener.Prefixes.Add(uriPref);
                listener.Start();
                while (true)
                {
                    context = await listener.GetContextAsync();
                    var req = context.Request;
                    var response = context.Response;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "text/plain";
                    using (StreamWriter sw = new StreamWriter(response.OutputStream))
                    {
                        sw.WriteLine($"Hello from MAUI {DateTime.Now.ToString()}");
                    }
                    response.OutputStream.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
