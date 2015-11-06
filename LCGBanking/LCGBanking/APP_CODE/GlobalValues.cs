using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCGBanking
{
    public static class GlobalValues
    {
        public static string testtyp { get; set; }
        public static string xmlfilename { get; set; }
        public static int anvandarid { get; set; }
        public static int namn { get; set; }
        public static int roles { get; set; }

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
        public static List<Fraga> GVIndResLista = new List<Fraga>();

            /*string path = "Test/Licenseringstest";*/

            /*
            if (användare == "förstanställd") 
            {
                path = "Test/Licenseringstest"; /
            else
                path = "Test/Kunskapstest";
            }
            */
            // string nyProvtillfalle = String.Empty;

        }
    }

