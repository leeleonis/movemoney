//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace BitcoinDeveloper.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public System.Guid id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CreateID { get; set; }
        public Nullable<System.DateTime> Createdt { get; set; }
        public string UpdateID { get; set; }
        public Nullable<System.DateTime> Updatedt { get; set; }
        public string DisableID { get; set; }
        public Nullable<System.DateTime> Disabledt { get; set; }
        public int Status { get; set; }
    }
}
