﻿#region Namespaces
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media;
#endregion

namespace SpeckleRevitPlugin
{
    [Transaction(TransactionMode.Manual)]
    class app : IExternalApplication
    {
        static string m_Path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static UIControlledApplication uiApp;

        #region Setup
        /// <summary>
        /// Load an Image Source from File
        /// </summary>
        /// <param name="SourceName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private ImageSource LoadPngImgSource(string SourceName)
        {


            try
            {
                // Assembly
                Assembly assembly = Assembly.GetExecutingAssembly();

                // Stream
                Stream icon = assembly.GetManifestResourceStream(SourceName);

                // Decoder
                PngBitmapDecoder decoder = new PngBitmapDecoder(icon, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

                // Source
                ImageSource m_source = decoder.Frames[0];
                return (m_source);


            }
            catch
            {
            }

            // Fail
            return null;

        }

        /// <summary>
        /// Add the Ribbon Item and Panel
        /// </summary>
        /// <param name="a"></param>
        /// <remarks></remarks>
        public void AddRibbonPanel(UIControlledApplication a)
        {
            try
            {
                // First Create the Tab
                a.CreateRibbonTab("Speckle");
            }
            catch
            {
                // Might already exist...
            }

            // Tools
            AddButton("Speckle", 
                    "Connection\r\nTest",
                    "Connection\r\nTest", 
                    "SpeckleRevitPlugin.Template_16.png", 
                    "SpeckleRevitPlugin.Template_32.png", 
                    (m_Path + "\\SpeckleRevitPlugin.dll"), 
                    "SpeckleRevitPlugin.cmd", 
                    "Speckle connection test for Revit.");
        }

        /// <summary>
        /// Add a button to a Ribbon Tab
        /// </summary>
        /// <param name="Rpanel">The name of the ribbon panel</param>
        /// <param name="ButtonName">The Name of the Button</param>
        /// <param name="ButtonText">Command Text</param>
        /// <param name="ImagePath16">Small Image</param>
        /// <param name="ImagePath32">Large Image</param>
        /// <param name="dllPath">Path to the DLL file</param>
        /// <param name="dllClass">Full qualified class descriptor</param>
        /// <param name="Tooltip">Tooltip to add to the button</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool AddButton(string Rpanel, string ButtonName, string ButtonText, string ImagePath16, string ImagePath32, string dllPath, string dllClass, string Tooltip)
        {
            try
            {
                // The Ribbon Panel
                RibbonPanel ribbonPanel = null;

                // Find the Panel within the Case Tab
                List<RibbonPanel> rp = new List<RibbonPanel>();
                rp = uiApp.GetRibbonPanels("Speckle");
                foreach (RibbonPanel x in rp)
                {
                    if (x.Name.ToUpper() == Rpanel.ToUpper())
                    {
                        ribbonPanel = x;
                    }
                }

                // Create the Panel if it doesn't Exist
                if (ribbonPanel == null)
                {
                    ribbonPanel = uiApp.CreateRibbonPanel("Speckle", Rpanel);
                }

                // Create the Pushbutton Data
                PushButtonData pushButtonData = new PushButtonData(ButtonName, ButtonText, dllPath, dllClass);
                if (!string.IsNullOrEmpty(ImagePath16))
                {
                    try
                    {
                        pushButtonData.Image = LoadPngImgSource(ImagePath16);
                    }
                    catch
                    {
                    }
                }
                if (!string.IsNullOrEmpty(ImagePath32))
                {
                    try
                    {
                        pushButtonData.LargeImage = LoadPngImgSource(ImagePath32);
                    }
                    catch
                    {
                    }
                }
                pushButtonData.ToolTip = Tooltip;

                // Add the button to the tab
                PushButton pushButtonDataAdd = (PushButton)ribbonPanel.AddItem(pushButtonData);
            }
            catch
            {
                // Quiet Fail
            }
            return true;
        }
        #endregion

        #region Startup
        public Result OnStartup(UIControlledApplication a)
        {
            try
            {
                // The Shared uiApp variable
                uiApp = a;

                // Add the Ribbon Panel!!
                AddRibbonPanel(a);

                // Return Success
                return Result.Succeeded;

            }
            catch { return Result.Failed; }
        }
        #endregion

        #region Shutdown
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
        #endregion
    }
}