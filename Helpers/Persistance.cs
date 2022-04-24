using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculatrice.Models;

namespace Calculatrice.Helpers
{
    public static class Persistance
    {
        private static string _CheminFichierHistorique = @"C:\Users\cbourdin\Documents\Calculatrice\CalculsHistorises.xml";

        public static void SauvegarderHistorique(List<Calcul> historique)
        {
            DataSerializer.XmlSerialize(typeof(List<Calcul>), historique, _CheminFichierHistorique);
        }

        public static List<Calcul> RecupererHistorique()
        {
            if (!File.Exists(_CheminFichierHistorique))
                return new List<Calcul>();

            return (List<Calcul>)DataSerializer.XmlDeserialize(typeof(List<Calcul>), _CheminFichierHistorique);
        }
    }
}
