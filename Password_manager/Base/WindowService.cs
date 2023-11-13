using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Password_manager.Base
{
    public class WindowService : IWindowService
    {
        public void CloseWindow(Window window)
        {
            window.Close();
        }

        public void ShowWindow(Window window, object viewModel)
        {
            window.DataContext = viewModel;
            window.Show();
        }

    }
}
