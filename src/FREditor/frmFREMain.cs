using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using Common.MySql;
using Common.Tools;
using FREditor.Helpers;
using FREditor.Properties;
using Inforoom.WinForms;
using MySql.Data.MySqlClient;
using RemotePriceProcessor;
using log4net;
using MySqlHelper = MySql.Data.MySqlClient.MySqlHelper;

namespace FREditor
{
	public enum dgFocus
	{
		Firm,
		Price
	}

	public partial class frmFREMain : Form
	{
		private bool _isFormatChanged;

		private ILog _logger = LogManager.GetLogger(typeof(frmFREMain));

		ArrayList gds = new ArrayList();
		List<DataTable> dtables = new List<DataTable>();
		ArrayList tblstyles = new ArrayList();
		DataTable dtPrice = new DataTable();
		
		private MySqlConnection connection = new MySqlConnection(Literals.ConnectionString());

		private MySqlCommand command = new MySqlCommand();
		private MySqlDataAdapter dataAdapter = new MySqlDataAdapter();
		private string BaseRegKey = "Software\\Inforoom\\FREditor";
		private string CregKey;

		string EndPath = Path.GetTempPath();
		string frmCaption = String.Empty;

		string delimiter = String.Empty;
		PriceFormat? fmt;

		//Текущий клиент, с которым происходит работа и текущий прайс
		long currentPriceItemId;
		long currentClientCode;

		string nameR = String.Empty;
		string FilterParams = String.Empty;

		private string shortNameFilter = String.Empty;
		private ulong regionCodeFilter = 0;
		private int sourceIndex = 0;

		bool fileExist;
		bool InSearch;

		StringFormat sf = new StringFormat();

		public frmWait fW;
		public frmNameMask frmNM;


		public DataTable dtCostTypes;
		public DataTable dtPriceTypes;
		protected dgFocus fcs;

		public PriceProcessorWcfHelper _priceProcessor;

		// Поле для хранения текста, который нужно найти 
		// внутри колонок прайса (или среди заголовков колонок)
		// (вкладка "Прайс")
		private string _searchTextInPrice = String.Empty;

		// Поле для подсчета времени бездействия пользователя 
		// при поиске внутри колонок прайс-листа
		private int _inactivityTime;

		private DataGridView _searchGrid;

		// Флаг, который показывает, зарегистрирован ли обработчик 
		// для cmbFormat (выпадающий список с форматами)
		private bool _isComboBoxFormatHandlerRegistered;

		// Флаг показывает нужно выбирать только действующих поставщиков (true)
		// или же всех подряд (false)
		private bool _showDisabledFirm = false;

		private PriceFileFormatHelper _priceFileFormatHelper;

		// Полный путь к текущему открытому файлу прайс-листа
		private string _currentFilename;

		private DataTableMarking _dataTableMarking = new DataTableMarking();

		public SynonymMatcher matcher;

		public frmFREMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			dtCostTypes = new DataTable("CostTypes");
			dtCostTypes.Columns.Add("ID", typeof(int));
			dtCostTypes.Columns.Add("Name", typeof(string));
			dtCostTypes.Rows.Add(new object[] { DBNull.Value, "<не указан>" });
			dtCostTypes.Rows.Add(new object[] { 0, "мультиколоночный" });
			dtCostTypes.Rows.Add(new object[] { 1, "многофайловый" });

			pCostTypeDataGridViewComboBoxColumn.DataSource = dtCostTypes;
			pCostTypeDataGridViewComboBoxColumn.DisplayMember = "Name";
			pCostTypeDataGridViewComboBoxColumn.ValueMember = "ID";

			dtPriceTypes = new DataTable("PriceTypes");
			dtPriceTypes.Columns.Add("ID", typeof(int));
			dtPriceTypes.Columns.Add("Name", typeof(string));
			dtPriceTypes.Rows.Add(new object[] { DBNull.Value, "<не указан>" });
			dtPriceTypes.Rows.Add(new object[] { 0, "обычный" });
			dtPriceTypes.Rows.Add(new object[] { 1, "ассортиментный" });
			dtPriceTypes.Rows.Add(new object[] { 2, "vip" });

			pPriceTypeDataGridViewComboBoxColumn.DataSource = dtPriceTypes;
			pPriceTypeDataGridViewComboBoxColumn.DisplayMember = "Name";
			pPriceTypeDataGridViewComboBoxColumn.ValueMember = "ID";

			this.mcmdInsertCostRules.CommandText = @"
INSERT INTO usersettings.PricesCosts (PriceCode, BaseCost, PriceItemId, CostName) values (?PriceCode, 0, ?PriceItemId, ?CostName);
SET @NewPriceCostId:=Last_Insert_ID();
INSERT INTO farm.costformrules (CostCode, FieldName, TxtBegin, TxtEnd) values (@NewPriceCostId, ?FieldName, ?TxtBegin, ?TxtEnd);
";
			this.mcmdInsertCostRules.Parameters.Add("?PriceItemId", MySqlDbType.Int64, 0, "CFRPriceItemId");
			this.mcmdInsertCostRules.Parameters.Add("?PriceCode", MySqlDbType.Int64, 0);
			this.mcmdInsertCostRules.Parameters.Add("?CostName", MySqlDbType.VarString, 0, "CFRCostName");
			this.mcmdInsertCostRules.Parameters.Add("?FieldName", MySqlDbType.VarString, 0, "CFRFieldName");
			this.mcmdInsertCostRules.Parameters.Add("?TxtBegin", MySqlDbType.VarString, 0, "CFRTextBegin");
			this.mcmdInsertCostRules.Parameters.Add("?TxtEnd", MySqlDbType.VarString, 0, "CFRTextEnd");

			this.mcmdDeleteCostRules.CommandText = "usersettings.DeleteCost";
			this.mcmdDeleteCostRules.CommandType = CommandType.StoredProcedure;
			this.mcmdDeleteCostRules.Parameters.Clear();
			this.mcmdDeleteCostRules.Parameters.Add("?inCostCode", MySqlDbType.Int64, 0, "CFRCost_Code");
			this.mcmdDeleteCostRules.Parameters["?inCostCode"].Direction = ParameterDirection.Input;


			this.mcmdUpdateCostRules.CommandText = @"
update usersettings.pricescosts pc
set
  CostName = ?CostName
where
	pc.CostCode = ?CostCode
and CostName <> ?CostName;
UPDATE farm.costformrules c SET
  FieldName = ?FieldName,
  TxtBegin = ?TxtBegin,
  TxtEnd = ?TxtEnd
WHERE c.CostCode = ?CostCode;";

			this.mcmdUpdateCostRules.Parameters.Add("?CostCode", MySqlDbType.Int64, 0, "CFRCost_Code");
			this.mcmdUpdateCostRules.Parameters.Add("?CostName", MySqlDbType.VarString, 0, "CFRCostName");
			this.mcmdUpdateCostRules.Parameters.Add("?FieldName", MySqlDbType.VarString, 0, "CFRFieldName");
			this.mcmdUpdateCostRules.Parameters.Add("?TxtBegin", MySqlDbType.VarString, 0, "CFRTextBegin");
			this.mcmdUpdateCostRules.Parameters.Add("?TxtEnd", MySqlDbType.VarString, 0, "CFRTextEnd");

			var sql = Fields.Columnds().Implode(f => String.Format("{0} = ?FR{0}", f));

			this.mcmdUpdateFormRules.CommandText = String.Format(@"
UPDATE formrules
SET
  JunkPos = ?FRSelfJunkPos,
  AwaitPos = ?FRSelfAwaitPos,
  VitallyImportantMask = ?FRSelfVitallyImportantMask,

  PriceFormatId = ?FRPriceFormatId,

  StartLine = ?FRStartLine,
  `Delimiter` = ?FRDelimiter,
  ListName = ?FRListName,
  NameMask = ?FRNameMask,
  ForbWords = ?FRForbWords,

  TxtCodeBegin = ?FRTxtCodeBegin,
  TxtCodeEnd = ?FRTxtCodeEnd,
  TxtCodeCrBegin = ?FRTxtCodeCrBegin,
  TxtCodeCrEnd = ?FRTxtCodeCrEnd,
  TxtNameBegin = ?FRTxtNameBegin,
  TxtNameEnd = ?FRTxtNameEnd,
  TxtFirmCrBegin = ?FRTxtFirmCrBegin,
  TxtFirmCrEnd = ?FRTxtFirmCrEnd,
  TxtMinBoundCostBegin = ?FRTxtMinBoundCostBegin,
  TxtMinBoundCostEnd = ?FRTxtMinBoundCostEnd,
  TxtUnitBegin = ?FRTxtUnitBegin,
  TxtUnitEnd = ?FRTxtUnitEnd,
  TxtVolumeBegin = ?FRTxtVolumeBegin,
  TxtVolumeEnd = ?FRTxtVolumeEnd,
  TxtQuantityBegin = ?FRTxtQuantityBegin,
  TxtQuantityEnd = ?FRTxtQuantityEnd,
  TxtNoteBegin = ?FRTxtNoteBegin,
  TxtNoteEnd = ?FRTxtNoteEnd,
  TxtPeriodBegin = ?FRTxtPeriodBegin,
  TxtPeriodEnd = ?FRTxtPeriodEnd,
  TxtDocBegin = ?FRTxtDocBegin,
  TxtDocEnd = ?FRTxtDocEnd,
  TxtJunkBegin = ?FRTxtJunkBegin,
  TxtJunkEnd = ?FRTxtJunkEnd,
  TxtAwaitBegin = ?FRTxtAwaitBegin,
  TxtAwaitEnd = ?FRTxtAwaitEnd,
  TxtRequestRatioBegin = ?FRTxtRequestRatioBegin,
  TxtRequestRatioEnd = ?FRTxtRequestRatioEnd,
  TxtRegistryCostBegin = ?FRTxtRegistryCostBegin,
  TxtRegistryCostEnd = ?FRTxtRegistryCostEnd,
  TxtVitallyImportantBegin = ?FRTxtVitallyImportantBegin,
  TxtVitallyImportantEnd = ?FRTxtVitallyImportantEnd,
  TxtMaxBoundCostBegin = ?FRTxtMaxBoundCostBegin,
  TxtMaxBoundCostEnd = ?FRTxtMaxBoundCostEnd,
  TxtOrderCostBegin = ?FRTxtOrderCostBegin,
  TxtOrderCostEnd = ?FRTxtOrderCostEnd,
  TxtMinOrderCountBegin = ?FRTxtMinOrderCountBegin,
  TxtMinOrderCountEnd = ?FRTxtMinOrderCountEnd,
  TxtProducerCostBegin = ?FRTxtProducerCostBegin,
  TxtProducerCostEnd = ?FRTxtProducerCostEnd,
  TxtNdsBegin = ?FRTxtNdsBegin,
  TxtNdsEnd = ?FRTxtNdsEnd,

  FCode = ?FRFCode,
  FCodeCr = ?FRFCodeCr,
  FName1 = ?FRFName1,
  FName2 = ?FRFName2,
  FName3 = ?FRFName3,
  FFirmCr = ?FRFFirmCr,
  FMinBoundCost = ?FRFMinBoundCost,
  FUnit = ?FRFUnit,
  FVolume = ?FRFVolume,
  FQuantity = ?FRFQuantity,
  FNote = ?FRFNote,
  FPeriod = ?FRFPeriod,
  FDoc = ?FRFDoc,
  FJunk = ?FRFJunk,
  FAwait = ?FRFAwait,
  FRequestRatio = ?FRFRequestRatio,
  FRegistryCost = ?FRFRegistryCost,
  FVitallyImportant = ?FRFVitallyImportant,
  FMaxBoundCost = ?FRFMaxBoundCost,
  FOrderCost = ?FRFOrderCost,
  FMinOrderCount = ?FRFMinOrderCount,
  FProducerCost = ?FRFProducerCost,	
  FNds = ?FRFNds,

  Memo = ?FRMemo,
  {0}
where
  id = ?FRFormID;

update
  usersettings.pricesdata
set
  ParentSynonym = ?FRSynonyms
where
  PriceCode = ?FRSelfPriceCode;

update
  usersettings.priceitems
set
  RowCount = if(RowCount <> ?FRPosNum, 0, RowCount)
where
  Id = ?FRPriceItemId", sql);
			var parameters = mcmdUpdateFormRules.Parameters;
			parameters.Add("?FRFormID", MySqlDbType.Int64);
			parameters.Add("?FRSelfPriceCode", MySqlDbType.Int64);
			parameters.Add("?FRPriceItemId", MySqlDbType.Int64);
			parameters.Add("?FRSynonyms", MySqlDbType.Int64);
			parameters.Add("?FRStartLine", MySqlDbType.Int32);

			parameters.Add("?FRTxtCodeBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtCodeEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtCodeCrBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtCodeCrEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtNameBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtNameEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtFirmCrBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtFirmCrEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtMinBoundCostBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtMinBoundCostEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtUnitBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtUnitEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtVolumeBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtVolumeEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtQuantityBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtQuantityEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtNoteBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtNoteEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtPeriodBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtPeriodEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtDocBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtDocEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtJunkBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtJunkEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtAwaitBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtAwaitEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtRequestRatioBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtRequestRatioEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtRegistryCostBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtRegistryCostEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtVitallyImportantBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtVitallyImportantEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtMaxBoundCostBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtMaxBoundCostEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtOrderCostBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtOrderCostEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtMinOrderCountBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtMinOrderCountEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtProducerCostBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtProducerCostEnd", MySqlDbType.Int32);
			parameters.Add("?FRTxtNdsBegin", MySqlDbType.Int32);
			parameters.Add("?FRTxtNdsEnd", MySqlDbType.Int32);

			parameters.Add("?FRDelimiter", MySqlDbType.VarString);
			parameters.Add("?FRPosNum", MySqlDbType.Int64);
			parameters.Add("?FRSelfJunkPos", MySqlDbType.VarString);
			parameters.Add("?FRSelfAwaitPos", MySqlDbType.VarString);
			parameters.Add("?FRPriceFormatId", MySqlDbType.Int64);
			parameters.Add("?FRListName", MySqlDbType.VarString);
			parameters.Add("?FRNameMask", MySqlDbType.VarString);
			parameters.Add("?FRForbWords", MySqlDbType.VarString);
			parameters.Add("?FRSelfVitallyImportantMask", MySqlDbType.VarString);

			parameters.Add("?FRFCode", MySqlDbType.VarString);
			parameters.Add("?FRFCodeCr", MySqlDbType.VarString);
			parameters.Add("?FRFName1", MySqlDbType.VarString);
			parameters.Add("?FRFName2", MySqlDbType.VarString);
			parameters.Add("?FRFName3", MySqlDbType.VarString);
			parameters.Add("?FRFFirmCr", MySqlDbType.VarString);
			parameters.Add("?FRFMinBoundCost", MySqlDbType.VarString);
			parameters.Add("?FRFUnit", MySqlDbType.VarString);
			parameters.Add("?FRFVolume", MySqlDbType.VarString);
			parameters.Add("?FRFQuantity", MySqlDbType.VarString);
			parameters.Add("?FRFNote", MySqlDbType.VarString);
			parameters.Add("?FRFPeriod", MySqlDbType.VarString);
			parameters.Add("?FRFDoc", MySqlDbType.VarString);
			parameters.Add("?FRFJunk", MySqlDbType.VarString);
			parameters.Add("?FRFAwait", MySqlDbType.VarString);
			parameters.Add("?FRFRequestRatio", MySqlDbType.VarString);
			parameters.Add("?FRFRegistryCost", MySqlDbType.VarString);
			parameters.Add("?FRFVitallyImportant", MySqlDbType.VarString);
			parameters.Add("?FRFMaxBoundCost", MySqlDbType.VarString);
			parameters.Add("?FRFOrderCost", MySqlDbType.VarString);
			parameters.Add("?FRFMinOrderCount", MySqlDbType.VarString);
			parameters.Add("?FRFProducerCost", MySqlDbType.VarString);
			parameters.Add("?FRFNds", MySqlDbType.VarString);
			parameters.Add("?FRMemo", MySqlDbType.VarString);

			foreach (var column in Fields.Columnds())
				parameters.Add(String.Format("?FR{0}", column), MySqlDbType.VarString);

			foreach (MySqlParameter ms in parameters)
				ms.SourceColumn = ms.ParameterName.Substring(1);

			dtSet.Tables.Add(_dataTableMarking);

			_priceProcessor = new PriceProcessorWcfHelper(Settings.Default.WCFServiceUrl, Settings.Default.WCFQueueName);

			matcher = new SynonymMatcher(this);

			var y = 5;
			var lineHeight = 23;
			var labelWidth = 100;
			var labelPadding = 7;
			var inputWidth = 27;
			foreach (var field in Fields.AdditionalFields()) {
				var x = (labelWidth +  labelPadding + inputWidth + inputWidth) * 3;
				var controls = pnlTxtFields.Controls;
				var controls2 = pnlGeneralFields.Controls;

				var text = String.Format("{0} :", field.Item2);
				controls.Add(new Label {
					Text = text,
					Size = new Size(labelWidth, lineHeight),
					TextAlign = ContentAlignment.MiddleRight,
					Location = new Point(x, y)
				});

				controls2.Add(new Label {
					Text = text,
					Size = new Size(labelWidth, lineHeight),
					TextAlign = ContentAlignment.MiddleRight,
					Location = new Point(x, y)
				});
				
				x += labelWidth + labelPadding;
				var column = "FR" + Fields.BeginColumn(field.Item1);
				controls.Add(BuildInput(x, y, inputWidth, field.Item1, column));

				column = "FR" + Fields.GeneralColumn(field.Item1);
				controls2.Add(BuildInput(x, y, inputWidth*2, field.Item1, column));

				x += inputWidth;

				column = "FR" + Fields.EndColumn(field.Item1);
				controls.Add(BuildInput(x, y, inputWidth, field.Item1, column));
				y += lineHeight;
			}
		}

