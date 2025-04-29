using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Option
{
    public class HasMultipleOptionAttributeException : Exception
    {
        public HasMultipleOptionAttributeException() : base()
        {
        }
        public HasMultipleOptionAttributeException(string message) : base(message)
        {
        }
        public HasMultipleOptionAttributeException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
