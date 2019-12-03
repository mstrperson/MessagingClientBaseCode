using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingClientBaseCode
{
    public class Message
    {
        public static readonly string Separator = "|";
        public static readonly string EOM = "<EOM>";

        private List<string> To;

        private string Text;

        public byte[] RawMessage
        {
            get
            {
                if (To == null) To = new List<string>() { "server" };

                string message = "";
                bool first = true;
                foreach(string recipient in To)
                {
                    message += string.Format("{0}{1}", first ? "" : ",", recipient);
                    first = false;
                }

                message += string.Format("{0}{1}{2}", Separator, Text, EOM);

                return Encoding.ASCII.GetBytes(message);
            }
        }

        public Message()
        {
        }

        public Message(string to, string text)
        {
            To = new List<string>() { to };
            Text = text;
        }

        public Message(List<string> to, string text)
        {
            To = to;
            Text = text;
        }
    }
}
