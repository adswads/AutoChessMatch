using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AutoChess.Web.Controllers
{
    /// <summary>
    /// 自走棋控制器
    /// </summary>
    public class AutoChessController : ApiController
    {
        // GET: api/AutoChess
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/AutoChess/5
        public string Get(int id)
        {
            var reStr = "";
            //传入的是当前版本号
            try
            {
                if (id < AppGlobal.AppConfig.ACClientVerMin)
                {
                    reStr = AppGlobal.AppConfig.ACUpdateTip;
                }
                else if (id < AppGlobal.AppConfig.ACClientVer)
                {
                    reStr = $"{AppGlobal.AppConfig.ACUpdateSucceedTip}|{AppGlobal.ACAddition}|{AppGlobal.ACChessPieces}";
                }
                else { }
            }
            catch { }
            return reStr;
        }
        // GET: api/AutoChess/5
        public string Get(string arg)
        {

            return "value";
        }

        // POST: api/AutoChess
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/AutoChess/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/AutoChess/5
        public void Delete(int id)
        {
        }
    }
}
