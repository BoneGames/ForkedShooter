using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
  /*
   ▄   ▄
  ▄██▄▄██▄          ╔╗ ┌─┐┌┐┌┌─┐█
  ███▀██▀██         ╠╩╗├─┤│││├┤ █
  ▀███████▀         ╚═╝┴ ┴┘└┘└─┘█
    ▀███████▄▄      ▀▀▀▀▀▀▀▀▀▀▀▀█▀
     ██████████▄
   ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀█▀ */
  public static class BaneTools
  {
    /// <summary>
    /// Formats a string with a color for use with Unity's acceptance of coloured console lines.
    /// Example usage: print(BaneTools.ColorString("MyString", "red"));
    /// </summary>
    /// <param name="_string"></param>
    /// <param name="_colour"></param>
    /// <returns></returns>
    public static string ColorString(string _string, string _colour)
    {
      return string.Format("<color={0}>{1}</color>", _colour, _string);
    }
    /// <summary>
    /// Formats a string with a color for use with Unity's acceptance of coloured console lines.
    /// Example usage: print(BaneTools.ColorString("MyString", BaneTools.Color255(255,0,0)));
    /// </summary>
    /// <param name="_string"></param>
    /// <param name="_colour"></param>
    /// <returns></returns>
    public static string ColorString(string _string, Color _colour)
    {
      Color32 _thing = _colour;
      string _hex = string.Format("#{0:X2}{1:X2}{2:X2}", _thing.r, _thing.g, _thing.b);

      return string.Format("<color={0}>{1}</color>", _hex, _string);
    }

    /// <summary>
    /// Returns a Color from given R, G and B values.
    /// </summary>
    /// <param name="_r">Red Channel 0 - 255</param>
    /// <param name="_g">Green Channel 0 - 255</param>
    /// <param name="_b">Blue Channel 0 - 255</param>
    /// <returns></returns>
    public static Color Color255(float _r, float _g, float _b)
    {
      Color color = new Color(_r / 255, _g / 255, _b / 255);
      return color;
    }
    /// <summary>
    /// Returns a Color from given R, G, B and A values.
    /// </summary>
    /// <param name="_r">Red Channel 0 - 255</param>
    /// <param name="_g">Green Channel 0 - 255</param>
    /// <param name="_b">Blue Channel 0 - 255</param>
    /// <param name="_a">Alpha Channel 0 - 1</param>
    /// <returns></returns>
    public static Color Color255(float _r, float _g, float _b, float _a)
    {
      Color color = new Color(_r / 255, _g / 255, _b / 255, _a);
      return color;
    }

    public static void SetWorldScale(Transform _object, Transform _parent, float _scale)
    {
      _object.SetParent(null);
      _object.localScale = BaneMath.MultipliedVector3(_object.localScale, _scale);
      _object.SetParent(_parent);
    }


  }

  public static class BaneRays
  {
    public static RaycastHit2D GetFirstHit(Vector2 _start, Vector2 _dir, float _dist, Color _color)
    {
      RaycastHit2D[] hits = new RaycastHit2D[2];
      Debug.DrawRay(_start, _dir, _color, _dist);
      Physics2D.RaycastNonAlloc(_start, _dir, hits, _dist);

      return hits[1];
    }

    /// <summary>
    /// Returns True if the whole object is visible, false if any of it is obscured (as defined by the offsets)
    /// </summary>
    /// <param name="objectBounds"></param>
    /// <param name="_start"></param>
    /// <param name="_target"></param>
    /// <returns></returns>
    public static bool ViewNotObstructed(Vector4 objectBounds, Transform _start, Transform _target)
    {
      //Stores the visibility checks
      Vector4 clearFlags;

      //Adds the offsets provided in the Inspector to reach each side.
      Vector3 leftSide = BaneMath.SplitAddedVector3(_start.position, +objectBounds.x, 0, 0);
      Vector3 rightSide = BaneMath.SplitAddedVector3(_start.position, +objectBounds.y, 0, 0);
      Vector3 topSide = BaneMath.SplitAddedVector3(_start.position, 0, +objectBounds.z, 0);
      Vector3 bottomSide = BaneMath.SplitAddedVector3(_start.position, 0, +objectBounds.w, 0);

      //Getting the direction from the correct side to the target
      Vector3 leftDir = _target.position - leftSide;
      Vector3 rightDir = _target.position - rightSide;
      Vector3 topDir = _target.position - topSide;
      Vector3 bottomDir = _target.position - bottomSide;

      //Defines the raycast hit variables
      RaycastHit leftHit;
      RaycastHit rightHit;
      RaycastHit topHit;
      RaycastHit botHit;

      //Set flag to false if the raycast hits something, else set it to true
      clearFlags.x = Physics.Raycast(leftSide, leftDir.normalized, out leftHit, leftDir.magnitude) ? 0 : 1;
      clearFlags.y = Physics.Raycast(rightSide, rightDir.normalized, out rightHit, rightDir.magnitude) ? 0 : 1;
      clearFlags.z = Physics.Raycast(topSide, topDir.normalized, out topHit, topDir.magnitude) ? 0 : 1;
      clearFlags.w = Physics.Raycast(bottomSide, bottomDir.normalized, out botHit, bottomDir.magnitude) ? 0 : 1;

      //If all four flags are true, return true
      return clearFlags == new Vector4(1, 1, 1, 1) ? true : false;
    }
    public static bool VewNotObstructed(Vector4 objectBounds, Transform _start, Transform _target, bool _debug)
    {
      //Stores the visibility checks
      Vector4 clearFlags = new Vector4(0,0,0,0);

      //Adds the offsets provided in the Inspector to reach each side.
      Vector3 leftSide = BaneMath.SplitAddedVector3(_start.position, +objectBounds.x, 0, 0);
      Vector3 rightSide = BaneMath.SplitAddedVector3(_start.position, +objectBounds.y, 0, 0);
      Vector3 topSide = BaneMath.SplitAddedVector3(_start.position, 0, +objectBounds.z, 0);
      Vector3 bottomSide = BaneMath.SplitAddedVector3(_start.position, 0, +objectBounds.w, 0);

      //Getting the direction from the correct side to the target
      Vector3 leftDir = _target.position - leftSide;
      Vector3 rightDir = _target.position - rightSide;
      Vector3 topDir = _target.position - topSide;
      Vector3 bottomDir = _target.position - bottomSide;

      //Defines the raycast hit variables
      RaycastHit leftHit;
      RaycastHit rightHit;
      RaycastHit topHit;
      RaycastHit botHit;

      if (_debug)
      {
        //Draw Rays so they can be seen in Scene View
        Debug.DrawRay(leftSide, leftDir, BaneTools.Color255(255, 255, 0), 1, false);
        Debug.DrawRay(rightSide, rightDir, BaneTools.Color255(0, 255, 255), 1, false);
        Debug.DrawRay(topSide, topDir, BaneTools.Color255(255, 0, 255), 1, false);
        Debug.DrawRay(bottomSide, bottomDir, BaneTools.Color255(0, 0, 255), 1, false);

        //if (Physics.Raycast(leftSide, leftDir.normalized, out leftHit, leftDir.magnitude))
        //{
        //  Debug.Log("Left hit " + leftHit.collider.name);
        //}
      }

      //Set flag to false if the raycast hits something, else set it to true
      clearFlags.x = Physics.Raycast(leftSide, leftDir.normalized, out leftHit, leftDir.magnitude) ? 0 : 1;
      clearFlags.y = Physics.Raycast(rightSide, rightDir.normalized, out rightHit, rightDir.magnitude) ? 0 : 1;
      clearFlags.z = Physics.Raycast(topSide, topDir.normalized, out topHit, topDir.magnitude) ? 0 : 1;
      clearFlags.w = Physics.Raycast(bottomSide, bottomDir.normalized, out botHit, bottomDir.magnitude) ? 0 : 1;

      Debug.Log(clearFlags);

      //If all four flags are true, return true
      return clearFlags == new Vector4(1, 1, 1, 1) ? true : false;
    }
    public static bool ViewNotObstructed(Transform _start, Transform _target, bool _debug)
    {
      //Stores the visibility checks
      bool clearFlag = false;

      //Adds the offsets provided in the Inspector to reach each side.
      Vector3 leftSide = _start.position;

      //Getting the direction from the correct side to the target
      Vector3 leftDir = _target.position - leftSide;

      //Defines the raycast hit variables
      RaycastHit leftHit;

      if (_debug)
      {
        //Draw Rays so they can be seen in Scene View
        Debug.DrawRay(leftSide, leftDir, BaneTools.Color255(255, 255, 0), 1, false);

        if (Physics.Raycast(leftSide, leftDir.normalized, out leftHit, leftDir.magnitude))
        {
          Debug.Log("Ray hit " + leftHit.collider.name);
        }
      }

      //Set flag to false if the raycast hits something, else set it to true
      if (Physics.Raycast(leftSide, leftDir.normalized, out leftHit, leftDir.magnitude))
      {
        if (leftHit.collider.transform == _target)
        {
          if (_debug)
          {
            Debug.Log("The view is not obstructed!");
          }
          clearFlag = true;
        }
      }
      else
      {
        clearFlag = false;
      }
      //clearFlag = Physics.Raycast(leftSide, leftDir.normalized, out leftHit, leftDir.magnitude, ) ? false : true;

      //Debug.Log(clearFlag);

      //If all four flags are true, return true
      return clearFlag;
    }
  }

  public static class BaneMath
  {
    public static Vector2 centre = new Vector2(.5f, .5f);

    public static Vector2 AddedVector2(Vector2 _vector2, float _value)
    {
      Vector2 _addedVector2 = new Vector2(_vector2.x + _value, _vector2.y + _value);
      return _addedVector2;
    }
    public static Vector2 SplitAddedVector2(Vector2 _vector2, float _valueX, float _valueY)
    {
      Vector2 _splitAddedVector2 = new Vector2(_vector2.x + _valueX, _vector2.y + _valueY);
      return _splitAddedVector2;
    }
    public static Vector2 MultipliedVector2(Vector2 _vector2, float _value)
    {
      Vector2 _addedVector2 = new Vector2(_vector2.x * _value, _vector2.y * _value);
      return _addedVector2;
    }
    public static Vector2 SplitMultipliedVector2(Vector2 _vector2, float _valueX, float _valueY)
    {
      Vector2 _splitAddedVector2 = new Vector2(_vector2.x * _valueX, _vector2.y * _valueY);
      return _splitAddedVector2;
    }

    public static Vector3 AddedVector3(Vector3 _vector3, float _value)
    {
      Vector3 _addedVector3 = new Vector3(_vector3.x + _value, _vector3.y + _value, _vector3.z + _value);
      return _addedVector3;
    }
    public static Vector3 SplitAddedVector3(Vector3 _vector3, float _valueX, float _valueY, float _valueZ)
    {
      Vector3 _splitAddedVector3 = new Vector3(_vector3.x + _valueX, _vector3.y + _valueY, _vector3.z + _valueZ);
      return _splitAddedVector3;
    }
    public static Vector3 MultipliedVector3(Vector3 _vector3, float _value)
    {
      Vector3 _addedVector3 = new Vector3(_vector3.x * _value, _vector3.y * _value, _vector3.z * _value);
      return _addedVector3;
    }
    public static Vector3 SplitMultipliedVector3(Vector3 _vector3, float _valueX, float _valueY, float _valueZ)
    {
      Vector3 _splitAddedVector3 = new Vector3(_vector3.x * _valueX, _vector3.y * _valueY, _vector3.z * _valueZ);
      return _splitAddedVector3;
    }
    public static Vector3 CombinedVector3(Vector3 _v1, Vector3 _v2)
    {
      Vector3 _addedVector3 = new Vector3(_v1.x + _v2.x, _v1.y + _v2.y, _v1.z + _v2.z);
      return _addedVector3;
    }
    public static Vector3 MultiplyCombinedVector3(Vector3 _v1, Vector3 _v2)
    {
      Vector3 _addedVector3 = new Vector3(_v1.x * _v2.x, _v1.y * _v2.y, _v1.z * _v2.z);
      return _addedVector3;
    }

    public static float DivideInts(int _a, int _b)
    {
      return ((float)_a / (float)_b);
    }

    /// <summary>
    /// Returns true if given value is within the provided start and end values.
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="_start"></param>
    /// <param name="_end"></param>
    /// <returns></returns>
    public static bool NumberWithinRange(float _value, float _start, float _end)
    {
      if (_value >= _start && _value <= _end)
      {
        return true;
      }
      return false;
    }
  }
}
