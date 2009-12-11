using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace FREditor.Helpers
{
	public class PriceFileFormatHelper
	{
		private PriceFormat? _currentPriceFormat = null;

		private PriceFormat? _currentOpenedFileFormat = null;

		private string _currentOpenedFileDelimiter = null;

		private string _currentDelimiter = String.Empty;

		private ulong _priceItemId = 0;

		private PriceFormat? _newPriceFormat = null;

		private string _newDelimiter = String.Empty;

		private DataTable _dataTableFormats = new DataTable();

		private string _errorMessage = String.Empty;

		private const string MessageFormatNotLoaded = "Необходимо сначала загрузить формат прайса с помощью метода LoadPriceFormat()";

		private const string MessageFormatNotSet = "Не указан формат (null)";

		public PriceFileFormatHelper(MySqlConnection connection)
		{            
			_dataTableFormats.Columns.AddRange(new DataColumn[] {
				new DataColumn("FormatName"),
				new DataColumn("FormatFileExtension"),
				new DataColumn("FormatId"), 
			});

			var querySelectFormats = @"
SELECT 
	Id AS FormatId,
	Format AS FormatName,
	FileExtention AS FormatFileExtension
FROM
	farm.pricefmts
";
			var command = new MySqlCommand(querySelectFormats, connection);
			var dataAdapter = new MySqlDataAdapter(command);
			dataAdapter.Fill(_dataTableFormats);
		}

		public void LoadPriceFormat(ulong priceItemId, PriceFormat? priceFormat, string delimiter)
		{
			_errorMessage = String.Empty;

			_currentPriceFormat = priceFormat;
			_currentDelimiter = delimiter;

			_newPriceFormat = priceFormat;
			_newDelimiter = delimiter;

			_currentOpenedFileFormat = priceFormat;
			_currentOpenedFileDelimiter = delimiter;

			_priceItemId = priceItemId;
		}

		public bool SetNewFormat(PriceFormat? newFormat, string newDelimiter)
		{
			if (_priceItemId != 0)
			{
				_errorMessage = String.Empty;
				if ((newFormat == PriceFormat.NativeDelimWIN) ||
				    (newFormat == PriceFormat.NativeDelimDOS))
				{
					if (String.IsNullOrEmpty(newDelimiter))
					{
						_errorMessage = "Для данного формата необходимо сначала задать разделитель";
						return false;
					}
				}
				if ((_newPriceFormat != newFormat) || (_newDelimiter != newDelimiter))
				{
					_newPriceFormat = newFormat;
					_newDelimiter = newDelimiter;
					return true;
				}
			}
			else
			{
				_errorMessage = MessageFormatNotLoaded;
			}
			return false;
		}

		public void OpenFileInNewFormat()
		{
			_currentOpenedFileFormat = _newPriceFormat;
			_currentOpenedFileDelimiter = _newDelimiter;
		}

		public bool ChangeFormat()
		{
			if (_priceItemId != 0)
			{
				var result = false;
				_errorMessage = String.Empty;

				if (_currentPriceFormat != _newPriceFormat)
				{
					_currentPriceFormat = _newPriceFormat;
					result = true;
				}

				if (_currentDelimiter != _newDelimiter)
				{
					_currentDelimiter = _newDelimiter;
					result = true;
				}
				return result;
			}
			else
			{
				_errorMessage = MessageFormatNotLoaded;
			}
			return false;
		}

		public PriceFormat? CurrentOpenedFileFormat
		{
			get { return _currentOpenedFileFormat; }
		}

		public string CurrentOpenedFileDelimiter
		{
			get { return _currentOpenedFileDelimiter; }
		}

		public void ClearFormat()
		{
			_errorMessage = String.Empty;

			_currentPriceFormat = null;
			_currentDelimiter = String.Empty;

			_newPriceFormat = null;
			_newDelimiter = String.Empty;

			_currentOpenedFileFormat = null;
			_currentOpenedFileDelimiter = String.Empty;

			_priceItemId = 0;
		}

		private string GetShortFileName(PriceFormat? priceFormat)
		{
            if (_priceItemId != 0)
            {
            	if (priceFormat != null)
            	{
            		var shortFileName = Convert.ToString(_priceItemId);
            		return shortFileName + GetFileExtension(priceFormat);
            	}
            	else
            	{
            		_errorMessage = MessageFormatNotSet;
            	}
			}
            else
            {
            	_errorMessage = MessageFormatNotLoaded;
            }
			return String.Empty;
		}

		public string CurrentShortFileName
		{
			get { return GetShortFileName(_currentPriceFormat); }
		}

		public string NewShortFileName
		{
			get { return GetShortFileName(_newPriceFormat); }
		}

		// Сообщение с текстом ошибки, с которой завершилась последняя операция
		public string LastErrorMessage
		{
			get { return _errorMessage; }
		}

		private string GetFileExtension(PriceFormat? priceFormat)
		{
			if (_priceItemId != 0)
			{
				if (priceFormat != null)
				{
					var extension = _dataTableFormats.Select("FormatName = '" + priceFormat + "'");
					if (extension.Length != 1)
					{
						_errorMessage = "Для данного формата задано более одного расширения";
						return String.Empty;
					}
					return Convert.ToString(extension[0]["FormatFileExtension"]);
				}
				else
				{
					_errorMessage = MessageFormatNotSet;
				}
			}
			else
			{
				_errorMessage = MessageFormatNotLoaded;
			}
			return String.Empty;
		}

		/// <summary>
		/// Расширение для файла прайс-листа в текущем формате
		/// </summary>
		public string CurrentFileExtension
		{
			get { return GetFileExtension(_currentPriceFormat); }
		}

		public string CurrentOpenedFileExtension
		{
			get { return GetFileExtension(_currentOpenedFileFormat); }
		}

		/// <summary>
		/// Расширение для файла прайс-листа в новом формате
		/// </summary>
		public string NewFileExtension
		{
			get { return GetFileExtension(_newPriceFormat); }
		}

		public PriceFormat? CurrentFormat
		{
			get { return _currentPriceFormat; }
		}

		public PriceFormat? NewFormat
		{
			get { return _newPriceFormat; }
		}

		public string CurrentDelimiter
		{
			get { return _currentDelimiter; }
		}

		public string NewDelimiter
		{
			get { return _newDelimiter; }
		}

		public void ResetNewFormat()
		{
			if (_priceItemId != 0)
			{
				_newPriceFormat = _currentOpenedFileFormat;
				_newDelimiter = _currentOpenedFileDelimiter;
			}
			else
			{
				_errorMessage = MessageFormatNotLoaded;
			}
		}

		public static bool IsTextFormat(PriceFormat? priceFormat)
		{
			return ((priceFormat == PriceFormat.FixedDOS) ||
			        (priceFormat == PriceFormat.FixedWIN) ||
			        (priceFormat == PriceFormat.NativeDelimDOS) ||
			        (priceFormat == PriceFormat.NativeDelimWIN) ||
			        (priceFormat == PriceFormat.NativeFixedDOS) ||
			        (priceFormat == PriceFormat.NativeFixedWIN) ||
			        (priceFormat == PriceFormat.DelimWIN) ||
			        (priceFormat == PriceFormat.DelimDOS));

		}
	}
}
