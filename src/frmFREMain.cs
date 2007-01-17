using System;
using System.Drawing;
using System.Collections;
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
    // ������� INDataGridView
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
        MaxBoundCost
    }

    public partial class frmFREMain : System.Windows.Forms.Form
    {
        ArrayList gds = new ArrayList();
        ArrayList dtables = new ArrayList();
        ArrayList tblstyles = new ArrayList();
        DataTable dtPrice = new DataTable();
        //INDataGrid.INDataGridTableStyle PriceDataTableStyle = new INDataGrid.INDataGridTableStyle();

#if DEBUG
		private MySqlConnection MyCn = new MySqlConnection("server=testSQL.analit.net; user id=system; password=123; database=farm;convert Zero Datetime=True;");
		//private MySqlConnection MyCn = new MySqlConnection("server=SQL.analit.net; user id=system; password=123; database=farm;convert Zero Datetime=True;");
#else
		private MySqlConnection MyCn = new MySqlConnection("server=sql.analit.net; user id=system; password=123; database=farm;convert Zero Datetime=True;");
#endif
		private MySqlCommand MyCmd = new MySqlCommand();
        private MySqlDataAdapter MyDA = new MySqlDataAdapter();
        private string BaseRegKey = "Software\\Inforoom\\FREditor";
        private string CregKey;

        private OleDbConnection dbcMain = new OleDbConnection();

        string StartPath = "\\"+"\\"+"FMS" + "\\" + "Prices" + "\\" + "Base" + "\\";
        //string StartPath = "C:\\TEMP\\Base\\";
        string EndPath = Path.GetTempPath();
        //string EndPath = "C:" + "\\" + "PricesCopy" + "\\";
        string TxtFilePath = String.Empty;
        string frmCaption = String.Empty;
        //bool Opened = false;

        DataRow openedPriceDR;
        string listName = String.Empty;
        string delimiter = String.Empty;
        string fmt = String.Empty;
        string delimiter0 = String.Empty;
        string fmt0 = String.Empty;
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
            dtCostTypes.Rows.Add(new object[] { DBNull.Value, "<�� ������>" });
            dtCostTypes.Rows.Add(new object[] { 0, "����������������" });
            dtCostTypes.Rows.Add(new object[] { 1, "�������������" });

            pCostTypeDataGridViewComboBoxColumn.DataSource = dtCostTypes;
            pCostTypeDataGridViewComboBoxColumn.DisplayMember = "Name";
            pCostTypeDataGridViewComboBoxColumn.ValueMember = "ID";

            dtPriceTypes = new DataTable("PriceTypes");
            dtPriceTypes.Columns.Add("ID", typeof(int));
            dtPriceTypes.Columns.Add("Name", typeof(string));
            dtPriceTypes.Rows.Add(new object[] { DBNull.Value, "<�� ������>" });
            dtPriceTypes.Rows.Add(new object[] { 0, "�������" });
            dtPriceTypes.Rows.Add(new object[] { 1, "��������������" });
            dtPriceTypes.Rows.Add(new object[] { 2, "vip" });

            pPriceTypeDataGridViewComboBoxColumn.DataSource = dtPriceTypes;
            pPriceTypeDataGridViewComboBoxColumn.DisplayMember = "Name";
            pPriceTypeDataGridViewComboBoxColumn.ValueMember = "ID";

            dtFirmSegment = new DataTable("FirmSegment");
            dtFirmSegment.Columns.Add("ID", typeof(int));
            dtFirmSegment.Columns.Add("Segment", typeof(string));
            dtFirmSegment.Rows.Add(new object[] { -1, "���" });
            dtFirmSegment.Rows.Add(new object[] { 0, "���" });
            dtFirmSegment.Rows.Add(new object[] { 1, "�������" });

            cbSegment.DataSource = dtFirmSegment;
            cbSegment.DisplayMember = "Segment";
            cbSegment.ValueMember = "ID";

            indgvMarking.DataSource = dtSet;
            indgvMarking.DataMember = dtMarking.TableName;

			//this.mcmdUCostRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FirmCode", MySql.Data.MySqlClient.MySqlDbType.Int64, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "CFRFR_if", System.Data.DataRowVersion.Current, null));
            this.mcmdUCostRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("CostCode", MySql.Data.MySqlClient.MySqlDbType.Int64, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "CFRCost_Code", System.Data.DataRowVersion.Current, null));
            this.mcmdUCostRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FieldName", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "CFRFieldName", System.Data.DataRowVersion.Current, null));
			this.mcmdUCostRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("TxtBegin", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "CFRTextBegin", System.Data.DataRowVersion.Current, null));
			this.mcmdUCostRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("TxtEnd", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "CFRTextEnd", System.Data.DataRowVersion.Current, null));

            this.mcmdUFormRules.CommandText =
