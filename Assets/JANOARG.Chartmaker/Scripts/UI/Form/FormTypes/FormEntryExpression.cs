using System;
using JANOARG.Chartmaker.UI.Themeable.ThemeableTypes;
using JANOARG.Chartmaker.UI.Tooltip;
using JANOARG.Chartmaker.Utils.Math;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace JANOARG.Chartmaker.UI.Form.FormTypes
{
    public class FormEntryExpression : FormEntry<string>
    {
        public TMP_InputField Field;
        public ExpressionContext TestContext;
        public Image IndicatorIcon;
        public Sprite IndicatorIconNormal;
        public Sprite IndicatorIconError;
        public GraphicThemeable IndicatorThemeable;
        public TooltipTarget IndicatorTooltip;

        public Expression CurrentExpression { get; private set; }
        public string LastError { get; private set; }

        public new void Start() 
        {
            base.Start();
            Reset();
        }

        public void TrySetExpression(string value)
        {
            if (TestExpression(value)) SetValue(value);
            UpdateIndicator();
        }

        public bool TestExpression(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value)) throw new ExpressionException("Expression is empty");
                var tokens = ExpressionUtils.Tokenize(value);
                var expression = ExpressionUtils.ParseTokens(tokens, false, TestContext);
                expression.Evaluate(TestContext);
                CurrentExpression = expression;
                LastError = "";
                return true;
            }
            catch (ExpressionException e)
            {
                LastError = e.Message;
                return false;
            }
        }

        public void UpdateIndicator()
        {
            bool hasError = !string.IsNullOrEmpty(LastError);
            IndicatorIcon.sprite = hasError ? IndicatorIconError : IndicatorIconNormal;
            IndicatorThemeable.ID = hasError ? "DangerHighlighted" : "Content1";
            IndicatorThemeable.SetColors();

            IndicatorTooltip.Text = "Valid items:"
                + "\n   Operators: " + string.Join(' ', Operator.Operators.Keys)
                + "\n   Constants: " + string.Join(' ', Constant.Constants.Keys)
                + (TestContext != null ? "\n   Variables: " + string.Join(' ', TestContext.Variables.Keys) : "")
            ;

            if (hasError)
            {
                IndicatorTooltip.Text = "Formula contains error:"
                    + "\n   " + LastError
                    + "\n\n" + IndicatorTooltip.Text
                ;
            }
        }

        public void Reset()
        {
            Field.SetTextWithoutNotify(CurrentValue);
            TestExpression(CurrentValue);
            UpdateIndicator();
        }
    }
}
