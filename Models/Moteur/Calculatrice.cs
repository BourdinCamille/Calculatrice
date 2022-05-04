using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculatrice.Models.Moteur
{
    public static class Calculatrice
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

        /// <summary>
        /// Cette méthode sert à faire des comparaisons d'objets par valeur au lieu de le faire par référence comme c'est le cas par défaut.
        /// </summary>
        /// <param name="calcul1"></param>
        /// <param name="calcul2"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static bool HasValeursIdentiques(Calcul calcul1, Calcul calcul2)
        {
            if (calcul1 != null && calcul2 != null)
            {
                if (calcul1.OperandeUn == calcul2.OperandeUn &&
                    calcul1.Operateur == calcul2.Operateur &&
                    calcul1.OperandeDeux == calcul2.OperandeDeux)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new NullReferenceException();
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
