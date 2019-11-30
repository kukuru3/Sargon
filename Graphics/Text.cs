using SFML.Graphics;
using System;
using System.Collections.Generic;
using Ur;
using Ur.Geometry;
using SFMLText = SFML.Graphics.Text;

namespace Sargon.Graphics {
    public class Text : IRenderable {

        public enum Anchors {
            TopLeft,
            TopCenter,
            TopRight,

            MidLeft,
            MidCenter,
            MidRight,

            BottomLeft,
            BottomCenter,
            BottomRight
        }

        #region Fields
        private float z = 0f;
        private Anchors anchor;
        private Vector2 scale;
        private Rect rect;
        private Assets.Font font;
        private int charsize;
        private bool isDirty;
        private string text;

        private List<SFMLText> lineObjects;
        #endregion

        #region Properties
        public bool Visible { get; set; } = true;
        public Ur.Color Color { get; set; }
        public Canvas OnCanvas { get; set; }
        public bool Additive { get; set; }
        public Effect Effect { get; set; }

        public float Zed { get { return z; } set { if (z.Approximately(value)) return; z = value; OnCanvas?.MarkMemberDepthAsDirty(); } }

        public Anchors Anchor {
            get => anchor;
            set { anchor = value; MarkDirty(); }
        }

        public Vector2 Scale {
            get => scale;
            set { scale = value; MarkDirty(); }
        }

        public int CharacterSize {
            get => charsize;
            set { charsize = value; MarkDirty(); }
        }

        public Assets.Font Font {
            get => font ?? Sargon.GameContext.Current.Assets.DefaultFont;
            set { font = value; MarkDirty(); }
        }

        public string Content {
            get => text;
            set { text = value; MarkDirty(); }
        }

        public Rect Rect {
            get => rect;
            set { rect = value; MarkDirty(); }
        }

        public Rect Bounds { get; private set; }
        #endregion

        #region C-tors
        private Text() {
            lineObjects = new List<SFMLText>();
            Color = Ur.Color.White;
            MarkDirty();
        }

        public Text(string text, Assets.Font font = null, int charSize = 0) : this() {
            Visible = true;
            this.text = text;
            this.font = font;
            this.charsize = charSize;
            if (this.charsize < 1) this.charsize = GameContext.Current.Assets.DefaultCharacterSize;
            if (this.font == null) this.font = GameContext.Current.Assets.DefaultFont;
            this.scale = Vector2.One;
            this.rect = Rect.FromBounds(0, 0, 500, 500);
        }
        #endregion

        #region Public stuff
        public void Display() {
            if (isDirty) {
                isDirty = false;
                RecalculateText();
            }
            var context = OnCanvas?.Pipeline.Game.Context;
            if (context == null) return;
            foreach (var sprite in this.lineObjects) {
                sprite.Color = this.Color.ToSFMLColor();
                context.Diagnostics.TextCharactersDrawn += sprite.DisplayedString.Length;
                sprite.Position = new SFML.System.Vector2f(
                    (float)Math.Round(sprite.Position.X),
                    (float)Math.Round(sprite.Position.Y)
                );

                context.Renderer.RenderText(this, sprite);
            }
        }

        public void Dispose() {
            foreach (var item in this.lineObjects) item.Dispose();
            lineObjects.Clear();
        }

        public Text Clone() {
            var t = new Text(this.text, this.font, this.charsize);
            t.scale = this.scale;
            t.rect = this.rect;
            t.anchor = this.anchor;
            t.z = z;
            t.Visible = Visible;
            t.Color = Color;
            t.Additive = Additive;
            return t;
        }
        #endregion

        #region Guts
        private void MarkDirty() {
            isDirty = true;
        }

        private void RecalculateText() {
            lineObjects.Clear();

            if (Font?.NativeFont == null) return;

            var paragraphs = text.Split(TextUtility.LineSeparators);
            var lines = new List<string>();

            foreach (var paragraph in paragraphs) {
                lines.AddRange(TextUtility.BreakText(paragraph, this.rect.W, this.Font, this.CharacterSize));
            }

            int index = -1;

            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;

            foreach (var l in lines) {
                index++;
                var q = l.Trim();
                if (q.Length > 0) {
                    var textLine = new SFMLText(q, Font.NativeFont);
                    textLine.CharacterSize = (uint)CharacterSize;
                    textLine.Scale = Scale.ToSFMLVector2f();
                    var rect = textLine.GetLocalBounds();

                    var rw = rect.Width * Scale.x;
                    var rh = rect.Height * Scale.y;

                    // align:
                    var xx = HorizontalAlign(textLine, rw, this.rect.W);
                    var yy = VerticalAlign(textLine, rh, this.rect.H, index, lines.Count);

                    // change position
                    textLine.Position = new SFML.System.Vector2f(this.rect.X0 + xx, this.rect.Y0 + yy);


                    // recalculate bounds:
                    minX = Numbers.Min(minX, xx);
                    minY = Numbers.Min(minY, yy);
                    if (xx + rw > maxX) maxX = xx + rw;
                    if (yy + rh > maxY) maxY = yy + rh;
                    this.Bounds = Rect.FromBounds(minX, minY, maxX, maxY);

                    // add the sprite:
                    lineObjects.Add(textLine);
                }
            }

        }

        private float HorizontalAlign(SFMLText textToAlign, float textWidth, float rectWidth) {
            var alignment = (int)Anchor % 3;
            switch (alignment) {
                case 0: return 0f;
                case 1: return 0.5f * (rectWidth - textWidth);
                case 2: return 1.0f * (rectWidth - textWidth);
            }
            return 0f;
        }

        private float VerticalAlign(SFMLText textToAlign, float textHeight, float rectHeight, int lineIndex, int lineTotal) {
            var ls = Font.NativeFont.GetLineSpacing((uint)CharacterSize) * scale.x;
            var totalTextHeight = ls * lineTotal;
            var factor = 0.5f * ((int)Anchor / 3);
            var y0 = (rectHeight - totalTextHeight) * factor;
            return y0 + lineIndex * ls;
        }
        #endregion


    }
}
