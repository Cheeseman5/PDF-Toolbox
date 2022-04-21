using Factories;
using PDFToolbox.Helpers;
using PDFToolbox.Interfaces;
using PDFToolbox.Interfaces.Helpers;
using PDFToolbox.IO;
using System;
using System.Reflection;
using System.Windows;

namespace PDFToolbox
{
    public class Program
    {
        private const bool USE_REFACTORED_CODE = false;
        private static void GenerateDependencied()
        {
            Window wnd;
            if (!USE_REFACTORED_CODE)
            {
                wnd = new MainWindow();
                wnd.Show();
                return;
            }

            var toolbox = new Helpers.Toolbox();
            var pageFactory = new PageFactory();
            ILogger logger = new ConsoleLogger(ConsoleLogger.eDebuggerDetail.Log);
            var fileIO = new Helpers.FileIO(toolbox, logger);

            IFileIOExtractor outlookExtractor = new Helpers.OutlookAttachmentExtractor(fileIO);
            IFileIOExtractor fileDropExtractor = new Helpers.FileDropExtractor();
            IFileIOStrategy pdfFileIO = new Helpers.PdfFileIO(toolbox, fileIO, pageFactory);

            // Register Extractors (pre-loaders)
            fileIO.RegisterExtractor(outlookExtractor);
            fileIO.RegisterExtractor(fileDropExtractor);
            // Register normal 
            fileIO.RegisterStrategy(pdfFileIO);

            wnd = new MainWindow(toolbox, fileIO);
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
