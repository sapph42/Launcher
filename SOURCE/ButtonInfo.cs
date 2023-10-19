using System.Windows.Forms;
using System.Drawing;
using Newtonsoft.Json;
using System;

namespace Launcher {
    [JsonObject(MemberSerialization.OptIn)]
    internal class ButtonInfo {
        [JsonProperty]
        public readonly string Caption;
        [JsonProperty]
        public readonly string Path;
        [JsonProperty]
        public Point GridLocation;
        public Button StandardControl;
        public Button AdminControl;

        public ButtonInfo(string caption, string path, Point grid, Button standard, Button admin) {
            Caption = caption;
            Path = path;
            GridLocation = grid;
            StandardControl = standard;
            AdminControl = admin;
        }
        [JsonConstructor]
        public ButtonInfo(string caption, string path, Point grid) {
            Caption = caption;
            Path = path;
            GridLocation = grid;
            StandardControl = null;
            AdminControl = null;
        }
        public ButtonInfo(string caption, string path) {
            Caption = caption;
            Path = path;
            GridLocation = new Point(0,0);
            StandardControl = null;
            AdminControl = null;
        }

        internal void ButtonBuilder(int buffer, int height, int width, int index) {
            Button newButton = new Button();
            Point buttonLocation = new Point(
                (buffer * GridLocation.X) + (width * (GridLocation.X - 1)),
                (buffer * GridLocation.Y) + (height * (GridLocation.Y - 1))
            );
            newButton.Location = buttonLocation;
            newButton.Name = $"std_Button_{GridLocation.X}_{GridLocation.Y}";
            newButton.Size = new Size(width, height);
            newButton.TabIndex = index;
            newButton.Tag = Path;
            newButton.Text = Caption;
            newButton.UseVisualStyleBackColor = true;
            newButton.AllowDrop = true;
            StandardControl = newButton.Clone();
            newButton.Name = $"adm_Button_{GridLocation.X}_{GridLocation.Y}";
            AdminControl = newButton.Clone();
        }

        public bool Equals(ButtonInfo other) {
            return Caption == other.Caption
                   && Path == other.Path;
        }

        public new int GetHashCode() {
            return HashCode.Combine(Caption, Path);
        }
    }
}
