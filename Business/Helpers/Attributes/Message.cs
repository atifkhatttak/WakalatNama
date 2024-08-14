using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helpers.Attributes
{
    public class Message
    {

    }
    [System.AttributeUsage(System.AttributeTargets.All)]

    public class MessageAttribute : System.Attribute
    {
        
        public string value { get;  }

        public MessageAttribute(string message)
        {
            this.value = message;
        }
    }
}
