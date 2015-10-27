using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCGBanking
{
    public static class GlobalValues
    {
        static int _frageNr = 1;

        public static int FrageNr
        {
            get
            {
                return _frageNr;
            }
            set
            {
                _frageNr = value;
            }
        }
    }
}