using ADOFAI.LevelEditor.Controls;
using HarmonyLib;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

namespace FieldRandomNumberGenerator {
    [HarmonyPatch(typeof(PropertyControl_Text), "Validate")]
    public static class RandomNumberGeneratorOnPropertyControl_Text {
        public static void Postfix(ref string __result, PropertyControl_Text __instance) {
            string inputText = __instance.inputField.text;
            if (string.IsNullOrEmpty(inputText))
                return;
            string[] rangeParts = inputText.Split('~');
            if (rangeParts.Length != 2)
                return;
            if (float.TryParse(rangeParts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float minValue) &&
                float.TryParse(rangeParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float maxValue)) {
                int decimalPlaces = GetDecimalPlaces(rangeParts[0], rangeParts[1]);
                float range = maxValue - minValue;                
                System.Random random = new System.Random();
                float randomValue = (float)(random.NextDouble() * range + minValue);
                randomValue = (float)Math.Round(randomValue, decimalPlaces);
                __result = randomValue.ToString("F" + decimalPlaces, CultureInfo.InvariantCulture);
            }
        }
        private static int GetDecimalPlaces(string minValueText, string maxValueText) {
            int decimalPlaces1 = minValueText.Contains(".") ? minValueText.Split('.')[1].Length : 0;
            int decimalPlaces2 = maxValueText.Contains(".") ? maxValueText.Split('.')[1].Length : 0;
            return Math.Max(decimalPlaces1, decimalPlaces2);
        }        
    }
    [HarmonyPatch(typeof(PropertyControl_Vector2), "Validate")]
    public static class RandomNumberGeneratorOnPropertyControl_Vector2 {
        public static bool Prefix(TMP_InputField x, TMP_InputField y, ref (string, string) __result) {
            (string xInputText, string yInputText) = __result;
            if (string.IsNullOrEmpty(xInputText) || string.IsNullOrEmpty(yInputText)) {
                return true;
            }
            string pattern = @"^\d+~\d+$";
            Regex regex = new Regex(pattern);
            if (regex.IsMatch(x.text) && regex.IsMatch(y.text)) {
                return false;
            }
            string[] xRangeParts = xInputText.Split('~');
            string[] yRangeParts = yInputText.Split('~');
            if (xRangeParts.Length == 2 && yRangeParts.Length == 2 &&
                float.TryParse(xRangeParts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float xMinValue) &&
                float.TryParse(xRangeParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float xMaxValue) &&
                float.TryParse(yRangeParts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float yMinValue) &&
                float.TryParse(yRangeParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float yMaxValue)) {
                int xDecimalPlaces = GetDecimalPlaces(xRangeParts[0], xRangeParts[1]);
                int yDecimalPlaces = GetDecimalPlaces(yRangeParts[0], yRangeParts[1]);
                float xRange = xMaxValue - xMinValue;
                float yRange = yMaxValue - yMinValue;
                System.Random random = new System.Random();
                float randomX = (float)(random.NextDouble() * xRange + xMinValue);
                float randomY = (float)(random.NextDouble() * yRange + yMinValue);
                randomX = (float)Math.Round(randomX, xDecimalPlaces);
                randomY = (float)Math.Round(randomY, yDecimalPlaces);
                __result = (randomX.ToString("F" + xDecimalPlaces, CultureInfo.InvariantCulture), randomY.ToString("F" + yDecimalPlaces, CultureInfo.InvariantCulture));
            }
            return true;
        }        
        private static int GetDecimalPlaces(string minValueText, string maxValueText) {
            int decimalPlaces1 = minValueText.Contains(".") ? minValueText.Split('.')[1].Length : 0;
            int decimalPlaces2 = maxValueText.Contains(".") ? maxValueText.Split('.')[1].Length : 0;
            return Math.Max(decimalPlaces1, decimalPlaces2);
        }
    }
}
