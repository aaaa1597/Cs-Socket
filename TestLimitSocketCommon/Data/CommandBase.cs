using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLimitSocketCommon.Data
{
    [Serializable]
    public class CommandBase
    {
        public readonly string Command;
        public CommandBase(string cmd)
        {
            Command = cmd;
        }
    }
}
