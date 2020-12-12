using System.ComponentModel;
using System.ServiceProcess;

namespace LabDeCSharp
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        ServiceInstaller si;
        ServiceProcessInstaller spi;
        public Installer1()
        {
            InitializeComponent();

            si = new ServiceInstaller
            {
                StartType = ServiceStartMode.Manual,
                ServiceName = "LabDeCSharp"
            };
            spi = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };
            Installers.Add(spi);
            Installers.Add(si);
        }
    }
}
