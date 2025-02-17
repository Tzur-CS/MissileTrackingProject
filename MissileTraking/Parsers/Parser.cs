namespace MissileTracking.Parsers;

using System.Collections.Generic;

public class Parser
{
    public static Dictionary<string, string> Parse(string str)
    {
        var map = new Dictionary<string, string>();

        string[] requestSplit = str.Split('@');
        map["CommandType"] = requestSplit[0];
        
        if (requestSplit.Length == 1)
        {
            return map;
        }

        map["args"] = requestSplit[1];
        return map;
    }
}
