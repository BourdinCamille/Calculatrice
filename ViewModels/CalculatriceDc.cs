using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Calculatrice.Commands;
using Calculatrice.Models;
using Calculatrice.Models.Moteur;

namespace Calculatrice.ViewModels
{
    // TO-DO :
    // - coder le corps des méthodes Can...
    // - gestion des exceptions en créant une classe dédiée
    // - gérer les références nulles
    // - gérer les exposants des chiffres trop longs
    // - écrire des TU
    public class CalculatriceDc : ViewModelBase
    {
        private Models.Calculatrice _Calculatrice;

        // On crée des champs correspondant à ceux du modèle et on les lie au moyen des get / set.

        /// <summary>
        /// Dans le set, on effectue la conversion vers le modèle uniquement si le dernier char de l'opérande n'est pas une virgule, sinon ça plante.
        /// Le traitement de ce cas de figure se fait dans la méthode <c>AjouterChiffreOuVirgule</c> et au moyen du booléen <c>IsDecimaleNonTraitee</c>
        /// </summary>
        public string OperandeUnVm
        {
            get
            {
                return _Calculatrice.OperandeUn.ToString();
            }
            set
            {
                if (OperandeUnVm != "")
                {
                    if (value[value.Length - 1] != ',')
                    {
                        _Calculatrice.OperandeUn = double.Parse(value);
                    }
                }
                else
                {
                    _Calculatrice.OperandeUn = 0.0;
                }
            }
        }

        /// <summary>
        /// Le but de cette variable est d'être passée en paramètre quand la méthode statique <c>Calculer</c> est appelée, pour ne
        /// pas avoir à refaire la conversion à chaque fois.
        /// </summary>
        public double OperandeUnDouble
        {
            get => double.Parse(OperandeUnVm);
        }

        /// <summary>
        /// Dans le set, on effectue la conversion vers le modèle uniquement si le dernier char de l'opérande n'est pas une virgule, sinon ça plante.
        /// Le traitement de ce cas de figure se fait dans la méthode <c>AjouterChiffreOuVirgule</c> et au moyen du booléen <c>IsDecimaleNonTraitee</c>
        /// </summary>
        public string OperandeDeuxVm
        {
            get
            {
                return _Calculatrice.OperandeDeux.ToString();
            }
            set
            {
                if (OperandeDeuxVm != "")
                {
                    if (value[value.Length - 1] != ',')
                    {
                        _Calculatrice.OperandeDeux = double.Parse(value);
                    }
                }
                else
                {
                    _Calculatrice.OperandeDeux = 0.0;
                }
            }
        }

        /// <summary>
        /// Le but de cette variable est d'être passée en paramètre quand la méthode statique <c>Calculer</c> est appelée, pour ne
        /// pas avoir à refaire la conversion à chaque fois.
        /// </summary>
        public double OperandeDeuxDouble
        {
            get => double.Parse(OperandeDeuxVm);
        }

        /// <summary>
        /// Ce booléen est un marqueur qui indique que la virgule est en cours d'ajout dans une opérande.
        /// </summary>
        public bool IsDecimaleNonTraitee { get; set; }

        public EnumOperateur OperateurVm
        {
            get
            {
                return _Calculatrice.Operateur;
            }
            set
            {
                _Calculatrice.Operateur = value;
            }
        }

        public string ResultatVm
        {
            get
            {
                return _Calculatrice.Resultat.ToString();
            }
            set
            {
                if (ResultatVm != "")
                {
                    _Calculatrice.Resultat = double.Parse(value);
                }
                else
                {
                    _Calculatrice.Resultat = 0.0;
                }
            }
        }

        /// <summary>
        /// Le but de cette variable est d'être passée en paramètre quand la méthode statique <c>Calculer</c> est appelée, pour ne
        /// pas avoir à refaire la conversion à chaque fois.
        /// </summary>
        public double ResultatDouble
        {
            get => double.Parse(ResultatVm);
        }

        /// <summary>
        /// Ce booléen est un marqueur qui va permettre d'appeler la méthode <c>CaculerResultat</c> s'il s'agit du premier clic sur le bouton = pour une nouvelle opération ou bien d'appeler la méthode <c>CalculerResultatClicsConsecutifsSurBtEgal</c> si ces clics sont cumulés. Il va également permettre de réinitaliser l'opération si l'utilisateur clique sur un chiffre après avoir cliqué sur le bouton =.
        /// </summary>
        public bool IsBtEgalDejaClique { get; set; }

