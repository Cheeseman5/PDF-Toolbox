using Factories;
using Helpers;
using PDFToolbox.Helpers;
using PDFToolbox.Interfaces;
using PDFToolbox.Interfaces.Helpers;
using System;
using System.Reflection;
using System.Windows;

namespace PDFToolbox
{
    public class Program
    {
        private static void GenerateDependencied()
        {
            IConfig config = new DefaultConfig();
            var toolbox = new Helpers.Toolbox(config);
            var pageFactory = new PageFactory();
            ILogger logger = new ConsoleLogger(ConsoleLogger.eDebuggerDetail.Log);
            var fileIO = new Helpers.FileIO(toolbox, logger, config);
            string defaultSaveLocation = config.DefaultSaveDirectory;

            IFileIOExtractor outlookExtractor = new Helpers.OutlookAttachmentExtractor(config);
            IFileIOExtractor fileDropExtractor = new Helpers.FileDropExtractor();
            IFileIOStrategy pdfFileIO = new Helpers.PdfFileIO(toolbox, fileIO, pageFactory, config);

            // Register Extractors (pre-loaders)
            fileIO.RegisterExtractor(outlookExtractor);
            fileIO.RegisterExtractor(fileDropExtractor);
            // Register normal 
            fileIO.RegisterStrategy(pdfFileIO);

            var wnd = new MainWindow(toolbox, fileIO);
            wnd.Show();

        }

        [STAThreadAttribute]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                String resourceName = "AssemblyLoadingAndReflection." + new AssemblyName(args.Name).Name + ".dll";
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    var assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
            GenerateDependencied();
            App.Main();
        }
    }
}
