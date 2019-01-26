using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoChess.Web.Models
{
    /// <summary>
    /// 配置
    /// </summary>
    public class Config
    {
        /// <summary>自走棋客户端版本 小于最新版本需要下载新版本</summary>
        public int ACClientVerMin { get; set; } = 1000;

        /// <summary>自走棋客户端版本 低于对应版本需要更新数据源 并提示下载</summary>
        public int ACClientVer { get; set; } = 1000;

        /// <summary>自走棋客户端版本 低于对应版本需要更新数据源 不提示下载</summary>
        public int ACClientDataVer { get; set; } = 1000;

        /// <summary>更新提示</summary>
        public string ACUpdateTip { get; set; } = "该版本太低，请到***下载新版本！";

        /// <summary>更新数据并提示下载 成功提示</summary>
        public string ACUpdateSucceedTip { get; set; } = "数据源为2019.01.18版本";
        /// <summary>更新数据源成功提示</summary>
        public string ACUpdateDataTip { get; set; } = "数据源为2019.01.18版本";

        /// <summary>自走棋服务器数据源更新 服务会自动读取数据源</summary>
        public bool ACDataUpdate { get; set; } = false;
    }
}