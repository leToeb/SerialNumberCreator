﻿using System;
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

namespace SerialNumberCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Regestrierung beim Eventhandler im ViewModel
            ((MainWindowViewModel)DataContext).MissingData += (sender, eventArge) => ShowError("Please enter value.");
            ((MainWindowViewModel)DataContext).LoadingError += (sender, eventArge) => ShowError("Loading fails.");
            ((MainWindowViewModel)DataContext).SavingError += (sender, eventArge) => ShowError("Saving fails.");
        }

        public void ShowError(string text)
        {
            MessageBox.Show(text);
        }


    }
}
