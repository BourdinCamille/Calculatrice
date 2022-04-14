using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculatrice.Models.Moteur
{
    public static class Calcul
    {
        /// <summary>
        /// Cette méthode est appelée à chaque fois que l'on rajoute un chiffre côté VM.
        /// Dans le cas où on cumule les clics sur le bouton = , c'est le résultat que l'on prend comme premier opérande.
        /// </summary>
        /// <returns>Retourne le résultat si tous les éléments nécessaires à un calcul ont une valeur, sinon retourne l'opérande un.</returns>
        public static double? Calculer(double? operandeUn, double? operandeDeux, EnumOperateur operateur)
        {
            if (operandeUn != null && operandeDeux != null && operateur != EnumOperateur.Aucun)
            {
                double? resultat = null;

                switch (operateur)
                {
                    case EnumOperateur.Addition:
                        resultat = operandeUn + operandeDeux;
                        break;
                    case EnumOperateur.Soustraction:
                        resultat = operandeUn - operandeDeux;
                        break;
                    case EnumOperateur.Multiplication:
                        resultat = operandeUn * operandeDeux;
                        break;
                    case EnumOperateur.Division:
                        try
                        {
                            resultat = operandeUn / operandeDeux;
                        }
                        catch (DivideByZeroException e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        break;
                }
                return resultat;
            }
            else
            {
                return operandeUn;
            }
        }

        // TO-DO : repenser la validation - possibilité de passer un type générique ?
        /*public static bool ValiderOperande(double operande)
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
