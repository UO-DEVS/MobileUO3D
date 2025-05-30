// SPDX-License-Identifier: BSD-2-Clause

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ClassicUO.Configuration;
using ClassicUO.Game.Data;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Assets;
using ClassicUO.Renderer;
using ClassicUO.Resources;
using Microsoft.Xna.Framework;

namespace ClassicUO.Game.UI.Gumps
{
    internal class BuffGump : Gump
    {
        private GumpPic _background;
        private Button _button;
        private GumpDirection _direction;
        private ushort _graphic;
        private DataBox _box;

        public BuffGump(World world) : base(world, 0, 0)
        {
            CanMove = true;
            CanCloseWithRightClick = true;
            AcceptMouseInput = true;
        }

        public BuffGump(World world, int x, int y) : this(world)
        {
            X = x;
            Y = y;

            _direction = GumpDirection.LEFT_HORIZONTAL;
            _graphic = 0x7580;

            SetInScreen();

            BuildGump();
        }

        public override GumpType GumpType => GumpType.Buff;

        private void BuildGump()
        {
            WantUpdateSize = true;

            _box?.Clear();
            _box?.Children.Clear();

            Clear();

            Add(_background = new GumpPic(0, 0, _graphic, 0) { LocalSerial = 1 });

            Add(
                _button = new Button(0, 0x7585, 0x7589, 0x7589)
                {
                    ButtonAction = ButtonAction.Activate
                }
            );

            switch (_direction)
            {
                case GumpDirection.LEFT_HORIZONTAL:
                    _button.X = -2;
                    _button.Y = 36;

                    break;

                case GumpDirection.RIGHT_VERTICAL:
                    _button.X = 34;
                    _button.Y = 78;

                    break;

                case GumpDirection.RIGHT_HORIZONTAL:
                    _button.X = 76;
                    _button.Y = 36;

                    break;

                case GumpDirection.LEFT_VERTICAL:
                default:
                    _button.X = 0;
                    _button.Y = 0;

                    break;
            }

            Add(_box = new DataBox(0, 0, 0, 0) { WantUpdateSize = true });

            if (World.Player != null)
            {
                foreach (KeyValuePair<BuffIconType, BuffIcon> k in World.Player.BuffIcons)
                {
                    _box.Add(new BuffControlEntry(World.Player.BuffIcons[k.Key]));
                }
            }

            _background.Graphic = _graphic;
            _background.X = 0;
            _background.Y = 0;

            UpdateElements();
        }

        public override void Save(XmlTextWriter writer)
        {
            base.Save(writer);
            writer.WriteAttributeString("graphic", _graphic.ToString());
            writer.WriteAttributeString("direction", ((int)_direction).ToString());
        }

        public override void Restore(XmlElement xml)
        {
            base.Restore(xml);

            _graphic = ushort.Parse(xml.GetAttribute("graphic"));
            _direction = (GumpDirection)byte.Parse(xml.GetAttribute("direction"));
            BuildGump();
        }

        protected override void UpdateContents()
        {
            BuildGump();
        }

        private void UpdateElements()
        {
            for (int i = 0, offset = 0; i < _box.Children.Count; i++, offset += 31)
            {
                Control e = _box.Children[i];

                switch (_direction)
                {
                    case GumpDirection.LEFT_VERTICAL:
                        e.X = 25;
                        e.Y = 26 + offset;

                        break;

                    case GumpDirection.LEFT_HORIZONTAL:
                        e.X = 26 + offset;
                        e.Y = 5;

                        break;

                    case GumpDirection.RIGHT_VERTICAL:
                        e.X = 5;
                        e.Y = _background.Height - 48 - offset;

                        break;

                    case GumpDirection.RIGHT_HORIZONTAL:
                        e.X = _background.Width - 48 - offset;
                        e.Y = 5;

                        break;
                }
            }
        }

