using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;

namespace QUANLYDIEMDANHSV
{
    public static class Fish
    {
        public static IEnumerable<int> range(int end, int step = 1)
        {
            end *= step;
            for (int i = 0; i * step < end; i += step)
                yield return i;
        }

        public static IEnumerable<int> range(int start, int end, int step = 1)
        {
            end *= step;
            for (int i = start; i * step < end; i += step)
                yield return i;
        }
    }

    public static class Singleton<Data> where Data : class, new()
    {
        private static Data instance;

        public static Data Instance
        {
            get
            {
                if (Singleton<Data>.instance == null)
                    Singleton<Data>.instance = new Data();
                return Singleton<Data>.instance;
            }
        }
    }

    public class ConnectString
    {
        string connect_string;

        public ConnectString(string data_source, string database_name)
        {
            this.connect_string = string.Format("Data Source={0}; Initial Catalog={1}; Integrated Security=True",
                data_source, database_name);
        }

        public ConnectString(string data_source, string database_name, string user_name, string password)
        {
            this.connect_string = string.Format("Data Source={0}; Initial Catalog={1}; User ID={2}; Password={3}",
                data_source, database_name, user_name, password);
        }

        public override string ToString()
        {
            return this.connect_string;
        }
    }

    public class QueryString : IDisposable
    {
        bool m_Disposed = false;
        SqlCommand cmd;

        public QueryString(SqlConnection connection, string query, params object[] variables)
        {
            string[] params_name = new string[variables.Length];
            foreach (int i in Fish.range(variables.Length))
                params_name[i] = "@param_" + i.ToString();

            query = string.Format(query, params_name);

            this.cmd = new SqlCommand(query, connection);
            foreach (int i in Fish.range(variables.Length))
                this.cmd.Parameters.AddWithValue(params_name[i], variables[i]);
        }

        public SqlCommand GetSqlCommand
        {
            get { return this.cmd; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.m_Disposed)
            {
                if (disposing)
                    this.cmd.Dispose();

                this.m_Disposed = true;
            }
        }

        ~QueryString()
        {
            Dispose(false);
        }
    }

    public interface IConnectDatabase
    {
        int ExecuteNonQuery(string query, params object[] variables);
        bool InsertObject<T>(T Object, string table_name = "") where T : class;
        object ExecuteScalar(string query, params object[] variables);
        ITableReader ExecuteReader(string query, params object[] variables);
    }

    public interface ITableReader
    {
        T SingleObject<T>() where T : class, new();
        List<T> MappingObject<T>() where T : class, new();
        List<object[]> GetAll();
        List<object> FirstfColumn();
        List<object> FirstRow();
    }

    public class SqlTableReader : ITableReader
    {
        SqlConnection connect;
        SqlDataReader reader;

        public SqlTableReader() { }

        public SqlTableReader(SqlConnection connect, SqlDataReader reader)
        {
            this.connect = connect;
            this.reader = reader;
        }

        public SqlConnection Connection
        {
            set { this.connect = value; }
        }

        public SqlDataReader Reader
        {
            set { this.reader = value; }
        }

        /// <summary>
        /// Phương thức SingleObject : truy vấn một bản ghi duy nhất, tạo ra một đối tượng.
        /// </summary>
        /// <returns>Một thể hiện của class T.</returns>
        public T SingleObject<T>() where T : class, new()
        {
            if (!this.reader.HasRows)
            {
                this.connect.Close();
                return null;
            }    
                
            this.reader.Read();

            T obj = new T();

            foreach (int i in Fish.range(this.reader.FieldCount))
            {
                PropertyInfo property = typeof(T).GetProperty(this.reader.GetName(i));
                if (property != null)
                    property.SetValue(obj, this.reader.GetValue(i));
            }

            this.connect.Close();
            return obj;
        }

        /// <summary>
        /// Phương thức MappingObject : truy vấn nhiều bản ghi, tạo ra một danh sách đối tượng.
        /// </summary>
        /// <returns>Trả về danh sách các thể hiện của class T.</returns>
        public List<T> MappingObject<T>() where T : class, new()
        {
            List<T> listObj = new List<T>();

            while (this.reader.Read())
            {
                T obj = new T();

                foreach (int i in Fish.range(this.reader.FieldCount))
                {
                    PropertyInfo property = typeof(T).GetProperty(this.reader.GetName(i));
                    if (property != null)
                        property.SetValue(obj, this.reader.GetValue(i));
                }

                listObj.Add(obj);
            }

            this.connect.Close();
            return listObj;
        }

        /// <summary>
        /// Phương thức GetAll : truy vấn nhiều giá trị ( nhiều dòng ), tạo ra bảng object ( mảng hai chiều ).
        /// </summary>
        /// <returns>Trả về bảng object.</returns>
        public List<object[]> GetAll()
        {
            List<object[]> matrix = new List<object[]>();

            while (this.reader.Read())
            {
                object[] objs = new object[this.reader.FieldCount];

                foreach (int i in Fish.range(this.reader.FieldCount))
                    objs[i] = this.reader.GetValue(i);

                matrix.Add(objs);
            }

            this.connect.Close();

            return matrix;
        }

