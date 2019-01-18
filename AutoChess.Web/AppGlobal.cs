using AutoChess.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AutoChess.Web
{
    /// <summary>
    /// 业务全局
    /// </summary>
    public class AppGlobal
    {
        /// <summary>配置</summary>
        public static Config AppConfig { get; set; } = new Config();

        /// <summary>自走棋加成</summary>
        public static string ACAddition { get; set; } = "";

        /// <summary>自走棋旗子</summary>
        public static string ACChessPieces { get; set; } = "";

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            try
            {
                var path = $"{HttpRuntime.AppDomainAppPath}/App_Data/AppConfig.ini";
                if (File.Exists(path))
                {
                    var str = File.ReadAllText(path, Encoding.UTF8);
                    AppConfig = JsonConvert.DeserializeObject<Config>(str) ?? new Config();
                }
                else
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(AppConfig, Formatting.Indented), Encoding.UTF8);
                }
                ReadAutoChessConfig();

                Task.Run(() =>
                {
                    while (true)
                    {
                        System.Threading.Thread.Sleep(5 * 60 * 1000);

                        var str = File.ReadAllText($"{HttpRuntime.AppDomainAppPath}/App_Data/AppConfig.ini", Encoding.UTF8);
                        AppConfig = JsonConvert.DeserializeObject<Config>(str) ?? new Config();
                        if (AppConfig.ACDataUpdate)
                        {
                            AppConfig.ACDataUpdate = false;
                            File.WriteAllText(path, JsonConvert.SerializeObject(AppConfig, Formatting.Indented), Encoding.UTF8);
                            ReadAutoChessConfig();
                        }
                    }
                });
            }
            catch { }
        }

        private static void ReadAutoChessConfig()
        {
            try
            {
                var path = $"{HttpRuntime.AppDomainAppPath}/App_Data/AutoChess/Addition.csv";
                if (File.Exists(path))
                {
                    ACAddition = File.ReadAllText(path, Encoding.UTF8);
                }

                path = $"{HttpRuntime.AppDomainAppPath}/App_Data/AutoChess/ChessPieces.csv";
                if (File.Exists(path))
                {
                    ACChessPieces = File.ReadAllText(path, Encoding.UTF8);
                }

            }
            catch
            {
                ACAddition = "";
                ACChessPieces = "";
            }
        }
    }
}