        public override void OnButtonClick(int buttonID)
        {
            if (buttonID == 0)
            {
                _graphic++;

                if (_graphic > 0x7582)
                {
                    _graphic = 0x757F;
                }

                switch (_graphic)
                {
                    case 0x7580:
                        _direction = GumpDirection.LEFT_HORIZONTAL;

                        break;

                    case 0x7581:
                        _direction = GumpDirection.RIGHT_VERTICAL;

                        break;

                    case 0x7582:
                        _direction = GumpDirection.RIGHT_HORIZONTAL;

                        break;

                    case 0x757F:
                    default:
                        _direction = GumpDirection.LEFT_VERTICAL;

                        break;
                }

                RequestUpdateContents();
            }
        }

        private enum GumpDirection
        {
            LEFT_VERTICAL,
            LEFT_HORIZONTAL,
            RIGHT_VERTICAL,
            RIGHT_HORIZONTAL
        }

        private class BuffControlEntry : GumpPic
        {
            private byte _alpha;
            private bool _decreaseAlpha;
            private readonly RenderedText _gText;
            private float _updateTooltipTime;

            public BuffControlEntry(BuffIcon icon) : base(0, 0, icon.Graphic, 0)
            {
                if (IsDisposed)
                {
                    return;
                }

                Icon = icon;
                _alpha = 0xFF;
                _decreaseAlpha = true;

                _gText = RenderedText.Create(
                    "",
                    0xFFFF,
                    2,
                    true,
                    FontStyle.Fixed | FontStyle.BlackBorder,
                    TEXT_ALIGN_TYPE.TS_CENTER,
                    Width
                );

                AcceptMouseInput = true;
                WantUpdateSize = false;
                CanMove = true;

                SetTooltip(icon.Text);
            }

            public BuffIcon Icon { get; }

            public override void Update()
            {
                base.Update();

                if (!IsDisposed && Icon != null)
                {
                    int delta = (int)(Icon.Timer - Time.Ticks);

                    if (_updateTooltipTime < Time.Ticks && delta > 0)
                    {
                        TimeSpan span = TimeSpan.FromMilliseconds(delta);

                        SetTooltip(
                            string.Format(
                                ResGumps.TimeLeft,
                                Icon.Text,
                                span.Hours,
                                span.Minutes,
                                span.Seconds
                            )
                        );

                        _updateTooltipTime = (float)Time.Ticks + 1000;

                        if (span.Hours > 0)
                        {
                            _gText.Text = string.Format(ResGumps.Span0Hours, span.Hours);
                        }
                        else
                        {
                            _gText.Text =
                                span.Minutes > 0
                                    ? $"{span.Minutes}:{span.Seconds:00}"
                                    : $"{span.Seconds:00}s";
                        }
                    }

                    if (Icon.Timer != 0xFFFF_FFFF && delta < 10000)
                    {
                        if (delta <= 0)
                        {
                            ((BuffGump)Parent.Parent)?.RequestUpdateContents();
                        }
                        else
                        {
                            int alpha = _alpha;
                            int addVal = (10000 - delta) / 600;

                            if (_decreaseAlpha)
                            {
                                alpha -= addVal;

                                if (alpha <= 60)
                                {
                                    _decreaseAlpha = false;
                                    alpha = 60;
                                }
                            }
                            else
                            {
                                alpha += addVal;

                                if (alpha >= 255)
                                {
                                    _decreaseAlpha = true;
                                    alpha = 255;
                                }
                            }

                            _alpha = (byte)alpha;
                        }
                    }
                }
            }

            public override bool Draw(UltimaBatcher2D batcher, int x, int y)
            {
                Vector3 hueVector = ShaderHueTranslator.GetHueVector(0, false, _alpha / 255f, true);

                ref readonly var gumpInfo = ref Client.Game.UO.Gumps.GetGump(Graphic);

                if (gumpInfo.Texture != null)
                {
                    batcher.Draw(gumpInfo.Texture, new Vector2(x, y), gumpInfo.UV, hueVector);

                    if (
                        ProfileManager.CurrentProfile != null
                        && ProfileManager.CurrentProfile.BuffBarTime
                    )
                    {
                        _gText.Draw(batcher, x - 3, y + gumpInfo.UV.Height / 2 - 3, hueVector.Z);
                    }
                }

                return true;
            }

            public override void Dispose()
            {
                _gText?.Destroy();
                base.Dispose();
            }
        }
    }
}
