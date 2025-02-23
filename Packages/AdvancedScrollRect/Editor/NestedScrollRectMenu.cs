using AdvancedScrollRect.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AdvancedScrollRect.Editor
{
    public static class NestedScrollRectMenu
    {
        [MenuItem("GameObject/UI/Nested Scroll Rect (Vertical + Horizontal)", false, 10)]
        private static void CreateNestedScrollRect()
        {
            // 1) Check if there is a Canvas in the scene
            var parent = Selection.activeTransform;
            if (parent == null || parent.GetComponentInParent<Canvas>() == null)
            {
                // Create a Canvas if there is no Canvas in the scene
                var canvasObj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                var canvas = canvasObj.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                // Create an EventSystem if not exists
                if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
                    new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
                parent = canvasObj.transform;
            }

            // 2) Create Vertical ScrollRect
            var verticalGo = CreateUIObject("VerticalScrollRect", parent);
            var vertScrollRect = verticalGo.AddComponent<ScrollRect>();
            var vertImage = verticalGo.AddComponent<Image>();
            vertImage.color = new Color(1, 1, 1, 0.2f); // sample color
            vertScrollRect.horizontal = false; // vertical only
            vertScrollRect.vertical = true;

            // Create Content inside the Vertical ScrollRect
            var verticalContent = CreateUIObject("Content", verticalGo.transform);
            var contentRect = verticalContent.GetComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2(0, 300); // sample size
            vertScrollRect.content = contentRect;

            // 3) Create Horizontal ScrollRect inside the Vertical ScrollRect
            var horizontalGo = CreateUIObject("HorizontalScrollRect", verticalContent.transform);
            var horizNested = horizontalGo.AddComponent<NestedScrollRect>();
            var horizImage = horizontalGo.AddComponent<Image>();
            horizImage.color = new Color(0, 1, 1, 0.2f);
            horizNested.horizontal = true; // horizontal only
            horizNested.vertical = false;

            // Set parentScrollRect
            horizNested.parentScrollRect = vertScrollRect;

            // Create Content inside the Horizontal ScrollRect
            var horizontalContent = CreateUIObject("Content", horizontalGo.transform);
            var horizContentRect = horizontalContent.GetComponent<RectTransform>();
            horizContentRect.sizeDelta = new Vector2(600, 0); // sample size
            horizNested.content = horizContentRect;

            // Select the Vertical ScrollRect
            Selection.activeGameObject = verticalGo;
        }

        private static GameObject CreateUIObject(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            return go;
        }
    }
}
