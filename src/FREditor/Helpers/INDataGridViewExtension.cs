using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inforoom.WinForms;
using Microsoft.Win32;
using System.Windows.Forms;


namespace FREditor.Helpers
{
	public static class INDataGridViewExtension
	{
		public static bool CanLoadSettings(this INDataGridView gridView, string RegKey)
		{
			using (RegistryKey k = Registry.CurrentUser.OpenSubKey(RegKey, true)) {
				if (k == null)
					return true;
				return k.SubKeyCount == gridView.ColumnCount;
			}
		}
	}
}