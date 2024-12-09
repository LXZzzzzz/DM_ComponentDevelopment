using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ToolsLibrary.EquipPart;

public class PolymorphicConverter_ZyVariableDataBase : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(ZyVariableDataBase).IsAssignableFrom(objectType);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        ZiYuanType zyType = (ZiYuanType)jsonObject["ZyType"].Value<int>();

        switch (zyType)
        {
            case ZiYuanType.SourceOfAFire:
                var fireData = new FireVariableData();
                fireData.ZyName = jsonObject["ZyName"].ToString();
                fireData.fs = jsonObject["fs"].Value<float>();
                fireData.pd = jsonObject["pd"].Value<float>();
                fireData.csrsmj = jsonObject["csrsmj"].Value<float>();
                return fireData;
            case ZiYuanType.DisasterArea:
                var disData = new DisasterVariableData();
                disData.ZyName = jsonObject["ZyName"].ToString();
                disData.type = jsonObject["type"].Value<int>();
                disData.personNum = jsonObject["personNum"].Value<int>();
                return disData;
            default:
                throw new JsonSerializationException("Unknown type discriminator: " + zyType);
        }
    }
}