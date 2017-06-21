using System.Collections;
using System.Collections.Generic;
using System.Text;

public static class StringTools {

	public static string RemoveSpecialCharacters(string str)
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < str.Length; i++)
		{
			if ((str[i] >= '0' && str[i] <= '9')
				|| (str[i] >= 'A' && str[i] <= 'z'
					|| (str[i] == '.' || str[i] == '_')))
			{
				sb.Append(str[i]);
			}
		}

		return sb.ToString();
	}
}
