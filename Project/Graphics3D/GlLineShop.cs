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
            gl.Color(color.red / 255f, color.green / 255f, color.blue / 255f);

            gl.Begin(BeginMode.Lines);

            gl.Vertex(startPoint.x, startPoint.y, startPoint.z);
            gl.Vertex(endPoint.x, endPoint.y, endPoint.z);

            gl.End();

            gl.Color(1, 1, 1);
            gl.LineWidth(1);
        }

        public static void DrawLines(OpenGLContext gl, IEnumerable<Tuple<Point3D, Point3D>> lines, HColor color, float width)
        {
            gl.LineWidth(width);
            gl.Color(color.red / 255f, color.green / 255f, color.blue / 255f);

            gl.Begin(BeginMode.Lines);

            foreach (Tuple<Point3D, Point3D> line in lines)
            {
                gl.Vertex(line.Item1.x, line.Item1.y, line.Item1.z);
                gl.Vertex(line.Item2.x, line.Item2.y, line.Item2.z);
            }

            gl.End();

            gl.Color(1, 1, 1);
            gl.LineWidth(1);
        }

        public static void DrawPolyline(OpenGLContext gl, IEnumerable<Point3D> points, HColor color, float width, bool closed)
        {
            gl.LineWidth(width);
            gl.Color(color.red / 255f, color.green / 255f, color.blue / 255f);

            if (closed)
                gl.Begin(BeginMode.LineLoop);
            else
                gl.Begin(BeginMode.LineStrip);

            foreach (Point3D p in points)
                gl.Vertex(p.x, p.y, p.z);

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