using Eiscp.Core.Helper;
using System.Collections.Specialized;

namespace Eiscp.Core.Model
{
    public class Response
    {
        private readonly OrderedDictionary _value;
        public string Key { get; }
        public string Value { get; }

        public Response(string response, OrderedDictionary value)
        {
            _value = value;
            this.Key = response.Substring(0, 3);
            this.Value = response.Substring(3);
        }

        public int FromHex()
        {
            if (int.TryParse(Value, System.Globalization.NumberStyles.HexNumber, null, out int result))
            {
                return result;
            }
            return -1;
        }

        public string ToHex() => string.Format("{0:X2}", ToInt()).ToUpper();

        public int ToInt()
        { 
            if (int.TryParse(Value, out int result))
            {
                return result;
            }
            return -1;
        }

        public bool ToBoolean() => ToInt() > 0;

        public string Description
        {
            get
            {
                try
                {
                    return (string)Utils.Nav(_value, "values", Value, "description");
                }
                catch
                {
                    return null;
                }
            }
        }

        //public static explicit operator int(Response response) => response.ToDec();

        //public static explicit operator string(Response response) => response.ToHex();

        //public static explicit operator bool(Response response) => response.ToBoolean();
    }
}
