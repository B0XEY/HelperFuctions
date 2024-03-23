using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public static class HelperFunctions {
    private const string CodeWord = "χΓΕρΣΙεζγυΞτΕΨξϑνΠψΝχΓπξΜϒΠμοΘδΧχθ";

    private static Camera _camera;
    public static Camera GetCamera {
        get {
            if (_camera == null) _camera = Camera.main; 
            return _camera;
        }
    }

    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds GetWaitTime(float time) {
        if (WaitDictionary.TryGetValue(time, out var wait)) return wait;
        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }

    private static PointerEventData _pointerEventData;
    private static List<RaycastResult> _results;
    public static bool IsOverUiElement() {
        _pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_pointerEventData, _results);
        return _results.Count > 0;
    }
    public static Vector2 GetWorldPosOfCanvasElement(RectTransform element) {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, GetCamera, out var result);
        return result;
    }
        
    //Extensions
    public static float[] GenerateCurveArray(this AnimationCurve self, int samplePoints) {
        var returnArray = new float[samplePoints];
        for (var j = 0; j < samplePoints; j++) { 
            returnArray[j] = self.Evaluate(j / (float)samplePoints);            
        }              
        return returnArray;
    }
    public static NativeArray<T> ToNativeArray<T>(this T[] input, Allocator type) where T : struct {
        var array = new NativeArray<T>(input.Length, type);
        for (var i = 0; i < input.Length; i++) { 
            array[i] = input[i];
        }
        return array;
    }
        
    public static GameObject FindChildWithTag(this GameObject t, string tag) {
        return (from child in t.GetComponentsInChildren<Transform>() where child.CompareTag(tag) select child.gameObject).FirstOrDefault();
    }
    public static void DeleteChildren(this Transform t) {
        if (Application.isPlaying) foreach (Transform child in t) Object.Destroy(child.gameObject);
        else foreach (Transform child in t) Object.DestroyImmediate(child.gameObject);
    }

    public static T Random<T>(this IList<T> list) => list[UnityEngine.Random.Range(0, list.Count)];

    public static string SplitString(this string s) => string.Join(" ", s.ToCharArray());
    public static float Clamp(this float input, float min, float max) => Mathf.Clamp(input, min, max);

    public static Vector2 ToVector2(this Vector3 input) => new Vector2(input.x, input.y);
    public static Vector2 Clamp(this Vector2 input, float min, float max) => new Vector2(Mathf.Clamp(input.y, min, max), Mathf.Clamp(input.y, min, max));
    public static float Random(this Vector2 input) => UnityEngine.Random.Range(input.x , input.y);

    public static Vector3Int ToVector3Int(this Vector3 input) => new Vector3Int((int)input.x, (int)input.y, (int)input.z);
    public static Vector3Int ToVector3Int(this int3 input) => new Vector3Int(input.x, input.y, input.z);
        
    public static Vector3 ToVector3(this float3 input) => new Vector3(input.x, input.y, input.z);
    public static Vector3 MultiplyVector3(Vector3 a, Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    public static Vector3 DivideVector3(Vector3 a, Vector3 b) => new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    public static Vector3 Clamp(this Vector3 input, float min, float max) => new Vector3(Mathf.Clamp(input.y, min, max), Mathf.Clamp(input.y, min, max), Mathf.Clamp(input.z, min, max));
        
    public static int3 RoundToInt3(this Vector3 input) => new int3(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y), Mathf.RoundToInt(input.z));
    public static float3 ToFloat3(this Vector3Int input) => new float3(input.x, input.y, input.z);
    public static int3 ToInt3(this Vector3Int input) => new int3(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y), Mathf.RoundToInt(input.z));
        
    public static string GetGuid() =>System.Guid.NewGuid().ToString();
        
    public static List<T> ToList<T>(this IEnumerable<T> array) => Enumerable.ToList(array);

    public static string EncryptDecrypt(this string s) { 
        var modifiedData = "";
        for (int i = 0; i < s.Length; i++) { 
            modifiedData += (char)(s[i] ^ CodeWord[i % CodeWord.Length]);
        }
        return modifiedData;
    }
}

public class Singleton<T> : MonoBehaviour where T : Component {
    public static T Instance { get; private set; }

    public void Awake() {
        if (Instance == null) {
            Instance = this as T;
        }else {
            Destroy(gameObject);
        }
    }
}
public class SingletonPersistent<T> : MonoBehaviour where T : Component {
    public static T Instance { get; private set; }

    public void Awake() {
        if (Instance == null) {
            Instance = this as T;
            DontDestroyOnLoad(this);
        }else {
            Destroy(gameObject);
        }
    }
}