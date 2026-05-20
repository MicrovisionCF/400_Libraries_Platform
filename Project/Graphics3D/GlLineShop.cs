using Microvision.Geometry;
using Microvision.Graphic;
using Microvision.OpenGL;

namespace Microvision.Graphics3D
{
    public sealed class GlLineShop
    {
        // ***************************************************************************************************
        // 24.04.19 : Création, utile au cas où on veuille tracer plein de lignes facilement
        // 21.11.19 : (libs 2.2) NotInheritable
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        private GlLineShop()
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static void DrawLine(OpenGLContext gl, Point3D startPoint, Point3D endPoint, HColor color, float width)
        {
            gl.LineWidth(width);
            gl.Color(color.Red / 255f, color.Green / 255f, color.Blue / 255f);

            gl.Begin(BeginMode.Lines);

            gl.Vertex(startPoint.X, startPoint.Y, startPoint.Z);
            gl.Vertex(endPoint.X, endPoint.Y, endPoint.Z);

            gl.End();

            gl.Color(1, 1, 1);
            gl.LineWidth(1);
        }

        public static void DrawLines(OpenGLContext gl, IEnumerable<Tuple<Point3D, Point3D>> lines, HColor color, float width)
        {
            gl.LineWidth(width);
            gl.Color(color.Red / 255f, color.Green / 255f, color.Blue / 255f);

            gl.Begin(BeginMode.Lines);

            foreach (Tuple<Point3D, Point3D> line in lines)
            {
                gl.Vertex(line.Item1.X, line.Item1.Y, line.Item1.Z);
                gl.Vertex(line.Item2.X, line.Item2.Y, line.Item2.Z);
            }

            gl.End();

            gl.Color(1, 1, 1);
            gl.LineWidth(1);
        }

        public static void DrawPolyline(OpenGLContext gl, IEnumerable<Point3D> points, HColor color, float width, bool closed)
        {
            gl.LineWidth(width);
            gl.Color(color.Red / 255f, color.Green / 255f, color.Blue / 255f);

            if (closed)
                gl.Begin(BeginMode.LineLoop);
            else
                gl.Begin(BeginMode.LineStrip);

            foreach (Point3D p in points)
                gl.Vertex(p.X, p.Y, p.Z);

            gl.End();

            gl.Color(1, 1, 1);
            gl.LineWidth(1);
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------


        // ----------------------------------------
        // Privées
        // ----------------------------------------


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}