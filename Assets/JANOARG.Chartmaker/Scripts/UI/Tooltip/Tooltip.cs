using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace JANOARG.Chartmaker.UI.Tooltip
{
    public class Tooltip : MonoBehaviour
    {
        public static Tooltip main;

        public TMP_Text    Label;
        public CanvasGroup Group;

        public void Awake()
        {
            main = this;
        }

        public void Show(string text, RectTransform target, TooltipPositionMode positionMode)
        {
            Label.text = text;
            Group.alpha = 1;

            RectTransform rt = (RectTransform)transform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

            float scale = Behaviors.Chartmaker.Chartmaker.main.ChartmakerCanvas.scaleFactor;

            Rect selfRect = rt.rect;

            Vector3[] corners = new Vector3[4];
            target.GetWorldCorners(corners);
            Rect targetRect = new(corners[0], corners[2] - corners[0]);
            targetRect.position /= scale;
            targetRect.size /= scale;

            Vector2 screenSize = new Vector2(Screen.width, Screen.height) / scale;

            if (positionMode == TooltipPositionMode.Cursor)
            {
                rt.anchoredPosition = new (
                    Input.mousePosition.x / scale + 18,
                    Input.mousePosition.y / scale - 18 - selfRect.height
                );
            }
            if (positionMode == TooltipPositionMode.Up)
            {
                rt.anchoredPosition = new (
                    targetRect.xMin + targetRect.width / 2 - selfRect.width / 2,
                    targetRect.yMax + 2
                );
            }
            else if (positionMode == TooltipPositionMode.Down)
            {
                rt.anchoredPosition = new (
                    targetRect.xMin + targetRect.width / 2 - selfRect.width / 2,
                    targetRect.yMin - selfRect.height - 2
                );
            }
            else if (positionMode == TooltipPositionMode.Left)
            {
                rt.anchoredPosition = new (
                    targetRect.xMin - selfRect.height - 2,
                    targetRect.yMin + targetRect.height / 2 - selfRect.height / 2
                );
            }
            else if (positionMode == TooltipPositionMode.Right)
            {
                rt.anchoredPosition = new (
                    targetRect.xMax + 2,
                    targetRect.yMin + targetRect.height / 2 - selfRect.height / 2
                );
            }

            rt.anchoredPosition = new (
                Mathf.Clamp(rt.anchoredPosition.x, 5, screenSize.x - rt.sizeDelta.x - 5),  
                Mathf.Clamp(rt.anchoredPosition.y, 5, screenSize.y - rt.sizeDelta.y - 5)
            );
        }

        public void Hide()
        {
            Group.alpha = 0;
        }
    }

    public enum TooltipPositionMode
    {
        Cursor, Up, Down, Left, Right
    }
}