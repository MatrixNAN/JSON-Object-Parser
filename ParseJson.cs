using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ParseJson : MonoBehaviour 
{
	public enum JsonType {Object = 1, Array = 2, String = 3, Int = 4, Float = 5, Bool = 6, Null = 7, DateTime = 8, NotSet = 9};

	public JSONObject createJsonObject(JSONObject json, string jsonStr)
	{
		jsonStr = jsonStr.Trim ();
		int openingCurlyBraces = 0;
		int firstCurlyBracesPosition = 0;
		int endingCurlyBracesPosition = 0;
		int endingCurlyBracesComputedPosition = 0;
		int openingBrackets = 0;
		int firstBracketsPosition = 0;
		int endingBracketsPosition = 0;
		int endingBracketsComputedPosition = 0;
		int charPosition = 0;
		//json = new JSONObject ();

		string propName = "";
		bool openQuote = false;
		bool inProp = false;
		bool inValue = false;
		string strValue = "";
		int intValue = 0;
		float floatValue = 0f;
		bool boolValue = false;
		bool inBrackets = false;
		string tempValue = "";
		string jsonElement = "";
		JsonType jsonMode = JsonType.NotSet;

		foreach (char c in jsonStr)
		{
			if (json.type == JSONObject.Type.ARRAY) {
				inValue = true;
				inProp = false;
			}
			if (c == '"' && !openQuote && !inProp && !inValue && openingCurlyBraces == 0 &&
			    openingBrackets == 0) 
			{
				openQuote = true;
				if (json.type != JSONObject.Type.ARRAY) 
				{
					inProp = true;
					propName = "";
					inValue = false;
				} 
				else 
					jsonMode = JsonType.String;
			}
			else if (openQuote && inProp && !inValue && c != '"' && 
				openingCurlyBraces == 0 && openingBrackets == 0) 
			{
				propName += c;
			}
			else if (c == '"' && openQuote && inProp && openingCurlyBraces == 0 && 
				openingBrackets == 0) 
			{
				openQuote = false;
				inProp = false;
			}
			else if (c == ':' && !openQuote && !inProp && !inValue && 
				openingCurlyBraces == 0 && openingBrackets == 0) {
				inValue = true;
				tempValue = "";
				jsonMode = JsonType.NotSet;
			}
			else if ((c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || 
				c == '7' || c == '8' || c == '9' || c == '0' || c == '-') && !openQuote && !inProp 
				&& inValue && openingCurlyBraces == 0 && openingBrackets == 0) 
			{
				tempValue += c;
				if (tempValue.Length == 1)
					jsonMode = JsonType.Int;
			}
			else if (c == '.' && !openQuote && !inProp && inValue && 
				openingCurlyBraces == 0 && openingBrackets == 0) 
			{
				tempValue += c;
				jsonMode = JsonType.Float;
			}
			else if (c == '"' && !openQuote && !inProp && inValue && 
				openingCurlyBraces == 0 && openingBrackets == 0) 
			{
				openQuote = true;
				jsonMode = JsonType.String;
			}
			else if (openQuote && !inProp && inValue && c != '"' && 
				openingCurlyBraces == 0 && openingBrackets == 0) 
			{
				strValue += c;
			}
			else if (c == '"' && openQuote && !inProp && inValue && jsonMode == JsonType.String 
				&& openingCurlyBraces == 0 && openingBrackets == 0) 
			{
				openQuote = false;
			}
			else if (c == ',' && !inProp && inValue && jsonMode == JsonType.String 
				&& openingCurlyBraces == 0 && openingBrackets == 0) 
			{
				if (json.type == JSONObject.Type.ARRAY) {
					json.Add (strValue);
					inValue = true;
				}
				else 
				{
					if (inBrackets) {
						inValue = true;
						json.Add (strValue);
					} else {
						inValue = false;
						json.SetField (propName, strValue);
					}
				}
				strValue = "";
			}
			else if (c == ',' && !inProp && inValue && jsonMode == JsonType.Int 
				&& openingCurlyBraces == 0 && openingBrackets == 0) 
			{
				intValue = Convert.ToInt32 (tempValue.Trim ());
				tempValue = "";

				if (json.type == JSONObject.Type.ARRAY)
					json.Add (intValue);
				else 
				{
					if (inBrackets) {
						inValue = true;
						json.Add (intValue);
					} else {
						inValue = false;
						json.SetField (propName, intValue);
					}
				}
			}
			else if (c == ',' && !inProp && inValue && jsonMode == JsonType.Float 
				&& openingCurlyBraces == 0 && openingBrackets == 0) 
			{
				floatValue = Convert.ToSingle (tempValue.Trim ());
				tempValue = "";
				//floatValue = float.Parse (tempValue.Trim (), System.Globalization.CultureInfo.InvariantCulture);
				if (json.type == JSONObject.Type.ARRAY)
					json.Add (floatValue);
				else 
				{
					if (inBrackets) {
						inValue = true;
						json.Add (floatValue);
					} else {
						inValue = false;
						json.SetField (propName, floatValue);
					}
				}
			}
			else if (c == ',' && !inProp && inValue && jsonMode == JsonType.NotSet
				&& openingCurlyBraces == 0 && openingBrackets == 0) 
			{
				if (inBrackets) 
					inValue = true;
				else 
					inValue = false;

				if (tempValue.Trim ().ToLower ().Equals ("null")) 
				{
					if (json.type == JSONObject.Type.ARRAY)
						json.Add ("null");
					else 
					{
						if (inBrackets)
							json.Add ("null");
						else
							json.SetField (propName, "null");
					}
				}
				else if (tempValue.Trim ().ToLower ().Equals ("false") || 
					tempValue.Trim ().ToLower ().Equals ("true")) 
				{
					boolValue = Convert.ToBoolean (tempValue.Trim ());
					if (json.type == JSONObject.Type.ARRAY)
						json.Add (boolValue);
					else 
					{
						if (inBrackets)
							json.Add (boolValue);
						else
							json.SetField (propName, boolValue);
					}
				}
				tempValue = "";
			}
			else if (c == '{' && openingBrackets == 0) {
				if (openingCurlyBraces == 0)
					firstCurlyBracesPosition = charPosition + 1;
				openingCurlyBraces++;
			}
			else if (c == '}' && openingBrackets == 0) {
				openingCurlyBraces--;
				if (openingCurlyBraces == 0) {
					jsonElement = "";
					endingCurlyBracesPosition = charPosition;
					endingCurlyBracesComputedPosition = endingCurlyBracesPosition - firstCurlyBracesPosition;
					jsonElement = jsonStr.Substring(firstCurlyBracesPosition, endingCurlyBracesComputedPosition);
					JSONObject jsonObject = new JSONObject ();
					jsonObject = createJsonObject (jsonObject, jsonElement);

					if (json.type == JSONObject.Type.ARRAY)
						json.Add (jsonObject);
					else 
					{
						if (propName != "")
							json.SetField (propName, jsonObject);
						else 
							json = jsonObject;
					}
					inValue = false;
				}
			}
			else if (c == '[' && openingCurlyBraces == 0) {
				if (openingBrackets == 0)
					firstBracketsPosition = charPosition;
				openingBrackets++;
			}
			else if (c == ']' && openingCurlyBraces == 0) {
				openingBrackets--;
				if (openingBrackets == 0) {
					endingBracketsPosition = charPosition + 1;
					if ((firstBracketsPosition + 2) != endingBracketsPosition) {
						endingBracketsComputedPosition = endingBracketsPosition - firstBracketsPosition;
						jsonElement = jsonStr.Substring (firstBracketsPosition, endingBracketsComputedPosition);
						JSONObject jsonObjectArray = JSONObject.arr;
						inBrackets = true;
						inValue = true;
						jsonElement = jsonElement.TrimStart ('[');
						jsonElement = jsonElement.TrimEnd (']');
						jsonObjectArray = createJsonObject (jsonObjectArray, jsonElement);

						if (json.type == JSONObject.Type.ARRAY)
							json.Add (jsonObjectArray);
						else 
							json.SetField (propName, jsonObjectArray);
		
						inBrackets = false;
						inValue = false;
					} else {
						JSONObject jsonObjectArray = JSONObject.arr;
						json.SetField (propName, jsonObjectArray);
					}
					inValue = false;
				}
			}
			else if (!openQuote && !inProp && inValue && 
				openingCurlyBraces == 0 && openingBrackets == 0) 
			{
				tempValue += c;
				jsonMode = JsonType.NotSet;
			}
			charPosition++;
		}
		if (inValue) 
		{
			if (jsonMode == JsonType.Int) {
				intValue = Convert.ToInt32 (tempValue.Trim ());
				if (json.type == JSONObject.Type.ARRAY)
					json.Add (intValue);
				else 
					json.SetField (propName, intValue);
			}
			else if (jsonMode == JsonType.Float) {
				floatValue = Convert.ToSingle (tempValue.Trim ());
				//floatValue = float.Parse (tempValue.Trim (), System.Globalization.CultureInfo.InvariantCulture);
				if (json.type == JSONObject.Type.ARRAY)
					json.Add (floatValue);
				else 
					json.SetField (propName, floatValue);
			}
			else if (jsonMode == JsonType.String) {
				if (json.type == JSONObject.Type.ARRAY)
					json.Add (strValue);
				else 
					json.SetField (propName, strValue);
				strValue = "";
			}
			else if (jsonMode == JsonType.NotSet) {
				inValue = false;
				if (tempValue.Trim ().ToLower ().Equals ("null")) 
				{
					if (json.type == JSONObject.Type.ARRAY)
						json.Add ("null");
					else
						json.SetField (propName, "null");
				}
				else if (tempValue.Trim ().ToLower ().Equals ("false") || 
					tempValue.Trim ().ToLower ().Equals ("true")) 
				{
					boolValue = Convert.ToBoolean (tempValue.Trim ());
					{
						if (json.type == JSONObject.Type.ARRAY)
							json.Add (boolValue);
						else
							json.SetField (propName, boolValue);
					}
				}
			}						
			inValue = false;
		}
		return json;			
	}


	public JSONObject createJson(JSONObject json, string jsonStr)
	{
		if (jsonStr.StartsWith ("[")) 
		{
			json = JSONObject.arr; 
			jsonStr = jsonStr.TrimStart ('[');
			jsonStr = jsonStr.TrimEnd (']');
		} 
		else 
			json = new JSONObject ();

		json = createJsonObject (json, jsonStr);
		return json;
	}
}
