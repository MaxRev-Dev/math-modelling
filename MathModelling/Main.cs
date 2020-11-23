using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MM.Abstractions;

namespace MM
{
    public partial class Main : Form
    {
        private readonly object _lockGate = new object();
        private readonly Dictionary<string, BaseMethod> _methodMap;
        private readonly Dictionary<string, FlowLayoutPanel> _panelMap;
        private FieldInfo _currentField;
        private NumericUpDown _currentFieldUI;
        private BaseMethod _currentMethod;
        private int _currentTimeLayer;
        private Form _preview;
        private bool _requireRedraw;
        private SeriesChartType _seriesType = SeriesChartType.Line;
        private Timer liveTimer;
        private Timer redrawTimer;
        private double[][] result2d;

        private double[][][] result3d;

        public Main()
        {
            InitializeComponent();
            _panelMap = new Dictionary<string, FlowLayoutPanel>();
            var methods = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.BaseType == typeof(BaseMethod));
            _methodMap = methods.Select(x => new
            { x.Name, Value = (BaseMethod)Activator.CreateInstance(x) })
                .OrderByDescending(c => c.Value.Priority)
                .ToDictionary(x => x.Name, x => x.Value);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            foreach (var method in _methodMap) MapProperties(method);

            var sct = new[] { SeriesChartType.Line, SeriesChartType.Spline };
            comboBox3.DataSource = sct;
            comboBox3.SelectedIndex = 0;
            comboBox3.SelectedIndexChanged += ChartTypeChanged;
            comboBox1.DataSource = _methodMap.Select(x => x.Key).ToArray();
            timeLayerValues.SelectedIndexChanged +=
                TimeLayerValues_SelectedIndexChanged;
            redrawTimer = new Timer { Interval = 500 };
            redrawTimer.Tick += (s, _) =>
            {
                if (!_requireRedraw) return;
                DoRedraw();
                _requireRedraw = false;
                redrawTimer.Stop();
            };
            redrawTimer.Start();
            liveTimer = new Timer { Interval = 100 };
            liveTimer.Tick += (s, _) =>
            {
                if (liveCheck.Checked)
                {
                    trackBar1.Value += 1;
                    if (trackBar1.Value == trackBar1.Maximum)
                        trackBar1.Value = trackBar1.Minimum;
                }
            };
            liveTimer.Start();
        }

        private void TimeLayerValues_SelectedIndexChanged(object sender,
            EventArgs e)
        {
            if (sender is ComboBox cb)
            {
                if (cb.SelectedIndex == -1) return;
                _currentTimeLayer = cb.SelectedIndex;
                VisualizeAs2D(_currentMethod,
                    result3d[result3d.Length - 1 - _currentTimeLayer]);
            }
        }

        private void ChartTypeChanged(object sender, EventArgs e)
        {
            if (!(sender is ComboBox s)) return;
            if (s.SelectedIndex == -1) return;

            _seriesType = (SeriesChartType)s.SelectedItem;
            _requireRedraw = true;
            redrawTimer.Start();
        }

        private void MapProperties(KeyValuePair<string, BaseMethod> method)
        {
            var panel = new FlowLayoutPanel
            {
                AutoScroll = true,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                Dock = DockStyle.Fill
            };
            panel.VerticalScroll.Enabled = true;
            Type t = method.Value.GetType();

            var fds = t.GetFields().Where(x =>
                    x.GetCustomAttributes<ReflectedUICoefsAttribute>().Any())
                .ToArray();
            foreach (FieldInfo field in fds)
            {
                var ruic =
                    field.GetCustomAttribute<ReflectedUICoefsAttribute>();
                var prec = ruic.P;
                var c = new FlowLayoutPanel();
                var lb = new Label { Text = field.Name.Trim('_') };
                var upDown = new NumericUpDown();
                if (field.Name.Contains("tau")) upDown.Name = "upDowntau";

                // 
                // label2
                // 
                lb.AutoSize = true;
                lb.Location = new Point(4, 22);
                lb.Size = new Size(80, 18);
                lb.TextAlign = ContentAlignment.MiddleCenter;
                lb.Font = new Font(label1.Font.Name, 15F);
                lb.TabIndex = 0;
                // 
                // numericUpDown1
                // 
                if (field.FieldType != typeof(int))
                {
                    upDown.DecimalPlaces = prec;
                    upDown.Increment = 1m / (decimal)Math.Pow(10, prec);
                }
                else
                {
                    upDown.Increment = 1;
                }

                upDown.Location = new Point(46, 14);
                upDown.Minimum = decimal.MinValue;
                upDown.Maximum = decimal.MaxValue;
                upDown.Size = new Size(100, 20);
                upDown.Text = field.GetValue(method.Value).ToString();
                upDown.Font = new Font(label1.Font.Name, 15F);
                c.Location = new Point(3, 3);
                c.Size = new Size(200, 35);
                c.Controls.Add(lb);
                c.Controls.Add(upDown);
                panel.Controls.Add(c);
                upDown.TextChanged += (s, e) =>
                {
                    try
                    {
                        field.SetValue(method.Value,
                            Convert.ChangeType(upDown.Text, field.FieldType));
                        if (field.Name.Contains("time"))
                            ResetTimelayerControl(true);

                        _requireRedraw = true;
                        redrawTimer?.Start();
                    }
                    catch
                    {
                        // ignored
                    }
                };
                if (lb.Text.Equals("tau"))
                    _currentFieldUI = upDown;
            }

            trackBar1.ValueChanged += OnTrackChange;
            _panelMap[method.Key] = panel;
        }

