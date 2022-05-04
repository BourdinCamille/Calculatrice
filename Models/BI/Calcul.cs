using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Calculatrice.Models
{
    /// <summary>
    /// Les variables numériques sont initialisées à null car au moment de faire le calcul, on va vérifier que les opérandes contiennent bien une valeur.
    /// Or, on veut que 0 puisse être considéré comme une valeur à part entière.
    /// </summary>
    [Serializable]
    public class Calcul
    {
        public double? OperandeUn { get; set; } = null;

        public double? OperandeDeux { get; set; } = null;

        public EnumOperateur Operateur { get; set; } = EnumOperateur.Aucun;

        public double? Resultat { get; set; } = null;

        public Calcul()
        {}

        /// <summary>
        /// On surcharge le constructeur pour permettre de copier un calcul existant sans conserver sa référence.
        /// </summary>
        /// <param name="calcul"></param>
        public Calcul(Calcul calcul)
        {
            this.OperandeUn = calcul.OperandeUn;
            this.OperandeDeux = calcul.OperandeDeux;
            this.Operateur = calcul.Operateur;
            this.Resultat = calcul.Resultat;
        }
    }
}
