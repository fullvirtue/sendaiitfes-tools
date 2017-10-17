//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ContentManagerModels.Entities.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Session
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Session()
        {
            this.Author = new HashSet<Author>();
        }
    
        public int SessionId { get; set; }
        public string SessionNo { get; set; }
        public string SessionNo1 { get; set; }
        public string SessionNo2 { get; set; }
        public string SessionNo3 { get; set; }
        public string Room { get; set; }
        public System.DateTime SessionStart { get; set; }
        public System.DateTime SessionEnd { get; set; }
        public int SessionGroupId { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public int SessionLevel { get; set; }
        public Nullable<int> TimetableOrder { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Author> Author { get; set; }
        public virtual SessionGroup SessionGroup { get; set; }
    }
}