        private void OnTrackChange(object sender, EventArgs e)
        {
            if (_currentField == default) return;
            FieldInfo field = _currentField;
            field.SetValue(_currentMethod,
                Convert.ChangeType(trackBar1.Value, field.FieldType));
            _currentFieldUI.Value = trackBar1.Value;
            _currentFieldUI.Refresh();
            _requireRedraw = true;
            redrawTimer?.Start();
        }

        private void SelectForChange(FieldInfo fieldInfo, BaseMethod method)
        {
            var val = fieldInfo.Name.Trim('_');
            trackBarCurLab.Text = val;
            trackBar1.Minimum = 1;
            trackBar1.Maximum = 500;
            trackBar1.Value =
                (int)Convert.ChangeType(fieldInfo.GetValue(method),
                    TypeCode.Int32);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0) return;
            DoRedraw();
            var sel = (string)comboBox1.SelectedItem;
            panel3.Controls.Clear();
            panel3.Controls.Add(_panelMap[sel]);
        }

        private void DoRedraw()
        {
            if (comboBox1.SelectedIndex < 0) return;
            try
            {
                var sel = (string)comboBox1.SelectedItem;

                _currentFieldUI =
                    _panelMap[sel].Controls.Find("upDowntau", true)
                        .FirstOrDefault() as NumericUpDown;
                lock (_lockGate)
                {
                    MethodAction(_methodMap[sel]);
                }
            }
            catch
            {
                // ignored
            }
        }

        private void MethodAction(BaseMethod method)
        {
            comboBox2.SelectedIndexChanged -= comboBox2_SelectedIndexChanged;
            is3dcheck.Visible =
            timeLayerBox.Visible = method.Is3D;
            _currentMethod = method;
            Type t = method.GetType();
            var fds = t.GetFields().Where(x =>
                    x.GetCustomAttributes<ReflectedUICoefsAttribute>().Any())
                .ToList();
            _currentField = fds.FirstOrDefault(x =>
                                x.GetCustomAttributes<DefaultModAttribute>()
                                    .Any()) ??
                            fds.FirstOrDefault(x => x.Name.Contains("tau"));
            if (_currentField != default)
            {
                SelectForChange(_currentField, _currentMethod);
            }

            trackBox.Visible = _currentField != default &&
                               _currentField.Name.Contains("tau");


            var msCalc = method.GetType().GetMethods()
                .Where(x =>
                    x.GetCustomAttributes<ReflectedTargetAttribute>().Any())
                .ToArray();
            if (method.SwitchData == default && msCalc.Any())
            {
                var ms = msCalc
                    .Select(x =>
                        x.Name.StartsWith("Get") ? x.Name.Substring(3) : x.Name)
                    .ToArray();
                method.SwitchData = ms;
                method.SwitchItem = ms[0];
                comboBox2.DataSource = method.SwitchData;
            }

            if (method.SwitchData != default)
            {
                panel5.Visible = true;
                if (comboBox2.DataSource == default)
                {
                    _currentTimeLayer = 0;
                    comboBox2.DataSource = method.SwitchData;
                    comboBox2.SelectedItem = method.SwitchItem;
                }
                else
                {
                    comboBox2.SelectedItem = method.SwitchItem;
                }
            }
            else
            {
                panel5.Visible = false;
                comboBox2.DataSource = default;
            }

            result2d = default;
            result3d = default;
            if (method.Is3D)
                BindChart3D(msCalc, method);
            else
                BindChart2D(msCalc, method);
        }


        private void BindChart3D(MethodInfo[] msCalc, BaseMethod method)
        {
            if (msCalc.Any())
                result3d = (double[][][])msCalc
                    .First(x => x.Name.Contains(method.SwitchItem))
                    .Invoke(method, Array.Empty<object>());
            else
                result3d = method.Calculate3D();

            ResetTimelayerControl();

            VisualizeAs2D(method, result3d[_currentTimeLayer]);
        }

        private void ResetTimelayerControl(bool force = false)
        {
            if (timeLayerValues.DataSource == default || force)
            {
                if (!_currentMethod.Is3D) return;
                timeLayerValues.SelectedIndexChanged -=
                    TimeLayerValues_SelectedIndexChanged;
                timeLayerValues.DataSource = Enumerable
                    .Range(1, result3d.Length)
                    .Select(x => "t" + _currentMethod.StepTime * x)
                    .ToArray();
                timeLayerValues.SelectedIndexChanged +=
                    TimeLayerValues_SelectedIndexChanged;
            }
        }

        private string GetTextFor3D(BaseMethod method, double[][][] doubles)
        {
            var sb = new StringBuilder();
            var tau = method.ChartStepY ?? 1;
            var i = 1;
            foreach (var layer in doubles.Reverse())
            {
                sb.Append("------------");
                sb.Append("time layer ");
                sb.Append(tau * i++);
                sb.AppendLine("------------");
                sb.AppendLine(method.AsString(layer.Reverse().ToArray()));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private void BindChart2D(MethodInfo[] msCalc, BaseMethod method)
        {
            if (msCalc.Any())
                result2d = (double[][])msCalc
                    .First(x => x.Name.Contains(method.SwitchItem))
                    .Invoke(method, Array.Empty<object>());
            else
                result2d = method.Calculate();

            VisualizeAs2D(method, result2d);
        }

        private void VisualizeAs2D(BaseMethod method, double[][] result)
        {
            var xv = Enumerable.Range(0, result[0].Length)
                .Select(x => x * method.ChartStepX).ToArray();
            var n = 0;

            chart1.Series.Clear();
            chart1.Series.SuspendUpdates();


            foreach (var array in result)
            {
                var s = new Series
                {
                    ChartType = method.SeriesType.HasValue ?
                        (_seriesType = method.SeriesType.Value)
                        : _seriesType,
                    BorderWidth = 3,
                    BackImageTransparentColor = Color.WhiteSmoke,
                    MarkerColor = Color.Blue
                };
                if (method.ChartStepY.HasValue)
                    if (method.Is3D)
                        s.Name = "h" + (result.Length - n++);
                    else
                    {
                        if (method.YLegend != default)
                        {
                            s.Name = method.YLegend[n++];
                        }
                        else
                        {
                            s.Name = "t" + method.ChartStepY * n++;
                        }
                    }

                if (method.MaxX.HasValue)
                {
                    if (method.SwapAxis)
                        s.Points.DataBindXY(array, xv);
                    else
                        s.Points.DataBindXY(xv, array);
                }
                else
                {
                    if (method.SwapAxis)
                        s.Points.DataBindXY(array, xv);
                    else
                        s.Points.DataBindY(array);
                }

                chart1.Series.Add(s);
            }

            ChartArea ca = chart1.ChartAreas[0];
            if (method.MaxX.HasValue)
            {
                ca.AxisX.Minimum = 0;
                ca.AxisX.Maximum = method.MaxX.Value;
            }
            else
            {
                ca.AxisY.Minimum =
                    ca.AxisY.Maximum =
                        ca.AxisX.Minimum =
                            ca.AxisX.Maximum = double.NaN;
            }

            chart1.ResetAutoValues();
            chart1.Series.ResumeUpdates();
            richTextBox1.Text = GetInfo(_currentMethod,method.AsString(result.Reverse().ToArray())).ToString();
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.SelectedIndexChanged -= comboBox2_SelectedIndexChanged;

            var sel = (string)comboBox1.SelectedItem;
            _methodMap[sel].SwitchItem = (string)comboBox2.SelectedItem;
            DoRedraw();
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox cb)
                chart1.ChartAreas[0].Area3DStyle.Enable3D = cb.Checked;
        }

        private void richTextBox1_MouseDoubleClick(object sender,
            MouseEventArgs e)
        {
            _preview = new Form { Name = "Preview", Size = Size };
            var textTable = result2d != default
                ? GetTextFor2D(_currentMethod, result2d)
                : GetTextFor3D(_currentMethod, result3d);
            var info = GetInfo(_currentMethod, textTable);

            var p = new RichTextBox
            {
                Font = new Font(Font.Name, 15F),
                Dock = DockStyle.Fill,
                Text = info.ToString()
            };
            _preview.Controls.Add(p);
            _preview.Show(this);
        }

        private StringBuilder GetInfo(BaseMethod currentMethod, string textTable)
        {
            var info = _currentMethod.Info;
            if (info.Length > 0)
            {
                info.Insert(0, "----- info -----\n");
                info.AppendLine("--- !!! info !!! ---");
            }
            info.AppendLine(textTable);
            return info;
        }

        private string GetTextFor2D(BaseMethod method, double[][] result)
        {
            return method.AsString(result.Reverse().ToArray());
        }
    }
}