using Deucalion.IP_Switcher.Features.MessageBox;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Deucalion.IP_Switcher.Helpers.ShowWindow
{
    public static class Show
    {
        public static bool Message(string Content, string Caption, bool AllowCancel = false)
        {
            return new MessageBoxViewModel().Show(GetTopWindow(), Caption, Content, AllowCancel);
        }

        public static bool Message(string Content, bool AllowCancel = false)
        {
            return new MessageBoxViewModel().Show(GetTopWindow(), Resources.ShowLoc.MessageCaption, Content, AllowCancel);
        }

        public static bool? Dialog<T>(Action<T> callback = null) where T : Window, new()
        {
            var dialog = new T() { Owner = GetTopWindow() };

            var result = dialog.ShowDialog();
            if (callback != null)
                callback(dialog);
            return result;
        }

        public static async Task<bool?> Dialog<T>(dynamic parameters, Func<T, Task> callback) where T : Window, new()
        {
            var dialog = (T)Activator.CreateInstance(typeof(T), parameters);

            dialog.Owner = GetTopWindow();

            dynamic result = new ExpandoObject();
            result.DialogResult = dialog.ShowDialog();
            if (callback != null)
               await callback(dialog);

            return result.DialogResult;
        }

        public static bool? Dialog<T>(dynamic parameters, Action<T> callback) where T : Window, new()
        {
            var dialog = (T)Activator.CreateInstance(typeof(T), parameters);

            dialog.Owner = GetTopWindow();

            dynamic result = new ExpandoObject();
            result.DialogResult = dialog.ShowDialog();
            if (callback != null)
                callback(dialog);

            return result.DialogResult;
        }

        public static void Window<T>(double? reduceWidthByPercent, double? reduceHeightByPercent) where T : Window, new()
        {
            if (reduceHeightByPercent == null || reduceWidthByPercent == null)
            {
                var window = new T() { Owner = GetTopWindow() };
                window.ShowDialog();
            }
            else
            {
                var size = GetWindowSize(reduceWidthByPercent.Value, reduceHeightByPercent.Value);
                var window = new T() { Owner = GetTopWindow(), Width = size.Width, Height = size.Height };
                window.ShowDialog();
            }
        }

        private static Size GetWindowSize(double reduceWidthByPercent, double reduceHeightByPercent)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;
            var windowCentre = mainWindow.PointToScreen(new Point(mainWindow.ActualWidth / 2, mainWindow.ActualHeight / 2));
            var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)windowCentre.X, (int)windowCentre.Y));

            return new Size(screen.WorkingArea.Width - (reduceWidthByPercent * screen.WorkingArea.Width),
                screen.WorkingArea.Height - (reduceHeightByPercent * screen.WorkingArea.Height));
        }

        private static Window GetTopWindow()
        {
            return System.Windows.Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
        }
    }
}
