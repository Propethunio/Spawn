using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpawnerObjectList")]
public class ObjectListSO : ScriptableObject {

    [field: SerializeField] public List<GameObject> objectsList { get; private set; }
    [field: SerializeField] public string objectName { get; private set; }
}