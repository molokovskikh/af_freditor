using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace FREditor.Helpers
{
	public class DataTableMarking : DataTable
	{
		private DataColumn _nameFieldColumn;
		private DataColumn _beginColumn;
		private DataColumn _endColumn;

		public DataTableMarking()
			: base()
		{
			_nameFieldColumn = new DataColumn("MNameField");
			_beginColumn = new DataColumn("MBeginField", typeof(int));
			_endColumn = new DataColumn("MEndField", typeof(int));
			
			Columns.AddRange(new DataColumn[] { _nameFieldColumn, _beginColumn, _endColumn });
			Constraints.AddRange(new Constraint[] {
				new UniqueConstraint("UniqueNameConstraint", new DataColumn[] { _nameFieldColumn }, false)
			});
			TableName = "Разметка";
		}

		public bool Check()
		{
			if (Rows.Count > 2)
			{
				var newMarking = DefaultView.ToTable();
				for (var i = 1; i < newMarking.Rows.Count; i++)
				{
					var previousRow = newMarking.Rows[i - 1];
					var currentRow = newMarking.Rows[i];
					if ((previousRow[_beginColumn.ColumnName] is DBNull) && (previousRow[_endColumn.ColumnName] is DBNull))
						return false;
					var begin = Convert.ToInt32(previousRow[_beginColumn.ColumnName]);
					var end = Convert.ToInt32(previousRow[_endColumn.ColumnName]);
					if (begin - end > 0)
						return false;
					if (currentRow[_endColumn.ColumnName] is DBNull)
						return false;
					begin = Convert.ToInt32(currentRow[_beginColumn.ColumnName]);
					end = Convert.ToInt32(previousRow[_endColumn.ColumnName]);
					if ((begin - end) != 1)
						return false;
				}
			}
			else
			{
				var begin = Convert.ToInt32(Rows[0][_beginColumn.ColumnName]);
				int end = Convert.ToInt32(Rows[0][_endColumn.ColumnName]);
				if (begin - end > 0)
					return false;
			}
			return true;
		}

		public void Fill(DataRow formRulesRow, DataRow[] costFormRulesRows)
		{
			int index;
			var prefixFieldName = "FRTxt";
			var suffixFieldNameBegin = "Begin";
			var suffixFieldNameEnd = "End";

			var markingFields = new ArrayList();
			foreach (PriceFields priceField in Enum.GetValues(typeof(PriceFields)))
			{
				var fieldName = priceField.ToString();
				if (PriceFields.OriginalName == priceField || PriceFields.Name1 == priceField)
					fieldName = "Name";
				try
				{
					index = formRulesRow.Table.Columns.IndexOf(prefixFieldName + fieldName + suffixFieldNameBegin);
				}
				catch
				{
					index = -1;
				}
				var name = ((index > -1) && !(formRulesRow[prefixFieldName + fieldName + suffixFieldNameBegin] is DBNull)) ? fieldName : String.Empty;
				
				if ((PriceFields.OriginalName != priceField) && (!String.IsNullOrEmpty(name)))
				{
					try
					{
						var begin = Convert.ToInt32(formRulesRow[prefixFieldName + name + suffixFieldNameBegin]);
						var end = Convert.ToInt32(formRulesRow[prefixFieldName + name + suffixFieldNameEnd]);
						markingFields.Add(new TxtFieldDef(name, begin, end));
					}
					catch {}
				}
			}
			
			//Добавляем в список цены, если у них выставлены границы
			foreach (var row in costFormRulesRows)
			{
				if (!(row["CFRTextBegin"] is DBNull) && !(row["CFRTextEnd"] is DBNull))
				{
					var name = "Cost" + row["CFRCost_Code"].ToString();
					var begin = Convert.ToInt32(row["CFRTextBegin"]);
					var end = Convert.ToInt32(row["CFRTextEnd"]);
					markingFields.Add(new TxtFieldDef(name, begin, end));
				}
			}
			markingFields.Sort(new TxtFieldDef());

			// Удаляем поля, которые выставлены в 0 или 
			// установлены неправильные значения для начала и конца (конец больше начала) или
			// есть перекрывающийся индекс
			var lastPosition = 0;
			for (var i = 0; i < markingFields.Count; i++)
			{
				var field = (TxtFieldDef)markingFields[i];
				if (((field.posBegin == 0) && (field.posEnd == 0)) ||
					(field.posBegin >= field.posEnd) ||
					(lastPosition >= field.posBegin))
				{
					markingFields.RemoveAt(i--);
				}
				else
				{
					lastPosition = field.posEnd;
				}
			}
			FillRows(markingFields);
			AcceptChanges();
		}

		private void FillRows(IList markingFields)
		{
			int countx = 1;
			if (markingFields.Count > 0)
			{
				TxtFieldDef prevTFD, currTFD = (TxtFieldDef)markingFields[0];

				if (1 == currTFD.posBegin)
					AddRow(currTFD.fieldName, 1, currTFD.posEnd);
				else
				{
					AddRow(String.Format("x{0}", countx), 1, currTFD.posBegin - 1);
					countx++;
					AddRow(currTFD.fieldName, currTFD.posBegin, currTFD.posEnd);
				}
				var i = 1;
				for (i = 1; i <= markingFields.Count - 1; i++)
				{
					prevTFD = (TxtFieldDef)markingFields[i - 1];
					currTFD = (TxtFieldDef)markingFields[i];
					if (currTFD.posBegin == prevTFD.posEnd + 1)
						AddRow(currTFD.fieldName, currTFD.posBegin, currTFD.posEnd);
					else
					{
						AddRow(String.Format("x{0}", countx), prevTFD.posEnd + 1, currTFD.posBegin - 1);
						countx++;
						AddRow(currTFD.fieldName, currTFD.posBegin, currTFD.posEnd);
					}
				}
				var lastTFD = (TxtFieldDef)markingFields[i - 1];
				if (lastTFD.posEnd < 255)
				{
					AddRow(String.Format("x{0}", countx), lastTFD.posEnd + 1, 255);
				}
			}
			else
				AddRow("x1", 1, 255);
		}

		private void AddRow(string nameField, int beginPosition, int endPosition)
		{
			var row = NewRow();
			row[_nameFieldColumn.ColumnName] = nameField;
			row[_beginColumn.ColumnName] = beginPosition;
			row[_endColumn.ColumnName] = endPosition;
			Rows.Add(row);
		}
	}
}
