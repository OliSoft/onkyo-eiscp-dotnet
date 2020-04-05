using System;
using System.Collections.Generic;
using System.Text;

namespace Onkyo.Core.Extention
{
    public static class StringExtention
    {
        public static bool ToBoolean(this string value)
        {
            switch (value.ToLower())
            {
                case "true":
                case "t":
                case "1":
                case "01":
                case "on":
                    return true;
                case "0":
                case "00":
                case "false":
                case "f":
                case "standby":
                    return false;
                default:
                    return false;
                    //throw new InvalidCastException("You can't cast that value to a bool!");
            }
        }

        public static int ToInt(this string value)
        {
            if(int.TryParse(value, out int result))
            {
                return result;
            }
            return -1;
        }
    }
}
