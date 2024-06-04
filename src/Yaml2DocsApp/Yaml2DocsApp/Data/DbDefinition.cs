using YamlDotNet.Serialization;

namespace Yaml2DocsApp.Data
{
    /// <summary>
    /// DB定義書のテンプレートデータクラス
    /// </summary>
    public class DbDefinition : YmlDataBase
    {
        /// <summary>
        /// テーブルの物理名
        /// </summary>
        [YamlMember(Alias = "id")]
        public string Id { get; set; }

        /// <summary>
        /// テーブルの論理名
        /// </summary>
        [YamlMember(Alias = "テーブル名称")]
        public string Nm { get; set; }

        /// <summary>
        /// テーブルの列項目
        /// </summary>
        [YamlMember(Alias = "列項目")]
        public List<DbColumn> Columns { get; set; }

        /// <summary>
        /// テーブルのインデックス項目
        /// </summary>
        [YamlMember(Alias = "インデックス項目")]
        public List<DbIndex> Indexes { get; set; } = new List<DbIndex>();

        /// <summary>
        /// テーブルのPK列名
        /// </summary>
        [YamlMember(Alias = "PKeys")]
        public string PKeys { get; set; }

        /// <summary>
        /// テーブルの列項目クラス
        /// </summary>
        public class DbColumn : YmlChildDataBase
        {
            /// <summary>
            /// 物理名称
            /// </summary>
            [YamlMember(Alias = "物理名称")]
            public string Id { get; set; }

            /// <summary>
            /// 論理名称
            /// </summary>
            [YamlMember(Alias = "論理名称")]
            public string Nm { get; set; }

            /// <summary>
            /// データ型
            /// </summary>
            [YamlMember(Alias = "データ型")]
            public string Type { get; set; }

            /// <summary>
            /// データ型の桁数
            /// </summary>
            [YamlMember(Alias = "桁数")]
            public string Length { get; set; }

            /// <summary>
            /// 初期値
            /// </summary>
            [YamlMember(Alias = "初期値")]
            public string InitVal { get; set; }

            /// <summary>
            /// PKフラグ
            /// </summary>
            [YamlMember(Alias = "PK")]
            public bool IsPKey { get; set; }

            /// <summary>
            /// Identityフラグ
            /// </summary>
            [YamlMember(Alias = "ID")]
            public bool IsIdentity { get; set; }

            /// <summary>
            /// NotNullフラグ
            /// </summary>
            [YamlMember(Alias = "NN")]
            public bool IsNotNull { get; set; }

            /// <summary>
            /// 備考
            /// </summary>
            [YamlMember(Alias = "備考")]
            public string Remarks { get; set; }
        }

        /// <summary>
        /// テーブルのインデックス項目クラス
        /// </summary>
        public class DbIndex : YmlChildDataBase
        {
            /// <summary>
            /// インデックス番号
            /// </summary>
            [YamlMember(Alias = "No")]
            public int No { get; set; }

            /// <summary>
            /// インデックス物理名
            /// </summary>
            [YamlMember(Alias = "インデックス名")]
            public string Nm { get; set; }

            /// <summary>
            /// インデックス列名
            /// </summary>
            [YamlMember(Alias = "列名")]
            public string Columns { get; set; }

            /// <summary>
            /// ユニークフラグ
            /// </summary>
            [YamlMember(Alias = "UNIQUE")]
            public bool Unique { get; set; }
        }
    }
}
