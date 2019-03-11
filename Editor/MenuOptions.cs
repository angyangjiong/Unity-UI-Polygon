using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI.Extensions;
namespace UnityEditor.UI.Extensions
{
    public static class MenuOptions
    {
        private readonly static Vector2 _polygonElementSize = new Vector2(100f, 100f);
        [MenuItem("GameObject/UI/Extensions/Polygon", false, 3001)]
        static public void AddPolygon(MenuCommand menuCommand)
        {
            //https://bitbucket.org/Unity-Technologies/ui/src/0651862509331da4e85f519de88c99d0529493a5/UnityEngine.UI/UI/Core/DefaultControls.cs
            Type defaultControlsType = Assembly.Load("UnityEngine.UI").GetType("UnityEngine.UI.DefaultControls");
            //GameObject CreateUIElementRoot(string name, Vector2 size)
            MethodInfo methodInfo = defaultControlsType.GetMethod("CreateUIElementRoot", BindingFlags.NonPublic | BindingFlags.Static);
            GameObject go = methodInfo.Invoke(null, new object[] { "Polygon", _polygonElementSize }) as GameObject;

            go.AddComponent<Polygon>();

            //https://bitbucket.org/Unity-Technologies/ui/src/0651862509331da4e85f519de88c99d0529493a5/UnityEditor.UI/UI/MenuOptions.cs
            Type menuOptionsType = Assembly.Load("UnityEditor.UI").GetType("UnityEditor.UI.MenuOptions");
            //void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
            methodInfo = menuOptionsType.GetMethod("PlaceUIElementRoot", BindingFlags.NonPublic | BindingFlags.Static);
            methodInfo.Invoke(null, new object[] { go, menuCommand });

            go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }
}
