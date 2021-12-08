using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine;
using System.IO;
using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Android;

namespace SteveRogers
{
    public enum ImageFilterMode : int
    {
        /// <summary>
        /// bad quality
        /// </summary>
        Nearest = 0,

        /// <summary>
        /// best quality
        /// </summary>
        Biliner = 1,

        /// <summary>
        /// avg quality
        /// </summary>
        Average = 2
    }

    public enum BOOL_EXTRA
    {
        USELESS, TRUE, FALSE
    } // DO NOT change the order of these items

    public enum Direction
    {
        None, Right, Left, Up, Down
    } // DO NOT change the order of these items

    #region ColorText

    public struct ColorText
    {
        private string text;
        public string _Text { get { return text; } }

        private Color color;
        public Color _Color { get { return color; } }

        public ColorText(string text, Color color)
        {
            this.text = text;
            this.color = color;
        }

        public ColorText(string text)
        {
            this.text = text;
            color = Color.black;
        }
    }

    #endregion

    #region Value Classes

    [Serializable]
    public class ColorAsString
    {
        public string code = null;

        private bool isDecoded = false;
        private Color value = default;

        public Color Value()
        {
            if (isDecoded)
            {
                return value;
            }
            else
            {
                isDecoded = true;
                value = Utilities.HexToColor(code);
                return Value();
            }
        }
    }

    [Serializable]
    public class IntInt
    {
        public int value_1;
        public int value_2;
    }

    [Serializable]
    public class IntFloat
    {
        public int value_1;
        public float value_2;
    }

    [Serializable]
    public class FloatColor32AsString
    {
        public float value_1;
        public ColorAsString value_2;
    }

    [Serializable]
    public class IntColor32
    {
        public int value_1;
        public Color32 value_2;
    }

    [Serializable]
    public class StringArray
    {
        public string[] items;
    }

    [Serializable]
    public class StringFloat
    {
        public string s;
        public float f;
    }

    [Serializable]
    public class IntArray
    {
        public int[] items;
    }

    #endregion


    [Serializable]
    public class RectTransformData
    {
        public Vector3 localPosition;
        public Vector2 size;
        public Vector3 scale;
        public Vector3 rotation;
        public Vector2 anchorMax;
        public Vector2 anchorMin;
        public Vector2 pivot;

        public void ApplyToGo(GameObject go) // Sub
        {
            if (go == null)
            {
                Debug.LogError("GO is null");
                return;
            }

            RectTransform r = go.GetComponent<RectTransform>();
            ApplyToGo(r);
        }

        public void ApplyToGo(RectTransform r) // Main 
        {
            if (r)
            {
                r.anchorMax = anchorMax;
                r.anchorMin = anchorMin;
                r.pivot = pivot;
                r.localPosition = localPosition;
                r.localScale = scale;
                r.rotation = Quaternion.Euler(rotation);
                r.sizeDelta = size;
            }
            else
            {
                Debug.LogError("Rect is null");
            }
        }

        public void ApplyToGoIfHasValue(GameObject go)
        {
            if (go == null)
            {
                Debug.LogError("GO is null");
                return;
            }

            RectTransform r = go.GetComponent<RectTransform>();

            if (r)
            {
                r.anchorMax = Utilities.MergeIfHasValue(r.anchorMax, anchorMax);
                r.anchorMin = Utilities.MergeIfHasValue(r.anchorMin, anchorMin);
                r.pivot = Utilities.MergeIfHasValue(r.pivot, pivot);
                r.localPosition = Utilities.MergeIfHasValue(r.localPosition, localPosition);
                r.localScale = Utilities.MergeIfHasValue(r.localScale, scale);
                r.rotation = Quaternion.Euler(Utilities.MergeIfHasValue(r.rotation.eulerAngles, rotation));
                r.sizeDelta = Utilities.MergeIfHasValue(r.sizeDelta, size);
            }
            else
            {
                Debug.LogError("r is null");
            }
        }

        public override string ToString()
        {
            return Utilities.CreateLogContent(
                localPosition,
                size,
                scale,
                rotation,
                anchorMax,
                anchorMin,
                pivot);
        }
    }

    public static partial class Utilities
    {
        #region Constants

        // COLOR 

        public static readonly Color Orange = new Color(1, 146 / 255f, 0, 1);

        // VECTOR

        public static readonly Vector3 VECTOR_1 = new Vector3(1, 1, 1);
        public static readonly Vector3 VECTOR_0 = new Vector3(0, 0, 0);

        // FILE NAMES

        public const string DEV_FILE_NAME__DEBUG = "DEBUG";
        public const string DEV_FILE_NAME__NO_ADS = "NO_ADS";
        public const string DEV_FILE_NAME__NO_ANALYTICS = "NO_ANALYTICS";

        // REGEX

        public const string EMAIL_REGEX = @"^([a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6})*$";
        public const string URL_REGEX = @"(https?:\/\/)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)";

        // FOLDER NAMES

        public readonly static string ASSET_BUNDLE_FOLDER_NAME = "AssetBundleBuilds";
        public readonly static string CHEAT_FOLDER_NAME = "Cheat";
        private readonly static string RUNTIME_DATA_FOLDER_NAME = "RuntimeData";
        private readonly static string DOWNLOADED_FOLDER_NAME = "Downloaded";
        private readonly static string RUNTIME_TMP_FOLDER_NAME = "Temp";

        // DEBUG DEVICES

        public readonly static string[] DEBUG_DEVICES = new string[] {
            "d751b148d4948bce99e43d4204c51b5859047a8b", // iMobility PC
            "02afa69bc72f77c65ca48c1a7596f8081f939fe5", // Me Lap
            "3893931d2188003e3ed78581865c3147", // OPPO
            "6fb998932921ec99982b855fb00e7393", // Mi
            "430cc64350edfdcb5ed70c62eb55c3ees", // Me
        };

        // MISC.

        public readonly static char[] SPLIT_CHAR_TAB = new char[] { '\t' };
        public readonly static char[] SPLIT_CHAR_SPACE = new char[] { ' ' };
        public readonly static char[] SPLIT_CHAR_COMMA = new char[] { ',' };
        public readonly static char[] SPLIT_CHAR_NEW_LINE = new char[] { '\n' };

        public static readonly WaitForSeconds WAIT_FOR_ONE_SECOND = new WaitForSeconds(1);

        #endregion

        #region Variables

        private static float _TimeTick = 0;
        private static EventSystem _LastEventSystem = null;

        #endregion

        #region Texture / PNG / Sprite / ScreenShot

        public static void SetSizeImageKeepRatio_Width(GameObject go, float width) // Sub 
        {
            SetSizeImageKeepRatio_Width(go.GetComponent<Image>(), width);
        }

        public static void SetSizeImageKeepRatio_Width(Image image, float width) // Main 
        {
            var rect = image.GetComponent<RectTransform>();
            var h = width * image.sprite.texture.height / image.sprite.texture.width;
            rect.sizeDelta = new Vector2(width, h);
        }

        public static void SetSizeImageKeepRatio_Height(GameObject go, float height) // Sub 
        {
            SetSizeImageKeepRatio_Height(go.GetComponent<Image>(), height);
        }

        public static void SetSizeImageKeepRatio_Height(Image image, float height) // Main 
        {
            var rect = image.GetComponent<RectTransform>();
            var w = height * image.sprite.texture.width / image.sprite.texture.height;
            rect.sizeDelta = new Vector2(w, height);
        }

        public static Texture2D ResizeTexture(Texture2D pSource, ImageFilterMode pFilterMode, float pScale)
        {

            //*** Variables
            int i;

            //*** Get All the source pixels
            Color[] aSourceColor = pSource.GetPixels(0);
            Vector2 vSourceSize = new Vector2(pSource.width, pSource.height);

            //*** Calculate New Size
            float xWidth = Mathf.RoundToInt((float)pSource.width * pScale);
            float xHeight = Mathf.RoundToInt((float)pSource.height * pScale);

            //*** Make New
            Texture2D oNewTex = new Texture2D((int)xWidth, (int)xHeight, TextureFormat.RGBA32, false);

            //*** Make destination array
            int xLength = (int)xWidth * (int)xHeight;
            Color[] aColor = new Color[xLength];

            Vector2 vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);

