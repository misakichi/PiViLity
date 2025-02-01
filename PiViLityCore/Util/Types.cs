using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Util
{
    public static class Types
    {
        public static bool HasParentType(Type _type, Type parentType)
        {
            Type? type = _type;  
            while (type != null && type!=typeof(object))
            {
                if (type == parentType)
                {
                    return true;
                }
                type = type.BaseType;
            }
            if (type == parentType)
            {
                return true;
            }
            return false;
        }
    }
}
