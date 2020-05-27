using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Device.Location;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapNET_Example
{
    public partial class Form1 : Form
    {

        private int _xPos;
        private int _yPos;
        private bool _dragging = true;
        List<PictureBox> pictureBoxes = new List<PictureBox>();
        Pen drawingPen = new Pen(Color.Black);

        public Form1()
        {
            InitializeComponent();
            for (int i=5;i<=19;i++)
            {
                comboBoxZoom.Items.Add(i.ToString());
            }
            comboBoxZoom.SelectedIndex = 5;

            this.WindowState = FormWindowState.Maximized;
            buttonLoadMap_Click(null,null);

            radioButtonLine.Checked = true;
            //comboBox1.Items.AddRange(new string[] { Pens.Black.ToString(), Pens.Blue.ToString(), Pens.Brown.ToString(), Pens.Red.ToString() });
            numericUpDown1.Value = 50;
            colorDialog1.Color = Color.Black;
            textBoxcolor.BackColor = colorDialog1.Color;

        }
        string m_baseDir = @"D:\osmMap\";
        private void buttonLoadMap_Click(object sender, EventArgs e)
        {
            gMapControl1.DragButton = MouseButtons.Left;
            
            gMapControl1.CanDragMap = true;
            //gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            gMapControl1.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            if (!Directory.Exists(m_baseDir  + "GMapCache"))
            {
                Directory.CreateDirectory(m_baseDir + "GMapCache");
            }
            gMapControl1.CacheLocation = Path.Combine(m_baseDir + "GMapCache");
            double lat = Convert.ToDouble( textBoxLat.Text);
            double lon = Convert.ToDouble(textBoxLong.Text);
            int zoom = Convert.ToInt32(comboBoxZoom.SelectedItem);

            //gMapControl1.SetPositionByKeywords("Maputo, Mozambique");
            gMapControl1.Position = new GMap.NET.PointLatLng(lat,lon);

            gMapControl1.MaxZoom = 19;
            gMapControl1.MinZoom = 5;
            gMapControl1.Zoom = zoom;
            //create overlay
            GMapOverlay gMapOverlay = new GMapOverlay("markers");
            gMapControl1.Overlays.Add(gMapOverlay);

            GMapOverlay gMapOverlayDraw = new GMapOverlay("drawings");

            gMapControl1.Overlays.Add(gMapOverlayDraw);


            if (line_overlay == null)
            {
                line_overlay = new GMapOverlay("line_overlay");
                line_layer = new GMapRoute("single_line");
                line_layer.Stroke = new Pen(Brushes.Black, 2); //width and color of line
            }

            gMapControl1.Overlays.Add(line_overlay);

            gMapControl1.HelperLineOption = new HelperLineOptions();
            //gMapControl1.CanDragMap = false;

            AddPin(lat,lon,"a1");       
       
        }


        public enum OVERLAYS
        {
            MARKERS,
            DRAWING,
            LINE
        }
        private void button1_Click(object sender, EventArgs e)
        {
           
            RectLatLng area = gMapControl1.SelectedArea;
            if (!area.IsEmpty)
            {
                for (int i = (int)gMapControl1.Zoom; i <= gMapControl1.MaxZoom; i++)
                {
                    DialogResult res = MessageBox.Show("Ready ripp at Zoom = " + i + " ?", "GMap.NET", MessageBoxButtons.YesNoCancel);

                    if (res == DialogResult.Yes)
                    {
                        bool mode = gMapControl1.Manager.Mode != AccessMode.CacheOnly;
                        using (TilePrefetcher obj = new TilePrefetcher())
                        {                   

                            obj.Shuffle = gMapControl1.Manager.Mode != AccessMode.CacheOnly;

                            obj.Owner = this;
                            obj.ShowCompleteMessage = true;
                            obj.Start(area, i, gMapControl1.MapProvider, gMapControl1.Manager.Mode == AccessMode.CacheOnly ? 0 : 100, gMapControl1.Manager.Mode == AccessMode.CacheOnly ? 0 : 1);
                        }
                    }
                    else if (res == DialogResult.No)
                    {
                        continue;
                    }
                    else if (res == DialogResult.Cancel)
                    {
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Select map area holding ALT", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void gMapControl1_OnMarkerEnter(GMapMarker item)
        {
            
        }

        private void gMapControl1_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag != null)
            {
                if (item.Tag.ToString() == "a1")
                {
                    int x = e.X;
                    int y = e.Y;
                    //MessageBox.Show("marker clicked" + x + "," + y);
                    //gMapControl1.Overlays[1].Markers[0].Position = new PointLatLng(31.790370+0.2, 74.598749+0.2);
                    gMapControl1.Overlays[(int)OVERLAYS.MARKERS].Markers[0].LocalPosition= new Point(x+1, y+1);
                    //item.LocalPosition = new Point(x+5,y+5);
                    //gMapControl1.Refresh();
                    gMapControl1.Update();

                }
            }
        }

        private void AddPin(double lat, double lang, string tag)
        {
            PointLatLng pointLatLng = new PointLatLng(lat, lang);
            GMapMarker gMapMarker = new GMarkerGoogle(pointLatLng, GMarkerGoogleType.red_pushpin);
            GMapMarkerPlane gMapMarkerPlane = new GMapMarkerPlane(pointLatLng,0);
            gMapMarker.Tag = tag;            
            gMapControl1.Overlays[0].Markers.Add(gMapMarker);         
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string ImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\Icons";

                if (listView1.SelectedItems[0].ImageKey == "planetracker.png")
                {
                    ImagePath = Path.Combine(ImagePath, "planetracker.png");
                }

                if (listView1.SelectedItems[0].ImageKey == "priceTag.png")
                {

                    ImagePath = Path.Combine(ImagePath, "priceTag.png");
                }

                if (listView1.SelectedItems[0].ImageKey == "shopping.png")
                {

                    ImagePath = Path.Combine(ImagePath, "shopping.png");
                }
                PictureBox picture = new PictureBox
                {
                    Name = "pictureBox",
                    Size = new Size(55, 55),
                    Location = new Point(300, 300),
                    Image = Image.FromFile(ImagePath),

                };
                this.Controls.Add(picture);

                picture.MouseDown += Picture_MouseDown;
                picture.MouseUp += Picture_MouseUp;
                picture.MouseMove += Picture_MouseMove;
                pictureBoxes.Add(picture);

                picture.BringToFront();
            }
            else
            {
                MessageBox.Show("Please Select Image from list");
            }

             
        }

        private void Picture_MouseMove(object sender, MouseEventArgs e)
        {
            var c = sender as PictureBox;
            if (!_dragging || null == c) return;
            c.Top = e.Y + c.Top - _yPos;
            c.Left = e.X + c.Left - _xPos;
        }

        private void Picture_MouseUp(object sender, MouseEventArgs e)
        {
            var c = sender as PictureBox;
            if (null == c) return;
            _dragging = false;
        }

        private void Picture_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _dragging = true;
            _xPos = e.X;
            _yPos = e.Y;
        }

        private void CreateCircle(Double lat, Double lon, double radius)
        {
            PointLatLng point = new PointLatLng(lat, lon);
            int segments = 1000;

            List<PointLatLng> gpollist = new List<PointLatLng>();

            for (int i = 0; i < segments; i++)
                gpollist.Add(FindPointAtDistanceFrom(point, i, radius / 1000));

            GMapPolygon gpol = new GMapPolygon(gpollist, "pol");
            GMapOverlay gMapOverlay = new GMapOverlay("markers");
            //add marker
            //gMapOverlay.Markers.Add(gMapMarker1);
            //gMapOverlay.Markers.Add(gMapMarkerPlane);
            gMapOverlay.Polygons.Add(gpol);
             

            gMapControl1.Overlays.Add(gMapOverlay);

            
        }

        public static GMap.NET.PointLatLng FindPointAtDistanceFrom(GMap.NET.PointLatLng startPoint, double initialBearingRadians, double distanceKilometres)
        {
            const double radiusEarthKilometres = 6371.01;
            var distRatio = distanceKilometres / radiusEarthKilometres;
            var distRatioSine = Math.Sin(distRatio);
            var distRatioCosine = Math.Cos(distRatio);

            var startLatRad = DegreesToRadians(startPoint.Lat);
            var startLonRad = DegreesToRadians(startPoint.Lng);

            var startLatCos = Math.Cos(startLatRad);
            var startLatSin = Math.Sin(startLatRad);

            var endLatRads = Math.Asin((startLatSin * distRatioCosine) + (startLatCos * distRatioSine * Math.Cos(initialBearingRadians)));

            var endLonRads = startLonRad + Math.Atan2(
                          Math.Sin(initialBearingRadians) * distRatioSine * startLatCos,
                          distRatioCosine - startLatSin * Math.Sin(endLatRads));

            return new GMap.NET.PointLatLng(RadiansToDegrees(endLatRads), RadiansToDegrees(endLonRads));
        }

        public static double DegreesToRadians(double degrees)
        {
            const double degToRadFactor = Math.PI / 180;
            return degrees * degToRadFactor;
        }

        public static double RadiansToDegrees(double radians)
        {
            const double radToDegFactor = 180 / Math.PI;
            return radians * radToDegFactor;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double lat = Convert.ToDouble(textBoxLat.Text);
            double lon = Convert.ToDouble(textBoxLong.Text);
            int zoom = Convert.ToInt32(comboBoxZoom.SelectedItem);
            AddPin(lat, lon, "a1");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            gMapControl1.Overlays[0].Markers.Clear();
            gMapControl1.Refresh();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            double lat = Convert.ToDouble(textBoxLat.Text);
            double lon = Convert.ToDouble(textBoxLong.Text);
            if (radioButtonLine.Checked)
            {
                gMapControl1.Overlays[1].Markers.Add(new GMapPoint(new PointLatLng(lat, lon),Convert.ToInt32( numericUpDown1.Value), shapes.line, this.drawingPen));
            }
            if (radioButtonCircle.Checked)
            {
                gMapControl1.Overlays[1].Markers.Add(new GMapPoint(new PointLatLng(lat, lon), Convert.ToInt32(numericUpDown1.Value), shapes.circle, this.drawingPen));
            }
            if (radioButtonRectangle.Checked)
            {
                gMapControl1.Overlays[1].Markers.Add(new GMapPoint(new PointLatLng(lat, lon), Convert.ToInt32(numericUpDown1.Value), shapes.rectangle, this.drawingPen));
            }
            gMapControl1.Refresh();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
          colorDialog1.AllowFullOpen = false;
            // Allows the user to get help. (The default is false.)
            colorDialog1.ShowHelp = true;
            // Sets the initial color select to the current text color.
           

            // Update the text box color if the user clicks OK 
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxcolor.BackColor = colorDialog1.Color;
                drawingPen = new Pen(colorDialog1.Color);
            }
        }

        GMapRoute line_layer;
        GMapOverlay line_overlay;

        List<double> m_latList = new List<double>();
        List<double> m_lonList = new List<double>();
        private void gMapControl1_MouseMove(object sender, MouseEventArgs e)
        {
             

            if (m_mouseDown && radioButtonLine.Checked)
            {
                double X = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lng;
                double Y = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lat;


                m_latList.Add(Y);
                m_lonList.Add(X);

                line_layer.Points.Add(gMapControl1.FromLocalToLatLng(e.X, e.Y));

                line_overlay.Routes.Add(line_layer);
                
                gMapControl1.Overlays[(int)OVERLAYS.LINE] = line_overlay;

                //To force the draw, you need to update the route
                gMapControl1.UpdateRouteLocalPosition(line_layer);

                 
            }
        }

        bool m_mouseDown = false;
        private void gMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            m_mouseDown = true;
            gMapControl1.CanDragMap = rbDrag.Checked;
        }

        private void gMapControl1_MouseUp(object sender, MouseEventArgs e)
        {
            m_mouseDown = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            gMapControl1.Overlays[(int)OVERLAYS.LINE].Clear();
            line_overlay.Routes.Clear();
            line_layer.Clear();
            m_latList.Clear();
            m_lonList.Clear();
            gMapControl1.Refresh();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            GeoCoordinate sc = null;
            GeoCoordinate ec = null;
            double distanceInMeters = 0;

            for (int i = 0; i < m_latList.Count - 1; i += 1)
            {
                sc = new GeoCoordinate(m_latList[i], m_lonList[i]);
                ec = new GeoCoordinate(m_latList[i + 1], m_lonList[i + 1]);
                distanceInMeters += sc.GetDistanceTo(ec);
            }

            MessageBox.Show("Line distance :" + distanceInMeters / 1000.0 + " Km");
            m_latList.Clear();
            m_lonList.Clear();
        }
    }
}
