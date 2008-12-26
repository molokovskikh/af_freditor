using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;
using System.Data.OleDb;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Inforoom.WinForms;
using FREditor.Properties;

namespace FREditor
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    /// 
    // рабочий INDataGridView
    public enum dgFocus : int
    {
        Firm,
        Price
    }
    public enum PriceFields : int
    {
        Code,
        CodeCr,
        Name1,
        Name2,
        Name3,
        FirmCr,
        Unit,
        Volume,
        Quantity,
        Note,
        Period,
        Doc,
        Currency,
        MinBoundCost,
        Junk,
        Await,
        OriginalName,
        VitallyImportant,
        RequestRatio,
        RegistryCost,
        MaxBoundCost,
		OrderCost,
		MinOrderCount
    }

	public enum PriceFormat
	{ 
		DelimWIN = 1,
		DelimDOS,
		XLS,
		DBF,
		XML,
		FixedWIN,
		FixedDOS
	}

    public partial class frmFREMain : System.Windows.Forms.Form
    {
        ArrayList gds = new ArrayList();
        ArrayList dtables = new ArrayList();
        ArrayList tblstyles = new ArrayList();
        DataTable dtPrice = new DataTable();

#if DEBUG
#if WorkDB
		private MySqlConnection MyCn = new MySqlConnection("server=SQL.analit.net; user id=Morozov; password=Srt38123; database=farm;convert Zero Datetime=True;default command timeout=0;");
#else
		private MySqlConnection MyCn = new MySqlConnection("server=testSQL.analit.net; user id=system; password=newpass; database=farm;convert Zero Datetime=True;default command timeout=0;");
#endif
#else
		private MySqlConnection MyCn = new MySqlConnection("server=sql.analit.net; user id=AppFREditor; password=samepass; database=farm;convert Zero Datetime=True;default command timeout=0;");
#endif
		private MySqlCommand MyCmd = new MySqlCommand();
        private MySqlDataAdapter MyDA = new MySqlDataAdapter();
        private string BaseRegKey = "Software\\Inforoom\\FREditor";
        private string CregKey;

        private OleDbConnection dbcMain = new OleDbConnection();

#if DEBUG
#if WorkDir
		string StartPath = @"\\FMS\Prices\Base\";
#else
		string StartPath = @"C:\TEMP\Base\";
#endif
#else
		string StartPath = @"\\FMS\Prices\Base\";
#endif

		string EndPath = Path.GetTempPath();
        string TxtFilePath = String.Empty;
        string frmCaption = String.Empty;

        DataRow openedPriceDR;
        string listName = String.Empty;
        string delimiter = String.Empty;
		PriceFormat? fmt = null;
		//Текущий клиент, с которым происходит работа и текущий прайс
		long currentPriceItemId = 0;
		long currentClientCode = 0;
        long startLine = 0;
        string[] FFieldNames;
        string existParentRules = String.Empty;

        string nameR = String.Empty;
        string FilterParams = String.Empty;

        private string shortNameFilter = String.Empty;
        private int regionCodeFilter = -1;
        private int segmentFilter = -1;

        bool fileExist = false;
        bool InSearch = false;

        StringFormat sf = new StringFormat();

        ArrayList fds;

        public frmWait fW = null;
        public frmNameMask frmNM = null;

        public DataTable dtCostTypes;
        public DataTable dtPriceTypes;
        public DataTable dtFirmSegment;
        protected dgFocus fcs;

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

            dtFirmSegment = new DataTable("FirmSegment");
            dtFirmSegment.Columns.Add("ID", typeof(int));
            dtFirmSegment.Columns.Add("Segment", typeof(string));
            dtFirmSegment.Rows.Add(new object[] { -1, "Все" });
            dtFirmSegment.Rows.Add(new object[] { 0, "Опт" });
            dtFirmSegment.Rows.Add(new object[] { 1, "Розница" });

            cbSegment.DataSource = dtFirmSegment;
            cbSegment.DisplayMember = "Segment";
            cbSegment.ValueMember = "ID";

			this.mcmdInsertCostRules.CommandText = @"
INSERT INTO usersettings.PricesCosts (PriceCode, BaseCost, PriceItemId, CostName) values (?PriceCode, 0, ?PriceItemId, ?CostName);
SET @NewPriceCostId:=Last_Insert_ID();
INSERT INTO farm.costformrules (CostCode, FieldName, TxtBegin, TxtEnd) values (@NewPriceCostId, ?FieldName, ?TxtBegin, ?TxtEnd);
";
			this.mcmdInsertCostRules.Parameters.Add("?PriceItemId", MySql.Data.MySqlClient.MySqlDbType.Int64, 0, "CFRPriceItemId");
			this.mcmdInsertCostRules.Parameters.Add("?PriceCode", MySql.Data.MySqlClient.MySqlDbType.Int64, 0);
			this.mcmdInsertCostRules.Parameters.Add("?CostName", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, "CFRCostName");
			this.mcmdInsertCostRules.Parameters.Add("?FieldName", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, "CFRFieldName");
			this.mcmdInsertCostRules.Parameters.Add("?TxtBegin", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, "CFRTextBegin");
			this.mcmdInsertCostRules.Parameters.Add("?TxtEnd", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, "CFRTextEnd");

			this.mcmdDeleteCostRules.CommandText = "usersettings.DeleteCost";
			this.mcmdDeleteCostRules.CommandType = CommandType.StoredProcedure;
			this.mcmdDeleteCostRules.Parameters.Clear();
			this.mcmdDeleteCostRules.Parameters.Add("?inCostCode", MySql.Data.MySqlClient.MySqlDbType.Int64, 0, "CFRCost_Code");
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

            this.mcmdUpdateCostRules.Parameters.Add("?CostCode", MySql.Data.MySqlClient.MySqlDbType.Int64, 0, "CFRCost_Code");
			this.mcmdUpdateCostRules.Parameters.Add("?CostName", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, "CFRCostName");
			this.mcmdUpdateCostRules.Parameters.Add("?FieldName", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, "CFRFieldName");
			this.mcmdUpdateCostRules.Parameters.Add("?TxtBegin", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, "CFRTextBegin");
			this.mcmdUpdateCostRules.Parameters.Add("?TxtEnd", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, "CFRTextEnd");

            this.mcmdUpdateFormRules.CommandText =
@"UPDATE formrules SET
  ParentFormRules = ?FRRules,

  JunkPos = ?FRSelfJunkPos,
  AwaitPos = ?FRSelfAwaitPos,
  VitallyImportantMask = ?FRSelfVitallyImportantMask,

  PriceFormatId = ?FRPriceFormatId,

  StartLine = ?FRStartLine,
  Currency = ?FRCurrency,
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
  TxtCurrencyBegin = ?FRTxtCurrencyBegin,
  TxtCurrencyEnd = ?FRTxtCurrencyEnd,
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

  FCode = ?FRFCode,
  FCodeCr = ?FRFCodeCr,
  FName1 = ?FRFName1,
  FName2 = ?FRFName2,
  FName3 = ?FRFName3,
  FFirmCr = ?FRFFirmCr,
  FMinBoundCost = ?FRFMinBoundCost,
  FCurrency = ?FRFCurrency,
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


  Memo = ?FRMemo
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
  Id = ?FRPriceItemId";
			this.mcmdUpdateFormRules.Parameters.Add("?FRFormID", MySql.Data.MySqlClient.MySqlDbType.Int64);
			this.mcmdUpdateFormRules.Parameters.Add("?FRSelfPriceCode", MySql.Data.MySqlClient.MySqlDbType.Int64);
			this.mcmdUpdateFormRules.Parameters.Add("?FRPriceItemId", MySql.Data.MySqlClient.MySqlDbType.Int64);
			this.mcmdUpdateFormRules.Parameters.Add("?FRSynonyms", MySql.Data.MySqlClient.MySqlDbType.Int64);
            this.mcmdUpdateFormRules.Parameters.Add("?FRRules", MySql.Data.MySqlClient.MySqlDbType.Int64);
            this.mcmdUpdateFormRules.Parameters.Add("?FRStartLine", MySql.Data.MySqlClient.MySqlDbType.Int32);

            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtCodeBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtCodeEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtCodeCrBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtCodeCrEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtNameBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtNameEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtFirmCrBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtFirmCrEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtMinBoundCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtMinBoundCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtCurrencyBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtCurrencyEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtUnitBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtUnitEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtVolumeBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtVolumeEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtQuantityBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtQuantityEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtNoteBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtNoteEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtPeriodBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtPeriodEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtDocBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtDocEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtJunkBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtJunkEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtAwaitBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtAwaitEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtRequestRatioBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtRequestRatioEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtRegistryCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtRegistryCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtVitallyImportantBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtVitallyImportantEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtMaxBoundCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUpdateFormRules.Parameters.Add("?FRTxtMaxBoundCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
			this.mcmdUpdateFormRules.Parameters.Add("?FRTxtOrderCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
			this.mcmdUpdateFormRules.Parameters.Add("?FRTxtOrderCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
			this.mcmdUpdateFormRules.Parameters.Add("?FRTxtMinOrderCountBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
			this.mcmdUpdateFormRules.Parameters.Add("?FRTxtMinOrderCountEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);			

            this.mcmdUpdateFormRules.Parameters.Add("?FRCurrency", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRDelimiter", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRPosNum", MySql.Data.MySqlClient.MySqlDbType.Int64);
            this.mcmdUpdateFormRules.Parameters.Add("?FRSelfJunkPos", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRSelfAwaitPos", MySql.Data.MySqlClient.MySqlDbType.VarString);
			this.mcmdUpdateFormRules.Parameters.Add("?FRPriceFormatId", MySql.Data.MySqlClient.MySqlDbType.Int64);
            this.mcmdUpdateFormRules.Parameters.Add("?FRListName", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRNameMask", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRForbWords", MySql.Data.MySqlClient.MySqlDbType.VarString);
			this.mcmdUpdateFormRules.Parameters.Add("?FRSelfVitallyImportantMask", MySql.Data.MySqlClient.MySqlDbType.VarString);

            this.mcmdUpdateFormRules.Parameters.Add("?FRFCode", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFCodeCr", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFName1", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFName2", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFName3", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFFirmCr", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFMinBoundCost", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFCurrency", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFUnit", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFVolume", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFQuantity", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFNote", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFPeriod", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFDoc", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFJunk", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFAwait", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFRequestRatio", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFRegistryCost", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFVitallyImportant", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUpdateFormRules.Parameters.Add("?FRFMaxBoundCost", MySql.Data.MySqlClient.MySqlDbType.VarString);
			this.mcmdUpdateFormRules.Parameters.Add("?FRFOrderCost", MySql.Data.MySqlClient.MySqlDbType.VarString);
			this.mcmdUpdateFormRules.Parameters.Add("?FRFMinOrderCount", MySql.Data.MySqlClient.MySqlDbType.VarString);			
            this.mcmdUpdateFormRules.Parameters.Add("?FRMemo", MySql.Data.MySqlClient.MySqlDbType.VarString);

            foreach (MySqlParameter ms in this.mcmdUpdateFormRules.Parameters)
            {
                ms.SourceColumn = ms.ParameterName.Substring(1);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(FREditorExceptionHandler.OnThreadException);
			Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            Application.Run(new frmFREMain());
        }

		static void Application_ApplicationExit(object sender, EventArgs e)
		{
			Settings.Default.Save();
		}

        private void Form1_Load(object sender, System.EventArgs e)
        {

			MyCn.Open();
			MyCmd.Connection = MyCn;
			MyDA = new MySqlDataAdapter(MyCmd);

			dtCatalogCurrencyFill();
			dtPriceFMTsFill();
			cbRegionsFill();

			MyCn.Close();

            bsCostsFormRules.SuspendBinding();
            bsFormRules.SuspendBinding();

            tbFirmName.Focus();
            tbFirmName.Select();

            sf.Alignment = StringAlignment.Center;
            sf.FormatFlags = StringFormatFlags.DirectionVertical;
            sf.LineAlignment = StringAlignment.Center;

            LoadFirmAndPriceSettings();
        }

        private void dtCatalogCurrencyFill()
        {
            dtCatalogCurrency.Clear();

            MyCmd.CommandText =
                @"SELECT 
					Currency AS CCCurrency
				FROM 
					farm.catalogcurrency Order By Currency";

            MyDA.Fill(dtCatalogCurrency);
        }

        private void FillCurrencyCmb()
        {
            ArrayList currency = new ArrayList();
            for (int i = 0; i < dtCatalogCurrency.Rows.Count; i++)
            {
                DataRow dr = dtCatalogCurrency.Rows[i];
                string item = dr["CCCurrency"].ToString();
                if (-1 == cmbMoney.Items.IndexOf(item))
                {
                    cmbMoney.Items.Add(item);
                }
            }
            cmbMoney.SelectedIndex = 0;
        }

        private void dtPriceFMTsFill()
        {
            dtPriceFMTs.Clear();

            MyCmd.CommandText = @"
SELECT 
  id as FMTId,
  Format as FMTFormat,
  FileExtention  as FMTExt
FROM 
  farm.PriceFmts 
order by Format";

            MyDA.Fill(dtPriceFMTs);
        }

        private string AddParams(string shname, int regcode, int seg)
        {
            string cmnd = String.Empty;
            if (shname != String.Empty)
                cmnd += "and cd.ShortName like ?ShortName ";
            if (regcode != 0)
                cmnd += "and r.regioncode = ?RegionCode ";
            if (seg != -1)
                cmnd += "and cd.firmSegment = ?Segment ";
            return cmnd;
        }

        private void FillTables(string shname, int regcode, int seg)
        {
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

            if (shname != String.Empty)
            {

                MyCmd.Parameters.Clear();

                if (shname != String.Empty)
                    MyCmd.Parameters.AddWithValue("?ShortName", "%" + shname + "%");
                if (regcode != 0)
                    MyCmd.Parameters.AddWithValue("?RegionCode", regcode);
                if (seg != -1)
                    MyCmd.Parameters.AddWithValue("?Segment", seg);

                FilterParams = AddParams(shname, regcode, seg);

                dtClientsFill(FilterParams);
                dtPricesFill(FilterParams);
                dtPricesCostFill(FilterParams);
                dtFormRulesFill(FilterParams);
                dtCostsFormRulesFill(FilterParams);
            }
        }

		private void FillCosts(string shname, int regcode, int seg)
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

				MyCmd.Parameters.Clear();

				if (shname != String.Empty)
					MyCmd.Parameters.AddWithValue("?ShortName", "%" + shname + "%");
				if (regcode != 0)
					MyCmd.Parameters.AddWithValue("?RegionCode", regcode);
				if (seg != -1)
					MyCmd.Parameters.AddWithValue("?Segment", seg);

				FilterParams = AddParams(shname, regcode, seg);

				dtPricesCostFill(FilterParams);
				dtCostsFormRulesFill(FilterParams);
			}
		}

        private void dtClientsFill(string param)
        {
            MyCmd.CommandText =
                @"SELECT 
					cd.FirmCode AS CCode,
					cd.ShortName AS CShortName,
					cd.FullName AS CFullName,
					r.Region AS CRegion,
                    cd.FirmSegment as CSegment
				FROM 
					usersettings.clientsdata cd
                    inner join farm.regions r on r.RegionCode = cd.RegionCode
				WHERE cd.firmtype = 0 ";
            MyCmd.CommandText += param;
            MyCmd.CommandText += " ORDER BY cd.ShortName";

            MyDA.Fill(dtClients);
        }

        private void dtPricesFill(string param)
        {
			//Выбираем прайс-листы с мультиколоночными ценами
            MyCmd.CommandText =
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
  if(pc.BaseCost = 1, 1, 0) PIsParent
FROM
  usersettings.pricesdata pd
  inner join usersettings.pricescosts pc on pc.pricecode = pd.pricecode
  inner join usersettings.PriceItems pim on pim.Id = pc.PriceItemId
  inner join usersettings.clientsdata cd on cd.FirmCode = pd.FirmCode
  inner join farm.formrules fr on fr.Id = pim.FormRuleId
  inner join farm.regions r on r.regioncode=cd.regioncode
where
    cd.FirmType = 0 
and ((pd.CostType = 1) or (pc.BaseCost = 1))";
            MyCmd.CommandText += param;
            MyCmd.CommandText += @" 
order by PPriceName";

            MyDA.Fill(dtPrices);
        }

        private void cbRegionsFill()
        {
            DataTable dtRegions = new DataTable("Region ");
            dtRegions.Columns.Add("RegionCode", typeof(int));
            dtRegions.Columns.Add("Region", typeof(string));
            dtRegions.Clear();
            dtRegions.Rows.Add(new object[] { -1, "Все" });
            MyCmd.CommandText = @"
SELECT
    RegionCode,
    Region
FROM
    Regions
WHERE regionCode > 0
Order By Region
";
            MyDA.Fill(dtRegions);

			cbRegions.DataSource = dtRegions;
            cbRegions.DisplayMember = "Region";
            cbRegions.ValueMember = "RegionCode";

        }

        private void dtPricesCostFill(string param)
        {
            MyCmd.CommandText =
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
  inner join usersettings.clientsdata cd on cd.firmcode = pd.firmcode
  inner join farm.regions r on r.regioncode=cd.regioncode
where
      cd.firmtype = 0
";
            MyCmd.CommandText += param;
            MyCmd.CommandText += @"  
order by PCCostName";

			MyDA.Fill(dtPricesCost);
        }

        private void dtCostsFormRulesFill(string param)
        {
            MyCmd.CommandText =
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
  inner join usersettings.pricescosts pc on pc.CostCode = cfr.costcode
  inner join usersettings.pricesdata pd on pd.pricecode = pc.pricecode
  inner join usersettings.clientsdata cd on cd.firmcode = pd.firmcode
  inner join farm.regions r on r.regioncode=cd.regioncode
where 
     cd.firmtype = 0 
and pd.CostType is not null
";
            MyCmd.CommandText += param;
            MyCmd.CommandText += @"   
order by CFRCostName";

            MyDA.Fill(dtCostsFormRules);
        }

        private void dtFormRulesFill(string param)
        {
            MyCmd.CommandText =
				@"
SELECT
    fr.Id as FRFormID,

    pim.Id As FRPriceItemId,
    
    pd.PriceCode as FRSelfPriceCode,
    pd.ParentSynonym AS FRSynonyms,

	FR.ParentFormRules AS FRRules,

    PFR.PriceFormatId as FRPriceFormatId,

    FR.Currency AS FRCurrency,
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

	PFR.TxtCurrencyBegin as FRTxtCurrencyBegin,
	PFR.TxtCurrencyEnd as FRTxtCurrencyEnd,

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


	PFR.FCode as FRFCode,
	PFR.FCodeCr as FRFCodeCr,
	PFR.FName1 as FRFName1,
	PFR.FName2 as FRFName2,
	PFR.FName3 as FRFName3,
	PFR.FFirmCr as FRFFirmCr,
	PFR.FMinBoundCost as FRFMinBoundCost,
	PFR.FCurrency as FRFCurrency,
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
	PFR.FMinOrderCount as FRFMinOrderCount
FROM 
  UserSettings.PricesData AS PD
  INNER JOIN UserSettings.ClientsData AS CD on cd.FirmCode = pd.FirmCode and cd.FirmType = 0
  INNER JOIN farm.regions r on r.regioncode=cd.regioncode
  inner join usersettings.pricescosts pc on pc.PriceCode = pd.PriceCode
  inner join usersettings.priceitems pim on pim.Id = pc.PriceItemId
  inner join Farm.formrules AS FR ON FR.Id = pim.FormRuleId
  left join Farm.FormRules AS PFR ON PFR.id = ifnull(fr.ParentFormRules, FR.Id)
  left join Farm.PriceFMTs as pfmt on pfmt.id = PFR.PriceFormatId
where
  ((pd.CostType = 1) or (pc.BaseCost = 1))
";
            MyCmd.CommandText += param;
            MyDA.Fill(dtFormRules);
        }

        private void Form1_Closed(object sender, System.EventArgs e)
        {
            MyCn.Close();
        }

        private void DoOpenPrice(DataRow drP)
        {
            fW = new frmWait();

			try
			{

				fW.openPrice += new OpenPriceDelegate(OpenPrice);
				fW.drP = drP;

				fW.ShowDialog();			

			}
			finally
			{
				fW = null;
			}

        }

        public delegate void OpenPriceDelegate(DataRow drP);

        private void OpenPrice(DataRow drP)
        {
            ClearPrice();

            Application.DoEvents();
            DataRow[] drFR;
            drFR = dtFormRules.Select("FRPriceItemId = " + drP[PPriceItemId.ColumnName].ToString());
            existParentRules = drFR[0]["FRRules"].ToString();

            DataRow drC = drP.GetParentRow(dtClients.TableName + "-" + dtPrices.TableName);
            frmCaption = String.Format("{0}; {1}", drC["CShortName"], drC["CRegion"]);

			string shortFileNameByPriceItemId = drFR[0]["FRPriceItemId"].ToString();

            fmt = (drFR[0]["FRPriceFormatId"] is DBNull) ? null : (PriceFormat?)Convert.ToInt32(drFR[0]["FRPriceFormatId"]);

			FillParentComboBox(cmbParentRules,
				@"
select
  pim.FormRuleID,
  concat(cd.ShortName, ' (', if(pd.CostType = 1, concat(pd.PriceName, ' [Колонка] ', pc.CostName), pd.PriceName), ') - ', r.Region) PriceName
from
  usersettings.clientsdata cd,
  usersettings.pricesdata pd,
  usersettings.pricescosts pc,
  usersettings.priceitems pim,
  farm.regions r
where
  pd.FirmCode = cd.FirmCode
and pd.CostType is not null
and r.RegionCode = cd.RegionCode
and pc.PriceCode = pd.PriceCode
and ((pd.CostType = 1) or (pc.BaseCost = 1))
and pim.id = pc.PriceItemId
and pim.FormRuleID = ?ParentValue
order by PriceName",
				drFR[0]["FRRules"],
				"FormRuleId",
				"PriceName");
			FillParentComboBox(
				cmbParentSynonyms, 
				@"
select
  pd.PriceCode,
  concat(cd.ShortName, ' (', pd.PriceName, ') - ', r.Region) PriceName
from
  usersettings.clientsdata cd,
  usersettings.pricesdata pd,
  farm.regions r
where
  pd.FirmCode = cd.FirmCode
and pd.CostType is not null
and r.RegionCode = cd.RegionCode
and pd.PriceCode = ?ParentValue
order by PriceName
",
				drFR[0]["FRSynonyms"],
				"PriceCode",
				"PriceName");

            string r = ".err";
            DataRow[] exts = dtPriceFMTs.Select("FMTFormat = '" + fmt + "'");
            if (exts.Length == 1)
                r = exts[0]["FMTExt"].ToString();

            delimiter = drFR[0]["FRDelimiter"].ToString();

            string takeFile = shortFileNameByPriceItemId + r;

			PrepareShowTab(fmt);

            if (!(File.Exists(StartPath + takeFile)))
            {
                fileExist = false;
                MessageBox.Show(String.Format("Файл {0} выбранного Прайс-листа отсутствует в директории по умолчанию ({1})", takeFile, StartPath), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                fileExist = true;
                Directory.CreateDirectory(EndPath + shortFileNameByPriceItemId);

                File.Copy(StartPath + takeFile, EndPath + shortFileNameByPriceItemId + "\\" + takeFile, true);
                string filePath = EndPath + shortFileNameByPriceItemId + "\\" + takeFile;

                Application.DoEvents();

                if ((fmt == PriceFormat.DBF))
                {
                    OpenDBFFile(filePath);
                }
                else
					if (fmt == PriceFormat.XLS)
                    {
                        listName = drFR[0]["FRListName"].ToString();
                        OpenEXLFile(filePath);
                    }
                    else
						if ((fmt == PriceFormat.DelimDOS) || (fmt == PriceFormat.DelimWIN))
						{
							dbcMain.Close();
							dbcMain.Dispose();
							OpenTXTDFile(filePath, fmt);
						}
				else
							if ((fmt == PriceFormat.FixedDOS) || (fmt == PriceFormat.FixedWIN))
							{
								dbcMain.Close();
								dbcMain.Dispose();
								startLine = drFR[0]["FRStartLine"] is DBNull ? -1 : Convert.ToInt64(drFR[0]["FRStartLine"]);

								TxtFilePath = EndPath + shortFileNameByPriceItemId + "\\" + takeFile;
								dtMarking.Clear();
								OpenTXTFFile(filePath, drFR[0]);
							}
                Application.DoEvents();
                ShowTab(fmt);
                Application.DoEvents();
                this.Text = String.Format("Редактор Правил Формализации ({0})", frmCaption);
                Application.DoEvents();
            }
        }

		private void PrepareShowTab(PriceFormat? fmt)
		{
			switch (fmt)
			{
				case PriceFormat.DelimDOS:
				case PriceFormat.DelimWIN:
					
					lBoxSheetName.Visible = false;
					txtBoxSheetName.Visible = false;
					pnlGeneralFields.Visible = true;
					pnlTxtFields.Visible = false;

					break;

				case PriceFormat.FixedDOS:
				case PriceFormat.FixedWIN:

					lBoxSheetName.Visible = false;
					txtBoxSheetName.Visible = false;
					pnlGeneralFields.Visible = false;
					pnlTxtFields.Visible = true;

					break;

				case PriceFormat.XLS:

					lBoxSheetName.Visible = true;
					txtBoxSheetName.Visible = true;
					pnlGeneralFields.Visible = true;
					pnlTxtFields.Visible = false;

					break;

				case PriceFormat.DBF:

					lBoxSheetName.Visible = false;
					txtBoxSheetName.Visible = false;
					pnlGeneralFields.Visible = true;
					pnlTxtFields.Visible = false;

					break;
			}

			if (existParentRules == String.Empty)
			{
				txtBoxSelfAwaitPos.ReadOnly = false;
				txtBoxSelfJunkPos.ReadOnly = false;
				txtBoxNameMask.ReadOnly = false;
				txtBoxForbWords.ReadOnly = false;
			}
			else
			{
				txtBoxSelfAwaitPos.ReadOnly = true;
				txtBoxSelfJunkPos.ReadOnly = true;
				txtBoxNameMask.ReadOnly = true;
				txtBoxForbWords.ReadOnly = true;
			}
		}

		private void FillParentComboBox(ComboBox cmbParent, string FillSQL, object ParentValue, string IdField, string NameField)
		{
			DataSet ParentDS = MySqlHelper.ExecuteDataset(MyCn, FillSQL, new MySqlParameter("?ParentValue", ParentValue));
			FillParentComboBoxFromTable(cmbParent, ParentDS.Tables[0], IdField, NameField);
		}

		private void FillParentComboBoxBySearch(ComboBox cmbParent, string FillSQL, string IdField, string NameField)
		{
			//Возможно есть способ более проще получить значение биндинга
			object SelectedValue = ((DataRowView)bsFormRules.Current).Row[cmbParent.DataBindings["SelectedValue"].BindingMemberInfo.BindingField];
			DataSet ParentDS = MySqlHelper.ExecuteDataset(
				MyCn, 
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
					tcInnerTable.SizeMode = TabSizeMode.Fixed;
					tcInnerTable.ItemSize = new Size(0, 1);
					tcInnerTable.Appearance = TabAppearance.Buttons;

					tcInnerSheets.SizeMode = TabSizeMode.Normal;
					tcInnerSheets.ItemSize = new Size(58, 18);
					tcInnerSheets.Appearance = TabAppearance.Normal;

					break;

				case PriceFormat.DBF:
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

        private void OpenDBFFile(string filePath)
        {
            Application.DoEvents();
            dbcMain.ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"dBase 5.0\"", System.IO.Path.GetDirectoryName(filePath));
            dbcMain.Open();
            try
            {
                Application.DoEvents();
                OleDbDataAdapter da = new OleDbDataAdapter(String.Format("select * from [{0}]", System.IO.Path.GetFileNameWithoutExtension(filePath)), dbcMain);
                CreateThread(da, dtPrice, indgvPriceData);
            }
            finally
            {
                dbcMain.Close();
                dbcMain.Dispose();
                Application.DoEvents();
            }
        }

        private void OpenEXLFile(string filePath)
        {
            Application.DoEvents();
            dbcMain.ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 5.0;HDR=No;IMEX=1\"", filePath);
            bool res = false;
            do
            {
                dbcMain.Open();
                try
                {
                    Application.DoEvents();
                    DataTable TableNames = dbcMain.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                        new object[] { null, null, null, "TABLE" });
                    string[] Sheet = null;
                    Sheet = new string[TableNames.Rows.Count];

                    Sheet[0] = (string)TableNames.Rows[0]["TABLE_NAME"];
                    tbpSheet1.Text = Sheet[0];

                    INDataGridView indgv;
                    for (int i = 1; i < TableNames.Rows.Count; i++)
                    {
                        DataRow dr = TableNames.Rows[i];
                        if (!(dr["TABLE_NAME"] is DBNull))
                        {
                            Sheet[i] = (string)dr["TABLE_NAME"];
                            TabPage tp = new TabPage();
                            tp.Name = "tbpSheet" + (i + 1);
                            tp.Text = Sheet[i];
                            tcInnerSheets.TabPages.Add(tp);

                            indgv = new INDataGridView();
                            indgv.Name = "indgvPriceData" + (i + 1);
                            indgv.Parent = tp;
                            indgv.KeyDown += new System.Windows.Forms.KeyEventHandler(indgvPriceData_KeyDown);
                            indgv.Dock = DockStyle.Fill;
                            indgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                            indgv.ReadOnly = true;
                            indgv.RowHeadersVisible = false;
                            indgv.MouseDown += new System.Windows.Forms.MouseEventHandler(this.indgvPriceData_MouseDown);
                            foreach (DataGridViewTextBoxColumn dc in indgv.Columns)
                                dc.Width = 300;
                            gds.Add(indgv);

                            DataTable dt = new DataTable();
                            dt.TableName = "Прайс" + (i + 1);
                            dtables.Add(dt);
                        }
                        Application.DoEvents();
                    }
                    for (int j = 0; j < TableNames.Rows.Count; j++)
                    {
                        try
                        {
                            DataTable ColumnNames = dbcMain.GetOleDbSchemaTable(OleDbSchemaGuid.Columns,
                                new object[] { null, null, Sheet[j], null });
                            string FieldNames = "F1";
                            int MaxColCount = (ColumnNames.Rows.Count >= 256) ? 255 : ColumnNames.Rows.Count;
                            for (int i = 1; i < MaxColCount; i++)
                            {
                                FieldNames = FieldNames + ", F" + Convert.ToString(i + 1);
                            }
                            OleDbDataAdapter da = new OleDbDataAdapter(String.Format("select {0} from [{1}]", FieldNames, Sheet[j]), dbcMain);
                            if (j == 0)
                            {
                                CreateThread(da, dtPrice, indgvPriceData);
                            }
                            else
                            {
                                ((INDataGridView)gds[j - 1]).Columns.Clear();
                                CreateThread(da, ((DataTable)(dtables[j - 1])), ((INDataGridView)gds[j - 1]));
                                ((INDataGridView)gds[j - 1]).DataSource = ((DataTable)(dtables[j - 1]));
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                finally
                {
                    dbcMain.Close();
                    dbcMain.Dispose();
                    Application.DoEvents();
                }
                res = true;
            } while (!res);
        }

        private void OpenTXTDFile(string filePath, PriceFormat? fmt)
        {
            using (StreamWriter w = new StreamWriter(Path.GetDirectoryName(filePath) + Path.DirectorySeparatorChar + "Schema.ini", false, Encoding.GetEncoding(1251)))
            {
                w.WriteLine("[" + Path.GetFileName(filePath) + "]");
                w.WriteLine((fmt == PriceFormat.DelimWIN) ? "CharacterSet=ANSI" : "CharacterSet=OEM");
                w.WriteLine(("TAB" == delimiter.ToUpper()) ? "Format=TabDelimited" : "Format=Delimited(" + delimiter + ")");
                w.WriteLine("ColNameHeader=False");
                w.WriteLine("MaxScanRows=300");
            }

			string replaceFile;
			using (StreamReader r = new StreamReader(filePath, Encoding.GetEncoding(1251)))
			{
				replaceFile = r.ReadToEnd();
			}

			replaceFile = replaceFile.Replace("\"", "");

			using (StreamWriter rw = new StreamWriter(filePath, false, Encoding.GetEncoding(1251)))
			{
				rw.Write(replaceFile);
			}

			int MaxColCount = 0;
            string TableName = System.IO.Path.GetFileName(filePath).Replace(".", "#");
            dbcMain.ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Text\"", System.IO.Path.GetDirectoryName(filePath));

            Application.DoEvents();

            dbcMain.Open();
            DataTable ColumnNames = dbcMain.GetOleDbSchemaTable(OleDbSchemaGuid.Columns,
                new object[] { null, null, TableName, null });
            MaxColCount = (ColumnNames.Rows.Count >= 256) ? 255 : ColumnNames.Rows.Count;
            dbcMain.Close();
            dbcMain.Dispose();

            Application.DoEvents();

            using (StreamWriter w = new StreamWriter(Path.GetDirectoryName(filePath) + Path.DirectorySeparatorChar + "Schema.ini", false, Encoding.GetEncoding(1251)))
            {
                w.WriteLine("[" + Path.GetFileName(filePath) + "]");
				w.WriteLine((fmt == PriceFormat.DelimWIN) ? "CharacterSet=ANSI" : "CharacterSet=OEM");
                w.WriteLine(("TAB" == delimiter.ToUpper()) ? "Format=TabDelimited" : "Format=Delimited(" + delimiter + ")");
                w.WriteLine("ColNameHeader=False");
                w.WriteLine("MaxScanRows=300");
                for (int i = 0; i <= MaxColCount; i++)
                {
                    w.WriteLine("Col{0}=F{0} Text", i);
                }
            }

            Application.DoEvents();
            dbcMain.ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Text\"", System.IO.Path.GetDirectoryName(filePath));
            dbcMain.Open();
            OleDbDataAdapter da = new OleDbDataAdapter(String.Format("select * from {0}", System.IO.Path.GetFileName(filePath).Replace(".", "#")), dbcMain);

            CreateThread(da, dtPrice, indgvPriceData);
            dbcMain.Close();
            dbcMain.Dispose();
            Application.DoEvents();
        }

        public void SetFieldName(PriceFields PF, string Value)
        {
            FFieldNames[(int)PF] = Value;
        }

        public string GetFieldName(PriceFields PF)
        {
            return FFieldNames[(int)PF];
        }

        private void FillMarking(string filePath, DataRow dr)
        {
            FFieldNames = new string[Enum.GetNames(typeof(PriceFields)).Length];

            string TmpName;
            int TmpIndex;

            foreach (PriceFields pf in Enum.GetValues(typeof(PriceFields)))
            {
                if (PriceFields.OriginalName == pf || PriceFields.Name1 == pf)//|| PriceFields.Name2 == pf || PriceFields.Name3 == pf)
                    TmpName = "Name";
                else
                    TmpName = pf.ToString();
                try
                {
                    TmpIndex = dr.Table.Columns.IndexOf("FRTxt" + TmpName + "Begin");
                }
                catch
                {
                    TmpIndex = -1;
                }
                SetFieldName(pf, ((-1 < TmpIndex) && !(dr["FRTxt" + TmpName + "Begin"] is DBNull)) ? TmpName : String.Empty);
            }

            fds = new ArrayList();
            int TxtBegin, TxtEnd;
            foreach (PriceFields pf in Enum.GetValues(typeof(PriceFields)))
            {
                TmpName = GetFieldName(pf);
				if ((PriceFields.OriginalName != pf) && (String.Empty != TmpName))
                {
                    try
                    {
                        TxtBegin = Convert.ToInt32(dr["FRTxt" + TmpName + "Begin"]);
                        TxtEnd = Convert.ToInt32(dr["FRTxt" + TmpName + "End"]);
                        fds.Add(new TxtFieldDef(TmpName, TxtBegin, TxtEnd));
                    }
                    catch { }
                }
            }

			//Добавляем в список цены, если у них выставленны границы
			foreach (DataRowView drv in bsCostsFormRules)
			{
				if (!(drv[CFRTextBegin.ColumnName] is DBNull) && !(drv[CFRTextEnd.ColumnName] is DBNull))
					fds.Add(new TxtFieldDef(
						"Cost" + drv[CFRCost_Code.ColumnName].ToString(),
						Convert.ToInt32(drv[CFRTextBegin.ColumnName]),
						Convert.ToInt32(drv[CFRTextEnd.ColumnName])));
			}
			

            fds.Sort(new TxtFieldDef());

            DataRow mdr;
            int countx = 1;

            if (fds.Count > 0)
            {
                TxtFieldDef prevTFD, currTFD = (TxtFieldDef)fds[0];

                if (1 == currTFD.posBegin)
                {
                    mdr = dtMarking.NewRow();
                    mdr["MNameField"] = currTFD.fieldName;
                    mdr["MBeginField"] = 1;
                    mdr["MEndField"] = currTFD.posEnd;
                    dtMarking.Rows.Add(mdr);
                }
                else
                {
                    mdr = dtMarking.NewRow();
                    mdr["MNameField"] = String.Format("x{0}", countx);
                    mdr["MBeginField"] = 1;
                    mdr["MEndField"] = currTFD.posBegin-1;
                    dtMarking.Rows.Add(mdr);

                    countx++;

                    mdr = dtMarking.NewRow();
                    mdr["MNameField"] = currTFD.fieldName;
                    mdr["MBeginField"] = currTFD.posBegin;
                    mdr["MEndField"] = currTFD.posEnd;
                    dtMarking.Rows.Add(mdr);
                }
                int i = 1;
                for (i = 1; i <= fds.Count - 1; i++)
                {
                    prevTFD = (TxtFieldDef)fds[i - 1];
                    currTFD = (TxtFieldDef)fds[i];
                    if (currTFD.posBegin == prevTFD.posEnd + 1)
                    {
                        mdr = dtMarking.NewRow();
                        mdr["MNameField"] = currTFD.fieldName;
                        mdr["MBeginField"] = currTFD.posBegin;
                        mdr["MEndField"] = currTFD.posEnd;
                        dtMarking.Rows.Add(mdr);
                    }
                    else
                    {
                        mdr = dtMarking.NewRow();
                        mdr["MNameField"] = String.Format("x{0}", countx);
                        mdr["MBeginField"] = prevTFD.posEnd + 1;
                        mdr["MEndField"] = currTFD.posBegin - 1;
                        dtMarking.Rows.Add(mdr);

                        countx++;

                        mdr = dtMarking.NewRow();
                        mdr["MNameField"] = currTFD.fieldName;
                        mdr["MBeginField"] = currTFD.posBegin;
                        mdr["MEndField"] = currTFD.posEnd;
                        dtMarking.Rows.Add(mdr);
                    }
                }
                TxtFieldDef lastTFD = (TxtFieldDef)fds[i - 1];
                if (lastTFD.posEnd < 255)
                {
                    mdr = dtMarking.NewRow();
                    mdr["MNameField"] = String.Format("x{0}", countx);
                    mdr["MBeginField"] = lastTFD.posEnd + 1;
                    mdr["MEndField"] = 255;
                    dtMarking.Rows.Add(mdr);
                }
            }
            else
            {
                mdr = dtMarking.NewRow();
                mdr["MNameField"] = "x1";
                mdr["MBeginField"] = 1;
                mdr["MEndField"] = 255;
                dtMarking.Rows.Add(mdr);
            }
			dtMarking.AcceptChanges();
        }

        private void OpenTXTFFile(string filePath, DataRow dr)
        {
            FillMarking(filePath, dr);
            OpenTable(fmt);
        }

        private void DelCostsColumns()
        {
            indgvCosts.Columns[cFRFieldNameDataGridViewTextBoxColumn.Name].Visible = false;
            indgvCosts.Columns[cFRTextBeginDataGridViewTextBoxColumn.Name].Visible = false;
            indgvCosts.Columns[cFRTextEndDataGridViewTextBoxColumn.Name].Visible = false;
        }

        public string strToANSI(string Dest)
        {
            System.Text.Encoding ansi = System.Text.Encoding.GetEncoding(1251);
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

        private bool Check_Marking()
        {
            bool flag = true;
            int i = 1;
            if (dtMarking.Rows.Count > 2)
            {
				DataTable newMarking = dtMarking.DefaultView.ToTable();
				while ((i < newMarking.Rows.Count) && (flag))
                {
					DataRow drP = newMarking.Rows[i - 1];
					DataRow drN = newMarking.Rows[i];
                    if ((!(drP["MBeginField"] is DBNull)) || (!(drP["MEndField"] is DBNull)))
                    {
                        int beg = Convert.ToInt32(drP["MBeginField"]);
                        int end = Convert.ToInt32(drP["MEndField"]);
                        if (beg - end > 0)
                        {
                            flag = false;
                        }
                        else
                            if (!(drN["MEndField"] is DBNull))
                            {
                                if (Convert.ToInt32(drN["MBeginField"]) - Convert.ToInt32(drP["MEndField"]) != 1)
                                {
                                    flag = false;
                                }
                                else
                                    i++;
                            }
                            else
                                flag = false;
                    }
                    else
                        flag = false;
                }
            }
            else
            {
                int beg = Convert.ToInt32(dtMarking.Rows[0]["MBeginField"]);
                int end = Convert.ToInt32(dtMarking.Rows[0]["MEndField"]);
                if (beg - end > 0)
                {
                    flag = false;
                }
            }
            return flag;
        }

        private void OpenTable(PriceFormat? fmt)
        {
            if (dtMarking.Rows.Count > 1)
            {
                string TmpName;
                fds = new ArrayList();
                for (int i = 0; i < dtMarking.Rows.Count; i++)
                {
                    DataRow dr = dtMarking.Rows[i];

                    int TxtBegin, TxtEnd;

                    TmpName = dr["MNameField"].ToString();
                    TxtBegin = Convert.ToInt32(dr["MBeginField"]);
                    TxtEnd = Convert.ToInt32(dr["MEndField"]);
                    fds.Add(new TxtFieldDef(TmpName, TxtBegin, TxtEnd));
                }

				fds.Sort(new TxtFieldDef());

				using (StreamWriter w = new StreamWriter(Path.GetDirectoryName(TxtFilePath) + Path.DirectorySeparatorChar + "Schema.ini", false, Encoding.GetEncoding(1251)))
                {
                    w.WriteLine("[" + Path.GetFileName(TxtFilePath) + "]");
                    w.WriteLine((fmt == PriceFormat.FixedWIN) ? "CharacterSet=ANSI" : "CharacterSet=OEM");
                    w.WriteLine("Format=FixedLength");
                    w.WriteLine("ColNameHeader=False");
                    w.WriteLine("MaxScanRows=300");

                    if (fds.Count > 0)
                    {
                        int j = 1;
                        TxtFieldDef prevTFD, currTFD = (TxtFieldDef)fds[0];

                        if (1 == currTFD.posBegin)
                        {
                            w.WriteLine(String.Format("Col{0}={1} Text Width {2}", j, currTFD.fieldName, currTFD.posEnd));
                            j++;
                        }
                        else
                        {
                            w.WriteLine(String.Format("Col{0}={1} Text Width {2}", j, "x", currTFD.posBegin - 1));
                            j++;
                            w.WriteLine(String.Format("Col{0}={1} Text Width {2}", j, currTFD.fieldName, currTFD.posEnd - currTFD.posBegin + 1));
                            j++;
                        }

                        for (int i = 1; i <= fds.Count - 1; i++)
                        {
                            prevTFD = (TxtFieldDef)fds[i - 1];
                            currTFD = (TxtFieldDef)fds[i];
                            if (currTFD.posBegin == prevTFD.posEnd + 1)
                            {
                                w.WriteLine(String.Format("Col{0}={1} Text Width {2}", j, currTFD.fieldName, currTFD.posEnd - currTFD.posBegin + 1));
                                j++;
                            }
                            else
                            {
                                w.WriteLine(String.Format("Col{0}={1} Text Width {2}", j, "x", currTFD.posBegin - prevTFD.posEnd - 1));
                                j++;
                                w.WriteLine(String.Format("Col{0}={1} Text Width {2}", j, currTFD.fieldName, currTFD.posEnd - currTFD.posBegin + 1));
                                j++;
                            }
                        }
                    }
                    else
                    {
                        w.WriteLine(String.Format("Col{0}=x1 Text Width {1}", 1, 255));
                    }
                }
                dbcMain.ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Text\"", System.IO.Path.GetDirectoryName(TxtFilePath));
                dbcMain.Open();
                try
                {
                    OleDbDataAdapter da = new OleDbDataAdapter(String.Format("select * from {0}", System.IO.Path.GetFileName(TxtFilePath).Replace(".", "#")), dbcMain);
                    indgvPriceData.Columns.Clear();

					dtPrice.Rows.Clear();
					dtPrice.Columns.Clear();
					dtPrice.Clear();

                    CreateThread(da, dtPrice, indgvPriceData);
                }
                finally
                {
                    dbcMain.Close();
                    dbcMain.Dispose();
                }
            }
        }

        private void tcInnerTable_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (tcInnerTable.SelectedTab == tbpTable)
            {
                SaveMarkingSettings();
                DataTable dtTemp = dtMarking.GetChanges();
                if (!(dtTemp == null))
                {
                    if (!Check_Marking())
                    {
                        tcInnerTable.SelectedTab = tbpMarking;
                        MessageBox.Show("Неправильный ввод поля!", "Плохо, очень плохо", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        fW = new frmWait();

						try
						{
							fW.openPrice += new OpenPriceDelegate(DoOpenTable);
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
            dtMarking.AcceptChanges();
            Application.DoEvents();
            indgvPriceData.DataSource = null;
            Application.DoEvents();

            OpenTable(fmt);
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
            dtMarking.Clear();

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
            dtClientsFill(String.Empty);
            dtPricesFill(String.Empty);
            dtPricesCostFill(String.Empty);
            dtFormRulesFill(String.Empty);
            dtCatalogCurrencyFill();
            dtPriceFMTsFill();
            dtCostsFormRulesFill(String.Empty);
            cbRegionsFill();
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

        private void tbControl_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            
            if (tbControl.SelectedTab == tpFirms)
            {
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
                tmrUpdateApply.Stop();
            }
            else
                if (tbControl.SelectedTab == tpPrice)
                {
                    CurrencyManager currencyManager = (CurrencyManager)BindingContext[indgvPrice.DataSource, indgvPrice.DataMember];
                    DataRowView drv = (DataRowView)currencyManager.Current;
                    DataView dv = (DataView)currencyManager.List;

                    DataRow drP = drv.Row;
                    openedPriceDR = drP;

					currentClientCode = (long)(((DataRowView)indgvFirm.CurrentRow.DataBoundItem)[CCode.ColumnName]);
					currentPriceItemId = (long)(((DataRowView)indgvPrice.CurrentRow.DataBoundItem)[PPriceItemId.ColumnName]);

					bsCostsFormRules.Filter = "CFRPriceItemId = " + currentPriceItemId.ToString();
					bsFormRules.Filter = "FRPriceItemId = " + currentPriceItemId.ToString();
					bsCostsFormRules.ResumeBinding();
					bsFormRules.ResumeBinding();

					//Разрешено добавлять и редактировать ценовые колонки только в том случае, если это мультиколоночный прайс-лист
					indgvCosts.AllowUserToAddRows = Convert.ToByte(((DataRowView)indgvPrice.CurrentRow.DataBoundItem)[PCostType.ColumnName]) == 0;
					indgvCosts.ReadOnly = !indgvCosts.AllowUserToAddRows;

					if (indgvCosts.AllowUserToAddRows)
					{
						(bsCostsFormRules.List as DataView).Table.Columns[CFRPriceItemId.ColumnName].DefaultValue = ((DataRowView)bsFormRules.Current)["FRPriceItemId"];
					}
					else
					{
						(bsCostsFormRules.List as DataView).Table.Columns[CFRPriceItemId.ColumnName].DefaultValue = DBNull.Value;
					}

					DoOpenPrice(drP);
                    tmrUpdateApply.Start();
				}
        }

		private void RefreshDataBind()
		{
			FillTables(shortNameFilter, regionCodeFilter, segmentFilter);

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

        private void MethodForThread(OleDbDataAdapter da, DataTable dt)
        {
            da.Fill(dt);
        }

        private void txtBoxCode_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DropField)))
            {
                if (existParentRules == String.Empty)
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
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
                if (existParentRules == String.Empty)
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
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
                if (existParentRules == String.Empty)
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
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

        private void CostsDataGrid_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
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

        private void txtBoxStartLine_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            ((TextBox)sender).Text = ((DropField)e.Data.GetData(typeof(DropField))).RowNumber.ToString();
        }

        private void txtBoxStartLine_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DropField)))
            {
                if (existParentRules == String.Empty)
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void lLblMaster_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
			if (MyCn.State == ConnectionState.Closed)
				MyCn.Open();
			try
			{
				long FirmCode = Convert.ToInt64(
					MySqlHelper.ExecuteScalar(MyCn, 
					"select FirmCode from usersettings.pricesdata where PriceCode = ?SelfPriceCode",
					new MySqlParameter("?SelfPriceCode", ((DataRowView)bsFormRules.Current)[FRSelfPriceCode.ColumnName])));
				System.Diagnostics.Process.Start(String.Format("mailto:{0}", GetContactText(FirmCode, 2, 0)));
			}
			finally
			{
				MyCn.Close();
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
			DataSet dsContacts = MySqlHelper.ExecuteDataset(MyCn, @"
select distinct c.contactText
from usersettings.clientsdata cd
  join contacts.contact_groups cg on cd.ContactGroupOwnerId = cg.ContactGroupOwnerId
    join contacts.contacts c on cg.Id = c.ContactOwnerId
where
    firmcode = ?FirmCode
and cg.Type = ?ContactGroupType
and c.Type = ?ContactType

union

select distinct c.contactText
from usersettings.clientsdata cd
  join contacts.contact_groups cg on cd.ContactGroupOwnerId = cg.ContactGroupOwnerId
    join contacts.persons p on cg.id = p.ContactGroupId
      join contacts.contacts c on p.Id = c.ContactOwnerId
where
    firmcode = ?FirmCode
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
            DataRowView drv = (currencyManager.Position > -1) ? (DataRowView)currencyManager.Current : null;
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

        public delegate void TestAsync(OleDbDataAdapter da, DataTable dt);

        void TestD(OleDbDataAdapter da, DataTable dt)
        {
			da.Fill(dt);
        }

        public delegate void CloseDelegate();

        void WaitClose()
        {
            fW.Close();
        }

        private void CreateThread(OleDbDataAdapter da, DataTable dt, INDataGridView dgv)
        {
            TestAsync ta = new TestAsync(TestD);
            Object state = new Object();

			DataTable temp = new DataTable();

			System.IAsyncResult ar = ta.BeginInvoke(da, dt, null, state);
			while (!ar.IsCompleted)
				Application.DoEvents();
        }

        private void btnFloatPanel_Click(object sender, System.EventArgs e)
        {
            if (pnlFloat.Visible)
            {
                pnlFloat.Visible = false;
                if (!(fmt.Equals((PriceFormat?)Convert.ToInt32(cmbFormat.SelectedValue))) || !(delimiter.Equals(tbDevider.Text)))
                        DoOpenPrice(openedPriceDR);
                
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
            e.Graphics.DrawString("Настройки", btnFloatPanel.Font, Brushes.Black, btnFloatPanel.ClientRectangle, new System.Drawing.StringFormat(sf));
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
                    r = new Regex(txtMask.Text);
                    if ((fmt == PriceFormat.FixedDOS) || (fmt == PriceFormat.FixedWIN))
                    {
                        if ((txtExistBegin.Text != String.Empty) && (txtExistEnd.Text != String.Empty))
                        {
                            foreach (DataRow dr in dtMarking.Rows)
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
                        DataRow[] drcol = dtMarking.Select("MBeginField = " + txtBoxName1Begin.Text);
                        col = drcol[0][MNameField].ToString();
                    }
                }

            return col;
        }
        private void btnEditMask_Click(object sender, System.EventArgs e)
        {
            string col = FindNameColumn();

            if (col != String.Empty)
            {
				DataRow dr = null;
				if (tcInnerSheets.SelectedIndex > 0)
					dr = ((DataTable)dtables[tcInnerSheets.SelectedIndex - 1]).Rows[((INDataGridView)gds[tcInnerSheets.SelectedIndex - 1]).CurrentRow.Index];
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

                Regex r = new Regex(txtBoxNameMask.Text);
                string[] groups = new string[17];
                int i = 0;
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
                {
                    CheckErrorsInAllNames(r, groups, FindNameColumn(), (DataTable)dtables[tcInnerSheets.SelectedIndex - 1], (INDataGridView)gds[tcInnerSheets.SelectedIndex - 1]);
                }
                else
                    CheckErrorsInAllNames(r, groups, FindNameColumn(), dtPrice, indgvPriceData);
            }
            else
                MessageBox.Show(String.Format("Не указана маска разбора товара!"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private void CheckErrorsInAllNames(Regex r, string[] GroupsToFind, string ColumnNameToSearchIn, DataTable dt, INDataGridView indgv)
        {
            dt.BeginLoadData();
            bool colExist = false;
            foreach (DataColumn c in dt.Columns)
            {
                if (c.ColumnName == ColumnNameToSearchIn)
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
                    Match m = r.Match(dr[ColumnNameToSearchIn].ToString());
                    if (m.Success)
                    {
						bool _groupNotExists = false;
                        for (int i = 0; i < GroupsToFind.Length; i++)
                        {
                            if (GroupsToFind[i] != null)
                            {
                                if (!m.Groups[GroupsToFind[i]].Success)
                                {
									_groupNotExists = true;
									break;
                                }
                            }
                        }

						if (!_groupNotExists)
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
				((CurrencyManager)BindingContext[indgvPrice.DataSource, indgvPrice.DataMember]).EndCurrentEdit();
            else
            {
				//Завершаем редактирование правил формализации цен
				if (((DataRowView)bsCostsFormRules.Current).IsNew && Convert.IsDBNull(((DataRowView)bsCostsFormRules.Current)[CFRCostName.ColumnName]))
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
				//Если мы пытаемся перейти на вкладку "Прайс", а у нас нет в списке прайсов, то делать это не нужно
				CurrencyManager currencyManager = (CurrencyManager)BindingContext[indgvPrice.DataSource, indgvPrice.DataMember];
				if (currencyManager.Count == 0)
					e.Cancel = true;
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
                        tsbApply.Enabled = false;
                        tsbCancel.Enabled = false;
                    }
                }
            }
		}

        private void tsbApply_Click(object sender, EventArgs e)
        {
            CommitAllEdit();
            DataSet chg = dtSet.GetChanges();
            if (chg != null)
            {
                if (tbControl.SelectedTab == tpPrice)
                {
                    if (MyCn.State == ConnectionState.Closed)
                        MyCn.Open();
                    try
                    {
                        MySqlTransaction tr = MyCn.BeginTransaction();
                        try
                        {
                            MySqlCommand SetCMD = new MySqlCommand("set @INHost = ?INHost; set @INUser = ?INUser;", MyCn, tr);
                            SetCMD.Parameters.AddWithValue("?INHost", Environment.MachineName);
                            SetCMD.Parameters.AddWithValue("?INUser", Environment.UserName);
                            SetCMD.ExecuteNonQuery();

                            mcmdUpdateCostRules.Connection = MyCn;
							mcmdInsertCostRules.Connection = MyCn;
							mcmdDeleteCostRules.Connection = MyCn;
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


                            mcmdUpdateFormRules.Connection = MyCn;
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

								SendNotificationLetter(body.ToString(), _priceName, _firmName, _regionName);
							}

							tr.Commit();
                            dtSet.AcceptChanges();
							//Обновляе цены и правила формализации цен для того, чтобы загрузить корректные ID новых цен
							FillCosts(shortNameFilter, regionCodeFilter, segmentFilter);

                        }
                        catch (Exception ex)
                        {
							MessageBox.Show("Не удалось применить изменения в правилах формализации прайс-листа. Сообщение было отправлено разработчику.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
							FREditorExceptionHandler.SendMessageOnException(null, new Exception("Ошибка при применении изменений в правилах формализации.", ex));
							tr.Rollback();
                        }
                    }
                    finally
                    {
                        MyCn.Close();
                    }
                }
                else if (tbControl.SelectedTab == tpFirms)
                {
                    ((CurrencyManager)BindingContext[indgvPrice.DataSource, indgvPrice.DataMember]).EndCurrentEdit();

                    if (MyCn.State == ConnectionState.Closed)
                        MyCn.Open();
                    try
                    {
                        MySqlTransaction tr = MyCn.BeginTransaction();
                        try
                        {
                            MySqlCommand SetCMD = new MySqlCommand("set @INHost = ?INHost; set @INUser = ?INUser;", MyCn, tr);
                            SetCMD.Parameters.AddWithValue("?INHost", Environment.MachineName);
                            SetCMD.Parameters.AddWithValue("?INUser", Environment.UserName);
                            SetCMD.ExecuteNonQuery();

							//todo: здесь надо переписать
                            MySqlCommand mcmdUPrice = new MySqlCommand();
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
                            mcmdUPrice.Parameters.Add("?PPriceType", MySql.Data.MySqlClient.MySqlDbType.Int32);
                            mcmdUPrice.Parameters.Add("?PCostType", MySql.Data.MySqlClient.MySqlDbType.Int32);
                            mcmdUPrice.Parameters.Add("?PWaitingDownloadInterval", MySql.Data.MySqlClient.MySqlDbType.Int32);
                            mcmdUPrice.Parameters.Add("?PMaxOld", MySql.Data.MySqlClient.MySqlDbType.Int32);
                            mcmdUPrice.Parameters.Add("?PPriceCode", MySql.Data.MySqlClient.MySqlDbType.Int64);
							mcmdUPrice.Parameters.Add("?PPriceItemId", MySql.Data.MySqlClient.MySqlDbType.Int64);
							mcmdUPrice.Parameters.Add("?PIsParent", MySql.Data.MySqlClient.MySqlDbType.Int32);

                            foreach (MySqlParameter ms in mcmdUPrice.Parameters)
                            {
                                ms.SourceColumn = ms.ParameterName.Substring(1);
                            }

                            mcmdUPrice.Connection = MyCn;
                            daPrice.UpdateCommand=mcmdUPrice;
                            daPrice.TableMappings.Clear();
                            daPrice.TableMappings.Add("Table", dtPrices.TableName);
                            daPrice.Update(chg.Tables[dtPrices.TableName]);
                            
                            tr.Commit();
                            dtSet.AcceptChanges();
							RefreshDataBind();
                        }
                        catch (Exception ex)
                        {
							MessageBox.Show("Не удалось применить изменения в настройках прайс-листов. Сообщение было отправлено разработчику.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
							FREditorExceptionHandler.SendMessageOnException(null, new Exception("Ошибка при применении изменений в настройках прайс-листов.", ex));
							tr.Rollback();
                        }
                    }
                    finally
                    {
                        MyCn.Close();
                    }
                }
            }
            tsbApply.Enabled = false;
            tsbCancel.Enabled = false;
        }

		private void SendNotificationLetter(string body, string priceName, string providerName, string regionName)
		{
			try
			{
				//Получаем e-mail оператора
				string operatorMail = (string)MySqlHelper.ExecuteScalar(
					MyCn,
@"SELECT 
  regionaladmins.email 
FROM 
  accessright.regionaladmins
WHERE  
  username = ?UserName", new MySqlParameter("?UserName", Environment.UserName));

				//Формируем сообщение
				System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
					"register@analit.net",
#if DEBUG
					"morozov@analit.net",
#else
					"RegisterList@subscribe.analit.net",
#endif
 String.Format("\"{0}\" - изменения в списке ценовых колонок", providerName),
					String.Format(@"
Оператор   : {0} 
Поставщик  : {1}
Регион     : {2}
Прайс-лист : {3}

{4}

С уважением,
  АК Инфорум.",
						Environment.UserName,
						providerName,
						regionName,
						priceName,
						body));
				if (!String.IsNullOrEmpty(operatorMail))
					m.Bcc.Add(operatorMail);
				System.Net.Mail.SmtpClient sm = new System.Net.Mail.SmtpClient("mail.adc.analit.net");
				sm.Send(m);


			}
			catch (Exception ex)
			{
				MessageBox.Show("Не удалось отправить уведомление об изменениях. Сообщение было отправлено разработчику.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				FREditorExceptionHandler.SendMessageOnException(null, new Exception("Ошибка при отправке уведомления.", ex));
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
            indgvPrice.LoadSettings(CregKey);
        }

        private void SaveCostsSettings()
        {
            if (fileExist)
            {
				if ((fmt == PriceFormat.FixedDOS) || (fmt == PriceFormat.FixedWIN))
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
			if ((fmt == PriceFormat.FixedDOS) || (fmt == PriceFormat.FixedWIN))
            {
                CregKey = BaseRegKey + "\\CostsDataGridFixed";
            }
            else
            {
                CregKey = BaseRegKey + "\\CostsDataGrid";
            }
            indgvCosts.LoadSettings(CregKey);

			if ((fmt == PriceFormat.FixedDOS) || (fmt == PriceFormat.FixedWIN))
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
			dtMarking.DefaultView.Sort = "MBeginField";
			indgvMarking.DataSource = dtMarking.DefaultView;
            indgvMarking.LoadSettings(BaseRegKey + "\\MarkingDataGrid");
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
                    while ((i < dtMarking.Rows.Count) && (!findField))
                    {
                        dr = dtMarking.Rows[i];
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

        private void tbRules_TextChanged(object sender, EventArgs e)
        {
            tmrUpdateApply.Stop();
            if (((TextBox)sender).Text == String.Empty)
                ((DataRowView)((TextBox)sender).DataBindings[0].BindingManagerBase.Current)[((TextBox)sender).DataBindings[0].BindingMemberInfo.BindingField] = ((TextBox)sender).DataBindings[0].DataSourceNullValue;
            tmrUpdateApply.Start();
        }

        private void tbFirmName_TextChanged(object sender, EventArgs e)
        {
            if (!InSearch)
            {
                tmrSearch.Stop();
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

                int regcode = 0;
                if ((cbRegions.SelectedItem != null) && (Convert.ToInt32(cbRegions.SelectedValue) != -1))
                    regcode = Convert.ToInt32(cbRegions.SelectedValue);
                int seg = -1;
                if ((cbSegment.SelectedItem != null) && (Convert.ToInt32(cbSegment.SelectedValue) != -1))
                    seg = Convert.ToInt32(cbSegment.SelectedValue);
                string shname = tbFirmName.Text;
                FillTables(shname, regcode, seg);
                shortNameFilter = shname;
                regionCodeFilter = regcode;
                segmentFilter = seg;
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

        private void indgvFirm_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == cSegmentDataGridViewTextBoxColumn.Index)
            {
                if ((int)e.Value == 0)
                    e.Value = "Опт";
                else if ((int)e.Value == 1)
                    e.Value = "Розница";
            }
        }

        private void indgvFirm_DoubleClick(object sender, EventArgs e)
        {
            CurrencyManager currencyManager = (CurrencyManager)BindingContext[indgvFirm.DataSource, indgvFirm.DataMember];
            CheckOnePrice(currencyManager);
        }

        private bool CostIsValid()
        {            
            if ((indgvPrice.CurrentRow.DataBoundItem == null) || ((indgvPrice.CurrentRow.DataBoundItem != null) && (((DataRowView)indgvPrice.CurrentRow.DataBoundItem)[PCostType.ColumnName] is DBNull)))
            {
                MessageBox.Show("Необходимо указать тип цены!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
                return true;
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
                }
            }
        }

        private void indgvPrice_DoubleClick(object sender, EventArgs e)
        {
            if (CostIsValid())
            {
                fcs = dgFocus.Price;
                tbControl.SelectedTab = tpPrice;
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
				((ComboBox)e.Control).Enabled = !indgvPrice.CurrentCell.ReadOnly;
				if (((ComboBox)e.Control).Enabled)
					((ComboBox)e.Control).SelectionChangeCommitted += new EventHandler(frmFREMain_SelectionChangeCommitted);
			}
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

		private string GetPriceFileExtention(long PriceItemtId)
		{
			MyCn.Open();
			try
			{
				return (string)MySqlHelper.ExecuteScalar(
					MyCn,
					@"
SELECT 
  p.FileExtention 
FROM 
  usersettings.priceitems,
  farm.formrules f, 
  farm.pricefmts p
where
    priceitems.id = ?PriceItemtId
and f.Id = priceitems.FormRuleID
and p.Id = f.PriceFormatID",
					new MySqlParameter("?PriceItemtId", PriceItemtId));
			}
			finally			
			{
				MyCn.Close();
			}
		}

		private void btnRetrancePrice_Click(object sender, EventArgs e)
		{
			if (indgvPrice.CurrentRow != null)
			{
				DataRow selectedPrice = ((DataRowView)indgvPrice.CurrentRow.DataBoundItem).Row;

				string BaseFolder = @"\\FMS\Prices\Base\";
				string InboundFolder = @"\\FMS\Prices\Inbound0\";

				string PriceExtention = GetPriceFileExtention(Convert.ToInt64(selectedPrice[PPriceItemId]));
				string sourceFile = Path.GetFullPath(BaseFolder) + Path.DirectorySeparatorChar + selectedPrice[PPriceItemId].ToString() + PriceExtention;
				string destinationFile = Path.GetFullPath(InboundFolder) + Path.DirectorySeparatorChar + selectedPrice[PPriceItemId].ToString() + PriceExtention;

				if (File.Exists(sourceFile))
				{
					if (!File.Exists(destinationFile))
					{
						File.Move(sourceFile, destinationFile);
						MyCn.Open();
						try
						{
							MySqlHelper.ExecuteNonQuery(
								MyCn,
								"insert into logs.pricesretrans (LogTime, OperatorName, OperatorHost, PriceItemId) values (now(), ?UserName, ?UserHost, ?PriceItemId)",
								new MySqlParameter("?UserName", Environment.UserName),
								new MySqlParameter("?UserHost", Environment.MachineName),
								new MySqlParameter("?PriceItemId", Convert.ToInt64(selectedPrice[PPriceItemId])));
						}
						finally
						{
							MyCn.Close();
						}
						MessageBox.Show("Прайс-лист успешно переподложен.");
					}
					else
						MessageBox.Show(String.Format("Данный прайс-лист находится в очереди на формализацию в папке {0}!", InboundFolder), "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				else
					MessageBox.Show(String.Format("Данный прайс-лист отсутствует в папке {0}!", BaseFolder), "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				indgvPrice.Focus();
			}
			else
				indgvFirm.Focus();
		}

		private void indgvPrice_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			//Если рассматриваются колонки с ComboBox и строки с данными
			if ((e.RowIndex >= 0) && ( ((INDataGridView)sender).Columns[e.ColumnIndex].CellTemplate is DataGridViewComboBoxCell))
			{
				//Если это не родительский прайс-лист, то это ценовая колонка многофайлового прайс-листа и изменять ее нельзя
				if (!Convert.ToBoolean( ((DataRowView)indgvPrice.Rows[e.RowIndex].DataBoundItem)[PIsParent.ColumnName]))				
					e.CellStyle.ForeColor = SystemColors.InactiveCaptionText;
			}
		}

		private void cmbParentRules_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				FillParentComboBoxBySearch((ComboBox)sender, @"
select
  pim.FormRuleID,
  concat(cd.ShortName, ' (', if(pd.CostType = 1, concat(pd.PriceName, ' [Колонка] ', pc.CostName), pd.PriceName), ') - ', r.Region) PriceName
from
  usersettings.clientsdata cd,
  usersettings.pricesdata pd,
  usersettings.pricescosts pc,
  usersettings.priceitems pim,
  farm.regions r
where
  pd.FirmCode = cd.FirmCode
and pd.CostType is not null
and r.RegionCode = cd.RegionCode
and pc.PriceCode = pd.PriceCode
and ((pd.CostType = 1) or (pc.BaseCost = 1))
and pim.id = pc.PriceItemId
and ((pim.FormRuleID = ?PrevParentValue) or (pd.PriceName like ?SearchText) or (cd.ShortName like ?SearchText))
order by PriceName
", 
				"FormRuleId", "PriceName");
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
  concat(cd.ShortName, ' (', pd.PriceName, ') - ', r.Region) PriceName
from
  usersettings.clientsdata cd,
  usersettings.pricesdata pd,
  farm.regions r
where
  pd.FirmCode = cd.FirmCode
and pd.CostType is not null
and r.RegionCode = cd.RegionCode
and ((pd.PriceCode = ?PrevParentValue) or (pd.PriceName like ?SearchText) or (cd.ShortName like ?SearchText))
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
				if (((DataRowView)bsCostsFormRules.Current).IsNew)
					((DataRowView)bsCostsFormRules.Current).Delete();
				else
					((DataRowView)bsCostsFormRules.Current)[CFRDeleted.ColumnName] = true;
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
			// && (indgvCosts.Columns[e.ColumnIndex] == cFRCostNameDataGridViewTextBoxColumn)
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
							if (!drv.Row[CFRCostName.ColumnName, DataRowVersion.Original].Equals(drv.Row[CFRCostName.ColumnName, DataRowVersion.Current]))
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
        public System.Int64 costCode = -1;
        public bool baseCost = false;
        public string costName = String.Empty;
        public string fieldName = String.Empty;
        public int txtBegin = -1;
        public int txtEnd = -1;
        public decimal cost = 0m;

        public CoreCost(System.Int64 ACostCode, string ACostName, bool ABaseCost, string AFieldName, int ATxtBegin, int ATxtEnd)
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

	internal class FREditorExceptionHandler
	{

		public static void SendMessageOnException(object sender, Exception ex)
		{
			try
			{
				System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
					"service@analit.net",
					"service@analit.net",
					"Необработанная ошибка в FREditor",
					String.Format(@"
Источник     = {0}
Пользователь = {1}
Компьютер    = {2}
Ошибка       =
{3}", 
						sender,
						Environment.UserName,
						Environment.MachineName,
						ex));
				System.Net.Mail.SmtpClient sm = new System.Net.Mail.SmtpClient("mail.adc.analit.net");
				sm.Send(m);
			}
			catch
			{ }
		}

		// Handles the exception event.
		public static void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs t)
		{
			SendMessageOnException(sender, t.Exception);
			MessageBox.Show("В приложении возникла необработанная ошибка.\r\nИнформация об ошибке была отправлена разработчику.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

	}
}
