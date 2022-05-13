using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Calculatrice.Commands;
using Calculatrice.GrpcClient;
using Calculatrice.Models;
using Calculatrice.Views;
using GrpcServer.BI;

namespace Calculatrice.ViewModels
{
    public class HistoriqueDC : ViewModelBase
    {
        private ObservableCollection<OperationVM> _Historique; // probablement à supprimer car plus utilisé

        private ObservableCollection<OperationVM> _HistoriquePourLaVue;

        private OperationVM _SelectedCalcul;

        private CalculatriceDc _CalculatriceDc;

        private HistoriqueClient _HistoriqueClient;

        public HistoriqueDC(CalculatriceDc calculatriceDc)
        {
            _CalculatriceDc = calculatriceDc;
            _HistoriqueClient = new HistoriqueClient();
            // N'appeler GetListeOperationVM qu'une seule fois pour ne pas surcharger le serveur inutilement.
            _Historique = new ObservableCollection<OperationVM>(GetListeOperationVM()); // mettre dans une List<> (pas besoin d'une ObsCol puisque pas dans la vue !)
            _HistoriquePourLaVue = new ObservableCollection<OperationVM>(GetListeOperationVM().TakeLast(10));
            UtiliserResultatSelectionCommeOperandeCommand = new CommandBase<OperationVM>(UtiliserResultatSelectionCommeOperande, CanUtiliserResultatSelectionCommeOperande);
        }
        public ObservableCollection<OperationVM> Historique
        {
            get => _Historique;
        }

        public ObservableCollection<OperationVM> HistoriquePourLaVue
        {
            get => _HistoriquePourLaVue;
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

        public HistoriqueClient HistoriqueClient
        {
            get => _HistoriqueClient;
        }

        public ICommand UtiliserResultatSelectionCommeOperandeCommand { get; }

        private IEnumerable<OperationVM> GetListeOperationVM()
        {
            // Pour chaque élément récupéré dans la liste de Calculs (objet BI), on crée une nouvelle OperationVM (objet VM) à partir de 
            // l'objet BI. Est-ce que la conversion de List vers IEnumerable est implicite ?
            return _HistoriqueClient.RecupererHistoriqueDepuisServeur().Select(c => new OperationVM(c));
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
                    _CalculatriceDc.IsOperandeFromHistorique = true;

                    var resultat = _CalculatriceDc.CalculsClient.EnvoyerCalculAuServeur(_CalculatriceDc.Calcul);
                    _CalculatriceDc.ResultatVm = resultat.ToString(CultureInfo.CurrentCulture);

                    _CalculatriceDc.EnregistrerCalculEtMettreVueAJour(this, _CalculatriceDc.Calcul); 
                    _CalculatriceDc.IsBtEgalDejaClique = false;
                    _CalculatriceDc.AffichageFinal = _CalculatriceDc.ResultatVm;
                }
            }
        }

        /// <summary>
        /// Cette méthode sert uniquement à mettre la vue à jour, car chaque calcul est automatiquement envoyé au serveur et persisté mais la mise à jour de l'historique ne se fait pas en temps réel - l'historique étant récupéré uniquement au moment où l'on relance l'application.
        /// </summary>
        /// <param name="calcul"></param>
        public void MettreVueAJour(Calcul calcul)
        {
            if (HistoriquePourLaVue.Count == 10)
            {
                HistoriquePourLaVue.RemoveAt(0);
            }
            HistoriquePourLaVue.Add(new OperationVM(calcul));
        }
    }
}
