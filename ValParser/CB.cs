using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace ValParser
{
    class CB
    {
        public DataTable Parse(string url, string elementType, string savePath)
        {
            var logger = MainWindow.logger;

            try
            {
                DataTable table = new DataTable();
                XmlDocument doc = new XmlDocument();

                logger.Info($"Trying parse {url} with element type: " + elementType.ToUpper());
                doc.Load(url);//Загрузка и парсинг данных из url

                string VoR;

                switch (elementType)
                {
                    case "Metall":
                        VoR = "Record";
                        break;

                    case "ValCurs":
                        VoR = "Valute";
                        break;

                    default:
                        VoR = "Valute";
                        break;
                }

                //Добавление столбцов в таблицу в зависимости от типа загружаемых данных
                if (elementType == "ValCurs")
                    table.Columns.AddRange(new DataColumn[] { 
                        new DataColumn("ID"),
                        new DataColumn("ЧислКод"),
                        new DataColumn("БуквКод"),
                        new DataColumn("Номинал"),
                        new DataColumn("Наименование"),
                        new DataColumn("Курс") });
                else
                    table.Columns.AddRange(new DataColumn[] { 
                        new DataColumn("Дата"),
                        new DataColumn("Покупка"),
                        new DataColumn("Продажа"),
                        new DataColumn("Код") });

                if (!doc.GetElementsByTagName(elementType)[0].InnerText.Contains("Error in parameters")) //Проверка на неправильно указааные параметры запроса
                {
                    Workbook wbToStream = new Workbook();
                    Worksheet sheet = wbToStream.Worksheets[0];

                    string cursDate = doc.GetElementsByTagName(elementType)[0].Attributes[0].Value.ToString();
                    XmlNodeList item = doc.GetElementsByTagName(VoR);


                    for (int i = 0; i < item.Count; i++) //Проход по строкам
                    {
                        DataRow dataRow = table.NewRow();

                        sheet.Range[i + 1, 1].Text = item[i].Attributes[0].Value.ToString(); //Добавление первого значения в строку excel
                        dataRow[0] = item[i].Attributes[0].Value.ToString(); //Добавление первого значения в строку

                        for (int j = 0; j < item[i].ChildNodes.Count; j++) //Добавление последующих значений в excel и строку
                        {
                            sheet.Range[i + 1, j + 2].Text = item[i].ChildNodes[j].InnerText;
                            dataRow[j+1] = item[i].ChildNodes[j].InnerText;
                        }

                        if (elementType == "Metall")
                        {   //Добавление кода металла, если выбранны таковые
                            sheet.Range[i + 1, item[i].ChildNodes.Count + 2].Text = item[i].Attributes[1].Value.ToString();
                            dataRow[item[i].ChildNodes.Count+1] = item[i].Attributes[1].Value.ToString();
                        }

                        table.Rows.Add(dataRow); //Добавление готовой строки в таблицу
                    }
                    //Запись excel файла
                    FileStream file_stream = new FileStream(savePath+ "\\" + elementType.ToUpper() + cursDate + ".xls", FileMode.OpenOrCreate);
                    wbToStream.SaveToStream(file_stream);
                    file_stream.Close();
                    logger.Info("Parsing done, excel file writed");

                    return table;
                }
                else
                {
                    MessageBox.Show(" Error in parameters ");
                    logger.Warn("Parsing aborted with Errors in parametrs");

                    return table;
                }
            }
            catch (System.Net.WebException webExcp)
            {
                WebExceptionStatus status = webExcp.Status;
                if (status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)webExcp.Response;
                    MessageBox.Show("Server status " + ((int)httpResponse.StatusCode).ToString());
                    logger.Warn("Parsing aborted with Server error code " + ((int)httpResponse.StatusCode).ToString());

                    return new DataTable();
                }
            }
            catch (System.Xml.XmlException xmlExcp)
            {
                MessageBox.Show("File Not Found");
                logger.Warn("Parsing aborted with Xml exception, probably file not gound");

                return new DataTable();

            }
            catch (System.IO.IOException io)
            {
                if (io.Message.Contains("Процесс не может получить доступ к файлу"))
                {
                    MessageBox.Show("Процесс не может получить доступ к файлу, он открыт или используется другим процессом");
                    logger.Warn("Parsing ended, but excel file not writed, file is busy or in use by another process");

                    return new DataTable();

                }
                return new DataTable();

            }
            logger.Error("Parsing has not started, unknown error");

            return new DataTable();
        }
    }
}
