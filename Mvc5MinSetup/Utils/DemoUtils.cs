using System.Web.Script.Serialization;

namespace Mvc5MinSetup.Utils
{
    public static class DemoUtils
    {
        public static string Encode(object input)
        {
            return new JavaScriptSerializer().Serialize(input);
        }
    }
}