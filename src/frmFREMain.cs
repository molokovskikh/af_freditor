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

namespace FREditor
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
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
        OriginalName
    }

    public partial class frmFREMain : System.Windows.Forms.Form
    {
        ArrayList gds = new ArrayList();
        ArrayList dtables = new ArrayList();
        ArrayList tblstyles = new ArrayList();
        DataTable dtPrice = new DataTable();
        //INDataGrid.INDataGridTableStyle PriceDataTableStyle = new INDataGrid.INDataGridTableStyle();

#if DEBUG
        private MySqlConnection MyCn = new MySqlConnection("server=TestSQL.analit.net; user id=system; password=123; database=farm; Allow Zero Datetime=True;");
#else
		private MySqlConnection MyCn = new MySqlConnection("server=sql.analit.net; user id=system; password=123; database=farm; Allow Zero Datetime=True;");
#endif
        private MySqlCommand MyCmd = new MySqlCommand();
        private MySqlDataAdapter MyDA = new MySqlDataAdapter();

        private OleDbConnection dbcMain = new OleDbConnection();

        //string StartPath = "\\"+"\\"+"FMS" + "\\" + "Prices" + "\\" + "Base" + "\\";
        string StartPath = "C:\\TEMP\\Base\\";
        string EndPath = Path.GetTempPath();
        //string EndPath = "C:" + "\\" + "PricesCopy" + "\\";
        string TxtFilePath = String.Empty;
        string frmCaption = String.Empty;

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

        bool firstFind = false;

        StringFormat sf = new StringFormat();

        ArrayList fds;

        public frmWait fW = null;
        public frmNameMask frmNM = null;

        public frmFREMain()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //this.mcmdUCostRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FirmCode", MySql.Data.MySqlClient.MySqlDbType.Int64, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "CFRFR_if", System.Data.DataRowVersion.Current, null));
            this.mcmdUCostRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("CostCode", MySql.Data.MySqlClient.MySqlDbType.Int64, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "CFRCost_Code", System.Data.DataRowVersion.Current, null));
            this.mcmdUCostRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FieldName", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "CFRFieldName", System.Data.DataRowVersion.Current, null));
            this.mcmdUCostRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("TxtBegin", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "CFRTextBegin", System.Data.DataRowVersion.Current, null));
            this.mcmdUCostRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("TxtEnd", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), "CFRTextEnd", System.Data.DataRowVersion.Current, null));

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
  FAwait = ?FRFAwait
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

            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRCurrency", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRDelimiter", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRSelfJunkPos", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRSelfAwaitPos", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFormat", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRListName", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRNameMask", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRForbWords", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));

            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFCode", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFCodeCr", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFName1", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFName2", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFName3", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFFirmCr", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFBaseCost", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFMinBoundCost", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFCurrency", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFUnit", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFVolume", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFQuantity", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFNote", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFPeriod", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFDoc", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFJunk", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));
            this.mcmdUFormRules.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("FRFAwait", MySql.Data.MySqlClient.MySqlDbType.String, 0, System.Data.ParameterDirection.Input, false, ((byte)(0)), ((byte)(0)), null, System.Data.DataRowVersion.Current, null));

            foreach (MySqlParameter ms in this.mcmdUFormRules.Parameters)
            {
                ms.SourceColumn = ms.ParameterName;
            }


            MyCn.Open();
            MyCmd.Connection = MyCn;
            MyDA = new MySqlDataAdapter(MyCmd);

            dtClientsFill();
            dtPricesFill();
            dtPricesCostFill();
            dtFormRulesFill();
            dtCatalogCurrencyFill();
            dtPriceFMTsFill();
            dtCostsFormRulesFill();

            dtPrice.TableName = "Прайс";
            PriceDataTableStyle.MappingName = dtPrice.TableName;
            PriceDataTableStyle.RowHeadersVisible = false;
            PriceDataTableStyle.ColumnSizeAutoFit = true;

            MyCn.Close();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
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
            PriceGrid.Focus();
            PriceGrid.Select();

            sf.Alignment = StringAlignment.Center;
            sf.FormatFlags = StringFormatFlags.DirectionVertical;
            sf.LineAlignment = StringAlignment.Center;
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
					Format AS FMTFormat
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

        private void dtClientsFill()
        {
            dtClients.Clear();

            MyCmd.CommandText =
                @"SELECT 
					cd.FirmCode AS CCode,
					cd.ShortName AS CShortName,
					cd.FullName AS CFullName,
					r.Region AS CRegion
				FROM 
					usersettings.clientsdata cd
                    inner join farm.regions r on r.RegionCode = cd.RegionCode
				WHERE cd.firmtype = 0 ORDER BY cd.ShortName";

            MyDA.Fill(dtClients);
        }

        private void dtPricesFill()
        {
            dtPrices.Clear();

            MyCmd.CommandText =
                @"SELECT distinct
          pd.FirmCode AS PFirmCode,
          pd.PriceName AS PFirmName,
                    pd.PriceCode as PPriceCode
        FROM
          usersettings.pricesdata pd
inner join usersettings.pricescosts pc on pc.showpricecode = pd.pricecode
inner join usersettings.clientsdata cd on cd.FirmCode = pd.FirmCode
where
  cd.FirmType = 0
order by pd.pricename";

            MyDA.Fill(dtPrices);
        }

        private void dtPricesCostFill()
        {
            dtPricesCost.Clear();

            MyCmd.CommandText =
                @"SELECT distinct
					pc.ShowPriceCode AS PCPriceCode,
					pc.CostCode AS PCCostCode,
                    pc.BaseCost as PCBaseCost,
					pc.CostName as PCCostName
				FROM 
					usersettings.pricescosts pc
inner join usersettings.pricesdata pd on pd.pricecode = pc.showpricecode
inner join usersettings.clientsdata cd on cd.firmcode = pd.firmcode
where cd.firmtype = 0
order by pc.costName";

            MyDA.Fill(dtPricesCost);
        }

        private void dtCostsFormRulesFill()
        {
            dtCostsFormRules.Clear();

            MyCmd.CommandText =
                @"SELECT distinct
					-- cfr.FR_id AS CFRfr_if,
					pc.showpricecode AS CFRfr_if,
					pc.CostName as CFRCostName,
					cfr.PC_CostCode AS CFRCost_Code,
					cfr.FieldName AS CFRFieldName,
                    cfr.TxtBegin as CFRTxtBegin,
					cfr.TxtEnd as CFRTxtEnd
				FROM 
					farm.costformrules cfr
inner join usersettings.pricescosts pc on pc.CostCode = cfr.pc_costcode
inner join usersettings.pricesdata pd on pd.pricecode = pc.showpricecode
inner join usersettings.clientsdata cd on cd.firmcode = pd.firmcode
where cd.firmtype = 0
order by pc.costName";

            MyDA.Fill(dtCostsFormRules);
        }

        private void dtFormRulesFill()
        {
            dtFormRules.Clear();

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

    -- PFR.*,
    CD.FirmStatus,
    CD.BillingStatus,
    CD.FirmSegment,
	CD.OrderManagerMail as FRManager
FROM UserSettings.PricesData AS PD
INNER JOIN
    UserSettings.ClientsData AS CD on cd.FirmCode = pd.FirmCode and cd.FirmType = 0
INNER JOIN
    Farm.formrules AS FR
    ON FR.FirmCode=PD.PriceCode
LEFT JOIN
    Farm.FormRules AS PFR
    ON PFR.FirmCode=IF(FR.ParentFormRules,FR.ParentFormRules,FR.FirmCode)
where exists(select * from usersettings.pricescosts pc where pc.ShowPriceCode = pd.PriceCode)";
            //WHERE FR.DateCurPrice > '2005.01.01'

            MyDA.Fill(dtFormRules);
        }

        private void Form1_Closed(object sender, System.EventArgs e)
        {
            MyCn.Close();
        }

        private void PriceGrid_ButtonPress(object sender, KeyEventArgs e)
        {
            CurrencyManager currencyManager = (CurrencyManager)BindingContext[PriceGrid.DataSource, PriceGrid.DataMember];
            DataRowView drv = (currencyManager.Position > -1) ? (DataRowView)currencyManager.Current : null;
            DataView dv = (DataView)currencyManager.List;

            if (e.KeyCode == Keys.Enter)
            {
                if ((currencyManager.Position > -1))
                {
                    if (((DataView)currencyManager.List).Table.TableName == dtClients.TableName)
                    {
                        CheckOnePrice(currencyManager);
                    }
                    else
                    {
                        if (((DataView)currencyManager.List).Table.TableName == dtPrices.TableName)
                        {
                            tbControl.SelectedTab = tpPrice;
                        }
                    }
                }
            }
            else
                if (e.KeyCode == Keys.Escape)
                {
                    if (dv.Table.ParentRelations.Count > 0)
                    {
                        PriceGrid.NavigateBack();
                        PriceGrid.Focus();
                        PriceGrid.Select();
                        e.Handled = true;
                    }
                }
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
            //			if (existParentRules != String.Empty)
            //			{
            //				drFR = dtFormRules.Select(String.Format("FRPriceCode = {0}", existParentRules));
            //			}

            DataRow drC = drP.GetParentRow(dtClients.TableName + "-" + dtPrices.TableName);
            frmCaption = String.Format("{0}; {1}; ", drC["CShortName"], drC["CRegion"]);

            string f = drFR[0]["FRPriceCode"].ToString();
            string r = String.Empty;
            fmt = drFR[0]["FRFormat"].ToString().ToLower();
            delimiter = drFR[0]["FRDelimiter"].ToString().ToLower();
            //				if (fmt0 == String.Empty)
            //					fmt0 = fmt;

            //				if((fmt0 == "win")||(fmt0 == "dos"))
            //					r = "txt";
            //				else
            //					r = fmt;

            if ((fmt == "win") || (fmt == "dos"))
                r = "txt";
            else
                r = fmt;

            string takeFile = f + "." + r;
            if (!(File.Exists(StartPath + takeFile)))
            {
                //tbControl.SelectedTab = tpFirms;
                MessageBox.Show(String.Format("Файл {0} выбранного Прайс-листа отсутствует в директории по умолчанию ({1})", takeFile, StartPath), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                Directory.CreateDirectory(EndPath + f);

                File.Copy(StartPath + takeFile, EndPath + f + "\\" + takeFile, true);
                string filePath = EndPath + f + "\\" + takeFile;

                Application.DoEvents();

                if ((fmt == "db") || (fmt == "dbf"))
                {
                    OpenDBFFile(filePath);
                }
                else
                    if (fmt == "xls")
                    {
                        listName = drFR[0]["FRListName"].ToString();
                        OpenEXLFile(filePath);
                    }
                    else
                        if ((fmt == "dos") || (fmt == "win"))
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
                CreateColumns(PriceDataTableStyle, dtPrice);
                Application.DoEvents();
                ShowTab(fmt);
                Application.DoEvents();
                this.Text = String.Format("Редактор Правил Формализации ({0}{1})", frmCaption, f);
                Application.DoEvents();
            }
        }

        private void ShowTab(string fmt)
        {
            tcInnerTable.Visible = true;
            INDataGrid.INDataGridColorTextBoxColumn c;

            PriceDataGrid.DataSource = dtPrice;
            if ((fmt == "win") || (fmt == "dos"))
            {
                if (delimiter == String.Empty)
                {
                    tcInnerTable.SizeMode = TabSizeMode.Normal;
                    tcInnerTable.ItemSize = new Size(58, 18);
                    tcInnerTable.Appearance = TabAppearance.Normal;

                    label22.Visible = false;
                    txtBoxStartLine.Visible = false;
                    label23.Visible = false;
                    txtBoxSheetName.Visible = false;
                    pnlGeneralFields.Visible = false;
                    pnlTxtFields.Visible = true;

                    c = new INDataGrid.INDataGridColorTextBoxColumn();
                    c.MappingName = "CFRTxtBegin";
                    c.NullText = String.Empty;
                    c.HeaderText = "Начало";
                    c.EditDisable = true;
                    CostsFormRulesTableStyle.GridColumnStyles.Add(c);

                    c = new INDataGrid.INDataGridColorTextBoxColumn();
                    c.MappingName = "CFRTxtEnd";
                    c.NullText = String.Empty;
                    c.HeaderText = "Конец";
                    c.EditDisable = true;
                    CostsFormRulesTableStyle.GridColumnStyles.Add(c);
                }
                else
                {
                    tcInnerTable.SizeMode = TabSizeMode.Fixed;
                    tcInnerTable.ItemSize = new Size(0, 1);
                    tcInnerTable.Appearance = TabAppearance.Buttons;

                    label22.Visible = false;
                    txtBoxStartLine.Visible = false;
                    label23.Visible = false;
                    txtBoxSheetName.Visible = false;
                    pnlGeneralFields.Visible = true;
                    pnlTxtFields.Visible = false;

                    c = new INDataGrid.INDataGridColorTextBoxColumn();
                    c.MappingName = "CFRFieldName";
                    c.NullText = String.Empty;
                    c.HeaderText = "Поле";
                    c.EditDisable = true;
                    CostsFormRulesTableStyle.GridColumnStyles.Add(c);
                }
                tcInnerSheets.SizeMode = TabSizeMode.Fixed;
                tcInnerSheets.ItemSize = new Size(0, 1);
                tcInnerSheets.Appearance = TabAppearance.FlatButtons;
            }
            else
            {
                if (fmt == "xls")
                {
                    tcInnerTable.SizeMode = TabSizeMode.Fixed;
                    tcInnerTable.ItemSize = new Size(0, 1);
                    tcInnerTable.Appearance = TabAppearance.Buttons;

                    tcInnerSheets.SizeMode = TabSizeMode.Normal;
                    tcInnerSheets.ItemSize = new Size(58, 18);
                    tcInnerSheets.Appearance = TabAppearance.Normal;

                    label22.Visible = true;
                    txtBoxStartLine.Visible = true;
                    label23.Visible = true;
                    txtBoxSheetName.Visible = true;
                    pnlGeneralFields.Visible = true;
                    pnlTxtFields.Visible = false;
                }
                else
                    if ((fmt == "db") || (fmt == "dbf"))
                    {
                        tcInnerTable.SizeMode = TabSizeMode.Fixed;
                        tcInnerTable.ItemSize = new Size(0, 1);
                        tcInnerTable.Appearance = TabAppearance.Buttons;

                        tcInnerSheets.SizeMode = TabSizeMode.Fixed;
                        tcInnerSheets.ItemSize = new Size(0, 1);
                        tcInnerSheets.Appearance = TabAppearance.Buttons;

                        label22.Visible = false;
                        txtBoxStartLine.Visible = false;
                        label23.Visible = false;
                        txtBoxSheetName.Visible = false;
                        pnlGeneralFields.Visible = true;
                        pnlTxtFields.Visible = false;
                    }

                pnlFloat.Visible = false;
                c = new INDataGrid.INDataGridColorTextBoxColumn();
                c.MappingName = "CFRFieldName";
                c.NullText = String.Empty;
                c.HeaderText = "Поле";
                c.EditDisable = true;
                CostsFormRulesTableStyle.GridColumnStyles.Add(c);
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

            PriceDataTableStyle.RowHeadersVisible = false;
            PriceDataGrid.Focus();
            PriceDataGrid.Select();
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
                CreateThread(da, dtPrice, PriceDataGrid);
                PriceDataGrid.DataSource = dtPrice;
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

                    INDataGrid.INDataGrid indg;
                    INDataGrid.INDataGridTableStyle tblstyle;
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
                            indg = new INDataGrid.INDataGrid();
                            indg.Name = "PriceDataGrid" + (i + 1);
                            indg.Parent = tp;
                            indg.ButtonPress += new INDataGrid.INDataGridKeyPressEventHandler(PriceDataGrid_ButtonPress);
                            indg.Dock = DockStyle.Fill;
                            indg.CaptionVisible = false;
                            indg.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PriceDataGrid_MouseDown);
                            gds.Add(indg);
                            DataTable dt = new DataTable();
                            dt.TableName = "Прайс" + (i + 1);
                            dtables.Add(dt);
                            tblstyle = new INDataGrid.INDataGridTableStyle();
                            tblstyle.MappingName = dt.TableName;
                            //								tblstyle.DataGrid = indg;
                            tblstyle.ColumnSizeAutoFit = true;
                            tblstyle.ReadOnly = true;
                            tblstyle.RowHeadersVisible = false;
                            tblstyles.Add(tblstyle);
                            indg.TableStyles.Add(tblstyle);
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
                                CreateThread(da, dtPrice, PriceDataGrid);
                                PriceDataGrid.DataSource = dtPrice;
                            }
                            else
                            {
                                ((INDataGrid.INDataGridTableStyle)tblstyles[j - 1]).GridColumnStyles.Clear();
                                CreateThread(da, ((DataTable)(dtables[j - 1])), ((INDataGrid.INDataGrid)gds[j - 1]));
                                ((INDataGrid.INDataGrid)gds[j - 1]).DataSource = ((DataTable)(dtables[j - 1]));
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

            CreateThread(da, dtPrice, PriceDataGrid);
            PriceDataGrid.DataSource = dtPrice;
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
                if (PriceFields.OriginalName != pf && PriceFields.BaseCost != pf && null != TmpName)
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
                    mdr["MEndField"] = currTFD.posEnd;
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
        }

        private void OpenTXTFFile(string filePath, DataRow dr)
        {
            FillMarking(filePath, dr);
            OpenTable(fmt);
        }

        public delegate void ttAa(INDataGrid.INDataGridTableStyle ts, INDataGrid.INDataGridColorTextBoxColumn cm);

        void aa(INDataGrid.INDataGridTableStyle ts, INDataGrid.INDataGridColorTextBoxColumn cm)
        {
            ts.GridColumnStyles.Add(cm);
        }

        private void CreateColumns(INDataGrid.INDataGridTableStyle ts, DataTable dt)
        {
            ts.GridColumnStyles.Clear();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                INDataGrid.INDataGridColorTextBoxColumn cs = new INDataGrid.INDataGridColorTextBoxColumn();
                cs.MappingName = dt.Columns[i].ColumnName.ToString();
                cs.HeaderText = dt.Columns[i].ColumnName;
                cs.NullText = String.Empty;
                //ts.DataGrid.BeginInvoke(new ttAa(aa), new object[] { ts, cs });
                ts.GridColumnStyles.Add(cs);
            }
        }

        private void DelCostsColumns()
        {
            for (int i = CostsFormRulesTableStyle.GridColumnStyles.Count - 1; i > 0; i--)
            {
                if ((CostsFormRulesTableStyle.GridColumnStyles[i].MappingName == CFRCost_Code.ColumnName) || (CostsFormRulesTableStyle.GridColumnStyles[i].MappingName == CFRCostName.ColumnName))
                {
                }
                else
                {
                    CostsFormRulesTableStyle.GridColumnStyles.Remove(CostsFormRulesTableStyle.GridColumnStyles[i]);
                }
            }
        }

        public string strToANSI(string Dest)
        {
            System.Text.Encoding ansi = System.Text.Encoding.GetEncoding(1251);
            byte[] unicodeBytes = System.Text.Encoding.Unicode.GetBytes(Dest);
            byte[] ansiBytes = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, ansi, unicodeBytes);
            return ansi.GetString(ansiBytes);
        }

        private void PriceDataGrid_ButtonPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                tbControl.SelectedTab = tpFirms;
        }

        private void tcInnerSheets_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (tcInnerSheets.SelectedTab == tbpSheet1)
            {
                PriceDataGrid.DataSource = dtPrice;
            }
            else
            {
                ((INDataGrid.INDataGrid)gds[tcInnerSheets.SelectedIndex - 1]).DataSource = ((DataTable)dtables[tcInnerSheets.SelectedIndex - 1]);
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
                    PriceDataTableStyle.GridColumnStyles.Clear();

                    CreateThread(da, dtPrice, PriceDataGrid);
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
        }

        private void DoOpenTable(DataRow drFict)
        {
            dtMarking.AcceptChanges();
            Application.DoEvents();
            PriceDataGrid.DataSource = null;
            Application.DoEvents();

            OpenTable(fmt);
            Application.DoEvents();

            PriceDataGrid.DataSource = dtPrice;
            Application.DoEvents();
            CreateColumns(PriceDataTableStyle, dtPrice);
            Application.DoEvents();
        }

        private void ClearPrice()
        {
            tcInnerSheets.SizeMode = TabSizeMode.Normal;
            tcInnerSheets.ItemSize = new Size(0, 1);
            tcInnerSheets.Appearance = TabAppearance.Normal;//

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

            PriceDataTableStyle.GridColumnStyles.Clear();

            gds.Clear();
            dtables.Clear();
            tblstyles.Clear();
            dtMarking.Clear();

            PriceDataGrid.DataSource = null;

            dtPrice.Rows.Clear();
            dtPrice.Columns.Clear();
            dtPrice.Clear();

            DelCostsColumns();

            label22.Visible = false;
            txtBoxStartLine.Visible = false;
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
            dtClientsFill();
            dtPricesFill();
            dtPricesCostFill();
            dtFormRulesFill();
            dtCatalogCurrencyFill();
            dtPriceFMTsFill();
            dtCostsFormRulesFill();
        }

        private void tbControl_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (tbControl.SelectedTab == tpFirms)
            {
                //TODO: восстанавливать позицию в таблице клиентов и прайс-листов.
                int CRI = PriceGrid.CurrentRowIndex;
                RefreshDataSet();
                PriceGrid.CurrentRowIndex = CRI;
                PriceGrid.Focus();
                PriceGrid.Select();
                this.Text = "Редактор Правил Формализации";
            }
            else
                if (tbControl.SelectedTab == tpPrice)
                {
                    //	fmt0 = String.Empty;
                    //	ClearPrice();
                    CurrencyManager currencyManager = (CurrencyManager)BindingContext[PriceGrid.DataSource, PriceGrid.DataMember];
                    DataRowView drv = (DataRowView)currencyManager.Current;
                    DataView dv = (DataView)currencyManager.List;

                    if (drv.Row.Table.TableName == dtPrices.TableName)
                    {
                        DataRow drP = drv.Row;
                        openedPriceDR = drP;
                        DoOpenPrice(drP);
                    }
                    else
                        if (drv.Row.Table.TableName == dtClients.TableName)
                        {
                            if (firstFind)
                            {
                                DataRow[] drs = drv.Row.GetChildRows(dtClients.TableName + "-" + dtPrices.TableName);
                                if (drs.Length > 0)
                                {
                                    //							foreach(DataRow drP in drs)
                                    //							{
                                    //								string rl = String.Empty;
                                    //
                                    //								DataRow[] drFR = dtFormRules.Select("FRPriceCode = " + drP["PPriceCode"].ToString());
                                    //
                                    //								string fl = drFR[0]["FRPriceCode"].ToString();
                                    //								fmt = drFR[0]["FRFormat"].ToString().ToLower();
                                    ////								if(fmt0 == String.Empty)
                                    ////									fmt0 = fmt;
                                    //
                                    ////								if((fmt0 == "win")||(fmt0 == "dos"))
                                    ////									rl = "txt";
                                    ////								else
                                    ////									rl = fmt;
                                    //
                                    //								if((fmt == "win")||(fmt == "dos"))
                                    //									rl = "txt";
                                    //								else
                                    //									rl = fmt;

                                    //								if(File.Exists(StartPath + fl + "." + rl))
                                    //								{
                                    openedPriceDR = drs[0];
                                    DoOpenPrice(drs[0]);
                                    //									break;
                                    //								}
                                    //							}
                                }
                                else
                                    tbControl.SelectedTab = tpFirms;
                                firstFind = false;
                            }
                            else
                            {
                                DataRow[] drs = drv.Row.GetChildRows(dtClients.TableName + "-" + dtPrices.TableName);
                                if (drs.Length > 0)
                                {
                                    DataRow drP = drs[0];
                                    openedPriceDR = drP;
                                    DoOpenPrice(drP);
                                }
                                else
                                    tbControl.SelectedTab = tpFirms;
                            }
                        }
                }
        }

        private void MethodForThread(OleDbDataAdapter da, DataTable dt)
        {
            da.Fill(dt);
        }

        private void PriceDataGrid_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            INDataGrid.INDataGrid dg = (INDataGrid.INDataGrid)sender;
            Point p = PriceGrid.PointToClient(Control.MousePosition);
            DataGrid.HitTestInfo hitTestInfo = dg.HitTest(p.X, p.Y);
            if (hitTestInfo.Type == DataGrid.HitTestType.Cell)
            {

                string FieldText = String.Empty;

                FieldText = dg.CurrentTableStyle.GridColumnStyles[hitTestInfo.Column].HeaderText;
                int RowText = hitTestInfo.Row;
                if (pnlGeneralFields.Visible)
                {
                    dg.DoDragDrop(new DropField(FieldText, RowText), DragDropEffects.Copy | DragDropEffects.Move);
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
                    dg.DoDragDrop(new DropField(beginText, endText, RowText), DragDropEffects.Copy | DragDropEffects.Move);
                }
            }
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

        private void CostsDataGrid_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            Point p = CostsDataGrid.PointToClient(new Point(e.X, e.Y));
            DataGrid.HitTestInfo costsHitTestInfo = CostsDataGrid.HitTest(p.X, p.Y);
            if (costsHitTestInfo.Type == DataGrid.HitTestType.Cell)
            {
                if (costsHitTestInfo.Row > -1)
                {
                    bool ed;
                    CurrencyManager cm = (CurrencyManager)BindingContext[CostsDataGrid.DataSource, CostsDataGrid.DataMember];
                    DataRowView drv = (DataRowView)cm.Current;
                    if (pnlGeneralFields.Visible)
                    {
                        /*
private void EditValue()
{ 
   int rowtoedit = 1;
   CurrencyManager myCurrencyManager = 
   (CurrencyManager)this.BindingContext[ds.Tables["Suppliers"]];
   myCurrencyManager.Position=rowtoedit;
   DataGridColumnStyle dgc = dataGrid1.TableStyles[0].GridColumnStyles[0];
   dataGrid1.BeginEdit(dgc, rowtoedit);
   // Insert code to edit the value.
   dataGrid1.EndEdit(dgc, rowtoedit, false);
}                         * 
                         */


                        //ed = CostsDataGrid.BeginEdit(CostsDataGrid.CurrentTableStyle.GridColumnStyles[2], costsHitTestInfo.Row);
                        CostsDataGrid[costsHitTestInfo.Row, 2] = ((DropField)e.Data.GetData(typeof(DropField))).FieldName;
                        //ed = CostsDataGrid.EndEdit(CostsDataGrid.CurrentTableStyle.GridColumnStyles[2], costsHitTestInfo.Row, false);
                        //drv.BeginEdit();
                        //drv.Row[2] = ((DropField)e.Data.GetData(typeof(DropField))).FieldName;
                        //drv.EndEdit();
                    }
                    else
                    {
                        //ed = CostsDataGrid.BeginEdit(CostsDataGrid.CurrentTableStyle.GridColumnStyles[2], costsHitTestInfo.Row);
                        CostsDataGrid[costsHitTestInfo.Row, 2] = ((DropField)e.Data.GetData(typeof(DropField))).FieldBegin;
                        //ed = CostsDataGrid.EndEdit(CostsDataGrid.CurrentTableStyle.GridColumnStyles[2], costsHitTestInfo.Row, false);

                        //CostsDataGrid.BeginEdit(CostsDataGrid.CurrentTableStyle.GridColumnStyles[3], costsHitTestInfo.Row);
                        CostsDataGrid[costsHitTestInfo.Row, 3] = ((DropField)e.Data.GetData(typeof(DropField))).FieldEnd;
                        //CostsDataGrid.EndEdit(CostsDataGrid.CurrentTableStyle.GridColumnStyles[3], costsHitTestInfo.Row, false);

                        //drv.BeginEdit();
                        //drv.Row[2] = ((DropField)e.Data.GetData(typeof(DropField))).FieldBegin;
                        //drv.Row[3] = ((DropField)e.Data.GetData(typeof(DropField))).FieldEnd;
                        //drv.EndEdit();
                    }
                }
            }
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

        private void PriceGrid_Navigate(object sender, System.Windows.Forms.NavigateEventArgs ne)
        {
            CurrencyManager cm = (CurrencyManager)BindingContext[((INDataGrid.INDataGrid)sender).DataSource, ((INDataGrid.INDataGrid)sender).DataMember];
            string N = ((DataView)cm.List).Table.TableName;
            if (ne.Forward && ((N != dtClients.TableName) && (N != dtPrices.TableName)))
            {
                ((INDataGrid.INDataGrid)sender).NavigateBack();
            }
            else
            {
                PriceGrid.CaptionText = N;
                if (PriceGrid.CaptionText == dtClients.TableName)
                    grpbGeneral.Visible = false;
                else
                    grpbGeneral.Visible = true;
            }
        }

        private void lLblMaster_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(String.Format("mailto:{0}", lLblMaster.Text));
        }

        private void CostsDataGrid_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            Point p = CostsDataGrid.PointToClient(new Point(e.X, e.Y));
            System.Windows.Forms.DataGrid.HitTestInfo costsHitTestInfo = CostsDataGrid.HitTest(p.X, p.Y);
            if ((costsHitTestInfo.Type == System.Windows.Forms.DataGrid.HitTestType.Cell) && (e.Data.GetDataPresent(typeof(DropField))))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void PriceGrid_Click(object sender, System.EventArgs e)
        {
            //			CurrencyManager currencyManager = (CurrencyManager)BindingContext[PriceGrid.DataSource, PriceGrid.DataMember];
            //			DataRowView drv = (DataRowView)currencyManager.Current;
            //			DataView dv = (DataView)currencyManager.List;
            //
            //			if (((DataView)currencyManager.List).Table.TableName == dtClients.TableName)
            //			{
            //				DataRow[] drs = drv.Row.GetChildRows(dtClients.TableName + "-" + dtPrices.TableName);
            //				if(drs.Length > 0)
            //				{
            //					firstFind = true;
            //					tbControl.SelectedTab = tpPrice;
            //				}
            //				else
            //					tbControl.SelectedTab = tpFirms;
            //			}
        }

        private void CheckOnePrice(CurrencyManager currencyManager)
        {
            DataRowView drv = (currencyManager.Position > -1) ? (DataRowView)currencyManager.Current : null;
            DataView dv = (DataView)currencyManager.List;

            if (drv.Row.GetChildRows(dtClients.TableName + "-" + dtPrices.TableName).Length > 1)
                PriceGrid.NavigateTo(currencyManager.Position, dtClients.TableName + "-" + dtPrices.TableName);
            else
            {
                if (drv.Row.GetChildRows(dtClients.TableName + "-" + dtPrices.TableName).Length == 1)
                    tbControl.SelectedTab = tpPrice;
            }
        }

        private void PriceGrid_DoubleClick(object sender, System.EventArgs e)
        {
            Point p = PriceGrid.PointToClient(Control.MousePosition);
            Debug.WriteLine(String.Format("PriceGrid_DoubleClick : {0} {1}", Control.MousePosition, p));
            DataGrid.HitTestInfo costsHitTestInfo = PriceGrid.HitTest(p.X, p.Y);
            if (costsHitTestInfo.Type == DataGrid.HitTestType.Cell)
            {
                //if (costsHitTestInfo.Row > -1)
                CurrencyManager currencyManager = (CurrencyManager)BindingContext[PriceGrid.DataSource, PriceGrid.DataMember];
                DataRowView drv = (currencyManager.Position > -1) ? (DataRowView)currencyManager.Current : null;

                if (((DataView)currencyManager.List).Table.TableName == dtClients.TableName)
                {
                    DataRow[] drs = drv.Row.GetChildRows(dtClients.TableName + "-" + dtPrices.TableName);
                    if (drs.Length > 0)
                    {
                        firstFind = true;
                        tbControl.SelectedTab = tpPrice;
                    }
                    else
                        tbControl.SelectedTab = tpFirms;
                }
                else
                    if ((drv.Row.Table.TableName == dtPrices.TableName) && (drv != null))
                    {
                        if ((currencyManager.Position > -1))
                        {
                            if (((DataView)currencyManager.List).Table.TableName == dtPrices.TableName)
                            {
                                tbControl.SelectedTab = tpPrice;
                            }
                        }
                    }
            }
        }

        public delegate void TestAsync(OleDbDataAdapter da, DataTable dt, INDataGrid.INDataGrid dg);

        void TestD(OleDbDataAdapter da, DataTable dt, INDataGrid.INDataGrid dg)
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

        private void CreateThread(OleDbDataAdapter da, DataTable dt, INDataGrid.INDataGrid dg)
        {
            TestAsync ta = new TestAsync(TestD);
            Object state = new Object();

            try
            {
                Debug.WriteLine("CreateThread begin");

                DataTable temp = new DataTable();

                System.IAsyncResult ar = ta.BeginInvoke(da, dt, dg, null, state);
                while (!ar.IsCompleted)
                    Application.DoEvents();

                //				dt.BeginLoadData();
                //				try
                //				{
                //					dt.Clear();
                //					dt.Columns.Clear();
                //					foreach(DataColumn dc in temp.Columns)
                //					{
                //						dt.Columns.Add(dc.ColumnName, dc.DataType);
                //					}
                //					foreach(DataRow dr in temp.Rows)
                //					{
                //						dt.Rows.Add(dr.ItemArray);
                //					}
                //				}
                //				finally
                //				{
                //					dt.EndLoadData();
                //				}
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
                if (!(fmt0.Equals(cmbFormat.Text.ToLower())) || !(delimiter0.Equals(tbDevider.Text.ToLower())))
                    DoOpenPrice(openedPriceDR);
                foreach (INDataGrid.INDataGridTableStyle ts in tblstyles)
                {
                    ts.RowHeadersVisible = false;
                }
                //PriceDataTableStyle.RowHeadersVisible = false;

                //erP.Clear();
            }
            else
            {
                pnlFloat.Visible = true;
                pnlFloat.BringToFront();
                fmt0 = fmt;
                delimiter0 = delimiter;
            }
        }

        private void btnFloatPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            //Debug.WriteLine(String.Format("{0}, {1}, {2}, {3}", e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height));
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
                    //r = new Regex(@"((?<Day>\d*?)[\s\.\-\/])?(?<Month>[^\s\.\-\/]*?)[\s\.\-\/](?<Year>\d*)$"); 
                    r = new Regex(txtMask.Text);
                    if ((fmt.ToUpper() == "WIN") && (delimiter == String.Empty))
                    {
                        if ((txtExistBegin.Text != String.Empty) && (txtExistEnd.Text != String.Empty))
                        {
                            foreach (DataRow dr in dtMarking.Rows)
                            {
                                if ((dr["MBeginField"].ToString() == txtExistBegin.Text) && (dr["MEndField"].ToString() == txtExistEnd.Text))
                                    CheckErrors(r, dr["MNameField"].ToString(), dtPrice, PriceDataTableStyle);
                            }
                        }
                    }
                    else
                    {
                        if (txtExist.Text != String.Empty)
                        {
                            if (tcInnerSheets.SelectedIndex > 0)
                            {
                                CheckErrors(r, txtExist.Text, (DataTable)dtables[tcInnerSheets.SelectedIndex - 1], (INDataGrid.INDataGridTableStyle)tblstyles[tcInnerSheets.SelectedIndex - 1]);
                            }
                            else
                                CheckErrors(r, txtExist.Text, dtPrice, PriceDataTableStyle);
                        }
                    }
                }
                catch (Exception e)
                {
                    erP.SetIconAlignment(txtMask, ErrorIconAlignment.MiddleLeft);

                    erP.SetError(txtMask, "Неправильно задана маска!");
                    //MessageBox.Show(String.Format("Неправильно задана маска"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void CheckErrors(Regex r, string FieldNameToSearch, DataTable dt, INDataGrid.INDataGridTableStyle ts)
        {
            //			DataGrid g = ts.DataGrid;

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
                        ts.RowHeadersVisible = true;
                        dr.RowError = "Несоответствие маски";
                    }
                }
            }
            dt.EndLoadData();
        }

        private void PriceGrid_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.Clicks > 1))
                Debug.WriteLine(String.Format("PriceGrid_MouseDown : {0} {1}", e.X, e.Y));
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
                DataRow dr = dtPrice.Rows[PriceDataGrid.CurrentRowIndex];
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
                    CheckErrorsInAllNames(r, groups, FindNameColumn(), (DataTable)dtables[tcInnerSheets.SelectedIndex - 1], (INDataGrid.INDataGridTableStyle)tblstyles[tcInnerSheets.SelectedIndex - 1]);
                }
                else
                    CheckErrorsInAllNames(r, groups, FindNameColumn(), dtPrice, PriceDataTableStyle);
            }
            else
                MessageBox.Show(String.Format("Не указана маска разбора товара!"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private void CheckErrorsInAllNames(Regex r, string[] GroupsToFind, string ColumnNameToSearchIn, DataTable dt, INDataGrid.INDataGridTableStyle ts)
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
                                    ts.RowHeadersVisible = true;
                                    dr.RowError = "Несоответствие маски";
                                }
                            }
                        }
                    }
                    else
                    {
                        ts.RowHeadersVisible = true;
                        dr.RowError = "Несоответствие поля";
                    }
                }
            }
            dt.EndLoadData();
        }

        private void CommitAllEdit()
        {
            CurrencyManager cm = (CurrencyManager)BindingContext[CostsDataGrid.DataSource, CostsDataGrid.DataMember];
            cm.EndCurrentEdit();
            //TODO: Здесь надо правильно биндить
            cm = (CurrencyManager)BindingContext[CostsDataGrid.DataSource, "Поставщики.Поставщики-Прайсы.Прайсы-правила"];
            cm.EndCurrentEdit();
        }

        private void tsbCancel_Click(object sender, EventArgs e)
        {
            CommitAllEdit();
            dtSet.RejectChanges();
        }

        private void tbControl_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tpPrice)
            {
                CommitAllEdit();
                DataSet dsc = dtSet.GetChanges();
                if (dsc != null)
                    if (MessageBox.Show("Имееются не сохраненые данные. Сохранить?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        tsbApply_Click(null, null);
            }
        }

        private void tsbApply_Click(object sender, EventArgs e)
        {
            CommitAllEdit();
            DataSet chg = dtSet.GetChanges();
            if (chg != null)
            {
                if (MyCn.State == ConnectionState.Closed)
                    MyCn.Open();
                try
                {
                    MySqlTransaction tr = MyCn.BeginTransaction();
                    try
                    {
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
