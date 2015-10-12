using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MobaLib;

namespace MobaTest
{
    public struct Line
    {
        public Vector2 Start;
        public Vector2 End;
        public Color Color;
    }

    public class DrawHelper
    {
        BasicEffect be;
        GraphicsDevice device;
        List<Line> _lineQueue;
        List<MobaLib.Rectangle> _rectQueue;

        public DrawHelper(GraphicsDevice device)
        {
            this.device = device;
            be = new BasicEffect(device);
            be.VertexColorEnabled = true;
            be.TextureEnabled = false;
            be.World = Matrix.Identity;
            be.Projection = Matrix.CreateTranslation(-device.Viewport.Width / 2 - 0.5f, -device.Viewport.Height / 2 - 0.5f, 0) * Matrix.CreateScale(2 / ((float)device.Viewport.Width), -2 / ((float)device.Viewport.Height), 1);
            _lineQueue = new List<Line>();
            _rectQueue = new List<MobaLib.Rectangle>();
        }

        public void DrawPolys(List<Polygon> polygons, Matrix View, Color color)
        {
            DrawPolys(polygons.ToArray(), View, color);
        }

        public void DrawPolys(Polygon[] polygons, Matrix View, Color color)
        {
            int len = 0;
            for (int x = 0; x < polygons.Length; x++)
            {
                len += polygons[x].Edges.Length * 2;
            }
            be.View = View;
            be.CurrentTechnique.Passes[0].Apply();
            VertexPositionColor[] vertices = new VertexPositionColor[len];
            int count = 0;
            for (int x = 0; x < polygons.Length; x++)
            {
                for (int y = 0; y < polygons[x].Edges.Length; y++)
                {
                    vertices[count] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(polygons[x].Edges[y].Start.X, polygons[x].Edges[y].Start.Z, 0), color);
                    vertices[count + 1] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(polygons[x].Edges[y].End.X, polygons[x].Edges[y].End.Z, 0), color);
                    count += 2;
                }
            }
            if (len < 2)
                return;
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, len / 2, VertexPositionColor.VertexDeclaration);
        }

        public void DrawLines(Vector2[] positions, ManualCamera2D cam, Color color)
        {
            DrawLines(positions, cam, color, positions.Length);
        }

        public void DrawLines(Vector2[] positions, ManualCamera2D cam, Color color, int count)
        {
            count = count < positions.Length ? count : positions.Length;
            if (count <= 0)
                return;
            be.View = cam.Transformation;
            be.CurrentTechnique.Passes[0].Apply();
            VertexPositionColor[] vertices = new VertexPositionColor[2 * count];
            for (int x = 0; x < count; x++)
            {
                vertices[2 * x] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(positions[x], 0), color);
                vertices[2 * x + 1] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(positions[(x + 1) % positions.Length], 0), color);
            }
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, count, VertexPositionColor.VertexDeclaration);
        }

        public void DrawRectangles(List<MobaLib.Rectangle> rects, ManualCamera2D cam, Color color)
        {
            int count = rects.Count;
            if (count == 0)
                return;
            be.View = cam.Transformation;
            be.CurrentTechnique.Passes[0].Apply();
            VertexPositionColor[] vertices = new VertexPositionColor[12 * count];
            for (int x = 0; x < count; x++)
            {
                Microsoft.Xna.Framework.Vector3 topLeft = new Microsoft.Xna.Framework.Vector3(rects[x].Left, rects[x].Top, 0);
                Microsoft.Xna.Framework.Vector3 topRight = new Microsoft.Xna.Framework.Vector3(rects[x].Right, rects[x].Top, 0);
                Microsoft.Xna.Framework.Vector3 botLeft = new Microsoft.Xna.Framework.Vector3(rects[x].Left, rects[x].Bottom, 0);
                Microsoft.Xna.Framework.Vector3 botRight = new Microsoft.Xna.Framework.Vector3(rects[x].Right, rects[x].Bottom, 0);
                vertices[12 * x] = new VertexPositionColor(topLeft, color);
                vertices[12 * x + 1] = new VertexPositionColor(topRight, color);
                vertices[12 * x + 2] = new VertexPositionColor(topRight, color);
                vertices[12 * x + 3] = new VertexPositionColor(botRight, color);
                vertices[12 * x + 4] = new VertexPositionColor(botRight, color);
                vertices[12 * x + 5] = new VertexPositionColor(botLeft, color);
                vertices[12 * x + 6] = new VertexPositionColor(botLeft, color);
                vertices[12 * x + 7] = new VertexPositionColor(topLeft, color);
                vertices[12 * x + 8] = new VertexPositionColor(botLeft, color);
                vertices[12 * x + 9] = new VertexPositionColor(topRight, color);
                vertices[12 * x + 10] = new VertexPositionColor(topLeft, color);
                vertices[12 * x + 11] = new VertexPositionColor(botRight, color);
            }
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 6 * count, VertexPositionColor.VertexDeclaration);
        }

        public void QueueLines(Vector2[] positions, Color color)
        {
            for (int x = 0; x < positions.Length - 1; x++)
            {
                Line newLine = new Line();
                newLine.Start = positions[x];
                newLine.End = positions[x + 1];
                newLine.Color = color;
                _lineQueue.Add(newLine);
            }
        }

        public void QueueRectangles(List<MobaLib.Rectangle> rects)
        {
            _rectQueue.AddRange(rects);
        }

        public void FlushRects(ManualCamera2D cam)
        {
            DrawRectangles(_rectQueue, cam, Color.White);
            _rectQueue.Clear();
        }

        public void FlushLines(ManualCamera2D cam)
        {
            int count = _lineQueue.Count;
            if (count <= 0)
                return;
            be.View = cam.Transformation;
            be.CurrentTechnique.Passes[0].Apply();
            VertexPositionColor[] vertices = new VertexPositionColor[2 * count];
            for (int x = 0; x < count; x++)
            {
                vertices[2 * x] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(_lineQueue[x].Start, 0), _lineQueue[x].Color);
                vertices[2 * x + 1] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(_lineQueue[x].End, 0), _lineQueue[x].Color);
            }
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, count, VertexPositionColor.VertexDeclaration);
            _lineQueue.Clear();
        }
    }
}
