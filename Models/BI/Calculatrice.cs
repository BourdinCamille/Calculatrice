using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculatrice.Models
{
    /// <summary>
    /// Les variables numériques sont initialisées à null car au moment de faire le calcul, on va vérifier que les opérandes contiennent bien une valeur.
    /// Or, on veut que 0 puisse être considéré comme une valeur à part entière.
    /// </summary>
    public class Calculatrice
    {
        public double? OperandeUn { get; set; } = null;

        public double? OperandeDeux { get; set; } = null;

        public EnumOperateur Operateur { get; set; } = EnumOperateur.Aucun;

        public double? Resultat { get; set; } = null;

        public Calculatrice()
        { }

    }
}
