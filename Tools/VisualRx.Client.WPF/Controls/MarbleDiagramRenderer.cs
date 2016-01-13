using SharpGL;
using SharpGL.Enumerations;
using SharpGL.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VisualRx.Contracts;

namespace VisualRx.Client.WPF
{
    public class MarbleDiagramRenderer
    {
        public static IEnumerable<MarbleDiagram> GetRender(OpenGLControl obj)
        {
            return (IEnumerable<MarbleDiagram>)obj.GetValue(RenderProperty);
        }

        public static void SetRender(OpenGLControl obj, IEnumerable<MarbleDiagram> value)
        {
            obj.SetValue(RenderProperty, value);
        }

        public static readonly DependencyProperty RenderProperty =
            DependencyProperty.RegisterAttached("Render",
                typeof(IEnumerable<MarbleDiagram>),
                typeof(MarbleDiagramRenderer),
                new PropertyMetadata(OnRenderPropertyChanged));

        public static int GetScaleValue(DependencyObject obj)
        {
            return (int)obj.GetValue(ScaleValueProperty);
        }

        public static void SetScaleValue(DependencyObject obj, int value)
        {
            obj.SetValue(ScaleValueProperty, value);
        }

        public static readonly DependencyProperty ScaleValueProperty =
            DependencyProperty.RegisterAttached("ScaleValue",
                typeof(int),
                typeof(MarbleDiagramRenderer),
                new PropertyMetadata(1));

        public static ScaleType GetScaleType(DependencyObject obj)
        {
            return (ScaleType)obj.GetValue(ScaleTypeProperty);
        }

        public static void SetScaleType(DependencyObject obj, ScaleType value)
        {
            obj.SetValue(ScaleTypeProperty, value);
        }

        public static readonly DependencyProperty ScaleTypeProperty =
            DependencyProperty.RegisterAttached("ScaleType",
                typeof(ScaleType),
                typeof(MarbleDiagramRenderer),
                new PropertyMetadata(ScaleType.Seconds));

        private static void OnRenderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (OpenGLControl)d;
            control.OpenGLDraw += Sender_OpenGLDraw;
            control.Resized += Sender_Resized;
            InitView(control, control.OpenGL);
        }

        private static void Sender_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            var control = (OpenGLControl)sender;
            var gl = args.OpenGL;
            InitView(control, gl);
        }

        private static void InitView(OpenGLControl control, OpenGL gl)
        {
            gl.MatrixMode(MatrixMode.Projection);
            gl.LoadIdentity();
            gl.Ortho(0, control.ActualWidth, control.ActualHeight, 0, -10, 10);

            gl.MatrixMode(MatrixMode.Modelview);
        }

        private static void Sender_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            var control = (OpenGLControl)sender;
            var mousePoint = Mouse.GetPosition(control);
            var gl = args.OpenGL;

            gl.ClearColor(1f, 1f, 1f, 1f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.PointSize(2.0f);
            gl.Enable(OpenGL.GL_BLEND);

            var collection = GetRender(control);
            if (!collection.Any())
                return;

            double tpi = 2 * Math.PI;
            double t = Math.PI / 20;
            double r = 15;
            double ticksPerScale = 0;
            var refreshRate = GetScaleValue(control);
            var rateType = GetScaleType(control);
            switch (rateType)
            {
                case ScaleType.Milliseconds:
                    ticksPerScale = TimeSpan.FromMilliseconds(refreshRate).Ticks;
                    break;
                case ScaleType.Centiseconds:
                    ticksPerScale = TimeSpan.FromMilliseconds(refreshRate * 10).Ticks;
                    break;
                case ScaleType.Deciseconds:
                    ticksPerScale = TimeSpan.FromMilliseconds(refreshRate * 100).Ticks;
                    break;
                case ScaleType.Seconds:
                    ticksPerScale = TimeSpan.FromSeconds(refreshRate).Ticks;
                    break;
                case ScaleType.Minutes:
                    ticksPerScale = TimeSpan.FromMinutes(refreshRate).Ticks;
                    break;
                case ScaleType.Houers:
                    ticksPerScale = TimeSpan.FromHours(refreshRate).Ticks;
                    break;
                default:
                    break;
            }

            var props = typeof(Colors).GetProperties();
            int colorIndex = 0;
            var offsety = 20;

            foreach (var diagram in collection)
            {
                var color = (Color)props[colorIndex++].GetValue(null);
                gl.Color(color.ScR, color.ScG, color.ScB);

                if (offsety > control.ActualHeight)
                    break;

                foreach (var marble in diagram.Items)
                {
                    var offsetx = (double)marble.Offset.Ticks / ticksPerScale * r + r;
                    if (offsetx > control.ActualWidth)
                        break;

                    var mlx = Math.Abs(mousePoint.X - offsetx);
                    var mly = Math.Abs(mousePoint.Y - offsety);
                    if (mlx <= r && mly <= r)
                        r = 20;

                    gl.Begin(BeginMode.Polygon);
                    for (double i = 0; i < tpi; i += t)
                        gl.Vertex(offsetx + Math.Cos(i) * r, offsety + Math.Sin(i) * r);
                    gl.End();

                    r = 15;
                }

                offsety += 40;
            }
        }
    }
}
