using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Tilemaps
{
    [Serializable]
    internal enum GridInformationType
    {
        Integer,
        String,
        Float,
        Double,
        UnityObject,
        Color
    }

    /// <summary>
    /// A simple MonoBehaviour that stores and provides information based on Grid positions and keywords.
    /// </summary>
    [Serializable]
    [HelpURL(
        "https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@latest/index.html?subfolder=/manual/GridInformation.html")]
    [AddComponentMenu("Tilemap/Grid Information")]
    public class GridInformation : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField] [HideInInspector] private List<GridInformationKey> m_PositionIntKeys = new();

        [SerializeField] [HideInInspector] private List<int> m_PositionIntValues = new();

        [SerializeField] [HideInInspector] private List<GridInformationKey> m_PositionStringKeys = new();

        [SerializeField] [HideInInspector] private List<string> m_PositionStringValues = new();

        [SerializeField] [HideInInspector] private List<GridInformationKey> m_PositionFloatKeys = new();

        [SerializeField] [HideInInspector] private List<float> m_PositionFloatValues = new();

        [SerializeField] [HideInInspector] private List<GridInformationKey> m_PositionDoubleKeys = new();

        [SerializeField] [HideInInspector] private List<double> m_PositionDoubleValues = new();

        [SerializeField] [HideInInspector] private List<GridInformationKey> m_PositionObjectKeys = new();

        [SerializeField] [HideInInspector] private List<Object> m_PositionObjectValues = new();

        [SerializeField] [HideInInspector] private List<GridInformationKey> m_PositionColorKeys = new();

        [SerializeField] [HideInInspector] private List<Color> m_PositionColorValues = new();

        internal Dictionary<GridInformationKey, GridInformationValue> PositionProperties { get; } = new();

        /// <summary>
        ///     Clears all information stored
        /// </summary>
        public virtual void Reset()
        {
            PositionProperties.Clear();
        }

        /// <summary>
        /// Callback before serializing this GridInformation
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_PositionIntKeys.Clear();
            m_PositionIntValues.Clear();
            m_PositionStringKeys.Clear();
            m_PositionStringValues.Clear();
            m_PositionFloatKeys.Clear();
            m_PositionFloatValues.Clear();
            m_PositionDoubleKeys.Clear();
            m_PositionDoubleValues.Clear();
            m_PositionObjectKeys.Clear();
            m_PositionObjectValues.Clear();
            m_PositionColorKeys.Clear();
            m_PositionColorValues.Clear();

            foreach (var kvp in PositionProperties)
                switch (kvp.Value.type)
                {
                    case GridInformationType.Integer:
                        m_PositionIntKeys.Add(kvp.Key);
                        m_PositionIntValues.Add((int)kvp.Value.data);
                        break;
                    case GridInformationType.String:
                        m_PositionStringKeys.Add(kvp.Key);
                        m_PositionStringValues.Add(kvp.Value.data as string);
                        break;
                    case GridInformationType.Float:
                        m_PositionFloatKeys.Add(kvp.Key);
                        m_PositionFloatValues.Add((float)kvp.Value.data);
                        break;
                    case GridInformationType.Double:
                        m_PositionDoubleKeys.Add(kvp.Key);
                        m_PositionDoubleValues.Add((double)kvp.Value.data);
                        break;
                    case GridInformationType.Color:
                        m_PositionColorKeys.Add(kvp.Key);
                        m_PositionColorValues.Add((Color)kvp.Value.data);
                        break;
                    default:
                        m_PositionObjectKeys.Add(kvp.Key);
                        m_PositionObjectValues.Add(kvp.Value.data as Object);
                        break;
                }
        }

        /// <summary>
        /// Callback after deserializing this GridInformation
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            PositionProperties.Clear();
            for (var i = 0; i != Math.Min(m_PositionIntKeys.Count, m_PositionIntValues.Count); i++)
            {
                GridInformationValue positionValue;
                positionValue.type = GridInformationType.Integer;
                positionValue.data = m_PositionIntValues[i];
                PositionProperties.Add(m_PositionIntKeys[i], positionValue);
            }

            for (var i = 0; i != Math.Min(m_PositionStringKeys.Count, m_PositionStringValues.Count); i++)
            {
                GridInformationValue positionValue;
                positionValue.type = GridInformationType.String;
                positionValue.data = m_PositionStringValues[i];
                PositionProperties.Add(m_PositionStringKeys[i], positionValue);
            }

            for (var i = 0; i != Math.Min(m_PositionFloatKeys.Count, m_PositionFloatValues.Count); i++)
            {
                GridInformationValue positionValue;
                positionValue.type = GridInformationType.Float;
                positionValue.data = m_PositionFloatValues[i];
                PositionProperties.Add(m_PositionFloatKeys[i], positionValue);
            }

            for (var i = 0; i != Math.Min(m_PositionDoubleKeys.Count, m_PositionDoubleValues.Count); i++)
            {
                GridInformationValue positionValue;
                positionValue.type = GridInformationType.Double;
                positionValue.data = m_PositionDoubleValues[i];
                PositionProperties.Add(m_PositionDoubleKeys[i], positionValue);
            }

            for (var i = 0; i != Math.Min(m_PositionObjectKeys.Count, m_PositionObjectValues.Count); i++)
            {
                GridInformationValue positionValue;
                positionValue.type = GridInformationType.UnityObject;
                positionValue.data = m_PositionObjectValues[i];
                PositionProperties.Add(m_PositionObjectKeys[i], positionValue);
            }

            for (var i = 0; i != Math.Min(m_PositionColorKeys.Count, m_PositionColorValues.Count); i++)
            {
                GridInformationValue positionValue;
                positionValue.type = GridInformationType.Color;
                positionValue.data = m_PositionColorValues[i];
                PositionProperties.Add(m_PositionColorKeys[i], positionValue);
            }
        }

        /// <summary>
        /// This is not supported.
        /// </summary>
        /// <param name="position">Position to store information for</param>
        /// <param name="name">Property name to store information for</param>
        /// <param name="positionProperty">The information to be stored at the position</param>
        /// <typeparam name="T">Type of the information to set</typeparam>
        /// <returns>Whether the information was set</returns>
        /// <exception cref="NotImplementedException">This is not implemented as only concrete Types are supported</exception>
        public bool SetPositionProperty<T>(Vector3Int position, string name, T positionProperty)
        {
            throw new NotImplementedException("Storing this type is not accepted in GridInformation");
        }

        /// <summary>
        /// Stores int information at the given position with the given property name
        /// </summary>
        /// <param name="position">Position to store information for</param>
        /// <param name="name">Property name to store information for</param>
        /// <param name="positionProperty">The information to be stored at the position</param>
        /// <returns>Whether the information was set</returns>
        public bool SetPositionProperty(Vector3Int position, string name, int positionProperty)
        {
            return SetPositionProperty(position, name, GridInformationType.Integer, positionProperty);
        }

        /// <summary>
        /// Stores string information at the given position with the given property name
        /// </summary>
        /// <param name="position">Position to store information for</param>
        /// <param name="name">Property name to store information for</param>
        /// <param name="positionProperty">The information to be stored at the position</param>
        /// <returns>Whether the information was set</returns>
        public bool SetPositionProperty(Vector3Int position, string name, string positionProperty)
        {
            return SetPositionProperty(position, name, GridInformationType.String, positionProperty);
        }

        /// <summary>
        /// Stores float information at the given position with the given property name
        /// </summary>
        /// <param name="position">Position to store information for</param>
        /// <param name="name">Property name to store information for</param>
        /// <param name="positionProperty">The information to be stored at the position</param>
        /// <returns>Whether the information was set</returns>
        public bool SetPositionProperty(Vector3Int position, string name, float positionProperty)
        {
            return SetPositionProperty(position, name, GridInformationType.Float, positionProperty);
        }

        /// <summary>
        /// Stores double information at the given position with the given property name
        /// </summary>
        /// <param name="position">Position to store information for</param>
        /// <param name="name">Property name to store information for</param>
        /// <param name="positionProperty">The information to be stored at the position</param>
        /// <returns>Whether the information was set</returns>
        public bool SetPositionProperty(Vector3Int position, string name, double positionProperty)
        {
            return SetPositionProperty(position, name, GridInformationType.Double, positionProperty);
        }

        /// <summary>
        /// Stores UnityEngine.Object information at the given position with the given property name
        /// </summary>
        /// <param name="position">Position to store information for</param>
        /// <param name="name">Property name to store information for</param>
        /// <param name="positionProperty">The information to be stored at the position</param>
        /// <returns>Whether the information was set</returns>
        public bool SetPositionProperty(Vector3Int position, string name, Object positionProperty)
        {
            return SetPositionProperty(position, name, GridInformationType.UnityObject, positionProperty);
        }

        /// <summary>
        /// Stores color information at the given position with the given property name
        /// </summary>
        /// <param name="position">Position to store information for</param>
        /// <param name="name">Property name to store information for</param>
        /// <param name="positionProperty">The information to be stored at the position</param>
        /// <returns>Whether the information was set</returns>
        public bool SetPositionProperty(Vector3Int position, string name, Color positionProperty)
        {
            return SetPositionProperty(position, name, GridInformationType.Color, positionProperty);
        }

        private bool SetPositionProperty(Vector3Int position, string name, GridInformationType dataType,
            object positionProperty)
        {
            var grid = GetComponentInParent<Grid>();
            if (grid != null && positionProperty != null)
            {
                GridInformationKey positionKey;
                positionKey.position = position;
                positionKey.name = name;

                GridInformationValue positionValue;
                positionValue.type = dataType;
                positionValue.data = positionProperty;

                PositionProperties[positionKey] = positionValue;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retrieves information stored at the given position with the given property name as the given Type
        /// </summary>
        /// <param name="position">Position to retrieve information for</param>
        /// <param name="name">Property name to retrieve information for</param>
        /// <param name="defaultValue">Default value if property does not exist at the given position</param>
        /// <typeparam name="T">Type of the information to retrieve</typeparam>
        /// <returns>The information stored at the position</returns>
        /// <exception cref="InvalidCastException">Thrown when information to be retrieved is not the given Type</exception>
        public T GetPositionProperty<T>(Vector3Int position, string name, T defaultValue) where T : Object
        {
            GridInformationKey positionKey;
            positionKey.position = position;
            positionKey.name = name;

            GridInformationValue positionValue;
            if (PositionProperties.TryGetValue(positionKey, out positionValue))
            {
                if (positionValue.type != GridInformationType.UnityObject)
                    throw new InvalidCastException("Value stored in GridInformation is not of the right type");
                return positionValue.data as T;
            }

            return defaultValue;
        }

        /// <summary>
        /// Retrieves int information stored at the given position with the given property name
        /// </summary>
        /// <param name="position">Position to retrieve information for</param>
        /// <param name="name">Property name to retrieve information for</param>
        /// <param name="defaultValue">Default int if property does not exist at the given position</param>
        /// <returns>The int stored at the position</returns>
        /// <exception cref="InvalidCastException">Thrown when information to be retrieved is not a int</exception>
        public int GetPositionProperty(Vector3Int position, string name, int defaultValue)
        {
            GridInformationKey positionKey;
            positionKey.position = position;
            positionKey.name = name;

            GridInformationValue positionValue;
            if (PositionProperties.TryGetValue(positionKey, out positionValue))
            {
                if (positionValue.type != GridInformationType.Integer)
                    throw new InvalidCastException("Value stored in GridInformation is not of the right type");
                return (int)positionValue.data;
            }

            return defaultValue;
        }

        /// <summary>
        /// Retrieves string information stored at the given position with the given property name
        /// </summary>
        /// <param name="position">Position to retrieve information for</param>
        /// <param name="name">Property name to retrieve information for</param>
        /// <param name="defaultValue">Default string if property does not exist at the given position</param>
        /// <returns>The string stored at the position</returns>
        /// <exception cref="InvalidCastException">Thrown when information to be retrieved is not a string</exception>
        public string GetPositionProperty(Vector3Int position, string name, string defaultValue)
        {
            GridInformationKey positionKey;
            positionKey.position = position;
            positionKey.name = name;

            GridInformationValue positionValue;
            if (PositionProperties.TryGetValue(positionKey, out positionValue))
            {
                if (positionValue.type != GridInformationType.String)
                    throw new InvalidCastException("Value stored in GridInformation is not of the right type");
                return (string)positionValue.data;
            }

            return defaultValue;
        }

        /// <summary>
        /// Retrieves float information stored at the given position with the given property name
        /// </summary>
        /// <param name="position">Position to retrieve information for</param>
        /// <param name="name">Property name to retrieve information for</param>
        /// <param name="defaultValue">Default float if property does not exist at the given position</param>
        /// <returns>The float stored at the position</returns>
        /// <exception cref="InvalidCastException">Thrown when information to be retrieved is not a float</exception>
        public float GetPositionProperty(Vector3Int position, string name, float defaultValue)
        {
            GridInformationKey positionKey;
            positionKey.position = position;
            positionKey.name = name;

            GridInformationValue positionValue;
            if (PositionProperties.TryGetValue(positionKey, out positionValue))
            {
                if (positionValue.type != GridInformationType.Float)
                    throw new InvalidCastException("Value stored in GridInformation is not of the right type");
                return (float)positionValue.data;
            }

            return defaultValue;
        }

        /// <summary>
        /// Retrieves double information stored at the given position with the given property name
        /// </summary>
        /// <param name="position">Position to retrieve information for</param>
        /// <param name="name">Property name to retrieve information for</param>
        /// <param name="defaultValue">Default double if property does not exist at the given position</param>
        /// <returns>The double stored at the position</returns>
        /// <exception cref="InvalidCastException">Thrown when information to be retrieved is not a double</exception>
        public double GetPositionProperty(Vector3Int position, string name, double defaultValue)
        {
            GridInformationKey positionKey;
            positionKey.position = position;
            positionKey.name = name;

            GridInformationValue positionValue;
            if (PositionProperties.TryGetValue(positionKey, out positionValue))
            {
                if (positionValue.type != GridInformationType.Double)
                    throw new InvalidCastException("Value stored in GridInformation is not of the right type");
                return (double)positionValue.data;
            }

            return defaultValue;
        }

        /// <summary>
        /// Retrieves Color information stored at the given position with the given property name
        /// </summary>
        /// <param name="position">Position to retrieve information for</param>
        /// <param name="name">Property name to retrieve information for</param>
        /// <param name="defaultValue">Default color if property does not exist at the given position</param>
        /// <returns>The color stored at the position</returns>
        /// <exception cref="InvalidCastException">Thrown when information to be retrieved is not a Color</exception>
        public Color GetPositionProperty(Vector3Int position, string name, Color defaultValue)
        {
            GridInformationKey positionKey;
            positionKey.position = position;
            positionKey.name = name;

            GridInformationValue positionValue;
            if (PositionProperties.TryGetValue(positionKey, out positionValue))
            {
                if (positionValue.type != GridInformationType.Color)
                    throw new InvalidCastException("Value stored in GridInformation is not of the right type");
                return (Color)positionValue.data;
            }

            return defaultValue;
        }

        /// <summary>
        /// Erases information stored at the given position with the given property name
        /// </summary>
        /// <param name="position">Position to erase</param>
        /// <param name="name">Property name to erase</param>
        /// <returns>Whether the information was erased</returns>
        public bool ErasePositionProperty(Vector3Int position, string name)
        {
            GridInformationKey positionKey;
            positionKey.position = position;
            positionKey.name = name;
            return PositionProperties.Remove(positionKey);
        }

        /// <summary>
        /// Gets all positions with information with the given property name
        /// </summary>
        /// <param name="propertyName">Property name to search for</param>
        /// <returns>An array of all positions with the property name</returns>
        public Vector3Int[] GetAllPositions(string propertyName)
        {
            return PositionProperties.Keys.ToList().FindAll(x => x.name == propertyName).Select(x => x.position)
                .ToArray();
        }

        [Serializable]
        internal struct GridInformationValue
        {
            public GridInformationType type;
            public object data;
        }

        [Serializable]
        internal struct GridInformationKey : IEquatable<GridInformationKey>
        {
            public Vector3Int position;
            public string name;

            public bool Equals(GridInformationKey key)
            {
                return position == key.position && name == key.name;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(position.GetHashCode(), name.GetHashCode());
            }
        }
    }
}
