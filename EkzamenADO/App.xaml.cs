using EkzamenADO;
using System.Windows;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        new LoginWindow().Show();
    }
}
