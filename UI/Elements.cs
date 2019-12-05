
using Sargon.Graphics;

namespace Sargon.UI {
    public class Group : UIElement {
        public override string Print => "Group";
        protected override void CreateGraphics() { }
        protected override void CleanupGraphics() { }

        internal override void Update() { }
    }

    public class Panel : UIElement {
        public Ur.Color BackgroundColor { get; set; }
        public override string Print => $"Panel {LocalRect.W}x{LocalRect.H}";

        public Panel() {
            BackgroundColor = Ur.Color.White;
        }

        Sprite background;
        protected override void CreateGraphics() { 
            background = new Sprite();
            background.Source = "white";
            UI.Canvas.Add(background);
        }

        protected override void CleanupGraphics() { 
            background?.OnCanvas?.Remove(background);
        }

        internal override void Update() { 
            if (background != null) {
                background.Color = BackgroundColor;
                background.Fit(WorldRect);
            }
        }

    }

    public class Label : UIElement {
        public string Text { get; set; }
        public Text.Anchors TextAnchor { get; set; }

        public override string Print => $"Label '{Text}'";

        Text textObject;

        protected override void CreateGraphics() {
            textObject = new Text(Text, Style?.Font, Style?.FontSize ?? 0 );
            UI.Canvas.Add(textObject);
        }

        protected override void CleanupGraphics() { 
            textObject?.OnCanvas?.Remove(textObject);
        }

        internal override void Update() {
            textObject.Content = Text;
            var f = Style?.Font;
            if (f != null && f != textObject.Font) textObject.Font = f;
            textObject.Anchor = TextAnchor;
            if (Style?.FontSize > 0)textObject.CharacterSize = Style.FontSize;
            textObject.Rect = WorldRect;
        }
    }
}
