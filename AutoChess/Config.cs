using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoChess
{
    public class Config : BaseModel
    {
        /// <summary>所有的棋子</summary>
        public List<ChessPieces> ListChessPieces { get; set; } = new List<ChessPieces>();
        /// <summary>所有的羁绊</summary>
        public List<Addition> ListAddition { get; set; } = new List<Addition>();

        /// <summary>查询的列表</summary>
        public List<string> ListQuery { get; set; } = new List<string>();

        string _Note;
        /// <summary>说明</summary>
        public string Note { get { return _Note; } set { SetProperty(ref _Note, value, nameof(Note)); } }

        public void Init()
        {
            try
            {
                var dir = $"{AppDomain.CurrentDomain.BaseDirectory}Data\\";
                var path = dir + "ChessPieces.csv";
                List<string> lines;
                if (Directory.Exists(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }
                if (File.Exists(path))
                {
                    lines = File.ReadAllLines(path, Encoding.UTF8).ToList();
                }
                else
                {
                    lines = ConfigData.ChessPieces.Replace("\r\n", "\r").Split('\r').ToList();
                    File.WriteAllText(path, ConfigData.ChessPieces, Encoding.UTF8);
                }
                foreach (var line in lines)
                {
                    var strs = line?.Split(',');
                    if (strs?.Length < 7)
                    {
                        continue;
                    }
                    var i = 0;
                    ListChessPieces.Add(new ChessPieces()
                    {
                        Name = strs[i++],
                        Race1 = strs[i++],
                        Race2 = strs[i++],
                        Profession = strs[i++],
                        Level = strs[i++],
                        Attack = strs[i++],
                        Skill = strs[i++],
                    });
                }

                path = dir + "Addition.csv";
                if (File.Exists(path))
                {
                    lines = File.ReadAllLines(path, Encoding.UTF8).ToList();
                }
                else
                {
                    lines = ConfigData.Addition.Replace("\r\n", "\r").Split('\r').ToList();
                    File.WriteAllText(path, ConfigData.Addition, Encoding.UTF8);
                }
                foreach (var line in lines)
                {
                    var strs = line?.Split(',');
                    if (strs?.Length < 3)
                    {
                        continue;
                    }
                    var i = 0;
                    ListAddition.Add(new Addition()
                    {
                        Name = strs[i++],
                        Number = int.Parse(strs[i++]),
                        Content = strs[i++],
                    });
                }

                ListChessPieces.Sort((s, s1) => s.Profession.CompareTo(s1.Profession));

                ListQuery = ListChessPieces.Select(b => b.QueryText).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化错误，请重新启动程序: " + ex.Message);
            }
        }
    }
    /// <summary>
    /// 加成
    /// </summary>
    public class Addition
    {
        /// <summary>名称</summary>
        public string Name { get; set; } = "";
        /// <summary>数量</summary>
        public int Number { get; set; } = 0;
        /// <summary>内容</summary>
        public string Content { get; set; } = "";

        public override string ToString()
        {
            return $"{Name} {Number} {Content}";
        }
    }

    /// <summary>
    /// 棋子
    /// </summary>
    public class ChessPieces : BaseModel
    {
        int _Sn=0;
        /// <summary>序号</summary>
        public int Sn { get { return _Sn; } set { SetProperty(ref _Sn, value, nameof(Sn)); } }
        string _Name;
        /// <summary>名称</summary>
        public string Name { get { return _Name; } set { SetProperty(ref _Name, value, nameof(Name)); } }
        string _Race1;
        /// <summary>种族1</summary>
        public string Race1 { get { return _Race1; } set { SetProperty(ref _Race1, value, nameof(Race1)); } }
        string _Race2;
        /// <summary>种族2</summary>
        public string Race2 { get { return _Race2; } set { SetProperty(ref _Race2, value, nameof(Race2)); } }
        string _Profession;
        /// <summary>职业</summary>
        public string Profession { get { return _Profession; } set { SetProperty(ref _Profession, value, nameof(Profession)); } }
        string _Level;
        /// <summary>星级</summary>
        public string Level { get { return _Level; } set { SetProperty(ref _Level, value, nameof(Level)); } }
        string _Attack;
        /// <summary>攻击距离</summary>
        public string Attack { get { return _Attack; } set { SetProperty(ref _Attack, value, nameof(Attack)); } }
        string _Skill;
        /// <summary>技能</summary>
        public string Skill { get { return _Skill; } set { SetProperty(ref _Skill, value, nameof(Skill)); } }

        /// <summary>名称</summary>
        public string RaceShow { get { return string.IsNullOrWhiteSpace(Race2) ? Race1 : $"{Race1} {Race2}"; } }
        /// <summary>名称</summary>
        public string QueryText { get { return $"{_Name} {RaceShow} {Profession}"; } }

        public override string ToString()
        {
            return QueryText;
        }

        /// <summary>
        /// 根据名称去重
        /// </summary>
        public class CompareByName : IEqualityComparer<ChessPieces>
        {
            public bool Equals(ChessPieces x, ChessPieces y)
            {
                if (x == null || y == null)
                    return false;
                if (x.Name == y.Name)
                    return true;
                else
                    return false;
            }

            public int GetHashCode(ChessPieces obj)
            {
                if (obj == null)
                    return 0;
                else
                    return obj.Name.GetHashCode();
            }
        }
    }


    #region 引用库

    /// <summary>
    /// 自动实现刷新接口的模型基类
    /// </summary>
    public class BaseModel : INotifyPropertyChanged
    {
        /// <summary>属性通知</summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 自动改变属性
        /// </summary>
        /// <param name="propertyName"></param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /// <summary>
        /// 自动改变属性
        /// </summary>
        /// <param name="propertyNames"></param>
        public void NotifyPropertyChanged(List<string> propertyNames)
        {
            if (this.PropertyChanged != null)
            {
                foreach (string name in propertyNames)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        /// <summary>
        /// 自动添加通知
        ///  set {base.SetProperty(ref _ButtonText, value, nameof(ButtonText));}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propField"></param>
        /// <param name="value"></param>
        /// <param name="propName"></param>
        protected void SetProperty<T>(ref T propField, T value, string propName)
        {
            if (propField?.Equals(value) == true) { return; }
            propField = value;
            this.NotifyPropertyChanged(propName);
        }
    }
    #endregion

}
