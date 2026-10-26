using System.Collections.Generic;

namespace Meta
{
	public class MetaData
	{
		private string delimiter = ":";

		protected bool hasNewData;

		public bool HasNewData
		{
			get
			{
				return hasNewData;
			}
			set
			{
				hasNewData = value;
			}
		}

		public virtual string FileName()
		{
			return null;
		}

		public virtual bool CanSave()
		{
			return hasNewData;
		}

		public virtual List<string> Serialize()
		{
			return null;
		}

		protected string SerializeData(string name, string value)
		{
			return name + delimiter + value;
		}

		protected string SerializeData(string name, int value)
		{
			return SerializeData(name, value.ToString());
		}

		protected string SerializeData(string name, uint value)
		{
			return SerializeData(name, value.ToString());
		}

		protected string SerializeData(string name, bool value)
		{
			return SerializeData(name, value.ToString());
		}

		public virtual void Deserialize(List<string> lines)
		{
			string name = string.Empty;
			string value = string.Empty;
			foreach (string line in lines)
			{
				if (DeserializeLine(line, out name, out value))
				{
					OnDeserializeLine(name, value);
				}
			}
		}

		protected bool DeserializeLine(string line, out string name, out string value)
		{
			name = string.Empty;
			value = string.Empty;
			int num = 0;
			num = line.LastIndexOf(delimiter);
			if (num != -1)
			{
				name = line.Substring(0, num);
				value = line.Substring(num + 1, line.Length - (num + 1));
				return true;
			}
			return false;
		}

		protected virtual void OnDeserializeLine(string name, string value)
		{
		}

		public virtual void DebugPrint()
		{
		}
	}
}
