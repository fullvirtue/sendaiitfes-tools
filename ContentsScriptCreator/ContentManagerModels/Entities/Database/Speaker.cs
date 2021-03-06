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
    
    public partial class Speaker
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Speaker()
        {
            this.Author = new HashSet<Author>();
        }
    
        public int SpeakerId { get; set; }
        public string SpeakerName { get; set; }
        public string Organization { get; set; }
        public string Organization2 { get; set; }
        public string Title { get; set; }
        public string Title2 { get; set; }
        public string ImageUrl { get; set; }
        public string Profile { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string Github { get; set; }
        public string Link { get; set; }
        public string email { get; set; }
        public string MSMVPExpertise { get; set; }
        public string AwardTitle { get; set; }
        public string DirectLink { get; set; }
        public Nullable<int> Order { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Author> Author { get; set; }
    }
}