@"UPDATE formrules SET
  ParentSynonym = ?FRSynonyms,
  ParentFormRules = ?FRRules,
  Flag = ?SelfFlag,
  StartLine = ?FRStartLine,

  Currency = ?FRCurrency,
  `Delimiter` = ?FRDelimiter,
  PosNum = ?FRPosNum,
  JunkPos = ?FRSelfJunkPos,
  AwaitPos = ?FRSelfAwaitPos,
  PriceFMT = ?FRFormat,
  ListName = ?FRListName,
  NameMask = ?FRNameMask,
  ForbWords = ?FRForbWords,
  VitallyImportantMask = ?FRVitallyImportantMask,
  FMaxBoundCost = ?FRFMaxBoundCost,

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
  TxtAsFactCostBegin = ?FRTxtAsFactCostBegin,
  TxtAsFactCostEnd = ?FRTxtAsFactCostEnd,
  Txt5DayCostBegin = ?FRTxt5DayCostBegin,
  Txt5DayCostEnd = ?FRTxt5DayCostEnd,
  Txt10DayCostBegin = ?FRTxt10DayCostBegin,
  Txt10DayCostEnd = ?FRTxt10DayCostEnd,
  Txt15DayCostBegin = ?FRTxt15DayCostBegin,
  Txt15DayCostEnd = ?FRTxt15DayCostEnd,
  Txt20DayCostBegin = ?FRTxt20DayCostBegin,
  Txt20DayCostEnd = ?FRTxt20DayCostEnd,
  Txt25DayCostBegin = ?FRTxt25DayCostBegin,
  Txt25DayCostEnd = ?FRTxt25DayCostEnd,
  Txt30DayCostBegin = ?FRTxt30DayCostBegin,
  Txt30DayCostEnd = ?FRTxt30DayCostEnd,
  Txt45DayCostBegin = ?FRTxt45DayCostBegin,
  Txt45DayCostEnd = ?FRTxt45DayCostEnd,
  TxtCurrencyBegin = ?FRTxtCurrencyBegin,
  TxtCurrencyEnd = ?FRTxtCurrencyEnd,
  TxtUnitBegin = ?FRTxtUnitBegin,
  TxtUnitEnd = ?FRTxtUnitEnd,
  TxtVolumeBegin = ?FRTxtVolumeBegin,
  TxtVolumeEnd = ?FRTxtVolumeEnd,
  TxtUpCostBegin = ?FRTxtUpCostBegin,
  TxtUpCostEnd = ?FRTxtUpCostEnd,
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
  TxtReserved3Begin = ?FRTxtReserved3Begin,
  TxtReserved3End = ?FRTxtReserved3End,
  TxtRequestRatioBegin = ?FRTxtRequestRatioBegin,
  TxtRequestRatioEnd = ?FRTxtRequestRatioEnd,
  TxtRegistryCostBegin = ?FRTxtRegistryCostBegin,
  TxtRegistryCostEnd = ?FRTxtRegistryCostEnd,
  TxtVitallyImportantBegin = ?FRTxtVitallyImportantBegin,
  TxtVitallyImportantEnd = ?FRTxtVitallyImportantEnd,
  TxtMaxBoundCostBegin = ?FRTxtMaxBoundCostBegin,
  TxtMaxBoundCostEnd = ?FRTxtMaxBoundCostEnd,

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

  Memo = ?FRMemo
