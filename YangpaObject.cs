using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace YangpaH
{
    class YangpaObject : Image
    {
        public delegate void SimpleDelegate(YangpaObject ypo, int z);
        //send YType directly to array index(0,1,2,3)
        public enum YType
        {
            General = 0,
            Red,
            Rotten,
            Pig,
            Half = 99
        };

        public YType Type { get; set; }
        public Point ActualPoint { get { return _actualpoint; } }
        private  Point _actualpoint;
        
        private static SimpleDelegate delCreate;
        private static SimpleDelegate delCollide;
        private static Rectangle[] jorects;
        private static Rect homerect;
        private static Window window;
        private static bool isEnvset = false;

        public static void SetEnvironment(Canvas _homerect, Window _window, SimpleDelegate _deli, SimpleDelegate _delc, Rectangle[] _rects)
        {
            delCreate = _deli;
            delCollide = _delc;
            window = _window;
            jorects = _rects;
            isEnvset = true;
            Point homep = GetAbsolutePath(_homerect);
//            Point jop = GetAbsolutePath(_jorect, _window);

            homerect = new Rect(homep, new Size(_homerect.ActualWidth, _homerect.ActualHeight));
//            jorect = new Rect(jop, new Size(_jorect.ActualWidth, _jorect.ActualHeight));
        }

        private static Point GetAbsolutePath(UIElement element)
        {
            return element.TransformToAncestor(window).Transform(new Point(0, 0));
        }

        public YangpaObject(YType type, Point point)
        {
            if (!isEnvset)
                throw new Exception("Yangpa Environment is not set.");
            this.Height = 80;
            this.Width = 80;

            this.MouseLeftButtonDown += new MouseButtonEventHandler(element_PreviewMouseDown);
            this.MouseMove += new MouseEventHandler(element_PreviewMouseMove);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(element_PreviewMouseUp);
            this.Type = type;
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;
            //this.Margin = new Thickness(point.X, point.Y,0,0);
            Canvas.SetLeft(this, point.X);
            Canvas.SetTop(this, point.Y);
            this.Visibility = System.Windows.Visibility.Visible;

            _actualpoint = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            LoadImage();
        }

        private void LoadImage()
        {
            string yt = null;
            switch (this.Type)
            {
                case YType.General:
                    yt = "yangpa_general.png";
                    break;
                case YType.Red:
                    yt = "yangpa_red.png";
                    break;
                case YType.Pig:
                    yt = "yangpa_pig.png";
                    break;
                case YType.Rotten:
                    yt = "yangpa_rotten.png";
                    break;
                case YType.Half:
                    yt = "yangpa_half.png";
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
            string uristring = "pack://application:,,/" + YangpaConstants.AssemblyName + ";component/Images/" + yt;
            this.Source = BitmapFromUri(uristring);

        }
        private ImageSource BitmapFromUri(string source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(source);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        public int CheckifinJorect()
        {
            for (int i = 0; i < jorects.Length; i++)
            {
                Rect rect = new Rect(GetAbsolutePath(jorects[i]), new Size(jorects[i].ActualWidth, jorects[i].ActualHeight));
                if (rect.Contains(GetAbsolutePath(this)))
                {
                    return i;
                }
            }
            return -1;
        }
        public bool CheckifinHomerect()
        {
            return homerect.Contains(GetAbsolutePath(this));
        }

        #region draggable func
        // Determine if we're presently dragging
        private bool _isDragging = false;
        // The offset from the top, left of the item being dragged 
        // and the original mouse down
        private Point _offset;

        // This is triggered when the mouse button is pressed 
        // on the element being hooked
        private void element_PreviewMouseDown(object sender,
                System.Windows.Input.MouseButtonEventArgs e)
        {
            Mouse.Capture(this);

            int z = Canvas.GetZIndex(this) + 1;
            if(CheckifinHomerect())
                delCreate.Invoke(new YangpaObject(this.Type, this.ActualPoint), z);
        
            // Ensure it's a framework element as we'll need to 
            // get access to the visual tree
            FrameworkElement element = sender as FrameworkElement;
            if (element == null) return;

            // start dragging and get the offset of the mouse 
            // relative to the element
            _isDragging = true;
            _offset = e.GetPosition(element);

        }

        // This is triggered when the mouse is moved over the element
        private void element_PreviewMouseMove(object sender,
                  MouseEventArgs e)
        {
            // If we're not dragging, don't bother - also validate the element
            if (!_isDragging) return;

            FrameworkElement element = sender as FrameworkElement;
            if (element == null) return;

            Canvas canvas = element.Parent as Canvas;
            if (canvas == null) return;

            // Get the position of the mouse relative to the canvas
            Point mousePoint = e.GetPosition(canvas);

            // Offset the mouse position by the original offset position
            mousePoint.Offset(-_offset.X, -_offset.Y);

            // Move the element on the canvas
            element.SetValue(Canvas.LeftProperty, mousePoint.X);
            element.SetValue(Canvas.TopProperty, mousePoint.Y);
        }

        // this is triggered when the mouse is released
        private void element_PreviewMouseUp(object sender,
                MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            if (_isDragging)
            {
                if (CheckifinHomerect())
                    this.Source = null;

                int c = CheckifinJorect();
                //if (c != -1)
                    delCollide.Invoke(this, c);
            }

            _isDragging = false;
        }
        #endregion
    }
}
