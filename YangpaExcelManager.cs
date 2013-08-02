using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.IO;

namespace YangpaH
{
    class YangpaExcelManager
    {
        //indexer for person with same name
        static int snc = 1;

        static string fileName = Path.Combine(
              Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "output.xlsx");

        private static Application application;

        public static string SaveExcelDatas(List<SInstance> instances, List<string> students)
        {
            try
            {
                fileName = fileName.Replace("output.x", "output." + ValidateFilepath(instances[0].Class) + ".x");
                Dictionary<string, int> sdic = new Dictionary<string, int>();
                try
                {
                    foreach (string s in students)
                    {
                        sdic.Add(s, 0);
                    }
                }
                catch (ArgumentException)
                {
                    return YangpaConstants.MSG_BUG_NO_DUP_NAME;
                }
                foreach (SInstance inst in instances)
                {
                    for (int i = 0; i < inst.JoMember.Length; i++)
                    {
                        int score = Int32.Parse(inst.JoScoreToActualScore(i));
                        foreach (string s in inst.JoMember[i])
                        {
                            sdic[s] += score;
                        }
                    }
                }
                /*string block = "";
                foreach (string s in students)
                {
                    block += s + ":" + sdic[s] + "\n";
                }*/
                Workbook workbook = null;
                Worksheet worksheet = null;
                try
                {
                    application = new Application();
                    workbook = application.Workbooks.Add();
                    worksheet = workbook.Worksheets[1] as
                        Microsoft.Office.Interop.Excel.Worksheet;
                    for (int i = 1; i < 32; i++)
                    {
                        if ((i - 1) % 5 == 0)
                            worksheet.Cells[1, i + 1] = i - 1;

                        worksheet.Cells[1, i].Interior.Color = ColorTranslator.ToOle(Color.FromRgb(34, 118, 4));
                        worksheet.Cells[1, i].Font.Color = ColorTranslator.ToOle(Color.FromRgb(255, 255, 255));
                    }
                    for (int i = 0; i < sdic.Count; i++)
                    {
                        worksheet.Cells[(i + 2), 1] = students[i];
                        worksheet.Cells[(i + 2), 2] = sdic[students[i]] + "점";
                        if (i % 2 == 1)
                        {
                            //홀짝 구분해서 구별 쉽게 명암 넣어주기
                            Range name = (Range)worksheet.Cells[(i + 2), 1];
                            Range score = (Range)worksheet.Cells[(i + 2), 2];
                            var gray = ColorTranslator.ToOle(Color.FromRgb(230, 230, 230));
                            name.Interior.Color = gray;
                            score.Interior.Color = gray;
                        }
                        for (int j = 0; j < sdic[students[i]]; j++)
                        {
                            Range c = (Range)worksheet.Cells[(i + 2), (j + 3)];
                            c.Interior.Color = ColorTranslator.ToOle(i % 2 == 1 ? Color.FromRgb(230, 230, 0) : Color.FromRgb(255, 255, 0));
                        }
                    }

                    // Save.
                    workbook.SaveAs(fileName);
                    workbook.Close();
                }
                catch (COMException e)
                {
                    return e.Message;
                }
                catch (NullReferenceException e)
                {
                    return e.Message;
                }
                catch (KeyNotFoundException e)
                {
                    return YangpaConstants.MSG_DB_CLS_MISMATCH;
                }
                finally
                {
                    // Clean up
                    ReleaseExcelObject(workbook);
                    ReleaseExcelObject(worksheet);
                    ReleaseExcelObject(application);
                }
                return YangpaConstants.MSG_EXC_EXP_SUCCESS;
            }
            catch (Exception ee)
            {
                return ee.ToString();
            }
        }

        private static string ValidateFilepath(string p)
        {
            return p.Replace(":", "").Replace("\\", "").Replace("<", "").Replace(">", "").Replace("/", "").Replace("?", "").Replace("|", "").Replace("*", "");
        }

        private static void ReleaseExcelObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }
    }

    static class ColorTranslator
    {
        const int RedShift = 0;
        const int GreenShift = 8;
        const int BlueShift = 16;
        /// <summary> 
        /// Translates an Ole color value to a System.Media.Color for WPF usage 
        /// </summary> 
        /// <param name="oleColor">Ole int32 color value</param> 
        /// <returns>System.Media.Color color value</returns> 
        public static Color FromOle(this int oleColor)
        {
            return Color.FromRgb(
                (byte)((oleColor >> RedShift) & 0xFF),
                (byte)((oleColor >> GreenShift) & 0xFF),
                (byte)((oleColor >> BlueShift) & 0xFF)
                );
        }

        /// <summary> 
        /// Translates the specified System.Media.Color to an Ole color. 
        /// </summary> 
        /// <param name="wpfColor">System.Media.Color source value</param> 
        /// <returns>Ole int32 color value</returns> 
        public static int ToOle(Color wpfColor)
        {
            return wpfColor.R << RedShift | wpfColor.G << GreenShift | wpfColor.B << BlueShift;
        }
    }
}
