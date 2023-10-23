using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Launcher {
    internal class ButtonCollection : List<ButtonInfo> {

        private int _x;
        private int _y;
        private readonly Point _default = new Point(0, 0);

        public int X => _x;
        public int Y => _y;

        public int Buffer { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public ButtonCollection() { }

        public ButtonCollection(int buffer, int width, int height) {
            Buffer = buffer;
            Width = width;
            Height = height;
        }

        public new void Add(ButtonInfo thisButton) {
            base.Add(thisButton);
            CalcGridSize();
            if (this.Select(bc => bc.GridLocation).Any(p => p.Equals(_default)))
                ArrangeGrid();
        }

        public new void AddRange(IEnumerable<ButtonInfo> collection) {
            base.AddRange(collection);
            CalcGridSize();
            if (this.Select(bc => bc.GridLocation).Any(p => p.Equals(_default)))
                ArrangeGrid();
        }

        private void CalcGridSize() {
            if (Count == 0) {
                _x = 0;
                _y = 0;
                return;
            }
            int buttonGridWidth = 1;
            int buttonGridHeight = 1;
            switch (Count) {
                case 1:
                    buttonGridHeight = 1;
                    buttonGridWidth = 1;
                    break;
                case 2:
                    buttonGridHeight = 1;
                    buttonGridWidth = 2;
                    break;
                case int n when (n >= 3):
                    buttonGridWidth = (int)Math.Round(Math.Sqrt(Count));
                    float ratio = (float)Count / buttonGridWidth;
                    var prelimHeight = (int)Math.Round(ratio);
                    int prelimArea = prelimHeight * buttonGridWidth;
                    int offset = prelimArea < Count ? Count - prelimArea : 0;
                    buttonGridHeight = prelimHeight + offset;
                    break;
            }

            _x = buttonGridWidth;
            _y = buttonGridHeight;
        }

        private void ArrangeGrid() {
            int x = 1;
            int y = 1;
            if (
                this.All(b => b.GridLocation != _default) 
                && this.GroupBy(b => b.GridLocation).All(g => g.Count() == 1)
            )
                return;
            foreach (ButtonInfo button in this) {
                if (y > _y) {
                    y = 1;
                    x++;
                }

                button.GridLocation = new Point(x, y);
                y++;
            }
        }

        private void ConstructButtons() {
            foreach (ButtonInfo button in this) {
                int index = (button.GridLocation.Y - 1) * _y + (button.GridLocation.X - 1);
                button.ButtonBuilder(Buffer, Height, Width, index);
            }
        }

        public void Validate() {
            CalcGridSize();
            ArrangeGrid();
            ConstructButtons();
        }

        public List<Button> Buttons() {
            List<Button> buttons = new List<Button>();
            foreach (ButtonInfo button in this) {
                buttons.Add(button.StandardControl);
                buttons.Add(button.AdminControl);
            }

            return buttons;
        }
        public void Rearrange(ButtonInfo button1, ButtonInfo button2) {
            Point temp1 = button1.GridLocation;
            Point temp2 = button2.GridLocation;
            foreach (ButtonInfo button in this) {
                if (button.Equals(button1)) {
                    button.GridLocation = temp2;
                    button.Caption = button1.Caption;
                    button.Path = button1.Path;
                }
                if (button.Equals(button2)) {
                    button.GridLocation = temp1;
                    button.Caption = button2.Caption;
                    button.Path = button2.Path;
                }
            }
        }
    }
}
