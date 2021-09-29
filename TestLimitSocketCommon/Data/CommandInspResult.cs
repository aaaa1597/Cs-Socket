using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLimitSocketCommon.Data
{
    [Serializable]
    public class CommandInspResult : CommandBase
    {
        public bool IsErr;
        public int WorkNo;
        public byte[] Scores = new byte[34];
        public int ImgSize;
        public byte[] ImgData;
        public int ImgSize2;
        public byte[] ImgData2;

        public CommandInspResult() : base("INSP_READY")
        {
        }
    }
}
