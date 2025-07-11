﻿// SPDX-License-Identifier: BSD-2-Clause

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.CompilerServices;
// MobileUO: added using
using ClassicUO.Configuration;
using ClassicUO.Input;

namespace ClassicUO.Renderer
{
    public class Camera
    {
        private const float MAX_PEEK_DISTANCE = 250f;
        private const float MIN_PEEK_SPEED = 0.01f;
        private const float PEEK_TIME_FACTOR = 5;

        private Matrix _transform = Matrix.Identity;
        private Matrix _inverseTransform = Matrix.Identity;
        private bool _updateMatrixes = true;
        private float _lerpZoom;
        private float _zoom;
        private Vector2 _lerpOffset;


        public Camera(float minZoomValue = 1f, float maxZoomValue = 1f, float zoomStep = 0.1f)
        {
            ZoomMin = minZoomValue;
            ZoomMax = maxZoomValue;
            ZoomStep = zoomStep;
            Zoom = _lerpZoom = 1f;
        }


        public Rectangle Bounds;

        public Matrix ViewTransformMatrix
        {
            get
            {
                UpdateMatrices();

                return _transform;
            }
        }

	    //ADDED DX4D
	    public void SetZoom(float minZoomValue = 1f, float maxZoomValue = 1f, float zoomStep = 0.1f)
	    {
            ZoomMin = minZoomValue;
            ZoomMax = maxZoomValue;
            ZoomStep = zoomStep;
            Zoom = _lerpZoom = 1f;
	    }
	    //END ADDED
	    
        public float ZoomStep { get; private set; }
        public float ZoomMin { get; private set; }
        public float ZoomMax { get; private set; }
        public float Zoom
        {
            get => _zoom;
            set
            {
                _zoom = MathHelper.Clamp(value, ZoomMin, ZoomMax);
                _updateMatrixes = true;
            }
        }



        public void ZoomIn() => Zoom -= ZoomStep;

        public void ZoomOut() => Zoom += ZoomStep;

        public Viewport GetViewport() => new Viewport(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);

        public Vector2 Offset => _lerpOffset;

        public bool PeekingToMouse;

        public bool PeekBackwards;

        private float _timeDelta = 0;
        private Point _mousePos;

        public void Update(bool force, float timeDelta, Point mousePos)
        {
            if (force)
            {
                _updateMatrixes = true;
            }

            _timeDelta= timeDelta;
            _mousePos = mousePos;

            UpdateMatrices();
        }

        public Point ScreenToWorld(Point point)
        {
            UpdateMatrices();

            Transform(ref point, ref _inverseTransform, out point);

            return point;
        }

        public Point WorldToScreen(Point point)
        {
            UpdateMatrices();

            //Transform(ref point, ref _transform, out point);

            // MobileUO: use old version of logic here to fix rendering
            // MobileUO: TODO: use new version
            point.X += Bounds.X;
			point.Y += Bounds.Y;

			point.X = (int) (point.X / Zoom);
			point.Y = (int) (point.Y / Zoom);

			point.X -= (int) (Bounds.X / Zoom);
			point.Y -= (int) (Bounds.Y / Zoom);

			point.X += Bounds.X;
			point.Y += Bounds.Y;

            return point;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Transform(ref Point position, ref Matrix matrix, out Point result)
        {
            float x = position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41;
            float y = position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42;
            result.X = (int) x;
            result.Y = (int) y;
        }

        public Point MouseToWorldPosition()
        {
            Point mouse = _mousePos;

            //mouse.X -= Bounds.X;
            //mouse.Y -= Bounds.Y;

            // MobileUO: use old version of logic here to fix mouse
            // MobileUO: TODO: use new version
			mouse.X = (int) ((Mouse.Position.X - (ProfileManager.CurrentProfile.GameWindowPosition.X + 5)) * Zoom);
			mouse.Y = (int) ((Mouse.Position.Y - (ProfileManager.CurrentProfile.GameWindowPosition.Y + 5)) * Zoom);
			return mouse;

            //return ScreenToWorld(mouse);
        }

        private void UpdateMatrices()
        {
            if (!_updateMatrixes)
            {
                return;
            }

            Matrix temp;

            var origin = new Vector2(Bounds.Width * 0.5f, Bounds.Height * 0.5f);

            Matrix.CreateTranslation(-origin.X, -origin.Y, 0f, out _transform);

            CalculateLerpZoom();

            Matrix.CreateScale(_lerpZoom, _lerpZoom, 1f, out temp);
            Matrix.Multiply(ref _transform, ref temp, out _transform);

            CalculatePeek(origin);

            Matrix.CreateTranslation(origin.X - _lerpOffset.X, origin.Y - _lerpOffset.Y, 0f, out temp);
            Matrix.Multiply(ref _transform, ref temp, out _transform);


            Matrix.Invert(ref _transform, out _inverseTransform);

            _updateMatrixes = false;
        }

        private void CalculateLerpZoom()
        {
            float zoom = 1f / Zoom;

            _lerpZoom = zoom;
        }

        private void CalculatePeek(Vector2 origin)
        {
            Vector2 target_offset = new Vector2();

            if (PeekingToMouse)
            {
                Vector2 target = new Vector2(_mousePos.X - Bounds.X, _mousePos.Y - Bounds.Y);

                if (PeekBackwards)
                {
                    target.X = 2 * origin.X - target.X;
                    target.Y = 2 * origin.Y - target.Y;
                }

                target_offset = target - origin;
                float length = target_offset.Length();

                if (length > 0)
                {
                    float length_factor = Math.Min(length / (Bounds.Height >> 1), 1f);
                    target_offset = Vector2.Normalize(target_offset) * Utility.Easings.OutQuad(length_factor) * MAX_PEEK_DISTANCE / Zoom;
                }
            }

            float dist = Vector2.Distance(target_offset, _lerpOffset);

            if (dist > 1f)
            {
                float time = Math.Max(Utility.Easings.OutQuart(dist / MAX_PEEK_DISTANCE) * _timeDelta * PEEK_TIME_FACTOR, MIN_PEEK_SPEED);
                _lerpOffset = Vector2.Lerp(_lerpOffset, target_offset, time);
            }
            else
            {
                _lerpOffset = target_offset;
            }
        }
    }
}