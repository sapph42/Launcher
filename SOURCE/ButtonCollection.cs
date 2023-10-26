using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Launcher {
    internal class ButtonCollection : HashSet<LauncherButton> {

        private int _width;
        private int _height;
        private readonly Point _default = new Point(0, 0);

        public int Width => _width;
        public int Height => _height;

        public int ButtonBuffer { get; set; }
        public int ButtonWidth { get; set; }
        public int ButtonHeight { get; set; }
        public int GridBufferHeight { get; set; }
        public int GridBufferWidth { get; set; }

        public ButtonCollection() { }

        public ButtonCollection(int buttonBuffer, int buttonWidth, int buttonHeight) {
            ButtonBuffer = buttonBuffer;
            ButtonWidth = buttonWidth;
            ButtonHeight = buttonHeight;
        }

        public new void Add(LauncherButton thisButton) {
            _ = base.Add(thisButton);
            CalcGridSize();
            if (this.Select(bc => bc.GridLocation).Any(p => p.Equals(_default)))
                ArrangeGrid();
        }

        public void Add(LauncherButton thisButton, bool noRecalc) {
            _ = base.Add(thisButton);
            if (noRecalc)
                return;
            CalcGridSize();
            if (this.Select(bc => bc.GridLocation).Any(p => p.Equals(_default)))
                ArrangeGrid();
        }

        public void AddRange(IEnumerable<LauncherButton> collection) {
            foreach (LauncherButton button in collection) {
                if (!this.Any(b => b.Caption==button.Caption && b.Path==button.Path && b.Arguments==button.Arguments))
                    _ = base.Add(button);
            }
            CalcGridSize();
            if (this.Select(bc => bc.GridLocation).Any(p => p.Equals(_default)))
                ArrangeGrid();
        }

        public void Remove(IEnumerable<LauncherButton> collection, bool noRecalc = false) {
            foreach (LauncherButton item in collection) {
                if (Contains(item))
                    Remove(item);
            }
            if (noRecalc)
                return;
            CalcGridSize();
        }

        private void CalcGridSize() {
            if (Count == 0) {
                _width = 0;
                _height = 0;
                return;
            }

            if (!this.Select(bc => bc.GridLocation).Any(p => p.Equals(_default))) {
                _width = this.Max(b => b.GridLocation.X);
                _height = this.Max(b => b.GridLocation.Y);
                return;
            }
            switch (Count) {
                case 1:
                    _height = 1;
                    _width = 1;
                    break;
                case 2:
                    _height = 1;
                    _width = 2;
                    break;
                case int n when (n >= 3):
                    _width = (int)Math.Round(Math.Sqrt(Count));
                    float ratio = (float)Count / _width;
                    var prelimHeight = (int)Math.Round(ratio);
                    int prelimArea = prelimHeight * _width;
                    int offset = prelimArea < Count ? Count - prelimArea : 0;
                    _height = prelimHeight + offset;
                    break;
            }
        }

        private void ArrangeGrid() {
            int x = 1;
            int y = 1;
            if (
                this.All(b => b.GridLocation != _default)
                && this.GroupBy(b => b.GridLocation)
                    .All(g => g.Count() == 1)
            ) {
                FillEmptySlots();
                return;
            }
            foreach (LauncherButton button in this) {
                if (x > _width) {
                    x = 1;
                    y++;
                }

                button.GridLocation = new Point(x, y);
                x++;
            }
            FillEmptySlots();
        }

        private void FillEmptySlots() {
            bool[,] filled = new bool[_width, _height];
            for (int x = 0; x < _width; x++) {
                for (int y = 0; y < _height; y++)
                    filled[x, y] = false;
            }
            foreach (LauncherButton button in this) {
                filled[button.GridLocation.X-1, button.GridLocation.Y-1] = true;
            }
            for (int x = 0; x < _width; x++) {
                for (int y = 0; y < _height; y++)
                     if (!filled[x, y])
                         Add(new LauncherButton("","", new Point(x+1, y+1)), true);
            }
        }
        private void ConstructButtons() {
            foreach (LauncherButton button in this) {
                int index = (button.GridLocation.Y - 1) * _height + (button.GridLocation.X - 1);
                button.Location = new Point(
                        (ButtonBuffer * button.GridLocation.X) + (ButtonWidth * (button.GridLocation.X-1)) + GridBufferWidth,
                        (ButtonBuffer * button.GridLocation.Y) + (ButtonHeight * (button.GridLocation.Y - 1)) + GridBufferHeight
                    );
                button.Size = new Size(ButtonWidth, ButtonHeight);
                button.TabIndex = index;
                button.UseVisualStyleBackColor = true;
                button.AllowDrop = true;
                button.ColorCheck();
                button.Name = $"button_{button.GridLocation.X}_{button.GridLocation.Y}";
            }
        }

        public void Validate(bool overrideSizeCalc = false) {
            if (overrideSizeCalc)
                CalcGridSize();
            else {
                _width = this.Max(b => b.GridLocation.X);
                _height = this.Max(b => b.GridLocation.Y);
            }
            ArrangeGrid();
            ConstructButtons();
        }
    }
}
