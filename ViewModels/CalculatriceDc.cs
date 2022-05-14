using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Calculatrice.Commands;
using Calculatrice.GrpcClient;
using Calculatrice.Models;
using Calculatrice.Views;
using Grpc.Net.Client;
using GrpcServer;
using GrpcServer.BI;
using EnumOperateur = Calculatrice.Models.EnumOperateur;

namespace Calculatrice.ViewModels
{
    // TO-DO :
    // - refacto pour toute la partie allant du calcul à son enregistrement (si possible)
    // - coder le corps des méthodes Can...
    // - gestion des exceptions en créant une classe dédiée
    // - gérer les références nulles
    // - gérer les exposants des chiffres trop longs
    // - écrire des TU
    public class CalculatriceDc : ViewModelBase
    {
        private Calcul _Calcul;

        private CalculsClient _CalculsClient;

        private HistoriqueDC _HistoriqueDC;

        private string _AffichageEnCours;

        private string _AffichageFinal;

        public CalculatriceDc()
        {
            _Calcul = new Calcul();
            _CalculsClient = new CalculsClient();
            _HistoriqueDC = new HistoriqueDC(this);
            OperandeUnVm = String.Empty;
            OperandeDeuxVm = String.Empty;
            IsDecimaleNonTraitee = false;
            IsOperandeFromHistorique = false;
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
                return _Calcul.OperandeUn.ToString();
            }
            set
            {
                if (OperandeUnVm != "")
                {
                    if (value[value.Length - 1] != ',')
                    {
                        _Calcul.OperandeUn = double.Parse(value);
                    }
                }
                else
                {
                    _Calcul.OperandeUn = 0.0;
                }
                OnPropertyChanged(nameof(OperandeUnVm));
            }
        }

        /// <summary>
        /// Dans le set, on effectue la conversion vers le modèle uniquement si le dernier char de l'opérande n'est pas une virgule, sinon ça plante.
        /// Le traitement de ce cas de figure se fait dans la méthode <c>AjouterChiffreOuVirgule</c> et au moyen du booléen <c>IsDecimaleNonTraitee</c>
        /// </summary>
        public string OperandeDeuxVm
        {
            get
            {
                return _Calcul.OperandeDeux.ToString();
            }
            set
            {
                if (OperandeDeuxVm != "")
                {
                    if (value[value.Length - 1] != ',')
                    {
                        _Calcul.OperandeDeux = double.Parse(value);
                    }
                }
                else
                {
                    _Calcul.OperandeDeux = 0.0;
                }
                OnPropertyChanged(nameof(OperandeDeuxVm));
            }
        }

        public EnumOperateur OperateurVm
        {
            get
            {
                return _Calcul.Operateur;
            }
            set
            {
                _Calcul.Operateur = value;
            }
        }

        public string ResultatVm
        {
            get
            {
                return _Calcul.Resultat.ToString();
            }
            set
            {
                if (ResultatVm != "")
                {
                    _Calcul.Resultat = double.Parse(value);
                }
                else
                {
                    _Calcul.Resultat = 0.0;
                }
                OnPropertyChanged(nameof(ResultatVm));
            }
        }

        /// <summary>
        /// Ce booléen est un marqueur qui va permettre d'appeler la méthode <c>CaculerResultat</c> s'il s'agit du premier clic sur le bouton = pour une nouvelle opération ou bien d'appeler la méthode <c>CalculerResultatClicsConsecutifsSurBtEgal</c> si ces clics sont cumulés. Il va également permettre de réinitaliser l'opération si l'utilisateur clique sur un chiffre après avoir cliqué sur le bouton =.
        /// </summary>
        public bool IsBtEgalDejaClique { get; set; }

        /// <summary>
        /// Ce booléen est un marqueur qui indique que la virgule est en cours d'ajout dans une opérande.
        /// </summary>
        public bool IsDecimaleNonTraitee { get; set; }

