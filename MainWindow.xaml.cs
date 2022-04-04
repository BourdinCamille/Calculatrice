using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculatrice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string operandeEnCours = ""; 
        public float operandeStocke;
        public char operateur = ' ';
        
        public float resultat { get; private set; }

        public bool IsAEteCalcule;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btChiffre_Click(object sender, RoutedEventArgs e)
        {
            string bouton = ((Button)sender).Name;

            if (IsAEteCalcule)
            {
                operandeEnCours = "";
                tbOperation.Clear();
            }

            switch (bouton)
            {
                case "bt0":
                    if (!(operandeEnCours.StartsWith("-")))
                    {
                        operandeEnCours += "0";
                        tbOperation.Text = operandeEnCours;
                        IsAEteCalcule = false;
                    }
                    break;
                case "bt1":
                    operandeEnCours += "1";
                    tbOperation.Text = operandeEnCours;
                    IsAEteCalcule = false;
                    break;
                case "bt2":
                    operandeEnCours += "2";
                    tbOperation.Text = operandeEnCours;
                    IsAEteCalcule = false;
                    break;
                case "bt3":
                    operandeEnCours += "3";
                    tbOperation.Text = operandeEnCours;
                    IsAEteCalcule = false;
                    break;
                case "bt4":
                    operandeEnCours += "4";
                    tbOperation.Text = operandeEnCours;
                    IsAEteCalcule = false;
                    break;
                case "bt5":
                    operandeEnCours += "5";
                    tbOperation.Text = operandeEnCours;
                    IsAEteCalcule = false;
                    break;
                case "bt6":
                    operandeEnCours += "6";
                    tbOperation.Text = operandeEnCours;
                    IsAEteCalcule = false;
                    break;
                case "bt7":
                    operandeEnCours += "7";
                    tbOperation.Text = operandeEnCours;
                    IsAEteCalcule = false;
                    break;
                case "bt8":
                    operandeEnCours += "8";
                    tbOperation.Text = operandeEnCours;
                    IsAEteCalcule = false;
                    break;
                case "bt9":
                    operandeEnCours += "9";
                    tbOperation.Text = operandeEnCours;
                    IsAEteCalcule = false;
                    break;
                case "btDecimal":
                    if (!operandeEnCours.Contains(",") && operandeEnCours != "")
                    {
                        operandeEnCours += ",";
                        tbOperation.Text = operandeEnCours;
                    }
                    break;
                case "btPositifOuNegatif":
                    if (operandeEnCours == "")
                    {
                        operandeEnCours += "-";
                        tbOperation.Text = operandeEnCours;
                    }
                    break;
            }
        }

        private void btAdditionner_Click(object sender, RoutedEventArgs e)
        {
            DefinirOperateur('+');
            IsAEteCalcule = false;
        }

        private void btSoustraire_Click(object sender, RoutedEventArgs e)
        {
            DefinirOperateur('-');
            IsAEteCalcule = false;
        }

        private void btMultiplier_Click(object sender, RoutedEventArgs e)
        {
            DefinirOperateur('*');
            IsAEteCalcule = false;
        }

        private void btDiviser_Click(object sender, RoutedEventArgs e)
        {
            DefinirOperateur('/');
            IsAEteCalcule = false;
        }

        private void btCalculerResultat_Click(object sender, RoutedEventArgs e)
        {
            Calculer();
        }

        private void btEffacerDerniereSaisie_Click(object sender, RoutedEventArgs e)
        {
            //string dernierChar = tbOperation.Text;

            if (operandeEnCours != "")
            {
                operandeEnCours = operandeEnCours.Remove(operandeEnCours.Length - 1);
                tbOperation.Text = operandeEnCours;
            }
            /*else if (operandeEnCours == "" &&
                     dernierChar.EndsWith('+') ||
                     dernierChar.EndsWith('-') ||
                     dernierChar.EndsWith('*') ||
                     dernierChar.EndsWith('/'))
            {
                operateur = ' ';
                dernierChar = dernierChar.Remove(dernierChar.Length-1);
                Console.WriteLine(dernierChar);
                tbOperation.Text = dernierChar;
            }*/
        }

        private void btToutEffacer_Click(object sender, RoutedEventArgs e)
        {
            operandeStocke = 0.0f;
            operateur = ' ';
            operandeEnCours = "";
            resultat = 0.0f;
            tbOperation.Clear();
            tbResultat.Clear();
        }

        private void DefinirOperateur(char operateurSelectionne)
        {
            if (operandeEnCours != "" && operandeEnCours.Substring(operandeEnCours.Length - 1) != ",")
            {
                // Cas de la première opération
                if (operateur == ' ')
                {
                    operandeStocke = float.Parse(operandeEnCours);
                    operandeEnCours = "";
                    operateur = operateurSelectionne;
                    tbOperation.Text = operandeStocke.ToString() + operateur;
                }
                // Cas où on enchaîne les clics sur la bouton =
                else if (IsAEteCalcule)
                {
                    operandeEnCours = "";
                    operateur = operateurSelectionne;
                    tbOperation.Text = operateur.ToString();
                }
            }
        }

        private void Calculer()
        {
            float resultat;

            if (operandeEnCours != "" && operateur != ' ')
            {
                try
                {
                    switch (operateur)
                    {
                        case '+':
                            resultat = operandeStocke + float.Parse(operandeEnCours);
                            operandeStocke = resultat;
                            tbResultat.Text = operandeStocke.ToString();
                            break;
                        case '-':
                            resultat = operandeStocke - float.Parse(operandeEnCours);
                            operandeStocke = resultat;
                            tbResultat.Text = operandeStocke.ToString();
                            break;
                        case '*':
                            resultat = operandeStocke * float.Parse(operandeEnCours);
                            operandeStocke = resultat;
                            tbResultat.Text = operandeStocke.ToString();
                            break;
                        case '/':
                            if (float.Parse(operandeEnCours) != 0)
                            {
                                resultat = operandeStocke / float.Parse(operandeEnCours);
                                operandeStocke = resultat;
                                tbResultat.Text = operandeStocke.ToString();
                            }
                            else
                            {
                                MessageBox.Show("Impossible de diviser par zéro");
                                operandeEnCours = operandeEnCours.Remove(operandeEnCours.Length - 1);
                                tbOperation.Text = operandeEnCours;
                            }
                            break;
                    }
                }
                catch (OverflowException exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
            IsAEteCalcule = true;
        }

    }
}