		private TextBox BuildInput(int x, int y, int inputWidth, string field, string column)
		{
			var input = new TextBox {
				AccessibleName = field,
				Name = column,
				AllowDrop = true,
				ReadOnly = true,
				DataBindings = {
					new Binding("Text", bsFormRules, column, true),
				},

				Width = inputWidth,
				Location = new Point(x, y)
			};
			input.DragDrop += txtBoxCode_DragDrop;
			input.DragEnter += txtBoxCode_DragEnter;
			input.DoubleClick += txtBoxCode_DoubleClick;
			return input;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			connection.Open();

			_priceFileFormatHelper = new PriceFileFormatHelper(connection);

			command.Connection = connection;
			dataAdapter = new MySqlDataAdapter(command);

			dtPriceFMTsFill();
			cbRegionsFill();
			cbSourceFill();
			connection.Close();

			bsCostsFormRules.SuspendBinding();
			bsFormRules.SuspendBinding();

			tbFirmName.Focus();
			tbFirmName.Select();

			sf.Alignment = StringAlignment.Center;
			sf.FormatFlags = StringFormatFlags.DirectionVertical;
			sf.LineAlignment = StringAlignment.Center;

			LoadFirmAndPriceSettings();
		}

		private void dtPriceFMTsFill()
		{
			dtPriceFMTs.Clear();

			command.CommandText = @"
SELECT 
  id as FMTId,
  Format as FMTFormat,
  FileExtention  as FMTExt
FROM 
  farm.PriceFmts 
order by Format";

			dataAdapter.Fill(dtPriceFMTs);
		}

		private string AddParams(string shname, ulong regcode)
		{
			string cmnd = String.Empty;
			if (!String.IsNullOrEmpty(shname))
				cmnd += " and s.Name like ?ShortName ";
			if (regcode != 0)
				cmnd += " and r.regioncode = ?RegionCode ";
			return cmnd;
		}

		private string AddParams(string shname, ulong regcode, bool showOnlyEnabledFirm, int _sourceIndex)
		{
			string cmnd = String.Empty;
			cmnd += AddParams(shname, regcode);
			if (showOnlyEnabledFirm)
				cmnd += " and s.Disabled = ?FirmStatus ";
			if (_sourceIndex > 0)
				cmnd += " and st.Id = ?sourceIndex ";
			return cmnd;
		}

		private void FillTables(string shname, ulong regcode, int _sourceIndex)
		{
			var filter = new Filter(shname, regcode, _sourceIndex);

			dtSet.EnforceConstraints = false;
			try
			{
				dtClients.Clear();
				dtPrices.Clear();
				dtPricesCost.Clear();
				dtFormRules.Clear();
				dtCostsFormRules.Clear();
			}
			finally
			{
				dtSet.EnforceConstraints = true;
			}
			dtSet.AcceptChanges();

			if (!filter.CanSearch())
				return;

			command.Parameters.Clear();
			if (!String.IsNullOrEmpty(shname))
				command.Parameters.AddWithValue("?ShortName", "%" + shname + "%");
			if (regcode != 0)
				command.Parameters.AddWithValue("?RegionCode", regcode);
			if (_sourceIndex > 0)
				command.Parameters.AddWithValue("?sourceIndex", _sourceIndex);

			var showOnlyEnabled = !_showDisabledFirm;
			if (showOnlyEnabled)
			{
				// Если галочка "Показывать недействующие" НЕ выделена
				// тогда мы добавляем параметры для выборки только действующих
				command.Parameters.AddWithValue("?AgencyEnabled", 1);
				command.Parameters.AddWithValue("?Enabled", 1);
				command.Parameters.AddWithValue("?FirmStatus", 0);
			}

			FilterParams = AddParams(shname, regcode, showOnlyEnabled, _sourceIndex);

			dtClientsFill(FilterParams, showOnlyEnabled);
			if (showOnlyEnabled)
				FilterParams += "and pd.AgencyEnabled = ?AgencyEnabled and pd.Enabled = ?Enabled ";
			dtPricesFill(FilterParams, showOnlyEnabled);
			dtPricesCostFill(FilterParams, showOnlyEnabled);
			dtFormRulesFill(FilterParams, showOnlyEnabled);
			dtCostsFormRulesFill(FilterParams, showOnlyEnabled);
		}

		private void FillCosts(string shname, ulong regcode)
		{
			dtSet.EnforceConstraints = false;
			try
			{
				dtCostsFormRules.Clear();
				dtPricesCost.Clear();
			}
			finally
			{
				dtSet.EnforceConstraints = true;
			}
			dtSet.AcceptChanges();

			if (shname != String.Empty)
			{

				command.Parameters.Clear();

				if (shname != String.Empty)
					command.Parameters.AddWithValue("?ShortName", "%" + shname + "%");
				if (regcode != 0)
					command.Parameters.AddWithValue("?RegionCode", regcode);

				bool showOnlyEnabled = !_showDisabledFirm;
				if (showOnlyEnabled)
				{
					// Если галочка "Показывать недействующие" НЕ выделена
					// тогда мы добавляем параметры для выборки только действующих
					command.Parameters.AddWithValue("?AgencyEnabled", 1);
					command.Parameters.AddWithValue("?Enabled", 1);
					command.Parameters.AddWithValue("?FirmStatus", 0);
				}

				FilterParams = AddParams(shname, regcode, showOnlyEnabled, sourceIndex);
				if (showOnlyEnabled)
					FilterParams += "and pd.AgencyEnabled = ?AgencyEnabled and pd.Enabled = ?Enabled ";

				dtPricesCostFill(FilterParams, !_showDisabledFirm);
				dtCostsFormRulesFill(FilterParams, !_showDisabledFirm);
			}
		}
		
		private void dtClientsFill(string param, bool showOnlyEnabled)
		{
			if (showOnlyEnabled)
			{
				command.CommandText = @"
SELECT s.Id AS CCode,
s.Name AS CShortName,
s.FullName AS CFullName,
r.Region AS CRegion,
st.Type AS CSourceIndex
FROM Customers.suppliers s
INNER JOIN farm.regions r ON r.RegionCode = s.HomeRegion
join usersettings.pricesdata PD on s.Id = PD.firmcode
join usersettings.pricescosts PC on pc.pricecode = pd.pricecode
join usersettings.priceitems pi on pc.priceitemid = pi.id
join Farm.Sources so on so.id = pi.SourceId
join farm.sourcetypes st on st.id = so.SourceTypeId
WHERE datediff(curdate(), date(pi.pricedate)) < 200 AND (PD.Enabled = 1)
AND (PD.AgencyEnabled = 1) 
 ";
			}
			else
			{
				command.CommandText = @"
SELECT s.Id AS CCode,
s.Name AS CShortName,
s.FullName AS CFullName,
r.Region AS CRegion,
st.Type AS CSourceIndex
FROM Customers.Suppliers s
join usersettings.pricesdata PD on s.Id = PD.firmcode
join usersettings.pricescosts PC on pc.pricecode = pd.pricecode
join usersettings.priceitems pi on pc.priceitemid = pi.id
join Farm.Sources so on so.id = pi.SourceId
join farm.sourcetypes st on st.id = so.SourceTypeId
INNER JOIN farm.regions r ON r.RegionCode = s.HomeRegion
WHERE 1=1 ";
			}
			command.CommandText += param;
			command.CommandText += " group by s.id ORDER BY s.Name";
			dataAdapter.Fill(dtClients);
		}
		
		private void dtPricesFill(string param, bool showOnlyEnabled)
		{
			var sqlPart = String.Empty;
			if (showOnlyEnabled)
				sqlPart += @" and (datediff(curdate(), date(pim.pricedate)) < 200) ";
			// Выбираем прайс-листы с мультиколоночными ценами

			command.CommandText =
@"
SELECT
  pd.FirmCode as PFirmCode,
  pim.Id as PPriceItemId,
  pd.PriceCode as PPriceCode,
  if(pd.CostType = 1, concat(pd.PriceName, ' [Колонка] ', pc.CostName), pd.PriceName) as PPriceName,
  pim.PriceDate as PPriceDate,
  pim.LastFormalization as PDateLastForm,
  fr.MaxOld as PMaxOld,
  pd.PriceType as PPriceType,
  pd.CostType as PCostType,
  pim.WaitingDownloadInterval as PWaitingDownloadInterval,
  -- редактировать тип ценовой колонки и тип прайс-листа можно только относительно базовой ценовой колонки
  if(pc.BaseCost = 1, 1, 0) PIsParent,
  pc.BaseCost as PBaseCost,
  pc.CostCode as PCostCode	
FROM
  usersettings.pricesdata pd
  inner join usersettings.pricescosts pc on pc.pricecode = pd.pricecode
  inner join usersettings.PriceItems pim on (pim.Id = pc.PriceItemId) 
"
+ sqlPart +
@"	
  inner join Customers.suppliers s on s.Id = pd.FirmCode
  inner join farm.formrules fr on fr.Id = pim.FormRuleId
  inner join farm.regions r on r.regioncode=s.HomeRegion
	join Farm.Sources so on so.id = pim.SourceId
join farm.sourcetypes st on st.id = so.SourceTypeId
where     
 ((pd.CostType = 1) or (pc.BaseCost = 1)) ";
			command.CommandText += param;
			command.CommandText += @" 
group by pim.Id
order by PPriceName";
			dataAdapter.Fill(dtPrices);
		}

		private void cbRegionsFill()
		{
			DataTable dtRegions = new DataTable("Region ");
			dtRegions.Columns.Add("RegionCode", typeof(ulong));
			dtRegions.Columns.Add("Region", typeof(string));
			dtRegions.Clear();
			dtRegions.Rows.Add(new object[] { 0, "Все" });
			command.CommandText = @"
SELECT
	RegionCode,
	Region
FROM
	Regions
WHERE regionCode > 0
Order By Region
";
			dataAdapter.Fill(dtRegions);

			cbRegions.DataSource = dtRegions;
			cbRegions.DisplayMember = "Region";
			cbRegions.ValueMember = "RegionCode";

		}

		private void cbSourceFill()
		{
			var dtSource = new DataTable("Source");
			dtSource.Columns.Add("Id", typeof(ulong));
			dtSource.Columns.Add("Type", typeof(string));
			dtSource.Clear();
			dtSource.Rows.Add(new object[] { 0, "Все" });
			command.CommandText = @"
SELECT
	Id,
	Type
FROM
	farm.sourcetypes";
			dataAdapter.Fill(dtSource);
			cbSource.DataSource = dtSource;
			cbSource.DisplayMember = "Type";
			cbSource.ValueMember = "Id";
		}


		
		private void dtPricesCostFill(string param, bool showOnlyEnabled)
		{
			string sqlPart = String.Empty;
			if (showOnlyEnabled)
				sqlPart +=
@"
and pc.PriceItemId in (
		SELECT Id FROM usersettings.priceitems
		WHERE (datediff(curdate(), date(pricedate)) < 200))
";

			command.CommandText =
@"
select
  pc.PriceItemId as PCPriceItemId,
  pc.PriceCode as PCPriceCode,
  pc.CostCode as PCCostCode,
  pc.BaseCost as PCBaseCost,
  pc.CostName as PCCostName
from
  usersettings.pricescosts pc
  inner join usersettings.pricesdata pd on pd.pricecode = pc.pricecode

  inner join usersettings.PriceItems pim on pim.Id = pc.PriceItemId
	join Farm.Sources so on so.id = pim.SourceId
	join farm.sourcetypes st on st.id = so.SourceTypeId

  inner join Customers.suppliers s on s.Id = pd.firmcode
  inner join farm.regions r on r.regioncode=s.HomeRegion
where
	  (1=1) 
" + sqlPart;
			command.CommandText += param;
			command.CommandText += @"  
order by PCCostName";
			try
			{
				dataAdapter.Fill(dtPricesCost);
			}
			catch (Exception ex)
			{
				var constraints = "";
				foreach (Constraint constr in dtPricesCost.Constraints)
				{
					constraints += String.Format("Constraint Name: {0}, Type: {1};",
						constr.ConstraintName, constr.GetType().ToString());
				}
				var parameters = "";
				foreach (MySqlParameter sqlparam in command.Parameters)
				{
					parameters += String.Format("SQL ParamName: {0}, Value: {1};",
						sqlparam.ParameterName, sqlparam.Value);
				}
				string message = String.Format(@"
Ошибка при применении изменений в правилах формализации. dtPricesCostFill(). Параметры SQL запроса: '{0}'. Ограничения: '{1}'",
					parameters, constraints);
				throw new CustomConstraintException(message, ex);
			}
		}
		
		private void dtCostsFormRulesFill(string param, bool showOnlyEnabled)
		{
			string sqlPart = String.Empty;
			if (showOnlyEnabled)
				sqlPart +=
@"
	and pc.PriceItemId in (
		SELECT Id 
		FROM usersettings.PriceItems 
		WHERE (datediff(curdate(), date(pricedate)) < 200))		
";

			command.CommandText =
@"
select
  pc.PriceItemId as CFRPriceItemId,
  pc.CostName as CFRCostName,
  cfr.CostCode AS CFRCost_Code,
  cfr.FieldName AS CFRFieldName,
  cfr.TxtBegin as CFRTextBegin,
  cfr.TxtEnd as CFRTextEnd,
  pc.BaseCost as CFRBaseCost
FROM 
  farm.costformrules cfr
  inner join usersettings.pricescosts pc on (pc.CostCode = cfr.costcode) 
"
+ sqlPart +
@"
  inner join usersettings.PriceItems pim on pim.Id = pc.PriceItemId
	join Farm.Sources so on so.id = pim.SourceId
	join farm.sourcetypes st on st.id = so.SourceTypeId
  inner join usersettings.pricesdata pd on pd.pricecode = pc.pricecode
  inner join Customers.suppliers s on s.Id = pd.firmcode
  inner join farm.regions r on r.regioncode=s.HomeRegion
where 
  pd.CostType is not null
";

			command.CommandText += param;
			command.CommandText += @"   
order by CFRCostName";

			dataAdapter.Fill(dtCostsFormRules);
		}

