using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher {
    public sealed class KeyboardHook : IDisposable {

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private class Window : NativeWindow, IDisposable {
            private static int WM_HOTKEY = 0x0312;

            public Window() {
                CreateHandle(new CreateParams());
            }
            protected override void WndProc(ref Message m) {
                base.WndProc(ref m);
                if (m.Msg == WM_HOTKEY) {
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    System.Windows.Input.ModifierKeys modifier = (System.Windows.Input.ModifierKeys)((int)m.LParam & 0xFFFF);
                    if (KeyPressed != null)
                        KeyPressed(this, new KeyPressedEventArgs(modifier, key));
                }
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;
            public void Dispose() {
                DestroyHandle();
            }
        }

        private Window _window = new Window();
        private int _currentId;

        public KeyboardHook() {
            _window.KeyPressed += delegate (object sender, KeyPressedEventArgs args)
            {
                if (KeyPressed != null)
                    KeyPressed(this, args);
            };
        }

        public int RegisterHotKey(System.Windows.Input.ModifierKeys modifier, Keys key) {
            _currentId = _currentId + 1;
            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
                throw new InvalidOperationException("Couldn’t register the hot key.");
            return _currentId; 
        }
        public void UnregisterHotKey(int id) {
            UnregisterHotKey(_window.Handle, id);
        }
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        public void Dispose() {
            for (int i = _currentId; i > 0; i--) {
                UnregisterHotKey(_window.Handle, i);
            }
            _window.Dispose();
        }
    }
    public class KeyPressedEventArgs : EventArgs {
        private System.Windows.Input.ModifierKeys _modifier;
        private Keys _key;

        internal KeyPressedEventArgs(System.Windows.Input.ModifierKeys modifier, Keys key) {
            _modifier = modifier;
            _key = key;
        }
        public System.Windows.Input.ModifierKeys Modifier {
            get { return _modifier; }
        }
        public Keys Key {
            get { return _key; }
        }
    }

    [Flags]
    public enum ModifierKeys : uint {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }
}