        private string _AffichageEnCours;

        public string AffichageEnCours
        {
            get
            {
                return _AffichageEnCours;
            }
            set
            {
                _AffichageEnCours = value;
                OnPropertyChanged(nameof(AffichageEnCours));
            }
        }

        private string _AffichageFinal;

        public string AffichageFinal
        {
            get
            {
                return _AffichageFinal;
            }
            set
            {
                _AffichageFinal = value;
                OnPropertyChanged(nameof(AffichageFinal));
            }
        }

        public ICommand AjouterChiffreOuVirguleCommand { get; }
        public ICommand DefinirOperateurCommand { get; }
        public ICommand CalculerResultatCommand { get; }
        public ICommand ReinitialiserCommand { get; }
        public ICommand ChangerSigneOperandeCommand { get; }

        public CalculatriceDc()
        {
            _Calculatrice = new Models.Calculatrice();
            OperandeUnVm = String.Empty;
            OperandeDeuxVm = String.Empty;
            IsDecimaleNonTraitee = false;
            OperateurVm = EnumOperateur.Aucun;
            ResultatVm = String.Empty;
            IsBtEgalDejaClique = false;
            _AffichageEnCours = String.Empty;
            _AffichageFinal = String.Empty;
            AjouterChiffreOuVirguleCommand = new CommandBase<string>(AjouterChiffreOuVirgule, CanAjouterChiffreOuVirgule);
            DefinirOperateurCommand = new CommandBase<string>(DefinirOperateur, CanDefinirOperateur);
            CalculerResultatCommand = new NoParameterCommand(CalculerResultat, CanCalculerResultat);
            ReinitialiserCommand = new NoParameterCommand(Reinitialiser, CanReinitialiser);
            ChangerSigneOperandeCommand = new NoParameterCommand(ChangerSigneOperande, CanChangerSigneOperande);
        }

        private bool CanAjouterChiffreOuVirgule(string chiffreOuVirgule)
        {
            return true;
        }

        private void AjouterChiffreOuVirgule(string chiffreOuVirgule)
        {
            if (IsBtEgalDejaClique)
            {
                Reinitialiser();
            }

            switch (chiffreOuVirgule)
            {
                case ",":
                    if (OperateurVm == EnumOperateur.Aucun)
                    {
                        if (!OperandeUnVm.Contains(",") && OperandeUnVm != "")
                        {
                            AffichageEnCours = OperandeUnVm + chiffreOuVirgule;
                            /* A ce niveau, on ne peut pas encore traiter la décimale (sinon ça plante dans le set de l'opérande), on va le faire lors de l'ajout du prochain chiffre.*/
                            IsDecimaleNonTraitee = true;
                        }
                    }
                    else
                    {
                        if (!OperandeDeuxVm.Contains(",") && OperandeDeuxVm != "")
                        {
                            AffichageEnCours = OperandeUnVm + (char)OperateurVm + OperandeDeuxVm + chiffreOuVirgule;
                            IsDecimaleNonTraitee = true;
                        }
                    }
                    break;
                default:
                    if (OperateurVm == EnumOperateur.Aucun)
                    {
                        /* Le marqueur booléen indique que la virgule est en cours d'ajout mais n'a pas encore été prise en compte dans l'opérande.
                        Maintenant que l'utilisateur a cliqué sur un autre chiffre, on peut enfin ajouter la virgule, cet autre chiffre étant la première décimale. 
                        Enfin, on remet le marqueur a false car la virgule a été traitée.*/
                        if (IsDecimaleNonTraitee && !OperandeUnVm.Contains(","))
                        {
                            OperandeUnVm = OperandeUnVm + "," + chiffreOuVirgule;
                            IsDecimaleNonTraitee = false;
                        }
                        else
                        {
                            OperandeUnVm += chiffreOuVirgule;
                        }

                        AffichageEnCours = OperandeUnVm;
                    }
                    else
                    {
                        if (IsDecimaleNonTraitee && !OperandeDeuxVm.Contains(","))
                        {
                            OperandeDeuxVm = OperandeDeuxVm + "," + chiffreOuVirgule;
                            IsDecimaleNonTraitee = false;
                        }
                        else
                        {
                            OperandeDeuxVm += chiffreOuVirgule;
                        }

                        AffichageEnCours = OperandeUnVm + (char)OperateurVm + OperandeDeuxVm;
                    }
                    ResultatVm = Calcul.Calculer(OperandeUnDouble, OperandeDeuxDouble, OperateurVm).ToString();
                    IsBtEgalDejaClique = false;
                    AffichageFinal = ResultatVm;
                    break;
            }
        }

