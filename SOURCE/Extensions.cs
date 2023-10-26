using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    }
}
