namespace HuobiApi.Objects
{
    public class HuobiApiResult<T>
    {
        public bool Status { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }

        public HuobiApiResult()
        {
            Status = true;
        }
    }
}