        public bool CanDefinirOperateur(string operateur)
        {
            return true;
        }
        public void DefinirOperateur(string operateur)
        {
            /* Au cas où l'utilisateur aurait cliqué sur la virgule juste avant de cliquer sur l'opérateur, on empêche le "transfert" de la virgule comme premier caractère du deuxième opérande.*/
            IsDecimaleNonTraitee = false;
            if (OperateurVm == EnumOperateur.Aucun)
            {
                switch (operateur)
                {
                    case "+":
                        OperateurVm = EnumOperateur.Addition;
                        break;
                    case "-":
                        OperateurVm = EnumOperateur.Soustraction;
                        break;
                    case "*":
                        OperateurVm = EnumOperateur.Multiplication;
                        break;
                    case "/":
                        OperateurVm = EnumOperateur.Division;
                        break;
                }
                AffichageEnCours = OperandeUnVm + (char)OperateurVm;
            }
            else
            {
                OperandeUnVm = ResultatVm;
                OperandeDeuxVm = "0";
                switch (operateur)
                {
                    case "+":
                        OperateurVm = EnumOperateur.Addition;
                        break;
                    case "-":
                        OperateurVm = EnumOperateur.Soustraction;
                        break;
                    case "*":
                        OperateurVm = EnumOperateur.Multiplication;
                        break;
                    case "/":
                        OperateurVm = EnumOperateur.Division;
                        break;
                }
            }
        }

        public bool CanCalculerResultat()
        {
            return true;
        }

        public void CalculerResultat()
        {
            if (IsBtEgalDejaClique == false)
            {
                ResultatVm = Calcul.Calculer(OperandeUnDouble, OperandeDeuxDouble, OperateurVm).ToString();
                AffichageEnCours = OperandeUnVm + (char)OperateurVm + OperandeDeuxVm;
                IsBtEgalDejaClique = true;
            }
            else
            {
                ResultatVm = Calcul.Calculer(ResultatDouble, OperandeDeuxDouble, OperateurVm).ToString();
                AffichageEnCours = (char)OperateurVm + OperandeDeuxVm;
            }
            AffichageFinal = ResultatVm;
        }

        public bool CanReinitialiser()
        {
            return true;
        }

        public void Reinitialiser()
        {
            OperandeUnVm = "0";
            OperandeDeuxVm = "0";
            OperateurVm = EnumOperateur.Aucun;
            ResultatVm = "0";
            IsBtEgalDejaClique = false;
            AffichageEnCours = String.Empty;
            AffichageFinal = String.Empty;
        }

        public bool CanChangerSigneOperande()
        {
            return true;
        }

        public void ChangerSigneOperande()
        {
            if (OperateurVm == EnumOperateur.Aucun)
            {
                if (OperandeUnVm.StartsWith("-"))
                {
                    OperandeUnVm = OperandeUnVm.Remove(0, 1);
                }
                else
                {
                    OperandeUnVm = "-" + OperandeUnVm;
                }
                AffichageEnCours = OperandeUnVm;
            }
            else
            {
                if (OperandeDeuxVm.StartsWith("-"))
                {
                    OperandeDeuxVm = OperandeDeuxVm.Remove(0, 1);
                }
                else
                {
                    OperandeDeuxVm = "-" + OperandeDeuxVm;
                }
                AffichageEnCours = OperandeUnVm + (char)OperateurVm + OperandeDeuxVm;
            }
            ResultatVm = Calcul.Calculer(OperandeUnDouble, OperandeDeuxDouble, OperateurVm).ToString();
            AffichageFinal = ResultatVm;

        }

    }
}