        public bool IsOperandeFromHistorique { get; set; }

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
            get => _Calcul;
        }

        public CalculsClient CalculsClient
        {
            get => _CalculsClient;
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

        private bool CanAjouterChiffreOuVirgule(string chiffreOuVirgule)
        {
            return !IsOperandeFromHistorique;
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
                    if(CalculIsConforme(_Calcul))
                    {
                        var resultat = _CalculsClient.EnvoyerCalculAuServeur(_Calcul);
                        ResultatVm = resultat.ToString(CultureInfo.CurrentCulture);
                        if (OperateurVm != EnumOperateur.Aucun)
                        {
                            EnregistrerCalculEtMettreVueAJour(_HistoriqueDC, _Calcul);
                        }
                        IsBtEgalDejaClique = false;
                        AffichageFinal = ResultatVm;
                    }
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
            IsOperandeFromHistorique = false;
        }

        public bool CanCalculerResultat()
        {
            return true;
        }

        public void CalculerResultat()
        {
            if (OperateurVm != EnumOperateur.Aucun && CalculIsConforme(_Calcul))
            {
                if (IsBtEgalDejaClique == false)
                {
                    var resultat = _CalculsClient.EnvoyerCalculAuServeur(_Calcul);
                    ResultatVm = resultat.ToString(CultureInfo.CurrentCulture);

                    // On récupère le dernier calcul de l'historique de la VM et on le compare avec celui qui vient d'être réalisé.S'ils sont identiques on ne l'affiche et on ne le persiste pas deux fois.
                    if (_HistoriqueDC.Historique.Count != 0)
                    {
                        OperationVM lastOpVm = _HistoriqueDC.Historique.Last();
                        Calcul lastCalcul = lastOpVm.CalculModel;

                        if (!_HistoriqueDC.HistoriqueClient.HasValeursIdentiques(_Calcul, lastCalcul))
                        {
                            EnregistrerCalculEtMettreVueAJour(_HistoriqueDC, _Calcul);
                        }
                    }
                    AffichageEnCours = OperandeUnVm + (char)OperateurVm + OperandeDeuxVm;
                    IsBtEgalDejaClique = true;
                }
                else
                {
                    // Dans le cas du cumul de clics sur le bouton "égal", le résultat de la dernière opération devient le premier opérande de l'opération suivante.
                    OperandeUnVm = ResultatVm;
                    _Calcul.OperandeUn = _Calcul.Resultat;
                    var resultat = _CalculsClient.EnvoyerCalculAuServeur(_Calcul);
                    ResultatVm = resultat.ToString(CultureInfo.CurrentCulture);
                    EnregistrerCalculEtMettreVueAJour(_HistoriqueDC, _Calcul);
                    AffichageEnCours = (char)OperateurVm + OperandeDeuxVm;
                }
                IsOperandeFromHistorique = false;
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
            IsOperandeFromHistorique = false;
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
            var resultat = _CalculsClient.EnvoyerCalculAuServeur(_Calcul);
            ResultatVm = resultat.ToString(CultureInfo.CurrentCulture);
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

        public void EnregistrerCalculEtMettreVueAJour(HistoriqueDC histoDc, Calcul calcul)
        {
            if (calcul.Operateur != EnumOperateur.Aucun)
            {
                histoDc.HistoriqueClient.EnregistrerCalculSurServeur(calcul);
                histoDc.MettreVueAJour(calcul);
            }
        }

        /// <summary>
        /// N'est utilisée que pour vérifier la conformité d'une divsion pour le moment, mais à développer...
        /// </summary>
        /// <param name="calcul"></param>
        /// <returns></returns>
        public bool CalculIsConforme(Calcul calcul)
        {
            if (calcul.Operateur == EnumOperateur.Division && calcul.OperandeDeux == 0)
            {
                MessageBox.Show("Division par zéro impossible");
                return false;
            }
            else return true;
        }
    }

}
