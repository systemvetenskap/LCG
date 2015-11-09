using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace LCGBanking
{
    public static class GlobalValues
    {
        private const string conString = "cirkus";
        public static string testtyp { get; set; }
        public static string xmlfilename { get; set; }
        public static int anvandarid { get; set; }
        public static string namn { get; set; }
        public static int roles { get; set; }

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


        /// <summary>
        /// Metoden använd för att hämta anvandarid med hjälp av anvandarnamn (i samband med inloggning). 
        /// Anvandarid används för person identifiering och styrning. Bland annat används detta för att 
        /// bestämma vilken typ av test användaren bör göra - ha tillgång till.
        /// </summary>
        /// <param name="anvandarnamn"></param>
        /// <returns></returns>
        public static int GeAnvandarId(string anvandarnamn)
        {
            int anvandarid = 0;
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "SELECT id FROM lcg_konto WHERE anvandarnamn = :anvandarnamn;";
                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);
                command.Parameters.Add(new NpgsqlParameter("anvandarnamn", NpgsqlDbType.Varchar));
                command.Parameters["anvandarnamn"].Value = anvandarnamn;
                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    anvandarid = (int)(dr["id"]);
                }
            }
            finally
            {
                conn.Close();
            }
            return anvandarid;
        }

        /// <summary>
        /// Returnerar för respektive användare
        /// </summary>
        /// <param name="personid"></param>
        /// <returns></returns>
        public static string GeNamn(int anvandarid)
        {
            string namn = string.Empty;
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[conString];
            NpgsqlConnection conn = new NpgsqlConnection(settings.ConnectionString);
            try
            {
                conn.Open();
                string sql = "SELECT p.namn namn FROM lcg_person p, lcg_konto k WHERE p.id = k.fk_person_id AND k.id = :anvandarid;";
                NpgsqlCommand command = new NpgsqlCommand(@sql, conn);

                command.Parameters.Add(new NpgsqlParameter("anvandarid", NpgsqlDbType.Integer));
                command.Parameters["anvandarid"].Value = anvandarid;

                NpgsqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    namn = Convert.ToString(dr["namn"]);
                }
            }
            finally
            {
                conn.Close();
            }
            return namn;
        }

    }
}