        public List<object> FirstfColumn()
        {
            List<object> listData = new List<object>();

            while (this.reader.Read())
                listData.Add(this.reader.GetValue(0));

            this.connect.Close();
            return listData;
        }

        public List<object> FirstRow()
        {
            List<object> row = new List<object>();

            if (this.reader.Read())
            {
                foreach (int i in Fish.range(this.reader.FieldCount))
                    row.Add(this.reader.GetValue(i));
                var type = this.reader.GetType();
            }
            else
                throw new SqlNoRowSelected();


            this.connect.Close();
            return row;
        }
    }

    public class ConnectSqlServer : IConnectDatabase
    {
        SqlConnection connect;

        public ConnectSqlServer() { }

        public ConnectSqlServer(ConnectString connect_string)
        {
            this.connect = new SqlConnection(connect_string.ToString());
        }

        public ConnectString IConnectString
        {
            set
            {
                if (this.connect == null && value != null)
                    this.connect = new SqlConnection(value.ToString());
            }
        }

        public string ConnectString
        {
            set
            {
                if (this.connect == null && value != string.Empty)
                    this.connect = new SqlConnection(value);
            }
        }

        public bool Connected
        {
            get { return this.connect != null; }
        }

        public List<string> QueryColumnsName(string table_name)
        {
            List<string> columns_name = new List<string>();

            // Get columns name
            using (QueryString queryObj =
                new QueryString(this.connect, "select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = {0}", table_name))
            {
                this.connect.Open();
                SqlDataReader reader = queryObj.GetSqlCommand.ExecuteReader();

                while (reader.Read())
                    columns_name.Add(reader.GetString(0));
                this.connect.Close();
            }

            return columns_name;
        }

        public bool InsertObject<T>(T Object, string table_name = "") where T : class
        {
            if (table_name == string.Empty)
                table_name = typeof(T).Name;

            List<string> columns_name = this.QueryColumnsName(table_name);

            string[] params_index = new string[columns_name.Count];
            foreach (int i in Fish.range(columns_name.Count))
                params_index[i] = "{" + i.ToString() + "}";

            object[] properties = new object[columns_name.Count];
            foreach (int i in Fish.range(columns_name.Count))
                properties[i] = typeof(T).GetProperty(columns_name[i]).GetValue(Object);

            string insert = string.Format("insert into {0} values ({1})", table_name, string.Join(", ", params_index));

            return this.ExecuteNonQuery(insert, properties) == 1;
        }

        public int ExecuteNonQuery(string query, params object[] variables)
        {
            int numberRowsAffected;

            using (QueryString queryObj = new QueryString(this.connect, query, variables))
            {
                this.connect.Open();
                numberRowsAffected = queryObj.GetSqlCommand.ExecuteNonQuery();
                this.connect.Close();
            }

            return numberRowsAffected;
        }

        public object ExecuteScalar(string query, params object[] variables)
        {
            object result;

            using (QueryString queryObj = new QueryString(this.connect, query, variables))
            {
                this.connect.Open();
                result = queryObj.GetSqlCommand.ExecuteScalar();
                this.connect.Close();
            }

            return result;
        }

        public ITableReader ExecuteReader(string query, params object[] variables)
        {
            SqlDataReader reader;
            this.connect.Open();

            using (QueryString queryObj = new QueryString(this.connect, query, variables))
                reader = queryObj.GetSqlCommand.ExecuteReader();

            return new SqlTableReader(this.connect, reader);
        }
    }

    //=====     My Exceptions     =====

    /// <summary>
    /// Cho biết độ dài một danh sách, hoặc số phần tử của một tập hợp không hợp lệ.
    /// </summary>
    public class LengthException : Exception
    {
        const string errorMessage = "Độ dài danh sách không hợp lệ !";
        public LengthException() : base(errorMessage) { }
    }

    /// <summary>
    /// Cho biết câu lệnh select không tồn tại bản ghi.
    /// </summary>
    public class SqlNoRowSelected : Exception
    {
        const string errorMessage = "Câu truy vấn không tồn tại bản ghi !";
        public SqlNoRowSelected() : base(errorMessage) { }
    }

    //=====     My Controls Inheritance     =====
    public class FishComboBox : ComboBox
    {
        private System.ComponentModel.IContainer components = null;
        private List<object> keys = new List<object>();

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        public FishComboBox()
        {
            InitializeComponent();
        }

