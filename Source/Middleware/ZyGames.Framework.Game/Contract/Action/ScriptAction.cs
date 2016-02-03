/****************************************************************************
Copyright (c) 2013-2015 scutgame.com

http://www.scutgame.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/

using ZyGames.Framework.Game.Lang;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.RPC.Sockets;

namespace ZyGames.Framework.Game.Contract.Action {
    /// <summary>
    /// 
    /// </summary>
    public enum ScriptType {
        /// <summary>
        /// 
        /// </summary>
        Csharp,
        /// <summary>
        /// 
        /// </summary>
        Python,
        /// <summary>
        /// 
        /// </summary>
        Lua
    }
    /// <summary>
    /// 
    /// </summary>
    public class JsonScriptAction : ScriptAction {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scriptType"></param>
        /// <param name="actionId"></param>
        /// <param name="actionGetter"></param>
        /// <param name="scriptScope"></param>
        /// <param name="ignoreAuthorize"></param>
        public JsonScriptAction(ScriptType scriptType, int actionId, ActionGetter actionGetter, object scriptScope, bool ignoreAuthorize)
            : base(scriptType, actionId, actionGetter, scriptScope, ignoreAuthorize) {
            EnableWebSocket = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string BuildJsonPack() {
            return scriptScope.buildPacket(urlParam, actionResult);
        }
    }

    /// <summary>
    /// 提供脚本支持
    /// </summary>
    public class ScriptAction : AuthorizeAction {
        /// <summary>
        /// The _script scope.
        /// </summary>
        protected readonly dynamic scriptScope;
        private readonly bool ignoreAuthorize;
        /// <summary>
        /// The _url parameter.
        /// </summary>
        protected dynamic urlParam;
        /// <summary>
        /// The _action result.
        /// </summary>
        protected dynamic actionResult;
        private ScriptType scriptType;

        /// <summary>
        /// /
        /// </summary>
        /// <param name="scriptType"></param>
        /// <param name="actionId"></param>
        /// <param name="actionGetter"></param>
        /// <param name="scriptScope"></param>
        /// <param name="ignoreAuthorize">忽略授权</param>
        public ScriptAction(ScriptType scriptType, object actionId, ActionGetter actionGetter, object scriptScope, bool ignoreAuthorize)
            : base(actionId, actionGetter) {
            this.scriptType = scriptType;
            this.scriptScope = scriptScope;
            this.ignoreAuthorize = ignoreAuthorize;
        }
        /// <summary>
        /// 子类实现
        /// </summary>
        protected override void InitChildAction() {
        }
        /// <summary>
        /// 接收用户请求的参数，并根据相应类进行检测
        /// </summary>
        /// <returns></returns>
        public override bool GetUrlElement() {
            urlParam = scriptScope.getUrlElement(actionGetter, this);
            return urlParam != null && urlParam.Result ? true : false;
        }
        /// <summary>
        /// 子类实现Action处理
        /// </summary>
        /// <returns></returns>
        public override bool TakeAction() {
            actionResult = scriptScope.takeAction(urlParam, this);
            return actionResult != null && actionResult.Result ? true : false;
        }
        /// <summary>
        /// 创建返回协议内容输出栈
        /// </summary>
        public override void BuildPacket() {
            bool result = scriptScope.buildPacket(dataStruct, urlParam, actionResult);
            if (!result) {
                ErrorCode = Language.Instance.ErrorCode;
                if (IsRelease) {
                    ErrorInfo = Language.Instance.ServerBusy;
                }
            }
        }
        /// <summary>
        /// 不检查的ActionID
        /// </summary>
        /// <value><c>true</c> if ignore action identifier; otherwise, <c>false</c>.</value>
        protected override bool IgnoreActionId {
            get { return ignoreAuthorize; }
        }
    }
}