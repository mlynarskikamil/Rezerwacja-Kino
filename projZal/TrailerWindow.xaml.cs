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
using System.Windows.Shapes;

namespace projZal
{
    /// <summary>
    /// Interaction logic for TrailerWindow.xaml
    /// </summary>
    public partial class TrailerWindow : Window
    {
        public TrailerWindow(string title, string url)
        {
            InitializeComponent();
            trailerWindow.Title = title;
            player.Address = url;
        }
    }
}