		private void dtFormRulesFill(string param, bool showOnlyEnabled)
		{
			string sqlPart = String.Empty;
			if (showOnlyEnabled)
				sqlPart += @"and (datediff(curdate(), date(pim.pricedate)) < 200)";

			var sql = Fields.Columnds().Implode(f => String.Format("PFR.{0} as FR{0}", f));

			command.CommandText = String.Format(@"
SELECT
	fr.Id as FRFormID,

	pim.Id As FRPriceItemId,
	
	pd.PriceCode as FRSelfPriceCode,
	pd.ParentSynonym AS FRSynonyms,

	PFR.PriceFormatId as FRPriceFormatId,

	FR.Memo As FRMemo,
	if(pd.CostType = 1, concat(pd.PriceName, ' [Колонка] ', pc.CostName), PD.PriceName) AS FRName,
	FR.JunkPos AS FRSelfJunkPos,
	FR.AwaitPos AS FRSelfAwaitPos,
	FR.VitallyImportantMask as FRSelfVitallyImportantMask,

	pim.RowCount AS FRPosNum,

	pfmt.FileExtention as FRExt,

	PFR.Delimiter As FRDelimiter,
	PFR.StartLine AS FRStartLine,
	PFR.ListName as FRListName,
	PFR.NameMask as FRNameMask,
	PFR.ForbWords as FRForbWords,


	PFR.TxtCodeBegin as FRTxtCodeBegin,
	PFR.TxtCodeEnd as FRTxtCodeEnd,

	PFR.TxtCodeCrBegin as FRTxtCodeCrBegin,
	PFR.TxtCodeCrEnd as FRTxtCodeCrEnd,

	PFR.TxtNameBegin as FRTxtNameBegin,
	PFR.TxtNameEnd as FRTxtNameEnd,

	PFR.TxtFirmCrBegin as FRTxtFirmCrBegin,
	PFR.TxtFirmCrEnd as FRTxtFirmCrEnd,

	PFR.TxtMinBoundCostBegin as FRTxtMinBoundCostBegin,
	PFR.TxtMinBoundCostEnd as FRTxtMinBoundCostEnd,

	PFR.TxtUnitBegin as FRTxtUnitBegin,
	PFR.TxtUnitEnd as FRTxtUnitEnd,

	PFR.TxtVolumeBegin as FRTxtVolumeBegin,
	PFR.TxtVolumeEnd as FRTxtVolumeEnd,

	PFR.TxtQuantityBegin as FRTxtQuantityBegin,
	PFR.TxtQuantityEnd as FRTxtQuantityEnd,

	PFR.TxtNoteBegin as FRTxtNoteBegin,
	PFR.TxtNoteEnd as FRTxtNoteEnd,

	PFR.TxtPeriodBegin as FRTxtPeriodBegin,
	PFR.TxtPeriodEnd as FRTxtPeriodEnd,

	PFR.TxtDocBegin as FRTxtDocBegin,
	PFR.TxtDocEnd as FRTxtDocEnd,

	PFR.TxtJunkBegin as FRTxtJunkBegin,
	PFR.TxtJunkEnd as FRTxtJunkEnd,

	PFR.TxtAwaitBegin as FRTxtAwaitBegin,
	PFR.TxtAwaitEnd as FRTxtAwaitEnd,

	PFR.TxtRequestRatioBegin as FRTxtRequestRatioBegin,
	PFR.TxtRequestRatioEnd as FRTxtRequestRatioEnd,

	PFR.TxtRegistryCostBegin as FRTxtRegistryCostBegin,
	PFR.TxtRegistryCostEnd as FRTxtRegistryCostEnd,

	PFR.TxtVitallyImportantBegin as FRTxtVitallyImportantBegin,
	PFR.TxtVitallyImportantEnd as FRTxtVitallyImportantEnd,

	PFR.TxtMaxBoundCostBegin as FRTxtMaxBoundCostBegin,
	PFR.TxtMaxBoundCostEnd as FRTxtMaxBoundCostEnd,

	PFR.TxtOrderCostBegin as FRTxtOrderCostBegin,
	PFR.TxtOrderCostEnd as FRTxtOrderCostEnd,

	PFR.TxtMinOrderCountBegin as FRTxtMinOrderCountBegin,
	PFR.TxtMinOrderCountEnd as FRTxtMinOrderCountEnd,

	PFR.TxtProducerCostBegin as FRTxtProducerCostBegin,
	PFR.TxtProducerCostEnd as FRTxtProducerCostEnd,

	PFR.TxtNdsBegin as FRTxtNdsBegin,
	PFR.TxtNdsEnd as FRTxtNdsEnd,

	PFR.FCode as FRFCode,
	PFR.FCodeCr as FRFCodeCr,
	PFR.FName1 as FRFName1,
	PFR.FName2 as FRFName2,
	PFR.FName3 as FRFName3,
	PFR.FFirmCr as FRFFirmCr,
	PFR.FMinBoundCost as FRFMinBoundCost,
	PFR.FUnit as FRFUnit,
	PFR.FVolume as FRFVolume,
	PFR.FQuantity as FRFQuantity,
	PFR.FNote as FRFNote,
	PFR.FPeriod as FRFPeriod,
	PFR.FDoc as FRFDoc,
	PFR.FJunk as FRFJunk,
	PFR.FAwait as FRFAwait,
	PFR.FRequestRatio as FRFRequestRatio,
	PFR.FRegistryCost as FRFRegistryCost,
	PFR.FVitallyImportant as FRFVitallyImportant,
	PFR.FMaxBoundCost as FRFMaxBoundCost,
	PFR.FOrderCost as FRFOrderCost,
	PFR.FMinOrderCount as FRFMinOrderCount,
	PFR.FProducerCost as FRFProducerCost,
	PFR.FNds as FRFNds,
	{0}
FROM 
  UserSettings.PricesData AS PD
  INNER JOIN Customers.suppliers s on s.Id = pd.FirmCode
  INNER JOIN farm.regions r on r.regioncode=s.HomeRegion
  inner join usersettings.pricescosts pc on pc.PriceCode = pd.PriceCode
  inner join usersettings.priceitems pim on pim.Id = pc.PriceItemId 
"
+ sqlPart +
@"	
join Farm.Sources so on so.id = pim.SourceId
join farm.sourcetypes st on st.id = so.SourceTypeId
  inner join Farm.formrules AS FR ON FR.Id = pim.FormRuleId
  left join Farm.FormRules AS PFR ON PFR.id = FR.Id
  left join Farm.PriceFMTs as pfmt on pfmt.id = PFR.PriceFormatId
where
  ((pd.CostType = 1) or (pc.BaseCost = 1)) ", sql);
			command.CommandText += param;
			dataAdapter.Fill(dtFormRules);
		}

		private void Form1_Closed(object sender, EventArgs e)
		{
			connection.Close();
		}

		private void DoOpenPrice(DataRow drP)
		{
			fW = new frmWait();
			try
			{
				fW.openPrice += OpenPrice;
				fW.drP = drP;
				fW.ShowDialog();
			}
			finally
			{
				fW = null;
			}
		}

		public delegate void OpenPriceDelegate(DataRow drP);

		// Открывает прайс
		private void OpenPrice(DataRow drP)
		{
			ClearPrice();
			Application.DoEvents();
			// Выбираем правила для текущего priceItemId
			var drFR = dtFormRules.Select("FRPriceItemId = " + drP[PPriceItemId.ColumnName]);
			var drC = drP.GetParentRow(dtClients.TableName + "-" + dtPrices.TableName);
			frmCaption = String.Format("{0}; {1}", drC["CShortName"], drC["CRegion"]);
			var shortFileNameByPriceItemId = drFR[0]["FRPriceItemId"].ToString();
			fmt = _priceFileFormatHelper.NewFormat;
			//Делаем фильтрацию по форматам прайс-листа
			var cm = ((CurrencyManager)cmbFormat.BindingContext[dtSet, "Форматы прайса"]);			
			if ((cm != null) && (cm.List is DataView))
				if (fmt.HasValue)
				{
					bsFormRules.SuspendBinding();
					switch (fmt)
					{
						case PriceFormat.DBF:
							//Запрещаем устаревшие текстовые форматы, NativeDBF и XLS
							((DataView)cm.List).RowFilter = "not (FmtId in (1, 2, 3, 6, 7, 8))";
							break;

						case PriceFormat.NativeXls:
							//Запрещаем устаревшие текстовые форматы, DBF и XLS
							((DataView)cm.List).RowFilter = "not (FmtId in (1, 2, 3, 4, 6, 7))";
							break;

						case PriceFormat.XLS:
								//Запрещаем устаревшие текстовые форматы, DBF и NativeXls								
								((DataView) cm.List).RowFilter = "not (FmtId in (1, 2, 10, 4, 6, 7))";								
								break;

						case PriceFormat.DelimDOS:
						case PriceFormat.DelimWIN:
						case PriceFormat.FixedWIN:
						case PriceFormat.FixedDOS:
							//Запрещаем новые текстовые форматы, DBF и XLS
							((DataView)cm.List).RowFilter = "not (FmtId in (3, 4, 11, 12, 13, 14))";
							break;

						default:
							{
								//Запрещаем устаревшие текстовые форматы, DBF и XLS
								((DataView)cm.List).RowFilter = "not (FmtId in (1, 2, 3, 4, 6, 7))";
								break;
							}
					}
					bsFormRules.ResumeBinding();
				}
				else
				{
					bsFormRules.SuspendBinding();
					//Запрещаем устаревшие текстовые форматы, DBF и XLS
					((DataView) cm.List).RowFilter = "not (FmtId in (1, 2, 4, 6, 7, 3))";
					bsFormRules.SuspendBinding();
				}

			FillParentComboBox(
				cmbParentSynonyms,
				@"
select
  pd.PriceCode,
  concat(s.Name, ' (', pd.PriceName, ') - ', r.Region) PriceName
from
  Customers.suppliers s,
  usersettings.pricesdata pd,
  farm.regions r
where
  pd.FirmCode = s.Id
and pd.CostType is not null
and r.RegionCode = s.HomeRegion
and pd.PriceCode = ?ParentValue
order by PriceName
",
				drFR[0]["FRSynonyms"],
				"PriceCode",
				"PriceName");            

			delimiter = _priceFileFormatHelper.NewDelimiter; 

			string takeFile = _priceFileFormatHelper.NewShortFileName;

			PrepareShowTab(fmt);

			fileExist = false;

			// Если формат не установлен, то больше ничего не делаем
			if (fmt != null)
			{
				try
				{
					fileExist = true;
					var filePath = EndPath + Convert.ToString(currentPriceItemId) + Path.DirectorySeparatorChar + takeFile;

					if (!_isFormatChanged)
					{
						Directory.CreateDirectory(EndPath + shortFileNameByPriceItemId);
						if (!LoadFileFromBase(shortFileNameByPriceItemId, filePath))
							return;
					}
					Application.DoEvents();
					_currentFilename = filePath;
					_dataTableMarking.Fill(drFR[0], dtCostsFormRules.Select("CFRPriceItemId = " + drP[PPriceItemId.ColumnName]));
					var tables = PriceFileHelper.OpenPriceFile(filePath, fmt, delimiter, _dataTableMarking);
					if (tables == null)
					{
						MessageBox.Show("Неправильный формат или файл поврежден", "Ошибка открытия файла", MessageBoxButtons.OK,
										MessageBoxIcon.Error);
						return;
					}
					dtPrice = tables[0]; 
					if ((fmt == PriceFormat.XLS) || (fmt == PriceFormat.NativeXls))
					{
						tbpSheet1.Text = tables[0].TableName;						
						tables.RemoveAt(0);
						dtables = tables;
						SetupXlsPriceView();
					}						
					Application.DoEvents();
					ShowTab(fmt);
					Application.DoEvents();
					Text = String.Format("Редактор Правил Формализации ({0})", frmCaption);
					Application.DoEvents();

				}
				catch (PriceProcessorException priceProcessorException)
				{
					MessageBox.Show(priceProcessorException.Message, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}

		private bool LoadFileFromBase(string shortFileNameByPriceItemId, string filePath)
		{
			try
			{
#if !DEBUG
				// Берем файл из Base
				using (var openFile = _priceProcessor.BaseFile(Convert.ToUInt32(shortFileNameByPriceItemId)))
				{
					if (openFile != null)
					{
						using (var fileStream = File.Create(filePath))
						{
							openFile.CopyTo(fileStream);
							return true;
						}
					}
					else
					{
						MessageBox.Show(_priceProcessor.LastErrorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
				}
#else
				var dataDir = @"..\..\..\..\FREditor.Test\Data\prices";
				var extention = Path.GetExtension(filePath);
				var files = Directory.GetFiles(dataDir, "*" + extention);
				if (files.Length > 0)
				{
					File.Copy(files[0], filePath, true);
					return true;
				}
				return false;
#endif
			}
			catch (Exception ex)
			{
				_logger.Error("Ошибка при попытке получить файл из Base", ex);
				return false;
			}
		}

		private void SetupXlsPriceView()
		{
			for (var i = 0; i < dtables.Count; i++)
			{
				var table = dtables[i];
				var tabPage = new TabPage(table.TableName);
				tabPage.Name = "tbpSheet" + (i + 1);			
				tcInnerSheets.TabPages.Add(tabPage);

				var indgv = new INDataGridView();
				indgv.Name = "indgvPriceData" + (i + 1);
				indgv.Parent = tabPage;
				indgv.KeyDown += indgvPriceData_KeyDown;
				indgv.Dock = DockStyle.Fill;
				indgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
				indgv.ReadOnly = true;
				indgv.RowHeadersVisible = false;
				indgv.MouseDown += indgvPriceData_MouseDown;
				indgv.KeyPress += indgvPriceData_KeyPress;
				foreach (DataGridViewTextBoxColumn dc in indgv.Columns)
					dc.Width = 300;
				gds.Add(indgv);
				((INDataGridView) gds[i]).Columns.Clear();
				((INDataGridView) gds[i]).DataSource = dtables[i];
			}
		}

		private void PrepareShowTab(PriceFormat? fmt)
		{
			switch (fmt)
			{
				case PriceFormat.DelimDOS:
				case PriceFormat.DelimWIN:
				case PriceFormat.NativeDelimWIN:
				case PriceFormat.NativeDelimDOS:
					
					lBoxSheetName.Visible = false;
					txtBoxSheetName.Visible = false;
					pnlGeneralFields.Visible = true;
					pnlTxtFields.Visible = false;

					break;

				case PriceFormat.FixedDOS:
				case PriceFormat.FixedWIN:
				case PriceFormat.NativeFixedWIN:
				case PriceFormat.NativeFixedDOS:

					lBoxSheetName.Visible = false;
					txtBoxSheetName.Visible = false;
					pnlGeneralFields.Visible = false;
					pnlTxtFields.Visible = true;

					break;

				case PriceFormat.XLS:
				case PriceFormat.NativeXls:

					lBoxSheetName.Visible = true;
					txtBoxSheetName.Visible = true;
					pnlGeneralFields.Visible = true;
					pnlTxtFields.Visible = false;

					break;

				case PriceFormat.DBF:
				case PriceFormat.NativeDbf:					

					lBoxSheetName.Visible = false;
					txtBoxSheetName.Visible = false;
					pnlGeneralFields.Visible = true;
					pnlTxtFields.Visible = false;

					break;
			}

			txtBoxSelfAwaitPos.ReadOnly = false;
			txtBoxSelfJunkPos.ReadOnly = false;
			txtBoxNameMask.ReadOnly = false;
			txtBoxForbWords.ReadOnly = false;
		}

		private void FillParentComboBox(ComboBox cmbParent, string FillSQL, object ParentValue, string IdField, string NameField)
		{
			DataSet ParentDS = MySqlHelper.ExecuteDataset(connection, FillSQL, new MySqlParameter("?ParentValue", ParentValue));
			FillParentComboBoxFromTable(cmbParent, ParentDS.Tables[0], IdField, NameField);
		}

		private void FillParentComboBoxBySearch(ComboBox cmbParent, string FillSQL, string IdField, string NameField)
		{
			//Возможно есть способ более проще получить значение биндинга
			object SelectedValue = ((DataRowView)bsFormRules.Current).Row[cmbParent.DataBindings["SelectedValue"].BindingMemberInfo.BindingField];
			DataSet ParentDS = MySqlHelper.ExecuteDataset(
				connection, 
				FillSQL, 
				new MySqlParameter("?PrevParentValue", SelectedValue), 
				new MySqlParameter("?SearchText", "%" + cmbParent.Text + "%"));
			FillParentComboBoxFromTable(cmbParent, ParentDS.Tables[0], IdField, NameField);
		}

		private void FillParentComboBoxFromTable(ComboBox cmbParent, DataTable dtParent, string IdField, string NameField)
		{
			bsFormRules.SuspendBinding();
			try
			{
				cmbParent.DataSource = null;
				cmbParent.Items.Clear();
				DataRow drNull = dtParent.NewRow();
				drNull[IdField] = DBNull.Value;
				drNull[NameField] = "<не установлены>";
				dtParent.Rows.InsertAt(drNull, 0);
				cmbParent.Items.Add(drNull);
				cmbParent.DataSource = dtParent;
				cmbParent.DisplayMember = NameField;
				cmbParent.ValueMember = IdField;
			}
			finally
			{
				bsFormRules.ResumeBinding();
			}
		}

		private void ShowTab(PriceFormat? fmt)
		{
			tcInnerTable.Visible = true;
			
			indgvPriceData.DataSource = dtPrice;
			switch (fmt)
			{
				case PriceFormat.DelimDOS:
				case PriceFormat.DelimWIN:
				case PriceFormat.NativeDelimWIN:
				case PriceFormat.NativeDelimDOS:
					tcInnerTable.SizeMode = TabSizeMode.Fixed;
					tcInnerTable.ItemSize = new Size(0, 1);
					tcInnerTable.Appearance = TabAppearance.Buttons;

					indgvCosts.Columns[cFRFieldNameDataGridViewTextBoxColumn.Name].Visible = true;

					tcInnerSheets.SizeMode = TabSizeMode.Fixed;
					tcInnerSheets.ItemSize = new Size(0, 1);
					tcInnerSheets.Appearance = TabAppearance.FlatButtons;

					break;

				case PriceFormat.FixedDOS:
				case PriceFormat.FixedWIN:
				case PriceFormat.NativeFixedWIN:
				case PriceFormat.NativeFixedDOS:
					tcInnerTable.SizeMode = TabSizeMode.Normal;
					tcInnerTable.ItemSize = new Size(58, 18);
					tcInnerTable.Appearance = TabAppearance.Normal;

					indgvCosts.Columns[cFRTextBeginDataGridViewTextBoxColumn.Name].Visible = true;
					indgvCosts.Columns[cFRTextEndDataGridViewTextBoxColumn.Name].Visible = true;

					tcInnerSheets.SizeMode = TabSizeMode.Fixed;
					tcInnerSheets.ItemSize = new Size(0, 1);
					tcInnerSheets.Appearance = TabAppearance.FlatButtons;

					break;

				case PriceFormat.XLS:
				case PriceFormat.NativeXls:
					tcInnerTable.SizeMode = TabSizeMode.Fixed;
					tcInnerTable.ItemSize = new Size(0, 1);
					tcInnerTable.Appearance = TabAppearance.Buttons;

					tcInnerSheets.SizeMode = TabSizeMode.Normal;
					tcInnerSheets.ItemSize = new Size(58, 18);
					tcInnerSheets.Appearance = TabAppearance.Normal;

					break;

				case PriceFormat.DBF:
				case PriceFormat.NativeDbf:
					tcInnerTable.SizeMode = TabSizeMode.Fixed;
					tcInnerTable.ItemSize = new Size(0, 1);
					tcInnerTable.Appearance = TabAppearance.Buttons;

					tcInnerSheets.SizeMode = TabSizeMode.Fixed;
					tcInnerSheets.ItemSize = new Size(0, 1);
					tcInnerSheets.Appearance = TabAppearance.Buttons;

					break;
			}

			LoadCostsSettings();
			indgvPriceData.Focus();
			indgvPriceData.Select();
		}

		private void DelCostsColumns()
		{
			indgvCosts.Columns[cFRFieldNameDataGridViewTextBoxColumn.Name].Visible = false;
			indgvCosts.Columns[cFRTextBeginDataGridViewTextBoxColumn.Name].Visible = false;
			indgvCosts.Columns[cFRTextEndDataGridViewTextBoxColumn.Name].Visible = false;
		}

		public string strToANSI(string Dest)
		{
			Encoding ansi = System.Text.Encoding.GetEncoding(1251);
			byte[] unicodeBytes = System.Text.Encoding.Unicode.GetBytes(Dest);
			byte[] ansiBytes = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, ansi, unicodeBytes);
			return ansi.GetString(ansiBytes);
		}

		private void tcInnerSheets_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (tcInnerSheets.SelectedTab == tbpSheet1)
			{
				indgvPriceData.DataSource = dtPrice;
			}
			else
			{
				((INDataGridView)gds[tcInnerSheets.SelectedIndex - 1]).DataSource = ((DataTable)dtables[tcInnerSheets.SelectedIndex - 1]);
			}
		}

		private void tcInnerTable_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (tcInnerTable.SelectedTab == tbpTable)
			{
				SaveMarkingSettings();
				DataTable dtTemp = _dataTableMarking.GetChanges();
				if (dtTemp != null)
				{
					if (!_dataTableMarking.Check())
					{
						tcInnerTable.SelectedTab = tbpMarking;
						MessageBox.Show("Неправильный ввод поля!", "Плохо, очень плохо", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					else
					{
						fW = new frmWait();

						try
						{
							fW.openPrice += DoOpenTable;
							fW.drP = dtTemp.Rows[0];// new DataRow();

							fW.ShowDialog();
						}
						finally
						{
							fW = null;
						}


					}
				}
			}
			else if (tcInnerTable.SelectedTab == tbpMarking)
			{
				LoadMarkingSettings();
			}
		}

		private void DoOpenTable(DataRow drFict)
		{
			_dataTableMarking.AcceptChanges();
			Application.DoEvents();
			indgvPriceData.DataSource = null;
			Application.DoEvents();

			dtPrice = PriceFileHelper.OpenPriceTable(_currentFilename, _dataTableMarking, _priceFileFormatHelper.NewFormat);// OpenTable(fmt);
			Application.DoEvents();

			indgvPriceData.DataSource = dtPrice;
			Application.DoEvents();
			Application.DoEvents();
		}

		private void ClearPrice()
		{
			indgvPriceData.DataSource = null;

			tcInnerSheets.SizeMode = TabSizeMode.Normal;
			tcInnerSheets.ItemSize = new Size(0, 1);
			tcInnerSheets.Appearance = TabAppearance.Normal;

			tcInnerTable.SizeMode = TabSizeMode.Normal;
			tcInnerTable.ItemSize = new Size(0, 1);
			tcInnerTable.Appearance = TabAppearance.Normal;//

			tcInnerSheets.SelectedTab = tbpSheet1;
			tcInnerTable.SelectedTab = tbpTable;

			foreach (TabPage tp in tcInnerSheets.TabPages)
			{
				if (!(tp.Equals(tbpSheet1)))
				{
					tcInnerSheets.TabPages.Remove(tp);
				}
			}

			indgvPriceData.Columns.Clear();

			gds.Clear();
			dtables.Clear();
			tblstyles.Clear();
			_dataTableMarking.Clear();

			dtPrice.Rows.Clear();
			dtPrice.Columns.Clear();
			dtPrice.Clear();

			DelCostsColumns();

			lBoxSheetName.Visible = false;
			txtBoxSheetName.Visible = false;

			txtBoxSelfAwaitPos.ReadOnly = true;
			txtBoxSelfJunkPos.ReadOnly = true;
			txtBoxNameMask.ReadOnly = true;
			txtBoxForbWords.ReadOnly = true;

			tcInnerTable.Visible = false;
		}
		
		public void RefreshDataSet()
		{
			dtSet.EnforceConstraints = false;
			try
			{
				dtSet.Clear();
			}
			finally
			{
				dtSet.EnforceConstraints = true;
			}
			bool showOnlyEnabled = !_showDisabledFirm;
			if (showOnlyEnabled)
			{
				// Если галочка "Показывать недействующие" НЕ выделена
				// тогда мы добавляем параметры для выборки только действующих
				command.Parameters.AddWithValue("?AgencyEnabled", 1);
				command.Parameters.AddWithValue("?Enabled", 1);
				command.Parameters.AddWithValue("?FirmStatus", 0);
			}

			FilterParams = AddParams(String.Empty, 0, showOnlyEnabled, sourceIndex);

			dtClientsFill(FilterParams, showOnlyEnabled);
			if (showOnlyEnabled)
				FilterParams += "and pd.AgencyEnabled = ?AgencyEnabled and pd.Enabled = ?Enabled ";
			dtPricesFill(FilterParams, showOnlyEnabled);
			dtPricesCostFill(FilterParams, showOnlyEnabled);
			dtFormRulesFill(FilterParams, showOnlyEnabled);
			dtPriceFMTsFill();
			dtCostsFormRulesFill(FilterParams, showOnlyEnabled);
			cbRegionsFill();
			cbSourceFill();
			dtSet.AcceptChanges();
		}

		private void CurrencyManagerPosition(CurrencyManager cm, string ColName, object value)
		{
			DataView dataView = (DataView)cm.List;
			string query;
			if (value is string)            
				query = string.Format("{0} = '{1}'", ColName, value);
			else
				query = string.Format("{0} = {1}", ColName, value);

			DataTable dataTable = dataView.ToTable();
			DataRow[] rows = dataTable.Select(query);
			if (rows.Length > 0)
			{
				DataRow[] tempRows;
				tempRows = new DataRow[dataTable.Rows.Count];
				dataTable.Rows.CopyTo(tempRows, 0);
				int rowIndex = Array.IndexOf(tempRows, rows[0]);
				cm.Position = rowIndex;
			}
		}

		private void tbControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			MatchPriceButton.Enabled = true;
			if (tbControl.SelectedTab == tpFirms)
			{
				if (_isComboBoxFormatHandlerRegistered)
					_isComboBoxFormatHandlerRegistered = !_isComboBoxFormatHandlerRegistered;
				fmt = null;
				delimiter = String.Empty;
				SaveCostsSettings();
				bsCostsFormRules.Filter = String.Empty;
				bsFormRules.Filter = String.Empty;
				bsCostsFormRules.SuspendBinding();
				bsFormRules.SuspendBinding();

				RefreshDataBind();
				currentClientCode = 0;
				currentPriceItemId = 0;
				tsbApply.Enabled = false;
				tsbCancel.Enabled = false;
//                tmrUpdateApply.Stop();
				tmrUpdateApply.Start();
			}
			else
				if (tbControl.SelectedTab == tpPrice)
				{
					CurrencyManager currencyManager = (CurrencyManager)BindingContext[indgvPrice.DataSource, indgvPrice.DataMember];
					DataRowView drv = (DataRowView)currencyManager.Current;
					DataView dv = (DataView)currencyManager.List;

					DataRow drP = drv.Row;

					currentClientCode = (long)(((DataRowView)indgvFirm.CurrentRow.DataBoundItem)[CCode.ColumnName]);
					currentPriceItemId = (long)(((DataRowView)indgvPrice.CurrentRow.DataBoundItem)[PPriceItemId.ColumnName]);

					var row = dtFormRules.Select("FRPriceItemId = " + drP[PPriceItemId.ColumnName].ToString());
					var priceFormat = (row[0]["FRPriceFormatId"] is DBNull) ? null : (PriceFormat?)Convert.ToInt32(row[0]["FRPriceFormatId"]);
					var delim = Convert.ToString(row[0]["FRDelimiter"]);
					_priceFileFormatHelper.LoadPriceFormat((ulong)currentPriceItemId, priceFormat, delim);

					bsCostsFormRules.Filter = "CFRPriceItemId = " + currentPriceItemId.ToString();
					bsFormRules.Filter = "FRPriceItemId = " + currentPriceItemId.ToString();
					tbCostFind.Text = String.Empty;
					bsCostsFormRules.ResumeBinding();
					bsFormRules.ResumeBinding();

					if (((DataRowView)indgvPrice.CurrentRow.DataBoundItem)[PCostType.ColumnName] is DBNull)
						indgvCosts.AllowUserToAddRows = false;

					indgvCosts.ReadOnly = !indgvCosts.AllowUserToAddRows;

					btnPutToBase.Enabled = !Convert.IsDBNull(
						((DataRowView)bsFormRules.Current).Row[FRPriceFormatId.ColumnName, DataRowVersion.Original]);

					if (indgvCosts.AllowUserToAddRows)
					{
						(bsCostsFormRules.List as DataView).Table
							.Columns[CFRPriceItemId.ColumnName]
							.DefaultValue = ((DataRowView)bsFormRules.Current)["FRPriceItemId"];
					}
					else
					{
						(bsCostsFormRules.List as DataView).Table
							.Columns[CFRPriceItemId.ColumnName]
							.DefaultValue = DBNull.Value;
					}
					fmt = null;
					//_prevFmt = null;
					delimiter = String.Empty;
					//_prevDelimiter = String.Empty;
					DoOpenPrice(drP);
					tmrUpdateApply.Start();
					if (indgvPriceData.RowCount > 0)
					{
						indgvPriceData.CurrentCell = indgvPriceData.Rows[0].Cells[0];
						indgvPriceData.Focus();
					}
					tbSearchInPrice.Text = String.Empty;
					if (!_isComboBoxFormatHandlerRegistered)
						_isComboBoxFormatHandlerRegistered = !_isComboBoxFormatHandlerRegistered;
				}        	
		}

		private void RefreshDataBind()
		{
			FillTables(shortNameFilter, regionCodeFilter, sourceIndex);

			this.Text = "Редактор Правил Формализации";
			if (fcs == dgFocus.Firm)
			{
				indgvFirm.Focus();
				CurrencyManagerPosition((CurrencyManager)BindingContext[indgvFirm.DataSource, indgvFirm.DataMember], CCode.ColumnName, currentClientCode);
			}
			else
				if (fcs == dgFocus.Price)
				{
					indgvPrice.Select();
					CurrencyManagerPosition((CurrencyManager)BindingContext[indgvFirm.DataSource, indgvFirm.DataMember], CCode.ColumnName, currentClientCode);
					CurrencyManagerPosition((CurrencyManager)BindingContext[indgvPrice.DataSource, indgvPrice.DataMember], PPriceItemId.ColumnName, currentPriceItemId);
				}
		}

		private void txtBoxCode_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(DropField)))
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
				e.Effect = DragDropEffects.None;
		}

		private void txtBoxCode_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			((TextBox)sender).Text = ((DropField)e.Data.GetData(typeof(DropField))).FieldName;
		}

