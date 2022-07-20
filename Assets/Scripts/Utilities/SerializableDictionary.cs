using System.Collections.Generic;

/// <summary>
/// Class <c>KeyValueEntry</c>
/// Class that represents an element in the dictionary
/// </summary>
[System.Serializable]
public class KeyValueEntry<K, V>
{
    public K Key;
    public V Value;

    /// <summary>
    /// Constructor <c>KeyValueEntry</c>
    /// </summary>
    /// <param name="key"> the element's key </param>
    /// <param name="value"> the element's value </param>
    public KeyValueEntry(K key, V value)
    {
        Key = key;
        Value = value;
    }
}

/// <summary>
/// Class <c>SerializableDictionary</c>
/// Class that represents the serializable dictionary
/// </summary>
[System.Serializable]
public class SerializableDictionary<K, V>
{
    public List<KeyValueEntry<K, V>> Dict;

    /// <summary>
    /// Constructor <c>SerializableDictionary</c>
    /// </summary>
    public SerializableDictionary()
    {
        Dict = new List<KeyValueEntry<K, V>>();
    }

    /// <summary>
    /// Procedure <c>Add</c>
    /// Procedure that adds an element in the dictionary with its value
    /// </summary>
    /// <param name="key"> the element's key to be added </param>
    /// <param name="value"> the element's value </param>
    public void Add(K key, V value)
    {
        if (!KeyExists(key))
        {
            KeyValueEntry<K, V> newEntry = new KeyValueEntry<K, V>(key, value);
            Dict.Add(newEntry);
        }  
    }

    /// <summary>
    /// Function <c>Get</c>
    /// Function that puts the element's value in the parameter "value"
    /// </summary>
    /// <param name="key"> the element's key to take the istances from </param>
    /// <param name="value"> the element's value </param>
    /// <returns> true if the dictionary contains the key, false otherwise </returns>
    public bool Get(K key, ref V value)
    {
        foreach (KeyValueEntry<K, V> entry in Dict)
        {
            if (KeysAreEquals(entry.Key, key))
            {
                value = entry.Value;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Procedure <c>Remove</c>
    /// Procedure that removes an element in the dictionary
    /// </summary>
    /// <param name="key"> the element's key to be removed </param>
    public void Remove(K key)
    {
        Dict.RemoveAll(entry => KeysAreEquals(entry.Key, key));
    }

    /// <summary>
    /// Procedure <c>ChangeValue</c>
    /// Procedure that changes the value of an entry, if the given key is in the dictionary.
    /// </summary>
    /// <param name="key"> the element's key </param>
    /// <param name="newValue"> the new element's value </param>
    public void ChangeValue(K key, V newValue)
    {
        if (KeyExists(key))
        {
            foreach (KeyValueEntry<K, V> entry in Dict)
            {
                if (KeysAreEquals(entry.Key, key))
                {
                    entry.Value = newValue;
                    break;
                }
            }
        }   
    }

    /// <summary>
    /// Function <c>KeyExists</c>
    /// Function that returns true if the dictionary contains the key, otherwise it returns false
    /// </summary>
    /// <param name="key"> the element's key </param>
    /// <returns> true if the dictionary contains the key, false otherwise </returns>
    public bool KeyExists(K key)
    {
        foreach (KeyValueEntry<K, V> entry in Dict)
        {
            if (KeysAreEquals(entry.Key, key))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Function <c>KeysAreEquals</c>
    /// Function that returns true if the keys are equals, false otherwise
    /// </summary>
    /// <param name="key1"> the first key </param>
    /// <param name="key2"> the second key </param>
    /// <returns> true if the keys are equals, false otherwise </returns>
    private bool KeysAreEquals(K key1, K key2)
    {
        if (key1 == null)
        {
            if (key2 == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return key1.Equals(key2);
        }
    }

    /// <summary>
    /// Function <c>ToDictionary</c>
    /// Function that transform the serializable dictionary into a classic dictionary
    /// </summary>
    /// <returns> a classic dictionary with the same elements of the serializable dictionary </returns>
    public Dictionary<K, V> ToDictionary()
    {
        Dictionary<K, V> dictionary = new Dictionary<K, V>();

        foreach (KeyValueEntry<K, V> entry in Dict)
        {
            dictionary.Add(entry.Key, entry.Value);
        }

        return dictionary;
    }
}