            //*** Loop through destination pixels and process
            Vector2 vCenter = new Vector2();
            for (i = 0; i < xLength; i++)
            {

                //*** Figure out x&y
                float xX = (float)i % xWidth;
                float xY = Mathf.Floor((float)i / xWidth);

                //*** Calculate Center
                vCenter.x = (xX / xWidth) * vSourceSize.x;
                vCenter.y = (xY / xHeight) * vSourceSize.y;

                //*** Do Based on mode
                //*** Nearest neighbour (testing)
                if (pFilterMode == ImageFilterMode.Nearest)
                {

                    //*** Nearest neighbour (testing)
                    vCenter.x = Mathf.Round(vCenter.x);
                    vCenter.y = Mathf.Round(vCenter.y);

                    //*** Calculate source index
                    int xSourceIndex = (int)((vCenter.y * vSourceSize.x) + vCenter.x);

                    //*** Copy Pixel
                    aColor[i] = aSourceColor[xSourceIndex];
                }

                //*** Bilinear
                else if (pFilterMode == ImageFilterMode.Biliner)
                {

                    //*** Get Ratios
                    float xRatioX = vCenter.x - Mathf.Floor(vCenter.x);
                    float xRatioY = vCenter.y - Mathf.Floor(vCenter.y);

                    //*** Get Pixel index's
                    int xIndexTL = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
                    int xIndexTR = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));
                    int xIndexBL = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
                    int xIndexBR = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));

                    //*** Calculate Color
                    aColor[i] = Color.Lerp(
                        Color.Lerp(aSourceColor[xIndexTL], aSourceColor[xIndexTR], xRatioX),
                        Color.Lerp(aSourceColor[xIndexBL], aSourceColor[xIndexBR], xRatioX),
                        xRatioY
                    );
                }

                //*** Average
                else if (pFilterMode == ImageFilterMode.Average)
                {

                    //*** Calculate grid around point
                    int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
                    int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
                    int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
                    int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);

                    //*** Loop and accumulate                    
                    Color oColorTemp = new Color();
                    float xGridCount = 0;
                    for (int iy = xYFrom; iy < xYTo; iy++)
                    {
                        for (int ix = xXFrom; ix < xXTo; ix++)
                        {

                            //*** Get Color
                            oColorTemp += aSourceColor[(int)(((float)iy * vSourceSize.x) + ix)];

                            //*** Sum
                            xGridCount++;
                        }
                    }

                    //*** Average Color
                    aColor[i] = oColorTemp / (float)xGridCount;
                }
            }

            //*** Set Pixels
            oNewTex.SetPixels(aColor);
            oNewTex.Apply();

            //*** Return
            return oNewTex;
        }

        public static Texture2D BytesToTexture2D(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;

            Texture2D tex = null;
            tex = new Texture2D(2, 2);
            tex.LoadImage(data); //..this will auto-resize the texture dimensions.
            return tex;
        }

        public static Texture2D FileToTexture2D(string filePath)
        {
            if (File.Exists(filePath))
            {
                return BytesToTexture2D(File.ReadAllBytes(filePath));
            }
            else
            {
                Debug.LogError("(FileToTexture2D) File not exist! " + filePath);
                return null;
            }
        }

        public static Sprite Texture2DToSprite(this Texture2D tex)
        {
            if (tex == null)
            {
                Debug.LogError("(Texture2DToSprite) Tex is null!");
                return null;
            }
            else
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        }

        public static Texture2D CropTexture2D(this Texture2D tex, int x, int y, int w, int h)
        {
            if (tex == null || x < 0 || y < 0 || w < 1 || h < 1 || x + w > tex.width || y + h > tex.height)
            {
                Debug.LogError("(CropTexture2D) Params are not valid!");
                return null;
            }

            var data = tex.GetPixels(x, y, w, h);

            if (data == null || data.Length == 0)
                return null;

            Texture2D texture2D = new Texture2D(w, h);
            texture2D.SetPixels(data);
            texture2D.Apply();

            return texture2D;
        }

        public static Texture2D CropTexture2DCenter(this Texture2D tex, int w, int h, bool getMinSizeIfWidthHeightOver)
        {
            if (tex == null)
                return null;

            if (w > tex.width || h > tex.height)
            {
                if (getMinSizeIfWidthHeightOver)
                {
                    if (w > tex.width)
                        w = tex.width;

                    if (h > tex.height)
                        h = tex.height;
                }
                else
                {
                    Debug.LogError("(CropTexture2DCenter) Params are not valid!");
                    return null;
                }
            }

            int x = Mathf.FloorToInt(tex.width / 2.0f) - Mathf.FloorToInt(w / 2.0f);
            int y = Mathf.FloorToInt(tex.height / 2.0f) - Mathf.FloorToInt(h / 2.0f);

            return CropTexture2D(tex, x, y, w, h);
        }

        public static Texture2D CropCenterSquare(this Texture2D tex, int size, bool getMinSizeIfWidthHeightOver)
        {
            if (tex == null)
                return null;

            if (size > tex.width || size > tex.height)
            {
                if (getMinSizeIfWidthHeightOver)
                {
                    size = Math.Min(tex.width, tex.height);
                }
                else
                {
                    Debug.LogError("(CropCenterSquare) Params are not valid!");
                    return null;
                }
            }

            int x = Mathf.FloorToInt(tex.width / 2.0f) - Mathf.FloorToInt(size / 2.0f);
            int y = Mathf.FloorToInt(tex.height / 2.0f) - Mathf.FloorToInt(size / 2.0f);

            return CropTexture2D(tex, x, y, size, size);
        }

        public static Sprite BytesToSprite(byte[] data)
        {
            var tex = BytesToTexture2D(data);
            return tex?.Texture2DToSprite();
        }

        public static void ToTexture2DFile(this Texture2D data, string filepath, bool isJPG)
        {
            File.WriteAllBytes(filepath, isJPG ? data.EncodeToJPG() : data.EncodeToPNG());
        }

        public static Texture2D CreateTexture2DSolidColor(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; ++i)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        #endregion

        #region DateTime

        public static string TodayShortDateStringWithFormatDayMonthClearly => GetShortDateStringWithFormatDayMonthClearly(DateTime.Now);

        public static string GetShortDateStringWithFormatDayMonthClearly(DateTime dateTime)
        {
            return dateTime.ToString("d/MMM/yyyy");
        }

        public static IEnumerator GetInternetTime_CRT(Action<DateTime> onFinish, DateTime defaultValue)
        {
            UnityWebRequest myHttpWebRequest = UnityWebRequest.Get("http://www.microsoft.com");
            yield return myHttpWebRequest.SendWebRequest();
            var dt = myHttpWebRequest.GetResponseHeader("date").Parse(defaultValue);
            onFinish(dt);
        }

        public static bool IsYesterDay(DateTime oldDateTime, DateTime newDateTime)
        {
            return (oldDateTime.Date.AddDays(1) == newDateTime.Date);
        }

        public static int GetWeekOfYear(DateTime time) // This presumes that weeks start with Monday 
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static DateTime FirstDateOfWeek() // First date (monday) of current time 
        {
            //var firstDayOfWeek = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek; 

            var diff = DateTime.Today.DayOfWeek - DayOfWeek.Monday;

            if (diff < 0)
                diff += 7;

            return DateTime.Today.AddDays(-diff).Date;
        }

        public static string GetTimeSpanString(TimeSpan timeSpan, bool showDays = false, bool showHours = false, bool showMinutes = false, string join = null, string numberFormat = "") // Main 
        {
            string time = "";
            bool hasChar = join.IsValid();

            if (timeSpan.Days > 0 || showDays)
                time += string.Format("{0}{1}", timeSpan.Days.ToString(numberFormat), hasChar ? join : "d ");

            if (timeSpan.Hours > 0 || showDays || showHours)
                time += string.Format("{0}{1}", timeSpan.Hours.ToString(numberFormat), hasChar ? join : "h ");

            if (timeSpan.Minutes > 0 || showDays || showHours || showMinutes)
                time += string.Format("{0}{1}", timeSpan.Minutes.ToString(numberFormat), hasChar ? join : "m ");

            if (timeSpan.Days == 0 && timeSpan.Seconds >= 0)
                time += string.Format("{0}{1}", timeSpan.Seconds.ToString(numberFormat), hasChar ? "" : "s ");

            if (string.IsNullOrEmpty(time.Trim()))
                return "0s";
            else
                return time.Trim();
        }

        public static string GetTimeSpanString(int seconds, bool showDays = false, bool showHours = false, bool showMinutes = false) // Sub  
        {
            return GetTimeSpanString(new TimeSpan(0, 0, seconds), showDays, showHours, showMinutes);
        }

        public static float TimeTick(bool isStart)
        {
            if (isStart)
            {
                _TimeTick = Time.realtimeSinceStartup;
                return 0;
            }
            else
            {
                return Time.realtimeSinceStartup - _TimeTick;
            }
        }

        public static string GetMonthName(int month, int year = -1, string format = "MMMM")
        {
            if (month < 1 || month > 12)
            {
                Debug.LogError("(GetMonthName) error!");
                return null;
            }
            else
                return (new DateTime(year > 0 ? year : DateTime.Today.Year, month, 1)).ToString(format);
        }

        #endregion

        #region Colors

        public static string StyleText_Bold(string text) // Main
        {
            return string.Format(@"<b>{0}</b>", text);
        }

        public static string StyleText_Size(string text, int size) // Main
        {
            return string.Format(@"<size={1}>{0}</size>", text, size);
        }

        public static string StyleText(string text, Color color, bool bold) // Sub
        {
            if (bold)
                return StyleText_Bold(StyleText(text, color));
            else
                return StyleText(text, color);
        }

        public static string StyleText(string text, string hexColor) // Sub
        {
            return StyleText(text, HexToColor(hexColor));
        }

        public static string StyleText(string text, Color color) // Main
        {
            return string.Format(@"<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color), text);
        }

        public static bool HexToColor(string hex, out Color color)
        {
            try
            {
                hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
                hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF

                byte a = 255;//assume fully visible unless specified in hex
                byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

                if (hex.Length == 8)
                {
                    a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                }

                color = new Color32(r, g, b, a);
                return true;
            }
            catch
            {
                color = Color.black;
                return false;
            }
        }

        public static Color HexToColor(string hex, Color defaultColor = new Color())
        {
            try
            {
                hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
                hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF

                byte a = 255;//assume fully visible unless specified in hex
                byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

                if (hex.Length == 8)
                {
                    a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                }

                return new Color32(r, g, b, a);
            }
            catch
            {
                return defaultColor;
            }
        }

        public static Color SetAlpha(Color color, float value)
        {
            return new Color(color.r, color.g, color.b, Mathf.Clamp01(value));
        }

        public static void SetAlpha(this SpriteRenderer item, float value)
        {
            item.color = SetAlpha(item.color, value);
        }

        public static void SetAlpha(this Graphic item, float value)
        {
            item.color = SetAlpha(item.color, value);
        }

        public static void SetAlpha(this Transform trans, float value) // Sub
        {
            trans.gameObject.SetAlpha(value);
        }

        public static void SetAlpha(this GameObject go, float value) // Main 
        {
            var image = go.GetComponent<Image>();

            if (image)
            {
                image.SetAlpha(value);
                return;
            }

            var renderer = go.GetComponent<SpriteRenderer>();

            if (renderer)
            {
                renderer.SetAlpha(value);
                return;
            }
        }

        public static void SetColor(this Transform trans, Color color) // Sub 
        {
            trans.gameObject.SetColor(color);
        }

        public static void SetColor(this GameObject go, Color color) // Main 
        {
            var image = go.GetComponent<Image>();

            if (image)
            {
                image.color = color;
                return;
            }

            var renderer = go.GetComponent<SpriteRenderer>();

            if (renderer)
            {
                renderer.color = color;
                return;
            }
        }

        #endregion

        #region GUI

        public static GUIStyle CreateSimpleBackgroundBox(Color color)
        {
            var currentStyle = new GUIStyle(GUI.skin.box);
            currentStyle.normal.background = CreateTexture2DSolidColor(2, 2, color);
            currentStyle.alignment = TextAnchor.MiddleCenter;
            return currentStyle;
        }

        #endregion

        #region File / Folder

        public static void OpenFileWithDefaultApp(string path)
        {
            System.Diagnostics.Process.Start(path);
        }

        public static List<List<string>> ReadExcelFormat_FromResources(string filepath) // sub
        {
            var textFile = Resources.Load<TextAsset>(filepath);

            if (textFile)
            {
                var t = textFile.text;
                return ReadExcelFormat(ref t);
            }
            else
                throw new FileNotFoundException("filepath: " + filepath);
        }

        public static List<List<string>> ReadExcelFormat_FromFile(string filepath) // sub
        {
            if (File.Exists(filepath))
            {
                var t = File.ReadAllText(filepath);
                return ReadExcelFormat(ref t);
            }
            else
                throw new FileNotFoundException("filepath: " + filepath);
        }

        public static List<List<string>> ReadExcelFormat(ref string text) // main
        {
            if (text.IsNullOrEmpty())
                return null;

            var lines = text.Split(SPLIT_CHAR_NEW_LINE);

            if (lines.IsNullOrEmpty())
                return null;

            List<List<string>> resu = new List<List<string>>();

            foreach (var line in lines)
            {
                var cells = line.Split(SPLIT_CHAR_TAB, StringSplitOptions.RemoveEmptyEntries);

                if (cells.IsNullOrEmpty())
                    continue;
                else
                    resu.Add(cells.ToList());
            }

            return resu;
        }

        public static List<List<string>> ReadCSV_FromResources_WithSplit(string filepath, bool isComma, bool removeFirstLine)
        {
            var text = ReadAllText_Resources(filepath);
            return ReadCSV_FromText_WithSplit(ref text, isComma, removeFirstLine);
        }

        public static List<List<string>> ReadCSV_FromText_WithSplit(ref string text, bool isComma, bool removeFirstLine)
        {
            if (text.IsNullOrEmpty())
                return null;

            var lines = text.Split(SPLIT_CHAR_NEW_LINE);

            if (lines.IsNullOrEmpty())
                return null;

            List<List<string>> resu = new List<List<string>>();
            bool passedFirstLine = false;

            foreach (var line in lines)
            {
                if (!passedFirstLine && removeFirstLine)
                {
                    passedFirstLine = true;
                    continue;
                }

                var cells = line.Split(new char[] { (isComma ? ',' : '\t') }, StringSplitOptions.RemoveEmptyEntries);

                if (cells.IsNullOrEmpty())
                    continue;
                else
                    resu.Add(cells.ToList());
            }

            return resu;
        }

        public static List<string> ReadCSV(string filepath, bool removeFirstLine = true)
        {
            if (File.Exists(filepath))
            {
                using (var reader = new StreamReader(filepath))
                {
                    List<string> lines = new List<string>();

                    if (removeFirstLine)
                        reader.ReadLine();

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        if (!string.IsNullOrEmpty(line))
                            lines.Add(line);
                    }

                    return lines;
                }
            }
            else
            {
                Debug.LogError("(ReadCSV) Not found find at: " + filepath);
                return null;
            }
        }

        public static List<string> ReadCSV_FromResources(string filepath, bool removeFirstLine = true)
        {
            var textFile = Resources.Load<TextAsset>(filepath);
            return ReadCSV_FromText(textFile.text);
        }

        public static List<string> ReadCSV_FromText(string text, bool removeFirstLine = true)
        {
            if (!string.IsNullOrEmpty(text))
            {
                string[] lines_raw = text.Split('\n');
                List<string> lines = new List<string>();

                for (int i = removeFirstLine ? 1 : 0; i < lines_raw.Length; i++)
                {
                    if (!string.IsNullOrEmpty(lines_raw[i]))
                        lines.Add(lines_raw[i]);
                }

                return lines;
            }
            else
            {
                Debug.LogError("(ReadCSVFromText) Text is null!");
                return null;
            }
        }

        public static bool DevFileExist_Debug // sub 
        {
            get { return DevFileExist(DEV_FILE_NAME__DEBUG); }
        }

        public static bool DevFileExist_No_Ads // sub 
        {
            get { return DevFileExist(DEV_FILE_NAME__NO_ADS); }
        }

        public static bool DevFileExist_No_Analytics // sub 
        {
            get { return DevFileExist(DEV_FILE_NAME__NO_ANALYTICS); }
        }

        public static bool DevFileExist(string name) // main 
        {
            var ex = GetDevFilePath(name);

            if (ex == null)
                return false;

            return File.Exists(ex);
        }

        public static string GetDevFilePath(string name)
        {
            var ex = GetExternalFolderPath(false, "Dev Files");

            if (ex == null)
                return null;

            return Path.Combine(ex, name);
        }

        public static bool FileExist(string filepath)
        {
            return File.Exists(filepath);
        }

        public static byte[] ReadAllBytes(string filepath)
        {
            return File.ReadAllBytes(filepath);
        }

        public static string[] ReadAllLinesSafe(string filepath)
        {
            if (File.Exists(filepath))
                return File.ReadAllLines(filepath);
            else
                return null;
        }

        public static byte[] ReadAllBytesSafe(string filepath)
        {
            if (File.Exists(filepath))
                return File.ReadAllBytes(filepath);
            else
                return null;
        }

        public static string ReadAllText_Resources(string path)
        {
            var t = Resources.Load<TextAsset>(path);
            return t?.text;
        }

        public static string ReadAllTextSafe(string filepath)
        {
            if (File.Exists(filepath))
                return ReadAllText(filepath);
            else
                return null;
        }

        public static string ReadAllText(string filepath)
        {
            return File.ReadAllText(filepath);
        }

        public static void WriteAllText(string filepath, string text, bool createFolder = false)
        {
            if (createFolder)
                CheckAndCreateDirectory(Path.GetDirectoryName(filepath));

            File.WriteAllText(filepath, text);
        }

        public static void WriteAllBytes(string filepath, byte[] data, bool createFolder = false)
        {
            if (createFolder)
                CheckAndCreateDirectory(Path.GetDirectoryName(filepath));

            File.WriteAllBytes(filepath, data);
        }


        public static string GetRuntimeDataFolderPath_Resources(bool withSeparator)
        {
            return withSeparator ? (RUNTIME_DATA_FOLDER_NAME + Path.DirectorySeparatorChar) : RUNTIME_DATA_FOLDER_NAME;
        }

        public static string GetExternalFolderPath(bool withSeparator, string folderName)
        {
            string GetPath(string _folder)
            {
                PlayerPrefs.SetString("external_folder_path", _folder);
                return _folder + (withSeparator ? Path.DirectorySeparatorChar.ToString() : string.Empty);
            }

#if UNITY_EDITOR            
            return GetPath(Path.Combine(Directory.GetCurrentDirectory(), folderName));
#elif UNITY_ANDROID
            // try get path saved

            var folder = PlayerPrefs.GetString("external_folder_path", null);

            if (folder.IsValid() && CanCreateFolderAndCanWrite(folder))
                return GetPath(folder);

            // try extract from persistent

            var android_idx = Application.persistentDataPath.IndexOf("Android");

            if (android_idx > 1)
            {
                folder = Path.Combine(Application.persistentDataPath.Substring(0, android_idx - 1), folderName);

                if (CanCreateFolderAndCanWrite(folder))
                    return GetPath(folder);
            }

            // try get paths

            string[] potentialDirectoriesFather = new string[]
                  {
                    "/storage/sdcard0",
                    "/storage/emulated/0",
                    "/storage/sdcard1",
                    "/storage",
                    "/sdcard",
                    "/mnt/sdcard",
                  };

            for (int i = 0; i < potentialDirectoriesFather.Length; i++)
            {
                folder = Path.Combine(potentialDirectoriesFather[i], folderName);
                if (CanCreateFolderAndCanWrite(folder))
                    return GetPath(folder);
            }

            return null;
#else
            throw new Exception("[GetExternalFolderPath] not implemented yet: " + Application.platform);
#endif
        }

        public static string GetRuntimeTempFolderPath_Resources(bool withSeparator)
        {
            return GetRuntimeDataFolderPath_Resources(true) + (withSeparator ? (RUNTIME_TMP_FOLDER_NAME + Path.DirectorySeparatorChar) : RUNTIME_TMP_FOLDER_NAME);
        }

        public static void CheckAndCreateDirectory(string path)
        {
            if (Directory.Exists(path))
                return;
            else
                Directory.CreateDirectory(path);
        }

        public static void CopyDirectory(string SourcePath, string DestinationPath)
        {
            //Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
        }

        public static void CopyFile(string filepath, string toFolder, bool overwrite, bool createDirectory)
        {
            if (!Directory.Exists(toFolder))
                CheckAndCreateDirectory(toFolder);

            var des = Path.Combine(toFolder, Path.GetFileName(filepath));
            File.Copy(filepath, des, overwrite);
        }

        public static void CutFile(string filepath, string toFolder, bool overwrite, bool createDirectory)
        {
            CopyFile(filepath, toFolder, overwrite, createDirectory);
            File.Delete(filepath);
        }

        public static void FileDeleteSafe(string filepath)
        {
            try
            {
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
            }
            catch { }
        }

        public static void DirectoryDeleteSafe(string filepath, bool recursive = true)
        {
            if (Directory.Exists(filepath))
                Directory.Delete(filepath, recursive);
        }

        public static bool IsEmptyFoder(string dirPath)
        {
            return Directory.GetFiles(dirPath).IsNullOrEmpty();
        }

        public static bool CanCreateFolderAndCanWrite(string folder_path)
        {
            try
            {
                CheckAndCreateDirectory(folder_path);
                var p = Path.Combine(folder_path, "tmpfile_steverogers");
                File.WriteAllBytes(p, new byte[] { });
                File.Delete(p);
                return true;
            }
            catch //(Exception e)
            {
                return false;
            }
        }

        public static string GetFolderPathNotExist(string path_with_prefix)
        {
            for (int i = 0; ; i++)
            {
                var now = path_with_prefix + i;

                if (!Directory.Exists(now))
                    return now;
            }
        }

        public static bool OpenDirectory(string dirPath)
        {
#if UNITY_EDITOR
            if (Directory.Exists(dirPath))
            {
                var dirs = Directory.GetDirectories(dirPath);

                if (dirs.IsValid())
                {
                    EditorUtility.RevealInFinder(dirs[0]);
                }
                else
                {
                    EditorUtility.RevealInFinder(dirPath);
                }

                return true;
            }
            else
#endif
                return false;
        }

        public static string FixPath(this string s)
        {
#if UNITY_EDITOR || UNITY_ANDROID
            return s;
#else
            return s.Replace('\\', '/');
#endif
        }

        #endregion

        #region Net / Download

        public static IEnumerator DownloadAudioClip_CRT(string url, Action<AudioClip> onDone)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    //Debug.Log(www.error);
                    onDone(null);
                }
                else
                {
                    //if (onDone != null)
                    onDone(DownloadHandlerAudioClip.GetContent(www));
                }
            }
        }

        public static IEnumerator DownloadGoogleSheet_CRT(string id, Action<bool, string> callback, string filepathToSave = null, string sheetId = null)
        {
            string url = "https://docs.google.com/spreadsheets/d/" + id + "/export?format=csv";

            if (!string.IsNullOrEmpty(sheetId))
                url += "&gid=" + sheetId;

            UnityWebRequest download = UnityWebRequest.Get(url);

            yield return download.SendWebRequest();

            if (download.IsNullOrEmpty())
                callback(false, "download fail url: " + url + ", error: " + download.error);
            else
            {
                if (filepathToSave.IsValid())
                    WriteAllText(filepathToSave, download.downloadHandler.text, true);

                callback(true, download.downloadHandler.text);
            }
        }

        public static IEnumerator DownloadFile_CRT(string url, Action<UnityWebRequest> successCallback, Action<string> failCallback)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                if (failCallback != null)
                    failCallback.Invoke(www.error);
            }
            else
            {
                if (successCallback != null)
                    successCallback.Invoke(www);
            }
        }

        public static IEnumerator DownloadTextureCoroutine(string url, Action<Texture2D> callback)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    if (callback != null)
                        callback(DownloadHandlerTexture.GetContent(uwr));
                }
            }
        }

        public static string FilepathToURL(string filepath)
        {
            return string.Format("file://{0}", filepath);
        }

        public static bool IsDownloadFailFailBecauseOf404(Exception e)
        {
            if (e == null)
                return false;

            var s = e.ToString().ToLower();
            return s.Contains("exist") || s.Contains("404") || s.Contains("not found");
        }

        public static bool IsTaskFail<T>(System.Threading.Tasks.Task<T> resultTask)
        {
            return resultTask == null || resultTask.IsFaulted || resultTask.IsCanceled || resultTask.Result == null;
        }

        public static bool IsTaskFail(System.Threading.Tasks.Task resultTask)
        {
            return resultTask == null || resultTask.IsFaulted || resultTask.IsCanceled;
        }

        #endregion

        #region GameObject / Object

        public static string LogGameobject(GameObject go)
        {
            if (!go)
                return "null";
            else
                return string.Format("name: {0}, scale: {1}, rotation: {2}, local pos: {3}, world pos: {4}, active: {5}, active in hier: {6}, root path: {7}",
                                    go.name,
                                    go.transform.localScale,
                                    go.transform.rotation.eulerAngles,
                                    go.transform.localPosition,
                                    go.transform.position,
                                    go.activeSelf,
                                    go.activeInHierarchy,
                                    Utilities.RootNameGo(go, true));
        }

        public static int GetMonoBehavioursWithMissingScriptCount(GameObject go)
        {
            if (go == null)
                return 0;

            int missing_count = 0;
            Component[] components = go.GetComponents<Component>();

            if (components == null)
                return 0;

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    missing_count++;
                }
            }

            return missing_count;
        }

        public static GameObject FindGameobject(string hierarchyPathFromRoot, bool includeIndex = false)
        {
            if (hierarchyPathFromRoot.IsNullOrEmpty())
            {
                throw new Exception("root path null");
            }

            var arr = hierarchyPathFromRoot.Split(new char[] { '<' }, StringSplitOptions.RemoveEmptyEntries);

            if (arr.IsNullOrEmpty())
                throw new Exception("path arr null when split: " + hierarchyPathFromRoot);

            string name = null;

            for (int i = arr.Length - 1; i >= 0; i--)
            {
                name = arr[i].Trim();

                if (!name.IsNullOrEmpty())
                    break;
            }

            var go = GameObject.Find(name);

            if (go == null)
                throw new Exception("cant found go: " + name);

            GameObject found = null;
            var trim = hierarchyPathFromRoot.Trim();

            TraverseDFS(go.transform, (t) =>
            {
                var path = RootNameGo(t.gameObject, includeIndex).Trim();

                if (trim.Equals(path))
                {
                    found = t.gameObject;
                    return false;
                }
                else
                    return true;
            });

            return found;
        }

        public static bool ActiveEventSystem
        {
            get
            {
                return EventSystem.current && EventSystem.current.enabled;
            }

            set
            {
                if (value) // active
                {
                    var sys = _LastEventSystem ?? EventSystem.current;

                    if (sys)
                    {
                        sys.enabled = true;
                        _LastEventSystem = null;
                    }
                    //else
                    //    Debug.LogError("Cant find EventSystem to active!");
                }
                else // disactive
                {
                    var sys = EventSystem.current;

                    if (sys)
                    {
                        sys.enabled = false;
                        _LastEventSystem = sys;
                    }
                    //else
                    //    Debug.LogError("Cant find EventSystem to inactive!");
                }
            }
        }

        public static bool InverseActive(this GameObject go)
        {
            if (go == null)
                return false;

            go.SetActive(!go.activeSelf);
            return go.activeSelf;
        }

        public static void CheckAndActiveGo(this Component component, bool active)
        {
            if (!component)
                return;

            component.gameObject.CheckAndActiveGo(active);
        }

        public static void CheckAndActiveGo(this GameObject go, bool active)
        {
            if (go == null || go.activeSelf == active)
                return;

            go.SetActive(active);
        }

        public static void LogRootNameGo(this GameObject go)
        {
            Debug.Log(go.RootNameGo());
        }

        public static string RootNameGo(this GameObject go, bool includeIndex = true)
        {
            if (go == null)
                return null;

            var cur = go.transform;
            StringBuilder builder = new StringBuilder();

            do
            {
                if (includeIndex)
                    builder.Append(cur.name + $" ({cur.GetSiblingIndex()}) < ");
                else
                    builder.Append(cur.name + " < ");

                cur = cur.parent;
            } while (cur != null);

            return builder.ToString();
        }

        public static RectTransform CreateRectTransformGo(string name, Transform parent)
        {
            if (parent == null)
            {
                Debug.LogError("(CreateRectTransformGo) Parent is null!");
                return null;
            }

            var tf = parent.Find(name);

            if (!tf)
            {
                var go = new GameObject();
                go.AddComponent<RectTransform>();
                var res = GameObject.Instantiate(go, parent).GetComponent<RectTransform>();
                res.name = name;

                if (Application.isPlaying)
                {
                    GameObject.Destroy(go);
                }
                else
                    GameObject.DestroyImmediate(go);

                return res;
            }
            else
                return tf.GetComponent<RectTransform>();
        }

        public static void CopyRectTransform(RectTransform from, RectTransform to) // Main 
        {
            if (from == null)
                throw new Exception("[CopyRectTransform] 'from' rectt is null.");

            if (to == null)
                throw new Exception("[CopyRectTransform] 'to' rectt is null.");

            to.anchorMax = from.anchorMax;
            to.anchorMin = from.anchorMin;
            to.pivot = from.pivot;
            to.localPosition = from.localPosition;
            to.localScale = from.localScale;
            to.rotation = from.rotation;
            to.sizeDelta = from.sizeDelta;
        }

        public static void CopyRectTransform(GameObject from, GameObject to) // Sub 
        {
            if (from == null)
            {
                Debug.LogError("[CopyRectTransform] 'from' go is null.");
                return;
            }

            if (to == null)
            {
                Debug.LogError("[CopyRectTransform] 'to' go is null.");
                return;
            }

            CopyRectTransform(from.GetComponent<RectTransform>(), to.GetComponent<RectTransform>());
        }

        public static float GetLeft(this RectTransform rt)
        {
            return rt.offsetMin.x;
        }

        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static float GetRight(this RectTransform rt)
        {
            return -rt.offsetMax.x;
        }

        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static float GetTop(this RectTransform rt)
        {
            return -rt.offsetMax.y;
        }

        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static float GetBottom(this RectTransform rt)
        {
            return rt.offsetMin.y;
        }

        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }

        public static void SetLocalScaleZero(this GameObject go)
        {
            go.transform.localScale = Vector3.zero;
        }

        public static void SetLocalScaleOne(this GameObject go)
        {
            go.transform.localScale = Vector3.one;
        }

        public static void SetLocalScaleZero(this Component compo)
        {
            compo.transform.localScale = Vector3.zero;
        }

        public static void SetLocalScaleOne(this Component compo)
        {
            compo.transform.localScale = Vector3.one;
        }

        public static void SetLocalScale(this GameObject go, float scale)
        {
            go.transform.localScale = Vector3.one * scale;
        }

        public static void SetLocalScale(this Component comp, float scale)
        {
            comp.transform.localScale = Vector3.one * scale;
        }

        public static void ResetRectTransform(GameObject go)
        {
            var r = go.GetComponent<RectTransform>();
            r.anchorMax = new Vector2(0.5f, 0.5f);
            r.anchorMin = new Vector2(0.5f, 0.5f);
            r.pivot = new Vector2(0.5f, 0.5f);
            r.localPosition = Vector3.zero;
            r.localScale = Vector3.one;
            r.rotation = Quaternion.identity;
            r.sizeDelta = new Vector2(100, 100);
        }

        public static List<GameObject> GetDontDestroyOnLoadObjects()
        {
            List<GameObject> result = new List<GameObject>();

            List<GameObject> rootGameObjectsExceptDontDestroyOnLoad = new List<GameObject>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                rootGameObjectsExceptDontDestroyOnLoad.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
            }

            List<GameObject> rootGameObjects = new List<GameObject>();
            Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
            for (int i = 0; i < allTransforms.Length; i++)
            {
                Transform root = allTransforms[i].root;
                if (root.hideFlags == HideFlags.None && !rootGameObjects.Contains(root.gameObject))
                {
                    rootGameObjects.Add(root.gameObject);
                }
            }

            for (int i = 0; i < rootGameObjects.Count; i++)
            {
                if (!rootGameObjectsExceptDontDestroyOnLoad.Contains(rootGameObjects[i]))
                    result.Add(rootGameObjects[i]);
            }

            return result;
        }

        public static void TraverseDFS(Predicate<Transform> callback)
        {
            var roots = ActiveScene.GetRootGameObjects();

            foreach (var i in roots)
            {
                if (!TraverseDFS(i.transform, callback))
                    break;
            }
        }

        public static bool TraverseDFS(Transform root, Predicate<Transform> callback)
        {
            //’Predicate’ is a C# delegate that accepts one parameter and returns a ‘bool’
            //We can use this ‘bool’ it to check if the user wants to keep searching the tree.
            if (!callback(root))
            {
                //The desired query was found and we can stop searching.
                return false;
            }

            for (int i = 0; i < root.childCount; i++)
            {
                if (!TraverseDFS(root.GetChild(i), callback))
                {
                    //Stop searching
                    return false;
                }
            }

            //Keep searching
            return true;
        }

        public static Vector3 CursorWorldPosOnNCP
        {
            get
            {
                return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            }
        }

        public static Vector3 CameraToCursor // move canvas objects follow pointer use this!
        {
            get
            {
                return CursorWorldPosOnNCP - Camera.main.transform.position;
            }
        }

        public static Vector3 CursorOnTransform(Transform transform)
        {
            Vector3 camToTrans = transform.position - Camera.main.transform.position;
            return Camera.main.transform.position + CameraToCursor * (Vector3.Dot(Camera.main.transform.forward, camToTrans) / Vector3.Dot(Camera.main.transform.forward, CameraToCursor));
        }

        public static void SetOrthographicSizeByWidth(float value)
        {
            Camera.main.orthographicSize = value * Screen.height / Screen.width;
        }

        public static Vector2 GetRectTransformDeltaSizeActualSize(this Component target)
        {
            RectTransform rect = target as RectTransform;

            if (rect == null)
                rect = target.GetComponent<RectTransform>();

            return RectTransformUtility.PixelAdjustRect(rect, rect.GetComponentInParent<Canvas>()).size;
        }

        public static void SetSizeDelta_X(this GameObject target, float value) // Sub 
        {
            target.GetComponent<RectTransform>().SetSizeDelta_X(value);
        }

        public static void SetSizeDelta_Y(this GameObject target, float value) // Sub 
        {
            target.GetComponent<RectTransform>().SetSizeDelta_Y(value);
        }

        public static void SetSizeDelta_X(this Component target, float value) // Main
        {
            RectTransform rect = target as RectTransform;

            if (rect == null)
                rect = target.GetComponent<RectTransform>();

            var ac = GetRectTransformDeltaSizeActualSize(rect);
            var half = (ac.x - value) / 2;

            GetRight(rect).LogGreen(GetLeft(rect));

            SetRight(rect, GetRight(rect) + half);
            SetLeft(rect, GetLeft(rect) + half);
        }

        public static void SetSizeDelta_Y(this Component target, float value) // Main
        {
            RectTransform rect = target as RectTransform;

            if (rect == null)
                rect = target.GetComponent<RectTransform>();

            var ac = GetRectTransformDeltaSizeActualSize(rect);
            var half = (ac.y - value) / 2;
            SetTop(rect, GetTop(rect) + half);
            SetBottom(rect, GetBottom(rect) + half);
        }

        public static void SetSizeDelta(this Component target, Vector2 value) // Main
        {
            SetSizeDelta_X(target, value.x);
            SetSizeDelta_Y(target, value.y);
        }

        public static void SetLocalScale_X(this Component component, float value)
        {
            component.transform.localScale = new Vector3(value, component.transform.localScale.y, component.transform.localScale.z);
        }

        public static void SetLocalScale_Y(this Component component, float value)
        {
            component.transform.localScale = new Vector3(component.transform.localScale.x, value, component.transform.localScale.z);
        }

        public static void SetLocalScale_X(this GameObject go_s, float value)
        {
            go_s.transform.localScale = new Vector3(value, go_s.transform.localScale.y, go_s.transform.localScale.z);
        }

        public static void SetLocalScale_Y(this GameObject go_s, float value)
        {
            go_s.transform.localScale = new Vector3(go_s.transform.localScale.x, value, go_s.transform.localScale.z);
        }

        public static void SetPos_X(this Component component, float value, bool isLocal = true) // Sub 
        {
            component.gameObject.SetPos_X(value, isLocal);
        }

        public static void SetPos_X(this GameObject go_s, float value, bool isLocal = true) // Main 
        {
            if (isLocal)
                go_s.transform.localPosition = new Vector3(value, go_s.transform.localPosition.y, go_s.transform.localPosition.z);
            else
                go_s.transform.position = new Vector3(value, go_s.transform.position.y, go_s.transform.position.z);
        }

        public static void SetPos_Y(this Component component, float value, bool isLocal = true) // Sub 
        {
            component.gameObject.SetPos_Y(value, isLocal);
        }

        public static void SetPos_Y(this GameObject go_s, float value, bool isLocal = true) // Main 
        {
            if (isLocal)
                go_s.transform.localPosition = new Vector3(go_s.transform.localPosition.x, value, go_s.transform.localPosition.z);
            else
                go_s.transform.position = new Vector3(go_s.transform.position.x, value, go_s.transform.position.z);
        }

        public static void SetPos_Z(this GameObject go_s, float value, bool isLocal = true)
        {
            if (isLocal)
                go_s.transform.localPosition = new Vector3(go_s.transform.localPosition.x, go_s.transform.localPosition.y, value);
            else
                go_s.transform.position = new Vector3(go_s.transform.position.x, go_s.transform.position.y, value);
        }

        public static void AddPos_X(this GameObject go_s, float value, bool isLocal = true)
        {
            if (isLocal)
                go_s.SetPos_X(go_s.transform.localPosition.x + value, true);
            else
                go_s.SetPos_X(go_s.transform.position.x + value, false);
        }

        public static void AddPos_Y(this GameObject go_s, float value, bool isLocal = true)
        {
            if (isLocal)
                go_s.SetPos_Y(go_s.transform.localPosition.y + value, true);
            else
                go_s.SetPos_Y(go_s.transform.position.y + value, false);
        }

        public static void AddPos_Z(this GameObject go_s, float value, bool isLocal = true)
        {
            if (isLocal)
                go_s.SetPos_Z(go_s.transform.localPosition.z + value, true);
            else
                go_s.SetPos_Z(go_s.transform.position.z + value, false);
        }

        public static void SetAnchoredPos_X(this Component target, float value)
        {
            RectTransform rect = target as RectTransform;

            if (rect == null)
                rect = target.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(value, rect.anchoredPosition.y);
        }

        public static void SetAnchoredPos_X(this GameObject go, float value)
        {
            if (go == null)
                return;

            var r = go.GetComponent<RectTransform>();

            if (r == null)
                return;

            r.anchoredPosition = new Vector3(value, r.anchoredPosition.y);
        }

        public static void SetAnchoredPos_Y(this GameObject go, float value)
        {
            if (go == null)
                return;

            var r = go.GetComponent<RectTransform>();

            if (r == null)
                return;

            r.anchoredPosition = new Vector3(r.anchoredPosition.x, value);
        }

        public static void SetAnchoredPos_Y(this Component target, float value)
        {
            RectTransform rect = target as RectTransform;

            if (rect == null)
                rect = target.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, value);
        }

        public static float GetAnchoredPos_Y(this GameObject go)
        {
            var r = go.GetComponent<RectTransform>();
            return r.anchoredPosition.y;
        }
        
        public static float GetAnchoredPos_X(this GameObject go)
        {
            var r = go.GetComponent<RectTransform>();
            return r.anchoredPosition.x;
        }

        public static void SeGridLayoutGroupCellSizeX(this Component target, float value)
        {
            GridLayoutGroup compo = target as GridLayoutGroup;

            if (compo == null)
                compo = target.GetComponent<GridLayoutGroup>();

            compo.cellSize = new Vector2(value, compo.cellSize.y);
        }

        public static void SeGridLayoutGroupCellSizeY(this Component target, float value)
        {
            GridLayoutGroup compo = target as GridLayoutGroup;

            if (compo == null)
                compo = target.GetComponent<GridLayoutGroup>();

            compo.cellSize = new Vector2(compo.cellSize.x, value);
        }

        public static void SetWorldScaleByParent(this Transform t)
        {
            t.localScale = new Vector3(t.localScale.x / t.parent.localScale.x, t.localScale.y / t.parent.localScale.y, t.localScale.z / t.parent.localScale.z);
        }

        public static GameObject InstantiateFarFromHome(this GameObject go)
        {
            return GameObject.Instantiate(go, Vector3.one * 100, Quaternion.identity, go.transform.parent);
        }

        public static void Destroy(this UnityEngine.Object item)
        {
            GameObject.Destroy(item);
            item = null;
        }

        public static IEnumerator ArcWorldMove_CRT(Transform go, Vector3 mid, Vector3 end, float speedScale = 1, Action onFinish = null, Action<float> percentCallback = null, float multiSpeedByTime = 1)
        {
            if (go == null || speedScale <= 0)
            {
                Debug.LogError("ArcWorldMove_CRT: " + go + " " + speedScale);
                yield break;
            }

            float count = 0;
            Vector3[] point = new Vector3[] { go.position, mid, end };

            while (count < 1.0f)
            {
                count += Time.deltaTime * speedScale;

                Vector3 m1 = Vector3.Lerp(point[0], point[1], count);
                Vector3 m2 = Vector3.Lerp(point[1], point[2], count);
                go.position = Vector3.Lerp(m1, m2, count);
                percentCallback.SafeCall(count);
                speedScale *= multiSpeedByTime;

                yield return null;
            }

            onFinish.SafeCall();
        }

        public static GameObject LastChild(this GameObject go)
        {
            if (go.transform.childCount == 0)
                return null;

            return go.transform.GetChild(go.transform.childCount - 1).gameObject;
        }

        public static Transform LastChild(this Transform go)
        {
            if (go.transform.childCount == 0)
                return null;

            return go.GetChild(go.childCount - 1);
        }

        public static GameObject FirstChild(this GameObject go)
        {
            if (go.transform.childCount == 0)
                return null;

            return go.transform.GetChild(0).gameObject;
        }

        public static Transform FirstChild(this Transform go)
        {
            if (go.transform.childCount == 0)
                return null;

            return go.GetChild(0);
        }

        public static IEnumerator DestroyAllChilds_CRT(Transform t)
        {
            if (t == null)
                yield break;

            while (t.childCount > 0)
            {
                Destroy(t.GetChild(0).gameObject);
                yield return null;
            }
        }

        #endregion

        #region Profiling

        public static double TotalReservedMemory { get { return Math.Round(UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1048576.0f, 1); } }

        public static double TotalAllocatedMemory { get { return Math.Round(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1048576.0f, 1); } }

        #endregion

        #region Log

        public static void LogDictionary<T, Q>(Dictionary<T, Q> dic, string title = null)
        {
            if (dic.IsNullOrEmpty())
            {
                Debug.LogError("LogDictionary - Dictionary is null!");
                return;
            }

            List<string> lines = new List<string>();

            if (title.IsValid())
                lines.Add(title + "\n");

            foreach (var i in dic)
                lines.Add(i.Key + " - " + i.Value + "\n");

            Debug.Log(CreateLogContent_List(lines, false));
        }

        public static string CreateLogContent_Lines(params object[] items)
        {
            if (items.IsNullOrEmpty())
                return null;

            try
            {
                return string.Join("\n", items);
            }
            catch
            {
                return null;
            }
        }

        public static string CreateLogContent(params object[] items)
        {
            if (items.IsNullOrEmpty())
                return null;

            try
            {
                return string.Join(" | ", items);
            }
            catch
            {
                return null;
            }
        }

        public static string CreateLogContent_List<T>(ICollection<T> items, bool inline = true)
        {
            if (items.IsNullOrEmpty())
                return null;

            StringBuilder builder = new StringBuilder();
            string join = inline ? " | " : "\n";

            foreach (var i in items)
            {
                builder.Append(i == null ? "null" : i.ToString());
                builder.Append(join);
            }

            return builder.ToString();
        }

        public static void LogCondidion(this object msg, params object[] items)
        {
            if (items.IsValid() && !((bool)items[0]))
                return;

            string add = null;
            UnityEngine.Object link = null;

            if (!items.IsNullOrEmpty())
            {
                try
                {
                    var list = items.ToList();
                    list.RemoveAt(0);
                    items = list.ToArray();

                    add = string.Join(" | ", items);

                    foreach (var i in items)
                    {

                        if (i is UnityEngine.Object)
                        {
                            link = i as UnityEngine.Object;
                            break;
                        }
                    }
                }
                catch
                { }
            }


            if (add == null)
                Debug.LogFormat(msg as UnityEngine.Object, "<color=blue>{0}</color>", msg);
            else
                Debug.LogFormat(link, "<color=blue>{0}</color>", string.Join(" | ", msg, add));
        }

        public static void LogConditionPurple(this object msg, params object[] items)
        {
            if (items.IsValid() && !((bool)items[0]))
                return;

            string add = null;
            UnityEngine.Object link = null;

            if (!items.IsNullOrEmpty())
            {
                try
                {
                    var list = items.ToList();
                    list.RemoveAt(0);
                    items = list.ToArray();

                    add = string.Join(" | ", items);

                    foreach (var i in items)
                        if (i is UnityEngine.Object)
                        {
                            link = i as UnityEngine.Object;
                            break;
                        }
                }
                catch
                { }
            }


            if (add == null)
                Debug.LogFormat(msg as UnityEngine.Object, "<color=purple>{0}</color>", msg);
            else
                Debug.LogFormat(link, "<color=purple>{0}</color>", string.Join(" | ", msg, add));
        }

        public static void LogGreen(this object msg, params object[] items)
        {
            string add = null;
            UnityEngine.Object link = null;

            if (!items.IsNullOrEmpty())
            {
                try
                {
                    add = string.Join(" | ", items);

                    foreach (var i in items)
                        if (i is UnityEngine.Object)
                        {
                            link = i as UnityEngine.Object;
                            break;
                        }
                }
                catch
                { }
            }

            if (add == null)
                Debug.LogFormat(msg as UnityEngine.Object, "<color=green>{0}</color>", msg);
            else
                Debug.LogFormat(link, "<color=green>{0}</color>", string.Join(" | ", msg, add));
        }

        public static void LogOrange(this object msg, params object[] items)
        {
            string add = null;
            UnityEngine.Object link = null;

            if (!items.IsNullOrEmpty())
            {
                try
                {
                    add = string.Join(" | ", items);

                    foreach (var i in items)
                        if (i is UnityEngine.Object)
                        {
                            link = i as UnityEngine.Object;
                            break;
                        }
                }
                catch
                { }
            }


            if (add == null)
                Debug.LogFormat(msg as UnityEngine.Object, "<color=orange>{0}</color>", msg);
            else
                Debug.LogFormat(link, "<color=orange>{0}</color>", string.Join(" | ", msg, add));
        }

        public static void LogBlue(this object msg, params object[] items)
        {
            string add = null;
            UnityEngine.Object link = null;

            if (!items.IsNullOrEmpty())
            {
                try
                {
                    add = string.Join(" | ", items);

                    foreach (var i in items)
                        if (i is UnityEngine.Object)
                        {
                            link = i as UnityEngine.Object;
                            break;
                        }
                }
                catch
                { }
            }


            if (add == null)
                Debug.LogFormat(msg as UnityEngine.Object, "<color=blue>{0}</color>", msg);
            else
                Debug.LogFormat(link, "<color=blue>{0}</color>", string.Join(" | ", msg, add));
        }

        public static void Log(this object msg, params object[] items)
        {
            string add = null;
            UnityEngine.Object link = null;

            if (!items.IsNullOrEmpty())
            {
                try
                {
                    add = string.Join(" | ", items);

                    foreach (var i in items)
                        if (i is UnityEngine.Object)
                        {
                            link = i as UnityEngine.Object;
                            break;
                        }
                }
                catch
                { }
            }


            if (add == null)
                Debug.LogFormat(msg as UnityEngine.Object, "<color=blue>{0}</color>", msg);
            else
                Debug.LogFormat(link, "<color=blue>{0}</color>", string.Join(" | ", msg, add));
        }

        public static void LogPurple(this object msg, params object[] items)
        {
            string add = null;
            UnityEngine.Object link = null;

            if (!items.IsNullOrEmpty())
            {
                try
                {
                    add = string.Join(" | ", items);

                    foreach (var i in items)
                        if (i is UnityEngine.Object)
                        {
                            link = i as UnityEngine.Object;
                            break;
                        }
                }
                catch
                { }
            }


            if (add == null)
                Debug.LogFormat(msg as UnityEngine.Object, "<color=purple>{0}</color>", msg);
            else
                Debug.LogFormat(link, "<color=purple>{0}</color>", string.Join(" | ", msg, add));
        }

        public static void LogPink(this object msg, params object[] items)
        {
            string add = null;
            UnityEngine.Object link = null;

            if (!items.IsNullOrEmpty())
            {
                try
                {
                    add = string.Join(" | ", items);

                    foreach (var i in items)
                        if (i is UnityEngine.Object)
                        {
                            link = i as UnityEngine.Object;
                            break;
                        }
                }
                catch
                { }
            }


            if (add == null)
                Debug.LogFormat(msg as UnityEngine.Object, "<color=FUCHSIA>{0}</color>", msg);
            else
                Debug.LogFormat(link, "<color=FUCHSIA>{0}</color>", string.Join(" | ", msg, add));
        }

        public static void LogWarning(this object msg, params object[] items)
        {
            string add = null;
            UnityEngine.Object link = null;

            if (!items.IsNullOrEmpty())
            {
                try
                {
                    add = string.Join(" | ", items);

                    foreach (var i in items)
                        if (i is UnityEngine.Object)
                        {
                            link = i as UnityEngine.Object;
                            break;
                        }
                }
                catch
                { }
            }

            if (add == null)
                Debug.LogWarning(msg, msg as UnityEngine.Object);
            else
                Debug.LogWarning(string.Join(" | ", msg, add), link);
        }

        public static void LogError(this object msg, params object[] items)
        {
            string add = null;
            UnityEngine.Object link = null;

            if (!items.IsNullOrEmpty())
            {
                try
                {
                    add = string.Join(" | ", items);

                    foreach (var i in items)
                        if (i is UnityEngine.Object)
                        {
                            link = i as UnityEngine.Object;
                            break;
                        }
                }
                catch
                { }
            }

            if (add == null)
                Debug.LogError(msg, msg as UnityEngine.Object);
            else
                Debug.LogError(string.Join(" | ", msg, add), link);
        }

        public static string GetLogContentIfOutOfRange<T>(ICollection<T> items, int index)
        {
            if (IsOutOfRange(items, index))
                return $"Out of range! count = { (items == null ? "null" : items.Count.ToString()) }, index = { index }";
            else
                return null;
        }

        public delegate bool CollectionCountDelegate<T>(T i);

        public static int Count<T>(ICollection<T> items, CollectionCountDelegate<T> collectionCountDelegate)
        {
            if (items.IsNullOrEmpty())
                return 0;

            int c = 0;

            foreach (var i in items)
                if (collectionCountDelegate(i))
                    c++;

            return c;
        }

        #endregion

        #region Animation / Effects

        public static IEnumerator Twinkle_CRT(GameObject go, float interval = 0.05f, int times = 5)
        {
            if (go == null || times < 1 || interval <= 0)
                yield break;

            var wait = new WaitForSeconds(interval);

            for (int i = 0; i < times; i++)
            {
                go.SetActive(false);
                yield return wait;
                go.SetActive(true);
                yield return wait;
            }
        }

        public static IEnumerator LerpColors_CRT(Color[] colors, float lerpStep, float stepTime, Action<Color> callback)
        {
            if (colors.IsNullOrEmpty())
                throw new Exception("Colors are null.");

            if (lerpStep <= 0 || lerpStep >= 1)
                throw new Exception("lerpStep is invalid (must > 0 and < 1): " + lerpStep);

            if (stepTime <= 0)
                throw new Exception("stepTime is <= 0: " + lerpStep);

            if (callback == null)
                throw new Exception("Callback is null.");

            int cur = 0;
            float curStep = 0;

            while (true)
            {
                var now = Color.Lerp(colors[cur % colors.Length], colors[(cur + 1) % colors.Length], curStep);
                callback(now);
                curStep = Mathf.Clamp01(curStep + lerpStep);


                if (curStep == 1)
                {
                    cur++;
                    curStep = 0;
                }

                yield return new WaitForSeconds(stepTime);
            }
        }

        /// <summary>
        /// to phải chia hết cho step (nếu muốn dừng)
        /// to số nào cũng được
        /// step dương quay trái, âm quay phải
        /// </summary>
        public static IEnumerator RotateToZ_CRT(Transform transform, int to, int step, Action done)
        {
            var limit = (int)Angle360(to);
            //limit.Log("limit");
            while (Mathf.RoundToInt(transform.rotation.eulerAngles.z) != limit)
            {
                //Mathf.RoundToInt(transform.rotation.eulerAngles.z).Log("now z");
                transform.Rotate(Vector3.forward, step, Space.Self);
                yield return null;
            }

            done.SafeCall();
        }

        public static IEnumerator WriteText_CRT(string content, float stepTime, Action<string> onUpdate)
        {
            if (content.IsNullOrEmpty() || onUpdate == null)
                yield break;

            StringBuilder builder = new StringBuilder(content.Length);

            foreach (var i in content)
            {
                builder.Append(i);
                onUpdate(builder.ToString());

                if (stepTime <= 0)
                    yield return null;
                else
                    yield return new WaitForSeconds(stepTime);
            }

            yield break;
        }

        #endregion

        #region Parse / Convert

        public static IEnumerable<T> GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static List<T> GetEnumList<T>(params T[] exceptValues) where T : Enum
        {
            var enu = Enum.GetValues(typeof(T)).Cast<T>();

            List<T> res = new List<T>();

            foreach (var i in enu)
            {
                var index = -1;

                if (exceptValues.IsValid())
                    index = exceptValues.FindIndex(i);

                if (index == -1)
                    res.Add(i);
            }

            return res;
        }

        public static T GetEnumValueFromPartOfItsString<T>(string partEnum, T defaultValue = default(T), bool ignoreCase = true) where T : Enum
        {
            if (partEnum.IsNullOrEmpty())
                return defaultValue;

            var enumer = GetEnumValues<T>();

            foreach (var i in enumer)
            {
                if (ignoreCase)
                {
                    if (i.ToString().ToLower().Contains(partEnum.ToLower()))
                        return i;
                }
                else // Not ignore case
                {
                    if (i.ToString().Contains(partEnum))
                        return i;
                }

            }

            return defaultValue;
        }

        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            if (System.Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Stream stream = new MemoryStream();

            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static string CapitalizeAllFirstLetters(string s)
        {
            return System.Text.RegularExpressions.Regex.Replace(s, @"(^\w)|(\s\w)", m => m.Value.ToUpper());
        }

        public static string AddSpacesToSentence(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            System.Text.StringBuilder newText = new System.Text.StringBuilder(text.Length * 2);
            newText.Append(text[0]);

            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                {
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    {
                        newText.Append(' ');
                    }
                }

                newText.Append(text[i]);
            }

            return newText.ToString();
        }

        public static string MoneyShorter(int num, int fraction = 0)
        {
            var limit = 1000000000;

            if (num >= limit)
            {
                if (fraction > 0)
                    return string.Format("{0}B", Math.Round(1f * num / limit, fraction));
                else
                    return string.Format("{0}B", (num / limit));
            }

            limit = 1000000;

            if (num >= limit)
            {
                if (fraction > 0)
                    return string.Format("{0}M", Math.Round(1f * num / limit, fraction));
                else
                    return string.Format("{0}M", (num / limit));
            }

            limit = 1000;

            if (num >= limit)
            {
                if (fraction > 0)
                    return string.Format("{0}K", Math.Round(1f * num / limit, fraction));
                else
                    return string.Format("{0}K", (num / limit));
            }
            else
                return num.ToString();
        }

        public static Vector2 Vector3ToVector2(Vector3 value)
        {
            return new Vector2(value.x, value.y);
        }

        public static Vector3 StringToVector3(string sVector)
        {
            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            if (sArray.Length != 3)
                throw new Exception("This is not a Vector3 string: " + sVector);

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }

        public static DateTime Parse(this string value, DateTime defaultValue)
        {
            try
            {
                return DateTime.Parse(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int Parse(this string value, int defaultValue = 0)
        {
            try
            {
                return int.Parse(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static float Parse(this string value, float defaultValue = 0)
        {
            try
            {
                return float.Parse(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        public static bool IsNotFoundIndex(this int value) => value < 0;

        public static bool IsFoundIndex(this int value) => value >= 0;

        public static bool ToBool(this int value)
        {
            return value == 0 ? false : true;
        }

        public static float Angle360(float angle)
        {
            if (angle < 0)
                return (angle % 360) + 360;
            else
                return (angle % 360);
        }

        public static float GetValueFromCSVDictionary(Dictionary<string, List<string>> dic, string key, float defautValue = default, int columeIndex = 0)
        {
            object raw = GetValueFromCSVDictionary(dic, key, (object)defautValue, columeIndex);

            if (float.TryParse(raw.ToString(), out float ress))
                return ress;
            else
            {
                Debug.LogError($"[GetValueFromCSVDictionary] can not cast [{raw}] to " + typeof(float));
                return defautValue;
            }
        }

        public static object GetValueFromCSVDictionary(Dictionary<string, List<string>> dic, string key, object defautValue = default, int columeIndex = 0)
        {
            if (dic == null)
                return defautValue;

            if (dic.TryGetValue(key, out List<string> output))
            {
                if (output == null || output.Count <= columeIndex)
                    return defautValue;
                else
                    return output[columeIndex];
            }
            else
                return defautValue;
        }

        public static Dictionary<string, List<string>> CreateDictionaryFromCSVContentFromFilepathIO(string filepath, bool isComma, bool removeFirstLine) // Sub 
        {
            var c = ReadAllTextSafe(filepath);
            return CreateDictionaryFromCSVContent(ref c, isComma, removeFirstLine);
        }

        public static Dictionary<string, List<string>> CreateDictionaryFromCSVContentFromResourcePath(string filepath, bool isComma, bool removeFirstLine) // Sub 
        {
            var c = ReadAllText_Resources(filepath);
            return CreateDictionaryFromCSVContent(ref c, isComma, removeFirstLine);
        }

        public static Dictionary<string, List<string>> CreateDictionaryFromCSVContent(ref string content, bool isComma, bool removeFirstLine)
        {
            var lists = ReadCSV_FromText_WithSplit(ref content, isComma, removeFirstLine);

            if (lists.IsNullOrEmpty())
                return null;

            Dictionary<string, List<string>> res = new Dictionary<string, List<string>>();

            foreach (var list in lists)
            {
                if (list.IsNullOrEmpty() || list.Count < 2)
                    continue;

                if (res.ContainsKey(list[0]))
                {
                    Debug.LogError("[CreateDictionaryFromCSVContent] already has key: " + list[0]);
                    continue;
                }

                res.Add(list[0], list.GetRange(1, list.Count - 1));
            }

            return res;
        }

        public static void SerializeAndLog(this object item)
        {
            JsonUtility.ToJson(item).Log();
        }

        public static T[] SafeJsonDeserializeArray<T>(string content, string split)
        {
            if (content.IsNullOrEmpty())
                return null;

            var arr = content.Split(new string[] { split }, StringSplitOptions.RemoveEmptyEntries);

            if (arr.IsNullOrEmpty())
                return null;

            T[] res = new T[arr.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                res[i] = SafeJsonDeserialize<T>(arr[i], out _);
            }

            return res;
        }

        public static T SafeJsonDeserializeFromResource<T>(string resourceFilePath) // sub  
        {
            return SafeJsonDeserializeFromResource<T>(resourceFilePath, out _);
        }

        public static T SafeJsonDeserializeFromResource<T>(string resourceFilePath, out Exception exception) // sub  
        {
            var json = ReadAllText_Resources(resourceFilePath);
            return SafeJsonDeserialize<T>(ref json, out exception);
        }

        public static T SafeJsonDeserializeFromFilepath<T>(string path) // sub 
        {
            return SafeJsonDeserializeFromFilepath<T>(path, out _);
        }

        public static T SafeJsonDeserializeFromFilepath<T>(string path, out Exception exception) // sub  
        {
            var json = ReadAllTextSafe(path);
            return SafeJsonDeserialize<T>(ref json, out exception);
        }

        public static T SafeJsonDeserialize<T>(ref string content) // sub  
        {
            return SafeJsonDeserialize<T>(ref content, out Exception _);
        }

        public static T SafeJsonDeserialize<T>(string content) // sub  
        {
            return SafeJsonDeserialize<T>(ref content, out Exception _);
        }

        public static T SafeJsonDeserialize<T>(string content, out Exception exception) // sub  
        {
            return SafeJsonDeserialize<T>(ref content, out exception);
        }

        public static T SafeJsonDeserialize<T>(ref string content, out Exception exception) // main 
        {
            try
            {
                exception = null;
                var res = JsonUtility.FromJson<T>(content);
                return res;
            }
            catch (Exception e)
            {
                exception = e;
                return default;
            }
        }

        #endregion

        #region Miscs

        public static IEnumerator Translate_CRT(string sourceText, Action<bool, string> callback, string targetLang = "en")
        {
            string sourceLang = "auto";

            string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=" + sourceLang + "&tl=" + targetLang + "&dt=t&q=" + UnityWebRequest.EscapeURL(sourceText);

            UnityWebRequest www = UnityWebRequest.Get(url);

            yield return www.SendWebRequest();

            if (www.IsNullOrEmpty())
                callback(false, null);
            else
            {
                var text = www.downloadHandler?.text;

                if (text.IsNullOrEmpty())
                    callback(false, null);
                else
                {
                    for (int i = 4; i < text.Length; i++)
                        if (text[i].Equals('"'))
                        {
                            callback(true, text.Substring(4, i - 4));
                            yield break;
                        }

                    callback(true, text);
                }
            }
        }

        public static bool RandomBool
        {
            get
            {
                UnityEngine.Random.InitState(DateTime.Now.Millisecond);
                return UnityEngine.Random.Range(0, 2) == 0 ? false : true;
            }
        }

        public static bool RandomBoolWithTruePercent(float truePercent)
        {
            return UnityEngine.Random.Range(0f, 1f) > truePercent ? false : true;
        }

        public static float Remap(float value, float inMin, float inMax, float outMin, float outMax, bool shouldClamp = true)
        {
            var res = outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);

            if (shouldClamp)
                return Mathf.Clamp(res, outMin, outMax);
            else
                return res;
        }

        public static Vector2 RandomVector(float maxX, float maxY)
        {
            return RandomVector(-maxX, maxX, -maxY, maxY);
        }

        public static Vector2 RandomVector(float minX, float maxX, float minY, float maxY)
        {
            return new Vector2(
              UnityEngine.Random.Range(minX, maxX),
              UnityEngine.Random.Range(minY, maxY));
        }

        public static Vector2 NextPosition(Vector2 startPos, float distanceMove, Vector2 targetPosition)
        {
            if (targetPosition.Equals(startPos))
                return targetPosition;

            return Vector2.Lerp(startPos, targetPosition, distanceMove / (targetPosition - startPos).magnitude);
        }

        public static Vector3 NextPosition(Vector3 startPos, float distanceMove, Vector3 targetPosition)
        {
            if (targetPosition.Equals(startPos))
                return targetPosition;

            return Vector3.Lerp(startPos, targetPosition, distanceMove / (targetPosition - startPos).magnitude);
        }

        public static bool GetPlayerPrefsBool(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }

        public static void SetPlayerPrefsBool(string key, bool defaultValue)
        {
            PlayerPrefs.SetInt(key, defaultValue ? 1 : 0);
        }

        public static bool WasPrefToday(string key, bool setToday = true)
        {
            var s = PlayerPrefs.GetString(key, string.Empty);

            if (s.Equals(DateTime.Today.ToShortDateString()))
                return true;
            else
            {
                if (setToday)
                    SetPrefToday(key);

                return false;
            }
        }

        public static bool WasPrefToday_Hour(string key, bool setToday = true)
        {
            var s = PlayerPrefs.GetString(key, string.Empty);

            if (s.Equals(string.Format("{0} {1}h", DateTime.Today.ToShortDateString(), DateTime.Now.Hour)))
                return true;
            else
            {
                if (setToday)
                    SetPrefToday_Hour(key);

                return false;
            }
        }

        public static void SetPrefToday(string key)
        {
            PlayerPrefs.SetString(key, DateTime.Today.ToShortDateString());
        }

        public static void SetPrefToday_Hour(string key)
        {
            PlayerPrefs.SetString(key, string.Format("{0} {1}h", DateTime.Today.ToShortDateString(), DateTime.Now.Hour));
        }

        public static int ClipboardInt
        {
            get
            {
                return Clipboard.Parse(0);
            }
        }

        public static float ClipboardFloat
        {
            get
            {
                return Clipboard.Parse(0f);
            }
        }

        public static string Clipboard
        {
            set
            {
#if UNITY_EDITOR
                EditorGUIUtility.systemCopyBuffer = value;
#else
                GUIUtility.systemCopyBuffer = value;
#endif
            }

            get
            {
#if UNITY_EDITOR
                return EditorGUIUtility.systemCopyBuffer;
#else
                return GUIUtility.systemCopyBuffer;
#endif
            }
        }

        public static Scene ActiveScene
        {
            get
            {
                return UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            }
        }

        public static bool IsScene(string name)
        {
            return ActiveScene.name.Equals(name);
        }

        public static void SetPrefabDirty()
        {
#if UNITY_EDITOR
            var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
#endif
        }

        public static void MarkAllScenesDirty()
        {
#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
#endif
        }

        public static string GetVariableName<T>(Expression<Func<T>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }

        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            if (index < 0)
            {
                Debug.LogError("Index is less than zero. Array is not modified");
                return array;
            }

            if (index >= array.Length)
            {
                Debug.LogError("Index exceeds array length. Array is not modified");
                return array;
            }

            T[] newArray = new T[array.Length - 1];
            int index1 = 0;
            for (int index2 = 0; index2 < array.Length; ++index2)
            {
                if (index2 == index) continue;

                newArray[index1] = array[index2];
                ++index1;
            }

            return newArray;
        }

        public static T[] InsertAt<T>(this T[] array, int index)
        {
            if (index < 0)
            {
                Debug.LogError("Index is less than zero. Array is not modified");
                return array;
            }

            if (index > array.Length)
            {
                Debug.LogError("Index exceeds array length. Array is not modified");
                return array;
            }

            T[] newArray = new T[array.Length + 1];
            int index1 = 0;
            for (int index2 = 0; index2 < newArray.Length; ++index2)
            {
                if (index2 == index) continue;

                newArray[index2] = array[index1];
                ++index1;
            }

            return newArray;
        }

        public static int FindIndex<T>(this T[] content, T item)
        {
            if (content == null)
                return -1;

            return Array.FindIndex(content, i => i.Equals(item));
        }

        public static bool CheckAndRequestPermission(string permission)
        {
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(permission))
            {
                Permission.RequestUserPermission(permission);
                return false;
            }
            else
                return true;
#else
            throw new NotImplementedException("CheckAndRequestPermission");
#endif
        }

        public static IEnumerator CheckAndRequestPermissionTillDone_CRT(string permission)
        {
            CheckAndRequestPermission(permission);

            while (!Permission.HasUserAuthorizedPermission(permission))
                yield return null;

            yield break;
        }


        public static T1 TryGetFromDictionary<T, T1>(this Dictionary<T, T1> dic, T key)
        {
            T1 value = default;

            if (dic.TryGetValue(key, out value))
                return value;
            else
                return default;
        }

        public static Vector3 MergeIfHasValue(Vector3 owner, Vector3 value)
        {
            if (value.x != 0)
                owner.x = value.x;

            if (value.y != 0)
                owner.y = value.y;

            if (value.z != 0)
                owner.z = value.z;

            return owner;
        }

        public static void SafeDestroy(GameObject go)
        {
            if (go)
                Destroy(go);
        }

        public static Vector2 MergeIfHasValue(Vector2 owner, Vector2 value)
        {
            if (value.x != 0)
                owner.x = value.x;

            if (value.y != 0)
                owner.y = value.y;

            return owner;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string[] SplitWordsInStringToLines(ref string text, int wordsPerLine, char replacebleChar = '^')
        {
            int count = 0;
            var arr = text.ToCharArray();

            for (int i = 0; i < arr.Length; i++)
            {
                if (char.IsWhiteSpace(arr[i]))
                {
                    count++;

                    if (count == wordsPerLine)
                    {
                        count = 0;
                        arr[i] = replacebleChar;
                    }
                }
            }

            // split!

            var s = new string(arr);
            return s.Split(new char[] { replacebleChar }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static int CountWords(string test)
        {
            return CountWords(ref test);
        }

        public static int CountWords(ref string test)
        {
            int count = 0;
            bool wasInWord = false;
            bool inWord = false;

            for (int i = 0; i < test.Length; i++)
            {
                if (inWord)
                {
                    wasInWord = true;
                }

                if (Char.IsWhiteSpace(test[i]))
                {
                    if (wasInWord)
                    {
                        count++;
                        wasInWord = false;
                    }
                    inWord = false;
                }
                else
                {
                    inWord = true;
                }
            }

            // Check to see if we got out with seeing a word
            if (wasInWord)
            {
                count++;
            }

            return count;
        }

        public static void ClearConsole()
        {
#if UNITY_EDITOR
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
#endif
        }

        public static void Swap<T>(this IList<T> list, int index1, int index2)
        {
            if (list.IsNullOrEmpty() || index1 == index2 || index2 < 0 || index1 < 0 || index1 >= list.Count || index2 >= list.Count)
                return;

            T tmp = list[index1];
            list[index1] = list[index2];
            list[index2] = tmp;
        }

        public static void AddSafe<T>(this List<T> list, T value)
        {
            if (list == null)
                list = new List<T>();

            list.Add(value);
        }

        public static void AddSafe<T>(this HashSet<T> list, T value)
        {
            if (list == null)
                list = new HashSet<T>();

            list.Add(value);
        }

        public static void ClearSafe<T>(this HashSet<T> list)
        {
            if (list != null)
                list.Clear();
        }

        public static string BytesToString(byte[] arr)
        {
            if (arr.IsNullOrEmpty())
                return null;

            return System.Text.Encoding.UTF8.GetString(arr);
        }

        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }

        }

        public static byte[] StringToBytes(string s)
        {
            if (s.IsNullOrEmpty())
                return null;

            return Encoding.UTF8.GetBytes(s);
        }

        public static List<T> ToList<T>(this T[] arr)
        {
            if (arr.IsNullOrEmpty())
                return null;

            return new System.Collections.Generic.List<T>(arr);
        }

        public delegate bool MergeCollectionsIsEqualComparer<T>(T a, T b);

        public static bool IsEqualCollection<T>(this ICollection<T> list, ICollection<T> other, bool ignoreOrder = true) where T : IEquatable<T>
        {
            var listIsEmpty = list.IsNullOrEmpty();
            var otherIsEmpty = other.IsNullOrEmpty();

            if (listIsEmpty && otherIsEmpty)
                return true;
            else if (!listIsEmpty && otherIsEmpty)
                return false;
            else if (listIsEmpty && !otherIsEmpty)
                return false;
            else if (list.Count != other.Count)
                return false;

            if (!ignoreOrder)
            {
                var l = list.GetEnumerator();
                var o = other.GetEnumerator();

                while (l.MoveNext() && o.MoveNext())
                {
                    if (!Equals(l.Current, o.Current))
                        return false;
                }

                return true;
            }
            else // ignore order
            {
                if (list.Except(other).Any())
                    return false;

                if (other.Except(list).Any())
                    return false;

                return true;
            }
        }

        public static T MergeCollections<T, T2>(T source, T target, bool clone = false, MergeCollectionsIsEqualComparer<T2> IsEqualComparer = null) where T : ICollection<T2>
        {
            if (target is Array)
            {
                Debug.LogError("[MergeCollections] Not support array!");
                return default(T);
            }

            if (source.IsNullOrEmpty())
            {
                if (target == null)
                    return default(T);
                else
                {
                    if (clone)
                        return Clone(target);
                    else
                        return target;
                }
            }

            if (target == null)
            {
                if (source == null)
                    return default(T);
                else
                {
                    if (clone)
                        return Clone(source);
                    else
                        return source;
                }
            }

            target = clone ? Clone(target) : target;

            foreach (var i in source)
            {
                var alreadyHave = false;

                foreach (var itarget in target)
                {
                    if (IsEqual(itarget, i))
                    {
                        alreadyHave = true;
                        break;
                    }
                }

                if (alreadyHave)
                    continue;
                else
                    target.Add(i);
            }

            return target;

            bool IsEqual(T2 a, T2 b)
            {
                if (IsEqualComparer == null)
                    return Equals(a, b);
                else
                    return IsEqualComparer(a, b);
            }
        }

        public static void RotateList<T>(this IList<T> list, int times)
        {
            if (times <= 0)
                return;

            for (int i = 0; i < times; i++)
            {
                list.RotateList();
            }
        }

        public static void RotateList<T>(this IList<T> list)
        {
            if (list.IsNullOrEmpty())
                return;

            var last = list.Last();

            for (int i = list.Count - 1; i > 0; i--)
            {
                list.Swap(i, i - 1);
            }

            list[0] = last;
        }

        public static void ShuffleList<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;

            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static IEnumerator ShuffleList_CRT<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;

            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;

                yield return null;
            }
        }

        public static float Average(this IList<int> list)
        {
            if (list.IsNullOrEmpty())
                throw new Exception("Average - List is null / empty!");

            float sum = 0;

            foreach (var i in list)
                sum += i;

            return sum / list.Count;
        }

        public static float Sum(this IList<int> list)
        {
            if (list.IsNullOrEmpty())
                throw new Exception("Sum(...) - List is null / empty!");

            float sum = 0;

            foreach (var i in list)
                sum += i;

            return sum;
        }
        
        public static float[] BytesToFloats(byte[] array)
        {
            float[] floatArr = new float[array.Length / 4];

            for (int i = 0; i < floatArr.Length; i++)
            {
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(array, i * 4, 4);

                floatArr[i] = BitConverter.ToSingle(array, i * 4);
            }
            return floatArr;
        }

        public static void WarningDone(string title = null)
        {
            Debug.LogFormat("<color=green><b>Done {0}!</b></color>", title == null ? "" : title);
        }

        public static float ScreenRatio(bool ignorePortrait)
        {
            if (ignorePortrait)
            {
                int w = Math.Max(Screen.width, Screen.height);
                int h = Math.Min(Screen.width, Screen.height);

                return w * 1f / h;
            }
            else
                return Screen.width * 1f / Screen.height;
        }

        public static Vector2 ScreenSizeInEditorOrInReal
        {
            get
            {
#if UNITY_EDITOR
                var view = EditorGameViewSize;
                float width = view.x;
                float height = view.y;
#else
                float width = Screen.width;
                float height = Screen.height;
#endif

                return new Vector2(width, height);
            }
        }

        public static float ScreenRatioInEditorOrInReal(bool maxDevideMin_Or_WidthDevideHeight)
        {
            var screenSize = ScreenSizeInEditorOrInReal;

            if (maxDevideMin_Or_WidthDevideHeight)
            {
                float max = Math.Max(screenSize.x, screenSize.y);
                float min = Math.Min(screenSize.x, screenSize.y);
                return max / min;
            }
            else
                return screenSize.x / screenSize.y;
        }

        public static bool IsDebugDevice
        {
            get
            {
                return Array.FindIndex(DEBUG_DEVICES, i => i.Equals(SystemInfo.deviceUniqueIdentifier)) > -1;
            }
        }

        public static KeyValuePair<T, T1> GetFirstItemOfDictionaryNotChecking<T, T1>(this Dictionary<T, T1> item)
        {
            var e = item.GetEnumerator();
            e.MoveNext();
            return e.Current;
        }

        public static bool IsTrue(this BOOL_EXTRA value)
        {
            return value == BOOL_EXTRA.TRUE;
        }

        public static bool IsNotUseless(this BOOL_EXTRA value)
        {
            return value != BOOL_EXTRA.USELESS;
        }

        public static List<T> GetElementsOfPage<T>(List<T> list, int pageIndex, int maxElementPerPage)
        {
            if (list.IsNullOrEmpty())
                return null;

            if (pageIndex < 0 || maxElementPerPage < 1)
            {
                Debug.LogError(CreateLogContent("Params invalid!", list, pageIndex, maxElementPerPage));
                return null;
            }

            var startIdx = maxElementPerPage * pageIndex;

            if (startIdx >= list.Count) // over page
            {
                return null;
            }

            var elementNumberOfThisPage = Mathf.Min(maxElementPerPage, list.Count - startIdx);

            return list.GetRange(startIdx, elementNumberOfThisPage);
        }

        public static Vector3 ZeroZ(Vector3 value, int z = 0)
        {
            value.z = z;
            return value;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    sb.Append(c);
            }

            return sb.ToString();
        }

        public static char[] GetSplitCharArray(char c)
        {
            return new char[] { c };
        }

        public static void SafeInvokeMethod<T>(string methodName)
        {
            try
            {
                var thisType = typeof(T);
                var theMethod = thisType.GetMethod(methodName);
                theMethod.Invoke(null, null);
            }
            catch
            { }
        }

        public static string BreakStackTrace(int start = 2, int count = -1)
        {
            if (start < 2)
                start = 2;

            var lines = Environment.StackTrace?.Split(SPLIT_CHAR_NEW_LINE);

            if (lines.IsNullOrEmpty())
                return "null stack trace";

            if (count < 1)
                count = lines.Length;

            StringBuilder builder = new StringBuilder(Math.Min(count, lines.Length));
            int c = 0;

            for (int i = 0; i < lines.Length && c < count; i++)
            {
                if (i < start)
                    continue;

                var stack = lines[i].Substring(5, lines[i].IndexOf('(') - 6);
                var arr = stack.Split(GetSplitCharArray('.'));

                if (arr.Length == 2)
                    builder.Append(string.Format("{0}.{1}, ", arr[0], arr[1]));
                else if (arr.Length == 3)
                    builder.Append(string.Format("{0}.{1}, ", arr[1], arr[2]));
                else
                    builder.Append(string.Format("{0}, ", stack));

                c++;
            }

            return builder.ToString();
        }

        #endregion

        #region Actions / Callbacks

        public static void SafeCall<T>(this Action<T> item, T arg)
        {
            if (item != null)
                item(arg);
        }

        public static void SafeCall<A, B>(this Action<A, B> item, A a, B b)
        {
            if (item != null)
                item(a, b);
        }

        public static void SafeCall(this Action item)
        {
            if (item != null)
                item();
        }

        #endregion

        #region Quick Check Input

        public static bool IsPressed_Space
        {
            get
            {
                return Input.GetKeyDown(KeyCode.Space);
            }
        }

        public static bool IsPressed_Return
        {
            get
            {
                return Input.GetKeyDown(KeyCode.Return);
            }
        }

        public static bool IsPressed_Num(int i)
        {
            return Input.GetKeyDown((KeyCode)(256 + i)) || Input.GetKeyDown((KeyCode)(48 + i));
        }

        public static int IsPressed_Num()
        {
            for (int i = 0; i < 10; i++)
                if (IsPressed_Num(i))
                    return i;

            return -1;
        }

        public static bool IsPressed_Escape
        {
            get
            {
                return Input.GetKeyDown(KeyCode.Escape);
            }
        }

        #endregion

        #region Validate

        public static bool IsOutOfRange<T>(ICollection<T> items, int index)
        {
            return items == null || index < 0 || index >= items.Count;
        }

        public static T Last<T>(this IList<T> list)
        {
            if (list.IsNullOrEmpty())
                return default;

            return list[list.Count - 1];
        }

        public static T GetLastIfOverRange<T>(this T[] items, int index)
        {
            if (items == null || items.Length == 0)
                return default;

            if (index < 0 || index >= items.Length)
            {
                return items[items.Length - 1];
            }

            return items[index];
        }

        public static T GetLastIfOverRange<T>(this List<T> items, int index)
        {
            if (items == null || items.Count == 0)
                return default;

            if (index < 0 || index >= items.Count)
                return items[items.Count - 1];

            return items[index];
        }

        public static T Get<T>(this T[] items, int index, T defaultValue = default(T))
        {
            if (items == null)
                return defaultValue;

            if (index < 0 || index >= items.Length)
                return defaultValue;

            return items[index];
        }

        public static T Get<T>(this List<T> items, int index, T defaultValue = default(T))
        {
            if (items == null)
                return defaultValue;

            if (index < 0 || index >= items.Count)
                return defaultValue;

            return items[index];
        }

        public static int SafeCount<T>(this ICollection<T> item)
        {
            return item == null ? 0 : item.Count;
        }

        public static bool IsNullOrEmpty<T>(this T[] item)
        {
            return item == null || item.Length == 0;
        }

        public static bool IsNullOrEmpty(this string item)
        {
            return item == null || item.Length == 0;
        }

        public static bool TrimIsNullOrEmpty(this string item)
        {
            return item == null || item.Trim().IsNullOrEmpty();
        }

        public static bool IsNullOrEmpty(this UnityWebRequest item)
        {
            return item == null || !item.isDone || !string.IsNullOrEmpty(item.error) || string.IsNullOrEmpty(item.downloadHandler?.text);
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> item)
        {
            return item == null || item.Count == 0;
        }

        public static bool IsValid<T>(this T[] item)
        {
            return item != null && item.Length > 0;
        }

        public static bool IsValid(this string item)
        {
            return item != null && item.Length > 0;
        }

        public static bool SafeContains<T>(this ICollection<T> item, T value)
        {
            return item != null && item.Contains(value);
        }

        public static bool IsValid<T>(this ICollection<T> item)
        {
            return item != null && item.Count > 0;
        }

        public static bool HasAnyValue(this in Vector3 value)
        {
            return (value.x != 0 || value.y != 0 || value.z != 0);
        }

        public static bool HasAnyValue(this in Vector2 value)
        {
            return (value.x != 0 || value.y != 0);
        }

        #endregion

        #region Editor

        public static UnityEngine.Object GetAsset(string assetpath)
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath(assetpath, typeof(UnityEngine.Object));
#else
            return null;
#endif
        }

        public static void SelectAsset(string assetpath)
        {
#if UNITY_EDITOR
            UnityEngine.Object obj = GetAsset(assetpath);

            if (obj == null)
            {
                Debug.LogError("SelectAsset, not found asset: " + assetpath);
                return;
            }

            Selection.activeObject = obj;
#endif
        }

#if UNITY_EDITOR
        public static void AddDefineSymbol(BuildTargetGroup target, string symbol)
        {

            var s = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

            if (s.Contains(symbol))
                return;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", s, symbol));
        }

        public static Vector2 EditorGameViewSize
        {
            get
            {
                System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
                System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
                return (Vector2)Res;
            }
        }
#endif

        #endregion

        #region Encrypt / Decrypt

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        private const string PASS_ENCRYPT = "a!b@c#";

        public static string Encrypt(string plainText, string passWord = null)
        {
            if (plainText == null)
                return null;

            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passWord.IsNullOrEmpty() ? PASS_ENCRYPT : passWord, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passWord = null)
        {
            if (cipherText == null)
                return null;

            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passWord.IsNullOrEmpty() ? PASS_ENCRYPT : passWord, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        #endregion
    }
}