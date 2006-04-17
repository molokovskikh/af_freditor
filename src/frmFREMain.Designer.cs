namespace FREditor
{

    partial class frmFREMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFREMain));
            this.tbControl = new System.Windows.Forms.TabControl();
            this.tpFirms = new System.Windows.Forms.TabPage();
            this.pnlGrid = new System.Windows.Forms.Panel();
            this.PriceGrid = new INDataGrid.INDataGrid();
            this.dtSet = new System.Data.DataSet();
            this.dtClients = new System.Data.DataTable();
            this.CCode = new System.Data.DataColumn();
            this.CShortName = new System.Data.DataColumn();
            this.CRegion = new System.Data.DataColumn();
            this.CFullName = new System.Data.DataColumn();
            this.dtPrices = new System.Data.DataTable();
            this.PPriceCode = new System.Data.DataColumn();
            this.PFirmCode = new System.Data.DataColumn();
            this.PFirmName = new System.Data.DataColumn();
            this.dtPricesCost = new System.Data.DataTable();
            this.PCPriceCode = new System.Data.DataColumn();
            this.PCBaseCost = new System.Data.DataColumn();
            this.PCCostCode = new System.Data.DataColumn();
            this.PCCostName = new System.Data.DataColumn();
            this.dtFormRules = new System.Data.DataTable();
            this.FRName = new System.Data.DataColumn();
            this.FRCurrency = new System.Data.DataColumn();
            this.FRFormat = new System.Data.DataColumn();
            this.FRPosNum = new System.Data.DataColumn();
            this.FRDelimiter = new System.Data.DataColumn();
            this.FRRules = new System.Data.DataColumn();
            this.FRSynonyms = new System.Data.DataColumn();
            this.FRManager = new System.Data.DataColumn();
            this.FRPriceCode = new System.Data.DataColumn();
            this.FRListName = new System.Data.DataColumn();
            this.FRStartLine = new System.Data.DataColumn();
            this.FRSelfAwaitPos = new System.Data.DataColumn();
            this.FRSelfJunkPos = new System.Data.DataColumn();
            this.FRNameMask = new System.Data.DataColumn();
            this.FRForbWords = new System.Data.DataColumn();
            this.FRTxtCodeBegin = new System.Data.DataColumn();
            this.FRTxtCodeEnd = new System.Data.DataColumn();
            this.FRTxtCodeCrBegin = new System.Data.DataColumn();
            this.FRTxtCodeCrEnd = new System.Data.DataColumn();
            this.FRTxtNameBegin = new System.Data.DataColumn();
            this.FRTxtNameEnd = new System.Data.DataColumn();
            this.FRTxtFirmCrBegin = new System.Data.DataColumn();
            this.FRTxtFirmCrEnd = new System.Data.DataColumn();
            this.FRTxtCountryCrBegin = new System.Data.DataColumn();
            this.FRTxtCountryCrEnd = new System.Data.DataColumn();
            this.FRTxtBaseCostBegin = new System.Data.DataColumn();
            this.FRTxtBaseCostEnd = new System.Data.DataColumn();
            this.FRTxtMinBoundCostBegin = new System.Data.DataColumn();
            this.FRTxtMinBoundCostEnd = new System.Data.DataColumn();
            this.FRTxtAsFactCostBegin = new System.Data.DataColumn();
            this.FRTxtAsFactCostEnd = new System.Data.DataColumn();
            this.FRTxt5DayCostBegin = new System.Data.DataColumn();
            this.FRTxt5DayCostEnd = new System.Data.DataColumn();
            this.FRTxt10DayCostBegin = new System.Data.DataColumn();
            this.FRTxt10DayCostEnd = new System.Data.DataColumn();
            this.FRTxt15DayCostBegin = new System.Data.DataColumn();
            this.FRTxt15DayCostEnd = new System.Data.DataColumn();
            this.FRTxt20DayCostBegin = new System.Data.DataColumn();
            this.FRTxt20DayCostEnd = new System.Data.DataColumn();
            this.FRTxt25DayCostBegin = new System.Data.DataColumn();
            this.FRTxt25DayCostEnd = new System.Data.DataColumn();
            this.FRTxt30DayCostBegin = new System.Data.DataColumn();
            this.FRTxt30DayCostEnd = new System.Data.DataColumn();
            this.FRTxt45DayCostBegin = new System.Data.DataColumn();
            this.FRTxt45DayCostEnd = new System.Data.DataColumn();
            this.FRTxtCurrencyBegin = new System.Data.DataColumn();
            this.FRTxtCurrencyEnd = new System.Data.DataColumn();
            this.FRTxtUnitBegin = new System.Data.DataColumn();
            this.FRTxtUnitEnd = new System.Data.DataColumn();
            this.FRTxtVolumeBegin = new System.Data.DataColumn();
            this.FRTxtVolumeEnd = new System.Data.DataColumn();
            this.FRTxtUpCostBegin = new System.Data.DataColumn();
            this.FRTxtUpCostEnd = new System.Data.DataColumn();
            this.FRTxtQuantityBegin = new System.Data.DataColumn();
            this.FRTxtQuantityEnd = new System.Data.DataColumn();
            this.FRTxtNoteBegin = new System.Data.DataColumn();
            this.FRTxtNoteEnd = new System.Data.DataColumn();
            this.FRTxtPeriodBegin = new System.Data.DataColumn();
            this.FRTxtPeriodEnd = new System.Data.DataColumn();
            this.FRTxtDocBegin = new System.Data.DataColumn();
            this.FRTxtDocEnd = new System.Data.DataColumn();
            this.FRTxtJunkBegin = new System.Data.DataColumn();
            this.FRTxtJunkEnd = new System.Data.DataColumn();
            this.FRTxtAwaitBegin = new System.Data.DataColumn();
            this.FRTxtAwaitEnd = new System.Data.DataColumn();
            this.FRTxtReservedBegin = new System.Data.DataColumn();
            this.FRTxtReservedEnd = new System.Data.DataColumn();
            this.FRFCode = new System.Data.DataColumn();
            this.FRFCodeCr = new System.Data.DataColumn();
            this.FRFName1 = new System.Data.DataColumn();
            this.FRFName2 = new System.Data.DataColumn();
            this.FRFName3 = new System.Data.DataColumn();
            this.FRFBaseCost = new System.Data.DataColumn();
            this.FRFCurrency = new System.Data.DataColumn();
            this.FRFUnit = new System.Data.DataColumn();
            this.FRFVolume = new System.Data.DataColumn();
            this.FRFQuantity = new System.Data.DataColumn();
            this.FRFNote = new System.Data.DataColumn();
            this.FRFPeriod = new System.Data.DataColumn();
            this.FRFDoc = new System.Data.DataColumn();
            this.FRFJunk = new System.Data.DataColumn();
            this.FRFAwait = new System.Data.DataColumn();
            this.FRFFirmCr = new System.Data.DataColumn();
            this.FRFMinBoundCost = new System.Data.DataColumn();
            this.FRPriceFile = new System.Data.DataColumn();
            this.FRMemo = new System.Data.DataColumn();
            this.dtPriceFMTs = new System.Data.DataTable();
            this.FMTFormat = new System.Data.DataColumn();
            this.dtCatalogCurrency = new System.Data.DataTable();
            this.CCCurrency = new System.Data.DataColumn();
            this.dtMarking = new System.Data.DataTable();
            this.MNameField = new System.Data.DataColumn();
            this.MBeginField = new System.Data.DataColumn();
            this.MEndField = new System.Data.DataColumn();
            this.dtCostsFormRules = new System.Data.DataTable();
            this.CFRCost_Code = new System.Data.DataColumn();
            this.CFRFieldName = new System.Data.DataColumn();
            this.CFRTextBegin = new System.Data.DataColumn();
            this.CFRTextEnd = new System.Data.DataColumn();
            this.CFRfr_if = new System.Data.DataColumn();
            this.CFRCostName = new System.Data.DataColumn();
            this.ClientsTableStyle = new INDataGrid.INDataGridTableStyle();
            this.CClientCodeTextBoxColumn = new INDataGrid.INDataGridColorTextBoxColumn();
            this.CClientNameTextBoxColumn = new INDataGrid.INDataGridColorTextBoxColumn();
            this.CRegionNameTextBoxColumn = new INDataGrid.INDataGridColorTextBoxColumn();
            this.PricesTableStyle = new INDataGrid.INDataGridTableStyle();
            this.PPriceCodeTextBoxColumn = new INDataGrid.INDataGridColorTextBoxColumn();
            this.PFirmNameTextBoxColumn = new INDataGrid.INDataGridColorTextBoxColumn();
            this.PricesCostTableStyle = new INDataGrid.INDataGridTableStyle();
            this.PCCostCodeTextBoxColumn = new INDataGrid.INDataGridColorTextBoxColumn();
            this.PCCostNameTextBoxColumn = new INDataGrid.INDataGridColorTextBoxColumn();
            this.PCBaseCostBoolColumn = new System.Windows.Forms.DataGridBoolColumn();
            this.tpPrice = new System.Windows.Forms.TabPage();
            this.pnlFloat = new System.Windows.Forms.Panel();
            this.grpbGeneral = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rtbArticle = new System.Windows.Forms.RichTextBox();
            this.lblArticle = new System.Windows.Forms.Label();
            this.lLblMaster = new System.Windows.Forms.LinkLabel();
            this.lblMaster = new System.Windows.Forms.Label();
            this.grpbParent = new System.Windows.Forms.GroupBox();
            this.lblSynonyms = new System.Windows.Forms.Label();
            this.lblRules = new System.Windows.Forms.Label();
            this.tbSynonyms = new System.Windows.Forms.TextBox();
            this.tbRules = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblDevider = new System.Windows.Forms.Label();
            this.tbDevider = new System.Windows.Forms.TextBox();
            this.lblPosition = new System.Windows.Forms.Label();
            this.tbPosition = new System.Windows.Forms.TextBox();
            this.lblFormat = new System.Windows.Forms.Label();
            this.cmbFormat = new System.Windows.Forms.ComboBox();
            this.lblMoney = new System.Windows.Forms.Label();
            this.cmbMoney = new System.Windows.Forms.ComboBox();
            this.lblPriceName = new System.Windows.Forms.Label();
            this.lblNameFirm = new System.Windows.Forms.Label();
            this.tcInnerTable = new System.Windows.Forms.TabControl();
            this.tbpTable = new System.Windows.Forms.TabPage();
            this.tcInnerSheets = new System.Windows.Forms.TabControl();
            this.tbpSheet1 = new System.Windows.Forms.TabPage();
            this.PriceDataGrid = new INDataGrid.INDataGrid();
            this.PriceDataTableStyle = new INDataGrid.INDataGridTableStyle();
            this.tbpMarking = new System.Windows.Forms.TabPage();
            this.MarkingDataGrid = new INDataGrid.INDataGrid();
            this.MarkingTableStyle = new INDataGrid.INDataGridTableStyle();
            this.btnFloatPanel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.CostsDataGrid = new INDataGrid.INDataGrid();
            this.CostsFormRulesTableStyle = new INDataGrid.INDataGridTableStyle();
            this.CFRCostCodeTextBoxColumn = new INDataGrid.INDataGridColorTextBoxColumn();
            this.CFRCostNameTextBoxColumn = new INDataGrid.INDataGridColorTextBoxColumn();
            this.grpbFields = new System.Windows.Forms.GroupBox();
            this.pnlTxtFields = new System.Windows.Forms.Panel();
            this.txtBoxAwaitEnd = new System.Windows.Forms.TextBox();
            this.txtBoxJunkEnd = new System.Windows.Forms.TextBox();
            this.txtBoxAwaitBegin = new System.Windows.Forms.TextBox();
            this.txtBoxJunkBegin = new System.Windows.Forms.TextBox();
            this.txtBoxMinBoundCostEnd = new System.Windows.Forms.TextBox();
            this.txtBoxMinBoundCostBegin = new System.Windows.Forms.TextBox();
            this.txtBoxCurrencyEnd = new System.Windows.Forms.TextBox();
            this.txtBoxDocEnd = new System.Windows.Forms.TextBox();
            this.txtBoxPeriodEnd = new System.Windows.Forms.TextBox();
            this.txtBoxNoteEnd = new System.Windows.Forms.TextBox();
            this.txtBoxCurrencyBegin = new System.Windows.Forms.TextBox();
            this.txtBoxDocBegin = new System.Windows.Forms.TextBox();
            this.txtBoxPeriodBegin = new System.Windows.Forms.TextBox();
            this.txtBoxNoteBegin = new System.Windows.Forms.TextBox();
            this.txtBoxQuantityEnd = new System.Windows.Forms.TextBox();
            this.txtBoxQuantityBegin = new System.Windows.Forms.TextBox();
            this.txtBoxVolumeEnd = new System.Windows.Forms.TextBox();
            this.txtBoxVolumeBegin = new System.Windows.Forms.TextBox();
            this.txtBoxUnitEnd = new System.Windows.Forms.TextBox();
            this.txtBoxUnitBegin = new System.Windows.Forms.TextBox();
            this.txtBoxFirmCrEnd = new System.Windows.Forms.TextBox();
            this.txtBoxFirmCrBegin = new System.Windows.Forms.TextBox();
            this.txtBoxName1End = new System.Windows.Forms.TextBox();
            this.txtBoxName1Begin = new System.Windows.Forms.TextBox();
            this.txtBoxCodeCrEnd = new System.Windows.Forms.TextBox();
            this.txtBoxCodeCrBegin = new System.Windows.Forms.TextBox();
            this.txtBoxCodeEnd = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.txtBoxCodeBegin = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.pnlGeneralFields = new System.Windows.Forms.Panel();
            this.txtBoxAwait = new System.Windows.Forms.TextBox();
            this.txtBoxJunk = new System.Windows.Forms.TextBox();
            this.txtBoxMinBoundCost = new System.Windows.Forms.TextBox();
            this.txtBoxCurrency = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtBoxDoc = new System.Windows.Forms.TextBox();
            this.txtBoxPeriod = new System.Windows.Forms.TextBox();
            this.txtBoxNote = new System.Windows.Forms.TextBox();
            this.txtBoxQuantity = new System.Windows.Forms.TextBox();
            this.txtBoxUnit = new System.Windows.Forms.TextBox();
            this.txtBoxVolume = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtBoxFirmCr = new System.Windows.Forms.TextBox();
            this.txtBoxName3 = new System.Windows.Forms.TextBox();
            this.txtBoxName2 = new System.Windows.Forms.TextBox();
            this.txtBoxName1 = new System.Windows.Forms.TextBox();
            this.txtBoxCodeCr = new System.Windows.Forms.TextBox();
            this.txtBoxCode = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grpbSettings = new System.Windows.Forms.GroupBox();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.btnEditMask = new System.Windows.Forms.Button();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.btnJunkCheck = new System.Windows.Forms.Button();
            this.btnAwaitCheck = new System.Windows.Forms.Button();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.txtBoxSheetName = new System.Windows.Forms.TextBox();
            this.txtBoxStartLine = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txtBoxSelfJunkPos = new System.Windows.Forms.TextBox();
            this.txtBoxSelfAwaitPos = new System.Windows.Forms.TextBox();
            this.txtBoxForbWords = new System.Windows.Forms.TextBox();
            this.txtBoxNameMask = new System.Windows.Forms.TextBox();
            this.erP = new System.Windows.Forms.ErrorProvider(this.components);
            this.tscMain = new System.Windows.Forms.ToolStripContainer();
            this.tsApply = new System.Windows.Forms.ToolStrip();
            this.tsbApply = new System.Windows.Forms.ToolStripButton();
            this.tsbCancel = new System.Windows.Forms.ToolStripButton();
            this.mcmdUCostRules = new MySql.Data.MySqlClient.MySqlCommand();
            this.daCostRules = new MySql.Data.MySqlClient.MySqlDataAdapter();
            this.mcmdUFormRules = new MySql.Data.MySqlClient.MySqlCommand();
            this.daFormRules = new MySql.Data.MySqlClient.MySqlDataAdapter();
            this.ttMain = new System.Windows.Forms.ToolTip(this.components);
            this.tbControl.SuspendLayout();
            this.tpFirms.SuspendLayout();
            this.pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PriceGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtClients)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPrices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPricesCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFormRules)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPriceFMTs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCatalogCurrency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtMarking)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCostsFormRules)).BeginInit();
            this.tpPrice.SuspendLayout();
            this.pnlFloat.SuspendLayout();
            this.grpbGeneral.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpbParent.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tcInnerTable.SuspendLayout();
            this.tbpTable.SuspendLayout();
            this.tcInnerSheets.SuspendLayout();
            this.tbpSheet1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PriceDataGrid)).BeginInit();
            this.tbpMarking.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MarkingDataGrid)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CostsDataGrid)).BeginInit();
            this.grpbFields.SuspendLayout();
            this.pnlTxtFields.SuspendLayout();
            this.pnlGeneralFields.SuspendLayout();
            this.grpbSettings.SuspendLayout();
            this.pnlSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.erP)).BeginInit();
            this.tscMain.ContentPanel.SuspendLayout();
            this.tscMain.TopToolStripPanel.SuspendLayout();
            this.tscMain.SuspendLayout();
            this.tsApply.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbControl
            // 
            this.tbControl.Controls.Add(this.tpFirms);
            this.tbControl.Controls.Add(this.tpPrice);
            this.tbControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbControl.Location = new System.Drawing.Point(0, 0);
            this.tbControl.Name = "tbControl";
            this.tbControl.SelectedIndex = 0;
            this.tbControl.Size = new System.Drawing.Size(864, 728);
            this.tbControl.TabIndex = 1;
            this.tbControl.Deselecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tbControl_Deselecting);
            this.tbControl.SelectedIndexChanged += new System.EventHandler(this.tbControl_SelectedIndexChanged);
            // 
            // tpFirms
            // 
            this.tpFirms.Controls.Add(this.pnlGrid);
            this.tpFirms.Location = new System.Drawing.Point(4, 22);
            this.tpFirms.Name = "tpFirms";
            this.tpFirms.Size = new System.Drawing.Size(856, 702);
            this.tpFirms.TabIndex = 0;
            this.tpFirms.Text = "Фирмы";
            // 
            // pnlGrid
            // 
            this.pnlGrid.Controls.Add(this.PriceGrid);
            this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrid.Location = new System.Drawing.Point(0, 0);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.Size = new System.Drawing.Size(856, 702);
            this.pnlGrid.TabIndex = 5;
            // 
            // PriceGrid
            // 
            this.PriceGrid.AllowDrop = true;
            this.PriceGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.PriceGrid.CaptionText = "Поставщики";
            this.PriceGrid.DataMember = "Поставщики";
            this.PriceGrid.DataSource = this.dtSet;
            this.PriceGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PriceGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.PriceGrid.Location = new System.Drawing.Point(0, 0);
            this.PriceGrid.Name = "PriceGrid";
            this.PriceGrid.ReadOnly = true;
            this.PriceGrid.Size = new System.Drawing.Size(856, 702);
            this.PriceGrid.TabIndex = 0;
            this.PriceGrid.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
            this.ClientsTableStyle,
            this.PricesTableStyle,
            this.PricesCostTableStyle});
            this.PriceGrid.ButtonPress += new INDataGrid.INDataGridKeyPressEventHandler(this.PriceGrid_ButtonPress);
            this.PriceGrid.DoubleClick += new System.EventHandler(this.PriceGrid_DoubleClick);
            this.PriceGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PriceGrid_MouseDown);
            this.PriceGrid.Navigate += new System.Windows.Forms.NavigateEventHandler(this.PriceGrid_Navigate);
            this.PriceGrid.Click += new System.EventHandler(this.PriceGrid_Click);
            // 
            // dtSet
            // 
            this.dtSet.DataSetName = "NewDataSet";
            this.dtSet.Locale = new System.Globalization.CultureInfo("ru-RU");
            this.dtSet.Relations.AddRange(new System.Data.DataRelation[] {
            new System.Data.DataRelation("Прайсы-Цены", "Прайсы", "Цены", new string[] {
                        "PPriceCode"}, new string[] {
                        "PCPriceCode"}, false),
            new System.Data.DataRelation("Поставщики-Прайсы", "Поставщики", "Прайсы", new string[] {
                        "CCode"}, new string[] {
                        "PFirmCode"}, false),
            new System.Data.DataRelation("Прайсы-правила", "Прайсы", "Правила формализации", new string[] {
                        "PPriceCode"}, new string[] {
                        "FRPriceCode"}, false),
            new System.Data.DataRelation("Прайсы-Правила формализации цен", "Прайсы", "Правила формализации цен", new string[] {
                        "PPriceCode"}, new string[] {
                        "CFRfr_if"}, false)});
            this.dtSet.Tables.AddRange(new System.Data.DataTable[] {
            this.dtClients,
            this.dtPrices,
            this.dtPricesCost,
            this.dtFormRules,
            this.dtPriceFMTs,
            this.dtCatalogCurrency,
            this.dtMarking,
            this.dtCostsFormRules});
            // 
            // dtClients
            // 
            this.dtClients.Columns.AddRange(new System.Data.DataColumn[] {
            this.CCode,
            this.CShortName,
            this.CRegion,
            this.CFullName});
            this.dtClients.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "CCode"}, true)});
            this.dtClients.PrimaryKey = new System.Data.DataColumn[] {
        this.CCode};
            this.dtClients.TableName = "Поставщики";
            // 
            // CCode
            // 
            this.CCode.AllowDBNull = false;
            this.CCode.Caption = "CCode";
            this.CCode.ColumnName = "CCode";
            this.CCode.DataType = typeof(long);
            // 
            // CShortName
            // 
            this.CShortName.ColumnName = "CShortName";
            // 
            // CRegion
            // 
            this.CRegion.ColumnName = "CRegion";
            // 
            // CFullName
            // 
            this.CFullName.ColumnName = "CFullName";
            // 
            // dtPrices
            // 
            this.dtPrices.Columns.AddRange(new System.Data.DataColumn[] {
            this.PPriceCode,
            this.PFirmCode,
            this.PFirmName});
            this.dtPrices.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "PPriceCode"}, true),
            new System.Data.ForeignKeyConstraint("Relation1", "Поставщики", new string[] {
                        "CCode"}, new string[] {
                        "PFirmCode"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade)});
            this.dtPrices.PrimaryKey = new System.Data.DataColumn[] {
        this.PPriceCode};
            this.dtPrices.TableName = "Прайсы";
            // 
            // PPriceCode
            // 
            this.PPriceCode.AllowDBNull = false;
            this.PPriceCode.ColumnName = "PPriceCode";
            this.PPriceCode.DataType = typeof(long);
            // 
            // PFirmCode
            // 
            this.PFirmCode.ColumnName = "PFirmCode";
            this.PFirmCode.DataType = typeof(long);
            // 
            // PFirmName
            // 
            this.PFirmName.ColumnName = "PFirmName";
            // 
            // dtPricesCost
            // 
            this.dtPricesCost.Columns.AddRange(new System.Data.DataColumn[] {
            this.PCPriceCode,
            this.PCBaseCost,
            this.PCCostCode,
            this.PCCostName});
            this.dtPricesCost.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "PCCostCode"}, true),
            new System.Data.ForeignKeyConstraint("Relation2", "Прайсы", new string[] {
                        "PPriceCode"}, new string[] {
                        "PCPriceCode"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade)});
            this.dtPricesCost.PrimaryKey = new System.Data.DataColumn[] {
        this.PCCostCode};
            this.dtPricesCost.TableName = "Цены";
            // 
            // PCPriceCode
            // 
            this.PCPriceCode.ColumnName = "PCPriceCode";
            this.PCPriceCode.DataType = typeof(long);
            // 
            // PCBaseCost
            // 
            this.PCBaseCost.ColumnName = "PCBaseCost";
            // 
            // PCCostCode
            // 
            this.PCCostCode.AllowDBNull = false;
            this.PCCostCode.ColumnName = "PCCostCode";
            this.PCCostCode.DataType = typeof(long);
            // 
            // PCCostName
            // 
            this.PCCostName.ColumnName = "PCCostName";
            // 
            // dtFormRules
            // 
            this.dtFormRules.Columns.AddRange(new System.Data.DataColumn[] {
            this.FRName,
            this.FRCurrency,
            this.FRFormat,
            this.FRPosNum,
            this.FRDelimiter,
            this.FRRules,
            this.FRSynonyms,
            this.FRManager,
            this.FRPriceCode,
            this.FRListName,
            this.FRStartLine,
            this.FRSelfAwaitPos,
            this.FRSelfJunkPos,
            this.FRNameMask,
            this.FRForbWords,
            this.FRTxtCodeBegin,
            this.FRTxtCodeEnd,
            this.FRTxtCodeCrBegin,
            this.FRTxtCodeCrEnd,
            this.FRTxtNameBegin,
            this.FRTxtNameEnd,
            this.FRTxtFirmCrBegin,
            this.FRTxtFirmCrEnd,
            this.FRTxtCountryCrBegin,
            this.FRTxtCountryCrEnd,
            this.FRTxtBaseCostBegin,
            this.FRTxtBaseCostEnd,
            this.FRTxtMinBoundCostBegin,
            this.FRTxtMinBoundCostEnd,
            this.FRTxtAsFactCostBegin,
            this.FRTxtAsFactCostEnd,
            this.FRTxt5DayCostBegin,
            this.FRTxt5DayCostEnd,
            this.FRTxt10DayCostBegin,
            this.FRTxt10DayCostEnd,
            this.FRTxt15DayCostBegin,
            this.FRTxt15DayCostEnd,
            this.FRTxt20DayCostBegin,
            this.FRTxt20DayCostEnd,
            this.FRTxt25DayCostBegin,
            this.FRTxt25DayCostEnd,
            this.FRTxt30DayCostBegin,
            this.FRTxt30DayCostEnd,
            this.FRTxt45DayCostBegin,
            this.FRTxt45DayCostEnd,
            this.FRTxtCurrencyBegin,
            this.FRTxtCurrencyEnd,
            this.FRTxtUnitBegin,
            this.FRTxtUnitEnd,
            this.FRTxtVolumeBegin,
            this.FRTxtVolumeEnd,
            this.FRTxtUpCostBegin,
            this.FRTxtUpCostEnd,
            this.FRTxtQuantityBegin,
            this.FRTxtQuantityEnd,
            this.FRTxtNoteBegin,
            this.FRTxtNoteEnd,
            this.FRTxtPeriodBegin,
            this.FRTxtPeriodEnd,
            this.FRTxtDocBegin,
            this.FRTxtDocEnd,
            this.FRTxtJunkBegin,
            this.FRTxtJunkEnd,
            this.FRTxtAwaitBegin,
            this.FRTxtAwaitEnd,
            this.FRTxtReservedBegin,
            this.FRTxtReservedEnd,
            this.FRFCode,
            this.FRFCodeCr,
            this.FRFName1,
            this.FRFName2,
            this.FRFName3,
            this.FRFBaseCost,
            this.FRFCurrency,
            this.FRFUnit,
            this.FRFVolume,
            this.FRFQuantity,
            this.FRFNote,
            this.FRFPeriod,
            this.FRFDoc,
            this.FRFJunk,
            this.FRFAwait,
            this.FRFFirmCr,
            this.FRFMinBoundCost,
            this.FRPriceFile,
            this.FRMemo});
            this.dtFormRules.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.ForeignKeyConstraint("Relation1", "Прайсы", new string[] {
                        "PPriceCode"}, new string[] {
                        "FRPriceCode"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade)});
            this.dtFormRules.TableName = "Правила формализации";
            // 
            // FRName
            // 
            this.FRName.ColumnName = "FRName";
            // 
            // FRCurrency
            // 
            this.FRCurrency.ColumnName = "FRCurrency";
            // 
            // FRFormat
            // 
            this.FRFormat.ColumnName = "FRFormat";
            // 
            // FRPosNum
            // 
            this.FRPosNum.ColumnName = "FRPosNum";
            this.FRPosNum.DataType = typeof(long);
            // 
            // FRDelimiter
            // 
            this.FRDelimiter.ColumnName = "FRDelimiter";
            // 
            // FRRules
            // 
            this.FRRules.ColumnName = "FRRules";
            // 
            // FRSynonyms
            // 
            this.FRSynonyms.ColumnName = "FRSynonyms";
            // 
            // FRManager
            // 
            this.FRManager.ColumnName = "FRManager";
            // 
            // FRPriceCode
            // 
            this.FRPriceCode.ColumnName = "FRPriceCode";
            this.FRPriceCode.DataType = typeof(long);
            // 
            // FRListName
            // 
            this.FRListName.ColumnName = "FRListName";
            // 
            // FRStartLine
            // 
            this.FRStartLine.ColumnName = "FRStartLine";
            // 
            // FRSelfAwaitPos
            // 
            this.FRSelfAwaitPos.ColumnName = "FRSelfAwaitPos";
            // 
            // FRSelfJunkPos
            // 
            this.FRSelfJunkPos.ColumnName = "FRSelfJunkPos";
            // 
            // FRNameMask
            // 
            this.FRNameMask.ColumnName = "FRNameMask";
            // 
            // FRForbWords
            // 
            this.FRForbWords.ColumnName = "FRForbWords";
            // 
            // FRTxtCodeBegin
            // 
            this.FRTxtCodeBegin.ColumnName = "FRTxtCodeBegin";
            // 
            // FRTxtCodeEnd
            // 
            this.FRTxtCodeEnd.ColumnName = "FRTxtCodeEnd";
            // 
            // FRTxtCodeCrBegin
            // 
            this.FRTxtCodeCrBegin.ColumnName = "FRTxtCodeCrBegin";
            // 
            // FRTxtCodeCrEnd
            // 
            this.FRTxtCodeCrEnd.ColumnName = "FRTxtCodeCrEnd";
            // 
            // FRTxtNameBegin
            // 
            this.FRTxtNameBegin.ColumnName = "FRTxtNameBegin";
            // 
            // FRTxtNameEnd
            // 
            this.FRTxtNameEnd.ColumnName = "FRTxtNameEnd";
            // 
            // FRTxtFirmCrBegin
            // 
            this.FRTxtFirmCrBegin.ColumnName = "FRTxtFirmCrBegin";
            // 
            // FRTxtFirmCrEnd
            // 
            this.FRTxtFirmCrEnd.ColumnName = "FRTxtFirmCrEnd";
            // 
            // FRTxtCountryCrBegin
            // 
            this.FRTxtCountryCrBegin.ColumnName = "FRTxtCountryCrBegin";
            // 
            // FRTxtCountryCrEnd
            // 
            this.FRTxtCountryCrEnd.ColumnName = "FRTxtCountryCrEnd";
            // 
            // FRTxtBaseCostBegin
            // 
            this.FRTxtBaseCostBegin.ColumnName = "FRTxtBaseCostBegin";
            // 
            // FRTxtBaseCostEnd
            // 
            this.FRTxtBaseCostEnd.ColumnName = "FRTxtBaseCostEnd";
            // 
            // FRTxtMinBoundCostBegin
            // 
            this.FRTxtMinBoundCostBegin.ColumnName = "FRTxtMinBoundCostBegin";
            // 
            // FRTxtMinBoundCostEnd
            // 
            this.FRTxtMinBoundCostEnd.ColumnName = "FRTxtMinBoundCostEnd";
            // 
            // FRTxtAsFactCostBegin
            // 
            this.FRTxtAsFactCostBegin.ColumnName = "FRTxtAsFactCostBegin";
            // 
            // FRTxtAsFactCostEnd
            // 
            this.FRTxtAsFactCostEnd.ColumnName = "FRTxtAsFactCostEnd";
            // 
            // FRTxt5DayCostBegin
            // 
            this.FRTxt5DayCostBegin.ColumnName = "FRTxt5DayCostBegin";
            // 
            // FRTxt5DayCostEnd
            // 
            this.FRTxt5DayCostEnd.ColumnName = "FRTxt5DayCostEnd";
            // 
            // FRTxt10DayCostBegin
            // 
            this.FRTxt10DayCostBegin.ColumnName = "FRTxt10DayCostBegin";
            // 
            // FRTxt10DayCostEnd
            // 
            this.FRTxt10DayCostEnd.ColumnName = "FRTxt10DayCostEnd";
            // 
            // FRTxt15DayCostBegin
            // 
            this.FRTxt15DayCostBegin.ColumnName = "FRTxt15DayCostBegin";
            // 
            // FRTxt15DayCostEnd
            // 
            this.FRTxt15DayCostEnd.ColumnName = "FRTxt15DayCostEnd";
            // 
            // FRTxt20DayCostBegin
            // 
            this.FRTxt20DayCostBegin.ColumnName = "FRTxt20DayCostBegin";
            // 
            // FRTxt20DayCostEnd
            // 
            this.FRTxt20DayCostEnd.ColumnName = "FRTxt20DayCostEnd";
            // 
            // FRTxt25DayCostBegin
            // 
            this.FRTxt25DayCostBegin.ColumnName = "FRTxt25DayCostBegin";
            // 
            // FRTxt25DayCostEnd
            // 
            this.FRTxt25DayCostEnd.ColumnName = "FRTxt25DayCostEnd";
            // 
            // FRTxt30DayCostBegin
            // 
            this.FRTxt30DayCostBegin.ColumnName = "FRTxt30DayCostBegin";
            // 
            // FRTxt30DayCostEnd
            // 
            this.FRTxt30DayCostEnd.ColumnName = "FRTxt30DayCostEnd";
            // 
            // FRTxt45DayCostBegin
            // 
            this.FRTxt45DayCostBegin.ColumnName = "FRTxt45DayCostBegin";
            // 
            // FRTxt45DayCostEnd
            // 
            this.FRTxt45DayCostEnd.ColumnName = "FRTxt45DayCostEnd";
            // 
            // FRTxtCurrencyBegin
            // 
            this.FRTxtCurrencyBegin.ColumnName = "FRTxtCurrencyBegin";
            // 
            // FRTxtCurrencyEnd
            // 
            this.FRTxtCurrencyEnd.ColumnName = "FRTxtCurrencyEnd";
            // 
            // FRTxtUnitBegin
            // 
            this.FRTxtUnitBegin.ColumnName = "FRTxtUnitBegin";
            // 
            // FRTxtUnitEnd
            // 
            this.FRTxtUnitEnd.ColumnName = "FRTxtUnitEnd";
            // 
            // FRTxtVolumeBegin
            // 
            this.FRTxtVolumeBegin.ColumnName = "FRTxtVolumeBegin";
            // 
            // FRTxtVolumeEnd
            // 
            this.FRTxtVolumeEnd.ColumnName = "FRTxtVolumeEnd";
            // 
            // FRTxtUpCostBegin
            // 
            this.FRTxtUpCostBegin.ColumnName = "FRTxtUpCostBegin";
            // 
            // FRTxtUpCostEnd
            // 
            this.FRTxtUpCostEnd.ColumnName = "FRTxtUpCostEnd";
            // 
            // FRTxtQuantityBegin
            // 
            this.FRTxtQuantityBegin.ColumnName = "FRTxtQuantityBegin";
            // 
            // FRTxtQuantityEnd
            // 
            this.FRTxtQuantityEnd.ColumnName = "FRTxtQuantityEnd";
            // 
            // FRTxtNoteBegin
            // 
            this.FRTxtNoteBegin.ColumnName = "FRTxtNoteBegin";
            // 
            // FRTxtNoteEnd
            // 
            this.FRTxtNoteEnd.ColumnName = "FRTxtNoteEnd";
            // 
            // FRTxtPeriodBegin
            // 
            this.FRTxtPeriodBegin.ColumnName = "FRTxtPeriodBegin";
            // 
            // FRTxtPeriodEnd
            // 
            this.FRTxtPeriodEnd.ColumnName = "FRTxtPeriodEnd";
            // 
            // FRTxtDocBegin
            // 
            this.FRTxtDocBegin.ColumnName = "FRTxtDocBegin";
            // 
            // FRTxtDocEnd
            // 
            this.FRTxtDocEnd.ColumnName = "FRTxtDocEnd";
            // 
            // FRTxtJunkBegin
            // 
            this.FRTxtJunkBegin.ColumnName = "FRTxtJunkBegin";
            // 
            // FRTxtJunkEnd
            // 
            this.FRTxtJunkEnd.ColumnName = "FRTxtJunkEnd";
            // 
            // FRTxtAwaitBegin
            // 
            this.FRTxtAwaitBegin.ColumnName = "FRTxtAwaitBegin";
            // 
            // FRTxtAwaitEnd
            // 
            this.FRTxtAwaitEnd.ColumnName = "FRTxtAwaitEnd";
            // 
            // FRTxtReservedBegin
            // 
            this.FRTxtReservedBegin.ColumnName = "FRTxtReservedBegin";
            // 
            // FRTxtReservedEnd
            // 
            this.FRTxtReservedEnd.ColumnName = "FRTxtReservedEnd";
            // 
            // FRFCode
            // 
            this.FRFCode.ColumnName = "FRFCode";
            // 
            // FRFCodeCr
            // 
            this.FRFCodeCr.ColumnName = "FRFCodeCr";
            // 
            // FRFName1
            // 
            this.FRFName1.ColumnName = "FRFName1";
            // 
            // FRFName2
            // 
            this.FRFName2.ColumnName = "FRFName2";
            // 
            // FRFName3
            // 
            this.FRFName3.ColumnName = "FRFName3";
            // 
            // FRFBaseCost
            // 
            this.FRFBaseCost.ColumnName = "FRFBaseCost";
            // 
            // FRFCurrency
            // 
            this.FRFCurrency.ColumnName = "FRFCurrency";
            // 
            // FRFUnit
            // 
            this.FRFUnit.ColumnName = "FRFUnit";
            // 
            // FRFVolume
            // 
            this.FRFVolume.ColumnName = "FRFVolume";
            // 
            // FRFQuantity
            // 
            this.FRFQuantity.ColumnName = "FRFQuantity";
            // 
            // FRFNote
            // 
            this.FRFNote.ColumnName = "FRFNote";
            // 
            // FRFPeriod
            // 
            this.FRFPeriod.ColumnName = "FRFPeriod";
            // 
            // FRFDoc
            // 
            this.FRFDoc.ColumnName = "FRFDoc";
            // 
            // FRFJunk
            // 
            this.FRFJunk.ColumnName = "FRFJunk";
            // 
            // FRFAwait
            // 
            this.FRFAwait.ColumnName = "FRFAwait";
            // 
            // FRFFirmCr
            // 
            this.FRFFirmCr.ColumnName = "FRFFirmCr";
            // 
            // FRFMinBoundCost
            // 
            this.FRFMinBoundCost.ColumnName = "FRFMinBoundCost";
            // 
            // FRPriceFile
            // 
            this.FRPriceFile.ColumnName = "FRPriceFile";
            // 
            // FRMemo
            // 
            this.FRMemo.ColumnName = "FRMemo";
            // 
            // dtPriceFMTs
            // 
            this.dtPriceFMTs.Columns.AddRange(new System.Data.DataColumn[] {
            this.FMTFormat});
            this.dtPriceFMTs.TableName = "Форматы прайса";
            // 
            // FMTFormat
            // 
            this.FMTFormat.ColumnName = "FMTFormat";
            // 
            // dtCatalogCurrency
            // 
            this.dtCatalogCurrency.Columns.AddRange(new System.Data.DataColumn[] {
            this.CCCurrency});
            this.dtCatalogCurrency.TableName = "Каталог валют";
            // 
            // CCCurrency
            // 
            this.CCCurrency.ColumnName = "CCCurrency";
            // 
            // dtMarking
            // 
            this.dtMarking.Columns.AddRange(new System.Data.DataColumn[] {
            this.MNameField,
            this.MBeginField,
            this.MEndField});
            this.dtMarking.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "MNameField"}, false)});
            this.dtMarking.TableName = "Разметка";
            // 
            // MNameField
            // 
            this.MNameField.ColumnName = "MNameField";
            // 
            // MBeginField
            // 
            this.MBeginField.ColumnName = "MBeginField";
            // 
            // MEndField
            // 
            this.MEndField.ColumnName = "MEndField";
            // 
            // dtCostsFormRules
            // 
            this.dtCostsFormRules.Columns.AddRange(new System.Data.DataColumn[] {
            this.CFRCost_Code,
            this.CFRFieldName,
            this.CFRTextBegin,
            this.CFRTextEnd,
            this.CFRfr_if,
            this.CFRCostName});
            this.dtCostsFormRules.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.ForeignKeyConstraint("Прайсы-Правила формализации цен", "Прайсы", new string[] {
                        "PPriceCode"}, new string[] {
                        "CFRfr_if"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade)});
            this.dtCostsFormRules.TableName = "Правила формализации цен";
            // 
            // CFRCost_Code
            // 
            this.CFRCost_Code.ColumnName = "CFRCost_Code";
            this.CFRCost_Code.DataType = typeof(long);
            // 
            // CFRFieldName
            // 
            this.CFRFieldName.ColumnName = "CFRFieldName";
            // 
            // CFRTextBegin
            // 
            this.CFRTextBegin.ColumnName = "CFRTextBegin";
            // 
            // CFRTextEnd
            // 
            this.CFRTextEnd.ColumnName = "CFRTextEnd";
            // 
            // CFRfr_if
            // 
            this.CFRfr_if.ColumnName = "CFRfr_if";
            this.CFRfr_if.DataType = typeof(long);
            // 
            // CFRCostName
            // 
            this.CFRCostName.ColumnName = "CFRCostName";
            // 
            // ClientsTableStyle
            // 
            this.ClientsTableStyle.ColumnSizeAutoFit = true;
            this.ClientsTableStyle.DataGrid = this.PriceGrid;
            this.ClientsTableStyle.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
            this.CClientCodeTextBoxColumn,
            this.CClientNameTextBoxColumn,
            this.CRegionNameTextBoxColumn});
            this.ClientsTableStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.ClientsTableStyle.MappingName = "Поставщики";
            this.ClientsTableStyle.ReadOnly = true;
            this.ClientsTableStyle.RowHeadersVisible = false;
            this.ClientsTableStyle.RowSelected = true;
            // 
            // CClientCodeTextBoxColumn
            // 
            this.CClientCodeTextBoxColumn.EditDisable = true;
            this.CClientCodeTextBoxColumn.Format = "";
            this.CClientCodeTextBoxColumn.FormatInfo = null;
            this.CClientCodeTextBoxColumn.HeaderText = "Код";
            this.CClientCodeTextBoxColumn.MappingName = "CCode";
            this.CClientCodeTextBoxColumn.NullText = "";
            this.CClientCodeTextBoxColumn.ReadOnly = true;
            this.CClientCodeTextBoxColumn.SearchColumn = true;
            this.CClientCodeTextBoxColumn.Width = 284;
            // 
            // CClientNameTextBoxColumn
            // 
            this.CClientNameTextBoxColumn.EditDisable = true;
            this.CClientNameTextBoxColumn.Format = "";
            this.CClientNameTextBoxColumn.FormatInfo = null;
            this.CClientNameTextBoxColumn.HeaderText = "Наименование";
            this.CClientNameTextBoxColumn.MappingName = "CShortName";
            this.CClientNameTextBoxColumn.NullText = "";
            this.CClientNameTextBoxColumn.ReadOnly = true;
            this.CClientNameTextBoxColumn.SearchColumn = true;
            this.CClientNameTextBoxColumn.Width = 284;
            // 
            // CRegionNameTextBoxColumn
            // 
            this.CRegionNameTextBoxColumn.EditDisable = true;
            this.CRegionNameTextBoxColumn.Format = "";
            this.CRegionNameTextBoxColumn.FormatInfo = null;
            this.CRegionNameTextBoxColumn.HeaderText = "Регион";
            this.CRegionNameTextBoxColumn.MappingName = "CRegion";
            this.CRegionNameTextBoxColumn.NullText = "";
            this.CRegionNameTextBoxColumn.ReadOnly = true;
            this.CRegionNameTextBoxColumn.SearchColumn = false;
            this.CRegionNameTextBoxColumn.Width = 284;
            // 
            // PricesTableStyle
            // 
            this.PricesTableStyle.ColumnSizeAutoFit = true;
            this.PricesTableStyle.DataGrid = this.PriceGrid;
            this.PricesTableStyle.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
            this.PPriceCodeTextBoxColumn,
            this.PFirmNameTextBoxColumn});
            this.PricesTableStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.PricesTableStyle.MappingName = "Прайсы";
            this.PricesTableStyle.ReadOnly = true;
            this.PricesTableStyle.RowHeadersVisible = false;
            this.PricesTableStyle.RowSelected = true;
            // 
            // PPriceCodeTextBoxColumn
            // 
            this.PPriceCodeTextBoxColumn.EditDisable = true;
            this.PPriceCodeTextBoxColumn.Format = "";
            this.PPriceCodeTextBoxColumn.FormatInfo = null;
            this.PPriceCodeTextBoxColumn.HeaderText = "Код прайс - листа";
            this.PPriceCodeTextBoxColumn.MappingName = "PPriceCode";
            this.PPriceCodeTextBoxColumn.NullText = "";
            this.PPriceCodeTextBoxColumn.SearchColumn = true;
            this.PPriceCodeTextBoxColumn.Width = 426;
            // 
            // PFirmNameTextBoxColumn
            // 
            this.PFirmNameTextBoxColumn.EditDisable = true;
            this.PFirmNameTextBoxColumn.Format = "";
            this.PFirmNameTextBoxColumn.FormatInfo = null;
            this.PFirmNameTextBoxColumn.HeaderText = "Наименование";
            this.PFirmNameTextBoxColumn.MappingName = "PFirmName";
            this.PFirmNameTextBoxColumn.NullText = "";
            this.PFirmNameTextBoxColumn.SearchColumn = false;
            this.PFirmNameTextBoxColumn.Width = 426;
            // 
            // PricesCostTableStyle
            // 
            this.PricesCostTableStyle.ColumnSizeAutoFit = true;
            this.PricesCostTableStyle.DataGrid = this.PriceGrid;
            this.PricesCostTableStyle.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
            this.PCCostCodeTextBoxColumn,
            this.PCCostNameTextBoxColumn,
            this.PCBaseCostBoolColumn});
            this.PricesCostTableStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.PricesCostTableStyle.MappingName = "Цены";
            this.PricesCostTableStyle.ReadOnly = true;
            this.PricesCostTableStyle.RowHeadersVisible = false;
            this.PricesCostTableStyle.RowSelected = true;
            // 
            // PCCostCodeTextBoxColumn
            // 
            this.PCCostCodeTextBoxColumn.EditDisable = true;
            this.PCCostCodeTextBoxColumn.Format = "";
            this.PCCostCodeTextBoxColumn.FormatInfo = null;
            this.PCCostCodeTextBoxColumn.HeaderText = "Код";
            this.PCCostCodeTextBoxColumn.MappingName = "PCCostCode";
            this.PCCostCodeTextBoxColumn.NullText = "";
            this.PCCostCodeTextBoxColumn.SearchColumn = false;
            this.PCCostCodeTextBoxColumn.Width = 284;
            // 
            // PCCostNameTextBoxColumn
            // 
            this.PCCostNameTextBoxColumn.EditDisable = true;
            this.PCCostNameTextBoxColumn.Format = "";
            this.PCCostNameTextBoxColumn.FormatInfo = null;
            this.PCCostNameTextBoxColumn.MappingName = "PCCostName";
            this.PCCostNameTextBoxColumn.NullText = "Наименование";
            this.PCCostNameTextBoxColumn.SearchColumn = false;
            this.PCCostNameTextBoxColumn.Width = 284;
            // 
            // PCBaseCostBoolColumn
            // 
            this.PCBaseCostBoolColumn.HeaderText = "Базовое?";
            this.PCBaseCostBoolColumn.MappingName = "PCBaseCost";
            this.PCBaseCostBoolColumn.NullText = "";
            this.PCBaseCostBoolColumn.Width = 284;
            // 
            // tpPrice
            // 
            this.tpPrice.Controls.Add(this.pnlFloat);
            this.tpPrice.Controls.Add(this.tcInnerTable);
            this.tpPrice.Controls.Add(this.btnFloatPanel);
            this.tpPrice.Controls.Add(this.panel1);
            this.tpPrice.Location = new System.Drawing.Point(4, 22);
            this.tpPrice.Name = "tpPrice";
            this.tpPrice.Size = new System.Drawing.Size(856, 702);
            this.tpPrice.TabIndex = 1;
            this.tpPrice.Text = "Прайс";
            // 
            // pnlFloat
            // 
            this.pnlFloat.Controls.Add(this.grpbGeneral);
            this.pnlFloat.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlFloat.Location = new System.Drawing.Point(600, 0);
            this.pnlFloat.Name = "pnlFloat";
            this.pnlFloat.Size = new System.Drawing.Size(232, 518);
            this.pnlFloat.TabIndex = 4;
            this.pnlFloat.Visible = false;
            // 
            // grpbGeneral
            // 
            this.grpbGeneral.Controls.Add(this.groupBox1);
            this.grpbGeneral.Controls.Add(this.grpbParent);
            this.grpbGeneral.Controls.Add(this.groupBox2);
            this.grpbGeneral.Controls.Add(this.lblPriceName);
            this.grpbGeneral.Controls.Add(this.lblNameFirm);
            this.grpbGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpbGeneral.Location = new System.Drawing.Point(0, 0);
            this.grpbGeneral.Name = "grpbGeneral";
            this.grpbGeneral.Size = new System.Drawing.Size(232, 518);
            this.grpbGeneral.TabIndex = 0;
            this.grpbGeneral.TabStop = false;
            this.grpbGeneral.Text = "Общая информация по прайс-листу";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rtbArticle);
            this.groupBox1.Controls.Add(this.lblArticle);
            this.groupBox1.Controls.Add(this.lLblMaster);
            this.groupBox1.Controls.Add(this.lblMaster);
            this.groupBox1.Location = new System.Drawing.Point(20, 344);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 163);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            // 
            // rtbArticle
            // 
            this.rtbArticle.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRMemo", true));
            this.rtbArticle.Location = new System.Drawing.Point(2, 74);
            this.rtbArticle.Name = "rtbArticle";
            this.rtbArticle.Size = new System.Drawing.Size(192, 83);
            this.rtbArticle.TabIndex = 29;
            this.rtbArticle.Text = "";
            // 
            // lblArticle
            // 
            this.lblArticle.Location = new System.Drawing.Point(6, 55);
            this.lblArticle.Name = "lblArticle";
            this.lblArticle.Size = new System.Drawing.Size(64, 16);
            this.lblArticle.TabIndex = 28;
            this.lblArticle.Text = "Заметки :";
            // 
            // lLblMaster
            // 
            this.lLblMaster.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRManager", true));
            this.lLblMaster.Location = new System.Drawing.Point(6, 39);
            this.lLblMaster.Name = "lLblMaster";
            this.lLblMaster.Size = new System.Drawing.Size(168, 16);
            this.lLblMaster.TabIndex = 27;
            // 
            // lblMaster
            // 
            this.lblMaster.Location = new System.Drawing.Point(6, 16);
            this.lblMaster.Name = "lblMaster";
            this.lblMaster.Size = new System.Drawing.Size(168, 23);
            this.lblMaster.TabIndex = 16;
            this.lblMaster.Text = "Ответственный за прайс-лист :";
            // 
            // grpbParent
            // 
            this.grpbParent.Controls.Add(this.lblSynonyms);
            this.grpbParent.Controls.Add(this.lblRules);
            this.grpbParent.Controls.Add(this.tbSynonyms);
            this.grpbParent.Controls.Add(this.tbRules);
            this.grpbParent.Location = new System.Drawing.Point(20, 220);
            this.grpbParent.Name = "grpbParent";
            this.grpbParent.Size = new System.Drawing.Size(200, 112);
            this.grpbParent.TabIndex = 31;
            this.grpbParent.TabStop = false;
            this.grpbParent.Text = "Родительские...";
            // 
            // lblSynonyms
            // 
            this.lblSynonyms.Location = new System.Drawing.Point(40, 72);
            this.lblSynonyms.Name = "lblSynonyms";
            this.lblSynonyms.Size = new System.Drawing.Size(80, 20);
            this.lblSynonyms.TabIndex = 3;
            this.lblSynonyms.Text = "... синонимы :";
            this.lblSynonyms.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblRules
            // 
            this.lblRules.Location = new System.Drawing.Point(48, 32);
            this.lblRules.Name = "lblRules";
            this.lblRules.Size = new System.Drawing.Size(72, 20);
            this.lblRules.TabIndex = 2;
            this.lblRules.Text = "... правила :";
            this.lblRules.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbSynonyms
            // 
            this.tbSynonyms.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRSynonyms", true));
            this.tbSynonyms.Location = new System.Drawing.Point(120, 72);
            this.tbSynonyms.Name = "tbSynonyms";
            this.tbSynonyms.Size = new System.Drawing.Size(64, 20);
            this.tbSynonyms.TabIndex = 1;
            // 
            // tbRules
            // 
            this.tbRules.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRRules", true));
            this.tbRules.Location = new System.Drawing.Point(120, 32);
            this.tbRules.Name = "tbRules";
            this.tbRules.Size = new System.Drawing.Size(64, 20);
            this.tbRules.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblDevider);
            this.groupBox2.Controls.Add(this.tbDevider);
            this.groupBox2.Controls.Add(this.lblPosition);
            this.groupBox2.Controls.Add(this.tbPosition);
            this.groupBox2.Controls.Add(this.lblFormat);
            this.groupBox2.Controls.Add(this.cmbFormat);
            this.groupBox2.Controls.Add(this.lblMoney);
            this.groupBox2.Controls.Add(this.cmbMoney);
            this.groupBox2.Location = new System.Drawing.Point(20, 70);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 144);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            // 
            // lblDevider
            // 
            this.lblDevider.Location = new System.Drawing.Point(32, 112);
            this.lblDevider.Name = "lblDevider";
            this.lblDevider.Size = new System.Drawing.Size(80, 20);
            this.lblDevider.TabIndex = 16;
            this.lblDevider.Text = "Разделитель :";
            this.lblDevider.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbDevider
            // 
            this.tbDevider.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRDelimiter", true));
            this.tbDevider.Location = new System.Drawing.Point(120, 112);
            this.tbDevider.Name = "tbDevider";
            this.tbDevider.Size = new System.Drawing.Size(64, 20);
            this.tbDevider.TabIndex = 15;
            // 
            // lblPosition
            // 
            this.lblPosition.Location = new System.Drawing.Point(56, 80);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(56, 20);
            this.lblPosition.TabIndex = 14;
            this.lblPosition.Text = "Позиций :";
            this.lblPosition.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbPosition
            // 
            this.tbPosition.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRPosNum", true));
            this.tbPosition.Location = new System.Drawing.Point(120, 80);
            this.tbPosition.Name = "tbPosition";
            this.tbPosition.Size = new System.Drawing.Size(64, 20);
            this.tbPosition.TabIndex = 13;
            // 
            // lblFormat
            // 
            this.lblFormat.Location = new System.Drawing.Point(56, 48);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(56, 21);
            this.lblFormat.TabIndex = 9;
            this.lblFormat.Text = "Формат :";
            this.lblFormat.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbFormat
            // 
            this.cmbFormat.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFormat", true));
            this.cmbFormat.DataSource = this.dtSet;
            this.cmbFormat.DisplayMember = "Форматы прайса.FMTFormat";
            this.cmbFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFormat.Location = new System.Drawing.Point(120, 48);
            this.cmbFormat.Name = "cmbFormat";
            this.cmbFormat.Size = new System.Drawing.Size(64, 21);
            this.cmbFormat.TabIndex = 8;
            this.cmbFormat.ValueMember = "Форматы прайса.FMTFormat";
            // 
            // lblMoney
            // 
            this.lblMoney.Location = new System.Drawing.Point(56, 16);
            this.lblMoney.Name = "lblMoney";
            this.lblMoney.Size = new System.Drawing.Size(56, 21);
            this.lblMoney.TabIndex = 7;
            this.lblMoney.Text = " Валюта :";
            this.lblMoney.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbMoney
            // 
            this.cmbMoney.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRCurrency", true));
            this.cmbMoney.DataSource = this.dtSet;
            this.cmbMoney.DisplayMember = "Каталог валют.CCCurrency";
            this.cmbMoney.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMoney.Location = new System.Drawing.Point(120, 16);
            this.cmbMoney.Name = "cmbMoney";
            this.cmbMoney.Size = new System.Drawing.Size(64, 21);
            this.cmbMoney.TabIndex = 6;
            this.cmbMoney.ValueMember = "Каталог валют.CCCurrency";
            // 
            // lblPriceName
            // 
            this.lblPriceName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRName", true));
            this.lblPriceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblPriceName.Location = new System.Drawing.Point(17, 32);
            this.lblPriceName.Name = "lblPriceName";
            this.lblPriceName.Size = new System.Drawing.Size(184, 23);
            this.lblPriceName.TabIndex = 29;
            // 
            // lblNameFirm
            // 
            this.lblNameFirm.Location = new System.Drawing.Point(17, 16);
            this.lblNameFirm.Name = "lblNameFirm";
            this.lblNameFirm.Size = new System.Drawing.Size(64, 16);
            this.lblNameFirm.TabIndex = 28;
            this.lblNameFirm.Text = "Название :";
            // 
            // tcInnerTable
            // 
            this.tcInnerTable.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tcInnerTable.Controls.Add(this.tbpTable);
            this.tcInnerTable.Controls.Add(this.tbpMarking);
            this.tcInnerTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcInnerTable.ItemSize = new System.Drawing.Size(0, 1);
            this.tcInnerTable.Location = new System.Drawing.Point(0, 0);
            this.tcInnerTable.Name = "tcInnerTable";
            this.tcInnerTable.SelectedIndex = 0;
            this.tcInnerTable.Size = new System.Drawing.Size(832, 518);
            this.tcInnerTable.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tcInnerTable.TabIndex = 3;
            // 
            // tbpTable
            // 
            this.tbpTable.Controls.Add(this.tcInnerSheets);
            this.tbpTable.Location = new System.Drawing.Point(4, 5);
            this.tbpTable.Name = "tbpTable";
            this.tbpTable.Size = new System.Drawing.Size(824, 509);
            this.tbpTable.TabIndex = 0;
            this.tbpTable.Text = "Таблица";
            // 
            // tcInnerSheets
            // 
            this.tcInnerSheets.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tcInnerSheets.Controls.Add(this.tbpSheet1);
            this.tcInnerSheets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcInnerSheets.ItemSize = new System.Drawing.Size(0, 1);
            this.tcInnerSheets.Location = new System.Drawing.Point(0, 0);
            this.tcInnerSheets.Name = "tcInnerSheets";
            this.tcInnerSheets.SelectedIndex = 0;
            this.tcInnerSheets.Size = new System.Drawing.Size(824, 509);
            this.tcInnerSheets.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tcInnerSheets.TabIndex = 0;
            this.tcInnerSheets.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tcInnerSheets_MouseDown);
            // 
            // tbpSheet1
            // 
            this.tbpSheet1.Controls.Add(this.PriceDataGrid);
            this.tbpSheet1.Location = new System.Drawing.Point(4, 5);
            this.tbpSheet1.Name = "tbpSheet1";
            this.tbpSheet1.Size = new System.Drawing.Size(816, 500);
            this.tbpSheet1.TabIndex = 0;
            this.tbpSheet1.Text = "sheet1";
            // 
            // PriceDataGrid
            // 
            this.PriceDataGrid.CaptionVisible = false;
            this.PriceDataGrid.DataMember = "";
            this.PriceDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PriceDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.PriceDataGrid.Location = new System.Drawing.Point(0, 0);
            this.PriceDataGrid.Name = "PriceDataGrid";
            this.PriceDataGrid.Size = new System.Drawing.Size(816, 500);
            this.PriceDataGrid.TabIndex = 1;
            this.PriceDataGrid.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
            this.PriceDataTableStyle});
            this.PriceDataGrid.ButtonPress += new INDataGrid.INDataGridKeyPressEventHandler(this.PriceDataGrid_ButtonPress);
            this.PriceDataGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PriceDataGrid_MouseDown);
            // 
            // PriceDataTableStyle
            // 
            this.PriceDataTableStyle.ColumnSizeAutoFit = false;
            this.PriceDataTableStyle.DataGrid = this.PriceDataGrid;
            this.PriceDataTableStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.PriceDataTableStyle.RowHeadersVisible = false;
            this.PriceDataTableStyle.RowSelected = false;
            // 
            // tbpMarking
            // 
            this.tbpMarking.Controls.Add(this.MarkingDataGrid);
            this.tbpMarking.Location = new System.Drawing.Point(4, 5);
            this.tbpMarking.Name = "tbpMarking";
            this.tbpMarking.Size = new System.Drawing.Size(824, 509);
            this.tbpMarking.TabIndex = 1;
            this.tbpMarking.Text = "Разметка";
            this.tbpMarking.Visible = false;
            // 
            // MarkingDataGrid
            // 
            this.MarkingDataGrid.CaptionVisible = false;
            this.MarkingDataGrid.DataMember = "";
            this.MarkingDataGrid.DataSource = this.dtMarking;
            this.MarkingDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MarkingDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.MarkingDataGrid.Location = new System.Drawing.Point(0, 0);
            this.MarkingDataGrid.Name = "MarkingDataGrid";
            this.MarkingDataGrid.Size = new System.Drawing.Size(824, 509);
            this.MarkingDataGrid.TabIndex = 0;
            this.MarkingDataGrid.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
            this.MarkingTableStyle});
            // 
            // MarkingTableStyle
            // 
            this.MarkingTableStyle.ColumnSizeAutoFit = true;
            this.MarkingTableStyle.DataGrid = this.MarkingDataGrid;
            this.MarkingTableStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.MarkingTableStyle.MappingName = "Разметка";
            this.MarkingTableStyle.RowHeadersVisible = false;
            this.MarkingTableStyle.RowSelected = false;
            // 
            // btnFloatPanel
            // 
            this.btnFloatPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnFloatPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnFloatPanel.Location = new System.Drawing.Point(832, 0);
            this.btnFloatPanel.Name = "btnFloatPanel";
            this.btnFloatPanel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnFloatPanel.Size = new System.Drawing.Size(24, 518);
            this.btnFloatPanel.TabIndex = 1;
            this.btnFloatPanel.Click += new System.EventHandler(this.btnFloatPanel_Click);
            this.btnFloatPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.btnFloatPanel_Paint);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CostsDataGrid);
            this.panel1.Controls.Add(this.grpbFields);
            this.panel1.Controls.Add(this.grpbSettings);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 518);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(856, 184);
            this.panel1.TabIndex = 0;
            // 
            // CostsDataGrid
            // 
            this.CostsDataGrid.AllowDrop = true;
            this.CostsDataGrid.CaptionText = "Цены";
            this.CostsDataGrid.DataMember = "Поставщики.Поставщики-Прайсы.Прайсы-Правила формализации цен";
            this.CostsDataGrid.DataSource = this.dtSet;
            this.CostsDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CostsDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.CostsDataGrid.Location = new System.Drawing.Point(728, 0);
            this.CostsDataGrid.Name = "CostsDataGrid";
            this.CostsDataGrid.ReadOnly = true;
            this.CostsDataGrid.Size = new System.Drawing.Size(128, 184);
            this.CostsDataGrid.TabIndex = 5;
            this.CostsDataGrid.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
            this.CostsFormRulesTableStyle});
            this.CostsDataGrid.DragEnter += new System.Windows.Forms.DragEventHandler(this.CostsDataGrid_DragEnter);
            this.CostsDataGrid.DragOver += new System.Windows.Forms.DragEventHandler(this.CostsDataGrid_DragOver);
            this.CostsDataGrid.DragDrop += new System.Windows.Forms.DragEventHandler(this.CostsDataGrid_DragDrop);
            // 
            // CostsFormRulesTableStyle
            // 
            this.CostsFormRulesTableStyle.ColumnSizeAutoFit = true;
            this.CostsFormRulesTableStyle.DataGrid = this.CostsDataGrid;
            this.CostsFormRulesTableStyle.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
            this.CFRCostCodeTextBoxColumn,
            this.CFRCostNameTextBoxColumn});
            this.CostsFormRulesTableStyle.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.CostsFormRulesTableStyle.MappingName = "Правила формализации цен";
            this.CostsFormRulesTableStyle.RowHeadersVisible = false;
            this.CostsFormRulesTableStyle.RowSelected = false;
            // 
            // CFRCostCodeTextBoxColumn
            // 
            this.CFRCostCodeTextBoxColumn.EditDisable = true;
            this.CFRCostCodeTextBoxColumn.Format = "";
            this.CFRCostCodeTextBoxColumn.FormatInfo = null;
            this.CFRCostCodeTextBoxColumn.HeaderText = "Код цены";
            this.CFRCostCodeTextBoxColumn.MappingName = "CFRCost_Code";
            this.CFRCostCodeTextBoxColumn.NullText = "";
            this.CFRCostCodeTextBoxColumn.SearchColumn = false;
            this.CFRCostCodeTextBoxColumn.Width = 62;
            // 
            // CFRCostNameTextBoxColumn
            // 
            this.CFRCostNameTextBoxColumn.EditDisable = true;
            this.CFRCostNameTextBoxColumn.Format = "";
            this.CFRCostNameTextBoxColumn.FormatInfo = null;
            this.CFRCostNameTextBoxColumn.HeaderText = "Наименование цены";
            this.CFRCostNameTextBoxColumn.MappingName = "CFRCostName";
            this.CFRCostNameTextBoxColumn.NullText = "";
            this.CFRCostNameTextBoxColumn.SearchColumn = false;
            this.CFRCostNameTextBoxColumn.Width = 62;
            // 
            // grpbFields
            // 
            this.grpbFields.Controls.Add(this.pnlTxtFields);
            this.grpbFields.Controls.Add(this.pnlGeneralFields);
            this.grpbFields.Dock = System.Windows.Forms.DockStyle.Left;
            this.grpbFields.Location = new System.Drawing.Point(246, 0);
            this.grpbFields.Name = "grpbFields";
            this.grpbFields.Size = new System.Drawing.Size(482, 184);
            this.grpbFields.TabIndex = 4;
            this.grpbFields.TabStop = false;
            this.grpbFields.Text = "Поля";
            // 
            // pnlTxtFields
            // 
            this.pnlTxtFields.Controls.Add(this.txtBoxAwaitEnd);
            this.pnlTxtFields.Controls.Add(this.txtBoxJunkEnd);
            this.pnlTxtFields.Controls.Add(this.txtBoxAwaitBegin);
            this.pnlTxtFields.Controls.Add(this.txtBoxJunkBegin);
            this.pnlTxtFields.Controls.Add(this.txtBoxMinBoundCostEnd);
            this.pnlTxtFields.Controls.Add(this.txtBoxMinBoundCostBegin);
            this.pnlTxtFields.Controls.Add(this.txtBoxCurrencyEnd);
            this.pnlTxtFields.Controls.Add(this.txtBoxDocEnd);
            this.pnlTxtFields.Controls.Add(this.txtBoxPeriodEnd);
            this.pnlTxtFields.Controls.Add(this.txtBoxNoteEnd);
            this.pnlTxtFields.Controls.Add(this.txtBoxCurrencyBegin);
            this.pnlTxtFields.Controls.Add(this.txtBoxDocBegin);
            this.pnlTxtFields.Controls.Add(this.txtBoxPeriodBegin);
            this.pnlTxtFields.Controls.Add(this.txtBoxNoteBegin);
            this.pnlTxtFields.Controls.Add(this.txtBoxQuantityEnd);
            this.pnlTxtFields.Controls.Add(this.txtBoxQuantityBegin);
            this.pnlTxtFields.Controls.Add(this.txtBoxVolumeEnd);
            this.pnlTxtFields.Controls.Add(this.txtBoxVolumeBegin);
            this.pnlTxtFields.Controls.Add(this.txtBoxUnitEnd);
            this.pnlTxtFields.Controls.Add(this.txtBoxUnitBegin);
            this.pnlTxtFields.Controls.Add(this.txtBoxFirmCrEnd);
            this.pnlTxtFields.Controls.Add(this.txtBoxFirmCrBegin);
            this.pnlTxtFields.Controls.Add(this.txtBoxName1End);
            this.pnlTxtFields.Controls.Add(this.txtBoxName1Begin);
            this.pnlTxtFields.Controls.Add(this.txtBoxCodeCrEnd);
            this.pnlTxtFields.Controls.Add(this.txtBoxCodeCrBegin);
            this.pnlTxtFields.Controls.Add(this.txtBoxCodeEnd);
            this.pnlTxtFields.Controls.Add(this.label24);
            this.pnlTxtFields.Controls.Add(this.label25);
            this.pnlTxtFields.Controls.Add(this.label26);
            this.pnlTxtFields.Controls.Add(this.label27);
            this.pnlTxtFields.Controls.Add(this.label29);
            this.pnlTxtFields.Controls.Add(this.label30);
            this.pnlTxtFields.Controls.Add(this.label31);
            this.pnlTxtFields.Controls.Add(this.label32);
            this.pnlTxtFields.Controls.Add(this.label33);
            this.pnlTxtFields.Controls.Add(this.label34);
            this.pnlTxtFields.Controls.Add(this.txtBoxCodeBegin);
            this.pnlTxtFields.Controls.Add(this.label35);
            this.pnlTxtFields.Controls.Add(this.label38);
            this.pnlTxtFields.Controls.Add(this.label39);
            this.pnlTxtFields.Controls.Add(this.label40);
            this.pnlTxtFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTxtFields.Location = new System.Drawing.Point(3, 16);
            this.pnlTxtFields.Name = "pnlTxtFields";
            this.pnlTxtFields.Size = new System.Drawing.Size(476, 165);
            this.pnlTxtFields.TabIndex = 1;
            // 
            // txtBoxAwaitEnd
            // 
            this.txtBoxAwaitEnd.AccessibleName = "AwaitEnd";
            this.txtBoxAwaitEnd.AllowDrop = true;
            this.txtBoxAwaitEnd.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtAwaitEnd", true));
            this.txtBoxAwaitEnd.Location = new System.Drawing.Point(443, 104);
            this.txtBoxAwaitEnd.Name = "txtBoxAwaitEnd";
            this.txtBoxAwaitEnd.ReadOnly = true;
            this.txtBoxAwaitEnd.Size = new System.Drawing.Size(27, 20);
            this.txtBoxAwaitEnd.TabIndex = 97;
            this.txtBoxAwaitEnd.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxAwaitEnd.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxJunkEnd
            // 
            this.txtBoxJunkEnd.AccessibleName = "JunkEnd";
            this.txtBoxJunkEnd.AllowDrop = true;
            this.txtBoxJunkEnd.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtJunkEnd", true));
            this.txtBoxJunkEnd.Location = new System.Drawing.Point(443, 72);
            this.txtBoxJunkEnd.Name = "txtBoxJunkEnd";
            this.txtBoxJunkEnd.ReadOnly = true;
            this.txtBoxJunkEnd.Size = new System.Drawing.Size(27, 20);
            this.txtBoxJunkEnd.TabIndex = 96;
            this.txtBoxJunkEnd.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxJunkEnd.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxAwaitBegin
            // 
            this.txtBoxAwaitBegin.AccessibleName = "AwaitBegin";
            this.txtBoxAwaitBegin.AllowDrop = true;
            this.txtBoxAwaitBegin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtAwaitBegin", true));
            this.txtBoxAwaitBegin.Location = new System.Drawing.Point(416, 104);
            this.txtBoxAwaitBegin.Name = "txtBoxAwaitBegin";
            this.txtBoxAwaitBegin.ReadOnly = true;
            this.txtBoxAwaitBegin.Size = new System.Drawing.Size(27, 20);
            this.txtBoxAwaitBegin.TabIndex = 95;
            this.txtBoxAwaitBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxAwaitBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxJunkBegin
            // 
            this.txtBoxJunkBegin.AccessibleName = "JunkBegin";
            this.txtBoxJunkBegin.AllowDrop = true;
            this.txtBoxJunkBegin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtJunkBegin", true));
            this.txtBoxJunkBegin.Location = new System.Drawing.Point(416, 72);
            this.txtBoxJunkBegin.Name = "txtBoxJunkBegin";
            this.txtBoxJunkBegin.ReadOnly = true;
            this.txtBoxJunkBegin.Size = new System.Drawing.Size(27, 20);
            this.txtBoxJunkBegin.TabIndex = 94;
            this.txtBoxJunkBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxJunkBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxMinBoundCostEnd
            // 
            this.txtBoxMinBoundCostEnd.AccessibleName = "MinBoundCostEnd";
            this.txtBoxMinBoundCostEnd.AllowDrop = true;
            this.txtBoxMinBoundCostEnd.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtMinBoundCostEnd", true));
            this.txtBoxMinBoundCostEnd.Location = new System.Drawing.Point(443, 40);
            this.txtBoxMinBoundCostEnd.Name = "txtBoxMinBoundCostEnd";
            this.txtBoxMinBoundCostEnd.ReadOnly = true;
            this.txtBoxMinBoundCostEnd.Size = new System.Drawing.Size(27, 20);
            this.txtBoxMinBoundCostEnd.TabIndex = 93;
            this.txtBoxMinBoundCostEnd.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxMinBoundCostEnd.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // txtBoxMinBoundCostBegin
            // 
            this.txtBoxMinBoundCostBegin.AccessibleName = "MinBoundCostBegin";
            this.txtBoxMinBoundCostBegin.AllowDrop = true;
            this.txtBoxMinBoundCostBegin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtMinBoundCostBegin", true));
            this.txtBoxMinBoundCostBegin.Location = new System.Drawing.Point(416, 40);
            this.txtBoxMinBoundCostBegin.Name = "txtBoxMinBoundCostBegin";
            this.txtBoxMinBoundCostBegin.ReadOnly = true;
            this.txtBoxMinBoundCostBegin.Size = new System.Drawing.Size(27, 20);
            this.txtBoxMinBoundCostBegin.TabIndex = 92;
            this.txtBoxMinBoundCostBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.CostsDataGrid_DragDrop);
            this.txtBoxMinBoundCostBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.CostsDataGrid_DragEnter);
            // 
            // txtBoxCurrencyEnd
            // 
            this.txtBoxCurrencyEnd.AccessibleName = "CurrencyEnd";
            this.txtBoxCurrencyEnd.AllowDrop = true;
            this.txtBoxCurrencyEnd.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtCurrencyEnd", true));
            this.txtBoxCurrencyEnd.Location = new System.Drawing.Point(443, 8);
            this.txtBoxCurrencyEnd.Name = "txtBoxCurrencyEnd";
            this.txtBoxCurrencyEnd.ReadOnly = true;
            this.txtBoxCurrencyEnd.Size = new System.Drawing.Size(27, 20);
            this.txtBoxCurrencyEnd.TabIndex = 91;
            this.txtBoxCurrencyEnd.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxCurrencyEnd.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxDocEnd
            // 
            this.txtBoxDocEnd.AccessibleName = "DocEnd";
            this.txtBoxDocEnd.AllowDrop = true;
            this.txtBoxDocEnd.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtDocEnd", true));
            this.txtBoxDocEnd.Location = new System.Drawing.Point(299, 136);
            this.txtBoxDocEnd.Name = "txtBoxDocEnd";
            this.txtBoxDocEnd.ReadOnly = true;
            this.txtBoxDocEnd.Size = new System.Drawing.Size(27, 20);
            this.txtBoxDocEnd.TabIndex = 89;
            this.txtBoxDocEnd.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxDocEnd.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxPeriodEnd
            // 
            this.txtBoxPeriodEnd.AccessibleName = "PeriodEnd";
            this.txtBoxPeriodEnd.AllowDrop = true;
            this.txtBoxPeriodEnd.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtPeriodEnd", true));
            this.txtBoxPeriodEnd.Location = new System.Drawing.Point(299, 104);
            this.txtBoxPeriodEnd.Name = "txtBoxPeriodEnd";
            this.txtBoxPeriodEnd.ReadOnly = true;
            this.txtBoxPeriodEnd.Size = new System.Drawing.Size(27, 20);
            this.txtBoxPeriodEnd.TabIndex = 88;
            this.txtBoxPeriodEnd.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxPeriodEnd.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxNoteEnd
            // 
            this.txtBoxNoteEnd.AccessibleName = "NoteEnd";
            this.txtBoxNoteEnd.AllowDrop = true;
            this.txtBoxNoteEnd.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtNoteEnd", true));
            this.txtBoxNoteEnd.Location = new System.Drawing.Point(299, 72);
            this.txtBoxNoteEnd.Name = "txtBoxNoteEnd";
            this.txtBoxNoteEnd.ReadOnly = true;
            this.txtBoxNoteEnd.Size = new System.Drawing.Size(27, 20);
            this.txtBoxNoteEnd.TabIndex = 87;
            this.txtBoxNoteEnd.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxNoteEnd.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxCurrencyBegin
            // 
            this.txtBoxCurrencyBegin.AccessibleName = "CurrencyBegin";
            this.txtBoxCurrencyBegin.AllowDrop = true;
            this.txtBoxCurrencyBegin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtCurrencyBegin", true));
            this.txtBoxCurrencyBegin.Location = new System.Drawing.Point(416, 8);
            this.txtBoxCurrencyBegin.Name = "txtBoxCurrencyBegin";
            this.txtBoxCurrencyBegin.ReadOnly = true;
            this.txtBoxCurrencyBegin.Size = new System.Drawing.Size(27, 20);
            this.txtBoxCurrencyBegin.TabIndex = 86;
            this.txtBoxCurrencyBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxCurrencyBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxDocBegin
            // 
            this.txtBoxDocBegin.AccessibleName = "DocBegin";
            this.txtBoxDocBegin.AllowDrop = true;
            this.txtBoxDocBegin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtDocBegin", true));
            this.txtBoxDocBegin.Location = new System.Drawing.Point(272, 136);
            this.txtBoxDocBegin.Name = "txtBoxDocBegin";
            this.txtBoxDocBegin.ReadOnly = true;
            this.txtBoxDocBegin.Size = new System.Drawing.Size(27, 20);
            this.txtBoxDocBegin.TabIndex = 84;
            this.txtBoxDocBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxDocBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxPeriodBegin
            // 
            this.txtBoxPeriodBegin.AccessibleName = "PeriodBegin";
            this.txtBoxPeriodBegin.AllowDrop = true;
            this.txtBoxPeriodBegin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtPeriodBegin", true));
            this.txtBoxPeriodBegin.Location = new System.Drawing.Point(272, 104);
            this.txtBoxPeriodBegin.Name = "txtBoxPeriodBegin";
            this.txtBoxPeriodBegin.ReadOnly = true;
            this.txtBoxPeriodBegin.Size = new System.Drawing.Size(27, 20);
            this.txtBoxPeriodBegin.TabIndex = 83;
            this.txtBoxPeriodBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxPeriodBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxNoteBegin
            // 
            this.txtBoxNoteBegin.AccessibleName = "NoteBegin";
            this.txtBoxNoteBegin.AllowDrop = true;
            this.txtBoxNoteBegin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtNoteBegin", true));
            this.txtBoxNoteBegin.Location = new System.Drawing.Point(272, 72);
            this.txtBoxNoteBegin.Name = "txtBoxNoteBegin";
            this.txtBoxNoteBegin.ReadOnly = true;
            this.txtBoxNoteBegin.Size = new System.Drawing.Size(27, 20);
            this.txtBoxNoteBegin.TabIndex = 82;
            this.txtBoxNoteBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxNoteBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxQuantityEnd
            // 
            this.txtBoxQuantityEnd.AccessibleName = "QuantityEnd";
            this.txtBoxQuantityEnd.AllowDrop = true;
            this.txtBoxQuantityEnd.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtQuantityEnd", true));
            this.txtBoxQuantityEnd.Location = new System.Drawing.Point(299, 40);
            this.txtBoxQuantityEnd.Name = "txtBoxQuantityEnd";
            this.txtBoxQuantityEnd.ReadOnly = true;
            this.txtBoxQuantityEnd.Size = new System.Drawing.Size(27, 20);
            this.txtBoxQuantityEnd.TabIndex = 81;
            this.txtBoxQuantityEnd.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxQuantityEnd.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxQuantityBegin
            // 
            this.txtBoxQuantityBegin.AccessibleName = "QuantityBegin";
            this.txtBoxQuantityBegin.AllowDrop = true;
            this.txtBoxQuantityBegin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtQuantityBegin", true));
            this.txtBoxQuantityBegin.Location = new System.Drawing.Point(272, 40);
            this.txtBoxQuantityBegin.Name = "txtBoxQuantityBegin";
            this.txtBoxQuantityBegin.ReadOnly = true;
            this.txtBoxQuantityBegin.Size = new System.Drawing.Size(27, 20);
            this.txtBoxQuantityBegin.TabIndex = 80;
            this.txtBoxQuantityBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxQuantityBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxVolumeEnd
            // 
            this.txtBoxVolumeEnd.AccessibleName = "VolumeEnd";
            this.txtBoxVolumeEnd.AllowDrop = true;
            this.txtBoxVolumeEnd.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtVolumeEnd", true));
            this.txtBoxVolumeEnd.Location = new System.Drawing.Point(299, 8);
            this.txtBoxVolumeEnd.Name = "txtBoxVolumeEnd";
            this.txtBoxVolumeEnd.ReadOnly = true;
            this.txtBoxVolumeEnd.Size = new System.Drawing.Size(27, 20);
            this.txtBoxVolumeEnd.TabIndex = 79;
            this.txtBoxVolumeEnd.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxVolumeEnd.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxVolumeBegin
            // 
            this.txtBoxVolumeBegin.AccessibleName = "VolumeBegin";
            this.txtBoxVolumeBegin.AllowDrop = true;
            this.txtBoxVolumeBegin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtVolumeBegin", true));
            this.txtBoxVolumeBegin.Location = new System.Drawing.Point(272, 8);
            this.txtBoxVolumeBegin.Name = "txtBoxVolumeBegin";
            this.txtBoxVolumeBegin.ReadOnly = true;
            this.txtBoxVolumeBegin.Size = new System.Drawing.Size(27, 20);
            this.txtBoxVolumeBegin.TabIndex = 78;
            this.txtBoxVolumeBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxVolumeBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxUnitEnd
            // 
            this.txtBoxUnitEnd.AccessibleName = "UnitEnd";
            this.txtBoxUnitEnd.AllowDrop = true;
            this.txtBoxUnitEnd.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtUnitEnd", true));
            this.txtBoxUnitEnd.Location = new System.Drawing.Point(139, 136);
            this.txtBoxUnitEnd.Name = "txtBoxUnitEnd";
            this.txtBoxUnitEnd.ReadOnly = true;
            this.txtBoxUnitEnd.Size = new System.Drawing.Size(27, 20);
            this.txtBoxUnitEnd.TabIndex = 77;
            this.txtBoxUnitEnd.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxUnitEnd.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxUnitBegin
            // 
            this.txtBoxUnitBegin.AccessibleName = "UnitBegin";
            this.txtBoxUnitBegin.AllowDrop = true;
            this.txtBoxUnitBegin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtUnitBegin", true));
            this.txtBoxUnitBegin.Location = new System.Drawing.Point(112, 136);
            this.txtBoxUnitBegin.Name = "txtBoxUnitBegin";
            this.txtBoxUnitBegin.ReadOnly = true;
            this.txtBoxUnitBegin.Size = new System.Drawing.Size(27, 20);
            this.txtBoxUnitBegin.TabIndex = 76;
            this.txtBoxUnitBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxUnitBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxFirmCrEnd
            // 
            this.txtBoxFirmCrEnd.AccessibleName = "FirmCrEnd";
            this.txtBoxFirmCrEnd.AllowDrop = true;
            this.txtBoxFirmCrEnd.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtFirmCrEnd", true));
            this.txtBoxFirmCrEnd.Location = new System.Drawing.Point(139, 104);
            this.txtBoxFirmCrEnd.Name = "txtBoxFirmCrEnd";
            this.txtBoxFirmCrEnd.ReadOnly = true;
            this.txtBoxFirmCrEnd.Size = new System.Drawing.Size(27, 20);
            this.txtBoxFirmCrEnd.TabIndex = 75;
            this.txtBoxFirmCrEnd.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxFirmCrEnd.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxFirmCrBegin
            // 
            this.txtBoxFirmCrBegin.AccessibleName = "FirmCrBegin";
            this.txtBoxFirmCrBegin.AllowDrop = true;
            this.txtBoxFirmCrBegin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtFirmCrBegin", true));
            this.txtBoxFirmCrBegin.Location = new System.Drawing.Point(112, 104);
            this.txtBoxFirmCrBegin.Name = "txtBoxFirmCrBegin";
            this.txtBoxFirmCrBegin.ReadOnly = true;
            this.txtBoxFirmCrBegin.Size = new System.Drawing.Size(27, 20);
            this.txtBoxFirmCrBegin.TabIndex = 74;
            this.txtBoxFirmCrBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxFirmCrBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxName1End
            // 
            this.txtBoxName1End.AccessibleName = "Name1End";
            this.txtBoxName1End.AllowDrop = true;
            this.txtBoxName1End.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtNameEnd", true));
            this.txtBoxName1End.Location = new System.Drawing.Point(139, 72);
            this.txtBoxName1End.Name = "txtBoxName1End";
            this.txtBoxName1End.ReadOnly = true;
            this.txtBoxName1End.Size = new System.Drawing.Size(27, 20);
            this.txtBoxName1End.TabIndex = 73;
            this.txtBoxName1End.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxName1End.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxName1Begin
            // 
            this.txtBoxName1Begin.AccessibleName = "Name1Begin";
            this.txtBoxName1Begin.AllowDrop = true;
            this.txtBoxName1Begin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtNameBegin", true));
            this.txtBoxName1Begin.Location = new System.Drawing.Point(112, 72);
            this.txtBoxName1Begin.Name = "txtBoxName1Begin";
            this.txtBoxName1Begin.ReadOnly = true;
            this.txtBoxName1Begin.Size = new System.Drawing.Size(27, 20);
            this.txtBoxName1Begin.TabIndex = 72;
            this.txtBoxName1Begin.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxName1Begin.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxCodeCrEnd
            // 
            this.txtBoxCodeCrEnd.AccessibleName = "CodeCrEnd";
            this.txtBoxCodeCrEnd.AllowDrop = true;
            this.txtBoxCodeCrEnd.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtCodeEnd", true));
            this.txtBoxCodeCrEnd.Location = new System.Drawing.Point(139, 40);
            this.txtBoxCodeCrEnd.Name = "txtBoxCodeCrEnd";
            this.txtBoxCodeCrEnd.ReadOnly = true;
            this.txtBoxCodeCrEnd.Size = new System.Drawing.Size(27, 20);
            this.txtBoxCodeCrEnd.TabIndex = 71;
            this.txtBoxCodeCrEnd.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxCodeCrEnd.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxCodeCrBegin
            // 
            this.txtBoxCodeCrBegin.AccessibleName = "CodeCrBegin";
            this.txtBoxCodeCrBegin.AllowDrop = true;
            this.txtBoxCodeCrBegin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtCodeCrBegin", true));
            this.txtBoxCodeCrBegin.Location = new System.Drawing.Point(112, 40);
            this.txtBoxCodeCrBegin.Name = "txtBoxCodeCrBegin";
            this.txtBoxCodeCrBegin.ReadOnly = true;
            this.txtBoxCodeCrBegin.Size = new System.Drawing.Size(27, 20);
            this.txtBoxCodeCrBegin.TabIndex = 70;
            this.txtBoxCodeCrBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxCodeCrBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // txtBoxCodeEnd
            // 
            this.txtBoxCodeEnd.AccessibleName = "CodeEnd";
            this.txtBoxCodeEnd.AllowDrop = true;
            this.txtBoxCodeEnd.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtCodeEnd", true));
            this.txtBoxCodeEnd.Location = new System.Drawing.Point(139, 8);
            this.txtBoxCodeEnd.Name = "txtBoxCodeEnd";
            this.txtBoxCodeEnd.ReadOnly = true;
            this.txtBoxCodeEnd.Size = new System.Drawing.Size(27, 20);
            this.txtBoxCodeEnd.TabIndex = 69;
            this.txtBoxCodeEnd.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxCodeEnd.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(336, 104);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(80, 23);
            this.label24.TabIndex = 63;
            this.label24.Text = "Ожидается :";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label25
            // 
            this.label25.Location = new System.Drawing.Point(328, 72);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(80, 23);
            this.label25.TabIndex = 62;
            this.label25.Text = "Срок :";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label26
            // 
            this.label26.Location = new System.Drawing.Point(328, 40);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(80, 23);
            this.label26.TabIndex = 61;
            this.label26.Text = "Цена мин. :";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label27
            // 
            this.label27.Location = new System.Drawing.Point(352, 8);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(56, 23);
            this.label27.TabIndex = 60;
            this.label27.Text = "Валюта :";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(168, 136);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(100, 23);
            this.label29.TabIndex = 52;
            this.label29.Text = "Документ :";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label30
            // 
            this.label30.Location = new System.Drawing.Point(168, 104);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(100, 23);
            this.label30.TabIndex = 51;
            this.label30.Text = "Срок годности :";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label31
            // 
            this.label31.Location = new System.Drawing.Point(168, 72);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(100, 23);
            this.label31.TabIndex = 50;
            this.label31.Text = "Примечание :";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label32
            // 
            this.label32.Location = new System.Drawing.Point(168, 40);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(100, 23);
            this.label32.TabIndex = 49;
            this.label32.Text = "Количество :";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label33
            // 
            this.label33.Location = new System.Drawing.Point(184, 8);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(80, 23);
            this.label33.TabIndex = 48;
            this.label33.Text = "Цех. уп. :";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label34
            // 
            this.label34.Location = new System.Drawing.Point(8, 136);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(100, 23);
            this.label34.TabIndex = 47;
            this.label34.Text = "Ед. измерения :";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBoxCodeBegin
            // 
            this.txtBoxCodeBegin.AccessibleName = "CodeBegin";
            this.txtBoxCodeBegin.AllowDrop = true;
            this.txtBoxCodeBegin.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRTxtCodeBegin", true));
            this.txtBoxCodeBegin.Location = new System.Drawing.Point(112, 8);
            this.txtBoxCodeBegin.Name = "txtBoxCodeBegin";
            this.txtBoxCodeBegin.ReadOnly = true;
            this.txtBoxCodeBegin.Size = new System.Drawing.Size(27, 20);
            this.txtBoxCodeBegin.TabIndex = 41;
            this.txtBoxCodeBegin.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragDrop);
            this.txtBoxCodeBegin.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCodeBegin_DragEnter);
            // 
            // label35
            // 
            this.label35.Location = new System.Drawing.Point(8, 104);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(100, 23);
            this.label35.TabIndex = 40;
            this.label35.Text = "Производитель :";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label38
            // 
            this.label38.Location = new System.Drawing.Point(0, 72);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(104, 23);
            this.label38.TabIndex = 37;
            this.label38.Text = "Наименование 1 :";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label39
            // 
            this.label39.Location = new System.Drawing.Point(8, 40);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(100, 23);
            this.label39.TabIndex = 36;
            this.label39.Text = "Код производ. :";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label40
            // 
            this.label40.Location = new System.Drawing.Point(8, 8);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(100, 23);
            this.label40.TabIndex = 35;
            this.label40.Text = "Код :";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlGeneralFields
            // 
            this.pnlGeneralFields.Controls.Add(this.txtBoxAwait);
            this.pnlGeneralFields.Controls.Add(this.txtBoxJunk);
            this.pnlGeneralFields.Controls.Add(this.txtBoxMinBoundCost);
            this.pnlGeneralFields.Controls.Add(this.txtBoxCurrency);
            this.pnlGeneralFields.Controls.Add(this.label17);
            this.pnlGeneralFields.Controls.Add(this.label16);
            this.pnlGeneralFields.Controls.Add(this.label15);
            this.pnlGeneralFields.Controls.Add(this.label14);
            this.pnlGeneralFields.Controls.Add(this.txtBoxDoc);
            this.pnlGeneralFields.Controls.Add(this.txtBoxPeriod);
            this.pnlGeneralFields.Controls.Add(this.txtBoxNote);
            this.pnlGeneralFields.Controls.Add(this.txtBoxQuantity);
            this.pnlGeneralFields.Controls.Add(this.txtBoxUnit);
            this.pnlGeneralFields.Controls.Add(this.txtBoxVolume);
            this.pnlGeneralFields.Controls.Add(this.label12);
            this.pnlGeneralFields.Controls.Add(this.label11);
            this.pnlGeneralFields.Controls.Add(this.label10);
            this.pnlGeneralFields.Controls.Add(this.label9);
            this.pnlGeneralFields.Controls.Add(this.label8);
            this.pnlGeneralFields.Controls.Add(this.label7);
            this.pnlGeneralFields.Controls.Add(this.txtBoxFirmCr);
            this.pnlGeneralFields.Controls.Add(this.txtBoxName3);
            this.pnlGeneralFields.Controls.Add(this.txtBoxName2);
            this.pnlGeneralFields.Controls.Add(this.txtBoxName1);
            this.pnlGeneralFields.Controls.Add(this.txtBoxCodeCr);
            this.pnlGeneralFields.Controls.Add(this.txtBoxCode);
            this.pnlGeneralFields.Controls.Add(this.label6);
            this.pnlGeneralFields.Controls.Add(this.label5);
            this.pnlGeneralFields.Controls.Add(this.label4);
            this.pnlGeneralFields.Controls.Add(this.label3);
            this.pnlGeneralFields.Controls.Add(this.label2);
            this.pnlGeneralFields.Controls.Add(this.label1);
            this.pnlGeneralFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGeneralFields.Location = new System.Drawing.Point(3, 16);
            this.pnlGeneralFields.Name = "pnlGeneralFields";
            this.pnlGeneralFields.Size = new System.Drawing.Size(476, 165);
            this.pnlGeneralFields.TabIndex = 0;
            // 
            // txtBoxAwait
            // 
            this.txtBoxAwait.AllowDrop = true;
            this.txtBoxAwait.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFAwait", true));
            this.txtBoxAwait.Location = new System.Drawing.Point(424, 80);
            this.txtBoxAwait.Name = "txtBoxAwait";
            this.txtBoxAwait.ReadOnly = true;
            this.txtBoxAwait.Size = new System.Drawing.Size(40, 20);
            this.txtBoxAwait.TabIndex = 68;
            this.txtBoxAwait.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxAwait.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // txtBoxJunk
            // 
            this.txtBoxJunk.AllowDrop = true;
            this.txtBoxJunk.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFJunk", true));
            this.txtBoxJunk.Location = new System.Drawing.Point(424, 56);
            this.txtBoxJunk.Name = "txtBoxJunk";
            this.txtBoxJunk.ReadOnly = true;
            this.txtBoxJunk.Size = new System.Drawing.Size(40, 20);
            this.txtBoxJunk.TabIndex = 67;
            this.txtBoxJunk.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxJunk.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // txtBoxMinBoundCost
            // 
            this.txtBoxMinBoundCost.AllowDrop = true;
            this.txtBoxMinBoundCost.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFMinBoundCost", true));
            this.txtBoxMinBoundCost.Location = new System.Drawing.Point(424, 32);
            this.txtBoxMinBoundCost.Name = "txtBoxMinBoundCost";
            this.txtBoxMinBoundCost.ReadOnly = true;
            this.txtBoxMinBoundCost.Size = new System.Drawing.Size(40, 20);
            this.txtBoxMinBoundCost.TabIndex = 66;
            this.txtBoxMinBoundCost.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxMinBoundCost.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // txtBoxCurrency
            // 
            this.txtBoxCurrency.AllowDrop = true;
            this.txtBoxCurrency.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFCurrency", true));
            this.txtBoxCurrency.Location = new System.Drawing.Point(424, 8);
            this.txtBoxCurrency.Name = "txtBoxCurrency";
            this.txtBoxCurrency.ReadOnly = true;
            this.txtBoxCurrency.Size = new System.Drawing.Size(40, 20);
            this.txtBoxCurrency.TabIndex = 65;
            this.txtBoxCurrency.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxCurrency.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(320, 80);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(100, 23);
            this.label17.TabIndex = 63;
            this.label17.Text = "Ожидается :";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(320, 56);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(100, 23);
            this.label16.TabIndex = 62;
            this.label16.Text = "Срок :";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(320, 32);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(100, 23);
            this.label15.TabIndex = 61;
            this.label15.Text = "Цена мин. :";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(320, 8);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(100, 23);
            this.label14.TabIndex = 60;
            this.label14.Text = "Валюта :";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBoxDoc
            // 
            this.txtBoxDoc.AllowDrop = true;
            this.txtBoxDoc.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFDoc", true));
            this.txtBoxDoc.Location = new System.Drawing.Point(272, 129);
            this.txtBoxDoc.Name = "txtBoxDoc";
            this.txtBoxDoc.ReadOnly = true;
            this.txtBoxDoc.Size = new System.Drawing.Size(40, 20);
            this.txtBoxDoc.TabIndex = 58;
            this.txtBoxDoc.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxDoc.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // txtBoxPeriod
            // 
            this.txtBoxPeriod.AllowDrop = true;
            this.txtBoxPeriod.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFPeriod", true));
            this.txtBoxPeriod.Location = new System.Drawing.Point(272, 105);
            this.txtBoxPeriod.Name = "txtBoxPeriod";
            this.txtBoxPeriod.ReadOnly = true;
            this.txtBoxPeriod.Size = new System.Drawing.Size(40, 20);
            this.txtBoxPeriod.TabIndex = 57;
            this.txtBoxPeriod.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxPeriod.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // txtBoxNote
            // 
            this.txtBoxNote.AllowDrop = true;
            this.txtBoxNote.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFNote", true));
            this.txtBoxNote.Location = new System.Drawing.Point(272, 81);
            this.txtBoxNote.Name = "txtBoxNote";
            this.txtBoxNote.ReadOnly = true;
            this.txtBoxNote.Size = new System.Drawing.Size(40, 20);
            this.txtBoxNote.TabIndex = 56;
            this.txtBoxNote.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxNote.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // txtBoxQuantity
            // 
            this.txtBoxQuantity.AllowDrop = true;
            this.txtBoxQuantity.Location = new System.Drawing.Point(272, 57);
            this.txtBoxQuantity.Name = "txtBoxQuantity";
            this.txtBoxQuantity.ReadOnly = true;
            this.txtBoxQuantity.Size = new System.Drawing.Size(40, 20);
            this.txtBoxQuantity.TabIndex = 55;
            this.txtBoxQuantity.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxQuantity.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // txtBoxUnit
            // 
            this.txtBoxUnit.AllowDrop = true;
            this.txtBoxUnit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFUnit", true));
            this.txtBoxUnit.Location = new System.Drawing.Point(272, 9);
            this.txtBoxUnit.Name = "txtBoxUnit";
            this.txtBoxUnit.ReadOnly = true;
            this.txtBoxUnit.Size = new System.Drawing.Size(40, 20);
            this.txtBoxUnit.TabIndex = 54;
            this.txtBoxUnit.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxUnit.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // txtBoxVolume
            // 
            this.txtBoxVolume.AllowDrop = true;
            this.txtBoxVolume.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFVolume", true));
            this.txtBoxVolume.Location = new System.Drawing.Point(272, 33);
            this.txtBoxVolume.Name = "txtBoxVolume";
            this.txtBoxVolume.ReadOnly = true;
            this.txtBoxVolume.Size = new System.Drawing.Size(40, 20);
            this.txtBoxVolume.TabIndex = 53;
            this.txtBoxVolume.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxVolume.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(168, 129);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 23);
            this.label12.TabIndex = 52;
            this.label12.Text = "Документ :";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(168, 105);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(100, 23);
            this.label11.TabIndex = 51;
            this.label11.Text = "Срок годности :";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(168, 81);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 23);
            this.label10.TabIndex = 50;
            this.label10.Text = "Примечание :";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(168, 57);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 23);
            this.label9.TabIndex = 49;
            this.label9.Text = "Количество :";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(168, 33);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 23);
            this.label8.TabIndex = 48;
            this.label8.Text = "Цех. уп. :";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(168, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 23);
            this.label7.TabIndex = 47;
            this.label7.Text = "Ед. измерения :";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBoxFirmCr
            // 
            this.txtBoxFirmCr.AllowDrop = true;
            this.txtBoxFirmCr.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFFirmCr", true));
            this.txtBoxFirmCr.Location = new System.Drawing.Point(112, 129);
            this.txtBoxFirmCr.Name = "txtBoxFirmCr";
            this.txtBoxFirmCr.ReadOnly = true;
            this.txtBoxFirmCr.Size = new System.Drawing.Size(40, 20);
            this.txtBoxFirmCr.TabIndex = 46;
            this.txtBoxFirmCr.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxFirmCr.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // txtBoxName3
            // 
            this.txtBoxName3.AllowDrop = true;
            this.txtBoxName3.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFName3", true));
            this.txtBoxName3.Location = new System.Drawing.Point(112, 105);
            this.txtBoxName3.Name = "txtBoxName3";
            this.txtBoxName3.ReadOnly = true;
            this.txtBoxName3.Size = new System.Drawing.Size(40, 20);
            this.txtBoxName3.TabIndex = 45;
            this.txtBoxName3.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxName3.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // txtBoxName2
            // 
            this.txtBoxName2.AllowDrop = true;
            this.txtBoxName2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFName2", true));
            this.txtBoxName2.Location = new System.Drawing.Point(112, 81);
            this.txtBoxName2.Name = "txtBoxName2";
            this.txtBoxName2.ReadOnly = true;
            this.txtBoxName2.Size = new System.Drawing.Size(40, 20);
            this.txtBoxName2.TabIndex = 44;
            this.txtBoxName2.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxName2.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // txtBoxName1
            // 
            this.txtBoxName1.AllowDrop = true;
            this.txtBoxName1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFName1", true));
            this.txtBoxName1.Location = new System.Drawing.Point(112, 57);
            this.txtBoxName1.Name = "txtBoxName1";
            this.txtBoxName1.ReadOnly = true;
            this.txtBoxName1.Size = new System.Drawing.Size(40, 20);
            this.txtBoxName1.TabIndex = 43;
            this.txtBoxName1.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxName1.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // txtBoxCodeCr
            // 
            this.txtBoxCodeCr.AllowDrop = true;
            this.txtBoxCodeCr.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFCodeCr", true));
            this.txtBoxCodeCr.Location = new System.Drawing.Point(112, 33);
            this.txtBoxCodeCr.Name = "txtBoxCodeCr";
            this.txtBoxCodeCr.ReadOnly = true;
            this.txtBoxCodeCr.Size = new System.Drawing.Size(40, 20);
            this.txtBoxCodeCr.TabIndex = 42;
            this.txtBoxCodeCr.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxCodeCr.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // txtBoxCode
            // 
            this.txtBoxCode.AllowDrop = true;
            this.txtBoxCode.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRFCode", true));
            this.txtBoxCode.Location = new System.Drawing.Point(112, 9);
            this.txtBoxCode.Name = "txtBoxCode";
            this.txtBoxCode.ReadOnly = true;
            this.txtBoxCode.Size = new System.Drawing.Size(40, 20);
            this.txtBoxCode.TabIndex = 41;
            this.txtBoxCode.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragDrop);
            this.txtBoxCode.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxCode_DragEnter);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(8, 129);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 23);
            this.label6.TabIndex = 40;
            this.label6.Text = "Производитель :";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 23);
            this.label5.TabIndex = 39;
            this.label5.Text = "Наименование 3 :";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 23);
            this.label4.TabIndex = 38;
            this.label4.Text = "Наименование 2 :";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 37;
            this.label3.Text = "Наименование 1 :";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 36;
            this.label2.Text = "Код производ. :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 35;
            this.label1.Text = "Код :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // grpbSettings
            // 
            this.grpbSettings.Controls.Add(this.pnlSettings);
            this.grpbSettings.Dock = System.Windows.Forms.DockStyle.Left;
            this.grpbSettings.Location = new System.Drawing.Point(0, 0);
            this.grpbSettings.Name = "grpbSettings";
            this.grpbSettings.Size = new System.Drawing.Size(246, 184);
            this.grpbSettings.TabIndex = 1;
            this.grpbSettings.TabStop = false;
            this.grpbSettings.Text = "Настройки";
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.btnEditMask);
            this.pnlSettings.Controls.Add(this.btnCheckAll);
            this.pnlSettings.Controls.Add(this.btnJunkCheck);
            this.pnlSettings.Controls.Add(this.btnAwaitCheck);
            this.pnlSettings.Controls.Add(this.label23);
            this.pnlSettings.Controls.Add(this.label22);
            this.pnlSettings.Controls.Add(this.txtBoxSheetName);
            this.pnlSettings.Controls.Add(this.txtBoxStartLine);
            this.pnlSettings.Controls.Add(this.label21);
            this.pnlSettings.Controls.Add(this.label20);
            this.pnlSettings.Controls.Add(this.label19);
            this.pnlSettings.Controls.Add(this.label18);
            this.pnlSettings.Controls.Add(this.txtBoxSelfJunkPos);
            this.pnlSettings.Controls.Add(this.txtBoxSelfAwaitPos);
            this.pnlSettings.Controls.Add(this.txtBoxForbWords);
            this.pnlSettings.Controls.Add(this.txtBoxNameMask);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSettings.Location = new System.Drawing.Point(3, 16);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(240, 165);
            this.pnlSettings.TabIndex = 0;
            // 
            // btnEditMask
            // 
            this.btnEditMask.Image = ((System.Drawing.Image)(resources.GetObject("btnEditMask.Image")));
            this.btnEditMask.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnEditMask.Location = new System.Drawing.Point(184, 8);
            this.btnEditMask.Name = "btnEditMask";
            this.btnEditMask.Size = new System.Drawing.Size(24, 20);
            this.btnEditMask.TabIndex = 23;
            this.ttMain.SetToolTip(this.btnEditMask, "Настройка маски разбора");
            this.btnEditMask.Click += new System.EventHandler(this.btnEditMask_Click);
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Image = ((System.Drawing.Image)(resources.GetObject("btnCheckAll.Image")));
            this.btnCheckAll.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnCheckAll.Location = new System.Drawing.Point(208, 8);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(24, 20);
            this.btnCheckAll.TabIndex = 22;
            this.ttMain.SetToolTip(this.btnCheckAll, "Проверка маски разбора");
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // btnJunkCheck
            // 
            this.btnJunkCheck.Image = ((System.Drawing.Image)(resources.GetObject("btnJunkCheck.Image")));
            this.btnJunkCheck.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnJunkCheck.Location = new System.Drawing.Point(208, 80);
            this.btnJunkCheck.Name = "btnJunkCheck";
            this.btnJunkCheck.Size = new System.Drawing.Size(24, 20);
            this.btnJunkCheck.TabIndex = 21;
            this.ttMain.SetToolTip(this.btnJunkCheck, "Проверка маски сроковых");
            this.btnJunkCheck.Click += new System.EventHandler(this.btnJunkCheck_Click);
            // 
            // btnAwaitCheck
            // 
            this.btnAwaitCheck.Image = ((System.Drawing.Image)(resources.GetObject("btnAwaitCheck.Image")));
            this.btnAwaitCheck.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnAwaitCheck.Location = new System.Drawing.Point(208, 56);
            this.btnAwaitCheck.Name = "btnAwaitCheck";
            this.btnAwaitCheck.Size = new System.Drawing.Size(24, 20);
            this.btnAwaitCheck.TabIndex = 20;
            this.ttMain.SetToolTip(this.btnAwaitCheck, "Проверка маски ожидаемых");
            this.btnAwaitCheck.Click += new System.EventHandler(this.btnAwaitCheck_Click);
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(44, 128);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(96, 23);
            this.label23.TabIndex = 19;
            this.label23.Text = "Название листа :";
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(64, 104);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(80, 23);
            this.label22.TabIndex = 18;
            this.label22.Text = "Старт-срока :";
            // 
            // txtBoxSheetName
            // 
            this.txtBoxSheetName.AllowDrop = true;
            this.txtBoxSheetName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRListName", true));
            this.txtBoxSheetName.Location = new System.Drawing.Point(144, 128);
            this.txtBoxSheetName.Name = "txtBoxSheetName";
            this.txtBoxSheetName.ReadOnly = true;
            this.txtBoxSheetName.Size = new System.Drawing.Size(88, 20);
            this.txtBoxSheetName.TabIndex = 17;
            this.txtBoxSheetName.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxSheetName_DragDrop);
            this.txtBoxSheetName.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxSheetName_DragEnter);
            // 
            // txtBoxStartLine
            // 
            this.txtBoxStartLine.AllowDrop = true;
            this.txtBoxStartLine.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRStartLine", true));
            this.txtBoxStartLine.Location = new System.Drawing.Point(144, 104);
            this.txtBoxStartLine.Name = "txtBoxStartLine";
            this.txtBoxStartLine.ReadOnly = true;
            this.txtBoxStartLine.Size = new System.Drawing.Size(88, 20);
            this.txtBoxStartLine.TabIndex = 16;
            this.txtBoxStartLine.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxStartLine_DragDrop);
            this.txtBoxStartLine.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxStartLine_DragEnter);
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(40, 80);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(104, 23);
            this.label21.TabIndex = 15;
            this.label21.Text = "Маска сроковых :";
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(32, 56);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(112, 23);
            this.label20.TabIndex = 14;
            this.label20.Text = "Маска ожидаемых :";
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(24, 32);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(120, 23);
            this.label19.TabIndex = 13;
            this.label19.Text = "Запрещённые слова :";
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(8, 8);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(136, 23);
            this.label18.TabIndex = 12;
            this.label18.Text = "Маска разбора товара :";
            // 
            // txtBoxSelfJunkPos
            // 
            this.txtBoxSelfJunkPos.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRSelfJunkPos", true));
            this.txtBoxSelfJunkPos.Location = new System.Drawing.Point(144, 80);
            this.txtBoxSelfJunkPos.Name = "txtBoxSelfJunkPos";
            this.txtBoxSelfJunkPos.Size = new System.Drawing.Size(64, 20);
            this.txtBoxSelfJunkPos.TabIndex = 11;
            // 
            // txtBoxSelfAwaitPos
            // 
            this.txtBoxSelfAwaitPos.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRSelfAwaitPos", true));
            this.txtBoxSelfAwaitPos.Location = new System.Drawing.Point(144, 56);
            this.txtBoxSelfAwaitPos.Name = "txtBoxSelfAwaitPos";
            this.txtBoxSelfAwaitPos.Size = new System.Drawing.Size(64, 20);
            this.txtBoxSelfAwaitPos.TabIndex = 10;
            // 
            // txtBoxForbWords
            // 
            this.txtBoxForbWords.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRForbWords", true));
            this.txtBoxForbWords.Location = new System.Drawing.Point(144, 32);
            this.txtBoxForbWords.Name = "txtBoxForbWords";
            this.txtBoxForbWords.Size = new System.Drawing.Size(88, 20);
            this.txtBoxForbWords.TabIndex = 9;
            // 
            // txtBoxNameMask
            // 
            this.txtBoxNameMask.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dtSet, "Поставщики.Поставщики-Прайсы.Прайсы-правила.FRNameMask", true));
            this.txtBoxNameMask.Location = new System.Drawing.Point(144, 8);
            this.txtBoxNameMask.Name = "txtBoxNameMask";
            this.txtBoxNameMask.Size = new System.Drawing.Size(40, 20);
            this.txtBoxNameMask.TabIndex = 8;
            // 
            // erP
            // 
            this.erP.ContainerControl = this;
            // 
            // tscMain
            // 
            this.tscMain.BottomToolStripPanelVisible = false;
            // 
            // tscMain.ContentPanel
            // 
            this.tscMain.ContentPanel.Controls.Add(this.tbControl);
            this.tscMain.ContentPanel.Size = new System.Drawing.Size(864, 728);
            this.tscMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tscMain.LeftToolStripPanelVisible = false;
            this.tscMain.Location = new System.Drawing.Point(0, 0);
            this.tscMain.Name = "tscMain";
            this.tscMain.RightToolStripPanelVisible = false;
            this.tscMain.Size = new System.Drawing.Size(864, 753);
            this.tscMain.TabIndex = 2;
            this.tscMain.Text = "toolStripContainer1";
            // 
            // tscMain.TopToolStripPanel
            // 
            this.tscMain.TopToolStripPanel.Controls.Add(this.tsApply);
            // 
            // tsApply
            // 
            this.tsApply.Dock = System.Windows.Forms.DockStyle.None;
            this.tsApply.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbApply,
            this.tsbCancel});
            this.tsApply.Location = new System.Drawing.Point(3, 0);
            this.tsApply.Name = "tsApply";
            this.tsApply.Size = new System.Drawing.Size(137, 25);
            this.tsApply.TabIndex = 0;
            // 
            // tsbApply
            // 
            this.tsbApply.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbApply.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbApply.Name = "tsbApply";
            this.tsbApply.Size = new System.Drawing.Size(66, 22);
            this.tsbApply.Text = "Применить";
            this.tsbApply.Click += new System.EventHandler(this.tsbApply_Click);
            // 
            // tsbCancel
            // 
            this.tsbCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbCancel.Image = ((System.Drawing.Image)(resources.GetObject("tsbCancel.Image")));
            this.tsbCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCancel.Name = "tsbCancel";
            this.tsbCancel.Size = new System.Drawing.Size(61, 22);
            this.tsbCancel.Text = "Отменить";
            this.tsbCancel.Click += new System.EventHandler(this.tsbCancel_Click);
            // 
            // mcmdUCostRules
            // 
            this.mcmdUCostRules.CommandText = "UPDATE farm.costformrules c SET\r\nFieldName = ?FieldName,\r\nTxtBegin = ?TxtBegin,\r\n" +
                "TxtEnd = ?TxtEnd\r\nWHERE c.PC_CostCode = ?CostCode";
            this.mcmdUCostRules.CommandTimeout = 0;
            this.mcmdUCostRules.CommandType = System.Data.CommandType.Text;
            this.mcmdUCostRules.Connection = null;
            this.mcmdUCostRules.Transaction = null;
            this.mcmdUCostRules.UpdatedRowSource = System.Data.UpdateRowSource.Both;
            // 
            // daCostRules
            // 
            this.daCostRules.DeleteCommand = null;
            this.daCostRules.InsertCommand = null;
            this.daCostRules.SelectCommand = null;
            this.daCostRules.TableMappings.AddRange(new System.Data.Common.DataTableMapping[] {
            new System.Data.Common.DataTableMapping("Table", "Table", new System.Data.Common.DataColumnMapping[0])});
            this.daCostRules.UpdateCommand = this.mcmdUCostRules;
            // 
            // mcmdUFormRules
            // 
            this.mcmdUFormRules.CommandText = null;
            this.mcmdUFormRules.CommandTimeout = 0;
            this.mcmdUFormRules.CommandType = System.Data.CommandType.Text;
            this.mcmdUFormRules.Connection = null;
            this.mcmdUFormRules.Transaction = null;
            this.mcmdUFormRules.UpdatedRowSource = System.Data.UpdateRowSource.Both;
            // 
            // daFormRules
            // 
            this.daFormRules.DeleteCommand = null;
            this.daFormRules.InsertCommand = null;
            this.daFormRules.SelectCommand = null;
            this.daFormRules.UpdateCommand = this.mcmdUFormRules;
            // 
            // frmFREMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(864, 753);
            this.Controls.Add(this.tscMain);
            this.KeyPreview = true;
            this.Name = "frmFREMain";
            this.Text = "Редактор Правил Формализации";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Closed += new System.EventHandler(this.Form1_Closed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmFREMain_KeyDown);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tbControl.ResumeLayout(false);
            this.tpFirms.ResumeLayout(false);
            this.pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PriceGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtClients)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPrices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPricesCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFormRules)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtPriceFMTs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCatalogCurrency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtMarking)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCostsFormRules)).EndInit();
            this.tpPrice.ResumeLayout(false);
            this.pnlFloat.ResumeLayout(false);
            this.grpbGeneral.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.grpbParent.ResumeLayout(false);
            this.grpbParent.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tcInnerTable.ResumeLayout(false);
            this.tbpTable.ResumeLayout(false);
            this.tcInnerSheets.ResumeLayout(false);
            this.tbpSheet1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PriceDataGrid)).EndInit();
            this.tbpMarking.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MarkingDataGrid)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CostsDataGrid)).EndInit();
            this.grpbFields.ResumeLayout(false);
            this.pnlTxtFields.ResumeLayout(false);
            this.pnlTxtFields.PerformLayout();
            this.pnlGeneralFields.ResumeLayout(false);
            this.pnlGeneralFields.PerformLayout();
            this.grpbSettings.ResumeLayout(false);
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.erP)).EndInit();
            this.tscMain.ContentPanel.ResumeLayout(false);
            this.tscMain.TopToolStripPanel.ResumeLayout(false);
            this.tscMain.TopToolStripPanel.PerformLayout();
            this.tscMain.ResumeLayout(false);
            this.tscMain.PerformLayout();
            this.tsApply.ResumeLayout(false);
            this.tsApply.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion


        private System.Windows.Forms.TabControl tbControl;
        private System.Windows.Forms.TabPage tpFirms;
        private System.Windows.Forms.TabPage tpPrice;
        private System.Windows.Forms.Panel pnlGrid;
        private INDataGrid.INDataGrid PriceGrid;
        private System.Data.DataSet dtSet;
        private System.Data.DataColumn CCode;
        private System.Data.DataColumn CShortName;
        private System.Data.DataColumn CRegion;
        private System.Data.DataColumn CFullName;
        private System.Data.DataTable dtPrices;
        private System.Data.DataColumn PFirmName;
        private System.Data.DataColumn PPriceCode;
        private System.Data.DataColumn PFirmCode;
        private System.Data.DataTable dtPricesCost;
        private System.Data.DataColumn PCPriceCode;
        private System.Data.DataColumn PCBaseCost;
        private System.Data.DataColumn PCCostCode;
        private System.Data.DataTable dtClients;
        private System.Data.DataColumn PCCostName;
        private System.Data.DataTable dtFormRules;
        private System.Data.DataColumn FRName;
        private System.Data.DataColumn FRCurrency;
        private System.Data.DataColumn FRFormat;
        private System.Data.DataColumn FRPosNum;
        private System.Data.DataColumn FRRules;
        private System.Data.DataColumn FRSynonyms;
        private System.Data.DataColumn FRManager;
        private System.Data.DataColumn FRPriceCode;
        private System.Data.DataColumn FRDelimiter;
        private System.Data.DataTable dtPriceFMTs;
        private System.Data.DataTable dtCatalogCurrency;
        private System.Data.DataColumn FMTFormat;
        private System.Data.DataColumn CCCurrency;
        private System.Windows.Forms.Panel panel1;
        private INDataGrid.INDataGridTableStyle ClientsTableStyle;
        private INDataGrid.INDataGridColorTextBoxColumn CClientCodeTextBoxColumn;
        private INDataGrid.INDataGridColorTextBoxColumn CClientNameTextBoxColumn;
        private INDataGrid.INDataGridColorTextBoxColumn CRegionNameTextBoxColumn;
        private INDataGrid.INDataGridTableStyle PricesTableStyle;
        private INDataGrid.INDataGridColorTextBoxColumn PFirmNameTextBoxColumn;
        private INDataGrid.INDataGridColorTextBoxColumn PPriceCodeTextBoxColumn;
        private INDataGrid.INDataGridTableStyle PricesCostTableStyle;
        private INDataGrid.INDataGridColorTextBoxColumn PCCostNameTextBoxColumn;
        private System.Windows.Forms.DataGridBoolColumn PCBaseCostBoolColumn;
        private INDataGrid.INDataGridColorTextBoxColumn PCCostCodeTextBoxColumn;
        private System.Data.DataColumn FRListName;
        private System.Data.DataColumn FRStartLine;
        private System.Data.DataTable dtMarking;
        private System.Data.DataColumn MNameField;
        private System.Data.DataColumn MBeginField;
        private System.Data.DataColumn MEndField;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.GroupBox grpbSettings;
        private System.Windows.Forms.Panel pnlSettings;
        private System.Data.DataColumn FRSelfAwaitPos;
        private System.Data.DataColumn FRSelfJunkPos;
        private System.Data.DataColumn FRNameMask;
        private System.Data.DataColumn FRForbWords;
        private System.Data.DataColumn FRTxtCodeBegin;
        private System.Data.DataColumn FRTxtCodeEnd;
        private System.Data.DataColumn FRTxtCodeCrBegin;
        private System.Data.DataColumn FRTxtCodeCrEnd;
        private System.Data.DataColumn FRTxtNameBegin;
        private System.Data.DataColumn FRTxtNameEnd;
        private System.Data.DataColumn FRTxtFirmCrBegin;
        private System.Data.DataColumn FRTxtFirmCrEnd;
        private System.Data.DataColumn FRTxtCountryCrBegin;
        private System.Data.DataColumn FRTxtCountryCrEnd;
        private System.Data.DataColumn FRTxtBaseCostBegin;
        private System.Data.DataColumn FRTxtBaseCostEnd;
        private System.Data.DataColumn FRTxtMinBoundCostBegin;
        private System.Data.DataColumn FRTxtMinBoundCostEnd;
        private System.Data.DataColumn FRTxtAsFactCostBegin;
        private System.Data.DataColumn FRTxtAsFactCostEnd;
        private System.Data.DataColumn FRTxt5DayCostBegin;
        private System.Data.DataColumn FRTxt5DayCostEnd;
        private System.Data.DataColumn FRTxt10DayCostBegin;
        private System.Data.DataColumn FRTxt10DayCostEnd;
        private System.Data.DataColumn FRTxt15DayCostBegin;
        private System.Data.DataColumn FRTxt15DayCostEnd;
        private System.Data.DataColumn FRTxt20DayCostBegin;
        private System.Data.DataColumn FRTxt20DayCostEnd;
        private System.Data.DataColumn FRTxt25DayCostBegin;
        private System.Data.DataColumn FRTxt25DayCostEnd;
        private System.Data.DataColumn FRTxt30DayCostBegin;
        private System.Data.DataColumn FRTxt30DayCostEnd;
        private System.Data.DataColumn FRTxt45DayCostBegin;
        private System.Data.DataColumn FRTxt45DayCostEnd;
        private System.Data.DataColumn FRTxtCurrencyBegin;
        private System.Data.DataColumn FRTxtCurrencyEnd;
        private System.Data.DataColumn FRTxtUnitBegin;
        private System.Data.DataColumn FRTxtUnitEnd;
        private System.Data.DataColumn FRTxtVolumeBegin;
        private System.Data.DataColumn FRTxtVolumeEnd;
        private System.Data.DataColumn FRTxtUpCostBegin;
        private System.Data.DataColumn FRTxtUpCostEnd;
        private System.Data.DataColumn FRTxtQuantityBegin;
        private System.Data.DataColumn FRTxtQuantityEnd;
        private System.Data.DataColumn FRTxtNoteBegin;
        private System.Data.DataColumn FRTxtNoteEnd;
        private System.Data.DataColumn FRTxtPeriodBegin;
        private System.Data.DataColumn FRTxtPeriodEnd;
        private System.Data.DataColumn FRTxtDocBegin;
        private System.Data.DataColumn FRTxtDocEnd;
        private System.Data.DataColumn FRTxtJunkBegin;
        private System.Data.DataColumn FRTxtJunkEnd;
        private System.Data.DataColumn FRTxtAwaitBegin;
        private System.Data.DataColumn FRTxtAwaitEnd;
        private System.Data.DataColumn FRTxtReservedBegin;
        private System.Data.DataColumn FRTxtReservedEnd;
        private System.Data.DataTable dtCostsFormRules;
        private System.Data.DataColumn CFRCost_Code;
        private System.Data.DataColumn CFRFieldName;
        private System.Data.DataColumn CFRTextBegin;
        private System.Data.DataColumn CFRTextEnd;
        private System.Data.DataColumn CFRfr_if;
        private System.Data.DataColumn FRFCode;
        private System.Data.DataColumn FRFCodeCr;
        private System.Data.DataColumn FRFName1;
        private System.Data.DataColumn FRFName2;
        private System.Data.DataColumn FRFName3;
        private System.Data.DataColumn FRFBaseCost;
        private System.Data.DataColumn FRFCurrency;
        private System.Data.DataColumn FRFUnit;
        private System.Data.DataColumn FRFVolume;
        private System.Data.DataColumn FRFQuantity;
        private System.Data.DataColumn FRFNote;
        private System.Data.DataColumn FRFPeriod;
        private System.Data.DataColumn FRFDoc;
        private System.Data.DataColumn FRFJunk;
        private System.Data.DataColumn FRFAwait;
        private System.Data.DataColumn FRFFirmCr;
        private System.Data.DataColumn FRFMinBoundCost;
        private System.Windows.Forms.GroupBox grpbFields;
        private System.Windows.Forms.Panel pnlTxtFields;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Panel pnlGeneralFields;
        private System.Windows.Forms.TextBox txtBoxAwait;
        private System.Windows.Forms.TextBox txtBoxJunk;
        private System.Windows.Forms.TextBox txtBoxMinBoundCost;
        private System.Windows.Forms.TextBox txtBoxCurrency;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtBoxDoc;
        private System.Windows.Forms.TextBox txtBoxPeriod;
        private System.Windows.Forms.TextBox txtBoxNote;
        private System.Windows.Forms.TextBox txtBoxQuantity;
        private System.Windows.Forms.TextBox txtBoxUnit;
        private System.Windows.Forms.TextBox txtBoxVolume;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtBoxFirmCr;
        private System.Windows.Forms.TextBox txtBoxName3;
        private System.Windows.Forms.TextBox txtBoxName2;
        private System.Windows.Forms.TextBox txtBoxName1;
        private System.Windows.Forms.TextBox txtBoxCodeCr;
        private System.Windows.Forms.TextBox txtBoxCode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBoxAwaitEnd;
        private System.Windows.Forms.TextBox txtBoxJunkEnd;
        private System.Windows.Forms.TextBox txtBoxAwaitBegin;
        private System.Windows.Forms.TextBox txtBoxJunkBegin;
        private System.Windows.Forms.TextBox txtBoxMinBoundCostEnd;
        private System.Windows.Forms.TextBox txtBoxMinBoundCostBegin;
        private System.Windows.Forms.TextBox txtBoxCurrencyEnd;
        private System.Windows.Forms.TextBox txtBoxDocEnd;
        private System.Windows.Forms.TextBox txtBoxPeriodEnd;
        private System.Windows.Forms.TextBox txtBoxNoteEnd;
        private System.Windows.Forms.TextBox txtBoxCurrencyBegin;
        private System.Windows.Forms.TextBox txtBoxDocBegin;
        private System.Windows.Forms.TextBox txtBoxPeriodBegin;
        private System.Windows.Forms.TextBox txtBoxNoteBegin;
        private System.Windows.Forms.TextBox txtBoxQuantityEnd;
        private System.Windows.Forms.TextBox txtBoxQuantityBegin;
        private System.Windows.Forms.TextBox txtBoxVolumeEnd;
        private System.Windows.Forms.TextBox txtBoxVolumeBegin;
        private System.Windows.Forms.TextBox txtBoxUnitEnd;
        private System.Windows.Forms.TextBox txtBoxUnitBegin;
        private System.Windows.Forms.TextBox txtBoxFirmCrEnd;
        private System.Windows.Forms.TextBox txtBoxFirmCrBegin;
        private System.Windows.Forms.TextBox txtBoxName1End;
        private System.Windows.Forms.TextBox txtBoxName1Begin;
        private System.Windows.Forms.TextBox txtBoxCodeCrEnd;
        private System.Windows.Forms.TextBox txtBoxCodeCrBegin;
        private System.Windows.Forms.TextBox txtBoxCodeEnd;
        private System.Windows.Forms.TextBox txtBoxCodeBegin;
        private INDataGrid.INDataGrid CostsDataGrid;
        private System.Data.DataColumn CFRCostName;
        private INDataGrid.INDataGridColorTextBoxColumn CFRCostCodeTextBoxColumn;
        private INDataGrid.INDataGridColorTextBoxColumn CFRCostNameTextBoxColumn;
        private INDataGrid.INDataGridTableStyle CostsFormRulesTableStyle;
        private System.Windows.Forms.TextBox txtBoxSheetName;
        private System.Windows.Forms.TextBox txtBoxStartLine;
        private System.Windows.Forms.TextBox txtBoxNameMask;
        private System.Windows.Forms.TextBox txtBoxForbWords;
        private System.Windows.Forms.TextBox txtBoxSelfAwaitPos;
        private System.Windows.Forms.TextBox txtBoxSelfJunkPos;
        private System.Windows.Forms.Button btnFloatPanel;
        private System.Windows.Forms.TabControl tcInnerTable;
        private System.Windows.Forms.TabPage tbpTable;
        private System.Windows.Forms.TabControl tcInnerSheets;
        private System.Windows.Forms.TabPage tbpSheet1;
        private INDataGrid.INDataGrid PriceDataGrid;
        private System.Windows.Forms.TabPage tbpMarking;
        private INDataGrid.INDataGrid MarkingDataGrid;
        private System.Windows.Forms.Panel pnlFloat;
        private System.Windows.Forms.GroupBox grpbGeneral;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox rtbArticle;
        private System.Windows.Forms.Label lblArticle;
        private System.Windows.Forms.LinkLabel lLblMaster;
        private System.Windows.Forms.Label lblMaster;
        private System.Windows.Forms.GroupBox grpbParent;
        private System.Windows.Forms.Label lblSynonyms;
        private System.Windows.Forms.Label lblRules;
        private System.Windows.Forms.TextBox tbSynonyms;
        private System.Windows.Forms.TextBox tbRules;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblDevider;
        private System.Windows.Forms.TextBox tbDevider;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.TextBox tbPosition;
        private System.Windows.Forms.Label lblFormat;
        private System.Windows.Forms.ComboBox cmbFormat;
        private System.Windows.Forms.Label lblMoney;
        private System.Windows.Forms.ComboBox cmbMoney;
        private System.Windows.Forms.Label lblPriceName;
        private System.Windows.Forms.Label lblNameFirm;
        private INDataGrid.INDataGridTableStyle PriceDataTableStyle;
        private INDataGrid.INDataGridTableStyle MarkingTableStyle;
        private System.Data.DataColumn FRPriceFile;
        private System.Data.DataColumn FRMemo;
        private System.Windows.Forms.Button btnAwaitCheck;
        private System.Windows.Forms.Button btnJunkCheck;
        private System.Windows.Forms.ErrorProvider erP;
        private System.Windows.Forms.Button btnCheckAll;
        private System.Windows.Forms.Button btnEditMask;
        private System.Windows.Forms.ToolStripContainer tscMain;
        private System.Windows.Forms.ToolStrip tsApply;
        private System.Windows.Forms.ToolStripButton tsbApply;
        private System.Windows.Forms.ToolStripButton tsbCancel;
        private MySql.Data.MySqlClient.MySqlCommand mcmdUCostRules;
        private MySql.Data.MySqlClient.MySqlDataAdapter daCostRules;
        private MySql.Data.MySqlClient.MySqlCommand mcmdUFormRules;
        private MySql.Data.MySqlClient.MySqlDataAdapter daFormRules;
        private System.Windows.Forms.ToolTip ttMain;
    }
}