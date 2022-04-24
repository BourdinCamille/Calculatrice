using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public ObservableCollection<OperationVM> Historique
        {
            get => _Historique;
        }

        private OperationVM _SelectedCalcul;

        public OperationVM SelectedCalcul
        {
            get => _SelectedCalcul;
            set
            {
                _SelectedCalcul = value;
                OnPropertyChanged(nameof(SelectedCalcul));
            }
        }

        public ICommand FermerEtSauverHistoriqueCommand { get; }

        public ICommand RecupererSelectionCommand { get; }

        public HistoriqueDC()
        {
            _Historique = new ObservableCollection<OperationVM>(GetListeOperationVM());
            FermerEtSauverHistoriqueCommand = new NoParameterCommand(FermerEtSauverHistorique, CanFermerEtSauverHistorique);
            RecupererSelectionCommand = new CommandBase<OperationVM>(RecupererSelection, CanRecupererSelection);
        }

        /// <summary>
        /// Création d'une nouvelle instance dans la méthode Add pour éviter l'ajout de la même référence à chaque fois.
        /// </summary>
        /// <param name="calcul"></param>
        public void EnregistrerCalcul(Calcul calcul)
        {
            _Historique.Add(new OperationVM(calcul));
        }

        public IEnumerable<OperationVM> GetListeOperationVM()
        {
            // Pour chaque élément récupéré dans la liste de Calculs (objet BI), on crée une nouvelle OperationVM (objet VM) à partir de 
            // l'objet BI. Est-ce que la conversion de List vers IEnumerable est implicite ?
            return Persistance.RecupererHistorique().Select(c => new OperationVM(c));

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

        public bool CanRecupererSelection(OperationVM CalculSelectionne)
        {
            return true;
        }

        public void RecupererSelection(OperationVM CalculSelectionne)
        {
            MessageBox.Show(CalculSelectionne.Resultat.ToString());
        }


    }
}
