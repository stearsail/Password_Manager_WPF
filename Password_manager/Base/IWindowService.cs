using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Password_manager.Base
{
    interface IWindowService
    {
        void ShowWindow(Window window, object viewModel);
        void CloseWindow(Window window);
    }
}