where
  FirmCode = ?FRPriceCode;";
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRPriceCode", MySql.Data.MySqlClient.MySqlDbType.Int64, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRSynonyms", MySql.Data.MySqlClient.MySqlDbType.Int64, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRRules", MySql.Data.MySqlClient.MySqlDbType.Int64, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("SelfFlag", MySql.Data.MySqlClient.MySqlDbType.Int16, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRStartLine", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));

            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtCodeBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtCodeEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtCodeCrBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtCodeCrEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtNameBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtNameEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtFirmCrBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtFirmCrEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtCountryCrBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtCountryCrEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtBaseCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtBaseCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtMinBoundCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtMinBoundCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtAsFactCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtAsFactCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxt5DayCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxt5DayCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxt10DayCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxt10DayCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxt15DayCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxt15DayCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxt20DayCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxt20DayCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxt25DayCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxt25DayCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxt30DayCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxt30DayCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxt45DayCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxt45DayCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtCurrencyBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtCurrencyEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtUnitBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtUnitEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtVolumeBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtVolumeEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtUpCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtUpCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtQuantityBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtQuantityEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtNoteBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtNoteEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtPeriodBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtPeriodEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtDocBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtDocEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtJunkBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtJunkEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtAwaitBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtAwaitEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtReserved3Begin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtReserved3End", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtRequestRatioBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtRequestRatioEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtRegistryCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtRegistryCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtVitallyImportantBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtVitallyImportantEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtMaxBoundCostBegin", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRTxtMaxBoundCostEnd", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));

            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRCurrency", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRDelimiter", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRPosNum", MySql.Data.MySqlClient.MySqlDbType.Int64, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRSelfJunkPos", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRSelfAwaitPos", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFormat", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRListName", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRNameMask", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRForbWords", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRVitallyImportantMask", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));

            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFCode", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFCodeCr", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFName1", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFName2", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFName3", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFFirmCr", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFBaseCost", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFMinBoundCost", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFCurrency", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFUnit", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFVolume", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFQuantity", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFNote", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFPeriod", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFDoc", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFJunk", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFAwait", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFRequestRatio", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFRegistryCost", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFVitallyImportant", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFMaxBoundCost", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRMemo", MySql.Data.MySqlClient.MySqlDbType.VarString, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));

            foreach (MySqlParameter ms in this.mcmdUFormRules.Parameters)
            {
                ms.SourceColumn = ms.ParameterName;
            }

            MyCn.Open();
            MyCmd.Connection = MyCn;
            MyDA = new MySqlDataAdapter(MyCmd);

            dtCatalogCurrencyFill();
            dtPriceFMTsFill();
            cbRegionsFill();
            //FillTables(String.Empty, 0, -1);

            //PriceDataTableStyle.ColumnSizeAutoFit = true;

            MyCn.Close();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new frmFREMain());
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
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
                    MyCmd.Parameters.Add("ShortName", "%" + shname + "%");
                if (regcode != 0)
                    MyCmd.Parameters.Add("RegionCode", regcode);
                if (seg != -1)
                    MyCmd.Parameters.Add("Segment", seg);

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
            //dtClients.Clear();

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
            //dtPrices.Clear();

            MyCmd.CommandText =
@"SELECT
  distinct
  pd.FirmCode AS PFirmCode,
  pd.PriceName AS PFirmName,
  pd.PriceCode as PPriceCode,
  fr.DatePrevPrice as PDatePrevPrice,
  fr.DateCurPrice as PDateCurPrice,
  fr.DateLastForm as PDateLastForm,
  fr.MaxOld as PMaxOld,
  pd.PriceType as PPriceType,
  pd.CostType as PCostType,
  pd.WaitingDownloadInterval as PWaitingDownloadInterval,
  1 as PIsParent
FROM
  usersettings.pricesdata pd
inner join usersettings.pricescosts pc on pc.showpricecode = pd.pricecode
inner join usersettings.clientsdata cd on cd.FirmCode = pd.FirmCode
inner join farm.formrules fr on fr.FirmCode=pd.pricecode
inner join farm.regions r on r.regioncode=cd.regioncode
where
  cd.FirmType = 0
and pd.CostType = 0 ";
            MyCmd.CommandText += param;
            MyCmd.CommandText += @" 
union all
SELECT 
  distinct
  pd.FirmCode AS PFirmCode,
  pc.CostName AS PFirmName,
  pc.CostCode as PPriceCode,
  fr.DatePrevPrice as PDatePrevPrice,
  fr.DateCurPrice as PDateCurPrice,
  fr.DateLastForm as PDateLastForm,
  fr.MaxOld as PMaxOld,
  pd.PriceType as PPriceType,
  pd.CostType as PCostType,
  pd.WaitingDownloadInterval as PWaitingDownloadInterval,
  (pd.pricecode = pc.CostCode) as PIsParent
FROM
  usersettings.pricesdata pd
inner join usersettings.pricescosts pc on pc.showpricecode = pd.pricecode
inner join usersettings.clientsdata cd on cd.FirmCode = pd.FirmCode
inner join farm.formrules fr on fr.FirmCode=pc.CostCode
inner join farm.regions r on r.regioncode=cd.regioncode
where
  cd.FirmType = 0
and pd.CostType = 1 ";
            MyCmd.CommandText += param;
            MyCmd.CommandText += @" 
union all
SELECT
  distinct
  pd.FirmCode AS PFirmCode,
  pd.PriceName AS PFirmName,
  pd.PriceCode as PPriceCode,
  fr.DatePrevPrice as PDatePrevPrice,
  fr.DateCurPrice as PDateCurPrice,
  fr.DateLastForm as PDateLastForm,
  fr.MaxOld as PMaxOld,
  pd.PriceType as PPriceType,
  pd.CostType as PCostType,
  pd.WaitingDownloadInterval as PWaitingDownloadInterval,
  1 as PIsParent
