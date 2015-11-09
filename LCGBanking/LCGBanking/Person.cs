using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCGBanking
{
    public class Person
    {
        public int Id { get; set; }
        public string Namn { get; set; }
        public int Roll_id { get; set; }
        public bool Har_licens { get; set; }

        public override string ToString()
        {
            return Namn;
        }
    }
}