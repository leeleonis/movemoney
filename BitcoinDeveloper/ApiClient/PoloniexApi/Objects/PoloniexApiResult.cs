namespace PoloniexApi.Objects
{
    public class PoloniexApiResult<T>
    {
        public bool Status { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }

        public PoloniexApiResult()
        {
            Status = true;
        }
    }
}
