using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MobaTest
{
    public class ManualCamera2D
    {
        public Vector2 Location
        {
            get { return _location; }
            set
            {
                transformIsDirty = true;
                _location = value;
            }
        }

        static Random random = new Random();

        public Rectangle Bounds;
        public bool UseBounds;
        public float Zoom = 1f;

        public Vector2 targetLocation = Vector2.Zero;


        private int width;
        private int height;
        private Vector2 screenHalf;
        private Vector2 _location;
        private Matrix transformation;
        private Rectangle viewFrustrum;
        private Rectangle expandedViewFrustrum;
        private bool transformIsDirty = false;

        private float screenShakeStrength = 0;
        private float screenShakeTimeout = -1;

        private readonly GraphicsDevice graphicsDevice;

        private float _brightness = 1;

        public ManualCamera2D(int width, int height, GraphicsDevice graphicsDevice)
        {
            this.width = width;
            this.height = height;
            this.graphicsDevice = graphicsDevice;

            this.screenHalf = new Vector2(width / 2f, height / 2f);
            this.transformIsDirty = true;
        }

        public Vector2 Center
        {
            get { return _location + (screenHalf / Zoom); }
        }

        public Matrix Transformation
        {
            get
            {
                InvalidateCamera();
                return transformation;
            }
        }

        public Rectangle ViewFrustrum
        {
            get
            {
                InvalidateCamera();
                return viewFrustrum;
            }
        }

        public Rectangle ExpandedViewFrustrum
        {
            get
            {
                InvalidateCamera();
                return expandedViewFrustrum;
            }
        }

        public void Update(float dt)
        {
            if (screenShakeTimeout > 0)
            {
                screenShakeTimeout -= dt;
                if (screenShakeTimeout <= 0)
                    ScreenShakeOff();
            }
        }

        public void SetBrightness(float brightness)
        {
            _brightness = brightness;
        }

        public void MoveBy(Vector2 amount)
        {
            MoveTo(_location + amount);
        }

        public void MoveBy(float x, float y)
        {
            MoveTo(_location + new Vector2(x, y));
        }

        public void ScreenShakeOn(float strength, float timeout = 0)
        {
            screenShakeTimeout = timeout;
            screenShakeStrength = strength;
        }

        public void ScreenShakeOff()
        {
            screenShakeTimeout = -1;
            screenShakeStrength = 0;
        }

        public void MoveTo(Vector2 loc)
        {
            targetLocation = loc;
            transformIsDirty = true;
        }

        public void CenterOnPoint(Vector2 loc)
        {
            CenterOnPoint(loc.X, loc.Y);
        }

        public void CenterHard(Vector2 loc)
        {
            CenterHard(loc.X, loc.Y);
        }

        public void SetZoom(float zoom)
        {
            Vector2 center = Center;
            Zoom = zoom;
            CenterHard(center);
        }

        public void CenterOnPoint(float x, float y)
        {
            float newx = x - (screenHalf.X / this.Zoom);
            float newy = y - (screenHalf.Y / this.Zoom);

            if (UseBounds)
            {
                if (newx < 0) newx = 0;
                if (newy < 0) newy = 0;
                if (newx + width > Bounds.Right) newx = Bounds.Right - width;
                if (newy + height > Bounds.Bottom) newy = Bounds.Bottom - height;
            }

            targetLocation = new Vector2(newx, newy);
            transformIsDirty = true;
        }

        public void CenterHard(float x, float y)
        {
            float newx = x - (screenHalf.X / this.Zoom);
            float newy = y - (screenHalf.Y / this.Zoom);

            if (UseBounds)
            {
                if (newx < 0) newx = 0;
                if (newy < 0) newy = 0;
                if (newx + width > Bounds.Right) newx = Bounds.Right - width;
                if (newy + height > Bounds.Bottom) newy = Bounds.Bottom - height;
            }

            _location = new Vector2(newx, newy);
            targetLocation = new Vector2(newx, newy);
            transformIsDirty = true;
        }

        public Vector2 ScreenToWorld(Vector2 screenPos)
        {
            return new Vector2(
                (screenPos.X / Zoom) + _location.X,
                (screenPos.Y / Zoom) + _location.Y);
        }

        public Vector2 WorldToScreen(Vector2 worldPos)
        {
            return new Vector2(
                (worldPos.X - _location.X) * Zoom,
                (worldPos.Y - _location.Y) * Zoom);
        }

        public float GetYOrder(float y)
        {
            float depth = y;
            depth -= this._location.Y;
            depth /= (this.height / Zoom);
            return depth;
        }

        private void InvalidateCamera()
        {
            if (transformIsDirty)
            {
                generateTransformation();
                updateViewFrustrum();
            }
        }

        private void generateTransformation()
        {
            transformation =
                Matrix.CreateTranslation(new Vector3((-_location.X + (float)random.NextDouble() * 2 * screenShakeStrength - screenShakeStrength), (-_location.Y + (float)random.NextDouble() * 2 * screenShakeStrength - screenShakeStrength), 0)) *
                Matrix.CreateScale(new Vector3(this.Zoom, this.Zoom, 0));
        }

        private void updateViewFrustrum()
        {
            // Calculate the view frustrum
            var worldAtZero = ScreenToWorld(Vector2.Zero);
            var worldAtView = ScreenToWorld(new Vector2(
                graphicsDevice.Viewport.Width,
                graphicsDevice.Viewport.Height));

            viewFrustrum = new Rectangle(
                (int)Math.Floor(worldAtZero.X),
                (int)Math.Floor(worldAtZero.Y),
                (int)Math.Ceiling(worldAtView.X - worldAtZero.X),
                (int)Math.Ceiling(worldAtView.Y - worldAtZero.Y));

            viewFrustrum.Inflate(100, 25);

            expandedViewFrustrum = viewFrustrum;
            expandedViewFrustrum.X -= expandedViewFrustrum.Width;
            expandedViewFrustrum.Y -= expandedViewFrustrum.Height;
            expandedViewFrustrum.Width *= 3;
            expandedViewFrustrum.Height *= 3;
        }

        public float Brightness
        {
            get { return _brightness; }
        }
    }
}
