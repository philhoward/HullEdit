﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace HullEdit
{
    class ItemList : ObservableCollection<String>
    {
        public ItemList() : base()
        { }
    }
    /// <summary>
    /// Interaction logic for PanelsWindow.xaml
    /// </summary>
    public partial class PanelsLayoutWindow : Window
    {
        private Brush DEFAULT_BACKGROUND = Brushes.White;
        private Brush DEFAULT_FOREGROUND = Brushes.Black;
        private Brush SELECTED_FOREGROUND = Brushes.Red;
        private double SCALE_MOVE = 0.25;
        private double MIN_ROTATE_DRAG = 3;
        private double ROTATE_STEP = Math.PI / 180;
        private bool m_dragging;
        private bool m_rotating;

        private double m_scale;
        private double scale
        {
            get { return m_scale; }
            set
            {
                m_scale = value;
                ScaleTransform scaler = new ScaleTransform();
                scaler.ScaleX = scale;
                scaler.ScaleY = scale;
                canvas.LayoutTransform = scaler;
                //foreach (PanelDisplay panel in m_displayPanels)
                //{
                //    panel.scale = m_scale;
                //}
            }
        }

        private PanelDisplay m_selectedPanel;
        Panels m_panels;
        ObservableCollection<PanelDisplay> m_displayPanels = new ObservableCollection<PanelDisplay>();
        private PanelSetupWindow setupWindow;

        Point m_dragLoc;

        public PanelsLayoutWindow(Panels panels)
        {
            m_scale = 1;
            m_panels = panels;
            InitializeComponent();

            ItemList panelList = (ItemList)this.FindResource("PanelList");
            if (panelList != null)
            {
                for (int ii = 1; ii <= m_panels.panels.Count; ii++)
                {
                    panelList.Add("Panel " + ii);
                }
                for (int ii = 1; ii <= m_panels.bulkheads.Count; ii++)
                {
                    panelList.Add("Bulkhead " + ii);
                }
            }

            setupWindow = new PanelSetupWindow();
            setupWindow.ShowDialog();
        }

        public void DisplayPanel(Panel p, double x, double y)
        {
            PanelDisplay panel = new PanelDisplay(p, m_scale);
            panel.Background = DEFAULT_BACKGROUND;
            panel.Foreground = DEFAULT_FOREGROUND;

            panel.X = x;
            panel.Y = y;

            DisplayPanel(panel);
        }

        public void DisplayPanel(PanelDisplay panel)
        {
            panel.Background = DEFAULT_BACKGROUND;
            panel.Foreground = DEFAULT_FOREGROUND;

            panel.PreviewMouseDown += Panel_PreviewMouseDown;
            panel.PreviewMouseMove += Panel_PreviewMouseMove;

            ContextMenu menu = (ContextMenu)this.FindResource("EditMenu");
            if (menu != null)
            {
                panel.ContextMenu = menu;
            }

            m_displayPanels.Add(panel);
            canvas.Children.Add(panel);
            Canvas.SetLeft(panel, panel.X);
            Canvas.SetTop(panel, panel.Y);
        }

        private void Panel_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            m_dragging = false;
            m_rotating = false;
        }

        private void Panel_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point loc = e.GetPosition(canvas);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (m_selectedPanel != null && m_dragging)
                {
                    // handle dragging
                    m_selectedPanel.X += loc.X - m_dragLoc.X;
                    m_selectedPanel.Y += loc.Y - m_dragLoc.Y;
                    m_dragLoc = loc;
                    Canvas.SetLeft(m_selectedPanel, m_selectedPanel.X);
                    Canvas.SetTop(m_selectedPanel, m_selectedPanel.Y);
                    ResizeCanvas();

                }
                else if (m_selectedPanel != null && m_rotating)
                {
                    // Handle rotations
                    double distance = loc.X - m_dragLoc.X;
                    Debug.WriteLine("Rotate: {0}", distance);

                    if (Math.Abs(distance) > MIN_ROTATE_DRAG)
                    {
                        m_dragLoc = loc;

                        if (distance > 0)
                            m_selectedPanel.Rotate(ROTATE_STEP);

                        else
                            m_selectedPanel.Rotate(-ROTATE_STEP);

                    }
                    ResizeCanvas();

                }
            }
        }

        private void Panel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine(sender);

            // If the conversion fails, we didn't select a panel
            PanelDisplay selectedPanel = sender as PanelDisplay;

 //           if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (selectedPanel != null)
                {
                    // Unselect all panels
                    foreach (PanelDisplay panel in m_displayPanels)
                    {
                        panel.Foreground = DEFAULT_FOREGROUND;
                    }

                    m_selectedPanel = selectedPanel;
                    m_selectedPanel.Foreground = SELECTED_FOREGROUND;
                    m_dragLoc = e.GetPosition(canvas);
                }

                Debug.WriteLine("MouseDown 1 {0} {1}", m_dragging, m_rotating);
                if (m_selectedPanel != null && selectedPanel == m_selectedPanel)
                    m_dragging = true;
                else if (m_selectedPanel != null)
                    m_rotating = true;

                Debug.WriteLine("MouseDown 2 {0} {1}", m_dragging, m_rotating);
            }
        }

        private void openClick(object sender, RoutedEventArgs e)
        {
            canvas.Height = canvas.ActualHeight + 200;
            canvas.Width = canvas.ActualWidth + 200;
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {
            canvas.Height -= 200;
            canvas.Width -= 200;
        }

        private void ZoomClick(object sender, RoutedEventArgs e)
        {
            scale += 1;

            ResizeCanvas();
        }

        private void LayoutClick(object sender, RoutedEventArgs e)
        {
            setupWindow.ShowDialog();
            ResizeCanvas();
        }

        private void HorizontalFlipClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Horizontal flip: {0}", sender);

            if (m_selectedPanel != null)
            {
                m_selectedPanel.HorizontalFlip();
                Debug.WriteLine("Horizontal Flip");
                ResizeCanvas();
                //InvalidateVisual();
            }
        }
        private void VerticalFlipClick(object sender, RoutedEventArgs e)
        {
            if (m_selectedPanel != null)
            {
                m_selectedPanel.VerticalFlip();
                Debug.WriteLine("Vertical Flip");
                ResizeCanvas();
                //InvalidateVisual();
            }
        }

        private void CopyClick(object sender, RoutedEventArgs e)
        {
            if (m_selectedPanel != null)
            {
                PanelDisplay panel = m_selectedPanel.Copy();
                DisplayPanel(panel);
                m_selectedPanel.Foreground = DEFAULT_FOREGROUND;
                panel.Foreground = SELECTED_FOREGROUND;

                m_selectedPanel = panel;
            }
        }
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (m_selectedPanel != null)
            {
                m_displayPanels.Remove(m_selectedPanel);
                canvas.Children.Remove(m_selectedPanel);
                ResizeCanvas();
            }
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Add Click {0} {1}", sender, PanelSelection.SelectedValue);
            string selection = PanelSelection.SelectedValue as String;
            Point loc = Mouse.GetPosition(canvas);

            if (selection != null)
            {
                string[] words = selection.Split();
                if (words.Length == 2)
                {
                    int index = Int32.Parse(words[1]);
                    Panel panel = null;

                    // Figure out what panel was selected
                    if (words[0] == "Panel")
                    {
                        panel = m_panels.panels[index - 1];
                    }
                    else if (words[0] == "Bulkhead")
                    {
                        panel = m_panels.bulkheads[index - 1];
                    }

                    if (panel != null)
                    {
                        // Move over by half the size so new panel shows up centered on the mouse
                        Size size = panel.GetSize();
                        loc.X -= size.Width / 2;
                        loc.Y -= size.Height / 2;

                        DisplayPanel(panel, loc.X, loc.Y);
                        ResizeCanvas();
                    }
                }
            }
        }

        private void AddAllClick(object sender, RoutedEventArgs e)
        {
            double y = 10;
            foreach (Panel p in m_panels.panels)
            {
                DisplayPanel(p, 10, y);
                y += 15;
            }

            foreach (Panel p in m_panels.bulkheads)
            {
                DisplayPanel(p, 10, y);
                y += 15;
            }

            ResizeCanvas();
        }

        private void ResizeCanvas()
        {
            double maxX = viewerGrid.ActualWidth/scale;
            double maxY = viewerGrid.ActualHeight/scale;

            //maxX = viewer.ActualWidth/scale;
            //maxY = viewer.ActualHeight/scale;

            foreach (PanelDisplay panel in m_displayPanels)
            {
                Size size = panel.size;
                maxX = Math.Max(maxX, size.Width + panel.X);
                maxY = Math.Max(maxY, size.Height + panel.Y);
            }

            Debug.WriteLine("Resize: Prev: {0} Viewer: {1} New: {2}", canvas.ActualHeight, viewerGrid.ActualHeight, maxY);

            canvas.Width = maxX;
            canvas.Height = maxY;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResizeCanvas();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeCanvas();
        }

        private void viewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta > 0)
                    scale += scale * SCALE_MOVE;
                else if (e.Delta < 0)
                    scale -= scale * SCALE_MOVE;
                Debug.WriteLine("Zoom {0} {1}", e.Delta, scale);
                ResizeCanvas();
                e.Handled = true;
            }
            //base.MouseWheel(sender, e);
        }
    }
}
