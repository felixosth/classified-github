using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REIDShared.NodeRED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace REIDShared
{
    public class NodeREDConnection
    {
        //public static NodeREDConnection Instance { get; set; }
        private Uri NodeREDUri { get; set; }

        private bool running = false;

        public NodeREDCategory[] Categories { get; private set; }
        public NodeREDPerson[] Persons { get; private set; }

        //public List>

        public NodeREDConnection(Uri uri)
        {
            this.NodeREDUri = uri;
        }

        public bool Connect()
        {
            var data = Get("/test");
            if (data != null)
            {
                return ((dynamic)JObject.Parse(data)).success;
            }

            return false;
        }

        public void Close()
        {
            running = false;
        }

        public void StartPoll()
        {
            if (running)
                return;

            running = true;
            new Thread(PollThread).Start();
        }

        public void Update()
        {
            Persons = GetPersons();
            Categories = GetCategories();
        }

        private void PollThread()
        {
            while(running)
            {
                try
                {
                    Categories = GetCategories();
                    Persons = GetPersons();
                }
                catch
                {
                }

                Thread.Sleep(1000);
            }
        }

        public string GetKafkaBroker()
        {
            var data = Get("/kafkabroker");
            if (data != null)
            {
                return ((dynamic)JObject.Parse(data)).broker;
            }
            return null;
        }

        public NodeREDCategory[] GetCategories()
        {
            var data = Get("/categories");
            if (data != null)
            {
                return JsonConvert.DeserializeObject<NodeREDCategory[]>(data);
            }
            return new NodeREDCategory[0];
        }

        public NodeREDPerson[] GetPersons()
        {
            var data = Get("/persons");
            if(data != null)
            {
                return JsonConvert.DeserializeObject<NodeREDPerson[]>(data);
            }
            return new NodeREDPerson[0];
        }

        public void UpdatePersons(List<NodeREDPerson> persons)
        {
            var res = PostJson("/editperson", JsonConvert.SerializeObject(persons));
        }

        private string Get(string relativeUrl)
        {
            try
            {
                using (var wc = new WebClient() { Encoding = Encoding.UTF8 })
                {
                    return wc.DownloadString(new Uri(NodeREDUri, relativeUrl));
                }
            }
            catch
            {
            }
            return null;
        }

        private string PostJson(string relativeUrl, string data)
        {
            try
            {
                using (var wc = new WebClient() { Encoding = Encoding.UTF8 })
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    return wc.UploadString(new Uri(NodeREDUri, relativeUrl), data);
                }
            }
            catch
            {
            }
            return null;
        }

        private string Post(string relativeUrl, string data)
        {
            try
            {
                using (var wc = new WebClient() { Encoding = Encoding.UTF8 })
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    return wc.UploadString(new Uri(NodeREDUri, relativeUrl), data);
                }
            }
            catch
            {
            }
            return null;
        }

        public bool AddPerson(NodeREDPerson person)
        {
            return Post("/addperson", $"key={person.key}&name={person.personName}&category={person.category}") != null;
        }

        public bool RemovePerson(NodeREDPerson person)
        {
            return Post("/removeperson", $"key={person.key}") != null;
        }

        public NodeREDRecognitionSearchResult[] Search(DateTime from, DateTime to, string personQuery, string categoriesQuery)
        {
            string url = string.Format("/search?from={0}&to={1}{2}{3}",
                HttpUtility.UrlEncode(from.ToString()),
                HttpUtility.UrlEncode(to.ToString()),
                personQuery, categoriesQuery);

            string data = Get(url);

            if(data != null)
            {
                return JsonConvert.DeserializeObject<NodeREDRecognitionSearchResult[]>(data);
            }
            return new NodeREDRecognitionSearchResult[0];
        }
    }
}
