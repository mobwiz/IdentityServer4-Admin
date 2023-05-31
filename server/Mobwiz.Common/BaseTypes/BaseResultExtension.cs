namespace Mobwiz.Common.BaseTypes
{
    public interface IApiResult { }

    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class BaseResultExtension
    {
        public static BaseResult OkResult(this IApiResult controller)
        {
            return new BaseResult(0, "success");
        }

        public static BaseResult<T> OkResult<T>(this IApiResult controller, T data)
        {
            return new BaseResult<T>(data);
        }

        public static BaseResult FailResult(this IApiResult controller, string message, params string[] messageArgs)
        {
            return new BaseResult(500, message, messageArgs);
        }

        public static BaseResult FailResult(this IApiResult controller, int errCode, string message, params string[] messageArgs)
        {
            return new BaseResult(errCode, message, messageArgs);
        }

        public static BaseResult<T> FailResult<T>(this IApiResult controller, int errCode, T data, string message, params string[] messageArgs)
        {
            return new BaseResult<T>(data, errCode, message, messageArgs);
        }
    }
}
