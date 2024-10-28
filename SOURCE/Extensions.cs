using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Launcher {
    public static class Extensions {
        public static T Clone<T>(this T controlToClone)
            where T : Control {
            T instance = Activator.CreateInstance<T>();
            foreach (PropertyInfo propInfo in typeof(T)
                         .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         .Where(p => !p.GetIndexParameters().Any())) {
                if (propInfo.CanWrite) {
                    if (propInfo.Name != "WindowTarget")
                        propInfo.SetValue(instance, propInfo.GetValue(controlToClone, null), null);
                }
            }

            return instance;
        }

        public static double GetRelativeLuminance(this Color color) {
            double rS = color.R / 255.0;
            double gS = color.G / 255.0;
            double bS = color.B / 255.0;
            double r, g, b;
            if (rS <= 0.03928) {
                r = rS / 12.92;
            }
            else {
                r = Math.Pow(((rS + 0.055) / 1.055), 2.4);
            }
            if (gS <= 0.03928) {
                g = gS / 12.92;
            } else {
                g = Math.Pow(((gS + 0.055) / 1.055), 2.4);
            }
            if (bS <= 0.03928) {
                b = bS / 12.92;
            } else {
                b = Math.Pow(((bS + 0.055) / 1.055), 2.4);
            }

            return (0.2126 * r) + (0.7152 * g) + (0.0722 * b);
        }

        public static double GetContrastRatio(this Color color1, Color color2) {
            if (color1.GetRelativeLuminance() > color2.GetRelativeLuminance())
                return (color1.GetRelativeLuminance() + 0.05) / (color2.GetRelativeLuminance() + 0.05);
            return (color2.GetRelativeLuminance() + 0.05) / (color1.GetRelativeLuminance() + 0.05);
        }

        public static IEnumerable<T> GetUniqueFlags<T>(this T flags) where T : Enum {
            if (flags is Keys keys) {
                foreach (Enum value in Enum.GetValues(keys.GetType()))
                    if (keys.HasFlag(value))
                        yield return (T)value;
            }
        }
    }
}