FROM
  usersettings.pricesdata pd
inner join usersettings.pricescosts pc on pc.showpricecode = pd.pricecode
inner join usersettings.clientsdata cd on cd.FirmCode = pd.FirmCode
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
            dtRegions.Rows.Add(new object[] { -1, "���" });
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

            //cbRegions.Items.Clear();
            //cbRegions.Items.Add("���");
            //if(dtRegions.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dtRegions.Rows)
            //    {
            //         cbRegions.Items.Add(dr["Region"].ToString());
            //    }
            //}

            cbRegions.DataSource = dtRegions;
            cbRegions.DisplayMember = "Region";
            cbRegions.ValueMember = "RegionCode";

        }

        private void dtPricesCostFill(string param)
        {
            //dtPricesCost.Clear();

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
            //dtCostsFormRules.Clear();

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
            //dtFormRules.Clear();

            MyCmd.CommandText =
                @"SELECT
    IF(FR.ParentFormRules,FR.ParentFormRules,FR.FirmCode) AS FormID,
    FR.ParentSynonym AS FRSynonyms,
    FR.FirmCode AS FRPriceCode,
	FR.Currency AS FRCurrency,
	PFR.Delimiter As FRDelimiter,
	FR.ParentFormRules AS FRRules,
	FR.PriceFile AS FRPriceFile,
	FR.Memo As FRMemo,
    CD.ShortName AS ClientShortName,
    CD.FirmCode AS ClientCode,
    PD.PriceName AS FRName,
    FR.Flag AS SelfFlag,
    FR.JunkPos AS FRSelfJunkPos,
    FR.AwaitPos AS FRSelfAwaitPos,
    FR.PosNum AS FRPosNum,
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

	PFR.TxtAsFactCostBegin as FRTxtAsFactCostBegin,
	PFR.TxtAsFactCostEnd as FRTxtAsFactCostEnd,

	PFR.Txt5DayCostBegin as FRTxt5DayCostBegin,
	PFR.Txt5DayCostEnd as FRTxt5DayCostEnd,

	PFR.Txt10DayCostBegin as FRTxt10DayCostBegin,
	PFR.Txt10DayCostEnd as FRTxt10DayCostEnd,

	PFR.Txt15DayCostBegin as FRTxt15DayCostBegin,
	PFR.Txt15DayCostEnd as FRTxt15DayCostEnd,

	PFR.Txt20DayCostBegin as FRTxt20DayCostBegin,
	PFR.Txt20DayCostEnd as FRTxt20DayCostEnd,

	PFR.Txt25DayCostBegin as FRTxt25DayCostBegin,
	PFR.Txt25DayCostEnd as FRTxt25DayCostEnd,

	PFR.Txt30DayCostBegin as FRTxt30DayCostBegin,
	PFR.Txt30DayCostEnd as FRTxt30DayCostEnd,

	PFR.Txt45DayCostBegin as FRTxt45DayCostBegin,
	PFR.Txt45DayCostEnd as FRTxt45DayCostEnd,

	PFR.TxtCurrencyBegin as FRTxtCurrencyBegin,
	PFR.TxtCurrencyEnd as FRTxtCurrencyEnd,

	PFR.TxtUnitBegin as FRTxtUnitBegin,
	PFR.TxtUnitEnd as FRTxtUnitEnd,

	PFR.TxtVolumeBegin as FRTxtVolumeBegin,
	PFR.TxtVolumeEnd as FRTxtVolumeEnd,

	PFR.TxtUpCostBegin as FRTxtUpCostBegin,
	PFR.TxtUpCostEnd as FRTxtUpCostEnd,

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

	PFR.TxtReserved3Begin as FRTxtReserved3Begin,
	PFR.TxtReserved3End as FRTxtReserved3End,

	PFR.TxtRequestRatioBegin as FRTxtRequestRatioBegin,
	PFR.TxtRequestRatioEnd as FRTxtRequestRatioEnd,

	PFR.TxtRegistryCostBegin as FRTxtRegistryCostBegin,
	PFR.TxtRegistryCostEnd as FRTxtRegistryCostEnd,

	PFR.TxtVitallyImportantBegin as FRTxtVitallyImportantBegin,
	PFR.TxtVitallyImportantEnd as FRTxtVitallyImportantEnd,

	PFR.TxtMaxBoundCostBegin as FRTxtMaxBoundCostBegin,
	PFR.TxtMaxBoundCostEnd as FRTxtMaxBoundCostEnd,

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

    -- PFR.*,
    CD.FirmStatus,
    CD.BillingStatus,
    CD.FirmSegment,
	CD.OrderManagerMail as FRManager
FROM UserSettings.PricesData AS PD
INNER JOIN
    UserSettings.ClientsData AS CD on cd.FirmCode = pd.FirmCode and cd.FirmType = 0
INNER JOIN
    farm.regions r on r.regioncode=cd.regioncode
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

            //WHERE FR.DateCurPrice > '2005.01.01'

            MyDA.Fill(dtFormRules);
        }

        private void Form1_Closed(object sender, System.EventArgs e)
        {
            MyCn.Close();
        }

        //private void PriceGrid_ButtonPress(object sender, KeyEventArgs e)
        //{
        //    CurrencyManager currencyManager = (CurrencyManager)BindingContext[PriceGrid.DataSource, PriceGrid.DataMember];
        //    DataRowView drv = (currencyManager.Position > -1) ? (DataRowView)currencyManager.Current : null;
        //    DataView dv = (DataView)currencyManager.List;

        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        if ((currencyManager.Position > -1))
        //        {
        //            if (((DataView)currencyManager.List).Table.TableName == dtClients.TableName)
        //            {
        //                CheckOnePrice(currencyManager);
        //            }
        //            else
        //            {
        //                if (((DataView)currencyManager.List).Table.TableName == dtPrices.TableName)
        //                {
        //                    tbControl.SelectedTab = tpPrice;
        //                }
        //            }
        //        }
        //    }
        //    else
        //        if (e.KeyCode == Keys.Escape)
        //        {
        //            if (dv.Table.ParentRelations.Count > 0)
        //            {
        //                PriceGrid.NavigateBack();
        //                PriceGrid.Focus();
        //                PriceGrid.Select();
        //                e.Handled = true;
        //            }
        //        }
        //}

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
            //			if (existParentRules != String.Empty)
            //			{
            //				drFR = dtFormRules.Select(String.Format("FRPriceCode = {0}", existParentRules));
            //			}

            DataRow drC = drP.GetParentRow(dtClients.TableName + "-" + dtPrices.TableName);
            frmCaption = String.Format("{0}; {1}; ", drC["CShortName"], drC["CRegion"]);

            string f = drFR[0]["FRPriceCode"].ToString();

            fmt = drFR[0]["FRFormat"].ToString();

            string r = ".err";
            DataRow[] exts = dtPriceFMTs.Select("FMTFormat = '" + fmt + "'");
            if (exts.Length == 1)
                r = exts[0]["FMTExt"].ToString();

            delimiter = drFR[0]["FRDelimiter"].ToString();

            string takeFile = f + r;
            if (!(File.Exists(StartPath + takeFile)))
            {
                //tbControl.SelectedTab = tpFirms;
                fileExist = false;
                MessageBox.Show(String.Format("���� {0} ���������� �����-����� ����������� � ���������� �� ��������� ({1})", takeFile, StartPath), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            //delimiter = drFR[0]["FRDelimiter"].ToString().ToLower();
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
                this.Text = String.Format("�������� ������ ������������ ({0}{1})", frmCaption, f);
                Application.DoEvents();
            }
        }

        private void ShowTab(string fmt)
        {
            tcInnerTable.Visible = true;
            
            indgvPriceData.DataSource = dtPrice;

            //CreateColumns(PriceDataTableStyle, dtPrice);
            //CreateIndgvColumns(indgvPriceData, dtPrice);

            if ((fmt == "WIN") || (fmt == "DOS"))
            {
                if (delimiter == String.Empty)
                {
                    tcInnerTable.SizeMode = TabSizeMode.Normal;
                    tcInnerTable.ItemSize = new Size(58, 18);
                    tcInnerTable.Appearance = TabAppearance.Normal;

                    //lStartLine.Visible = true;
                    //txtBoxStartLine.Visible = true;
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

                    //lStartLine.Visible = true;
                    //txtBoxStartLine.Visible = true;
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

                    //lStartLine.Visible = true;
                    //txtBoxStartLine.Visible = true;
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

                        //lStartLine.Visible = false;
                        //txtBoxStartLine.Visible = false;
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
            //pnlFloat.Visible = true;
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
                //PriceDataGrid.DataSource = dtPrice;
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
                            //indgv.CaptionVisible = false;
                            indgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                            indgv.ReadOnly = true;
                            indgv.RowHeadersVisible = false;
                            indgv.MouseDown += new System.Windows.Forms.MouseEventHandler(this.indgvPriceData_MouseDown);
                            foreach (DataGridViewTextBoxColumn dc in indgv.Columns)
                                dc.Width = 300;
                            gds.Add(indgv);

                            DataTable dt = new DataTable();
                            dt.TableName = "�����" + (i + 1);
                            dtables.Add(dt);
                            //indg.DataSource = dt;
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
                                //PriceDataGrid.DataSource = dtPrice;
                            }
                            else
                            {
                                ((INDataGridView)gds[j - 1]).Columns.Clear();
//                                CreateThread(da, ((DataTable)(dtables[j - 1])), ((INDataGrid.INDataGrid)gds[j - 1]), indgvPriceData);
                                CreateThread(da, ((DataTable)(dtables[j - 1])), ((INDataGridView)gds[j - 1]));
                                ((INDataGridView)gds[j - 1]).DataSource = ((DataTable)(dtables[j - 1]));
                                //CreateColumns((INDataGrid.INDataGridTableStyle)tblstyles[j-1], (DataTable)(dtables[j-1]));
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

            Application.DoEvents();
            dbcMain.ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Text\"", System.IO.Path.GetDirectoryName(filePath));
            dbcMain.Open();
            OleDbDataAdapter da = new OleDbDataAdapter(String.Format("select * from {0}", System.IO.Path.GetFileName(filePath).Replace(".", "#")), dbcMain);

            CreateThread(da, dtPrice, indgvPriceData);
            //PriceDataGrid.DataSource = dtPrice;
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
            //foreach (string pf in FFieldNames)
            foreach (PriceFields pf in Enum.GetValues(typeof(PriceFields)))
            {
//                TmpName = pf;
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
                    mdr["MBeginField"] = "1";
                    mdr["MEndField"] = currTFD.posEnd;
                    dtMarking.Rows.Add(mdr);
                }
                else
                {
                    mdr = dtMarking.NewRow();
                    mdr["MNameField"] = String.Format("x{0}", countx);
                    mdr["MBeginField"] = "1";
                    mdr["MEndField"] = currTFD.posBegin-1;
//                    mdr["MEndField"] = currTFD.posEnd;
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
                    mdr["MEndField"] = "255";
                    dtMarking.Rows.Add(mdr);
                }
            }
            else
            {
                mdr = dtMarking.NewRow();
                mdr["MNameField"] = "x1";
                mdr["MBeginField"] = "1";
                mdr["MEndField"] = "255";
                dtMarking.Rows.Add(mdr);
            }
            dtSet.AcceptChanges();
        }

        private void OpenTXTFFile(string filePath, DataRow dr)
        {
            FillMarking(filePath, dr);
            OpenTable(fmt);
        }

        private void CreateIndgvColumns(INDataGridView indgv, DataTable dt)
        {
            indgv.Columns.Clear();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                INDataGridViewTextBoxColumn cs = new INDataGridViewTextBoxColumn();
                cs.Name = dt.Columns[i].ColumnName.ToString();
                cs.HeaderText = dt.Columns[i].ColumnName;
                cs.ReadOnly = true;
                indgv.SearchColumnName = cs.Name;
                indgv.Columns.Add(cs);
            }
        }

        private void DelCostsColumns()
        {
            indgvCosts.Columns[cFRFieldNameDataGridViewTextBoxColumn.Name].Visible = false;
            indgvCosts.Columns[cFRTextBeginDataGridViewTextBoxColumn.Name].Visible = false;
            indgvCosts.Columns[cFRTextEndDataGridViewTextBoxColumn.Name].Visible = false;

            //for (int i = indgvCosts.Columns.Count - 1; i > 0; i--)
            //{
            //    if ((indgvCosts.Columns[i].Name == CFRCost_Code.ColumnName) || (indgvCosts.Columns[i].Name == CFRCostName.ColumnName))
            //    {
            //    }
            //    else
            //    {
            //        indgvCosts.Columns.Remove(indgvCosts.Columns[i]);
            //    }
            //  }
        }

        public string strToANSI(string Dest)
        {
            System.Text.Encoding ansi = System.Text.Encoding.GetEncoding(1251);
            byte[] unicodeBytes = System.Text.Encoding.Unicode.GetBytes(Dest);
            byte[] ansiBytes = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, ansi, unicodeBytes);
            return ansi.GetString(ansiBytes);
        }

        //private void PriceDataGrid_ButtonPress(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Escape)
        //        tbControl.SelectedTab = tpFirms;
        //}

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
                while ((i < dtMarking.Rows.Count) && (flag))
                {
                    DataRow drP = dtMarking.Rows[i - 1];
                    DataRow drN = dtMarking.Rows[i];
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
                        MessageBox.Show("������������ ���� ����!", "�����, ����� �����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                dtSet.AcceptChanges();
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
                tsbApply.Enabled = false;
                tsbCancel.Enabled = false;
                tmrUpdateApply.Stop();
                //pnlFloat.Visible = false;
            }
            else
                if (tbControl.SelectedTab == tpPrice)
                {
                    CurrencyManager currencyManager = (CurrencyManager)BindingContext[indgvPrice.DataSource, indgvPrice.DataMember];
                    DataRowView drv = (DataRowView)currencyManager.Current;
                    DataView dv = (DataView)currencyManager.List;

                    DataRow drP = drv.Row;
                    openedPriceDR = drP;
                    DoOpenPrice(drP);
                    tmrUpdateApply.Start();
                    bsCostsFormRules.Filter = "CFRfr_if = " + drP[PPriceCode.ColumnName].ToString();
                    bsFormRules.Filter = "FRPriceCode = " + drP[PPriceCode.ColumnName].ToString();
                    bsCostsFormRules.ResumeBinding();
                    bsFormRules.ResumeBinding();
                }
        }

		private void RefreshDataBind()
		{
			long ClientCode = (long)(((DataRowView)indgvFirm.CurrentRow.DataBoundItem)[CCode.ColumnName]);
			long PriceCode = (long)(((DataRowView)indgvPrice.CurrentRow.DataBoundItem)[PPriceCode.ColumnName]);
			FillTables(shortNameFilter, regionCodeFilter, segmentFilter);

			this.Text = "�������� ������ ������������";
			if (fcs == dgFocus.Firm)
			{
				indgvFirm.Focus();
				CurrencyManagerPosition((CurrencyManager)BindingContext[indgvFirm.DataSource, indgvFirm.DataMember], CCode.ColumnName, ClientCode);
			}
			else
				if (fcs == dgFocus.Price)
				{
					indgvPrice.Select();
					CurrencyManagerPosition((CurrencyManager)BindingContext[indgvFirm.DataSource, indgvFirm.DataMember], CCode.ColumnName, ClientCode);
					CurrencyManagerPosition((CurrencyManager)BindingContext[indgvPrice.DataSource, indgvPrice.DataMember], PPriceCode.ColumnName, PriceCode);
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
            System.Diagnostics.Process.Start(String.Format("mailto:{0}", lLblMaster.Text));
        }

        private void CheckOnePrice(CurrencyManager currencyManager)
        {
            DataRowView drv = (currencyManager.Position > -1) ? (DataRowView)currencyManager.Current : null;
            DataView dv = (DataView)currencyManager.List;

            if (drv.Row.GetChildRows(dtClients.TableName + "-" + dtPrices.TableName).Length > 1)
            {
                indgvPrice.Focus();
                //PriceGrid.NavigateTo(currencyManager.Position, dtClients.TableName + "-" + dtPrices.TableName);
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

        //		public void FillComplete(IAsyncResult ar)
        //		{
        //			try
        //			{
        //				Debug.WriteLine("FillComplete begin");
        //				fW.BeginInvoke(new CloseDelegate(WaitClose));
        //			}
        //			catch(Exception e)
        //			{
        //				Debug.WriteLine(String.Format("FillComplete error {0}", e));
        //			}
        //			finally
        //			{
        //				Debug.WriteLine("FillComplete end");
        //			}
        //		}

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
               // if (!(fmt0.Equals(cmbFormat.Text.ToLower())) || !(delimiter0.Equals(tbDevider.Text.ToLower())))
                if (!(fmt.Equals(cmbFormat.Text)) || !(delimiter.Equals(tbDevider.Text)))
                        DoOpenPrice(openedPriceDR);
                
                foreach (INDataGridView indg in gds)
                {
                    indg.RowHeadersVisible = false;
                }
                //PriceDataTableStyle.RowHeadersVisible = false;

                //erP.Clear();
            }
            else
            {
                pnlFloat.Visible = true;
                pnlFloat.BringToFront();
                //fmt0 = fmt;
                //delimiter0 = delimiter;
            }
        }

        private void btnFloatPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.DrawString("���������", btnFloatPanel.Font, Brushes.Black, btnFloatPanel.ClientRectangle, new System.Drawing.StringFormat(sf));
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
                    //r = new Regex(@"((?<Day>\d*?)[\s\.\-\/])?(?<Month>[^\s\.\-\/]*?)[\s\.\-\/](?<Year>\d*)$"); 
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

                    erP.SetError(txtMask, "����������� ������ �����!");
                    //MessageBox.Show(String.Format("����������� ������ �����"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        dr.RowError = "�������������� �����";
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
                DataRow dr = dtPrice.Rows[indgvPriceData.CurrentRow.Index];
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
                    MessageBox.Show(String.Format("������ ������� �����!"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                MessageBox.Show(String.Format("�� ������� ���� ������������!"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show(String.Format("�� ������� ����� ������� ������!"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
                                    dr.RowError = "�������������� �����";
                                }
                            }
                        }
                    }
                    else
                    {
                        indgv.RowHeadersVisible = true;
                        dr.RowError = "�������������� ����";
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
                //TODO: ����� ���� ��������� �������
                //cm = (CurrencyManager)BindingContext[indgvCosts.DataSource, "����������.����������-������.������-�������"];
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
                    if (MessageBox.Show("�������� �� ���������� ������. ���������?", "��������", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
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
                    ((CurrencyManager)BindingContext[indgvPriceData.DataSource, indgvPriceData.DataMember]).EndCurrentEdit();

                    if (MyCn.State == ConnectionState.Closed)
                        MyCn.Open();
                    try
                    {
                        MySqlTransaction tr = MyCn.BeginTransaction();
                        try
                        {
                            MySqlCommand SetCMD = new MySqlCommand("set @INHost = ?INHost; set @INUser = ?INUser;", MyCn, tr);
                            SetCMD.Parameters.Add("INHost", Environment.MachineName);
                            SetCMD.Parameters.Add("INUser", Environment.UserName);
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
                            SetCMD.Parameters.Add("INHost", Environment.MachineName);
                            SetCMD.Parameters.Add("INUser", Environment.UserName);
                            SetCMD.ExecuteNonQuery();

                            MySqlCommand mcmdUPrice = new MySqlCommand();
                            MySqlDataAdapter daPrice = new MySqlDataAdapter();
                            mcmdUPrice.CommandText = @"
UPDATE usersettings.PricesData pd SET
    PriceType = ?PPriceType,
    CostType = ?PCostType,
    WaitingDownloadInterval = ?PWaitingDownloadInterval
WHERE 
    pd.PriceCode = ?PPriceCode;
UPDATE farm.FormRules fr SET
    MaxOld = ?PMaxOld
WHERE fr.FirmCode = ?PPriceCode;";

                            mcmdUPrice.Parameters.Clear();
                            mcmdUPrice.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("PPriceType", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
                            mcmdUPrice.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("PCostType", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
                            mcmdUPrice.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("PWaitingDownloadInterval", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
                            mcmdUPrice.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("PMaxOld", MySql.Data.MySqlClient.MySqlDbType.Int32, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
                            mcmdUPrice.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("PPriceCode", MySql.Data.MySqlClient.MySqlDbType.Int64, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));

                            foreach (MySqlParameter ms in mcmdUPrice.Parameters)
                            {
                                ms.SourceColumn = ms.ParameterName;
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

            //CommitAllEdit();
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
                //tbFirmName.Text = String.Empty;
            }
            finally
            {
                InSearch = false;
            }

            //{
            //    if (FilterString != String.Empty)
            //        FilterString += " and ";
            //    int segment = -1;
            //    if (cbSegment.SelectedItem.ToString() == "���")
            //        segment = 0;
            //    else if (cbSegment.SelectedItem.ToString() == "�������")
            //        segment = 1;
            //    FilterString += String.Format("CSegment = {0}", segment);
            //}
            //if (tbFirmName.Text != String.Empty)
            //{
            //    if (FilterString != String.Empty)
            //        FilterString += " and ";
            //    FilterString += String.Format("CShortName like '%{0}%'", tbFirmName.Text);
            //}

            //CurrencyManager cm = (CurrencyManager)BindingContext[indgvFirm.DataSource, indgvFirm.DataMember];
            //((DataView)cm.List).RowFilter = FilterString;
            //FillTables(shname, 0, seg);
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
                    e.Value = "���";
                else if ((int)e.Value == 1)
                    e.Value = "�������";
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
                MessageBox.Show("���������� ������� ��� ����!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                
                CurrencyManager currencyManager = (CurrencyManager)BindingContext[indgvPrice.DataSource, indgvPrice.DataMember];
                DataRowView drv = (DataRowView)currencyManager.Current;
                DataView dv = (DataView)currencyManager.List;

                DataRow drP = drv.Row;
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
			//���� ������������ ComboBox, �� ������������� �� ��� �������: ��������� �������� ��������
			if (e.Control is ComboBox)
				((ComboBox)e.Control).SelectionChangeCommitted += new EventHandler(frmFREMain_SelectionChangeCommitted);
		}

		void frmFREMain_SelectionChangeCommitted(object sender, EventArgs e)
		{
			//����� ��������� �������� ����� ��������� ��� � ������, ����� �������� ������� CellValueChanged
			indgvPrice.CurrentCell.Value = ((ComboBox)sender).SelectedValue;
		}

        private void btnVitallyImportantCheck_Click(object sender, EventArgs e)
        {
            FindErrors(txtBoxVitallyImportantMask, txtBoxVitalyImportant, txtBoxVitalyImportantBegin, txtBoxVitalyImportantEnd);
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
}
