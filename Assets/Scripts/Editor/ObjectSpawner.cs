using UnityEditor;
using UnityEngine;

public class ObjectSpawner : EditorWindow {

    bool SceneClicks;
    ObjectListSO objectsToSpawn;
    int numberOfObjects = 1;
    float spawnRadious = 5f;
    float minScaleLimit = .4f;
    float maxScaleLimit = 3f;
    float minObjectScale;
    float maxObjectScale;
    Vector3? clickPosition = null;

    [MenuItem("Tools/ObjectPainter")]
    public static void ShowWindow() {
        GetWindow(typeof(ObjectSpawner));
    }
    void OnEnable() {
        SceneView.duringSceneGui += OnSceneGUI;
        minObjectScale = minScaleLimit;
        maxObjectScale = maxScaleLimit;
    }

    void OnDisable() {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnGUI() {
        GUILayout.Label("Spawn New Objects", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        SceneClicks = EditorGUILayout.Toggle("Scene Clicks", SceneClicks);
        objectsToSpawn = EditorGUILayout.ObjectField("Objects To Spawn", objectsToSpawn, typeof(ObjectListSO), false) as ObjectListSO;
        numberOfObjects = EditorGUILayout.IntField("Number of Objects", numberOfObjects);
        spawnRadious = EditorGUILayout.FloatField("Spawn Radious", spawnRadious);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Min: " + minScaleLimit);
        EditorGUILayout.MinMaxSlider(ref minObjectScale, ref maxObjectScale, minScaleLimit, maxScaleLimit);
        EditorGUILayout.PrefixLabel("Max: " + maxScaleLimit);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Min Value: " + minObjectScale);
        EditorGUILayout.LabelField("Max Value: " + maxObjectScale);
        EditorGUILayout.EndHorizontal();

        if(GUILayout.Button("Spawn")) {
            SpawnObjects();
        }
    }

    void OnSceneGUI(SceneView sceneView) {
        if(clickPosition.HasValue) {
            Handles.color = Color.red;
            Handles.DrawSolidDisc(clickPosition.Value, Vector3.up, 0.1f);
            Handles.color = Color.green;
            Handles.DrawWireDisc(clickPosition.Value, Vector3.up, spawnRadious);
        }

        sceneView.Repaint();

        if(!SceneClicks) {
            return;
        }
        Event e = Event.current;

        if(e.type == EventType.MouseDown && e.button == 0) {
            HandleMouseClick(e);
        }

        if(e.type == EventType.KeyDown && e.keyCode == KeyCode.Space) {
            SpawnObjects();
        }
    }

    void HandleMouseClick(Event e) {
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 3)) {
            clickPosition = hit.point;
        }
    }

    void SpawnObjects() {
        if(objectsToSpawn == null) {
            Debug.Log("Error: Assign objects to spawn");
            return;
        }

        if(!clickPosition.HasValue) {
            Debug.Log("Error: Set position to spawn objects");
            return;
        }

        for(int i = 0; i < numberOfObjects; i++) {
            float objectScale = Random.Range(minObjectScale, maxObjectScale);
            Vector2 spawnCircle = Random.insideUnitCircle * spawnRadious;
            Vector3 spawnPos = new Vector3(spawnCircle.x, 0f, spawnCircle.y) + clickPosition.Value;
            float randomYRotation = Random.Range(0f, 360f);
            Quaternion randomRotation = Quaternion.Euler(0f, randomYRotation, 0f);
            GameObject objectToSpawn = objectsToSpawn.objectsList[Random.Range(0, objectsToSpawn.objectsList.Count - 1)];
            GameObject go = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);
            go.name = objectsToSpawn.objectName;
            go.transform.localScale = Vector3.one * objectScale;
            go.transform.rotation = randomRotation;
        }
    }
}