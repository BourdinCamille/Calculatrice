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
            var reply = _Client.Calculer(ConvertToDto(calcul));
            return reply.Resultat;
        }

        // Cette méthode étant référencée dans au moins une autre classe, il aurait mieux valu créer une classe Conversion dans le repértoire Helpers et la placer dedans.
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

        private static EnumOperateurProto ConversionOperateurFromBiToProto(Calcul calcul)
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
