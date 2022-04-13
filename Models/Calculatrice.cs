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

        // Ca ne me sert pas puisque je veux que les opérandes puissent avoir une valeur de 0 mais je garde l'exemple
        /*public bool IsOperandeUnRempli
        {
            get => OperandeUn != 0.0;
        }*/

        public Calculatrice()
        { }

        /// <summary>
        /// Cette méthode est appelée à chaque fois que l'on rajoute un chiffre côté VM.
        /// </summary>
        /// <returns>Retourne le résultat si tous les éléments nécessaires à un calcul ont une valeur, sinon retourne l'opérande un.</returns>
        public double? CalculerResultat()
        {
            if (OperandeUn != null && OperandeDeux != null && Operateur != EnumOperateur.Aucun)
            {
                switch (Operateur)
                {
                    case EnumOperateur.Addition:
                        Resultat = OperandeUn + OperandeDeux;
                        break;
                    case EnumOperateur.Soustraction:
                        Resultat = OperandeUn - OperandeDeux;
                        break;
                    case EnumOperateur.Multiplication:
                        Resultat = OperandeUn * OperandeDeux;
                        break;
                    case EnumOperateur.Division:
                        try
                        {
                            Resultat = OperandeUn / OperandeDeux;
                        }
                        catch (DivideByZeroException e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        break;
                }
                return Resultat;
            }
            else
            {
                return OperandeUn;
            }
        }

        /// <summary>
        /// Cette méthode est appelée quand on clique sur le bouton =. Dans ce cas, le résultat se substitue à l'opérande un et sa valeur varie
        /// à chaque nouveau clic sur le bouton = tandis que la valeur de l'opérande deux, elle, ne change pas.
        /// </summary>
        /// <returns></returns>
        public double? CalculerResultatClicsConsecutifsSurBtEgal()
        {
            if (Resultat != null && OperandeDeux != null && Operateur != EnumOperateur.Aucun)
            {
                switch (Operateur)
                {
                    case EnumOperateur.Addition:
                        Resultat = Resultat + OperandeDeux;
                        break;
                    case EnumOperateur.Soustraction:
                        Resultat = Resultat - OperandeDeux;
                        break;
                    case EnumOperateur.Multiplication:
                        Resultat = Resultat * OperandeDeux;
                        break;
                    case EnumOperateur.Division:
                        try
                        {
                            Resultat = Resultat / OperandeDeux;
                        }
                        catch (DivideByZeroException e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        break;
                }
                return Resultat;
            }
            else
            {
                return OperandeUn;
            }
        }
        // TO-DO : repenser la validation - possibilité de passer un type générique ?
        /*public bool ValiderOperande(double operande)
        {
            if (Double.IsNaN(operande))
            {
                throw new ArgumentException();
            }
            else if (operande <= double.MaxValue && operande >= double.MinValue)
            {
                throw new OverflowException();
            }
            else
            {
                return true;
            }
        }*/
    }
}
