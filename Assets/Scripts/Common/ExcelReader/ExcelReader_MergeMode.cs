using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using OfficeOpenXml;
using UnityEngine;


/// <summary>
/// 읽고, 쓰는 부분
/// 다 했는데 에러처리나 null값에 대한 것은 신경을 안썼음
/// </summary>
public partial class ExcelReader_MergeMode : MonoBehaviour
{
    public static string m_ExcelPath = @"Assets/SaveData/ExcelFiles/MergeMode/";
    public static string m_ScriptPath = @"Assets/Scripts/Common/ExcelReader/TableScript/";
    public static string m_TablePath = @"Assets/Resources/Tables/";

    public static List<ExcelWorksheet> m_Worksheets = new List<ExcelWorksheet>();
    public static Dictionary<ExcelWorksheet, Dictionary<string, List<string>>> m_TableInfos =
        new Dictionary<ExcelWorksheet, Dictionary<string, List<string>>>();
    public static Dictionary<ExcelWorksheet, string> m_BinaryFolders =
        new Dictionary<ExcelWorksheet, string>();

    public static string m_FileName;

    public static void SetTables(string fileName)
    {
        if(!File.Exists(m_ExcelPath + fileName + ".xlsx"))
        {
            Debug.Log("File is not exist.");
            return;
        }

        m_FileName = fileName;
        RemoveOldData();

        SetWorkSheets();

        foreach (var worksheet in m_Worksheets)
        {
            m_TableInfos.Add(worksheet, ExcelToBinary(worksheet));
        }

        CreateTableScript();
    }
    

    /// <summary>
    /// 기존 데이터들을 지우는 함수, 주의 요함!
    /// </summary>
    public static void RemoveOldData()
    {
        m_Worksheets.Clear();
        m_TableInfos.Clear();
        m_BinaryFolders.Clear();
    }

    /// <summary>
    /// Excel 파일에서 Worksheet를 추출하는 함수
    /// </summary>
    /// <returns></returns>
    public static void SetWorkSheets()
    {
        // 경로 내의 엑셀 파일 '이름'들
        List<string> excelFiles = new List<string>(Directory.GetFiles(m_ExcelPath, m_FileName + ".xlsx"));
        // 각 엑셀 파일들
        List<ExcelPackage> excelPackages = new List<ExcelPackage>();
        // 각 엑셀 내의 Worksheet
        List<ExcelWorksheet> collectWorksheets = new List<ExcelWorksheet>();


        foreach (var excelFile in excelFiles)
        {
            FileInfo newFile = new FileInfo(excelFile);

            ExcelPackage excelPackage = new ExcelPackage(newFile);
            ExcelWorksheets worksheets = excelPackage.Workbook.Worksheets;

            foreach (var worksheet in worksheets)
            {
                // 없는 worksheet 추가
                if (!collectWorksheets.Contains(worksheet))
                {
                    collectWorksheets.Add(worksheet);
                    m_BinaryFolders.Add(worksheet, Path.GetFileNameWithoutExtension(newFile.Name));
                }
            }
        }

        collectWorksheets = collectWorksheets.OrderBy(x => x.Name.ToLower()).ToList();

        m_Worksheets = collectWorksheets;
    }


