using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using Atomus.Service;
using Atomus.Control.Grid;
using Atomus.Control.Dictionary;
using Atomus.Control.AutoGeneration.Controllers;
using Atomus.Diagnostics;

namespace Atomus.Control.AutoGeneration
{
    public partial class SearchControl : UserControl, IAction
    {
        private AtomusControlEventHandler beforeActionEventHandler;
        private AtomusControlEventHandler afterActionEventHandler;
        private System.Windows.Forms.Control gridCongrol;
        private string actionSearchName;
        private DataTable procedureParameterInfo;
        private TableLayoutPanel tableLayoutPanel;
        private float searchControlHight;
        private int visibleCount;
        private int visibleRowCount;

        string serviceName;
        string connectionName;
        string info;
        string procedureSearch;

        #region Init
        public SearchControl()
        {
            InitializeComponent();

            this.visibleCount = 0;
        }

        private bool InitSearchControl()
        {
            Button button;
            System.Windows.Forms.Control control;
            //TextBox textBox;
            //DateTimePicker dateTimePicker;
            string parameterName;
            string[] searchControlColumnWidth;
            int searchControlColumnCount;
            bool addVisibleRow;

            int visibleControlCount;
            int totalVisibleRowCount;
            string tmp;
            string tmp1;

            int y;
            int x;

            this.searchControlHight = ((string)this.afterActionEventHandler?.Invoke(this, "SearchControl.Hight")).ToFloat();

            searchControlColumnWidth = ((string)this.afterActionEventHandler?.Invoke(this, "SearchControl.ColumnWidth")).Split(',');
            searchControlColumnCount = searchControlColumnWidth.Length;

            this.tableLayoutPanel = new TableLayoutPanel()
            {
                Name = "tableLayoutPanel1",
                //CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                ColumnCount = (searchControlColumnWidth.Length * 2) - 1//라벨, 컨트롤 쌍으로 컬럼 생성하고 마지막 여백 컬럼 라벨 제거
            };

            //마지막 컬럼은 제외(여백 컬럼)
            for (int j = 0; j < searchControlColumnCount - 1; j++)
            {
                this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

                if (searchControlColumnWidth[j].Contains("%"))
                    this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, searchControlColumnWidth[j].Replace("%", "").ToFloat()));
                else
                    this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, searchControlColumnWidth[j].ToFloat()));
            }


            //마지막 컬럼(여백 컬럼)
            if (searchControlColumnWidth[searchControlColumnCount - 1].Contains("%"))
                this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, searchControlColumnWidth[searchControlColumnCount - 1].Replace("%", "").ToFloat()));
            else
                this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, searchControlColumnWidth[searchControlColumnCount - 1].ToFloat()));


            visibleControlCount = 0;
            //라벨 이름 있는 항목 수
            foreach (DataRow _DataRowParameterInfo in this.procedureParameterInfo.Rows)
            {
                parameterName = (string)_DataRowParameterInfo["Parameter_name"];

                tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "Text"));

                if (tmp != null && tmp != "")
                {
                    visibleControlCount += 1;
                }
            }

            //전체 보이는 행 수
            totalVisibleRowCount = (int)Math.Round(visibleControlCount / (searchControlColumnCount - 1M));
            //최소로 보여지는 행 수
            this.visibleRowCount = ((string)this.afterActionEventHandler?.Invoke(this, "SearchControl.VisibleRowCount")).ToInt();


            //전체 보이는 행이 보여줄 최소로 보여지는 행보다 많으면 숨기기/보이기 버튼 추가
            if (totalVisibleRowCount > this.visibleRowCount)
            {
                this.visibleRowCount += 1;

                //조회 컨트롤 숨기기/보이기 버튼 행
                this.visibleCount += 1;

                this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, searchControlHight));

                button = new Button()
                {
                    Text = "▲",
                    Size = new System.Drawing.Size(20, 20),
                    Dock = DockStyle.Fill
                };
                button.Click += Button_Click;

                this.tableLayoutPanel.Controls.Add(button, 0, 0);
                this.tableLayoutPanel.SetColumnSpan(button, this.tableLayoutPanel.ColumnCount);
            }


            y = this.tableLayoutPanel.RowStyles.Count;
            x = 0;
            addVisibleRow = false;

            //라벨 이름 있는 항목만
            foreach (DataRow dataRowParameterInfo in this.procedureParameterInfo.Rows)
            {
                parameterName = (string)dataRowParameterInfo["Parameter_name"];

                tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "Text"));
                            
                if (tmp != null && tmp != "")
                {
                    addVisibleRow = false;

                    this.tableLayoutPanel.Controls.Add(
                        new Label()
                        {
                            AutoSize = true,
                            Text = tmp,
                            TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                            Padding = new Padding(10, 0, 0, 0),
                            Dock = DockStyle.Fill
                        }, x, y);

                    x += 1;


                    tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "ControlType"));

                    switch (tmp)
                    {
                        case "DateTime":
                            tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "Format"));

                            control = new DateTimePicker()
                            {
                                Name = parameterName,
                                Dock = DockStyle.Fill,
                                CustomFormat = (tmp != null && tmp != "") ? tmp : "",
                                Format = (tmp != null && tmp != "") ? DateTimePickerFormat.Custom : DateTimePickerFormat.Long,
                            };

                            tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "Add"));
                            if (tmp != null && tmp != "")
                                ((DateTimePicker)control).Value = ((DateTimePicker)control).Value.Add(new TimeSpan(long.Parse(tmp)));

                            tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "AddDays"));
                            if (tmp != null && tmp != "")
                                ((DateTimePicker)control).Value = ((DateTimePicker)control).Value.AddDays(double.Parse(tmp));

                            tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "AddHours"));
                            if (tmp != null && tmp != "")
                                ((DateTimePicker)control).Value = ((DateTimePicker)control).Value.AddHours(double.Parse(tmp));

                            tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "AddMilliseconds"));
                            if (tmp != null && tmp != "")
                                ((DateTimePicker)control).Value = ((DateTimePicker)control).Value.AddMilliseconds(double.Parse(tmp));

                            tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "AddMinutes"));
                            if (tmp != null && tmp != "")
                                ((DateTimePicker)control).Value = ((DateTimePicker)control).Value.AddMinutes(double.Parse(tmp));

                            tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "AddMonths"));
                            if (tmp != null && tmp != "")
                                ((DateTimePicker)control).Value = ((DateTimePicker)control).Value.AddMonths(int.Parse(tmp));

                            tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "AddSeconds"));
                            if (tmp != null && tmp != "")
                                ((DateTimePicker)control).Value = ((DateTimePicker)control).Value.AddSeconds(double.Parse(tmp));

                            tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "AddTicks"));
                            if (tmp != null && tmp != "")
                                ((DateTimePicker)control).Value = ((DateTimePicker)control).Value.AddTicks(long.Parse(tmp));

                            tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "AddYears"));
                            if (tmp != null && tmp != "")
                                ((DateTimePicker)control).Value = ((DateTimePicker)control).Value.AddYears(int.Parse(tmp));

                            break;

                        case "Check":
                            control = new CheckBox()
                            {
                                Name = parameterName,
                                Dock = DockStyle.Fill
                            };
                            
                            break;

                        default:
                            control = new TextBox()
                            {
                                Name = parameterName,
                                Dock = DockStyle.Fill
                            };

                            break;
                    }

                    this.tableLayoutPanel.Controls.Add(control, x, y);


                    tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "DefautValue"));

                    if (tmp != null & tmp != "")
                    {
                        if (control is DateTimePicker)
                        {
                            if (tmp.StartsWith("Atomus.Config.Client.GetAttribute("))
                                ((DateTimePicker)control).Value = DateTime.Parse(Config.Client.GetAttribute(tmp.Split("\"".ToCharArray())[1]).ToString());
                            else
                                ((DateTimePicker)control).Value = DateTime.Parse(tmp);
                        }
                        else if (control is CheckBox)
                        {
                            if (tmp.StartsWith("Atomus.Config.Client.GetAttribute("))
                                tmp = Config.Client.GetAttribute(tmp.Split("\"".ToCharArray())[1]).ToString();

                            tmp1 = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "CheckedValue"));
                            if (tmp1 != null && tmp1 != "")
                                ((CheckBox)control).Checked = (tmp == tmp1) ? true : false;
                        }
                        else
                        {
                            if (tmp.StartsWith("Atomus.Config.Client.GetAttribute("))
                                control.Text = Config.Client.GetAttribute(tmp.Split("\"".ToCharArray())[1]).ToString();
                            else
                                control.Text = tmp;
                        }
                    }


                    if (!(control is DateTimePicker))
                    {
                        tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "Dictionary"));

                        if (tmp != null & tmp != "")
                        {
                            tmp1 = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "DictionaryColumnIndex"));

                            if (tmp1 == null & tmp1 != "")
                                this.AddDictionary(tmp, 0, control);
                            else
                                this.AddDictionary(tmp, tmp1.ToInt(), control);
                        }

                    }


                    x += 1;

                    if (x % (this.tableLayoutPanel.ColumnCount - 1) == 0)
                    {
                        this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, searchControlHight));
                        this.visibleCount += 1;
                        y += 1;
                        x = 0;
                        addVisibleRow = true;
                    }
                }
            }

            if (addVisibleRow == false)
            {
                this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, searchControlHight));
                this.visibleCount += 1;
                y += 1;
            }

            x = 0;
            addVisibleRow = false;

            //라벨 이름 없는 항목만
            foreach (DataRow dataRowParameterInfo in this.procedureParameterInfo.Rows)
            {
                parameterName = (string)dataRowParameterInfo["Parameter_name"];

                tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "Text"));

                if (tmp == null || tmp == "")
                {
                    addVisibleRow = false;

                    control = new TextBox()
                    {
                        Name = parameterName,
                        Dock = DockStyle.Fill,
                        Enabled = false
                    };
                    this.tableLayoutPanel.Controls.Add(control, x, y);

                    tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "DefautValue"));

                    if (tmp != null & tmp != "")
                    {
                        if (tmp.StartsWith("Atomus.Config.Client.GetAttribute("))
                            control.Text = Config.Client.GetAttribute(tmp.Split("\"".ToCharArray())[1]).ToString();
                        else
                            control.Text = tmp;
                    }

                    x += 1;

                    if (x % (this.tableLayoutPanel.ColumnCount - 1) == 0)
                    {
                        this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, searchControlHight));
                        y += 1;
                        x = 0;
                        addVisibleRow = true;
                    }
                }
            }

            if (addVisibleRow == false)
                this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, searchControlHight));

            this.tableLayoutPanel.Height = this.visibleCount * (int)this.searchControlHight;

            this.Controls.Add(this.tableLayoutPanel);
            this.tableLayoutPanel.Dock = DockStyle.Top;

            return true;
        }

        ////두번째 행부터;  ;마지막 여백 컬럼 제외
        //for (int i = 1; i <= this._ProcedureParameterInfo.Rows.Count; i += (_SearchControlColumnCount - 1))
        //{
        //    //5개 파라미터
        //    //5개 컬럼폭 : 20%,20%,10%,50% (마지막은 여백 컬럼)
        //    //라벨1, 컨트롤1, 라벨2, 컨트롤2, 여백 컬럼
        //    //라벨3, 컨트롤3

        //    _Count = 0;//행에서 보이는 컨트롤 수 초기화

        //    //여백 컬럼 제외 = _SearchControlColumnCount -1
        //    for (int j = 0; j < _SearchControlColumnCount - 1; j++)
        //    {
        //        if (this._ProcedureParameterInfo.Rows.Count <= i - 1 + j)
        //            break;

        //        _Parameter_name = (string)this._ProcedureParameterInfo.Rows[i - 1 + j]["Parameter_name"];

        //        _TextBox = new TextBox();
        //        _TextBox.Name = _Parameter_name;
        //        this._TableLayoutPanel.Controls.Add(_TextBox, (j * 2) + 1, this._TableLayoutPanel.RowStyles.Count);//1,1 => 3,1 => 5,1
        //        _TextBox.Dock = DockStyle.Fill;


        //        _AtomusControlArgs.Action = string.Format("{0}.{1}", _Parameter_name, "Text");
        //        this._AfterActionEventHandler?.Invoke(this, _AtomusControlArgs);


        //        if (_AtomusControlArgs.Value != null & (string)_AtomusControlArgs.Value != "")
        //        {
        //            _Count += 1;//보이는 컨트롤

        //            _Label = new Label();
        //            _Label.AutoSize = true;
        //            _Label.Text = (string)_AtomusControlArgs.Value;
        //            _Label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        //            _Label.Padding = new Padding(10, 0, 0, 0);

        //            this._TableLayoutPanel.Controls.Add(_Label, (j * 2), this._TableLayoutPanel.RowStyles.Count);//0,1 => 2,1 => 4,1

        //            _Label.Dock = DockStyle.Fill;
        //        }
        //        else
        //        {
        //            _Label = new Label();
        //            _Label.AutoSize = true;
        //            _Label.Text = string.Format("{0},{1}", i, j);
        //            _Label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        //            _Label.Padding = new Padding(10, 0, 0, 0);

        //            this._TableLayoutPanel.Controls.Add(_Label, (j * 2), this._TableLayoutPanel.RowStyles.Count);//0,1 => 2,1 => 4,1

        //            _TextBox.Enabled = false;
        //        }


        //        _AtomusControlArgs.Action = string.Format("{0}.{1}", _Parameter_name, "DefautValue");
        //        this._AfterActionEventHandler?.Invoke(this, _AtomusControlArgs);

        //        _DefautValue = (string)_AtomusControlArgs.Value;

        //        if (_DefautValue != null & (string)_DefautValue != "")
        //        {
        //            if (_DefautValue.StartsWith("Atomus.Config.Client.GetAttribute("))
        //                _TextBox.Text = Config.Client.GetAttribute(_DefautValue.Split("\"".ToCharArray())[1]).ToString();
        //            else
        //                _TextBox.Text = (string)_AtomusControlArgs.Value;
        //        }


        //        _AtomusControlArgs.Action = string.Format("{0}.{1}", _Parameter_name, "Dictionary");
        //        this._AfterActionEventHandler?.Invoke(this, _AtomusControlArgs);

        //        if (_AtomusControlArgs.Value != null & (string)_AtomusControlArgs.Value != "")
        //        {
        //            _DictionaryCode = (string)_AtomusControlArgs.Value;

        //            _AtomusControlArgs.Action = string.Format("{0}.{1}", _Parameter_name, "DictionaryColumnIndex");
        //            this._AfterActionEventHandler?.Invoke(this, _AtomusControlArgs);

        //            if (_AtomusControlArgs.Value == null & (string)_AtomusControlArgs.Value != "")
        //                this.AddDictionary(_DictionaryCode, 0, _TextBox);
        //            else
        //                this.AddDictionary(_DictionaryCode, (int)_AtomusControlArgs.Value, _TextBox);
        //        }
        //    }

        //    //행에 보이는 컨트롤이 하나라도 있으면
        //    if (_Count > 0)
        //    {
        //        this._VisibleCount += 1;
        //        this._TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, _SearchControlHight));
        //    }
        //    else
        //        this._TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));





        //_Parameter_name = (string)this._ProcedureParameterInfo.Rows[i]["Parameter_name"];

        //_TextBox = new TextBox();
        //_TextBox.Name = _Parameter_name;
        //this._TableLayoutPanel.Controls.Add(_TextBox, 1, i + 1);
        //_TextBox.Dock = DockStyle.Fill;

        //_AtomusControlArgs.Action = string.Format("{0}.{1}", _Parameter_name, "Text");
        //this._AfterActionEventHandler?.Invoke(this, _AtomusControlArgs);

        //if (_AtomusControlArgs.Value != null & (string)_AtomusControlArgs.Value != "")
        //{
        //    this._VisibleCount += 1;

        //    _Label = new Label();
        //    _Label.AutoSize = false;
        //    _Label.Text = (string)_AtomusControlArgs.Value;
        //    _Label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

        //    this._TableLayoutPanel.Controls.Add(_Label, 0, i + 1);

        //    _Label.Dock = DockStyle.Fill;

        //    this._TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, _SearchControlHight));
        //}
        //else
        //{
        //    _TextBox.Visible = false;

        //    this._TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
        //}


        //_AtomusControlArgs.Action = string.Format("{0}.{1}", _Parameter_name, "DefautValue");
        //this._AfterActionEventHandler?.Invoke(this, _AtomusControlArgs);

        //_DefautValue = (string)_AtomusControlArgs.Value;

        //if (_DefautValue != null & (string)_DefautValue != "")
        //{
        //    if (_DefautValue.StartsWith("Atomus.Config.Client.GetAttribute("))
        //        _TextBox.Text = Config.Client.GetAttribute(_DefautValue.Split("\"".ToCharArray())[1]).ToString();
        //    else
        //        _TextBox.Text = (string)_AtomusControlArgs.Value;
        //}

        //_AtomusControlArgs.Action = string.Format("{0}.{1}", _Parameter_name, "Dictionary");
        //this._AfterActionEventHandler?.Invoke(this, _AtomusControlArgs);

        //if (_AtomusControlArgs.Value != null & (string)_AtomusControlArgs.Value != "")
        //{
        //    _DictionaryCode = (string)_AtomusControlArgs.Value;

        //    _AtomusControlArgs.Action = string.Format("{0}.{1}", _Parameter_name, "DictionaryColumnIndex");
        //    this._AfterActionEventHandler?.Invoke(this, _AtomusControlArgs);

        //    if (_AtomusControlArgs.Value == null & (string)_AtomusControlArgs.Value != "")
        //        this.AddDictionary(_DictionaryCode, 0, _TextBox);
        //    else
        //        this.AddDictionary(_DictionaryCode, (int)_AtomusControlArgs.Value, _TextBox);
        //}
        //}

        //this._TableLayoutPanel.Height = this._VisibleCount * (int)this._SearchControlHight;

        //this.Controls.Add(this._TableLayoutPanel);
        //this._TableLayoutPanel.Dock = DockStyle.Top;

        //return true;

        private bool InitControl()
        {
            return true;
        }
        #endregion

        #region Dictionary
        private void InitDictionary()
        {
        }
        private void AddDictionary(string dictionaryCode, int dictionaryColumnIndex, System.Windows.Forms.Control control)
        {
            IDictionary dictionary;
            System.Windows.Forms.Control[] controls;

            if (dictionaryCode == "")
                return;

            if (dictionaryColumnIndex < 0)
                return;

            controls = new System.Windows.Forms.Control[dictionaryColumnIndex + 1];
            controls[dictionaryColumnIndex] = control;

            dictionary = this.Dictionary();
            dictionary.WaterMark = true;
            dictionary.Add(dictionaryCode, this.BeforeAction, this.AfterAction, controls);
        }

        private bool BeforeAction(object sender, IBeforeEventArgs e)
        {
            //if (sender.Equals(this.PARENT_MENU_ID) || sender.Equals(this.PARENT_MENU_NAME))
            //{
            //    if (this.MENU_ID.Text.Equals(""))
            //        e.Where = "";
            //    else
            //        e.Where = string.Format("MENU_ID <> {0}", this.MENU_ID.Text);
            //}

            return true;
        }
        private bool AfterAction(object sender, IAfterEventArgs e)
        {
            //if (sender.Equals(this.ASSEMBLY_ID) || sender.Equals(this.NAMESPACE))
            //{
            //    if (e.DataRow != null)
            //    {
            //    }
            //}

            return true;
        }
        #endregion

        #region Spread
        //private async void InitGrid()
        //{
        //    DataTable _DataTable;
        //
        //    IDataGridAgent _GridAgent;
        //
        //    _GridAgent = this.DataGridAgent(this._GridCongrol);
        //    _GridAgent.Init( EditAble.False, AddRows.False, DeleteRows.False, ResizeRows.False, AutoSizeColumns.False, AutoSizeRows.False, ColumnsHeadersVisible.True, EnableMenu.True, MultiSelect.True, Alignment.MiddleCenter, -1, 1, -1, RowHeadersVisible.True, Selection.CellSelect);
        //
        //    _DataTable = await this.GetSampleSearchData();
        //
        //    this.SetGrid(_DataTable.DefaultView);
        //
        //
        //    //_GridAgent.AddColumn(60, ColumnVisible.True, EditAble.False, Filter.True, Merge.False, Sort.Automatic, null, Alignment.MiddleRight, string.Empty, "USER_ID", "User ID");
        //    //_GridAgent.AddColumn(200, ColumnVisible.True, EditAble.False, Filter.True, Merge.False, Sort.Automatic, null, Alignment.MiddleLeft, string.Empty, "EMAIL", "Email");
        //    //_GridAgent.AddColumn(200, ColumnVisible.True, EditAble.False, Filter.True, Merge.False, Sort.Automatic, null, Alignment.MiddleLeft, string.Empty, "NICKNAME", "Nickname");
        //    //_GridAgent.AddColumn(80, ColumnVisible.False, EditAble.False, Filter.True, Merge.False, Sort.Automatic, null, Alignment.MiddleRight, string.Empty, "RESPONSIBILITY_ID", "RESPONSIBILITY_ID");
        //    //_GridAgent.AddColumn(200, ColumnVisible.True, EditAble.False, Filter.True, Merge.False, Sort.Automatic, null, Alignment.MiddleLeft, string.Empty, "NAME", "Menu Permission");
        //    //_GridAgent.AddColumn(135, ColumnVisible.True, EditAble.False, Filter.True, Merge.False, Sort.Automatic, null, Alignment.MiddleCenter, string.Empty, "INACTIVE_DATE", "Inactive Date");
        //    //_GridAgent.AddColumn(60, ColumnVisible.True, EditAble.False, Filter.True, Merge.False, Sort.Automatic, null, Alignment.MiddleCenter, string.Empty, "IS_CONFIRM", "Confirm");
        //    //_GridAgent.AddColumn(60, ColumnVisible.True, EditAble.False, Filter.True, Merge.False, Sort.Automatic, null, Alignment.MiddleCenter, string.Empty, "IS_DELETE", "Deleted");
        //
        //    //_GridAgent.AddColumnFiter(SearchAll.False, StartsWith.False, AutoComplete.SuggestAppend, "EMAIL", this.EMAIL_Search);
        //    //_GridAgent.AddColumnFiter(SearchAll.False, StartsWith.False, AutoComplete.SuggestAppend, "NICKNAME", this.NICKNAME_Search);
        //
        //
        //}


        private async void InitGrid()
        {
            DataView dataView;
            IDataGridAgent gridAgent;
            Alignment textAlign;
            string tmp;
            string[] caption;
            string[] split;
            int width;
            int tmpIndex;
            ColumnVisible visible;
            string parameterName;
            System.Windows.Forms.Control[] controls;

            try
            {
                caption = new string[1];

                gridAgent = this.DataGridAgent(this.gridCongrol);
                //_GridAgent.GridControl = this._GridCongrol;
                gridAgent.Clear();

                gridAgent.Init(EditAble.False, AddRows.False, DeleteRows.False, ResizeRows.False, AutoSizeColumns.False, AutoSizeRows.False, ColumnsHeadersVisible.True, EnableMenu.True, MultiSelect.True, Alignment.MiddleCenter, -1, 1, -1, RowHeadersVisible.True, Selection.CellSelect);

                dataView = (await this.GetSampleSearchData()).DefaultView;

                tmpIndex = -1;
                foreach (DataColumn dataColumn in dataView.Table.Columns)
                {
                    tmpIndex += 1;

                    if (dataColumn.DataType.IsNumeric())//숫자일 경우에 오른쪽 정렬
                        textAlign = Alignment.MiddleRight;
                    else
                        textAlign = Alignment.MiddleLeft;

                    tmp = dataColumn.Caption;

                    if (tmp.Contains("^"))
                    {
                        split = tmp.Split('^');

                        caption[0] = split[0];
                        width = split[1].ToInt();
                        visible = ColumnVisible.True;
                    }
                    else
                    {
                        caption[0] = tmp;
                        width = 0;
                        visible = ColumnVisible.False;
                    }

                    gridAgent.AddColumn(width, visible, EditAble.False, Filter.False, Merge.False, Sort.NotSortable, null, textAlign, string.Empty, dataColumn.ColumnName, caption);
                }


                //라벨 이름 있는 항목 수
                foreach (DataRow _DataRowParameterInfo in this.procedureParameterInfo.Rows)
                {
                    parameterName = (string)_DataRowParameterInfo["Parameter_name"];

                    tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "AddColumnFiter"));

                    if (tmp != null && tmp != "")
                    {
                        controls = this.tableLayoutPanel.Controls.Find(parameterName, true);

                        if (controls != null && controls.Length == 1 && controls[0] is TextBox)
                            gridAgent.AddColumnFiter(SearchAll.False, StartsWith.False, AutoComplete.SuggestAppend, tmp, (TextBox)controls[0]);//컬럼 필터 추가
                    }
                }

                this.Controls.Add(this.gridCongrol);
                this.gridCongrol.Dock = DockStyle.Fill;
                this.gridCongrol.BringToFront();
            }
            catch (Exception exception)
            {
                DiagnosticsTool.MyTrace(exception);
            }
        }
        #endregion

        #region IO
        object IAction.ControlAction(ICore sender, AtomusControlArgs e)
        {
            try
            {
                this.beforeActionEventHandler?.Invoke(this, e);

                if (e.Action == actionSearchName)
                    return this.Search();


                if (e.Action.EndsWith(".DefautValue"))
                    return true;

                switch (e.Action)
                {
                    case "GridControl":
                        gridCongrol = (System.Windows.Forms.Control)e.Value;
                        return true;

                    case "Action.SearchName":
                        actionSearchName = (string)e.Value;
                        return true;
                    default:
                        throw new AtomusException(this.GetMessage("Common", "00047", "'{0}'은(는) 처리할 수 없는 {1} 입니다.").Message.Translate(e.Action, "Action"));
                }
            }
            finally
            {
                this.afterActionEventHandler?.Invoke(this, e);
            }
        }

        private async Task<bool> GetProcedureInfo()
        {
            IResponse result;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                result = await this.SearchProcedureInfo(new Models.ProcedureInfoModel()
                {
                    ServiceName = this.serviceName,
                    ConnectionName = this.connectionName,
                    Info = this.info,
                    ProcedureSearch = this.procedureSearch
                });

                if (result.Status == Status.OK)
                {
                    if (result.DataSet.Tables.Count == 2)
                    {
                        if (result.DataSet.Tables[0].Rows[0]["Type"].ToString() == "stored procedure")
                        {
                            this.procedureParameterInfo = result.DataSet.Tables[1];

                            this.InitSearchControl();
                            this.InitGrid();
                            return true;
                        }
                        else
                            throw new AtomusException(this.GetMessage("Common", "00010", "{0}가 없습니다.").Message.Translate("프로시저 정보"));
                    }
                    else
                        return false;
                }
                else
                {
                    this.MessageBoxShow(this, result.Message);
                    return false;
                }
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
                return false;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async Task<DataTable> GetSampleSearchData()
        {
            IResponse result;

            try
            {
                result = await this.SearchSampleSearchData(new Models.ProcedureInfoModel()
                {
                    ServiceName = this.serviceName,
                    ConnectionName = this.connectionName,
                    Info = this.info,
                    ProcedureSearch = this.procedureSearch,
                    ProcedureParameterInfo = this.procedureParameterInfo
                });

                if (result.Status == Status.OK)
                {
                    return result.DataSet.Tables[0];
                }
                else
                {
                    this.MessageBoxShow(this, result.Message);
                    return null;
                }
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
                return null;
            }
            finally
            {
            }
        }

        IErrorAlert errorAlert;
        private async Task<bool> Search()
        {
            IResponse result;
            object[] parameterValue;
            string parameterName;
            System.Windows.Forms.Control[] control;
            string tmp;

            try
            {
                this.gridCongrol.Cursor = Cursors.WaitCursor;

                this.errorAlert = this.errorAlert ?? this.ErrorAlert(false);
                this.errorAlert.Clear();

                parameterValue = new object[this.procedureParameterInfo.Rows.Count];

                for (int i = 0; i < this.procedureParameterInfo.Rows.Count; i++)
                {
                    parameterName = (string)this.procedureParameterInfo.Rows[i]["Parameter_name"];

                    control = this.tableLayoutPanel.Controls.Find(parameterName, true);

                    tmp = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "ErrorAlert"));

                    if (tmp != null && tmp == "TextLengthGreaterThan")
                        this.errorAlert.TextLengthGreaterThan(0, control);

                    if (control != null & control.Length == 1)
                    {
                        if (control[0] is DateTimePicker)
                            parameterValue[i] = ((DateTimePicker)control[0]).Value;
                        else if (control[0] is CheckBox)

                            if (((CheckBox)control[0]).Checked)
                            {
                                parameterValue[i] = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "CheckedValue"));
                            }
                            else
                            {
                                parameterValue[i] = (string)this.afterActionEventHandler?.Invoke(this, string.Format("{0}.{1}", parameterName, "UncheckedValue"));
                            }
                        else
                            parameterValue[i] = control[0].Text;
                    }
                }
                
                if (!this.errorAlert.Result)
                    return false;


                result = await this.SearchhData(new Models.ProcedureInfoModel()
                {
                    ServiceName = this.serviceName,
                    ConnectionName = this.connectionName,
                    Info = this.info,
                    ProcedureSearch = this.procedureSearch,
                    ProcedureParameterInfo = this.procedureParameterInfo,
                    ParameterValue = parameterValue
                });

                if (result.Status == Status.OK)
                {
                    ((dynamic)gridCongrol).DataSource = result.DataSet.Tables[0];
                    return true;
                }
                else
                {
                    this.MessageBoxShow(this, result.Message);
                    return false;
                }

            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
                return false;
            }
            finally
            {
                this.gridCongrol.Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Event
        event AtomusControlEventHandler IAction.BeforeActionEventHandler
        {
            add
            {
                this.beforeActionEventHandler += value;
            }
            remove
            {
                this.beforeActionEventHandler -= value;
            }
        }
        event AtomusControlEventHandler IAction.AfterActionEventHandler
        {
            add
            {
                this.afterActionEventHandler += value;
            }
            remove
            {
                this.afterActionEventHandler -= value;
            }
        }

        private async void SearchControl_Load(object sender, EventArgs e)
        {
            try
            {
                this.serviceName = (string)this.afterActionEventHandler?.Invoke(this, "ServiceName");
                this.connectionName = (string)this.afterActionEventHandler?.Invoke(this, "ConnectionName");
                this.info = (string)this.afterActionEventHandler?.Invoke(this, "ProcedureInfo");
                this.procedureSearch = (string)this.afterActionEventHandler?.Invoke(this, "ProcedureSearch");

                await this.GetProcedureInfo();

                //this.SetGrid((this._GridCongrol);

                this.InitControl();
                this.InitDictionary();
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button;

            button = (Button)sender;

            if (button.Text == "▲")
            {
                this.tableLayoutPanel.Height = this.visibleRowCount * (int)this.searchControlHight;//최소보이는 행 수 * 행높이
                button.Text = "▼";
            }
            else
            {
                this.tableLayoutPanel.Height = this.visibleCount * (int)this.searchControlHight;//전체보이는 행 수 * 행높이
                button.Text = "▲";
            }
        }
        #endregion
    }
}