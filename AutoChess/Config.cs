﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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

        string _Note = "";
        /// <summary>说明</summary>
        public string Note { get { return _Note; } set { SetProperty(ref _Note, value, nameof(Note)); } }

        string _Notice = "初始化完成";
        /// <summary>提示</summary>
        public string Notice { get { return _Notice; } set { SetProperty(ref _Notice, value, nameof(Notice)); } }

        /// <summary>版本</summary>
        public int Version { get; } = 1200;

        /// <summary>所有友方恶魔视为一个恶魔的数量</summary>
        public int DemonAssNum { get; set; } = 2;

        public void Init()
        {
            try
            {
                //var dir = $"{AppDomain.CurrentDomain.BaseDirectory}Data\\";
                //var path = dir + "ChessPieces.csv";
                List<string> linesChessPieces;
                List<string> linesAddition;
                //if (Directory.Exists(dir) == false)
                //{
                //    Directory.CreateDirectory(dir);
                //}
                //if (File.Exists(path))
                //{
                //    linesChessPieces = File.ReadAllLines(path, Encoding.UTF8).ToList();
                //}
                //else
                {
                    linesChessPieces = ConfigData.ChessPieces.Replace("\r\n", "\n").Split('\n').ToList();
                    //File.WriteAllText(path, ConfigData.ChessPieces, Encoding.UTF8);
                }


                //path = dir + "Addition.csv";
                //if (File.Exists(path))
                //{
                //    linesAddition = File.ReadAllLines(path, Encoding.UTF8).ToList();
                //}
                //else
                {
                    linesAddition = ConfigData.Addition.Replace("\r\n", "\n").Split('\n').ToList();
                    //File.WriteAllText(path, ConfigData.Addition, Encoding.UTF8);
                }

                InitData(linesChessPieces, linesAddition);
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化错误，请重新启动程序: " + ex.Message);
            }
        }
        /// <summary>
        /// 初始化加成和棋子的数据
        /// </summary>
        /// <param name="linesChessPieces"></param>
        /// <param name="linesAddition"></param>
        public void InitData(List<string> linesChessPieces, List<string> linesAddition)
        {
            ListChessPieces.Clear();
            foreach (var line in linesChessPieces)
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

            ListAddition.Clear();
            foreach (var line in linesAddition)
            {
                var strs = line?.Split(',');
                if (strs?.Length < 3)
                {
                    continue;
                }
                var i = 0;
                var add = new Addition()
                {
                    Name = strs[i++],
                    Number = int.Parse(strs[i++]),
                    Content = strs[i++],
                };
                ListAddition.Add(add);
                if (add.Content.Contains("所有友方恶魔为同一种类"))
                {
                    DemonAssNum = add.Number;
                }
            }

            ListChessPieces.Sort((s, s1) => s.Profession.CompareTo(s1.Profession));

            ListQuery = ListChessPieces.Select(b => b.QueryText).ToList();
        }


        /// <summary>
        /// get请求获取返回的html
        /// </summary>
        /// <param name="url">无参URL</param>
        /// <returns></returns>
        public string HttpGet(string url)
        {
            //if (querydata?.Count > 0)
            //{
            //    url += "?" + string.Join("&", querydata.Select(it => it.Key + "=" + it.Value));
            //}

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.CookieContainer = new CookieContainer();
            request.Accept = "*/*";
            request.Timeout = 5000;
            request.Proxy = new WebProxy();

            var response = request.GetResponse() as HttpWebResponse;
            using (Stream s = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();
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
        int _Sn = 0;
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
