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
using System.Windows.Shapes;
using Calculatrice.ViewModels;

namespace Calculatrice.Views
{
    /// <summary>
    /// Logique d'interaction pour CalculatriceView.xaml
    /// </summary>
    public partial class CalculatriceView : Window
    {
        public CalculatriceView()
        {
            InitializeComponent();
            DataContext = new CalculatriceDc();
        }
    }
}
