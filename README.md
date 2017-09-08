# JSON-Object-Parser
C# JSON Parser

A parser for the JSONObject Unity 3D Asset
https://assetstore.unity.com/packages/tools/input-management/json-object-710
The GitHub repository is https://github.com/mtschoen/JSONObject

Special Thanks to C Me Local World Wide Marketing LLC for allowing me to release this code library open source.
Visit us at https://www.cmelocal.com

Example of using the ParseJson class from JSON Object Parser for JSON Object library.
Reading in a Json string that gets converted properly to json objects:

string jsonStr = "{ "name":"John", "age":31, "city":"New York", "longitude":76.2345, "male": true }";
Values = new JSONObject ();
ParseJson parse = new ParseJson ();
Values = parse.createJson(Values, jsonStr);

Now it is just standard JSON Object access after that:
if (Values.HasField ("name")) 
{
	string name = Values.GetField ("name").str;
	int age = Convert.ToSingle (Values.GetField ("age").n);
	float longitude = Convert.ToSingle (Values.GetField ("longitude").n);
	bool isMale = Values.GetField ("male").b;
}


Loop Example with a list of results for a JSON Tag results in the original json string.
JSONObject results = Values.GetField ("results");
foreach (JSONObject result in results.list) 
{
	if (result.HasField ("locations")) 
	{
	 	JSONObject locations = result.GetField ("locations");
	}
}
     
