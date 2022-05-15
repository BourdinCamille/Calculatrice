using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Calculatrice.Models;
using Calculatrice.ViewModels;
using Grpc.Net.Client;
using GrpcServer;
using GrpcServer.BI;
using GrpcServer.Protos;
using EnumOperateurClient = Calculatrice.Models.EnumOperateur;

namespace Calculatrice.GrpcClient
{
    public class HistoriqueClient
    {
        private readonly GrpcChannel _Channel;

        private Historique.HistoriqueClient _Client;

        public HistoriqueClient()
        {
            _Channel = GrpcChannel.ForAddress("https://localhost:7021");
            _Client = new Historique.HistoriqueClient(_Channel);
        }

        public CalculsClient CalculsClient { get; } = new CalculsClient();

        public bool HasValeursIdentiques(Calcul calcul1, Calcul calcul2)
        {
            var reply = _Client.ComparerValeursOperations(new ComparaisonRequest
            {
                Operation1 = CalculsClient.ConvertToDto(calcul1),
                Operation2 = CalculsClient.ConvertToDto(calcul2)
            });
            return reply.EstIdentique;
        }

        public void EnregistrerCalculSurServeur(Calcul calcul)
        {
            _Client.EnregistrerCalcul(new EnregistrerCalculRequest
            {
                CalculAEnregistrer = CalculsClient.ConvertToDto(calcul)
            });
        }

        public List<Calcul> RecupererHistoriqueDepuisServeur()
        {
            var reply = _Client.GetHistorique(new GetHistoriqueRequest());

            static Calcul ConvertirFromDtoToBi(CalculAvecResultatRequest dto)
            {
                return new Calcul
                {
                    OperandeUn = dto.Operande1,
                    Operateur = ConversionOperateurFromDtoToBi(dto),
                    OperandeDeux = dto.Operande2,
                    Resultat = dto.Resultat
                };
            }

            var historique = reply.OperationsRecuperees.Select(c => ConvertirFromDtoToBi(c)).ToList();

            return historique;
        }

        private static EnumOperateurClient ConversionOperateurFromDtoToBi(CalculAvecResultatRequest calculRequest)
        {
            EnumOperateurClient operateurBi;

            switch (calculRequest.Operateur)
            {
                case (GrpcServer.EnumOperateur)1:
                    operateurBi = EnumOperateurClient.Addition;
                    break;
                case (GrpcServer.EnumOperateur)2:
                    operateurBi = EnumOperateurClient.Soustraction;
                    break;
                case (GrpcServer.EnumOperateur)3:
                    operateurBi = EnumOperateurClient.Multiplication;
                    break;
                case (GrpcServer.EnumOperateur)4:
                    operateurBi = EnumOperateurClient.Division;
                    break;
                default:
                    operateurBi = EnumOperateurClient.Aucun;
                    break;
            }
            return operateurBi;
        }
    }
}
