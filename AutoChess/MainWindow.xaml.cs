using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoChess
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        Config _gConfig = new Config();

        ObservableCollection<ChessPieces> _gListChessPieces = new ObservableCollection<ChessPieces>();
        ObservableCollection<string> _gListQuery = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();

            _gConfig.Init();

            _gConfig.ListQuery.ForEach(b => _gListQuery.Add(b));

            _vCardSelect.ItemsSource = _gListQuery;
            _vCards.ItemsSource = _gListChessPieces;
            //_vCardFetter.SetBinding(TextBox.TextProperty, new Binding(nameof(_gConfig.Note)) { Source = _gConfig, Mode = BindingMode.TwoWay });
            _vNotice.SetBinding(TextBox.TextProperty, new Binding(nameof(_gConfig.Notice)) { Source = _gConfig, Mode = BindingMode.TwoWay });
            //增加行号           ;
            //_vCards.LoadingRow += (s, e) => { e.Row.Header = e.Row.GetIndex() + 1; };

            _vAbout.Text = @"可自由修改使用，本人不对任何结果负责
有问题可到github上反馈给我
https://github.com/adswads/AutoChessMatch";

            _vCardSelect.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter && _vCardSelect.Text != "")
                {
                    _vCardMod_Click(null, null);
                }
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var str = _gConfig.HttpGet($"http://www.adswads.com:8222/api/AutoChess/{_gConfig.Version}");
                    str = str.Replace("\\\"", "\"").Trim('\"').Replace("\\r\\n", "\n");
                    if (string.IsNullOrWhiteSpace(str))
                    { }
                    else if (str.Contains("|"))
                    {
                        var strs = str.Split('|');
                        if (strs.Length == 3)
                        {
                            var linesAddition = strs[1].Replace("\r\n", "\n").Split('\n').ToList();
                            var linesChessPieces = strs[2].Replace("\r\n", "\n").Split('\n').ToList();
                            _gConfig.InitData(linesChessPieces, linesAddition);

                            Dispatcher.Invoke(new Action(() =>
                            {
                                _gListQuery.Clear();
                                _gConfig.ListQuery.ForEach(b => _gListQuery.Add(b));
                            }));
                            _gConfig.Notice = strs[0];
                        }
                    }
                    else
                    {
                        _gConfig.Notice = str;
                    }
                }
                catch { }
            });
        }

        private void _vCardSelect_TextChanged(object sender, TextChangedEventArgs e)
        {
            var list = _gConfig.ListQuery.Where(b => b.Contains((e.OriginalSource as TextBox)?.Text + "")).ToList();
            _gListQuery.Clear();
            list.ForEach(b =>
            {
                if (_gListChessPieces.FirstOrDefault(sb => sb.QueryText == b) == null)
                {
                    _gListQuery.Add(b);
                }
            });
            _vCardSelect.IsDropDownOpen = true;

            e.Handled = true;
        }
        /// <summary>
        /// 计算羁绊
        /// </summary>
        private void CalFetter()
        {
            var additions = new List<string>();

            var listChessPieces = _gListChessPieces.Distinct(new ChessPieces.CompareByName()).ToList();
            //获取所有加成
            foreach (var b in listChessPieces)
            {
                if (string.IsNullOrWhiteSpace(b.Race1) == false)
                {
                    additions.Add(b.Race1);
                }
                if (string.IsNullOrWhiteSpace(b.Race2) == false)
                {
                    additions.Add(b.Race2);
                }
                if (string.IsNullOrWhiteSpace(b.Profession) == false)
                {
                    additions.Add(b.Profession);
                }
            }
            //获取有效加成
            var classifys = additions.GroupBy(b => b).ToDictionary(b => b.Key, b => b.Count());
            var sb = new StringBuilder();

            //调整为富文本编辑框
            //当前有的效果
            _vCardFetter.Document.Blocks.Clear();
            var par = new Paragraph();
            //可以形成的效果
            var parhope = new Paragraph();
            foreach (var classify in classifys)
            {
                if (classify.Key == "恶魔" && (classify.Value > 1 && classifys.FirstOrDefault(b => b.Key == "恶魔猎手").Value < _gConfig.DemonAssNum))
                {
                    continue;
                }
                var list = _gConfig.ListAddition.Where(b => b.Name == classify.Key && b.Number <= classify.Value).ToList();
                list?.ForEach(b =>
                {
                    //sb.AppendLine(b.ToString());
                    par.Inlines.Add(new Run() { Text = $"{b.Name} {b.Number} ", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Blue) });
                    par.Inlines.Add(new Run() { Text = $"{b.Content}\r\n" });
                });

                list = _gConfig.ListAddition.Where(b => b.Name == classify.Key && b.Number > classify.Value && b.Number - classify.Value < 4).ToList();
                list?.ForEach(b =>
                {
                    parhope.Inlines.Add(new Run() { Text = $"{b.Name} {classifys[b.Name]}/{b.Number} ", FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.YellowGreen) });
                    parhope.Inlines.Add(new Run() { Text = $"{b.Content}\r\n", Foreground = new SolidColorBrush(Colors.DarkGray) });
                });
            }
            _vCardFetter.Document.Blocks.Add(par);
            _vCardFetter.Document.Blocks.Add(parhope);
            //当前可以形成的组合


            //_gConfig.Note = sb.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var pieces = _vCards.SelectedValue as ChessPieces;
                if (pieces != null)
                {
                    _gListChessPieces.Remove(pieces);
                    var i = 1;
                    foreach (var b in _gListChessPieces)
                    {
                        b.Sn = i++;
                    }
                    CalFetter();
                }
            }
            catch { }
        }

        private void _vCardMod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var pieces = _gConfig.ListChessPieces.FirstOrDefault(b => _vCardSelect.Text == b.QueryText);
                if (pieces != null && _gListChessPieces.Count < 10)
                {
                    _gListChessPieces.Add(pieces);
                    pieces.Sn = _gListChessPieces.Count;

                    CalFetter();

                    _vCardSelect.Text = "";
                }
            }
            catch { }
        }

        private void _vCardDel_Click(object sender, RoutedEventArgs e)
        {
            _gListChessPieces.Clear();
            CalFetter();
        }

    }
}
