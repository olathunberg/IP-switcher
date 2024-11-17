using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TTech.IP_Switcher.Features.MessageBox;

namespace TTech.IP_Switcher.Helpers.ShowWindow
{
    public static class Show
    {
        public static bool Message(string Caption, string Content, bool AllowCancel = false)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                return new MessageBoxViewModel().Show(GetTopWindow(), Caption, Content, AllowCancel);
            });
        }

        public static bool Message(string Content, bool AllowCancel = false)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                return new MessageBoxViewModel().Show(GetTopWindow(), Resources.ShowLoc.MessageCaption, Content, AllowCancel);
            });
        }

        public static bool? Dialog<T>(Action<T> callback = null) where T : Window, new()
        {
            var owner = GetTopWindow();

            var dialog = new T { Owner = owner };

            var result = dialog.ShowDialog();
            callback?.Invoke(dialog);
            return result;
        }

        public static async Task<bool?> Dialog<T>(dynamic parameters, Func<T, Task> callback) where T : Window, new()
        {
            var owner = GetTopWindow();
            var dialog = (T)Activator.CreateInstance(typeof(T), parameters);

            dialog.Owner = owner;

            dynamic result = new ExpandoObject();
            result.DialogResult = dialog.ShowDialog();
            if (callback != null)
                await callback(dialog);

            return result.DialogResult;
        }

        public static bool? Dialog<T>(dynamic parameters, Action<T> callback) where T : Window, new()
        {
            var owner = GetTopWindow();
            var dialog = (T)Activator.CreateInstance(typeof(T), parameters);

            dialog.Owner = owner;

            dynamic result = new ExpandoObject();
            result.DialogResult = dialog.ShowDialog();
            callback?.Invoke(dialog);

            return result.DialogResult;
        }

        private static Window GetTopWindow()
        {
            var topWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            if (topWindow != null)
                return topWindow;
            return Application.Current.Windows.OfType<Window>().LastOrDefault();
        }
    }
}
