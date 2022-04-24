using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Calculatrice.Models;

namespace Calculatrice.ViewModels
{
    public class OperationVM : ViewModelBase
    {
        /// <summary>
        /// Pour ne pas exposer / exploiter le BI directement dans la vue, on a besoin d'un intermédiaire.
        /// C'est le rôle de cette VM qui prend l'objet BI en paramètre.
        /// </summary>
        /// <param name="calcul"></param>
        public OperationVM(Calcul calcul)
        {
            CalculModel = new Calcul(calcul);
        }

        public Calcul CalculModel { get; }

        public double? OperandeUn
        {
            get => CalculModel.OperandeUn;
        }

        public double? OperandeDeux
        {
            get => CalculModel.OperandeDeux;
        }

        public EnumOperateur Operateur
        {
            get => CalculModel.Operateur;
        }

        public double? Resultat
        {
            get => CalculModel.Resultat;
        }
    }
}
