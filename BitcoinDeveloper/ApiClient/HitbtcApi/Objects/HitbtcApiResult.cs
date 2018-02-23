namespace HitbtcApi.Objects
{
    public class HitbtcApiResult<T>
    {
        public bool Status { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }

        public HitbtcApiResult()
        {
            Status = true;
        }
    }
}
