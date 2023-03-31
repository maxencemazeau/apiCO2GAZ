using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT
{
    public class historiqueCO2
    {
        public int Id { get; set; }
        public string Niveau { get; set; }
        public DateTime Date { get; set; }
        public int UtilisateurId { get; set; }
        // Add other properties based on your database schema
    }

    public class Utilisateur
    {
        public int Id { get; set; }
        public string Login { get; set;  }
        public string Password { get; set; }
    }
}