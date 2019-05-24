using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("glitch/Analogue Glitch")]
public class AnalogueGlitch : MonoBehaviour
{
  #region Public Properties
  //[SerializeField, Range(0, 1)]
  //float _scanLineJitter;
  //public float ScanLineJitter
  //{
  //  get { return _scanLineJitter; }
  //  set { _scanLineJitter = value; }
  //}
  //[SerializeField, Range(0, 1)]
  //float _verticalJump;
  //public float VerticalJump
  //{
  //  get { return _verticalJump; }
  //  set { _verticalJump = value; }
  //}
  //[SerializeField, Range(0, 1)]
  //float _horizontalShake;
  //public float HorizontalShake
  //{
  //  get { return _horizontalShake; }
  //  set { _horizontalShake = value; }
  //}
  //[SerializeField, Range(0, 1)]
  //float _colourDrift;
  //public float ColourDrift
  //{
  //  get { return _colourDrift; }
  //  set { _colourDrift = value; }
  //}

  [SerializeField, Range(0, 1)]
  public float _ScanLineJitter;
  [SerializeField, Range(0, 1)]
  public float _VerticalJump;
  [SerializeField, Range(0, 1)]
  public float _HorizontalShake;
  [SerializeField, Range(0, 1)]
  public float _ColourDrift;
  #endregion

  #region Private Properties
  [SerializeField] Shader _shader;
  Material _material;
  float _verticalJumpTime;
  #endregion

  #region MonoBehaviour Functions
  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (_material == null)
    {
      _material = new Material(_shader);
      _material.hideFlags = HideFlags.DontSave;
    }
    //_verticalJumpTime += Time.deltaTime * _verticalJump * 11.3f;
    //var sl_thresh = Mathf.Clamp01(1.0f - _scanLineJitter * 1.2f);
    //var sl_disp = 0.002f + Mathf.Pow(_scanLineJitter, 3) * 0.05f;
    //_material.SetVector("_ScanLineJitter", new Vector2(sl_disp, sl_thresh));

    //var vj = new Vector2(_verticalJump, _verticalJumpTime);
    //_material.SetVector("_VerticalJump", vj);
    //_material.SetFloat("_HorizontalShake", _horizontalShake * 0.2f);

    //var cd = new Vector2(_colourDrift * 0.04f, Time.time * 606.11f);
    //_material.SetVector("_ColourDrift", cd);

    _verticalJumpTime += Time.deltaTime * _VerticalJump * 11.3f;
    var sl_thresh = Mathf.Clamp01(1.0f - _ScanLineJitter * 1.2f);
    var sl_disp = 0.002f + Mathf.Pow(_ScanLineJitter, 3) * 0.05f;
    _material.SetVector("_ScanLineJitter", new Vector2(sl_disp, sl_thresh));

    var vj = new Vector2(_VerticalJump, _verticalJumpTime);
    _material.SetVector("_VerticalJump", vj);
    _material.SetFloat("_HorizontalShake", _HorizontalShake * 0.2f);

    var cd = new Vector2(_ColourDrift * 0.04f, Time.time * 606.11f);
    _material.SetVector("_ColourDrift", cd);

    Graphics.Blit(source, destination, _material);
  }
  #endregion
}

//[CanEditMultipleObjects]
//[CustomEditor(typeof(AnalogueGlitch))]
//public class AnalogueGlitchEditor : Editor
//{
//  SerializedProperty _scanLinejitter;
//  SerializedProperty _verticalJump;
//  SerializedProperty _horizontalShake;
//  SerializedProperty _colourDrift;

//  private void OnEnable()
//  {
//    _scanLinejitter = serializedObject.FindProperty("_scanLineJitter");
//    _verticalJump = serializedObject.FindProperty("_verticalJump");
//    _horizontalShake = serializedObject.FindProperty("_horizontalShake");
//    _colourDrift = serializedObject.FindProperty("_colourDrift");
//  }

//  public override void OnInspectorGUI()
//  {
//    serializedObject.Update();

//    EditorGUILayout.PropertyField(_scanLinejitter);
//    EditorGUILayout.PropertyField(_verticalJump);
//    EditorGUILayout.PropertyField(_horizontalShake);
//    EditorGUILayout.PropertyField(_colourDrift);

//    serializedObject.ApplyModifiedProperties();
//  }
//}
