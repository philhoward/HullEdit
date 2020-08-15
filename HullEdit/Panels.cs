﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace HullEdit
{
    public class Panels
    {
        private const int POINTS_PER_CHINE = 50;
        private List<Panel> m_panels;
        private List<Panel> m_bulkheads;

        public List<Panel> panels { get { return m_panels; } }
        public List<Panel> bulkheads {  get { return m_bulkheads; } }

        public Panels(Hull hull)
        {
            int numPanels = hull.numChines - 1;

            m_panels = new List<Panel>();

            for (int ii = 0; ii < numPanels; ii++)
            {
                Panel panel = new Panel(hull.GetChine(ii), hull.GetChine(ii + 1));
                panel.scale = 3;
                m_panels.Add(panel);
            }

            //*********************************
            // bulkheads:
            int numBulkheads = hull.numBulkheads;

            if (hull.GetBulkhead(numBulkheads - 1).type == Bulkhead.BulkheadType.BOW) numBulkheads--;

            m_bulkheads = new List<Panel>();
            for (int bulkhead=0; bulkhead<hull.numBulkheads; bulkhead++)
            {
                int numChines = hull.numChines;
                Point3D point;

                //if (hull.GetBulkhead(bulkhead).type != Bulkhead.BulkheadType.BOW)
                //{
                //    Point3DCollection points = hull.GetBulkhead(bulkhead);

                //    // Add reflection
                //    for (int chine = numChines - 1; chine >= 0; chine--)
                //    {
                //        point = new Point3D();
                //        point = hull.GetBulkheadPoint(bulkhead, chine);
                //        point.X = -points[chine].X;
                //        points.Add(point);
                //    }

                //    // close the shape
                //    if (points[0].X != 0) points.Add(points[0]);

                //    m_bulkheads.Add(new Panel(points));
                //}
            }


        }
    }
}
