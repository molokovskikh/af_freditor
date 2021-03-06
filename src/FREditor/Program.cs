﻿using System;
using System.Collections.Generic;
using System.Text;
using Common.MySql;
using log4net;
using System.Windows.Forms;
using System.Threading;
using Inforoom.WinForms.Helpers;
using FREditor.Properties;
using log4net.Config;

namespace FREditor
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			XmlConfigurator.Configure();
			ConnectionHelper.DefaultConnectionStringName = "Main";
			With.DefaultConnectionStringName = ConnectionHelper.GetConnectionName();
			Application.ThreadException += OnThreadException;
			Application.ApplicationExit += ApplicationExit;
			GlobalContext.Properties["GlobalContext"] = Application.ProductVersion;


			//Эти две строчки есть в StatViewer'е, возможно, из-за одной из них не работает "корректное" отображение 
			//значений столбца "Сегмент" в фильтрах компонентов DevExpress
			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);

			InputLanguageHelper.SetToRussian();

			Application.Run(new frmFREMain());
		}

		private static void ApplicationExit(object sender, EventArgs e)
		{
			Settings.Default.Save();
		}

		public static void SendMessageOnException(object sender, Exception exception)
		{
			ILog _logger;
			if (sender == null)
				_logger = LogManager.GetLogger(typeof(Program));
			else
				_logger = LogManager.GetLogger(sender.GetType());

			_logger.Error(exception);
		}

		// Handles the exception event.
		public static void OnThreadException(object sender, ThreadExceptionEventArgs t)
		{
			SendMessageOnException(sender, t.Exception);
			MessageBox.Show("В приложении возникла необработанная ошибка.\r\nИнформация об ошибке была отправлена разработчику.");
		}
	}
}