        public FishComboBox(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        public string SelectedV
        {
            get
            {
                return (string)this.Items[this.SelectedIndex];
            }
        }

        public object SelectedK
        {
            get
            {
                return this.keys[this.SelectedIndex];
            }
        }

        public void Fill(List<object[]> table)
        {
            this.Items.Clear();
            this.keys.Clear();

            foreach (object[] pair in table)
            {
                this.Items.Add(pair[1]);
                this.keys.Add(pair[0]);
            }

            this.SelectedIndex = 0;
        }

        /// <summary>
        /// Đổ dữ liệu vào FishComboBox bằng hai danh sách gồm khóa và dữ liệu, với số phần tử của hai danh sách phải bằng nhau.
        /// Nếu changeSelected == true : FishComboBox sẽ chọn dòng dữ liệu đầu tiên, ngược lại nó chọn dòng dữ liệu chứa khóa cũ.
        /// </summary>
        /// <typeparam name="T">ComboBox có khóa là kiểu dữ liệu T.</typeparam>
        /// <param name="keys">Danh sách các khóa ứng với mỗi giá trị.</param>
        /// <param name="values">Danh sách các giá trị.</param>
        /// <param name="changeSelected">Tự động thay đổi lựa chọn</param>
        public void Fill<T>(List<T> keys, List<string> values)
        {
            // Xóa dữ liệu cũ
            if (this.Items.Count != 0)
            {
                this.Items.Clear();
                this.keys.Clear();
            }

            // Thêm dữ liệu mới
            foreach (string value in values)
                this.Items.Add(value);
            foreach (T key in keys)
                this.keys.Add(key);

            this.SelectedIndex = 0;
        }
    }

    public class FishListView : ListView
    {
        private System.ComponentModel.IContainer components = null;
        private List<object> keys = new List<object>();
        public int SelectedIndex = -1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        public FishListView()
        {
            InitializeComponent();

            this.GridLines = true;
            this.View = System.Windows.Forms.View.Details;
            this.SelectedIndexChanged += e_SelectedIndexChanged;
        }

        public FishListView(IContainer container)
        {
            container.Add(this);
            InitializeComponent();

            this.GridLines = true;
            this.View = System.Windows.Forms.View.Details;
            this.SelectedIndexChanged += e_SelectedIndexChanged;
        }

        /// <summary>
        /// Đổ dữ liệu vào FishListView bằng mảng hai chiều với mỗi ô là một chuỗi.
        /// </summary>
        /// <param name="listData">Mảng hai chiều các chuỗi</par
        /// am>
        public virtual void Fill(List<string[]> listData)
        {
            foreach (string[] row in listData)
                this.Items.Add(new ListViewItem(row));
        }

        public virtual void Fill(List<object[]> listData)
        {
            foreach (object[] row in listData)
            {
                string[] stringArr = (from block in row select block.ToString()).ToArray();
                this.Items.Add(new ListViewItem(stringArr));
            }
        }

        public virtual void FillKeys(List<object> keys)
        {
            this.keys.Clear();
            foreach (object key in keys)
                this.keys.Add(key);
        }

        public void AddValue(object key, string[] value)
        {
            this.keys.Add(key);
            this.Items.Add(new ListViewItem(value));
        }

        public void AddValue(object key, object[] value)
        {
            this.keys.Add(key);
            var value_to_string = (from v in value select v.ToString()).ToArray();
            this.Items.Add(new ListViewItem(value_to_string));
        }

        public void RemoveSelectedRow()
        {
            if (this.SelectedIndex < 0 || this.SelectedIndex >= this.keys.Count)
                throw new IndexOutOfRangeException();

            this.keys.RemoveAt(this.SelectedIndex);
            int index = this.SelectedIndex;
            this.SelectedIndex = 0;
            this.Items.RemoveAt(index);

        }

        static private void e_SelectedIndexChanged(object sender, EventArgs e)
        {
            FishListView fListView = (FishListView)sender;

            if (fListView.SelectedItems.Count > 0)
                fListView.SelectedIndex = fListView.Items.IndexOf(fListView.SelectedItems[0]);
        }

        public object SelectedK
        {
            get
            {
                if (this.SelectedIndex < 0)
                    return null;
                return this.keys[this.SelectedIndex];
            }
        }

        public ListViewItem SelectedV
        {
            get
            {
                if (this.SelectedIndex < 0)
                    return null;
                return this.Items[this.SelectedIndex];
            }
        }

        public List<object> Keys
        {
            get
            {
                return this.keys.ToList();
            }
        }

        public int RowNumber
        {
            get
            {
                return this.Items.Count;
            }
        }

        public int ColNumber
        {
            get
            {
                return this.Items[0].SubItems.Count;
            }
        }

        public string this[int row, int col]
        {
            get
            {
                return this.Items[row].SubItems[col].Text;
            }

            set
            {
                if (row >= 0 && row < this.Items.Count && col >= 0 && col < this.Items[row].SubItems.Count)
                    this.Items[row].SubItems[col].Text = value;
            }
        }
    }
}