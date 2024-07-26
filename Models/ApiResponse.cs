namespace LoanGeteway.Models
{
    public class ApiResponse<TData, TMessage>
    {
        public bool Success { get; set; }
        public TData Data { get; set; }
        public TMessage Message { get; set; }

        public static ApiResponse<TData, TMessage> SuccessObject(TData data, TMessage message)
        {
            return new ApiResponse<TData, TMessage> { Success = true, Data = data, Message = message };
        }
        public static ApiResponse<TData, TMessage> ErrorObject(TMessage message)
        {
            return new ApiResponse<TData, TMessage> { Success = false, Data = default, Message = message };
        }
    }

    public class ErrorDetail
    {
        public string Message { get; set; }
        public string ErrorCode { get; set; }
    }
  

}
