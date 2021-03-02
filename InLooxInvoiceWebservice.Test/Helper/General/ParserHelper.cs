using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InLooxInvoiceWebservice.Test.Helper.General
{
    public class ParserHelper<T>
    {
        public T GetObjectSingle(string text)
        {
            var list = JsonConvert.DeserializeObject<JObject>(text);
            var single = list.Last.First.ToString()[1..^1].Trim();
            var output = JsonConvert.DeserializeObject<T>(single);
            return output;
        }
    }
}
