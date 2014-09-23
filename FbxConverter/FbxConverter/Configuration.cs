using System;
using System.Collections.Generic;
using System.Configuration;
using FaustGames.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FbxConverter
{
    public enum VertexDeclarationTypes
    {
        Position,
        Normal,
        Tangent,
        BiTangent,
        VertexColor,
        TexturePosition2D,
        MeshIndex,
    }

    public class VertexDeclaration
    {
        private VertexDeclarationTypes _type;

        [JsonConverter(typeof(StringEnumConverter))]
        public VertexDeclarationTypes Type
        {
            get { return _type; }
            set 
            { 
                _type = value;
                Configurations.Save();
            }
        }

        public override string ToString()
        {
            return _type.ToString();
        }
    }

    public class Configuration
    {
        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Configurations.Save();
            }
        }

        [JsonProperty("vertexDeclarations")]
        public VertexDeclaration[] VertexDeclarations
        {
            get { return _vertexDeclarations.ToArray(); }
            set
            {
                _vertexDeclarations = new List<VertexDeclaration>(value);
                Configurations.Save();
            }
        }

        private List<VertexDeclaration> _vertexDeclarations = new List<VertexDeclaration>();
        private string _name;
        private bool _createMeshTransforms;

        public void Add(VertexDeclaration declaration)
        {
            _vertexDeclarations.Add(declaration);
            Configurations.Save();
        }

        public void Remove(VertexDeclaration declaration)
        {
            _vertexDeclarations.Remove(declaration);
            Configurations.Save();
        }
        public override string ToString()
        {
            return Name;
        }
    }

    public class Configurations
    {
        private List<Configuration> _values = new List<Configuration>();

        [JsonProperty("values")]
        public Configuration[] Values
        {
            get { return _values.ToArray(); }
            set { _values = new List<Configuration>(value); }
        }

        public void Add(Configuration configuration)
        {
            _values.Add(configuration);
            Configurations.Save();
        }

        public void Remove(Configuration configuration)
        {
            _values.Remove(configuration);
            Configurations.Save();
        }

        static ApplicationSettingsBase Settings
        {
            get { return SettingsService.Settings; }
        }

        static string PropertyName
        {
            get
            {
                return "ConfigurationsData";
            }
        }

        static SettingsProperty SettingsProperty
        {
            get
            {
                var p = Settings.Properties[PropertyName];
                if (p == null)
                {
                    p = new SettingsProperty(PropertyName)
                    {
                        DefaultValue = "",
                        IsReadOnly = false,
                        PropertyType = typeof(string),
                        Provider = Settings.Providers["LocalFileSettingsProvider"]
                    };
                    p.Attributes.Add(typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute());
                    Settings.Properties.Add(p);
                    Settings.Reload();
                }
                return p;
            }
        }

        public static Configurations Instance = new Configurations();

        private static bool _suspendSettingsProcessing;

        public static void Load()
        {
            if (_suspendSettingsProcessing) return;
            _suspendSettingsProcessing = true;
            Settings.Reload();
            var text = (string)Settings[SettingsProperty.Name];
            if (!string.IsNullOrEmpty(text))
                Instance = JsonConvert.DeserializeObject<Configurations>(text);
            if (Instance == null)
                Instance = new Configurations();
            _suspendSettingsProcessing = false;
        }

        public static EventHandler AfterSave;

        public static void Save()
        {
            if (_suspendSettingsProcessing) return;
            _suspendSettingsProcessing = true;
            Settings[SettingsProperty.Name] = JsonConvert.SerializeObject(Instance);
            Settings.Save();
            _suspendSettingsProcessing = false;
            if (AfterSave != null)
                AfterSave(null, EventArgs.Empty);
        }
    }
}
