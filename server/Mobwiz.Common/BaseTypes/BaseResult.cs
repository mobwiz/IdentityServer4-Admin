using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mobwiz.Common.BaseTypes
{
    /// <summary>
    /// Web结果
    /// </summary>
    public class BaseResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="messageArgs"></param>
        public BaseResult(int code = 0, string message = "", params string[] messageArgs)
        {
            Code = code;
            Message = message;
            MessageArgs = messageArgs;
        }

        /// <summary>
        /// 响应码，0 代表成功，其他值代表出错
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息模板
        /// </summary>        
        public string Message { get; set; }

        /// <summary>
        /// 消息模板参数
        /// </summary>
        [JsonIgnore]
        public string[] MessageArgs { get; set; }
    }

    /// <summary>
    /// 带数据的结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseResult<T> : BaseResult
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="messageArgs"></param>
        public BaseResult(T data, int code = 0, string message = "", params string[] messageArgs) : base(code, message, messageArgs)
        {
            Data = data;
        }
    }
}