    /// <summary>
    /// 각 Worksheet를 BinaryFile로 변환하는 함수
    /// </summary>
    /// <param name="worksheet"></param>
    public static Dictionary<string, List<string>> ExcelToBinary(ExcelWorksheet worksheet)
    {
        Dictionary<string, List<string>> infos = new Dictionary<string, List<string>>();

        // Table 각 column의 Type List 생성
        // ex) string int int float ~~~
        List<string> types = new List<string>();
        for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
        {
            types.Add(worksheet.Cells[1, col].GetValue<string>());
        }
        infos.Add("type", types);

        // Table 각 column의 Name List 생성
        // ex) Key name1 name2 name3 ~~~~
        List<string> names = new List<string>();
        for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
        {
            names.Add(worksheet.Cells[2, col].GetValue<string>());
        }
        infos.Add("name", names);

        MemoryStream memoryStream = new MemoryStream();
        BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

        // Table 내의 Tuple의 갯수
        binaryWriter.Write(worksheet.Dimension.End.Row - 2);

        // Table 각 row의 Tuple List 생성
        for (int row = worksheet.Dimension.Start.Row + 2; row <= worksheet.Dimension.End.Row; row++)
        {
            for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
            {
                string value = worksheet.Cells[row, col].GetValue<string>();
                if (value == null)
                {
                    switch (types[col - 1])
                    {
                        case "int":
                            binaryWriter.Write(1);
                            break;
                        case "float":
                            binaryWriter.Write(1.0f);
                            break;
                        case "bool":
                            binaryWriter.Write(false);
                            break;
                        case "string":
                            binaryWriter.Write(string.Empty);
                            break;
                        case "byte":
                            binaryWriter.Write(1);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (types[col - 1])
                    {
                        case "int":
                            binaryWriter.Write(int.Parse(value));
                            break;
                        case "float":
                            binaryWriter.Write(float.Parse(value));
                            break;
                        case "bool":
                            binaryWriter.Write(bool.Parse(value));
                            break;
                        case "string":
                            binaryWriter.Write(value);
                            break;
                        case "byte":
                            binaryWriter.Write(byte.Parse(value));
                            break;
                        default:
                            break;
                    }
                }
                
            }
        }

        // 생성된 Tuple List를 각 Name.bytes 파일로 저장
        byte[] binaryTable = memoryStream.ToArray();

        CreateNewFolder(m_TablePath);
        CreateNewFolder(m_TablePath + m_BinaryFolders[worksheet]);

        FileStream fileStream = new FileStream(m_TablePath + m_BinaryFolders[worksheet] + "/"
                                            + worksheet.Name + ".bytes", FileMode.Create);
        fileStream.Write(binaryTable, 0, binaryTable.Length);
        fileStream.Close();
        binaryWriter.Close();

        return infos;
    }

    /// <summary>
    /// 폴더가 없으면 만드는 함수
    /// </summary>
    /// <param name="path"></param>
    public static void CreateNewFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            DirectoryInfo info = new DirectoryInfo(path);
            info.Create();
        }
    }


    /// <summary>
    /// BinaryFile을 읽기 위한 Class를 만드는 함수
    /// </summary>
    public static void CreateTableScript()
    {
        string tempString = string.Empty;
        StringBuilder stringBuilder = new StringBuilder();

        // using부분
        stringBuilder.AppendLine(AddTab(@"using System.IO;"));
        stringBuilder.AppendLine(AddTab(@"using System.Linq;"));
        stringBuilder.AppendLine(AddTab(@"using System.Collections.Generic;"));
        stringBuilder.AppendLine(AddTab(@"using UnityEngine;"));
        stringBuilder.AppendLine();

        KeyValuePair<ExcelWorksheet, Dictionary<string,List<string>>> sampleInfo = m_TableInfos.First();
        
        // 각 worksheet의 Info, Table Class 만드는 곳
        //----------------- Info Class ---------------
        {
            stringBuilder.AppendLine(AddTab("public class " + m_FileName + "Info"));
            stringBuilder.AppendLine(AddTab("{"));

            // name으로 변수들 생성
            // ex) public int m_Name { get; private set; }
            for (int i = 1; i < sampleInfo.Value["name"].Count; i++)
            {
                stringBuilder.AppendLine(AddTab("public " + sampleInfo.Value["type"][i] + " m_"
                                                        + sampleInfo.Value["name"][i] + " { get; private set; }", 1));
            }

            stringBuilder.AppendLine();

            // 각 변수들의 set함수 생성
            // ex) public void SetName(int name) { m_Name = name; }
            for (int i = 1; i < sampleInfo.Value["name"].Count; i++)
            {
                stringBuilder.AppendLine(AddTab("public void Set" + sampleInfo.Value["name"][i]
                    + "(" + sampleInfo.Value["type"][i] + " " + sampleInfo.Value["name"][i] + ")"
                    + " { m_" + sampleInfo.Value["name"][i] + " = " + sampleInfo.Value["name"][i] + "; }", 1));
            }


            stringBuilder.AppendLine(AddTab("}"));
            stringBuilder.AppendLine();
        }
        //----------------- Table Class ---------------
        {

            stringBuilder.AppendLine(AddTab("public class " + m_FileName + "Table : MonoSingleton<" + m_FileName + "Table>"));
            stringBuilder.AppendLine(AddTab("{"));

            // private static Dictionary<type, NameInfo> Table = new Dictionary<type, NameInfo>();
            {
                stringBuilder.AppendLine(AddTab(@"private Dictionary<string, Dictionary<" + sampleInfo.Value["type"][0] + ", " + m_FileName + "Info>> " + "Tables"
                                               + @" = new Dictionary<string, Dictionary<" + sampleInfo.Value["type"][0] + ", " + m_FileName + "Info>>();", 1));
                stringBuilder.AppendLine();
            }

            // protected override void Init()
            {
                stringBuilder.AppendLine(AddTab(@"protected override void Init() ", 1));
                stringBuilder.AppendLine(AddTab("{", 1));
                stringBuilder.AppendLine(AddTab("DontDestroyOnLoad(gameObject);", 2));
                stringBuilder.AppendLine(AddTab("}", 1));
                stringBuilder.AppendLine();
            }

            // private void Start()
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(AddTab(@"private void Start() ", 1));
                stringBuilder.AppendLine(AddTab("{", 1));
                stringBuilder.AppendLine(AddTab(@"ReadBinaryTable();", 2));
                stringBuilder.AppendLine(AddTab("}", 1));
            }
            
            // private void ReadBinaryTable()
            {
                string sheetNames = "";
                foreach(var sheet in m_Worksheets)
                {
                    sheetNames += "\"" + sheet.Name + "\"";

                    if (sheet != m_Worksheets.Last())
                    {
                        sheetNames += ", ";
                    }
                }

                stringBuilder.AppendLine(AddTab(@"private void ReadBinaryTable()", 1));
                stringBuilder.AppendLine(AddTab("{", 1));
                stringBuilder.AppendLine(AddTab("string[] resourceNames = new string[] {" + sheetNames + "};", 2));

                stringBuilder.AppendLine(AddTab("foreach(var name in resourceNames)", 2));
                stringBuilder.AppendLine(AddTab("{", 2));


                stringBuilder.AppendLine(AddTab("TextAsset textAsset = Resources.Load(\"Tables/" + m_FileName + "/\" + name) as TextAsset;", 3));
                stringBuilder.AppendLine(AddTab("MemoryStream memoryStream = new MemoryStream(textAsset.bytes);", 3));
                stringBuilder.AppendLine(AddTab("BinaryReader binaryReader = new BinaryReader(memoryStream);", 3));
                stringBuilder.AppendLine(AddTab("Dictionary<" + sampleInfo.Value["type"][0] + ", " + m_FileName + "Info> table = new Dictionary<" + sampleInfo.Value["type"][0] + ", " + m_FileName + "Info>();", 3));
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(AddTab("int tupleCount = binaryReader.ReadInt32();", 3));
                stringBuilder.AppendLine();
                

                // for(int i = 1; i < tupleCount; i++)
                {
                    stringBuilder.AppendLine(AddTab("for( int i = 0; i < tupleCount; i++)", 3));
                    stringBuilder.AppendLine(AddTab("{", 3));

                    stringBuilder.AppendLine(AddTab(m_FileName + "Info info = new " + m_FileName +"Info();", 4));

                    // Key값 read
                    switch (sampleInfo.Value["type"][0])
                    {
                        case "int":
                            stringBuilder.AppendLine(AddTab(sampleInfo.Value["type"][0] + " key = binaryReader.ReadInt32();", 4));
                            break;
                        case "string":
                            stringBuilder.AppendLine(AddTab(sampleInfo.Value["type"][0] + " key = binaryReader.ReadString();", 4));
                            break;
                        case "float":
                            stringBuilder.AppendLine(AddTab(sampleInfo.Value["type"][0] + " key = binaryReader.ReadSingle();", 4));
                            break;
                        case "bool":
                            stringBuilder.AppendLine(AddTab(sampleInfo.Value["type"][0] + " key = binaryReader.ReadBoolean();", 4));
                            break;
                        case "byte":
                            stringBuilder.AppendLine(AddTab(sampleInfo.Value["type"][0] + " key = binaryReader.ReadByte();", 4));
                            break;
                        default:
                            break;
                    }

                    // 각 Attribute값 read
                    for (int i = 1; i < sampleInfo.Value["type"].Count; i++)
                    {
                        switch (sampleInfo.Value["type"][i])
                        {
                            case "int":
                                stringBuilder.AppendLine(AddTab(@"info.Set" + sampleInfo.Value["name"][i]
                                                                + "(binaryReader.ReadInt32());", 4));
                                break;
                            case "string":
                                stringBuilder.AppendLine(AddTab(@"info.Set" + sampleInfo.Value["name"][i]
                                                                + "(binaryReader.ReadString());", 4));
                                break;
                            case "float":
                                stringBuilder.AppendLine(AddTab(@"info.Set" + sampleInfo.Value["name"][i]
                                                                + "(binaryReader.ReadSingle());", 4));
                                break;
                            case "bool":
                                stringBuilder.AppendLine(AddTab(@"info.Set" + sampleInfo.Value["name"][i]
                                                                + "(binaryReader.ReadBoolean());", 4));
                                break;
                            case "byte":
                                stringBuilder.AppendLine(AddTab(@"info.Set" + sampleInfo.Value["name"][i]
                                                                + "(binaryReader.ReadByte());", 4));
                                break;
                            default:
                                //null값을 읽어야 하나요??
                                break;
                        }
                    }

                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(AddTab(@"table.Add(key, info);", 4));
                    stringBuilder.AppendLine(AddTab("}", 3));
                    stringBuilder.AppendLine(AddTab(@"Tables.Add(name, table);", 3));
                    stringBuilder.AppendLine(AddTab("}", 2));  // foreach
                    stringBuilder.AppendLine(AddTab("}", 1));  // readbinarytable
                    stringBuilder.AppendLine();
                }
            }

            // public static Dictionary<type, NameInfo> GetTable(int Key)
            {
                stringBuilder.AppendLine(AddTab(@"public Dictionary<" + sampleInfo.Value["type"][0] + ", " + m_FileName + "Info> "
                                                + "GetTable(string sheetName)", 1));
                stringBuilder.AppendLine(AddTab("{", 1));
                stringBuilder.AppendLine(AddTab(@"return Tables[sheetName];", 2));
                stringBuilder.AppendLine(AddTab("}", 1));
                stringBuilder.AppendLine();
            }

            // public static NameInfo GetTuple(int Key)
            {
                stringBuilder.AppendLine(AddTab(@"public " + m_FileName + "Info "
                                                + @"GetTuple(string sheetName, " + sampleInfo.Value["type"][0] + " key)", 1));
                stringBuilder.AppendLine(AddTab("{", 1));
                stringBuilder.AppendLine(AddTab(m_FileName + "Info value;", 2));
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(AddTab(@"if (Tables[sheetName].TryGetValue(key, out value))", 2));
                stringBuilder.AppendLine(AddTab(@"return value;", 3));
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(AddTab(@"return null;", 2));
                stringBuilder.AppendLine(AddTab("}", 1));
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine(AddTab("}"));
        }

        CreateNewFolder(m_ScriptPath);

        string path = m_ScriptPath + m_FileName + "Table" + ".cs";

        FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        StreamWriter streamWriter = new StreamWriter(fileStream);
        streamWriter.WriteLine(stringBuilder.ToString());
        streamWriter.Close();
        fileStream.Close();
    }


    /// <summary>
    /// 문자열에 맞게 tab을 주는 함수
    /// </summary>
    /// <param name="str"></param>
    /// <param name="tabCount"></param>
    /// <returns></returns>
    public static string AddTab(string code, int tabCount = 0)
    {
        int tab = 4;
        string tabCode = code;

        if (tabCount > 0)
            tabCode = code.PadLeft(tabCode.Length + tab * tabCount);

        return tabCode;
    }
}
