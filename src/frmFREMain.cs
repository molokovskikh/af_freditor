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
        CountryCr,
        Unit,
        Volume,
        Quantity,
        Note,
        Period,
        Doc,
        BaseCost,
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

    public partial class frmFREMain : System.Windows.Forms.Form
    {
        ArrayList gds = new ArrayList();
        ArrayList dtables = new ArrayList();
        ArrayList tblstyles = new ArrayList();
        DataTable dtPrice = new DataTable();

#if DEBUG
		//private MySqlConnection MyCn = new MySqlConnection("server=testSQL.analit.net; user id=system; password=123; database=farm;convert Zero Datetime=True;");
		private MySqlConnection MyCn = new MySqlConnection("server=SQL.analit.net; user id=system; password=123; database=farm;convert Zero Datetime=True;");
#else
		private MySqlConnection MyCn = new MySqlConnection("server=sql.analit.net; user id=system; password=123; database=farm;convert Zero Datetime=True;");
#endif
		private MySqlCommand MyCmd = new MySqlCommand();
        private MySqlDataAdapter MyDA = new MySqlDataAdapter();
        private string BaseRegKey = "Software\\Inforoom\\FREditor";
        private string CregKey;

        private OleDbConnection dbcMain = new OleDbConnection();

#if DEBUG
		//string StartPath = "\\"+"\\"+"FMS" + "\\" + "Prices" + "\\" + "Base" + "\\";
		string StartPath = "C:\\TEMP\\Base\\";
#else
		string StartPath = "\\"+"\\"+"FMS" + "\\" + "Prices" + "\\" + "Base" + "\\";
#endif

        string EndPath = Path.GetTempPath();
        string TxtFilePath = String.Empty;
        string frmCaption = String.Empty;

        DataRow openedPriceDR;
        string listName = String.Empty;
        string delimiter = String.Empty;
        string fmt = String.Empty;
		//Текущий клиент, с которым происходит работа и текущий прайс
		long currentPriceCode = 0;
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

            this.mcmdUCostRules.Parameters.Add("?CostCode", MySql.Data.MySqlClient.MySqlDbType.Int64, 0, "CFRCost_Code");
            this.mcmdUCostRules.Parameters.Add("?FieldName", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, "CFRFieldName");
			this.mcmdUCostRules.Parameters.Add("?TxtBegin", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, "CFRTextBegin");
			this.mcmdUCostRules.Parameters.Add("?TxtEnd", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, "CFRTextEnd");

            this.mcmdUFormRules.CommandText =
@"UPDATE formrules SET
  ParentSynonym = ?FRSynonyms,
  ParentFormRules = ?FRRules,
  Flag = ?SelfFlag,
  StartLine = ?FRStartLine,

  Currency = ?FRCurrency,
  `Delimiter` = ?FRDelimiter,
  JunkPos = ?FRSelfJunkPos,
  AwaitPos = ?FRSelfAwaitPos,
  PriceFMT = ?FRFormat,
  ListName = ?FRListName,
  NameMask = ?FRNameMask,
  ForbWords = ?FRForbWords,
  VitallyImportantMask = ?FRVitallyImportantMask,

  TxtCodeBegin = ?FRTxtCodeBegin,
  TxtCodeEnd = ?FRTxtCodeEnd,
  TxtCodeCrBegin = ?FRTxtCodeCrBegin,
  TxtCodeCrEnd = ?FRTxtCodeCrEnd,
  TxtNameBegin = ?FRTxtNameBegin,
  TxtNameEnd = ?FRTxtNameEnd,
  TxtFirmCrBegin = ?FRTxtFirmCrBegin,
  TxtFirmCrEnd = ?FRTxtFirmCrEnd,
  TxtCountryCrBegin = ?FRTxtCountryCrBegin,
  TxtCountryCrEnd = ?FRTxtCountryCrEnd,
  TxtBaseCostBegin = ?FRTxtBaseCostBegin,
  TxtBaseCostEnd = ?FRTxtBaseCostEnd,
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
  FBaseCost = ?FRFBaseCost,
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
  FirmCode = ?FRPriceCode;

update
  usersettings.price_update_info
set
  RowCount = if(RowCount <> ?FRPosNum, 0, RowCount)
where
  PriceCode = ?FRPriceCode";
            this.mcmdUFormRules.Parameters.Add("?FRPriceCode", MySql.Data.MySqlClient.MySqlDbType.Int64);
            this.mcmdUFormRules.Parameters.Add("?FRSynonyms", MySql.Data.MySqlClient.MySqlDbType.Int64);
            this.mcmdUFormRules.Parameters.Add("?FRRules", MySql.Data.MySqlClient.MySqlDbType.Int64);
            this.mcmdUFormRules.Parameters.Add("?SelfFlag", MySql.Data.MySqlClient.MySqlDbType.Int16);
            this.mcmdUFormRules.Parameters.Add("?FRStartLine", MySql.Data.MySqlClient.MySqlDbType.Int32);

            this.mcmdUFormRules.Parameters.Add("?FRTxtCodeBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtCodeEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtCodeCrBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtCodeCrEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtNameBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtNameEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtFirmCrBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtFirmCrEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtCountryCrBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtCountryCrEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtBaseCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtBaseCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtMinBoundCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtMinBoundCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtCurrencyBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtCurrencyEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtUnitBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtUnitEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtVolumeBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtVolumeEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtQuantityBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtQuantityEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtNoteBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtNoteEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtPeriodBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtPeriodEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtDocBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtDocEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtJunkBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtJunkEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtAwaitBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtAwaitEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtRequestRatioBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtRequestRatioEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtRegistryCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtRegistryCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtVitallyImportantBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtVitallyImportantEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtMaxBoundCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
            this.mcmdUFormRules.Parameters.Add("?FRTxtMaxBoundCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
			this.mcmdUFormRules.Parameters.Add("?FRTxtOrderCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
			this.mcmdUFormRules.Parameters.Add("?FRTxtOrderCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);
			this.mcmdUFormRules.Parameters.Add("?FRTxtMinOrderCountBegin", MySql.Data.MySqlClient.MySqlDbType.Int32);
			this.mcmdUFormRules.Parameters.Add("?FRTxtMinOrderCountEnd", MySql.Data.MySqlClient.MySqlDbType.Int32);			

            this.mcmdUFormRules.Parameters.Add("?FRCurrency", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRDelimiter", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRPosNum", MySql.Data.MySqlClient.MySqlDbType.Int64);
            this.mcmdUFormRules.Parameters.Add("?FRSelfJunkPos", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRSelfAwaitPos", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFormat", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRListName", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRNameMask", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRForbWords", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRVitallyImportantMask", MySql.Data.MySqlClient.MySqlDbType.VarString);

            this.mcmdUFormRules.Parameters.Add("?FRFCode", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFCodeCr", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFName1", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFName2", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFName3", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFFirmCr", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFBaseCost", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFMinBoundCost", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFCurrency", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFUnit", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFVolume", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFQuantity", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFNote", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFPeriod", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFDoc", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFJunk", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFAwait", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFRequestRatio", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFRegistryCost", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFVitallyImportant", MySql.Data.MySqlClient.MySqlDbType.VarString);
            this.mcmdUFormRules.Parameters.Add("?FRFMaxBoundCost", MySql.Data.MySqlClient.MySqlDbType.VarString);
			this.mcmdUFormRules.Parameters.Add("?FRFOrderCost", MySql.Data.MySqlClient.MySqlDbType.VarString);
			this.mcmdUFormRules.Parameters.Add("?FRFMinOrderCount", MySql.Data.MySqlClient.MySqlDbType.VarString);			
            this.mcmdUFormRules.Parameters.Add("?FRMemo", MySql.Data.MySqlClient.MySqlDbType.VarString);

            foreach (MySqlParameter ms in this.mcmdUFormRules.Parameters)
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
			FREditorExceptionHandler feh = new FREditorExceptionHandler();

			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(feh.OnThreadException);

            Application.Run(new frmFREMain());
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

            MyCmd.CommandText =
                @"SELECT 
					Format AS FMTFormat,
                    FileExtention  as FMTExt
				FROM 
					farm.PriceFmts order by Format";

            MyDA.Fill(dtPriceFMTs);
        }

        private void FillFormatCmb()
        {
            for (int i = 0; i < dtPriceFMTs.Rows.Count; i++)
            {
                DataRow dr = dtPriceFMTs.Rows[i];
                string item = dr["FMTFormat"].ToString();
                if (-1 == cmbFormat.Items.IndexOf(item))
                {
                    cmbFormat.Items.Add(item);
                }
            }
            cmbFormat.SelectedIndex = 0;
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
@"SELECT
  distinct
  pd.FirmCode AS PFirmCode,
  pd.PriceName AS PFirmName,
  pd.PriceCode as PPriceCode,
  pui.DatePrevPrice as PDatePrevPrice,
  pui.DateCurPrice as PDateCurPrice,
  pui.DateLastForm as PDateLastForm,
  fr.MaxOld as PMaxOld,
  pd.PriceType as PPriceType,
  pd.CostType as PCostType,
  pd.WaitingDownloadInterval as PWaitingDownloadInterval,
  1 as PIsParent
FROM
  usersettings.pricesdata pd
inner join usersettings.pricescosts pc on pc.showpricecode = pd.pricecode
inner join usersettings.price_update_info pui on  pui.pricecode = pd.pricecode
inner join usersettings.clientsdata cd on cd.FirmCode = pd.FirmCode
inner join farm.formrules fr on fr.FirmCode=pd.pricecode
inner join farm.regions r on r.regioncode=cd.regioncode
where
  cd.FirmType = 0
and pd.CostType = 0 ";
            MyCmd.CommandText += param;
			//Выбираем прайс-листы с многофайловыми ценами и сами многофайловые цены как прайс-листы
            MyCmd.CommandText += @" 
union all
SELECT 
  distinct
  pd.FirmCode AS PFirmCode,
  if((parentpd.PriceCode = pc.pricecode), pd.PriceName, concat('[Колонка] ', pc.CostName)) AS PFirmName,
  pc.CostCode as PPriceCode,
  pui.DatePrevPrice as PDatePrevPrice,
  pui.DateCurPrice as PDateCurPrice,
  pui.DateLastForm as PDateLastForm,
  fr.MaxOld as PMaxOld,
  parentpd.PriceType as PPriceType,
  parentpd.CostType as PCostType,
  pd.WaitingDownloadInterval as PWaitingDownloadInterval,
  (parentpd.PriceCode = pc.pricecode) as PIsParent
FROM
  usersettings.pricesdata pd
inner join usersettings.pricescosts pc on pc.pricecode = pd.pricecode
inner join usersettings.clientsdata cd on cd.FirmCode = pd.FirmCode
inner join usersettings.price_update_info pui on  pui.pricecode = pc.CostCode
inner join farm.formrules fr on fr.FirmCode=pc.CostCode
inner join farm.regions r on r.regioncode=cd.regioncode
inner join usersettings.pricesdata parentpd on parentpd.PriceCode = pc.showpricecode
where
  cd.FirmType = 0
and parentpd.CostType = 1 ";
            MyCmd.CommandText += param;
			//Выбираем прайс-листы, у которых еще не выставлен тип ценовых колонок
            MyCmd.CommandText += @" 
union all
SELECT
  distinct
  pd.FirmCode AS PFirmCode,
  pd.PriceName AS PFirmName,
  pd.PriceCode as PPriceCode,
  pui.DatePrevPrice as PDatePrevPrice,
  pui.DateCurPrice as PDateCurPrice,
  pui.DateLastForm as PDateLastForm,
  fr.MaxOld as PMaxOld,
  pd.PriceType as PPriceType,
  pd.CostType as PCostType,
  pd.WaitingDownloadInterval as PWaitingDownloadInterval,
  1 as PIsParent
FROM
  usersettings.pricesdata pd
inner join usersettings.pricescosts pc on pc.showpricecode = pd.pricecode
inner join usersettings.clientsdata cd on cd.FirmCode = pd.FirmCode
inner join usersettings.price_update_info pui on  pui.pricecode = pd.pricecode
inner join farm.formrules fr on fr.FirmCode=pd.pricecode
inner join farm.regions r on r.regioncode=cd.regioncode
where
  cd.FirmType = 0
and pd.CostType is null ";
            MyCmd.CommandText += param;
            MyCmd.CommandText += @" 
order by 2";

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
@"SELECT 
  distinct
	pc.ShowPriceCode AS PCPriceCode,
	pc.CostCode AS PCCostCode,
	pc.BaseCost as PCBaseCost,
	pc.CostName as PCCostName
FROM
	usersettings.pricescosts pc
inner join usersettings.pricesdata pd on pd.pricecode = pc.showpricecode
inner join usersettings.clientsdata cd on cd.firmcode = pd.firmcode
inner join farm.regions r on r.regioncode=cd.regioncode
where 
    cd.firmtype = 0
and pd.CostType = 0 ";
            MyCmd.CommandText += param;
            MyCmd.CommandText += @"  
union all
SELECT 
  distinct
	pc.PriceCode AS PCPriceCode,
	pc.CostCode AS PCCostCode,
	pc.BaseCost as PCBaseCost,
	pc.CostName as PCCostName
FROM
  usersettings.pricescosts pc
inner join usersettings.pricesdata pd on pd.pricecode = pc.showpricecode
inner join usersettings.clientsdata cd on cd.firmcode = pd.firmcode
inner join farm.regions r on r.regioncode=cd.regioncode
where 
    cd.firmtype = 0
and pd.CostType = 1 ";
            MyCmd.CommandText += param;
            MyCmd.CommandText += @"  
order by 4";

			MyDA.Fill(dtPricesCost);
        }

        private void dtCostsFormRulesFill(string param)
        {
            MyCmd.CommandText =
@"SELECT 
  distinct
  pc.showpricecode AS CFRfr_if,
  pc.CostName as CFRCostName,
  cfr.PC_CostCode AS CFRCost_Code,
  cfr.FieldName AS CFRFieldName,
  cfr.TxtBegin as CFRTextBegin,
  cfr.TxtEnd as CFRTextEnd
FROM 
  farm.costformrules cfr
inner join usersettings.pricescosts pc on pc.CostCode = cfr.pc_costcode
inner join usersettings.pricesdata pd on pd.pricecode = pc.showpricecode
inner join usersettings.clientsdata cd on cd.firmcode = pd.firmcode
inner join farm.regions r on r.regioncode=cd.regioncode
where 
     cd.firmtype = 0 
and pd.CostType = 0 ";
            MyCmd.CommandText += param;
            MyCmd.CommandText += @"   
union all
SELECT 
  distinct
  pc.pricecode AS CFRfr_if,
  pc.CostName as CFRCostName,
  cfr.PC_CostCode AS CFRCost_Code,
  cfr.FieldName AS CFRFieldName,
  cfr.TxtBegin as CFRTextBegin,
  cfr.TxtEnd as CFRTextEnd
FROM 
  farm.costformrules cfr
inner join usersettings.pricescosts pc on pc.CostCode = cfr.pc_costcode
inner join usersettings.pricesdata pd on pd.pricecode = pc.showpricecode
inner join usersettings.clientsdata cd on cd.firmcode = pd.firmcode
inner join farm.regions r on r.regioncode=cd.regioncode
where 
    cd.firmtype = 0 
and pd.CostType = 1 ";
            MyCmd.CommandText += param;
            MyCmd.CommandText += @"   
order by 2";

            MyDA.Fill(dtCostsFormRules);
        }

        private void dtFormRulesFill(string param)
        {
            MyCmd.CommandText =
				@"SELECT
    IF(FR.ParentFormRules,FR.ParentFormRules,FR.FirmCode) AS FormID,
    FR.ParentSynonym AS FRSynonyms,
    FR.FirmCode AS FRPriceCode,
	FR.Currency AS FRCurrency,
	PFR.Delimiter As FRDelimiter,
	FR.ParentFormRules AS FRRules,
	FR.Memo As FRMemo,
    CD.ShortName AS ClientShortName,
    CD.FirmCode AS ClientCode,
    PD.PriceName AS FRName,
    FR.Flag AS SelfFlag,
    FR.JunkPos AS FRSelfJunkPos,
    FR.AwaitPos AS FRSelfAwaitPos,
    pui.RowCount AS FRPosNum,
	PFR.StartLine AS FRStartLine,
    PFR.PriceFMT as FRFormat,
    pfmt.FileExtention as FRExt,
	PFR.ListName as FRListName,
	PFR.NameMask as FRNameMask,
	PFR.ForbWords as FRForbWords,
    FR.VitallyImportantMask as FRVitallyImportantMask,

	PFR.TxtCodeBegin as FRTxtCodeBegin,
	PFR.TxtCodeEnd as FRTxtCodeEnd,

	PFR.TxtCodeCrBegin as FRTxtCodeCrBegin,
	PFR.TxtCodeCrEnd as FRTxtCodeCrEnd,

	PFR.TxtNameBegin as FRTxtNameBegin,
	PFR.TxtNameEnd as FRTxtNameEnd,

	PFR.TxtFirmCrBegin as FRTxtFirmCrBegin,
	PFR.TxtFirmCrEnd as FRTxtFirmCrEnd,

	PFR.TxtCountryCrBegin as FRTxtCountryCrBegin,
	PFR.TxtCountryCrEnd as FRTxtCountryCrEnd,

	PFR.TxtBaseCostBegin as FRTxtBaseCostBegin,
	PFR.TxtBaseCostEnd as FRTxtBaseCostEnd,

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
	PFR.FBaseCost as FRFBaseCost,
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
	PFR.FMinOrderCount as FRFMinOrderCount,

    CD.FirmStatus,
    CD.BillingStatus,
    CD.FirmSegment
FROM UserSettings.PricesData AS PD
INNER JOIN
    UserSettings.ClientsData AS CD on cd.FirmCode = pd.FirmCode and cd.FirmType = 0
INNER JOIN
    farm.regions r on r.regioncode=cd.regioncode
INNER JOIN
    usersettings.price_update_info pui on pui.PriceCode = PD.PriceCode
INNER JOIN
    Farm.formrules AS FR
    ON FR.FirmCode=PD.PriceCode
LEFT JOIN
    Farm.FormRules AS PFR
    ON PFR.FirmCode=IF(FR.ParentFormRules,FR.ParentFormRules,FR.FirmCode)
left join Farm.PriceFMTs as pfmt on PFR.PriceFMT = pfmt.Format
where
pd.PriceCode in
(
SELECT 
  distinct
  pd.PriceCode as PPriceCode
FROM
  usersettings.pricesdata pd
inner join usersettings.pricescosts pc on pc.showpricecode = pd.pricecode
inner join usersettings.clientsdata cd on cd.FirmCode = pd.FirmCode
where
  cd.FirmType = 0
and pd.CostType = 0
union all
SELECT 
  distinct
  pc.CostCode as PPriceCode
FROM
  usersettings.pricesdata pd
inner join usersettings.pricescosts pc on pc.showpricecode = pd.pricecode
inner join usersettings.clientsdata cd on cd.FirmCode = pd.FirmCode
where
  cd.FirmType = 0
and pd.CostType = 1
)";
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
                Debug.WriteLine("DoOpenPrice begin");

                fW.openPrice += new OpenPriceDelegate(OpenPrice);
                fW.drP = drP;

                fW.ShowDialog();

            }
            catch (Exception e)
            {
                Debug.WriteLine("DoOpenPrice error : " + e.ToString());
            }
            finally
            {
                Debug.WriteLine("DoOpenPrice end");
            }

            fW = null;
        }

        public delegate void OpenPriceDelegate(DataRow drP);

        private void OpenPrice(DataRow drP)
        {
            ClearPrice();

            Application.DoEvents();
            DataRow[] drFR;
            drFR = dtFormRules.Select("FRPriceCode = " + drP["PPriceCode"].ToString());
            existParentRules = drFR[0]["FRRules"].ToString();

            DataRow drC = drP.GetParentRow(dtClients.TableName + "-" + dtPrices.TableName);
            frmCaption = String.Format("{0}; {1}", drC["CShortName"], drC["CRegion"]);

            string f = drFR[0]["FRPriceCode"].ToString();

            fmt = drFR[0]["FRFormat"].ToString();

			FillParentComboBox(cmbParentRules, drFR[0]["FRRules"]);
			FillParentComboBox(cmbParentSynonyms, drFR[0]["FRSynonyms"]);

            string r = ".err";
            DataRow[] exts = dtPriceFMTs.Select("FMTFormat = '" + fmt + "'");
            if (exts.Length == 1)
                r = exts[0]["FMTExt"].ToString();

            delimiter = drFR[0]["FRDelimiter"].ToString();

            string takeFile = f + r;
            if (!(File.Exists(StartPath + takeFile)))
            {
                fileExist = false;
                MessageBox.Show(String.Format("Файл {0} выбранного Прайс-листа отсутствует в директории по умолчанию ({1})", takeFile, StartPath), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                fileExist = true;
                Directory.CreateDirectory(EndPath + f);

                File.Copy(StartPath + takeFile, EndPath + f + "\\" + takeFile, true);
                string filePath = EndPath + f + "\\" + takeFile;

                Application.DoEvents();

                if ((fmt == "DB") || (fmt == "DBF"))
                {
                    OpenDBFFile(filePath);
                }
                else
                    if (fmt == "XLS")
                    {
                        listName = drFR[0]["FRListName"].ToString();
                        OpenEXLFile(filePath);
                    }
                    else
                        if ((fmt == "DOS") || (fmt == "WIN"))
                        {
                            dbcMain.Close();
                            dbcMain.Dispose();
                            if (delimiter == String.Empty)
                            {
                                startLine = drFR[0]["FRStartLine"] is DBNull ? -1 : Convert.ToInt64(drFR[0]["FRStartLine"]);

                                TxtFilePath = EndPath + f + "\\" + takeFile;
                                dtMarking.Clear();
                                OpenTXTFFile(filePath, drFR[0]);
                            }
                            else
                            {
                                OpenTXTDFile(filePath, fmt);
                            }
                        }
                Application.DoEvents();
                ShowTab(fmt);
                Application.DoEvents();
                this.Text = String.Format("Редактор Правил Формализации ({0})", frmCaption);
                Application.DoEvents();
            }
        }

		private void FillParentComboBox(ComboBox cmbParent, object ParentValue)
		{
			DataSet ParentDS = MySqlHelper.ExecuteDataset(MyCn, @"
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
and pd.PriceCode = ?PriceCode
order by 2", new MySqlParameter("?PriceCode", ParentValue));
			FillParentComboBoxFromTable(cmbParent, ParentDS.Tables[0]);
		}

		private void FillParentComboBoxBySearch(ComboBox cmbParent)
		{
			//Возможно есть способ более проще получить значение биндинга
			object SelectedValue = ((DataRowView)bsFormRules.Current).Row[cmbParent.DataBindings["SelectedValue"].BindingMemberInfo.BindingField];
			DataSet ParentDS = MySqlHelper.ExecuteDataset(MyCn, @"
select
  distinct
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
and ((pd.PriceCode = ?PrevPriceCode) or (pd.PriceName like ?SearchText) or (cd.ShortName like ?SearchText))
order by 2", new MySqlParameter("?PrevPriceCode", SelectedValue), new MySqlParameter("?SearchText", "%" + cmbParent.Text + "%"));
			FillParentComboBoxFromTable(cmbParent, ParentDS.Tables[0]);
		}

		private void FillParentComboBoxFromTable(ComboBox cmbParent, DataTable dtParent)
		{
			bsFormRules.SuspendBinding();
			try
			{
				cmbParent.DataSource = null;
				cmbParent.Items.Clear();
				DataRow drNull = dtParent.NewRow();
				drNull["PriceCode"] = DBNull.Value;
				drNull["PriceName"] = "<не установлены>";
				dtParent.Rows.InsertAt(drNull, 0);
				cmbParent.Items.Add(drNull);
				cmbParent.DataSource = dtParent;
				cmbParent.DisplayMember = "PriceName";
				cmbParent.ValueMember = "PriceCode";
			}
			finally
			{
				bsFormRules.ResumeBinding();
			}
		}

        private void ShowTab(string fmt)
        {
            tcInnerTable.Visible = true;
            
            indgvPriceData.DataSource = dtPrice;

            if ((fmt == "WIN") || (fmt == "DOS"))
            {
                if (delimiter == String.Empty)
                {
                    tcInnerTable.SizeMode = TabSizeMode.Normal;
                    tcInnerTable.ItemSize = new Size(58, 18);
                    tcInnerTable.Appearance = TabAppearance.Normal;

                    label23.Visible = false;
                    txtBoxSheetName.Visible = false;
                    pnlGeneralFields.Visible = false;
                    pnlTxtFields.Visible = true;

                    indgvCosts.Columns[cFRTextBeginDataGridViewTextBoxColumn.Name].Visible = true;
                    indgvCosts.Columns[cFRTextEndDataGridViewTextBoxColumn.Name].Visible = true;
                }
                else
                {
                    tcInnerTable.SizeMode = TabSizeMode.Fixed;
                    tcInnerTable.ItemSize = new Size(0, 1);
                    tcInnerTable.Appearance = TabAppearance.Buttons;

                    label23.Visible = false;
                    txtBoxSheetName.Visible = false;
                    pnlGeneralFields.Visible = true;
                    pnlTxtFields.Visible = false;

                    indgvCosts.Columns[cFRFieldNameDataGridViewTextBoxColumn.Name].Visible = true;
                }
                tcInnerSheets.SizeMode = TabSizeMode.Fixed;
                tcInnerSheets.ItemSize = new Size(0, 1);
                tcInnerSheets.Appearance = TabAppearance.FlatButtons;
            }
            else
            {
                if (fmt == "XLS")
                {
                    tcInnerTable.SizeMode = TabSizeMode.Fixed;
                    tcInnerTable.ItemSize = new Size(0, 1);
                    tcInnerTable.Appearance = TabAppearance.Buttons;

                    tcInnerSheets.SizeMode = TabSizeMode.Normal;
                    tcInnerSheets.ItemSize = new Size(58, 18);
                    tcInnerSheets.Appearance = TabAppearance.Normal;

                    label23.Visible = true;
                    txtBoxSheetName.Visible = true;
                    pnlGeneralFields.Visible = true;
                    pnlTxtFields.Visible = false;
                }
                else
                    if ((fmt == "DB") || (fmt == "DBF"))
                    {
                        tcInnerTable.SizeMode = TabSizeMode.Fixed;
                        tcInnerTable.ItemSize = new Size(0, 1);
                        tcInnerTable.Appearance = TabAppearance.Buttons;

                        tcInnerSheets.SizeMode = TabSizeMode.Fixed;
                        tcInnerSheets.ItemSize = new Size(0, 1);
                        tcInnerSheets.Appearance = TabAppearance.Buttons;

                        label23.Visible = false;
                        txtBoxSheetName.Visible = false;
                        pnlGeneralFields.Visible = true;
                        pnlTxtFields.Visible = false;
                    }

                indgvCosts.Columns[cFRFieldNameDataGridViewTextBoxColumn.Name].Visible = true;
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

        private void OpenTXTDFile(string filePath, string fmt)
        {
            using (StreamWriter w = new StreamWriter(Path.GetDirectoryName(filePath) + Path.DirectorySeparatorChar + "Schema.ini", false, Encoding.GetEncoding(1251)))
            {
                w.WriteLine("[" + Path.GetFileName(filePath) + "]");
                w.WriteLine(("WIN" == fmt.ToUpper()) ? "CharacterSet=ANSI" : "CharacterSet=OEM");
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
                w.WriteLine(("WIN" == fmt.ToUpper()) ? "CharacterSet=ANSI" : "CharacterSet=OEM");
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
                if (PriceFields.OriginalName != pf && PriceFields.BaseCost != pf && String.Empty != TmpName)
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

        private void OpenTable(string fmt)
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
                    w.WriteLine(("WIN" == fmt.ToUpper()) ? "CharacterSet=ANSI" : "CharacterSet=OEM");
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
                            Debug.WriteLine("DoOpenTable begin");

                            fW.openPrice += new OpenPriceDelegate(DoOpenTable);
                            fW.drP = dtTemp.Rows[0];// new DataRow();

                            fW.ShowDialog();

                        }
                        catch
                        {
                            Debug.WriteLine("DoOpenTable error");
                        }
                        finally
                        {
                            Debug.WriteLine("DoOpenTable end");
                        }

                        fW = null;
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

            label23.Visible = false;
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
				currentPriceCode = 0;
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
					currentPriceCode = (long)(((DataRowView)indgvPrice.CurrentRow.DataBoundItem)[PPriceCode.ColumnName]);

					bsCostsFormRules.Filter = "CFRfr_if = " + drP[PPriceCode.ColumnName].ToString();
					bsFormRules.Filter = "FRPriceCode = " + drP[PPriceCode.ColumnName].ToString();
					bsCostsFormRules.ResumeBinding();
					bsFormRules.ResumeBinding();

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
					CurrencyManagerPosition((CurrencyManager)BindingContext[indgvPrice.DataSource, indgvPrice.DataMember], PPriceCode.ColumnName, currentPriceCode);
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
				long FirmCode = Convert.ToInt64(MySqlHelper.ExecuteScalar(MyCn, "select FirmCode from usersettings.pricesdata where PriceCode = ?PriceCode", new MySqlParameter("?PriceCode", ((DataRowView)bsFormRules.Current)[FRPriceCode.ColumnName])));
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
            try
            {
                Debug.WriteLine("TestD begin");
                da.Fill(dt);
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format("TestD error {0}", e));
            }
            finally
            {
                Debug.WriteLine("TestD end");
            }
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

            try
            {
                Debug.WriteLine("CreateThread begin");

                DataTable temp = new DataTable();

                System.IAsyncResult ar = ta.BeginInvoke(da, dt, null, state);
                while (!ar.IsCompleted)
                    Application.DoEvents();

            }
            catch (Exception e)
            {
                Debug.WriteLine("CreateThread error : " + e.ToString());
            }
            finally
            {
                Debug.WriteLine("CreateThread end");
            }
        }

        private void btnFloatPanel_Click(object sender, System.EventArgs e)
        {
            if (pnlFloat.Visible)
            {
                pnlFloat.Visible = false;
                if (!(fmt.Equals(cmbFormat.Text)) || !(delimiter.Equals(tbDevider.Text)))
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
                    erP.Dispose();
                    r = new Regex(txtMask.Text);
                    if ((fmt.ToUpper() == "WIN") && (delimiter == String.Empty))
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
                    if (!(m.Success))
                    {
                        indgv.RowHeadersVisible = true;
                        dr.RowError = "Несоответствие маски";
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
                        for (int i = 0; i < GroupsToFind.Length; i++)
                        {
                            if (GroupsToFind[i] != null)
                            {
                                if (m.Groups[GroupsToFind[i]].Success == false)
                                {
                                    indgv.RowHeadersVisible = true;
                                    dr.RowError = "Несоответствие маски";
                                }
                            }
                        }
                    }
                    else
                    {
                        indgv.RowHeadersVisible = true;
                        dr.RowError = "Несоответствие поля";
                    }
                }
            }
            dt.EndLoadData();
        }

        private void CommitAllEdit()
        {
            CurrencyManager cm;
            if (tbControl.SelectedTab == tpFirms)
            {
                cm = (CurrencyManager)BindingContext[indgvPrice.DataSource, indgvPrice.DataMember];
                cm.EndCurrentEdit();
            }
            else
            {
                cm = (CurrencyManager)BindingContext[indgvCosts.DataSource, indgvCosts.DataMember];
                cm.EndCurrentEdit();
                //TODO: Здесь надо правильно биндить
                //cm = (CurrencyManager)BindingContext[indgvCosts.DataSource, "Поставщики.Поставщики-Прайсы.Прайсы-правила"];
                //cm.EndCurrentEdit();
                cm = (CurrencyManager)BindingContext[bsCostsFormRules.DataSource, bsCostsFormRules.DataMember];
                cm.EndCurrentEdit();
                bsFormRules.EndEdit();
                //bsFormRules.CurrencyManager.EndCurrentEdit();
                //cm = (CurrencyManager)BindingContext[bsFormRules.DataSource, bsFormRules.DataMember];
                //cm.EndCurrentEdit();
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

                            mcmdUCostRules.Connection = MyCn;
                            daCostRules.TableMappings.Clear();
                            daCostRules.TableMappings.Add("Table", dtCostsFormRules.TableName);
                            daCostRules.Update(chg.Tables[dtCostsFormRules.TableName]);

                            mcmdUFormRules.Connection = MyCn;
                            daFormRules.TableMappings.Clear();
                            daFormRules.TableMappings.Add("Table", dtFormRules.TableName);
                            daFormRules.Update(chg.Tables[dtFormRules.TableName]);

                            tr.Commit();
                            dtSet.AcceptChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
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

                            MySqlCommand mcmdUPrice = new MySqlCommand();
                            MySqlDataAdapter daPrice = new MySqlDataAdapter();
                            mcmdUPrice.CommandText = @"
insert into usersettings.price_update_info (PriceCode)
select
  pc.PriceCode
from
  usersettings.PricesData pd,
  usersettings.PricesCosts pc
where
    (?PIsParent = 1)
and pd.PriceCode = ?PPriceCode
and (?PCostType = 1)
and pc.ShowPriceCode = pd.PriceCode
and pc.PriceCode <> pd.PriceCode
and not exists(select * from usersettings.price_update_info pui where pui.PriceCode = pc.PriceCode);

delete from
  usersettings.price_update_info
where
    PriceCode in 
(
select
  pc.PriceCode
from
  usersettings.PricesData pd,
  usersettings.PricesCosts pc
where
    (?PIsParent = 1)
and pd.PriceCode = ?PPriceCode
and (?PCostType = 0)
and pc.ShowPriceCode = pd.PriceCode
and pc.PriceCode <> pd.PriceCode
);

UPDATE 
  usersettings.PricesData pd 
SET
  PriceType = if(?PIsParent = 1, ?PPriceType, PriceType),
  CostType = if(?PIsParent = 1, ?PCostType, CostType), 
  WaitingDownloadInterval = ?PWaitingDownloadInterval
WHERE 
    pd.PriceCode = ?PPriceCode;
UPDATE 
  farm.FormRules fr 
SET
    MaxOld = ?PMaxOld
WHERE 
    fr.FirmCode = ?PPriceCode;";

                            mcmdUPrice.Parameters.Clear();
                            mcmdUPrice.Parameters.Add("?PPriceType", MySql.Data.MySqlClient.MySqlDbType.Int32);
                            mcmdUPrice.Parameters.Add("?PCostType", MySql.Data.MySqlClient.MySqlDbType.Int32);
                            mcmdUPrice.Parameters.Add("?PWaitingDownloadInterval", MySql.Data.MySqlClient.MySqlDbType.Int32);
                            mcmdUPrice.Parameters.Add("?PMaxOld", MySql.Data.MySqlClient.MySqlDbType.Int32);
                            mcmdUPrice.Parameters.Add("?PPriceCode", MySql.Data.MySqlClient.MySqlDbType.Int64);
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
                            MessageBox.Show(ex.ToString());
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

        private void frmFREMain_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Escape) && (tbControl.SelectedTab == tpPrice))
                tbControl.SelectedTab = tpFirms;
        }

		private void tmrUpdateApply_Tick(object sender, EventArgs e)
		{
            tmrUpdateApply.Stop();
            int ss = rtbArticle.SelectionStart;

            CommitAllEdit();

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
                    if (pnlGeneralFields.Visible)
                    {
                        indgvCosts[cFRFieldNameDataGridViewTextBoxColumn.Name, costsHitTestInfo.RowIndex].Value = ((DropField)e.Data.GetData(typeof(DropField))).FieldName;
                    }
                    else
                    {
                        indgvCosts[cFRTextBeginDataGridViewTextBoxColumn.Name, costsHitTestInfo.RowIndex].Value = ((DropField)e.Data.GetData(typeof(DropField))).FieldBegin;
                        indgvCosts[cFRTextEndDataGridViewTextBoxColumn.Name, costsHitTestInfo.RowIndex].Value = ((DropField)e.Data.GetData(typeof(DropField))).FieldEnd;
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
                if (((fmt == "WIN") || (fmt == "DOS")) && (delimiter == String.Empty))
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
            if (((fmt == "WIN") || (fmt == "DOS")) && (delimiter == String.Empty))
            {
                CregKey = BaseRegKey + "\\CostsDataGridFixed";
            }
            else
            {
                CregKey = BaseRegKey + "\\CostsDataGrid";
            }
            indgvCosts.LoadSettings(CregKey);
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

			//TODO: Возможно этот код не нужен, а был вставлен для отладки
            DataTable dt = new DataTable();
            if(dtSet.HasChanges())
                dt = dtFormRules.GetChanges();
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

        private void indgvCosts_DoubleClick(object sender, EventArgs e)
        {
            Point p = indgvCosts.PointToClient(Control.MousePosition);
            DataGridView.HitTestInfo costsHitTestInfo = indgvCosts.HitTest(p.X, p.Y);
            if (costsHitTestInfo.Type == DataGridViewHitTestType.Cell)
            {
                if (costsHitTestInfo.RowIndex > -1)
                {
                    CurrencyManager cm = (CurrencyManager)BindingContext[indgvCosts.DataSource, indgvCosts.DataMember];
                    DataRowView drv = (DataRowView)cm.Current;
                    if (pnlGeneralFields.Visible)
                    {
                        indgvCosts[cFRFieldNameDataGridViewTextBoxColumn.Name, costsHitTestInfo.RowIndex].Value = String.Empty;
                    }
                    else
                    {
                        indgvCosts[cFRTextBeginDataGridViewTextBoxColumn.Name, costsHitTestInfo.RowIndex].Value = String.Empty;
                        indgvCosts[cFRTextEndDataGridViewTextBoxColumn.Name, costsHitTestInfo.RowIndex].Value = String.Empty;
                    }
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

		private void cmbParentComboBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				FillParentComboBoxBySearch((ComboBox)sender);
			}
		}

		private string GetPriceFileExtention(long PriceCode)
		{
			MyCn.Open();
			try
			{
				return (string)MySqlHelper.ExecuteScalar(
					MyCn,
					@"
SELECT p.FileExtention 
FROM 
  farm.formrules f, farm.pricefmts p
where
f.FirmCode = ?PriceCode
and f.PriceFMT = p.Format",
					new MySqlParameter("?PriceCode", PriceCode));
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

				string PriceExtention = GetPriceFileExtention(Convert.ToInt64(selectedPrice[PPriceCode]));
				string sourceFile = Path.GetFullPath(BaseFolder) + Path.DirectorySeparatorChar + selectedPrice[PPriceCode].ToString() + PriceExtention;
				string destinationFile = Path.GetFullPath(InboundFolder) + Path.DirectorySeparatorChar + selectedPrice[PPriceCode].ToString() + PriceExtention;

				if (File.Exists(sourceFile))
				{
					if (!File.Exists(destinationFile))
					{
						File.Copy(sourceFile, destinationFile);
						MyCn.Open();
						try
						{
							MySqlHelper.ExecuteNonQuery(
								MyCn,
								"insert into logs.pricesretrans (LogTime, OperatorName, OperatorHost, PriceCode) values (now(), ?UserName, ?UserHost, ?PriceCode)",
								new MySqlParameter("?UserName", Environment.UserName),
								new MySqlParameter("?UserHost", Environment.MachineName),
								new MySqlParameter("?PriceCode", Convert.ToInt64(selectedPrice[PPriceCode])));
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
			if ((e.ColumnIndex >= 6) && (e.RowIndex >= 0))
			{
				//Если это не родительский прайс-лист, то это ценовая колонка многофайлового прайс-листа и изменять ее нельзя
				if (!Convert.ToBoolean( ((DataRowView)indgvPrice.Rows[e.RowIndex].DataBoundItem)[PIsParent.ColumnName]))				
					e.CellStyle.ForeColor = SystemColors.InactiveCaptionText;
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

		// Handles the exception event.
		public void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs t)
		{
			try
			{
				System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
					"service@analit.net",
					"service@analit.net",
					"Необработанная ошибка в FREditor",
					String.Format("Sender = {0}\r\nException = = {1}", sender, t.Exception));
				System.Net.Mail.SmtpClient sm = new System.Net.Mail.SmtpClient("box.analit.net");
				sm.Send(m);
			}
			catch
			{ }
			MessageBox.Show("В приложении возникла необработанная ошибка.\r\nИнформация об ошибке была отправлена разработчику.");
		}

	}
}
