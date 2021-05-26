using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace ConsoleApp1
{
    /* */
    class Program
    {
        private static readonly string tlgToken = "";           // Use personal token
        private static readonly string WeatherToken = "";       //https://openweathermap.org 
        private static readonly string BaseTlgUrl = "https://api.telegram.org/bot";
        private static readonly string BaseWeatherUlr = "https://api.openweathermap.org/data/2.5/weather?q=";
        private static readonly HttpClient client = new HttpClient();
        private static WebClient webClient = new WebClient() { Encoding = System.Text.Encoding.UTF8 };

        static void Main(string[] args)
        {
            ProcessGet();
        }
        private static void ProcessGet()
        {
            int IdMess = 0;
            var TlgResponse = webClient.DownloadString($"{BaseTlgUrl}{tlgToken}/getUpdates?offset={IdMess}");
            while (true)
            {
                var _TlgResponse = webClient.DownloadString($"{BaseTlgUrl}{tlgToken}/getUpdates?offset={IdMess}");
                var TelegramApiModel = JObject.Parse(_TlgResponse)["result"].ToArray();
                var LastMess = TelegramApiModel.Last().Last();
                if (TlgResponse != _TlgResponse)
                {
                    foreach (dynamic item in LastMess)
                    {
                        int mesId = item.message_id;
                        string mesText = item.text;
                        int UserId = item.from.id;
                        try
                        {
                            var GetWeatherInfo = new WebClient().DownloadString($"{BaseWeatherUlr}{mesText}&appid={WeatherToken}&units=metric");
                            var list = JsonConvert.DeserializeObject<Repository>(GetWeatherInfo);
                            Repository tempInfo = new Repository();
                            var sendInfo = new WebClient().DownloadString($"{BaseTlgUrl}{tlgToken}/sendMessage?chat_id={UserId}&text=На улице {list.Main.Temp} °C");
                        }
                        catch (Exception)
                        {
                            string errorMes = "Упс, такой город не найден, попробуйте снова!";
                            var sendMess = new WebClient().DownloadString($"{BaseTlgUrl}{tlgToken}/sendMessage?chat_id={UserId}&text={errorMes}");
                        }
                    }
                }
                TlgResponse = _TlgResponse;
            }
        }
        
    }
}




