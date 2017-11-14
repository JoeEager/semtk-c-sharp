using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace SemTK_Universal_Support.SemTK.ResultSet
{
    public class Table
    {
        // json keys used here and there
        public static String JSON_KEY_COL_NAMES = "col_names";
        public static String JSON_KEY_COL_TYPES = "col_type";
        public static String JSON_KEY_ROWS = "rows";
        public static String JSON_KEY_ROW_COUNT = "row_count";
        public static String JSON_KEY_COL_COUNT = "col_count";
        public static String JSON_TYPE = "type";

        // book keeping values
        private String[] columnNames;
        private String[] columnTypes;
        private List<List<String>> rows;
        private Dictionary<String, int> columnPositionInfo = new Dictionary<string, int>();

        public Table(String[] cols, String[] colTypes, List<List<String>> rows)
        {
            // validate what we can.
            if(cols == null) { throw new Exception("cannot create a table: no columns provided."); }
            if(colTypes == null) { throw new Exception("cannot create a table: no column types provided."); }
            if(cols.Length != colTypes.Length) {  throw new Exception("Cannot create a Table: must provide the same number of columns and column types"); }

            // set up the rows.
            if (rows != null && rows.Count > 0)
            {
                if(rows[0] == null) { throw new Exception("Cannot create a Table: row is null"); }
                if(rows[0].Count != cols.Length) { throw new Exception("Cannot create a Table: wrong number of entries in rows"); }
                this.rows = rows;
            }
            else // create a new set of rows.
            {
                this.rows = new List<List<string>>();
            }

            this.columnNames = cols;
            this.columnTypes = colTypes;

            // add all the column names to the dictionary to speed up lookups.
            int colNum = 0;
            foreach(String c in this.columnNames)
            {
                columnPositionInfo.Add(c, colNum);
                colNum++;
            }
        }

        public void ReplaceColumnNames(String[] newColumnNames)
        {   // this allows the replacement of the existing column names with a new set. 
            // it requires that the incoming count of names match the ones being replaced.

            if(this.columnNames.Length != newColumnNames.Length) { throw new Exception("replaceColumnNames: the incoming column name count (" + newColumnNames.Length + ") does not match the target column names count (" + this.columnNames.Length + ")"); }
            else { this.columnNames = newColumnNames; }
        }

        public int GetNumRows() { return this.rows.Count; }
        public int GetNumColumns() { return this.columnNames.Length; }
        public String[] GetColumnNames() { return this.columnNames; }

        public Table Slice(int offset, int size)
        {
            int upper;
            // make sure the upper limit is in bounds
            if(size == 0 || (offset + size) > this.GetNumRows()) { upper = this.GetNumRows(); }
            else { upper = offset + size; }

            // get the rows we want... this was performed more simply on the java side.
            List<List<String>> returnRows = new List<List<string>>();
            for(int currInterest = offset; currInterest < upper; currInterest++)
            {
                returnRows.Add(this.rows[currInterest]);
            }

            Table retval = new Table(this.columnNames, this.columnTypes, returnRows);
            return retval;
        }

        public Boolean HasColumn(String colName)
        {
            if(this.GetColumnIndex(colName) == -1 ) { return false; }
            else { return true; }
        }

        public String[] GetColumnTypes() { return this.columnTypes; }

        public String GetColumnType(String columnName) { return this.columnTypes[this.GetColumnIndex(columnName)]; }

        public String[] GetColumn(String columnName) { return this.GetColumn(this.GetColumnIndex(columnName)); }

        public String[] GetColumn(int indexNumber)
        {
            String[] retval = new String[this.rows.Count];
            int i = 0;
            foreach (List<String> row in this.rows)
            {
                retval[i] = row[indexNumber];
                i++;
            }
            return retval;             
        }

        public int GetColumnIndex(String columnName)
        {
            int retval = -1;

            if (this.columnPositionInfo.ContainsKey(columnName)) { retval = this.columnPositionInfo[columnName]; }

            return retval;
        }

        public  String[] GetColumnUniqueValues(String columnName)
        {
            return this.GetColumnUniqueValues(this.GetColumnIndex(columnName));
        }

        public String[] GetColumnUniqueValues(int indexNumber)
        {
            List<String> returnColumn = new List<string>();

            foreach(List<String> currRow in this.rows)
            {
                if(! returnColumn.Contains(currRow[indexNumber])) { returnColumn.Add(currRow[indexNumber]); }
            }

            return returnColumn.ToArray();
        }

        public void AddRow(List<String> newRow)
        {
            if(newRow.Count != this.columnNames.Length) { throw new Exception("Incoming row has " + newRow.Count + " columns but " + this.columnNames.Length + " were expected."); }
            else { this.rows.Add(newRow); }
        }

        public List<List<String>> GetRows() { return this.rows; }

        public List<String> GetRow(int rowNum) { return this.rows[rowNum]; }

        public String GetCell(int row, int col) { return (this.rows[row])[col]; }

        public String GetCellAsString(int row, int col) { return this.GetCell(row, col); }

        public int GetCellAsInt(int row, int col) { return int.Parse(this.GetCell(row, col)); }

        public long GetCellAsLong(int row, int col) { return long.Parse(this.GetCell(row, col)); }

        public float GetCellAsFloat(int row, int col) { return float.Parse(this.GetCell(row, col)); }

        public void Truncate(int length)
        {
            int size = this.rows.Count;
            if(size > length)
            {
                this.rows.RemoveRange(length, size);

            }
        }

        public void ClearRows() { this.rows = new List<List<string>>(); }

        public static Table FromJson(JsonObject jsonObj)
        {
            // gather columns
            JsonArray columnNamesJson = jsonObj.GetNamedArray(JSON_KEY_COL_NAMES);
            String[] cols = new String[columnNamesJson.Count];

            for(int i = 0; i < columnNamesJson.Count; i++)
            {
                cols[i] = columnNamesJson[i].GetString();   // should get the string name from the entity.
            }

            // gather the column types
            JsonArray columnTypesJson = jsonObj.GetNamedArray(JSON_KEY_COL_TYPES);
            if(columnTypesJson == null) { throw new Exception("Cannot create Table from json: no column types specified"); }
            String[] colTypes = new String[columnTypesJson.Count];

            for(int i = 0; i < columnTypesJson.Count; i++)
            {
                if(columnTypesJson[i] == null) { throw new Exception("Cannot create Table with null column type"); }
                colTypes[i] = columnTypesJson[i].GetString();
            }


            // gather rows
            String s;
            List<List<String>> rows = new List<List<String>>();
            JsonArray rowsJson = jsonObj.GetNamedArray(JSON_KEY_ROWS);
            for(int i = 0; i < rowsJson.Count; i++)
            {
                List<String> row = new List<String>();          // the current row we will be making.
                JsonArray rowJson = rowsJson.GetArrayAt((uint)i);

                for(int j = 0; j < rowJson.Count; j++)
                {
                    s = rowJson.GetStringAt((uint)j);         // get the string
                    if(s != null) { row.Add(s); }
                    else { row.Add("null"); }
                }
                rows.Add(row);
            }

            return new Table(cols, colTypes, rows);
        }

        public JsonObject ToJson(Boolean includeDataRows)
        {
            JsonArray allRows = new JsonArray();
            int rowCount = 0;

            // collect data rows in JsonArray
            try
            {   // we can do this faster....
                int counter = 0;
                int[] columnNumbersInOrderIWanted = new int[this.columnNames.Length];

                foreach(String colName in this.columnNames)
                {
                    columnNumbersInOrderIWanted[counter] = this.GetColumnIndex(colName);
                    counter++;
                }

                if (includeDataRows)
                {
                    foreach(List<String> row in this.GetRows())
                    {
                        if(row != null) {  // never include null rows.
                            JsonArray currRow = new JsonArray();
                            foreach(int k in columnNumbersInOrderIWanted) {
                                currRow.Add( JsonValue.CreateStringValue(row[k]));       // this hopefullt corectly converts from the string to a jsonValue string.
                            } 
                            allRows.Add(currRow);
                            rowCount += 1;
                        }
                    }
                }
                else
                {
                    rowCount = this.GetRows().Count;
                }
            }
            catch(Exception e)
            {
                throw new Exception ("Unable to collect row data for JSON table result set: " + e.Message);
            }

            // get the columns int a json array
            JsonArray colHeaders = new JsonArray();
            JsonArray colTypes = new JsonArray();

            try
            {
                for(int i = 0; i < this.columnNames.Length; i++)
                {
                    colHeaders.Add(JsonValue.CreateStringValue(columnNames[i]));
                    colTypes.Add(JsonValue.CreateStringValue(columnTypes[i]));
                }
            }
            catch(Exception e)
            {
                throw new Exception("Unable to collect column names/types for JSON table result set: " + e.Message);
            }

            // assemble the Json Object
            try
            {
                // create the table
                JsonObject tbl = new JsonObject();
                if (includeDataRows) { tbl.Add(JSON_KEY_ROWS, allRows); }
                tbl.Add(JSON_TYPE, JsonValue.CreateStringValue("TABLE"));
                tbl.Add(JSON_KEY_ROW_COUNT, JsonValue.CreateNumberValue(rowCount));
                tbl.Add(JSON_KEY_COL_NAMES, colHeaders);
                tbl.Add(JSON_KEY_COL_TYPES, colTypes);
                tbl.Add(JSON_KEY_COL_COUNT, JsonValue.CreateNumberValue(this.columnNames.Length));

                return tbl;         // send it back
            }
            catch(Exception e)
            {
                throw new Exception("Error assembling JSON table result set: " + e.Message);
            }
        }

        public JsonObject ToJson() { return this.ToJson(true); }
    }
}
