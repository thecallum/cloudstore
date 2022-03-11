using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentServiceListener.Helpers
{
    public static class EntityHelper
    {
        public static Guid? TryParseGuid(Dictionary<string, object> body, string name)
        {
            if (!body.ContainsKey(name)) return null;

            var paramter = body[name].ToString();

            try
            {
                return Guid.Parse(paramter);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
