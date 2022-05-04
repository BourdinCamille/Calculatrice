using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Calculatrice.Commands;
using Calculatrice.Models;
using Calculatrice.Models.Moteur;
using Calculatrice.Views;

namespace Calculatrice.ViewModels
{
    // TO-DO :
    // - empêcher la concaténation d'un opérande issu de l'historique avec une entrée depuis la calculatrice !! (voir CanAjouterChiffreOuVirgule)
    // - coder le corps des méthodes Can...
    // - gestion des exceptions en créant une classe dédiée
    // - gérer les références nulles
    // - gérer les exposants des chiffres trop longs
    // - écrire des TU
    public class CalculatriceDc : ViewModelBase
    {
        private Calcul _calcul;

        private HistoriqueDC _HistoriqueDC;

        private string _AffichageEnCours;

        private string _AffichageFinal;

        public CalculatriceDc()
        {
            _calcul = new Calcul();
            _HistoriqueDC = new HistoriqueDC(this);
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
            AccederHistoriqueCommand = new NoParameterCommand(AccederHistorique, CanAccederHistorique);
            FermerEtSauverHistoriqueCommand = new NoParameterCommand(FermerEtSauverHistorique, CanFermerEtSauverHistorique);
        }

        // On crée des champs correspondant à ceux du modèle et on les lie au moyen des get / set.

        /// <summary>
        /// Dans le set, on effectue la conversion vers le modèle uniquement si le dernier char de l'opérande n'est pas une virgule, sinon ça plante.
        /// Le traitement de ce cas de figure se fait dans la méthode <c>AjouterChiffreOuVirgule</c> et au moyen du booléen <c>IsDecimaleNonTraitee</c>
        /// </summary>
        public string OperandeUnVm
        {
            get
            {
                return _calcul.OperandeUn.ToString();
            }
            set
            {
                if (OperandeUnVm != "")
                {
                    if (value[value.Length - 1] != ',')
                    {
                        _calcul.OperandeUn = double.Parse(value);
                    }
                }
                else
                {
                    _calcul.OperandeUn = 0.0;
                }
                OnPropertyChanged(nameof(OperandeUnVm));
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
                return _calcul.OperandeDeux.ToString();
            }
            set
            {
                if (OperandeDeuxVm != "")
                {
                    if (value[value.Length - 1] != ',')
                    {
                        _calcul.OperandeDeux = double.Parse(value);
                    }
                }
                else
                {
                    _calcul.OperandeDeux = 0.0;
                }
                OnPropertyChanged(nameof(OperandeDeuxVm));
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

        public EnumOperateur OperateurVm
        {
            get
            {
                return _calcul.Operateur;
            }
            set
            {
                _calcul.Operateur = value;
            }
        }

        public string ResultatVm
        {
            get
            {
                return _calcul.Resultat.ToString();
            }
            set
            {
                if (ResultatVm != "")
                {
                    _calcul.Resultat = double.Parse(value);
                }
                else
                {
                    _calcul.Resultat = 0.0;
                }
                OnPropertyChanged(nameof(ResultatVm));
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

        /// <summary>
        /// Ce booléen est un marqueur qui indique que la virgule est en cours d'ajout dans une opérande.
        /// </summary>
        public bool IsDecimaleNonTraitee { get; set; }

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

        public Calcul Calcul
        {
            get => _calcul;
        }

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
        public ICommand AccederHistoriqueCommand { get; }
        public ICommand FermerEtSauverHistoriqueCommand { get; }

        private bool CanAjouterChiffreOuVirgule(string chiffreOuVirgule)
        {
            return !_HistoriqueDC.IsOperandeFromHistorique;
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
                    ResultatVm = Models.Moteur.Calculatrice.Calculer(OperandeUnDouble, OperandeDeuxDouble, OperateurVm).ToString();
                    if (OperateurVm != EnumOperateur.Aucun)
                    {
                        _HistoriqueDC.EnregistrerCalcul(_calcul);
                    }
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
            _HistoriqueDC.IsOperandeFromHistorique = false;
        }

        public bool CanCalculerResultat()
        {
            return true;
        }

        public void CalculerResultat()
        {
            if (OperateurVm != EnumOperateur.Aucun)
            {
                if (IsBtEgalDejaClique == false)
                {
                    ResultatVm = Models.Moteur.Calculatrice.Calculer(OperandeUnDouble, OperandeDeuxDouble, OperateurVm).ToString();

                    // On récupère le dernier calcul de l'historique et on le compare avec celui qui vient d'être réalisé.S'ils sont identiques on ne persiste pas deux fois.
                    OperationVM lastOpVm = _HistoriqueDC.Historique.Last();
                    Calcul lastCalcul = lastOpVm.CalculModel;

                    if (!Models.Moteur.Calculatrice.HasValeursIdentiques(_calcul, lastCalcul))
                    {
                        _HistoriqueDC.EnregistrerCalcul(_calcul);
                    }
                    AffichageEnCours = OperandeUnVm + (char)OperateurVm + OperandeDeuxVm;
                    IsBtEgalDejaClique = true;
                }
                else
                {
                    OperandeUnVm = ResultatVm;
                    ResultatVm = Models.Moteur.Calculatrice.Calculer(ResultatDouble, OperandeDeuxDouble, OperateurVm).ToString();
                    _HistoriqueDC.EnregistrerCalcul(_calcul);
                    AffichageEnCours = (char)OperateurVm + OperandeDeuxVm;
                }
                _HistoriqueDC.IsOperandeFromHistorique = false;
                AffichageFinal = ResultatVm;
            }
            else
            {
                MessageBox.Show("Pas d'opération sans opérateur !");
            }
            
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
            _HistoriqueDC.IsOperandeFromHistorique = false;
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
            ResultatVm = Models.Moteur.Calculatrice.Calculer(OperandeUnDouble, OperandeDeuxDouble, OperateurVm).ToString();
            AffichageFinal = ResultatVm;

        }

        public bool CanAccederHistorique()
        {
            return true;
        }

        public void AccederHistorique()
        {
            FormHistorique historiqueView = new FormHistorique(_HistoriqueDC);
            historiqueView.Show();
        }

        public bool CanFermerEtSauverHistorique()
        {
            return true;
        }

        public void FermerEtSauverHistorique()
        {
            _HistoriqueDC.FermerEtSauverHistorique();
        }
    }
}
