﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCGBanking
{
    public static class GlobalValues
    {
        /*string path = "Test/Licenseringstest";*/
        
        /*
        if (användare == "förstanställd") 
        {
            path = "Test/Licenseringstest"; /
        else
            path = "Test/Kunskapstest";
        }
        */
        string nyProvtillfalle = String.Empty;

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

        public static List<Fraga> Fragor = new List<Fraga>();
    }
}
