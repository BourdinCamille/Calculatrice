using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Calculatrice.Commands;
using Calculatrice.Helpers;
using Calculatrice.Models;
using Calculatrice.Views;

namespace Calculatrice.ViewModels
{
    public class HistoriqueDC : ViewModelBase
    {
        private ObservableCollection<OperationVM> _Historique;

        private OperationVM _SelectedCalcul;

        private CalculatriceDc _CalculatriceDc;

        public HistoriqueDC(CalculatriceDc calculatriceDc)
        {
            _CalculatriceDc = calculatriceDc;
            _Historique = new ObservableCollection<OperationVM>(GetListeOperationVM());
            IsOperandeFromHistorique = false;
            FermerEtSauverHistoriqueCommand = new NoParameterCommand(FermerEtSauverHistorique, CanFermerEtSauverHistorique);
            UtiliserResultatSelectionCommeOperandeCommand = new CommandBase<OperationVM>(UtiliserResultatSelectionCommeOperande, CanUtiliserResultatSelectionCommeOperande);
        }
        public ObservableCollection<OperationVM> Historique
        {
            get => _Historique;
        }

        public ObservableCollection<OperationVM> DixDerniersCalculs
        {
            get => new ObservableCollection<OperationVM>(_Historique.TakeLast(10));
        }

        public OperationVM SelectedCalcul
        {
            get => _SelectedCalcul;
            set
            {
                _SelectedCalcul = value;
                OnPropertyChanged(nameof(SelectedCalcul));
            }
        }

        public bool IsOperandeFromHistorique { get; set; }

        public ICommand FermerEtSauverHistoriqueCommand { get; }

        public ICommand UtiliserResultatSelectionCommeOperandeCommand { get; }

        /// <summary>
        /// Création d'une nouvelle instance dans la méthode Add pour éviter l'ajout de la même référence à chaque fois.
        /// </summary>
        /// <param name="calcul"></param>
        public void EnregistrerCalcul(Calcul calcul)
        {
            if (calcul.Operateur != EnumOperateur.Aucun)
            {
                _Historique.Add(new OperationVM(calcul));
                DixDerniersCalculs.Add(new OperationVM(calcul));
            }
        }

        private IEnumerable<OperationVM> GetListeOperationVM()
        {
            // Pour chaque élément récupéré dans la liste de Calculs (objet BI), on crée une nouvelle OperationVM (objet VM) à partir de 
            // l'objet BI. Est-ce que la conversion de List vers IEnumerable est implicite ?
            return Persistance.RecupererHistorique().Select(c => new OperationVM(c));
            //return Persistance.RecupererHistorique().TakeLast(10).Select(c => new OperationVM(c));

            // Equivalent :
            //foreach (var calcul in Persistance.RecupererHistorique())
            //    yield return new OperationVM(calcul);
        }

        public bool CanFermerEtSauverHistorique()
        {
            return true;
        }

        public void FermerEtSauverHistorique()
        {
            if (_Historique != null && _Historique.Count != 0)
            {
                // A l'inverse de la récupération, pour chaque élément enregistré dans l'ObservableCollection, on va transformer l'OperationVM
                // en objet BI puis convertir l'ObservableCollection en List.
                Persistance.SauvegarderHistorique(Historique.Select(o => o.CalculModel).ToList());
            }
            else
            {
                MessageBox.Show("Aucune donnée à enregistrer");
            }
        }

        public bool CanUtiliserResultatSelectionCommeOperande(OperationVM CalculSelectionne)
        {
            return true;
        }

        public void UtiliserResultatSelectionCommeOperande(OperationVM CalculSelectionne)
        {
            if (_CalculatriceDc is not null)
            {
                if (_CalculatriceDc.IsBtEgalDejaClique)
                {
                    _CalculatriceDc.Reinitialiser();
                    _CalculatriceDc.OperandeUnVm = CalculSelectionne.Resultat.ToString();
                    _CalculatriceDc.AffichageEnCours = _CalculatriceDc.OperandeUnVm;
                }
                else
                {
                    if (_CalculatriceDc.OperateurVm == EnumOperateur.Aucun)
                    {
                        _CalculatriceDc.OperandeUnVm = CalculSelectionne.Resultat.ToString();
                        _CalculatriceDc.AffichageEnCours = _CalculatriceDc.OperandeUnVm;
                    }
                    else
                    {
                        _CalculatriceDc.OperandeDeuxVm = CalculSelectionne.Resultat.ToString();
                        _CalculatriceDc.AffichageEnCours = _CalculatriceDc.OperandeUnVm + (char)_CalculatriceDc.OperateurVm + _CalculatriceDc.OperandeDeuxVm;
                    }
                    IsOperandeFromHistorique = true;
                    _CalculatriceDc.ResultatVm = Models.Moteur.Calculatrice.Calculer(_CalculatriceDc.OperandeUnDouble, _CalculatriceDc.OperandeDeuxDouble, _CalculatriceDc.OperateurVm).ToString();
                    EnregistrerCalcul(_CalculatriceDc.Calcul);
                    _CalculatriceDc.IsBtEgalDejaClique = false;
                    _CalculatriceDc.AffichageFinal = _CalculatriceDc.ResultatVm;
                }
            }
        }


    }
}
