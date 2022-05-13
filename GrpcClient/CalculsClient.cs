using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Calculatrice.Models;
using Grpc.Net.Client;
using GrpcServer;
using EnumOperateurClient = Calculatrice.Models.EnumOperateur;
using EnumOperateurProto = GrpcServer.EnumOperateur;

namespace Calculatrice.GrpcClient
{
    public class CalculsClient
    {
        private readonly GrpcChannel _Channel;

        private Calculs.CalculsClient _Client;

        public CalculsClient()
        {
            _Channel = GrpcChannel.ForAddress("https://localhost:7021");
            _Client = new Calculs.CalculsClient(_Channel);
        }

        public double EnvoyerCalculAuServeur(Calcul calcul)
        {
            if (!(calcul.Operateur == EnumOperateurClient.Division && calcul.OperandeDeux == 0))
            {
                var reply = _Client.Calculer(ConvertToDto(calcul));
                return reply.Resultat;
            }
            else
            {
                MessageBox.Show("Division par zéro impossible");
                return (double)calcul.OperandeUn;
            }
        }

        public static CalculRequest ConvertToDto(Calcul calcul)
        {
            var request = new CalculRequest
            {
                Operande1 = (double)calcul.OperandeUn,
                Operateur = ConversionOperateurFromBiToProto(calcul),
                Operande2 = (double)calcul.OperandeDeux
            };
            return request;
        }

        public static EnumOperateurProto ConversionOperateurFromBiToProto(Calcul calcul)
        {
            EnumOperateurProto operateurRequest;

            switch (calcul.Operateur)
            {
                case EnumOperateurClient.Addition:
                    operateurRequest = (EnumOperateurProto)1;
                    break;
                case EnumOperateurClient.Soustraction:
                    operateurRequest = (EnumOperateurProto)2;
                    break;
                case EnumOperateurClient.Multiplication:
                    operateurRequest = (EnumOperateurProto)3;
                    break;
                case EnumOperateurClient.Division:
                    operateurRequest = (EnumOperateurProto)4;
                    break;
                default:
                    operateurRequest = (EnumOperateurProto)0;
                    break;
            }
            return operateurRequest;
        }

    }
}
