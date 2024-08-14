using Business.Helpers.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helpers
{
    public static class EnumHelpers
    {
        public static string GetDescription(this Enum value)
        {
            string des = "";
            try
            {
                FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
                if (fieldInfo == null) return null;
                var attribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute));
                des= attribute==null?value.ToString():attribute?.Description;
            }
            catch (Exception ex)
            {
            }
            return des;
        }
        public static string GetMessage(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null) return null;
            var attribute = (MessageAttribute)fieldInfo.GetCustomAttribute(typeof(MessageAttribute));
            return attribute.value;
        }
    }
}
