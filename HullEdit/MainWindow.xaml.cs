﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace HullEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Hull myHull;
        private double m_xAngle, m_yAngle, m_zAngle;

        public MainWindow()
        {
            InitializeComponent();
            myHull = new Hull();

            FrontDisplay.SetHull(myHull);
            TopDisplay.SetHull(myHull);
            SideDisplay.SetHull(myHull);
            PerspectiveDisplay.SetHull(myHull);

            myHull.PropertyChanged += hull_PropertyChanged;
        }

        private void openClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Hull files (*.hul)|*.hul|All files (*.*)|*.*";
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                if (myHull == null) myHull = new Hull();
                string result = myHull.LoadFromHullFile(openFileDialog.FileName);

                if (result != "")
                    Console.WriteLine(result);
                else
                {
                    //m_hullEditor = new HullEditor(myHull, 0, 0, 0, Perspective);

                    m_xAngle = 10;
                    m_yAngle = 30;
                    m_zAngle = 190;

                    UpdateDrawings();

                    PanelsMenu.IsEnabled = true;
                }
            }

        }

        private void saveClick(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateDrawings()
        {
            if (myHull != null && myHull.IsValid)
            {
                FrontDisplay.RotateTo(0, 0, 180);
                FrontDisplay.Scale();
                FrontDisplay.Draw();

                SideDisplay.RotateTo(0, 90, 180);
                SideDisplay.Scale();
                SideDisplay.Draw();

                TopDisplay.RotateTo(0, 90, 90);
                TopDisplay.Scale();
                TopDisplay.Draw();

                PerspectiveDisplay.RotateTo(m_xAngle, m_yAngle, m_zAngle);
                PerspectiveDisplay.Scale();
                PerspectiveDisplay.Draw();
            }
        }

        private void HullMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PerspectiveDisplay.IsEditable = false;

            Debug.WriteLine("HullMouseDown");

            if (sender == FrontDisplay)
            {
                Debug.WriteLine("Front");

                m_xAngle = 0;
                m_yAngle = 180;
                m_zAngle = 180;
            }
            else if (sender == TopDisplay)
            {
                Debug.WriteLine("Top");

                m_xAngle = 0;
                m_yAngle = 90;
                m_zAngle = 90;
            }
            else if (sender == SideDisplay)
            {
                Debug.WriteLine("Side");

                m_xAngle = 0;
                m_yAngle = 90;
                m_zAngle = 180;
            }

            PerspectiveDisplay.RotateTo(m_xAngle, m_yAngle, m_zAngle);
            PerspectiveDisplay.Scale();
            PerspectiveDisplay.IsEditable = true;
            PerspectiveDisplay.Draw();
        }

        private void RotateClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            if ((string)button.Content == "+X")
                m_xAngle += 5;
            else if ((string)button.Content == "-X")
                m_xAngle -= 5;
            else if ((string)button.Content == "+Y")
                m_yAngle += 5;
            else if ((string)button.Content == "-Y")
                m_yAngle -= 5;
            else if ((string)button.Content == "+Z")
                m_zAngle += 5;
            else if ((string)button.Content == "-Z")
                m_zAngle -= 5;

            PerspectiveDisplay.RotateTo(m_xAngle, m_yAngle, m_zAngle);
            PerspectiveDisplay.Scale();

            PerspectiveDisplay.IsEditable = false;

            PerspectiveDisplay.Draw();
        }

        private void YClick(object sender, RoutedEventArgs e)
        {
            m_yAngle += 5;
            PerspectiveDisplay.RotateTo(m_xAngle, m_yAngle, m_zAngle);
            PerspectiveDisplay.Scale();

            PerspectiveDisplay.IsEditable = false;

            PerspectiveDisplay.Draw();
        }

        private void PanelsClick(object sender, RoutedEventArgs e)
        {
            Panels p = new Panels(myHull);
            PanelsLayoutWindow layout = new PanelsLayoutWindow();
            layout.SetPanels(p);
            layout.Show();
        }

        private void ZClick(object sender, RoutedEventArgs e)
        {
            m_zAngle += 5;
            PerspectiveDisplay.RotateTo(m_xAngle, m_yAngle, m_zAngle);
            PerspectiveDisplay.Scale();

            PerspectiveDisplay.IsEditable = false;

            PerspectiveDisplay.Draw();
        }

        void hull_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateDrawings();
        }
    }
}