		private void txtBoxSheetName_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(DropSheet)))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void txtBoxSheetName_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			((TextBox)sender).Text = ((DropSheet)e.Data.GetData(typeof(DropSheet))).SheetName;
		}

		private void txtBoxCodeBegin_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(DropField)))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void txtBoxCodeBegin_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (((TextBox)sender).AccessibleName.EndsWith("Begin"))
			{
				string name = ((TextBox)sender).AccessibleName.Substring(0, ((TextBox)sender).AccessibleName.Length - 5);
				TextBox pairtb = PairTextBox(name, "End");
				if (pairtb != null)
				{
					((TextBox)sender).Text = ((DropField)e.Data.GetData(typeof(DropField))).FieldBegin;
					pairtb.Text = ((DropField)e.Data.GetData(typeof(DropField))).FieldEnd;
				}
			}
			else
				if (((TextBox)sender).AccessibleName.EndsWith("End"))
				{
					string name = ((TextBox)sender).AccessibleName.Substring(0, ((TextBox)sender).AccessibleName.Length - 3);
					TextBox pairtb = PairTextBox(name, "Begin");
					if (pairtb != null)
					{
						((TextBox)sender).Text = ((DropField)e.Data.GetData(typeof(DropField))).FieldEnd;
						pairtb.Text = ((DropField)e.Data.GetData(typeof(DropField))).FieldBegin;
					}
				}
		}

		private TextBox PairTextBox(string name, string tail)
		{
			foreach (Control c in pnlTxtFields.Controls)
			{
				if ((c.GetType().Equals(typeof(TextBox))) && (c.AccessibleName == name + tail))
				{
					return (TextBox)c;
				}
			}
			return null;
		}

		private void tcInnerSheets_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			string NameText = tcInnerSheets.SelectedTab.Text;
			tcInnerSheets.DoDragDrop(new DropSheet(NameText), DragDropEffects.Copy | DragDropEffects.Move);
		}

		private void txtBoxStartLine_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			((TextBox)sender).Text = ((DropField)e.Data.GetData(typeof(DropField))).RowNumber.ToString();
		}

		private void txtBoxStartLine_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(DropField)))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void lLblMaster_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				if (bsFormRules.Current == null)
					return;
				var priceCode = Convert.ToInt64(((DataRowView) bsFormRules.Current)[FRSelfPriceCode.ColumnName]);
				var firmCode = GetFirmCodeByPriceCode(priceCode);
				Process.Start(String.Format("mailto:{0}", GetContactText(firmCode, 2, 0)));
			}
			catch (Exception ex)
			{
				_logger.Error("Ошибка при попытке создать письмо ответственному за прайс лист из вкладки 'Прайс'", ex);
			}
		}

		/// <summary>
		/// Получить текст контактов из базы
		/// </summary>
		/// <param name="FirmCode">Код поставщика</param>
		/// <param name="ContactGroupType">Тип контактной группы: 0 - General, 1 - ClientManager, 2 - OrderManager, 3 - Accountant</param>
		/// <param name="ContactType">Тип контакта: 0 - Email, 1 - Phone</param>
		/// <returns>Текст контактов, разделенный ";"</returns>
		private string GetContactText(long FirmCode, byte ContactGroupType, byte ContactType)
		{
			DataSet dsContacts = MySqlHelper.ExecuteDataset(connection,@"
select distinct c.contactText
from Customers.suppliers s
  join contacts.contact_groups cg on s.ContactGroupOwnerId = cg.ContactGroupOwnerId
	join contacts.contacts c on cg.Id = c.ContactOwnerId
where
	s.Id = ?FirmCode
and cg.Type = ?ContactGroupType
and c.Type = ?ContactType

union

select distinct c.contactText
from Customers.suppliers s
  join contacts.contact_groups cg on s.ContactGroupOwnerId = cg.ContactGroupOwnerId
	join contacts.persons p on cg.id = p.ContactGroupId
	  join contacts.contacts c on p.Id = c.ContactOwnerId
where
	s.Id = ?FirmCode
and cg.Type = ?ContactGroupType
and c.Type = ?ContactType;",
				new MySqlParameter("?FirmCode", FirmCode),
				new MySqlParameter("?ContactGroupType", ContactGroupType),
				new MySqlParameter("?ContactType", ContactType));
			List<string> contacts = new List<string>();
			foreach (DataRow drContact in dsContacts.Tables[0].Rows)
			{
				if (!contacts.Contains(drContact["contactText"].ToString()))
					contacts.Add(drContact["contactText"].ToString());
			}

			return String.Join(";", contacts.ToArray());
		}

		private void CheckOnePrice(CurrencyManager currencyManager)
		{
			if (currencyManager.Position > -1)
			{
				DataRowView drv = (DataRowView)currencyManager.Current;
				DataView dv = (DataView)currencyManager.List;

				if (drv.Row.GetChildRows(dtClients.TableName + "-" + dtPrices.TableName).Length > 1)
				{
					indgvPrice.Focus();
				}
				else
				{
					if (drv.Row.GetChildRows(dtClients.TableName + "-" + dtPrices.TableName).Length == 1)
					{
						if (CostIsValid())
						{
							tbControl.SelectedTab = tpPrice;
							fcs = dgFocus.Firm;
						}
					}
				}
			}
		}

		private void btnFloatPanel_Click(object sender, System.EventArgs e)
		{
			if (pnlFloat.Visible)
			{
				pnlFloat.Visible = false;
				foreach (INDataGridView indg in gds)
				{
					indg.RowHeadersVisible = false;
				}
			}
			else
			{
				pnlFloat.Visible = true;
				pnlFloat.BringToFront();
			}
		}

		private void btnFloatPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.DrawString("Настройки", btnFloatPanel.Font, Brushes.Black, 
				btnFloatPanel.ClientRectangle, new System.Drawing.StringFormat(sf));
		}

		private void btnAwaitCheck_Click(object sender, System.EventArgs e)
		{
			FindErrors(txtBoxSelfAwaitPos, txtBoxAwait, txtBoxAwaitBegin, txtBoxAwaitEnd);
		}

		private void btnJunkCheck_Click(object sender, System.EventArgs e)
		{
			FindErrors(txtBoxSelfJunkPos, txtBoxJunk, txtBoxJunkBegin, txtBoxJunkEnd);
		}

		private void FindErrors(TextBox txtMask, TextBox txtExist, TextBox txtExistBegin, TextBox txtExistEnd)
		{
			Regex r;

			if (txtMask.Text != String.Empty)
			{
				try
				{
					erP.Clear();
					var regexMask = txtMask.Text;//.Replace("*", "+?");
					r = new Regex(regexMask, RegexOptions.IgnoreCase);
					if ((fmt == PriceFormat.FixedDOS) || (fmt == PriceFormat.FixedWIN) ||
						(fmt == PriceFormat.NativeFixedDOS) || (fmt == PriceFormat.NativeFixedWIN))
					{
						if ((txtExistBegin.Text != String.Empty) && (txtExistEnd.Text != String.Empty))
						{
							foreach (DataRow dr in _dataTableMarking.Rows)
							{
								if ((dr["MBeginField"].ToString() == txtExistBegin.Text) && (dr["MEndField"].ToString() == txtExistEnd.Text))
									CheckErrors(r, dr["MNameField"].ToString(), dtPrice, indgvPriceData);
							}
						}
					}
					else
					{
						if (txtExist.Text != String.Empty)
						{
							if (tcInnerSheets.SelectedIndex > 0)
							{
								CheckErrors(r, txtExist.Text, (DataTable)dtables[tcInnerSheets.SelectedIndex - 1], (INDataGridView)gds[tcInnerSheets.SelectedIndex - 1]);
							}
							else
								CheckErrors(r, txtExist.Text, dtPrice, indgvPriceData);
						}
					}
				}
				catch
				{
					erP.SetIconAlignment(txtMask, ErrorIconAlignment.MiddleLeft);

					erP.SetError(txtMask, "Неправильно задана маска!");
				}
			}
		}

		private void CheckErrors(Regex r, string FieldNameToSearch, DataTable dt, INDataGridView indgv)
		{
			dt.BeginLoadData();

			bool colExist = false;
			foreach (DataColumn c in dt.Columns)
			{
				if (c.ColumnName == FieldNameToSearch)
				{
					colExist = true;
					break;
				}
			}

			if (colExist)
			{
				foreach (DataRow dr in dt.Rows)
				{
					dr.RowError = String.Empty;
					dr.ClearErrors();
					Match m = r.Match(dr[FieldNameToSearch].ToString());
					if (m.Success)
					{
						indgv.RowHeadersVisible = true;
						dr.RowError = "Маска совпала";
					}
				}
			}
			dt.EndLoadData();
		}

		private string FindNameColumn()
		{
			string col = String.Empty;
			if (pnlGeneralFields.Visible)
			{
				if (txtBoxName1.Text != String.Empty)
					col = txtBoxName1.Text;
				else
				{
					if (txtBoxName2.Text != String.Empty)
						col = txtBoxName2.Text;
					else
					{
						if (txtBoxName3.Text != String.Empty)
							col = txtBoxName3.Text;
					}
				}

			}
			else
				if (pnlTxtFields.Visible)
				{
					if (txtBoxName1Begin.Text != String.Empty)
					{
						DataRow[] drcol = _dataTableMarking.Select("MBeginField = " + txtBoxName1Begin.Text);
						col = drcol[0]["MNameField"].ToString();
					}
				}

			return col;
		}
		private void btnEditMask_Click(object sender, EventArgs e)
		{
			string col = FindNameColumn();

			if (col != String.Empty)
			{
				DataRow dr;
				if (tcInnerSheets.SelectedIndex > 0)
					dr = dtables[tcInnerSheets.SelectedIndex - 1].Rows[((INDataGridView)gds[tcInnerSheets.SelectedIndex - 1]).CurrentRow.Index];
				else
					dr = dtPrice.Rows[indgvPriceData.CurrentRow.Index];

				if (dr[col].ToString() != String.Empty)
				{
					nameR = dr[col].ToString();

					frmNM = new frmNameMask();
					frmNM.txtBoxNameMaskNM.Text = txtBoxNameMask.Text;
					frmNM.txtBoxName.Text = nameR;

					frmNM.ShowDialog();
					if (frmNM.DialogResult == DialogResult.OK)
					{
						txtBoxNameMask.Text = frmNM.txtBoxNameMaskNM.Text;
					}
				}
				else
					MessageBox.Show(String.Format("Строка разбора пуста!"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else
				MessageBox.Show(String.Format("Не указано поле наименования!"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		private void btnCheckAll_Click(object sender, EventArgs e)
		{
			if (txtBoxNameMask.Text != String.Empty)
			{
				var regexMask = txtBoxNameMask.Text;//.Replace("*", "+?");
				var r = new Regex(regexMask, RegexOptions.IgnoreCase);
				var groups = new string[17];
				var i = 0;
				foreach (PriceFields pf in Enum.GetValues(typeof(PriceFields)))
				{
					if ((pf.ToString() != PriceFields.Name1.ToString()) && (pf.ToString() != PriceFields.Name2.ToString()) && (pf.ToString() != PriceFields.Name3.ToString()))
					{
						if (txtBoxNameMask.Text.IndexOf(pf.ToString()) > -1)
						{
							groups[i] = pf.ToString();
							i++;
						}
					}
					else
					{
						if (txtBoxNameMask.Text.IndexOf("Name") > -1)
						{
							if (i > 0)
							{
								if (groups[i - 1] != "Name")
								{
									groups[i] = "Name";
									i++;
								}
							}
							else
							{
								groups[i] = "Name";
								i++;
							}
						}
					}
				}

				if (tcInnerSheets.SelectedIndex > 0)
					CheckErrorsInAllNames(r, groups, FindNameColumn(), dtables[tcInnerSheets.SelectedIndex - 1], (INDataGridView)gds[tcInnerSheets.SelectedIndex - 1]);
				else
					CheckErrorsInAllNames(r, groups, FindNameColumn(), dtPrice, indgvPriceData);
			}
			else
				MessageBox.Show(String.Format("Не указана маска разбора товара!"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

		}

		private void CheckErrorsInAllNames(Regex r, string[] GroupsToFind, string ColumnNameToSearchIn, DataTable dt, INDataGridView indgv)
		{
			dt.BeginLoadData();
			var colExist = dt.Columns.Cast<DataColumn>().Any(c => c.ColumnName == ColumnNameToSearchIn);

			if (colExist)
			{
				foreach (DataRow dr in dt.Rows)
				{
					dr.RowError = String.Empty;
					dr.ClearErrors();
					var m = r.Match(dr[ColumnNameToSearchIn].ToString());
					if (m.Success)
					{
						var groupNotExists = false;
						for (int i = 0; i < GroupsToFind.Length; i++)
						{
							if (GroupsToFind[i] != null)
							{
								if (!m.Groups[GroupsToFind[i]].Success)
								{
									groupNotExists = true;
									break;
								}
							}
						}

						if (!groupNotExists)
						{
							indgv.RowHeadersVisible = true;
							dr.RowError = "Маска совпала";
						}
					}
				}
			}
			dt.EndLoadData();
		}

		private void CommitAllEdit()
		{
			if (tbControl.SelectedTab == tpFirms)
				//Завершаем редактирование общих настроек прайс-листов
				BindingContext[indgvPrice.DataSource, indgvPrice.DataMember].EndCurrentEdit();
			else
			{
				//Завершаем редактирование правил формализации цен
				if ((bsCostsFormRules.Current != null) && ((DataRowView)bsCostsFormRules.Current).IsNew && Convert.IsDBNull(((DataRowView)bsCostsFormRules.Current)[CFRCostName.ColumnName]))
					//Если создалась пустая строка в правилах формализации цен, то при сохранении отменяем ее
					bsCostsFormRules.CancelEdit();
				else
					//иначе просто применяем изменения
					bsCostsFormRules.EndEdit();
				//Завершаем редактирование правил формализации
				bsFormRules.EndEdit();
			}
		}

		private void tsbCancel_Click(object sender, EventArgs e)
		{
			CommitAllEdit();
			dtSet.RejectChanges();
			tsbApply.Enabled = false;
			tsbCancel.Enabled = false;
		}

		private void tbControl_Deselecting(object sender, TabControlCancelEventArgs e)
		{
			if (e.TabPage == tpPrice)
			{
				TrySaveData();
			}
			if (e.TabPage == tpFirms)
			{
				if (!CostIsValid())
					e.Cancel = true;
				else
				{
					int selectedRow = indgvPrice.SelectedCells[0].RowIndex;
					TrySaveData();
					// Если мы пытаемся перейти на вкладку "Прайс", а у нас нет 
					// в списке прайсов, то делать это не нужно
					CurrencyManager currencyManager = (CurrencyManager)BindingContext[indgvPrice.DataSource, 
						indgvPrice.DataMember];
					if (currencyManager.Count == 0)
						e.Cancel = true;
					try
					{
						indgvPrice.CurrentCell = indgvPrice.Rows[selectedRow].Cells[0];
					}
					catch(Exception)
					{}
				}
			}
		}

		private void TrySaveData()
		{
			CommitAllEdit();
			if (dtSet.HasChanges())
			{
				DataSet dsc = dtSet.GetChanges();
				if (dsc != null)
				{
					if (MessageBox.Show("Имееются не сохраненые данные. Сохранить?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						tsbApply_Click(null, null);
					}
					else
					{
						tsbCancel_Click(null, null);
					}
				}
			}
		}

		private void tsbApply_Click(object sender, EventArgs e)
		{
			Point selectedFirmCell = new Point(indgvFirm.SelectedCells[0].RowIndex, indgvFirm.SelectedCells[0].ColumnIndex);
			Point selectedPriceCell = new Point(indgvPrice.SelectedCells[0].RowIndex, indgvPrice.SelectedCells[0].ColumnIndex);
			string filePath = String.Empty;
			filePath = EndPath + Path.GetFileNameWithoutExtension(_priceFileFormatHelper.NewShortFileName) +
					   Path.DirectorySeparatorChar + _priceFileFormatHelper.NewShortFileName;

			CommitAllEdit();
			DataSet chg = dtSet.GetChanges();
			if (chg != null)
			{
				if (tbControl.SelectedTab == tpPrice)
				{
					if (connection.State == ConnectionState.Closed)
						connection.Open();
					try
					{
						MySqlTransaction tr = connection.BeginTransaction();
						try
						{							
							DbHelper.SetLogParameters(connection);

							mcmdUpdateCostRules.Connection = connection;
							mcmdInsertCostRules.Connection = connection;
							mcmdDeleteCostRules.Connection = connection;
							mcmdInsertCostRules.Parameters["?PriceCode"].Value = ((DataRowView)bsFormRules.Current)[FRSelfPriceCode.ColumnName];
							daCostRules.TableMappings.Clear();
							daCostRules.TableMappings.Add("Table", dtCostsFormRules.TableName);

							//Формируем тело письма с изменениями в колонках
							StringBuilder body = new StringBuilder();
							foreach (DataRow changeRow in chg.Tables[dtCostsFormRules.TableName].Rows)
							{
								if (changeRow.RowState == DataRowState.Added)
									body.AppendFormat("Добавлена ценовая колонка \"{0}\".\n", changeRow[CFRCostName.ColumnName]);
								else
									if ((bool)changeRow[CFRDeleted.ColumnName])
									{
										body.AppendFormat("Удалена ценовая колонка \"{0}\".\n", changeRow[CFRCostName.ColumnName]);
										changeRow.Delete();
									}
									else
										if (!changeRow[CFRCostName.ColumnName, DataRowVersion.Original].Equals(changeRow[CFRCostName.ColumnName, DataRowVersion.Current]))
											body.AppendFormat("Наименование ценовой колонки изменено с \"{0}\" на \"{1}\".\n",
												changeRow[CFRCostName.ColumnName, DataRowVersion.Original],
												changeRow[CFRCostName.ColumnName, DataRowVersion.Current]);
							}

							//Делается Copy для того, чтобы созданные записи (Added) при применении не помечались как неизмененные Unchanged
							daCostRules.Update(chg.Tables[dtCostsFormRules.TableName].Copy());

							mcmdUpdateFormRules.Connection = connection;
							daFormRules.TableMappings.Clear();
							daFormRules.TableMappings.Add("Table", dtFormRules.TableName);
							daFormRules.Update(chg.Tables[dtFormRules.TableName].Copy());

							if (body.Length > 0)
							{
								//Получаем информацию о поставщике, регионе и прайс-листе
								long _selfPriceCode = (long)(((DataRowView)bsFormRules.Current)[FRSelfPriceCode.ColumnName]);
								DataRow drPrice = dtPrices.Select("PPriceCode = " + _selfPriceCode)[0];
								string _priceName = drPrice[PPriceName.ColumnName].ToString();

								long _firmCode = (long)drPrice[PFirmCode.ColumnName];
								DataRow drClient = dtClients.Select("CCode = " + _firmCode)[0];
								string _firmName = drClient[CShortName.ColumnName].ToString();
								string _regionName = drClient[CRegion.ColumnName].ToString();

								Mailer.SendNotificationLetter(connection, body.ToString(), 
									_priceName, _firmName, _regionName);
							}
							dtSet.AcceptChanges();
							//Обновляе цены и правила формализации цен для того, чтобы загрузить корректные ID новых цен
							FillCosts(shortNameFilter, regionCodeFilter);
							tr.Commit();

							// Перепроводим прайс
							RetrancePrice.Go(indgvPrice, indgvFirm, connection, _priceProcessor, PPriceItemId);
						}
						catch (Exception ex)
						{
							tr.Rollback();
							MessageBox.Show("Не удалось применить изменения в правилах формализации прайс-листа. Сообщение было отправлено разработчику.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
							Program.SendMessageOnException(null, new Exception("Ошибка при применении изменений в правилах формализации.", ex));
						}
					}
					finally
					{
						if (connection.State != ConnectionState.Closed)
							connection.Close();
					}
					
					btnPutToBase.Enabled = !Convert.IsDBNull(((DataRowView)bsFormRules.Current).Row[FRPriceFormatId.ColumnName, DataRowVersion.Original]);

				}
				else if (tbControl.SelectedTab == tpFirms)
				{
					BindingContext[indgvPrice.DataSource, indgvPrice.DataMember].EndCurrentEdit();

					if (connection.State == ConnectionState.Closed)
						connection.Open();
					try
					{
						MySqlTransaction tr = connection.BeginTransaction();
						try
						{
							DbHelper.SetLogParameters(connection);

							//todo: здесь надо переписать
							MySqlCommand mcmdUPrice = new MySqlCommand();
							MySqlCommand mcmdDPrice = new MySqlCommand();
							MySqlDataAdapter daPrice = new MySqlDataAdapter();
							mcmdUPrice.CommandText = @"
call usersettings.UpdateCostType(?PPriceCode, ?PCostType);

UPDATE 
  usersettings.PricesData pd 
SET
  PriceType = if(?PIsParent = 1, ?PPriceType, PriceType)
WHERE 
	pd.PriceCode = ?PPriceCode;

UPDATE 
  usersettings.priceitems pim
SET
  pim.WaitingDownloadInterval = ?PWaitingDownloadInterval
WHERE 
	pim.Id = ?PPriceItemId;

UPDATE 
  usersettings.priceitems pim,
  farm.FormRules fr 
SET
  fr.MaxOld = ?PMaxOld
WHERE 
	pim.Id = ?PPriceItemId
and fr.Id = pim.FormRuleId;
";

							mcmdUPrice.Parameters.Clear();
							mcmdUPrice.Parameters.Add("?PPriceType", MySqlDbType.Int32);
							mcmdUPrice.Parameters.Add("?PCostType", MySqlDbType.Int32);
							mcmdUPrice.Parameters.Add("?PWaitingDownloadInterval", MySqlDbType.Int32);
							mcmdUPrice.Parameters.Add("?PMaxOld", MySqlDbType.Int32);
							mcmdUPrice.Parameters.Add("?PPriceCode", MySqlDbType.Int64);
							mcmdUPrice.Parameters.Add("?PPriceItemId", MySqlDbType.Int64);
							mcmdUPrice.Parameters.Add("?PIsParent", MySqlDbType.Int32);

							foreach (MySqlParameter ms in mcmdUPrice.Parameters)
							{
								ms.SourceColumn = ms.ParameterName.Substring(1);
							}

							mcmdDPrice.CommandText = "usersettings.DeleteCost";
							mcmdDPrice.CommandType = CommandType.StoredProcedure;
							mcmdDPrice.Parameters.Clear();
							mcmdDPrice.Parameters.Add("?inCostCode", MySqlDbType.Int64, 0, "PCostCode");
							mcmdDPrice.Parameters["?inCostCode"].Direction = ParameterDirection.Input;
													
							mcmdUPrice.Connection = connection;
							mcmdDPrice.Connection = connection;

							daPrice.UpdateCommand = mcmdUPrice;
							daPrice.DeleteCommand = mcmdDPrice;
							daPrice.TableMappings.Clear();
							daPrice.TableMappings.Add("Table", dtPrices.TableName);

							//Формируем тело письма с изменениями в колонках
							var body = new StringBuilder();
							string _priceName = "";
							foreach (DataRow changeRow in chg.Tables[dtPrices.TableName].Rows)
							{
								if ((bool)changeRow[PDeleted.ColumnName])
								{
									body.AppendFormat("Удалена ценовая колонка \"{0}\".\n", changeRow[PPriceName.ColumnName]);
									_priceName = changeRow[PPriceName.ColumnName].ToString();
									changeRow.Delete();
								}
							}

							daPrice.Update(chg.Tables[dtPrices.TableName]);

							if (body.Length > 0)
							{
								//Получаем информацию о поставщике, регионе и прайс-листе
								if (indgvFirm.CurrentRow != null)
								{
									var firm = (DataRowView) indgvFirm.CurrentRow.DataBoundItem;
									var _firmName = firm[CShortName.ColumnName].ToString();
									var _regionName = firm[CRegion.ColumnName].ToString();
									Mailer.SendNotificationLetter(connection, body.ToString(), _priceName, _firmName, _regionName);
								}
							}

							dtSet.AcceptChanges();
							RefreshDataBind();
							tr.Commit();
						}
						catch (Exception ex)
						{
							MessageBox.Show("Не удалось применить изменения в настройках прайс-листов. Сообщение было отправлено разработчику.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
							Program.SendMessageOnException(null, new Exception("Ошибка при применении изменений в настройках прайс-листов.", ex));
							tr.Rollback();
						}
					}
					finally
					{
						connection.Close();
					}
				}
			}
			tsbApply.Enabled = false;
			tsbCancel.Enabled = false;

			try
			{
				indgvFirm.CurrentCell = indgvFirm.Rows[selectedFirmCell.X].Cells[selectedFirmCell.Y];
				indgvPrice.CurrentCell = indgvPrice.Rows[selectedPriceCell.X].Cells[selectedPriceCell.Y];
			}
			catch (Exception)
			{ }

			if (_priceFileFormatHelper.ChangeFormat())
			{
				if (File.Exists(filePath))
				{
					try
					{
						if (!_priceProcessor.PutFileToInbound(Convert.ToUInt32(currentPriceItemId), File.OpenRead(filePath)))
						{
							MessageBox.Show(_priceProcessor.LastErrorMessage, "Ошибка", MessageBoxButtons.OK,
											MessageBoxIcon.Error);
						}
					}
					catch (Exception ex)
					{
						_logger.Error("Ошибка при применении изменений после смены формата файла (или разделителя). Невозможно положить файл в Inbound", ex);
					}
				}
				else
				{
					Mailer.SendWarningLetter(String.Format(
						"При применении изменений после смена формата файла (или разделителя) файл {0} не найден", filePath));
					MessageBox.Show(String.Format("Невозможно изменить формат файла. Файл {0} не найден",
						filePath), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void frmFREMain_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyCode == Keys.Escape) && (tbControl.SelectedTab == tpPrice))
				tbControl.SelectedTab = tpFirms;
		}        

		private void tmrUpdateApply_Tick(object sender, EventArgs e)
		{
			tmrUpdateApply.Stop();
			int ss = rtbArticle.SelectionStart;

			if (tbControl.SelectedTab == tpFirms)
				//Завершаем редактирование общих настроек прайс-листов
				BindingContext[indgvPrice.DataSource, indgvPrice.DataMember].EndCurrentEdit();
			else
			{
				//Раньше завершали редактирование bsCostsFormRules, но теперь в этом нет необходимости, 
				//т.к. редактирование завершается гридом или после DragAndDrop

				//Завершаем редактирование правил формализации
				bsFormRules.EndEdit();
			}

			if (dtSet.HasChanges())
			{
				tsbApply.Enabled = true;
				tsbCancel.Enabled = true;
				if (rtbArticle.Focused)
				{
					rtbArticle.SelectionStart = ss;
				}
			}
			tmrUpdateApply.Start();
		
		}

		private void frmFREMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			_priceProcessor.Dispose();
			TrySaveData();
			SaveCostsSettings();
			SaveMarkingSettings();
			SaveFirmAndPriceSettings();
		}

		private void indgvCosts_DragDrop(object sender, DragEventArgs e)
		{
			Point p = indgvCosts.PointToClient(new Point(e.X, e.Y));
			DataGridView.HitTestInfo costsHitTestInfo = indgvCosts.HitTest(p.X, p.Y);
			if (costsHitTestInfo.Type == DataGridViewHitTestType.Cell)
			{
				if (costsHitTestInfo.RowIndex > -1)
				{
					CurrencyManager cm = (CurrencyManager)BindingContext[indgvCosts.DataSource, indgvCosts.DataMember];
					DataRowView drv = (DataRowView)cm.Current;

					//Если запись не является помеченной на удаление, то позволяем назначать поля
					if (!(bool)drv[CFRDeleted.ColumnName]) 
					{
						string _concurentCostName = String.Empty;

						if (pnlGeneralFields.Visible)
						{
							DataRow[] drs = dtCostsFormRules.Select(
								String.Format("CFRCost_Code <> {0} and CFRPriceItemId = {1} and CFRFieldName = '{2}'",
									drv[CFRCost_Code.ColumnName],
									drv[CFRPriceItemId.ColumnName],
									((DropField)e.Data.GetData(typeof(DropField))).FieldName));
							if (drs.Length == 0)
								indgvCosts[cFRFieldNameDataGridViewTextBoxColumn.Name, costsHitTestInfo.RowIndex].Value = ((DropField)e.Data.GetData(typeof(DropField))).FieldName;
							else
								_concurentCostName = drs[0][CFRCostName.ColumnName].ToString();
						}
						else
						{
							DataRow[] drs = dtCostsFormRules.Select(
								String.Format("CFRCost_Code <> {0} and CFRPriceItemId = {1} and CFRTextBegin = '{2}' and CFRTextEnd = '{3}'",
									drv[CFRCost_Code.ColumnName],
									drv[CFRPriceItemId.ColumnName],
									((DropField)e.Data.GetData(typeof(DropField))).FieldBegin,
									((DropField)e.Data.GetData(typeof(DropField))).FieldEnd));
							if (drs.Length == 0)
							{
								indgvCosts[cFRTextBeginDataGridViewTextBoxColumn.Name, costsHitTestInfo.RowIndex].Value = ((DropField)e.Data.GetData(typeof(DropField))).FieldBegin;
								indgvCosts[cFRTextEndDataGridViewTextBoxColumn.Name, costsHitTestInfo.RowIndex].Value = ((DropField)e.Data.GetData(typeof(DropField))).FieldEnd;
							}
							else
								_concurentCostName = drs[0][CFRCostName.ColumnName].ToString();
						}

						if (String.IsNullOrEmpty(_concurentCostName))
							//Завершаем явно редактирование этой строчки, чтобы появились изменения в DataSet.GetChanges()
							drv.EndEdit();
						else
							MessageBox.Show(
								String.Format("Назначение не возможно, т.к. данное поле уже используется в ценовой колонке '{0}'.",
									_concurentCostName),
								"Предупреждение",
								MessageBoxButtons.OK,
								MessageBoxIcon.Warning);
					}
				}
			}
		}

		private void indgvCosts_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(DropField)))
			{
				if (dtCostsFormRules.Rows.Count > 0)
					e.Effect = DragDropEffects.Copy;
				else
					e.Effect = DragDropEffects.None;
			}
			else
				e.Effect = DragDropEffects.None;
		}

		private void indgvCosts_DragOver(object sender, DragEventArgs e)
		{
			Point p = indgvCosts.PointToClient(new Point(e.X, e.Y));
			DataGridView.HitTestInfo costsHitTestInfo = indgvCosts.HitTest(p.X, p.Y);
			if ((costsHitTestInfo.Type == DataGridViewHitTestType.Cell) && (e.Data.GetDataPresent(typeof(DropField))))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void txtBoxNameMask_TextChanged(object sender, EventArgs e)
		{
			tmrUpdateApply.Stop();
			tmrUpdateApply.Start();
		}

		private void rtbArticle_TextChanged(object sender, EventArgs e)
		{
			tmrUpdateApply.Stop();
			tmrUpdateApply.Start();

		}

		private void SaveFirmAndPriceSettings()
		{
			CregKey = BaseRegKey + "\\FirmDataGrid";
			indgvFirm.SaveSettings(CregKey);
			CregKey = BaseRegKey + "\\PriceDataGrid";
			indgvPrice.SaveSettings(CregKey);
		}
		
		private void LoadFirmAndPriceSettings()
		{
			CregKey = BaseRegKey + "\\FirmDataGrid";
			indgvFirm.LoadSettings(CregKey);
			CregKey = BaseRegKey + "\\PriceDataGrid";
			if (indgvPrice.CanLoadSettings(CregKey))
				indgvPrice.LoadSettings(CregKey);
		}

		private void SaveCostsSettings()
		{
			if (fileExist)
			{
				if ((fmt == PriceFormat.FixedDOS) || (fmt == PriceFormat.FixedWIN) ||
					(fmt == PriceFormat.NativeFixedDOS) || (fmt == PriceFormat.NativeFixedWIN))
				{
					CregKey = BaseRegKey + "\\CostsDataGridFixed";
				}
				else
				{
					CregKey = BaseRegKey + "\\CostsDataGrid";
				}
				indgvCosts.SaveSettings(CregKey);
			}
		}

		private void LoadCostsSettings()
		{
			if ((fmt == PriceFormat.FixedDOS) || (fmt == PriceFormat.FixedWIN) ||
				(fmt == PriceFormat.NativeFixedDOS) || (fmt == PriceFormat.NativeFixedWIN))
			{
				indgvCosts.Columns[cFRCostNameDataGridViewTextBoxColumn.Name].Visible = true;
				indgvCosts.Columns[cFRTextBeginDataGridViewTextBoxColumn.Name].Visible = true;
				indgvCosts.Columns[cFRTextEndDataGridViewTextBoxColumn.Name].Visible = true;
			}
			else
			{
				indgvCosts.Columns[cFRCostNameDataGridViewTextBoxColumn.Name].Visible = true;
				indgvCosts.Columns[cFRFieldNameDataGridViewTextBoxColumn.Name].Visible = true;
			}
		}

		private void LoadMarkingSettings()
		{
			_dataTableMarking.DefaultView.Sort = "MBeginField";
			indgvMarking.DataSource = _dataTableMarking.DefaultView;
			indgvMarking.LoadSettings(BaseRegKey + "\\MarkingDataGrid");
		}

		private void InDataGridViewMarking_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			var testString = e.FormattedValue.ToString();
			var dataGridView = sender as DataGridView;
			if (dataGridView == null)
				return;
			dataGridView.Rows[e.RowIndex].ErrorText = String.Empty;
			if (testString.Contains(" "))
			{				
				e.Cancel = true;
				dataGridView.Rows[e.RowIndex].ErrorText = "Строка не должна содержать пробелов";
			}			
		}

		private void SaveMarkingSettings()
		{
			if (fileExist)
			{
				indgvMarking.SaveSettings(BaseRegKey + "\\MarkingDataGrid");
			}
		}

		private void indgvPriceData_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				tbControl.SelectedTab = tpFirms;
		}

		private void indgvPriceData_MouseDown(object sender, MouseEventArgs e)
		{
			INDataGridView dgv = (INDataGridView)sender;
			Point p = indgvPriceData.PointToClient(Control.MousePosition);
			DataGridView.HitTestInfo hitTestInfo = dgv.HitTest(p.X, p.Y);
			if (hitTestInfo.Type == DataGridViewHitTestType.Cell)
			{
				string FieldText = String.Empty;

				FieldText = dgv.Columns[hitTestInfo.ColumnIndex].HeaderText;
				int RowText = hitTestInfo.RowIndex;
				if (pnlGeneralFields.Visible)
				{
					dgv.DoDragDrop(new DropField(FieldText, RowText), DragDropEffects.Copy | DragDropEffects.Move);
				}
				else
				{
					string beginText = String.Empty;
					string endText = String.Empty;
					int i = 0;
					bool findField = false;
					DataRow dr;
					while ((i < _dataTableMarking.Rows.Count) && (!findField))
					{
						dr = _dataTableMarking.Rows[i];
						if (dr["MNameField"].ToString() == FieldText)
						{
							findField = true;
							beginText = dr["MBeginField"].ToString();
							endText = dr["MEndField"].ToString();
						}
						else i++;
					}
					dgv.DoDragDrop(new DropField(beginText, endText, RowText), DragDropEffects.Copy | DragDropEffects.Move);
				}
			}
		}

		private void txtBoxCode_DoubleClick(object sender, EventArgs e)
		{
			((DataRowView)((TextBox)sender).DataBindings[0].BindingManagerBase.Current)[((TextBox)sender).DataBindings[0].BindingMemberInfo.BindingField] = ((TextBox)sender).DataBindings[0].DataSourceNullValue;
		}

		private void txtBoxCodeBegin_DoubleClick(object sender, EventArgs e)
		{
			if (((TextBox)sender).AccessibleName.EndsWith("Begin"))
			{
				string name = ((TextBox)sender).AccessibleName.Substring(0, ((TextBox)sender).AccessibleName.Length - 5);
				TextBox pairtb = PairTextBox(name, "End");
				if (pairtb != null)
				{
					((DataRowView)((TextBox)sender).DataBindings[0].BindingManagerBase.Current)[((TextBox)sender).DataBindings[0].BindingMemberInfo.BindingField] = ((TextBox)sender).DataBindings[0].DataSourceNullValue;
					((DataRowView)pairtb.DataBindings[0].BindingManagerBase.Current)[pairtb.DataBindings[0].BindingMemberInfo.BindingField] = ((TextBox)sender).DataBindings[0].DataSourceNullValue;
				}
			}
			else
				if (((TextBox)sender).AccessibleName.EndsWith("End"))
				{
					string name = ((TextBox)sender).AccessibleName.Substring(0, ((TextBox)sender).AccessibleName.Length - 3);
					TextBox pairtb = PairTextBox(name, "Begin");
					if (pairtb != null)
					{
						((DataRowView)((TextBox)sender).DataBindings[0].BindingManagerBase.Current)[((TextBox)sender).DataBindings[0].BindingMemberInfo.BindingField] = ((TextBox)sender).DataBindings[0].DataSourceNullValue;
						((DataRowView)pairtb.DataBindings[0].BindingManagerBase.Current)[pairtb.DataBindings[0].BindingMemberInfo.BindingField] = ((TextBox)sender).DataBindings[0].DataSourceNullValue;
					}
				}
		}

		private void tbFirmName_TextChanged(object sender, EventArgs e)
		{
			if (!InSearch)
			{				
				tmrSearch.Stop();
				TrySaveData();
				tmrSearch.Start();
			}
		}

		private void cbRegions_SelectedValueChanged(object sender, EventArgs e)
		{
			if (!InSearch)
			{
				tmrSearch.Stop();
				tmrSearch.Start();
			}
		}

		private void tmrSearch_Tick(object sender, EventArgs e)
		{
			InSearch = true;
			try
			{
				tmrSearch.Stop();

				ulong regcode = 0;
				if ((cbRegions.SelectedItem != null) && (Convert.ToUInt64(cbRegions.SelectedValue) != 0))
					regcode = Convert.ToUInt64(cbRegions.SelectedValue);
				var _sourceIndex = 0;
				if (cbSource.SelectedItem != null)
					_sourceIndex = Convert.ToInt32(cbSource.SelectedValue);
				string shname = tbFirmName.Text;
				FillTables(shname, regcode, _sourceIndex);
				shortNameFilter = shname;
				regionCodeFilter = regcode;
				indgvFirm.Focus();
			}
			finally
			{
				InSearch = false;
			}

		}

		private void indgvPrice_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			tmrUpdateApply.Stop();
			tmrUpdateApply.Start();
		}

		private void indgvFirm_DoubleClick(object sender, EventArgs e)
		{
			var currencyManager = (CurrencyManager)BindingContext[indgvFirm.DataSource, 
				indgvFirm.DataMember];
			CheckOnePrice(currencyManager);
		}

		private bool CostIsValid()
		{
			if (indgvPrice.CurrentRow != null)
			{
				if ((indgvPrice.CurrentRow.DataBoundItem == null) ||
					((indgvPrice.CurrentRow.DataBoundItem != null) &&
					 (((DataRowView) indgvPrice.CurrentRow.DataBoundItem)[PCostType.ColumnName] is DBNull)))
				{
					MessageBox.Show("Необходимо указать тип цены!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				else
					return true;
			}
			return false;
		}

		private void indgvFirm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				e.Handled = true;
				CurrencyManager currencyManager = (CurrencyManager)BindingContext[indgvFirm.DataSource, indgvFirm.DataMember];
				CheckOnePrice(currencyManager);
			}
		}

		private void indgvPrice_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				indgvFirm.Focus();
			}
			if (e.KeyCode == Keys.Enter)
			{
				e.Handled = true;
				
				if(CostIsValid())                
				{
					fcs = dgFocus.Price;
					tbControl.SelectedTab = tpPrice;
					PrepareCreateNewPriceCollumn();
				}
			}
			if (e.KeyCode == Keys.Delete) // удаление ценовой колонки для многофайлового ПЛ
			{
				if(CostIsValid())
				{
					DataRowView item = (DataRowView)indgvPrice.CurrentRow.DataBoundItem;
					int cost_type = (int)item[PCostType.ColumnName];
					if (cost_type == 0) return;	// для удаления цен из мультиколоночных прайсов используется другой механизм
					if ((bool)item[PDeleted.ColumnName] == true) return;
					byte isBaseCost = (byte) item[PBaseCost.ColumnName];		
					if (isBaseCost == 1)
					{
						MessageBox.Show("Нельзя удалить базовую ценовую колонку", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;	
					}

					if (MessageBox.Show("Вы уверены в удалении ценовой колонки?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
					{
						item[PDeleted.ColumnName] = true; // помечаем запись на удаление
						item.EndEdit();						
						indgvPrice.Refresh();
					}
					else
						return;					
				}				
			}
		}

		private void PrepareCreateNewPriceCollumn()
		{
			var item = (DataRowView)indgvPrice.CurrentRow.DataBoundItem;
			var cost_type = 0;
			Int32.TryParse(item[PCostType.ColumnName].ToString(), out cost_type);
			if (cost_type == 0)
				createNewPriceCollumn.Enabled = true;
			else
				createNewPriceCollumn.Enabled = false;
		}

		private void indgvPrice_DoubleClick(object sender, EventArgs e)
		{
			if (CostIsValid())
			{
				fcs = dgFocus.Price;
				tbControl.SelectedTab = tpPrice;
				PrepareCreateNewPriceCollumn();
			}
		}

		private void indgvFirm_KeyPress(object sender, KeyPressEventArgs e)
		{
			if(char.IsLetter(e.KeyChar))
			{
				tbFirmName.Focus();
				tbFirmName.Text = e.KeyChar.ToString();
				tbFirmName.SelectionStart = 1;
			}
		}

		private void indgvPrice_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			//Если отображается ComboBox, то привязываемся на его событие: изменение текущего элемента
			if (e.Control is ComboBox)
			{
				indgvPrice.CurrentCell.ReadOnly = !Convert.ToBoolean(((DataRowView)indgvPrice.CurrentCell.OwningRow.DataBoundItem)["PIsParent"]);
				e.Control.Enabled = !indgvPrice.CurrentCell.ReadOnly;
				if (e.Control.Enabled)
					((ComboBox)e.Control).SelectionChangeCommitted += frmFREMain_SelectionChangeCommitted;

				// Если была первоначальная настройка прайс-листа, тогда
				// не позволяем редактировать тип колонок (с мультифайловых на мультиколоночные)
				var grid = (sender as INDataGridView);
				bool isCostTypeColumn = (grid.CurrentCell.ColumnIndex == 
					pCostTypeDataGridViewComboBoxColumn.Index);
				if (isCostTypeColumn)
				{
					string priceItemId = Convert.ToString(((DataRowView)indgvPrice.CurrentCell
						.OwningRow.DataBoundItem)["PPriceItemId"]);
					bool priceEdited = PriceWasEdited(priceItemId);
					e.Control.Enabled = !priceEdited;
					indgvPrice.CurrentCell.ReadOnly = priceEdited;
				}
			}
		}

		// Проверяет, была ли уже настройка прайса
		private bool PriceWasEdited(string priceItemId)
		{			
			var drPrice = dtPrices.Select("PPriceItemId = " + priceItemId);
			string format;
			string costType;
			try
			{
				costType = Convert.ToString(drPrice[0]["PCostType"]);
				var rowPriceItems = drPrice[0].GetChildRows(dtPrices.ChildRelations[2]);
				format = Convert.ToString(rowPriceItems[0]["FRPriceFormatId"]);
			}
			catch (Exception)
			{
				return false;
			}
			return (format.Length != 0) && (costType.Length != 0);
		}

		void frmFREMain_SelectionChangeCommitted(object sender, EventArgs e)
		{
			//После изменения элемента сразу сохраняем его в ячейке, чтобы возникло событие CellValueChanged
			//Позволяем изменять на одно из корректных значений, а не на DBNull
			if (!(((ComboBox)sender).SelectedValue is DBNull))
				indgvPrice.CurrentCell.Value = ((ComboBox)sender).SelectedValue;
			else
				indgvPrice.CancelEdit();
		}

		private void btnVitallyImportantCheck_Click(object sender, EventArgs e)
		{
			FindErrors(txtBoxVitallyImportantMask, txtBoxVitalyImportant, txtBoxVitalyImportantBegin, txtBoxVitalyImportantEnd);
		}

		private void btnRetrancePrice_Click(object sender, EventArgs e)
		{
			RetrancePrice.Go(indgvPrice, indgvFirm, connection, _priceProcessor, PPriceItemId);
		}

		private void indgvPrice_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			BindingContext[indgvPrice.DataSource].EndCurrentEdit();
			//Если рассматриваются колонки с ComboBox и строки с данными
			if ((e.RowIndex >= 0) && (((INDataGridView)sender).Columns[e.ColumnIndex].CellTemplate is DataGridViewComboBoxCell))
			{
				//Если это не родительский прайс-лист, то это ценовая колонка многофайлового прайс-листа и изменять ее нельзя
				if (!Convert.ToBoolean(((DataRowView)indgvPrice.Rows[e.RowIndex].DataBoundItem)[PIsParent.ColumnName]))
					e.CellStyle.ForeColor = SystemColors.InactiveCaptionText;

				// Проверяем, если была первоначальная настройка прайс-листа,
				// то не выделяем колонки серым цветом (неактивные)
				INDataGridView inGridView = (sender as INDataGridView);
				bool isCostTypeColumn = (e.ColumnIndex == pCostTypeDataGridViewComboBoxColumn.Index);
				if (isCostTypeColumn)
				{
					string priceItemId = Convert.ToString(((DataRowView)indgvPrice
						.Rows[e.RowIndex].DataBoundItem)["PPriceItemId"]);
					if (PriceWasEdited(priceItemId))
					{
						e.CellStyle.ForeColor = SystemColors.InactiveCaptionText;
					}
				}				
			}
			DataRowView drv = (DataRowView)indgvPrice.Rows[e.RowIndex].DataBoundItem;
			if ((bool)drv[PDeleted.ColumnName])
			{
				e.CellStyle.BackColor = btnDeletedCostColor.BackColor;
			}
		}

		private void cmbParentSynonyms_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				FillParentComboBoxBySearch(
					(ComboBox)sender,
@"select
  pd.PriceCode,
  concat(s.Name, ' (', pd.PriceName, ') - ', r.Region) PriceName
from
  Customers.suppliers s,
  usersettings.pricesdata pd,
  farm.regions r
where
  pd.FirmCode = s.Id
and pd.CostType is not null
and r.RegionCode = s.HomeRegion
and pd.ParentSynonym is null
and ((pd.PriceCode = ?PrevParentValue) or (pd.PriceName like ?SearchText) or (s.Name like ?SearchText))
order by PriceName
",
	"PriceCode", 
	"PriceName");
			}
		}

		private void indgvCosts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			//По двойному клику на ячейках FieldName, TextBegin, TextEnd в таблице настройки правил формализации цен
			//очищаем данные поля
			if (e.ColumnIndex > 0)
			{
				if ((indgvCosts.Columns[e.ColumnIndex] == cFRFieldNameDataGridViewTextBoxColumn)
					|| (indgvCosts.Columns[e.ColumnIndex] == cFRTextBeginDataGridViewTextBoxColumn)
					|| (indgvCosts.Columns[e.ColumnIndex] == cFRTextEndDataGridViewTextBoxColumn))
				{
					DataGridViewRow row = indgvCosts.Rows[e.RowIndex];
					DataRowView drv = (DataRowView)row.DataBoundItem;

					//Если запись не является помеченной на удаление, то позволяем редактировать
					if (!(bool)drv[CFRDeleted.ColumnName])
					{
						drv.BeginEdit();
						if (pnlGeneralFields.Visible)
						{
							drv[cFRFieldNameDataGridViewTextBoxColumn.DataPropertyName] = String.Empty;
						}
						else
						{
							drv[cFRTextBeginDataGridViewTextBoxColumn.DataPropertyName] = String.Empty;
							drv[cFRTextEndDataGridViewTextBoxColumn.DataPropertyName] = String.Empty;
						}
						drv.EndEdit();
					}
				}
			}
		}

		private void indgvCosts_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			DataGridViewRow r = indgvCosts.Rows[e.RowIndex];

			DataRowView drv = null;
			//Если при вводе новой ценовой колонке отменить создание, то при попытке получить доступ
			//к DataBoundItem будет происходить IndexOutOfRangeException
			try
			{
				if (r.DataBoundItem != null)
					drv = (DataRowView)r.DataBoundItem;
			}
			catch (IndexOutOfRangeException)
			{
			}

			if (drv != null)
			{
				//Если это новая запись и мы еще не установили поля PriceItemId и CostCode
				if (drv.IsNew && Convert.IsDBNull(drv["CFRCost_Code"]))
				{
					long NewCostCode = (long)dtSet.Tables["Правила формализации цен"].Compute("Max(CFRCost_Code)", null);
					drv["CFRCost_Code"] = NewCostCode + 1;
					//Если мы добавили новую ценовую колонку и есть фильтр, то сбрасываем фильтр
					if (!String.IsNullOrEmpty(tbCostFind.Text))
					{
						tbCostFind.Text = String.Empty;
						tmrSetNewCost.Start();
					}
				}
			}
		}

		private void indgvCosts_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			indgvCosts.Rows[e.RowIndex].ErrorText = "";

			// Don't try to validate the 'new row' until finished 
			// editing since there
			// is not any point in validating its initial value.
			if (indgvCosts.Rows[e.RowIndex].IsNewRow) { return; }

			//Если мы редактировали или ввели название ценовой колонки пустым, то выдать соответствующее предупреждение
			if ((indgvCosts.Columns[e.ColumnIndex] == cFRCostNameDataGridViewTextBoxColumn) && String.IsNullOrEmpty(e.FormattedValue.ToString()))
			{
				e.Cancel = true;
				indgvCosts.Rows[e.RowIndex].ErrorText = "Название ценовой колонки не может быть пустой строкой";
			}
			else
				//Если мы ввели новое название ценовой колонки, то проверяем на несовпадение с существующими ценовыми колонками
				if ((indgvCosts.Columns[e.ColumnIndex] == cFRCostNameDataGridViewTextBoxColumn) && !String.IsNullOrEmpty(e.FormattedValue.ToString()))
				{
					DataRowView _checkedRow = (DataRowView)indgvCosts.Rows[e.RowIndex].DataBoundItem;
					if (_checkedRow.IsEdit)
					{
						if (_checkedRow.IsNew)
						{
							DataRow[] drs = dtCostsFormRules.Select(
								String.Format("CFRCost_Code is not NULL and CFRPriceItemId = {0} and CFRCostName = '{1}'",
									_checkedRow[CFRPriceItemId.ColumnName],
									e.FormattedValue));
							if (drs.Length > 0)
							{
								e.Cancel = true;
								indgvCosts.Rows[e.RowIndex].ErrorText = "Название ценовой колонки совпадает с существующей ценовой колонкой";
							}
						}
						else
						{
							DataRow[] drs = dtCostsFormRules.Select(
								String.Format("CFRCost_Code <> {0} and CFRPriceItemId = {1} and CFRCostName = '{2}'",
									_checkedRow[CFRCost_Code.ColumnName],
									_checkedRow[CFRPriceItemId.ColumnName],
									e.FormattedValue));
							if (drs.Length > 0)
							{
								e.Cancel = true;
								indgvCosts.Rows[e.RowIndex].ErrorText = "Название ценовой колонки совпадает с существующей ценовой колонкой";
							}
						}
					}
				}

		}

		private void indgvCosts_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyCode == Keys.Delete) && bsCostsFormRules.AllowNew && !(bool)((DataRowView)bsCostsFormRules.Current)[CFRBaseCost.ColumnName])
			{
				if (((DataRowView)bsCostsFormRules.Current).Row.RowState == DataRowState.Added)
					((DataRowView)bsCostsFormRules.Current).Delete();
				else
					((DataRowView)bsCostsFormRules.Current)[CFRDeleted.ColumnName] = true;
				((DataRowView)bsCostsFormRules.Current).EndEdit();
				indgvCosts.Refresh();
			}
		}

		private void indgvCosts_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
		{
			DataGridViewRow r = indgvCosts.Rows[e.RowIndex];
			if (r.IsNewRow) return;

			//Если мы пытаемся отредактировать название ценовой колонки, помечанной на удаление, то не позволяем это сделать
			if (indgvCosts.Columns[e.ColumnIndex] == cFRCostNameDataGridViewTextBoxColumn)
			{
				DataRowView drv = (DataRowView)r.DataBoundItem;
				if ((bool)drv[CFRDeleted.ColumnName])
					e.Cancel = true;
			}
		}

		private void indgvCosts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if ((e.ColumnIndex > -1) && (e.RowIndex > -1))
			{
				if (!indgvCosts.Rows[e.RowIndex].IsNewRow)
				{
					DataRowView drv = (DataRowView)indgvCosts.Rows[e.RowIndex].DataBoundItem;

					if (drv != null)
					{
						if (!Convert.IsDBNull(drv[CFRBaseCost.ColumnName]) && (bool)drv[CFRBaseCost.ColumnName])
							e.CellStyle.BackColor = btnBaseCostColor.BackColor;

						if (drv.Row.RowState == DataRowState.Added)
							e.CellStyle.BackColor = btnNewCostColor.BackColor;
						else
							if ((drv.Row.RowState == DataRowState.Modified) && !drv.Row[CFRCostName.ColumnName, DataRowVersion.Original].Equals(drv.Row[CFRCostName.ColumnName, DataRowVersion.Current]))
								e.CellStyle.BackColor = btnChangedCostColor.BackColor;

						if ((bool)drv[CFRDeleted.ColumnName])
							e.CellStyle.BackColor = btnDeletedCostColor.BackColor;
					}
				}
			}
		}

		private void ColorChange(object sender, EventArgs e)
		{
			cdLegend.Color = ((Button)sender).BackColor;
			if (cdLegend.ShowDialog((Button)sender) == DialogResult.OK)
			{
				((Button)sender).BackColor = cdLegend.Color;
				indgvCosts.Refresh();
			}
		}

		private void bsCostsFormRules_ListChanged(object sender, ListChangedEventArgs e)
		{
			if ((bsCostsFormRules.List != null) && (bsCostsFormRules.List is DataView))
			{
				string selectFieldName;

				if ((fmt == PriceFormat.FixedDOS) || (fmt == PriceFormat.FixedWIN) ||
					(fmt == PriceFormat.NativeFixedDOS) || (fmt == PriceFormat.NativeFixedWIN))
					selectFieldName = "CFRTextBegin";
				else
					selectFieldName = "CFRFieldName";

				object count = ((DataView)bsCostsFormRules.List).ToTable().Compute(
					String.Format("count({0})", selectFieldName),
					String.Format("({0} is not null) and ({0} <> '')", selectFieldName));				
				lCostCount.Text = String.Format("Общее кол-во цен: {0}   Кол-во настроенных цен: {1}", bsCostsFormRules.Count, count);
			}
			else
				lCostCount.Text = String.Format("Общее кол-во цен: {0}", bsCostsFormRules.Count);
		}

		private void lCostCount_TextChanged(object sender, EventArgs e)
		{
			ttMain.SetToolTip(lCostCount, lCostCount.Text);
		}

		private void tmrCostSearch_Tick(object sender, EventArgs e)
		{
			tmrCostSearch.Stop();

			//Завершаем редактирование правил формализации цен
			if ((bsCostsFormRules.Current != null) && ((DataRowView)bsCostsFormRules.Current).IsNew && Convert.IsDBNull(((DataRowView)bsCostsFormRules.Current)[CFRCostName.ColumnName]))
				//Если создалась пустая строка в правилах формализации цен, то при сохранении отменяем ее
				bsCostsFormRules.CancelEdit();
			else
				//иначе просто применяем изменения
				bsCostsFormRules.EndEdit();

			bsCostsFormRules.Filter = 
				String.Format("CFRPriceItemId = {0} and CFRCostName like '*{1}*'", currentPriceItemId, tbCostFind.Text);

			indgvCosts.Refresh();
			indgvCosts.Focus();
		}

		private void tbCostFind_TextChanged(object sender, EventArgs e)
		{
			tmrCostSearch.Stop();
			if (!String.IsNullOrEmpty(tbCostFind.Text))
				tmrCostSearch.Start();
			else
			{
				//Завершаем редактирование правил формализации цен
				if ((bsCostsFormRules.Current != null) && ((DataRowView)bsCostsFormRules.Current).IsNew && Convert.IsDBNull(((DataRowView)bsCostsFormRules.Current)[CFRCostName.ColumnName]))
					//Если создалась пустая строка в правилах формализации цен, то при сохранении отменяем ее
					bsCostsFormRules.CancelEdit();
				else
					//иначе просто применяем изменения
					bsCostsFormRules.EndEdit();

				//Сбросили фильтр
				bsCostsFormRules.Filter =
					String.Format("CFRPriceItemId = {0}", currentPriceItemId);
			}
		}


		private void tbCostFind_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				tmrCostSearch_Tick(null, null);
		}

		private void tmrSetNewCost_Tick(object sender, EventArgs e)
		{
			tmrSetNewCost.Stop();
			bsCostsFormRules.Position = bsCostsFormRules.Count - 1;
		}

		private void btnPutToBase_Click(object sender, EventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.RestoreDirectory = true;
			_priceFileFormatHelper.SetNewFormat((PriceFormat?)Convert.ToInt32(cmbFormat.SelectedValue), tbDevider.Text);
			if (PriceFileFormatHelper.IsTextFormat(_priceFileFormatHelper.NewFormat))
				dialog.Filter = String.Format("Все файлы|*.*|{0}|*{1}", _priceFileFormatHelper.NewFormat, _priceFileFormatHelper.NewFileExtension);
			else
				dialog.Filter = String.Format("{0}|*{1}|Все файлы|*.*", _priceFileFormatHelper.NewFormat, _priceFileFormatHelper.NewFileExtension);
			dialog.FilterIndex = 0;
			dialog.Title = String.Format("Выберите файл в формате {0}. ", _priceFileFormatHelper.NewFormat.ToString());
			if (!String.IsNullOrEmpty(_priceFileFormatHelper.NewDelimiter))
				dialog.Title += String.Format("Разделитель '{0}'", _priceFileFormatHelper.NewDelimiter);
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				var fileName = dialog.FileName;
				var tables = PriceFileHelper.AsyncOpenPriceFile(fileName, _priceFileFormatHelper.NewFormat,
					_priceFileFormatHelper.NewDelimiter, _dataTableMarking);
				if ((tables == null) || (tables.Count == 0) || (tables[0].Rows.Count == 0))
				{
					MessageBox.Show("Неправильный формат или файл поврежден", "Ошибка открытия файла", MessageBoxButtons.OK,
									MessageBoxIcon.Error);
					return;
				}
				try
				{
					currentPriceItemId = (long)(((DataRowView)indgvPrice.CurrentRow.DataBoundItem)[PPriceItemId.ColumnName]);
					if (!_priceProcessor.PutFileToBase(Convert.ToUInt32(currentPriceItemId), File.OpenRead(fileName)))
					{
						MessageBox.Show(_priceProcessor.LastErrorMessage, "Ошибка", MessageBoxButtons.OK,
										MessageBoxIcon.Error);
					}
					else
					{
						MessageBox.Show("Прайс-лист успешно положен в Base.", "Информация", MessageBoxButtons.OK,
										MessageBoxIcon.Information);
						tbControl_SelectedIndexChanged(sender, e);
					}
				}
				catch (Exception ex)
				{
					_logger.Error("Ошибка при попытке положить файл в Base", ex);
					MessageBox.Show("Не удалось положить файл в Base. Сообщение об ошибке отправлено разработчику",
									"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void checkBoxShowDisabled_CheckedChanged(object sender, EventArgs e)
		{
			_showDisabledFirm = checkBoxShowDisabled.Checked;
			FillTables(shortNameFilter, regionCodeFilter, sourceIndex);
			indgvFirm.Focus();
		}

		// Обработчик для таймера, использующегося для поиска 
		// по колонкам прайс-листа (вкладка "Прайс")
		private void tmrSearchInPrice_Tick(object sender, EventArgs e)
		{
			_inactivityTime += tmrSearchInPrice.Interval;
			if (_inactivityTime > 2000)
			{
				tmrSearchInPrice.Enabled = false;
			}
		}

		private bool SearchTextInGridView(string text, DataGridView grid, bool moveToNextResult, bool enterPressed)
		{
			if (grid == null)
				grid = indgvPriceData;
			if (grid.CurrentRow != null) {
				// Сначала ищем по заголовкам столбцов
				foreach (DataGridViewColumn column in grid.Columns) {
					if (column.HeaderText.ToUpper().Contains(text.ToUpper())) {
						var rowIndex = grid.CurrentRow.Index;
						grid.CurrentCell = grid.Rows[rowIndex].Cells[column.Index];
						return true;
					}
				}
				// Потом ищем по ячейкам. Сначала по тем, которые ниже текущей
				var indexColumn = moveToNextResult ? (grid.CurrentCell.ColumnIndex + 1) : 0;
				var indexRow = enterPressed ? (grid.CurrentRow.Index - 1) : grid.CurrentRow.Index;
				for (var i = indexRow; i < grid.Rows.Count; i++) {
					for (var j = indexColumn; j < grid.Columns.Count; j++) {
						var cell = grid.Rows[i].Cells[j];
						if (cell.Value != null)
							if (cell.Value.ToString().ToUpper().Contains(text.ToUpper())) {
								grid.CurrentCell = grid.Rows[i].Cells[cell.ColumnIndex];
								return true;
							}
					}
					indexColumn = 0;
				}
				// Затем сначала и до текущей строки 
				for (var i = 0; i < grid.CurrentRow.Index; i++)
					foreach (DataGridViewCell cell in grid.Rows[i].Cells)
						if (cell.Value != null)
							if (cell.Value.ToString().ToUpper().Contains(text.ToUpper())) {
								grid.CurrentCell = grid.Rows[i].Cells[cell.ColumnIndex];
								return true;
							}
			}
			return false;
		}

		private void indgvPriceData_KeyPress(object sender, KeyPressEventArgs e)
		{
			_searchGrid = (sender as DataGridView);
			if (!tmrSearchInPrice.Enabled)
			{
				tmrSearchInPrice.Enabled = true;
				_searchTextInPrice = String.Empty;
				tbSearchInPrice.Text = _searchTextInPrice;
			}
			if (e.KeyChar.Equals('\b'))
			{
				if (tbSearchInPrice.Text.Length > 0)
					tbSearchInPrice.Text = tbSearchInPrice.Text.Remove(
						tbSearchInPrice.Text.Length - 1, 1);
			} else if (e.KeyChar.Equals('\r'))
			{
				if (!String.IsNullOrEmpty(tbSearchInPrice.Text))
				{
					if (SearchTextInGridView(tbSearchInPrice.Text, indgvPriceData, true, true))
						tbSearchInPrice.BackColor = Color.Green;
					else
						tbSearchInPrice.BackColor = Color.Red;
				}
				else
				{
					tbSearchInPrice.BackColor = Color.White;
					tbSearchInPrice.ForeColor = Color.Black;
				}
			}
			else
				tbSearchInPrice.Text += e.KeyChar;
			_inactivityTime = 0;
		}

		private void tbSearchInPrice_TextChanged(object sender, EventArgs e)
		{
			if (tbSearchInPrice.Text.Length > 0)
			{
				tbSearchInPrice.ForeColor = Color.White;
				if (SearchTextInGridView(tbSearchInPrice.Text, _searchGrid, false, false))
					tbSearchInPrice.BackColor = Color.Green;
				else
					tbSearchInPrice.BackColor = Color.Red;
			}
			else
			{
				tbSearchInPrice.ForeColor = Color.Black;
				tbSearchInPrice.BackColor = Color.White;
			}
		}

		private void cmbFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_isComboBoxFormatHandlerRegistered)
			{
				_isComboBoxFormatHandlerRegistered = !_isComboBoxFormatHandlerRegistered;

				var tempDirectory = EndPath + Convert.ToString(currentPriceItemId) + Path.DirectorySeparatorChar;
				if (!Directory.Exists(tempDirectory))
					Directory.CreateDirectory(tempDirectory);

				if (_priceFileFormatHelper.SetNewFormat((PriceFormat?) Convert.ToInt32(cmbFormat.SelectedValue), tbDevider.Text))
				{
					var newFormatFileExtension = _priceFileFormatHelper.NewFileExtension;
					if (PriceFileFormatHelper.IsTextFormat(_priceFileFormatHelper.NewFormat))
						ofdNewFormat.Filter = String.Format("Все файлы|*.*|{0}|*{1}", _priceFileFormatHelper.NewFormat, newFormatFileExtension);
					else
						ofdNewFormat.Filter = _priceFileFormatHelper.NewFormat + "|*" + newFormatFileExtension + "|Все файлы|*.*";
					ofdNewFormat.FilterIndex = 0;
					ofdNewFormat.Title = "Выберите файл в новом формате (" + _priceFileFormatHelper.NewFormat + ").";
					if (!String.IsNullOrEmpty(_priceFileFormatHelper.NewDelimiter))
						ofdNewFormat.Title += " Разделитель: '" + tbDevider.Text + "'";

					if (ofdNewFormat.ShowDialog() == DialogResult.OK)
					{
						if (File.Exists(ofdNewFormat.FileName))
						{
							using (var newSourceFile = File.OpenRead(ofdNewFormat.FileName))
							{
								var destinationFilePath = tempDirectory + _priceFileFormatHelper.NewShortFileName;
								try
								{
									using (var newDestinationFile = File.Create(destinationFilePath))
									{
										newSourceFile.CopyTo(newDestinationFile, 102400);
										_isFormatChanged = true;
									}
								}
								catch (Exception)
								{
									ComboBoxFormatSetSelectedText(_priceFileFormatHelper.CurrentOpenedFileFormat.ToString());
									_priceFileFormatHelper.ResetNewFormat();									
									Mailer.SendWarningLetter(
										String.Format(
											@"Изменение формата прайс-листа. Невозможно записать указанный файл в новом формате в локальную веременную директорию. Файл {0}",
											destinationFilePath));
								}
							}
							var dataRowPrice = (indgvPrice.CurrentRow.DataBoundItem as DataRowView).Row;
							DoOpenPrice(dataRowPrice);
							_isFormatChanged = false;
							_priceFileFormatHelper.OpenFileInNewFormat();
						}
						else
						{
							ComboBoxFormatSetSelectedText(_priceFileFormatHelper.CurrentOpenedFileFormat.ToString());
							_priceFileFormatHelper.ResetNewFormat();							
							dtSet.RejectChanges();
							// TODO: выставить в comboBox старый формат
							Mailer.SendWarningLetter(
								String.Format(
									@"Изменение формата прайс-листа. Пользователь выбрал файл в новом формате, однако этот файл не был найден. Файл {0}",
									ofdNewFormat.FileName));
							MessageBox.Show("Выбранный вами файл был переименован, перемещен или удален. Выберите другой файл", "Ошибка",
											MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
					else
					{
						ComboBoxFormatSetSelectedText(_priceFileFormatHelper.CurrentOpenedFileFormat.ToString());
						_priceFileFormatHelper.ResetNewFormat();						
					}
				}
				else
				{
					ComboBoxFormatSetSelectedText(_priceFileFormatHelper.CurrentOpenedFileFormat.ToString());
					_priceFileFormatHelper.ResetNewFormat();					
					if (!String.IsNullOrEmpty(_priceFileFormatHelper.LastErrorMessage))
						MessageBox.Show(_priceFileFormatHelper.LastErrorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				_isComboBoxFormatHandlerRegistered = !_isComboBoxFormatHandlerRegistered;
			}
		}

		private void ComboBoxFormatSetSelectedText(string text)
		{
			try
			{
				foreach (var item in cmbFormat.Items)
				{
					var rowView = (item as DataRowView);
					if (rowView != null)
					{
						var dataRow = rowView.Row;
						if (dataRow != null)
						{
							var formatText = dataRow[FMTFormat.ColumnName].ToString();
							if (String.Equals(formatText.ToUpper(), text.ToUpper()))
							{
								cmbFormat.SelectedItem = item;
								return;
							}
						}
					}
				}
			}
			catch (Exception)
			{}
		}

		private void tbDevider_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				cmbFormat_SelectedIndexChanged(sender, e);
			}
		}

		private void indgvPriceData_Enter(object sender, EventArgs e)
		{
			cmbFormat_SelectedIndexChanged(sender, e);
		}

		private void buttonSearchNext_Click(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(tbSearchInPrice.Text))
			{
				if (SearchTextInGridView(tbSearchInPrice.Text, indgvPriceData, true, false))
					tbSearchInPrice.BackColor = Color.Green;
				else
					tbSearchInPrice.BackColor = Color.Red;
			}
			else
			{
				tbSearchInPrice.BackColor = Color.White;
				tbSearchInPrice.ForeColor = Color.Black;
			}
		}

		private void tbSearchInPrice_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				buttonSearchNext_Click(sender, e);
			}
		}

		private void buttonCreateMail_Click(object sender, EventArgs e)
		{
			try
			{
				if (indgvPrice.CurrentRow == null)
				{
					MessageBox.Show("Выберите прайс-лист", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				var currencyManager = (CurrencyManager)BindingContext[indgvPrice.DataSource, indgvPrice.DataMember];
				var dataRowView = (DataRowView) currencyManager.Current;
				var row = dataRowView.Row;

				var priceCode = Convert.ToInt64(row[PPriceCode.ColumnName]);
				var firmCode = GetFirmCodeByPriceCode(priceCode);
				if (firmCode > 0)
					System.Diagnostics.Process.Start(String.Format("mailto:{0}", GetContactText(firmCode, 2, 0)));
				else
					MessageBox.Show("Ошибка при попытке создать письмо", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception ex)
			{
				_logger.Error("Ошибка при попытке создать письмо ответственному за прайс-лист из вкладки 'Фирмы'", ex);
				MessageBox.Show("Ошибка при попытке создать письмо", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private long GetFirmCodeByPriceCode(long priceCode)
		{
			if (connection.State == ConnectionState.Closed)
				connection.Open();
			try
			{
				var firmCode = Convert.ToInt64(
					MySqlHelper.ExecuteScalar(connection,
					"select FirmCode from usersettings.pricesdata where PriceCode = ?SelfPriceCode",
					new MySqlParameter("?SelfPriceCode", priceCode)));
				return firmCode;
			}
			catch (Exception ex)
			{
				_logger.Error("Ошибка при попытке получить FirmCode по коду прайса", ex);
				return -1;
			}
			finally
			{
				connection.Close();
			}
		}

		private void SavePriceButton_Click(object sender, EventArgs e)
		{
			var row = (indgvPrice.CurrentRow.DataBoundItem as DataRowView).Row;
			var id = row[PPriceItemId.ColumnName].ToString();
			var file = _priceFileFormatHelper.CurrentShortFileName;
			if (!String.IsNullOrEmpty(file))
			{
				file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), file);
				if (LoadFileFromBase(id, file))
					MessageBox.Show(String.Format("Прайс сохранен на рабочий стол, файл {0}", Path.GetFileName(file)));
			}
		}

		private void MatchPriceButton_Click(object sender, EventArgs e)
		{
			var row = (indgvPrice.CurrentRow.DataBoundItem as DataRowView).Row;
			var priceItemId = Convert.ToUInt32(row[PPriceItemId.ColumnName]);
			var priceCode = Convert.ToUInt32(row[PPriceCode.ColumnName]);
			matcher.Start(priceItemId, priceCode);
			MatchPriceButton.Enabled = false;
		}

		public void EnableMatchBtn()
		{
			if(!MatchPriceButton.Enabled)
				MatchPriceButton.Enabled = true;
		}

		private void cbSource_SelectedValueChanged(object sender, EventArgs e)
		{
			if (!InSearch)
			{
				tmrSearch.Stop();
				tmrSearch.Start();
			}
		}

		private void createNewPriceCollumn_Click(object sender, EventArgs e)
		{
			var currencyManager = (CurrencyManager)BindingContext[indgvPrice.DataSource, indgvPrice.DataMember];
			var dataRowView = (DataRowView) currencyManager.Current;
			var row = dataRowView.Row;

			var priceCode = Convert.ToUInt32(row[PPriceCode.ColumnName]);

			var collumnCreator = new CostCollumnCreator(field => {
				var smtp = new SmtpClient("box.analit.net");
#if !DEBUG
				var mailToAdress = "RegisterList@subscribe.analit.net";
#else
				var mailToAdress = "a.zolotarev@analit.net";
#endif
				var message = new MailMessage();
				message.To.Add(mailToAdress);
				message.Subject = String.Format("\"{0}\" - регистрация ценовой колонки", field.ShortName);
				message.From = new MailAddress("register@analit.net");
				message.Body = String.Format(
@"Оператор: {0} 
Поставщик: {1}
Регион: {2}
Прайс-лист: {3}
", field.OperatorName, field.ShortName, field.Region, field.PriceName);
				smtp.Send(message);
			});

			collumnCreator.CreateCost(priceCode, connection, "FREditor");

			RefreshDataBind();
		}

		private void createCostCollumnInManyFilesPrice_Click(object sender, EventArgs e)
		{
			var firmCode = (long)(((DataRowView)indgvFirm.CurrentRow.DataBoundItem)[CCode.ColumnName]);

			createNewPriceCollumn_Click(null, null);
			checkBoxShowDisabled.Checked = true;
			
			var currencyManager = (CurrencyManager)BindingContext[indgvFirm.DataSource, indgvFirm.DataMember];
			var count = 0;
			foreach (var row in indgvFirm.Rows) {
				((DataGridViewRow)row).Selected = false;
				if ((long)((DataRowView)(((DataGridViewRow)row).DataBoundItem))[CCode.ColumnName] == firmCode)
					currencyManager.Position = count;
				count++;
			}
		}

		private void indgvFirm_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (indgvPrice.CurrentRow != null) {
				var item = (DataRowView)indgvPrice.CurrentRow.DataBoundItem;
				var cost_type = 0;
				Int32.TryParse(item[PCostType.ColumnName].ToString(), out cost_type);
				if (cost_type > 0)
					createCostCollumnInManyFilesPrice.Enabled = true;
				else
					createCostCollumnInManyFilesPrice.Enabled = false;
			}
		}
	}

	public class WaitWindowThread
	{
		private OleDbDataAdapter da;
		private DataTable dt;
		private frmWait fw;

		public WaitWindowThread(frmWait FW, OleDbDataAdapter DA, DataTable DT)
		{
			da = DA;
			dt = DT;
			fw = FW;
		}

		public void ThreadProc()
		{
			da.Fill(dt);
			fw.Stop = true;
			fw.DialogResult = DialogResult.OK;
		}
	}

	public class TxtFieldDef : IComparer
	{
		public string fieldName;
		public int posBegin;
		public int posEnd;

		public TxtFieldDef()
		{
		}

		public TxtFieldDef(string AFieldName, int AposBegin, int AposEnd)
		{
			fieldName = AFieldName;
			posBegin = AposBegin;
			posEnd = AposEnd;
		}

		int IComparer.Compare(Object x, Object y)
		{
			return (((TxtFieldDef)x).posBegin - ((TxtFieldDef)y).posBegin);
		}
	}
	public class CoreCost
	{
		public Int64 costCode = -1;
		public bool baseCost;
		public string costName = String.Empty;
		public string fieldName = String.Empty;
		public int txtBegin = -1;
		public int txtEnd = -1;
		public decimal cost;

		public CoreCost(Int64 ACostCode, string ACostName, bool ABaseCost, string AFieldName, int ATxtBegin, int ATxtEnd)
		{
			costCode = ACostCode;
			baseCost = ABaseCost;
			costName = ACostName;
			fieldName = AFieldName;
			txtBegin = ATxtBegin;
			txtEnd = ATxtEnd;
		}
	}
	class DropField
	{
		public string FieldName;
		public string FieldBegin;
		public string FieldEnd;
		public int RowNumber;

		public DropField(string fieldName, int rowNumber)
		{
			FieldName = fieldName;

			RowNumber = rowNumber;
		}

		public DropField(string fieldBegin, string fieldEnd, int rowNumber)
		{
			FieldBegin = fieldBegin;
			FieldEnd = fieldEnd;

			RowNumber = rowNumber;
		}
	}

	class DropSheet
	{
		public string SheetName;

		public DropSheet(string sheetName)
		{
			SheetName = sheetName;
		}
	}
}
