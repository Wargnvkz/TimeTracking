using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace TimeTracking
{
    public abstract class DataGridViewCustomComboBoxColumn: DataGridViewComboBoxColumn
    {
        /// <summary>
        /// Поле в таблице, формирующей этот список, которое ссылается на родительскую таблицу и по которому этот список ограничивается
        /// </summary>
        public string[] ParentField { get; protected set; }

        public DataGridViewColumn[] ParentColumns { get; protected set; }

        public DataGridView dgvParent { get; protected set; }

        public void CellValueChange(DataGridView dgv, int RowIndex, int ColumnIndex)
        {
            if (ParentColumns == null || ParentField.Length == 0) return;
            if (dgv == null) return;
            var column = dgv.Columns[ColumnIndex];
            if (ParentColumns.Contains(column))
            {
                //RefreshFieldData();
                FillCellCombobox(RowIndex);
            }
        }
        public abstract void FillCellCombobox(int Row);
        //protected abstract void CauseChildValueChange();

        


    }
    public class DataGridViewCustomComboBoxColumn<T> : DataGridViewCustomComboBoxColumn where T : class, new()
    {
        public DbSet<T> DatabaseDataSource;
        List<T> data;
        public DataGridViewCustomComboBoxColumn() : base()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="externalField">Поле главной таблицы, по которой, которое получает выбранное в списке значение</param>
        /// <param name="textField">Поле, представляющее имена из таблицы-списка</param>
        /// <param name="valueField">Поле значений таблицы-списка</param>
        /// <param name="parentField">Поле в таблице, по которому ограничивается его отображение(Foreing key)</param>
        /// <param name="databaseDataSource">DbSet базы данных, отку будет браться данные для выпадающего списка</param>
        /// <param name="parent">DataGridView, в который эта колонка будет добавлена</param>
        /// <param name="headerText">Текст заголовка столбца</param>
        /// <param name="parentColumn">Колонка, изменения в которой приведут к фильтрации данных в данной колонке</param>
        public DataGridViewCustomComboBoxColumn(
            string headerText,
            string externalField,
            string textField,
            string valueField,
            string[] parentField,
            DbSet<T> databaseDataSource,
            DataGridView parent,
            DataGridViewColumn[] parentColumn
        ) : base()
        {
            if (parentColumn != null && parentField != null)
                if (parentColumn.Length != parentField.Length) throw new RankException("Количество родительских полей должно совпадать с количеством полей в базе данных");
            HeaderText = headerText;
            DisplayMember = textField;
            ValueMember = valueField;
            DataPropertyName = externalField;
            ParentField = parentField;
            DatabaseDataSource = databaseDataSource;
            ParentColumns = parentColumn;
            dgvParent = parent;
            dgvParent.Columns.Add(this);
            if (ParentColumns != null)
            {
                dgvParent.CellValueChanged += Parent_CellValueChanged;
                dgvParent.CellBeginEdit += DgvParent_CellBeginEdit;
            }
            //dgvParent.DataSourceChanged += DgvParent_DataSourceChanged;
            RefreshFieldData();
        }

        private void DgvParent_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (ParentColumns == null || ParentField.Length == 0) return;
            if (dgvParent == null) return;
            var column = dgvParent.Columns[e.ColumnIndex];
            if (column==this)
            {

                for(int c=0;c< dgvParent.Columns.Count;c++)
                {
                    var parentcol = dgvParent.Columns[c];
                    if (ParentColumns.Contains(parentcol))
                    {
                        //RefreshFieldData();
                        CellValueChange(sender as DataGridView, e.RowIndex, c);
                        //FillCellCombobox(RowIndex);
                    }
                }
                //var cell = dgvParent.Rows[e.RowIndex].Cells[e.RowIndex] as DataGridViewComboBoxCell;
            }
            
        }

        /*private void DgvParent_DataSourceChanged(object sender, EventArgs e)
        {
            if (DataSource != null)
            {
                if (ParentColumns == null || ParentColumns.Length == 0)
                {
                    CauseChildValueChange();
                }
            }
        }*/

        private void Parent_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CellValueChange(sender as DataGridView, e.RowIndex, e.ColumnIndex);
        }

        /*protected override void CauseChildValueChange()
        {
            for (int i = 0; i < dgvParent.Columns.Count; i++)
            {
                var column = dgvParent.Columns[i] as DataGridViewCustomComboBoxColumn;
                if (column != null && column.ParentColumns!=null)
                {
                    if (column.ParentColumns.Contains(this))
                        for (int r = 0; r < dgvParent.Rows.Count; r++)
                        {
                            column.CellValueChange(dgvParent, r,i);
                        }
                }
            }
        }*/

        private void RefreshFieldData()
        {
            data = DatabaseDataSource.ToList();
            //if (ParentColumns == null || ParentField.Length == 0)
                DataSource = data;
        }

        /*public void FillColumn(int? ParentValue)
        {
            List<T> data;
            if (ParentValue.HasValue)
            {
                var res = GetListWithParentValue(DatabaseDataSource, ParentField, ParentValue.Value);
                data = res.ToList();
            }
            else
            {
                data = DatabaseDataSource.ToList();
            }

            DataSource = data;
        }*/

        /*private static List<T> GetListWithParentValue(DbSet<T> dbset, string ParentField, int ParentValue)
        {
            var x = Expression.Parameter(typeof(DbSet<T>), "x");
            var r = Expression.Parameter(typeof(T), "r");
            var r_ParentField = Expression.PropertyOrField(r, ParentField);
            var constant = Expression.Constant(ParentValue);
            var equality = Expression.Equal(r_ParentField, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equality, r);
            MethodInfo mi = x.Type.GetMethod("Where");
            var mis = x.Type.GetMethods();

            var whereCall = Expression.Call(x, mi, lambda);

            var exCall = Expression.Lambda<Func<DbSet<T>, IQueryable<T>>>(whereCall, x);
            var xCall = exCall.Compile();
            var res = xCall(dbset);

            var data = res.ToList();
            return data;
        }*/
        private static List<T> GetListWithParentValue(List<T> TList, string ParentField, int ParentValue)
        {
            var x = Expression.Parameter(typeof(List<T>), "x");
            var r = Expression.Parameter(typeof(T), "r");
            var r_ParentFieldInput = Expression.PropertyOrField(r, ParentField);
            var PropertyType = r_ParentFieldInput.Type;

            Expression r_ParentField;
            if (PropertyType == typeof(int?))
            {
                var mi_getValue = r_ParentFieldInput.Type.GetMethod("GetValueOrDefault", new Type[0]);
                var getvalueExec = Expression.Call(r_ParentFieldInput, mi_getValue);

                r_ParentField = getvalueExec;
            }
            else
            {
                r_ParentField = r_ParentFieldInput;
            }

            var value = Expression.Constant(ParentValue);
            var equality = Expression.Equal(r_ParentField, value);
            var lambda = Expression.Lambda<Predicate<T>>(equality, r);
            MethodInfo mi = x.Type.GetMethod("FindAll");

            var whereCall = Expression.Call(x, mi, lambda);

            var exCall = Expression.Lambda<Func<List<T>, List<T>>>(whereCall, x);
            var xCall = exCall.Compile();
            var res = xCall(TList);


            return res;
        }
        private static List<T> GetListWithParentValue(List<T> TList, List<Tuple<string, int?>> ParentFieldValue)//string[] ParentField, int[] ParentValue)
        {
            // (x)=>x.FindAll(r=>r.ParentField==ParentValue)
            // (x)=>x.FindAll(r=>r.ParentField.GetValueOrDefault()==ParentValue)
            if (ParentFieldValue == null || ParentFieldValue.Count == 0) return TList;
            var x = Expression.Parameter(typeof(List<T>), "x");

            var r = Expression.Parameter(typeof(T), "r");

            var equality0 = EqualFieldToValueExpression(r, ParentFieldValue[0].Item1, ParentFieldValue[0].Item2);
            var lastexpression = equality0;
            for (int i = 1; i < ParentFieldValue.Count; i++)
            {
                var equality = EqualFieldToValueExpression(r, ParentFieldValue[i].Item1, ParentFieldValue[i].Item2);
                var newexpression = Expression.And(lastexpression, equality);
                lastexpression = newexpression;
            }

            var lambda = Expression.Lambda<Predicate<T>>(lastexpression, r);
            MethodInfo mi = x.Type.GetMethod("FindAll");

            var whereCall = Expression.Call(x, mi, lambda);

            var exCall = Expression.Lambda<Func<List<T>, List<T>>>(whereCall, x);
            var xCall = exCall.Compile();
            var res = xCall(TList);


            return res;
        }

        /*private static Expression EqualFieldToValueExpression(ParameterExpression r, string FieldName, int Value)
        {
            var r_ParentFieldInput = Expression.PropertyOrField(r, FieldName);
            var PropertyType = r_ParentFieldInput.Type;

            Expression r_ParentField;
            if (PropertyType == typeof(int?))
            {
                var mi_getValue = r_ParentFieldInput.Type.GetMethod("GetValueOrDefault", new Type[0]);
                var getvalueExec = Expression.Call(r_ParentFieldInput, mi_getValue);

                r_ParentField = getvalueExec;
            }
            else
            {
                r_ParentField = r_ParentFieldInput;
            }

            var value = Expression.Constant(Value);
            var equality = Expression.Equal(r_ParentField, value);
            return equality;
        }*/
        private static Expression EqualFieldToValueExpression(ParameterExpression r, string FieldName, int? Value)
        {
            var r_ParentFieldInput = Expression.PropertyOrField(r, FieldName);
            var PropertyType = r_ParentFieldInput.Type;

            Expression valueExpression;

            Expression r_ParentField = r_ParentFieldInput;
            if (PropertyType == typeof(int))
            {
                valueExpression = Expression.Constant(Value.GetValueOrDefault());
            }
            else
            {
                valueExpression = Expression.Convert(Expression.Constant(Value), PropertyType);
            }

            var equality = Expression.Equal(r_ParentField, valueExpression);
            return equality;
        }

        private static List<T> GetListWithParentValueOld(List<T> TList, string ParentField, int ParentValue)
        {
            var x = Expression.Parameter(typeof(List<T>), "x");
            var r = Expression.Parameter(typeof(T), "r");
            var r_ParentField = Expression.PropertyOrField(r, ParentField);
            var value = Expression.Constant(ParentValue);
            var equality = Expression.Equal(r_ParentField, value);
            var lambda = Expression.Lambda<Predicate<T>>(equality, r);
            MethodInfo mi = x.Type.GetMethod("FindAll");

            var whereCall = Expression.Call(x, mi, lambda);

            var exCall = Expression.Lambda<Func<List<T>, List<T>>>(whereCall, x);
            var xCall = exCall.Compile();
            var res = xCall(TList);

            //var data = res.ToList();
            return res;
        }
        /*public void FillCellCombobox(int Row)
        {
            if (ParentColumn == null || ParentField.Length == 0) return;
            var ParentColumnIndex = DataGridView.Columns.IndexOf(ParentColumn);
            var thisColumnIndex = DataGridView.Columns.IndexOf(this);
            var parentCell = DataGridView.Rows[Row].Cells[ParentColumnIndex];
            if (parentCell.Value != null)
            {
                try
                {
                    var value = Convert.ToInt32(parentCell.Value);
                    DataGridViewComboBoxCell thisCell = (DataGridViewComboBoxCell)DataGridView.Rows[Row].Cells[thisColumnIndex];
                    thisCell.DataSource = GetListWithParentValue(data, ParentField, value);
                    //thisCell.DataSource = GetListWithParentValue(DatabaseDataSource, ParentField, value);
                }
                catch (Exception ex)
                { }
            }
        }*/
        public override void FillCellCombobox(int Row)
        {
            List<T> tmpdata;
            if (ParentColumns == null || ParentColumns.Length == 0)
            {
                tmpdata = data;
            }
            else
            {
                var parentFieldValues = new List<Tuple<string, int?>>();

                for (int i = 0; i < ParentColumns.Length; i++)
                {
                    var ParentColumn = ParentColumns[i];
                    var ParentColumnIndex = DataGridView.Columns.IndexOf(ParentColumn);
                    var parentCell = DataGridView.Rows[Row].Cells[ParentColumnIndex];
                    int? value = null;
                    if (parentCell.Value != null)
                        value = Convert.ToInt32(parentCell.Value);
                    parentFieldValues.Add(new Tuple<string, int?>(ParentField[i], value));
                }

                tmpdata = GetListWithParentValue(data, parentFieldValues);
            }
            var thisColumnIndex = DataGridView.Columns.IndexOf(this);
            DataGridViewComboBoxCell thisCell = (DataGridViewComboBoxCell)DataGridView.Rows[Row].Cells[thisColumnIndex];
            thisCell.Value = null;
            tmpdata.Insert(0, new T());
            thisCell.DataSource = tmpdata;
        }

        public Dictionary<int, DataGridViewCustomComboBoxColumn> GetChildren()
        {
            var res = new Dictionary<int, DataGridViewCustomComboBoxColumn>();
            for(int i=0;i<dgvParent.Columns.Count;i++)
            {
                var column=dgvParent.Columns[i] as DataGridViewCustomComboBoxColumn;
                if (column != null)
                {
                    if (column.ParentColumns.Contains(this))
                        res.Add(i,column);
                }
            }
            return res;
        }
    }
}

