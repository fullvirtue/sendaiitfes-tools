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
    
    public partial class Author
    {
        public int AuthorId { get; set; }
        public int SessionId { get; set; }
        public int SpeakerId { get; set; }
        public int Order { get; set; }
    
        public virtual Speaker Speaker { get; set; }
        public virtual Session Session { get; set; }
    }
}
