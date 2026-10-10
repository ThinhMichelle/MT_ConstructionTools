using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

public static class ProjectSettingsManager
{
    private static readonly Guid SchemaGuid =
        new Guid("B5D5D2B8-7F4A-4E0E-9B2A-123456789ABC");

    private const string SchemaName =
        "ProjectSettings";

    private const string JsonFieldName =
        "Json";


    #region Schema

    private static Schema GetOrCreateSchema()
    {
        Schema schema = Schema.Lookup(SchemaGuid);

        if (schema != null)
            return schema;

        SchemaBuilder builder =
            new SchemaBuilder(SchemaGuid);

        builder.SetSchemaName(SchemaName);

        builder.AddSimpleField(
            JsonFieldName,
            typeof(string));

        builder.SetReadAccessLevel(
            AccessLevel.Public);

        builder.SetWriteAccessLevel(
            AccessLevel.Public);

        return builder.Finish();
    }

    #endregion


    #region Save

    public static void Save<T>(Document doc,string key,T settings)
    {
        if (doc == null)
            throw new ArgumentNullException(nameof(doc));

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException(
                "Settings key is empty.",
                nameof(key));

        if (settings == null) throw new ArgumentNullException(nameof(settings));


        Schema schema = GetOrCreateSchema();

        DataStorage storage = FindDataStorage(doc, schema);


        Dictionary<string, string> allSettings = LoadAll(doc);


        // Serialize riêng phần setting
        string json = JsonConvert.SerializeObject(settings);


        // Add / Update
        allSettings[key] = json;


        // Serialize toàn bộ dictionary
        string finalJson = JsonConvert.SerializeObject(allSettings, Formatting.Indented);


        using (Transaction transaction = new Transaction(doc, "Save Project Settings"))
        {
            transaction.Start();

            if (storage == null)
            {
                storage = DataStorage.Create(doc);
            }

            Entity entity = new Entity(schema);

            Field field = schema.GetField(JsonFieldName);

            entity.Set(field, finalJson);

            storage.SetEntity(entity);

            transaction.Commit();
        }
    }

    #endregion


    #region Load

    public static T Load<T>(Document doc,string key)
    {
        if (doc == null) throw new ArgumentNullException(nameof(doc));

        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Settings key is empty.",nameof(key));


        Dictionary<string, string> allSettings = LoadAll(doc);


        if (!allSettings.TryGetValue(key,out string json))
        {
            return default;
        }


        if (string.IsNullOrWhiteSpace(json)) return default;


        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch
        {
            return default;
        }
    }

    #endregion


    #region Load All

    private static Dictionary<string, string> LoadAll(Document doc)
    {
        Schema schema = Schema.Lookup(SchemaGuid);

        if (schema == null)
        {
            return new Dictionary<string, string>();
        }


        DataStorage storage = FindDataStorage(doc, schema);

        if (storage == null)
        {
            return new Dictionary<string, string>();
        }


        Entity entity = storage.GetEntity(schema);

        if (!entity.IsValid())
        {
            return new Dictionary<string, string>();
        }


        Field field = schema.GetField(JsonFieldName);

        string json = entity.Get<string>(field);


        if (string.IsNullOrWhiteSpace(json))
        {
            return new Dictionary<string, string>();
        }


        try
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }

    #endregion


    #region Find DataStorage

    private static DataStorage FindDataStorage(Document doc, Schema schema)
    {
        return new FilteredElementCollector(doc)
            .OfClass(typeof(DataStorage))
            .Cast<DataStorage>()
            .FirstOrDefault(x =>
            {
                Entity entity = x.GetEntity(schema);

                return entity.IsValid();
            });
    }

    #endregion
}