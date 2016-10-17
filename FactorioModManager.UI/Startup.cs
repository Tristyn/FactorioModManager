using System.Windows.Forms;
using FactorioModManager.UI.Views;

namespace FactorioModManager.UI
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            var form = new DebugView();
            Application.Run(form);
        }
    }
}
